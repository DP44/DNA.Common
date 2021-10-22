using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace DNA.Net.Lidgren
{
	public class NetPeer
	{
		private List<byte[]> m_storagePool;

		private NetQueue<NetOutgoingMessage> m_outgoingMessagesPool;

		private NetQueue<NetIncomingMessage> m_incomingMessagesPool;

		internal int m_storagePoolBytes;

		private int m_lastUsedFragmentGroup;

		private Dictionary<NetConnection, Dictionary<int, ReceivedFragmentGroup>> m_receivedFragmentGroups;

		private NetPeerStatus m_status;

		private Thread m_networkThread;

		private Socket m_socket;

		internal byte[] m_sendBuffer;

		internal byte[] m_receiveBuffer;

		internal NetIncomingMessage m_readHelperMessage;

		private EndPoint m_senderRemote;

		private object m_initializeLock = new object();

		private uint m_frameCounter;

		private double m_lastHeartbeat;

		private NetUPnP m_upnp;

		internal readonly NetPeerConfiguration m_configuration;

		private readonly NetQueue<NetIncomingMessage> m_releasedIncomingMessages;

		internal readonly NetQueue<NetTuple<IPEndPoint, NetOutgoingMessage>> m_unsentUnconnectedMessages;

		internal Dictionary<IPEndPoint, NetConnection> m_handshakes;

		internal readonly NetPeerStatistics m_statistics;

		internal long m_uniqueIdentifier;

		internal bool m_executeFlushSendQueue;

		private AutoResetEvent m_messageReceivedEvent;

		private List<NetTuple<SynchronizationContext, SendOrPostCallback>> m_receiveCallbacks;

		private static int s_initializedPeersCount;

		private int m_listenPort;

		private object m_tag;

		internal readonly List<NetConnection> m_connections;

		private readonly Dictionary<IPEndPoint, NetConnection> m_connectionLookup;

		private string m_shutdownReason;

		private void InitializePools()
		{
			if (this.m_configuration.UseMessageRecycling)
			{
				this.m_storagePool = new List<byte[]>(16);
				this.m_outgoingMessagesPool = new NetQueue<NetOutgoingMessage>(4);
				this.m_incomingMessagesPool = new NetQueue<NetIncomingMessage>(4);
				return;
			}
			this.m_storagePool = null;
			this.m_outgoingMessagesPool = null;
			this.m_incomingMessagesPool = null;
		}

		internal byte[] GetStorage(int minimumCapacityInBytes)
		{
			if (this.m_storagePool == null)
			{
				return new byte[minimumCapacityInBytes];
			}
			lock (this.m_storagePool)
			{
				for (int i = 0; i < this.m_storagePool.Count; i++)
				{
					byte[] array = this.m_storagePool[i];
					if (array != null && array.Length >= minimumCapacityInBytes)
					{
						this.m_storagePool[i] = null;
						this.m_storagePoolBytes -= array.Length;
						return array;
					}
				}
			}
			this.m_statistics.m_bytesAllocated += (long)minimumCapacityInBytes;
			return new byte[minimumCapacityInBytes];
		}

		internal void Recycle(byte[] storage)
		{
			if (this.m_storagePool == null)
			{
				return;
			}
			lock (this.m_storagePool)
			{
				this.m_storagePoolBytes += storage.Length;
				int count = this.m_storagePool.Count;
				for (int i = 0; i < count; i++)
				{
					if (this.m_storagePool[i] == null)
					{
						this.m_storagePool[i] = storage;
						return;
					}
				}
				this.m_storagePool.Add(storage);
			}
		}

		public NetOutgoingMessage CreateMessage()
		{
			return this.CreateMessage(this.m_configuration.m_defaultOutgoingMessageCapacity);
		}

		public NetOutgoingMessage CreateMessage(string content)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(content);
			NetOutgoingMessage netOutgoingMessage = this.CreateMessage(2 + bytes.Length);
			netOutgoingMessage.WriteVariableUInt32((uint)bytes.Length);
			netOutgoingMessage.Write(bytes);
			return netOutgoingMessage;
		}

		public NetOutgoingMessage CreateMessage(int initialCapacity)
		{
			NetOutgoingMessage netOutgoingMessage;
			if (this.m_outgoingMessagesPool == null || !this.m_outgoingMessagesPool.TryDequeue(out netOutgoingMessage))
			{
				netOutgoingMessage = new NetOutgoingMessage();
			}
			byte[] storage = this.GetStorage(initialCapacity);
			netOutgoingMessage.m_data = storage;
			return netOutgoingMessage;
		}

		internal NetIncomingMessage CreateIncomingMessage(NetIncomingMessageType tp, byte[] useStorageData)
		{
			NetIncomingMessage netIncomingMessage;
			if (this.m_incomingMessagesPool == null || !this.m_incomingMessagesPool.TryDequeue(out netIncomingMessage))
			{
				netIncomingMessage = new NetIncomingMessage(tp);
			}
			else
			{
				netIncomingMessage.m_incomingMessageType = tp;
			}
			netIncomingMessage.m_data = useStorageData;
			return netIncomingMessage;
		}

		internal NetIncomingMessage CreateIncomingMessage(NetIncomingMessageType tp, int minimumByteSize)
		{
			NetIncomingMessage netIncomingMessage;
			if (this.m_incomingMessagesPool == null || !this.m_incomingMessagesPool.TryDequeue(out netIncomingMessage))
			{
				netIncomingMessage = new NetIncomingMessage(tp);
			}
			else
			{
				netIncomingMessage.m_incomingMessageType = tp;
			}
			netIncomingMessage.m_data = this.GetStorage(minimumByteSize);
			return netIncomingMessage;
		}

		public void Recycle(NetIncomingMessage msg)
		{
			if (this.m_incomingMessagesPool == null)
			{
				return;
			}
			byte[] data = msg.m_data;
			msg.m_data = null;
			this.Recycle(data);
			msg.Reset();
			this.m_incomingMessagesPool.Enqueue(msg);
		}

		public void Recycle(IEnumerable<NetIncomingMessage> toRecycle)
		{
			if (this.m_incomingMessagesPool == null)
			{
				return;
			}
			if (this.m_storagePool != null)
			{
				lock (this.m_storagePool)
				{
					foreach (NetIncomingMessage netIncomingMessage in toRecycle)
					{
						byte[] data = netIncomingMessage.m_data;
						netIncomingMessage.m_data = null;
						this.m_storagePoolBytes += data.Length;
						int count = this.m_storagePool.Count;
						for (int i = 0; i < count; i++)
						{
							if (this.m_storagePool[i] == null)
							{
								this.m_storagePool[i] = data;
								return;
							}
						}
						netIncomingMessage.Reset();
						this.m_storagePool.Add(data);
					}
				}
			}
			this.m_incomingMessagesPool.Enqueue(toRecycle);
		}

		internal void Recycle(NetOutgoingMessage msg)
		{
			if (this.m_outgoingMessagesPool == null)
			{
				return;
			}
			byte[] data = msg.m_data;
			msg.m_data = null;
			if (msg.m_fragmentGroup == 0)
			{
				this.Recycle(data);
			}
			msg.Reset();
			this.m_outgoingMessagesPool.Enqueue(msg);
		}

		internal NetIncomingMessage CreateIncomingMessage(NetIncomingMessageType tp, string text)
		{
			NetIncomingMessage netIncomingMessage;
			if (string.IsNullOrEmpty(text))
			{
				netIncomingMessage = this.CreateIncomingMessage(tp, 1);
				netIncomingMessage.Write(string.Empty);
				return netIncomingMessage;
			}
			int byteCount = Encoding.UTF8.GetByteCount(text);
			netIncomingMessage = this.CreateIncomingMessage(tp, byteCount + ((byteCount > 127) ? 2 : 1));
			netIncomingMessage.Write(text);
			return netIncomingMessage;
		}

		private void SendFragmentedMessage(NetOutgoingMessage msg, IList<NetConnection> recipients, NetDeliveryMethod method, int sequenceChannel)
		{
			int num = Interlocked.Increment(ref this.m_lastUsedFragmentGroup);
			if (num >= 65534)
			{
				this.m_lastUsedFragmentGroup = 1;
				num = 1;
			}
			msg.m_fragmentGroup = num;
			int lengthBytes = msg.LengthBytes;
			int mtu = this.GetMTU(recipients);
			int bestChunkSize = NetFragmentationHelper.GetBestChunkSize(num, lengthBytes, mtu);
			int num2 = lengthBytes / bestChunkSize;
			if (num2 * bestChunkSize < lengthBytes)
			{
				num2++;
			}
			int num3 = bestChunkSize * 8;
			int num4 = msg.LengthBits;
			for (int i = 0; i < num2; i++)
			{
				NetOutgoingMessage netOutgoingMessage = this.CreateMessage(mtu);
				netOutgoingMessage.m_bitLength = ((num4 > num3) ? num3 : num4);
				netOutgoingMessage.m_data = msg.m_data;
				netOutgoingMessage.m_fragmentGroup = num;
				netOutgoingMessage.m_fragmentGroupTotalBits = lengthBytes * 8;
				netOutgoingMessage.m_fragmentChunkByteSize = bestChunkSize;
				netOutgoingMessage.m_fragmentChunkNumber = i;
				Interlocked.Add(ref netOutgoingMessage.m_recyclingCount, recipients.Count);
				foreach (NetConnection netConnection in recipients)
				{
					netConnection.EnqueueMessage(netOutgoingMessage, method, sequenceChannel);
				}
				num4 -= num3;
			}
		}

		private void HandleReleasedFragment(NetIncomingMessage im)
		{
			int key;
			int num2;
			int num3;
			int num4;
			int num = NetFragmentationHelper.ReadHeader(im.m_data, 0, out key, out num2, out num3, out num4);
			int num5 = NetUtility.BytesToHoldBits(num2);
			int num6 = num5 / num3;
			if (num6 * num3 < num5)
			{
				num6++;
			}
			if (num4 >= num6)
			{
				this.LogWarning(string.Concat(new object[]
				{
					"Index out of bounds for chunk ",
					num4,
					" (total chunks ",
					num6,
					")"
				}));
				return;
			}
			Dictionary<int, ReceivedFragmentGroup> dictionary;
			if (!this.m_receivedFragmentGroups.TryGetValue(im.SenderConnection, out dictionary))
			{
				dictionary = new Dictionary<int, ReceivedFragmentGroup>();
				this.m_receivedFragmentGroups[im.SenderConnection] = dictionary;
			}
			ReceivedFragmentGroup receivedFragmentGroup;
			if (!dictionary.TryGetValue(key, out receivedFragmentGroup))
			{
				receivedFragmentGroup = new ReceivedFragmentGroup();
				receivedFragmentGroup.Data = new byte[num5];
				receivedFragmentGroup.ReceivedChunks = new NetBitVector(num6);
				dictionary[key] = receivedFragmentGroup;
			}
			receivedFragmentGroup.ReceivedChunks[num4] = true;
			receivedFragmentGroup.LastReceived = (float)NetTime.Now;
			int dstOffset = num4 * num3;
			Buffer.BlockCopy(im.m_data, num, receivedFragmentGroup.Data, dstOffset, im.LengthBytes - num);
			receivedFragmentGroup.ReceivedChunks.Count();
			if (receivedFragmentGroup.ReceivedChunks.Count() == num6)
			{
				im.m_data = receivedFragmentGroup.Data;
				im.m_bitLength = num2;
				im.m_isFragment = false;
				dictionary.Remove(key);
				this.ReleaseMessage(im);
				return;
			}
			this.Recycle(im);
		}

		public void Introduce(IPEndPoint hostInternal, IPEndPoint hostExternal, IPEndPoint clientInternal, IPEndPoint clientExternal, string token)
		{
			NetOutgoingMessage netOutgoingMessage = this.CreateMessage(10 + token.Length + 1);
			netOutgoingMessage.m_messageType = NetMessageType.NatIntroduction;
			netOutgoingMessage.Write(0);
			netOutgoingMessage.Write(hostInternal);
			netOutgoingMessage.Write(hostExternal);
			netOutgoingMessage.Write(token);
			this.m_unsentUnconnectedMessages.Enqueue(new NetTuple<IPEndPoint, NetOutgoingMessage>(clientExternal, netOutgoingMessage));
			netOutgoingMessage = this.CreateMessage(10 + token.Length + 1);
			netOutgoingMessage.m_messageType = NetMessageType.NatIntroduction;
			netOutgoingMessage.Write(1);
			netOutgoingMessage.Write(clientInternal);
			netOutgoingMessage.Write(clientExternal);
			netOutgoingMessage.Write(token);
			this.m_unsentUnconnectedMessages.Enqueue(new NetTuple<IPEndPoint, NetOutgoingMessage>(hostExternal, netOutgoingMessage));
		}

		internal void HandleNatIntroduction(int ptr)
		{
			NetIncomingMessage netIncomingMessage = this.SetupReadHelperMessage(ptr, 1000);
			byte b = netIncomingMessage.ReadByte();
			IPEndPoint item = netIncomingMessage.ReadIPEndPoint();
			IPEndPoint item2 = netIncomingMessage.ReadIPEndPoint();
			string source = netIncomingMessage.ReadString();
			if (b == 0 && !this.m_configuration.IsMessageTypeEnabled(NetIncomingMessageType.NatIntroductionSuccess))
			{
				return;
			}
			NetOutgoingMessage netOutgoingMessage = this.CreateMessage(1);
			netOutgoingMessage.m_messageType = NetMessageType.NatPunchMessage;
			netOutgoingMessage.Write(b);
			netOutgoingMessage.Write(source);
			this.m_unsentUnconnectedMessages.Enqueue(new NetTuple<IPEndPoint, NetOutgoingMessage>(item, netOutgoingMessage));
			netOutgoingMessage = this.CreateMessage(1);
			netOutgoingMessage.m_messageType = NetMessageType.NatPunchMessage;
			netOutgoingMessage.Write(b);
			netOutgoingMessage.Write(source);
			this.m_unsentUnconnectedMessages.Enqueue(new NetTuple<IPEndPoint, NetOutgoingMessage>(item2, netOutgoingMessage));
		}

		private void HandleNatPunch(int ptr, IPEndPoint senderEndPoint)
		{
			NetIncomingMessage netIncomingMessage = this.SetupReadHelperMessage(ptr, 1000);
			if (netIncomingMessage.ReadByte() == 0)
			{
				return;
			}
			string source = netIncomingMessage.ReadString();
			NetIncomingMessage netIncomingMessage2 = this.CreateIncomingMessage(NetIncomingMessageType.NatIntroductionSuccess, 10);
			netIncomingMessage2.m_senderEndPoint = senderEndPoint;
			netIncomingMessage2.Write(source);
			this.ReleaseMessage(netIncomingMessage2);
			NetOutgoingMessage netOutgoingMessage = this.CreateMessage(1);
			netOutgoingMessage.m_messageType = NetMessageType.NatPunchMessage;
			netOutgoingMessage.Write(0);
			netOutgoingMessage.Write(source);
			this.m_unsentUnconnectedMessages.Enqueue(new NetTuple<IPEndPoint, NetOutgoingMessage>(senderEndPoint, netOutgoingMessage));
		}

		public Socket Socket
		{
			get
			{
				return this.m_socket;
			}
		}

		public void RegisterReceivedCallback(SendOrPostCallback callback)
		{
			if (SynchronizationContext.Current == null)
			{
				throw new NetException("Need a SynchronizationContext to register callback on correct thread!");
			}
			if (this.m_receiveCallbacks == null)
			{
				this.m_receiveCallbacks = new List<NetTuple<SynchronizationContext, SendOrPostCallback>>();
			}
			this.m_receiveCallbacks.Add(new NetTuple<SynchronizationContext, SendOrPostCallback>(SynchronizationContext.Current, callback));
		}

		public void UnregisterReceivedCallback(SendOrPostCallback callback)
		{
			if (this.m_receiveCallbacks == null)
			{
				return;
			}
			this.m_receiveCallbacks.Remove(new NetTuple<SynchronizationContext, SendOrPostCallback>(SynchronizationContext.Current, callback));
			if (this.m_receiveCallbacks.Count < 1)
			{
				this.m_receiveCallbacks = null;
			}
		}

		internal void ReleaseMessage(NetIncomingMessage msg)
		{
			if (msg.m_isFragment)
			{
				this.HandleReleasedFragment(msg);
				return;
			}
			this.m_releasedIncomingMessages.Enqueue(msg);
			if (this.m_messageReceivedEvent != null)
			{
				this.m_messageReceivedEvent.Set();
			}
			if (this.m_receiveCallbacks != null)
			{
				foreach (NetTuple<SynchronizationContext, SendOrPostCallback> netTuple in this.m_receiveCallbacks)
				{
					netTuple.Item1.Post(netTuple.Item2, this);
				}
			}
		}

		private void InitializeNetwork()
		{
			lock (this.m_initializeLock)
			{
				this.m_configuration.Lock();
				if (this.m_status != NetPeerStatus.Running)
				{
					if (this.m_configuration.m_enableUPnP)
					{
						this.m_upnp = new NetUPnP(this);
					}
					this.InitializePools();
					this.m_releasedIncomingMessages.Clear();
					this.m_unsentUnconnectedMessages.Clear();
					this.m_handshakes.Clear();
					IPEndPoint ipendPoint = new IPEndPoint(this.m_configuration.LocalAddress, this.m_configuration.Port);
					EndPoint localEP = ipendPoint;
					this.m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
					this.m_socket.ReceiveBufferSize = this.m_configuration.ReceiveBufferSize;
					this.m_socket.SendBufferSize = this.m_configuration.SendBufferSize;
					this.m_socket.Blocking = false;
					this.m_socket.Bind(localEP);
					try
					{
						uint ioControlCode = 2550136844U;
						this.m_socket.IOControl((int)ioControlCode, new byte[]
						{
							Convert.ToByte(false)
						}, null);
					}
					catch
					{
					}
					IPEndPoint ipendPoint2 = this.m_socket.LocalEndPoint as IPEndPoint;
					this.m_listenPort = ipendPoint2.Port;
					this.m_receiveBuffer = new byte[this.m_configuration.ReceiveBufferSize];
					this.m_sendBuffer = new byte[this.m_configuration.SendBufferSize];
					this.m_readHelperMessage = new NetIncomingMessage(NetIncomingMessageType.Error);
					this.m_readHelperMessage.m_data = this.m_receiveBuffer;
					byte[] array = new byte[8];
					NetRandom.Instance.NextBytes(array);
					try
					{
						PhysicalAddress macAddress = NetUtility.GetMacAddress();
						if (macAddress != null)
						{
							array = macAddress.GetAddressBytes();
						}
						else
						{
							this.LogWarning("Failed to get Mac address");
						}
					}
					catch (NotSupportedException)
					{
					}
					byte[] bytes = BitConverter.GetBytes(ipendPoint2.GetHashCode());
					byte[] array2 = new byte[bytes.Length + array.Length];
					Array.Copy(bytes, 0, array2, 0, bytes.Length);
					Array.Copy(array, 0, array2, bytes.Length, array.Length);
					this.m_uniqueIdentifier = BitConverter.ToInt64(SHA1.Create().ComputeHash(array2), 0);
					this.m_status = NetPeerStatus.Running;
				}
			}
		}

		private void NetworkLoop()
		{
			do
			{
				try
				{
					this.Heartbeat();
				}
				catch (Exception ex)
				{
					this.LogWarning(ex.ToString());
				}
			}
			while (this.m_status == NetPeerStatus.Running);
			this.ExecutePeerShutdown();
		}

		private void ExecutePeerShutdown()
		{
			List<NetConnection> list = new List<NetConnection>(this.m_handshakes.Count + this.m_connections.Count);
			lock (this.m_connections)
			{
				foreach (NetConnection netConnection in this.m_connections)
				{
					if (netConnection != null)
					{
						list.Add(netConnection);
					}
				}
				lock (this.m_handshakes)
				{
					foreach (NetConnection netConnection2 in this.m_handshakes.Values)
					{
						if (netConnection2 != null)
						{
							list.Add(netConnection2);
						}
					}
					foreach (NetConnection netConnection3 in list)
					{
						netConnection3.Shutdown(this.m_shutdownReason);
					}
				}
			}
			this.FlushDelayedPackets();
			this.Heartbeat();
			Thread.Sleep(10);
			lock (this.m_initializeLock)
			{
				try
				{
					if (this.m_socket != null)
					{
						try
						{
							this.m_socket.Shutdown(SocketShutdown.Receive);
						}
						catch
						{
						}
						this.m_socket.Close(2);
					}
					if (this.m_messageReceivedEvent != null)
					{
						this.m_messageReceivedEvent.Set();
						this.m_messageReceivedEvent.Close();
						this.m_messageReceivedEvent = null;
					}
				}
				finally
				{
					this.m_socket = null;
					this.m_status = NetPeerStatus.NotRunning;
				}
				this.m_receiveBuffer = null;
				this.m_sendBuffer = null;
				this.m_unsentUnconnectedMessages.Clear();
				this.m_connections.Clear();
				this.m_connectionLookup.Clear();
				this.m_handshakes.Clear();
			}
		}

		private void Heartbeat()
		{
			double now = NetTime.Now;
			float num = (float)now;
			double num2 = now - this.m_lastHeartbeat;
			int num3 = 1250 - this.m_connections.Count;
			if (num3 < 250)
			{
				num3 = 250;
			}
			if (num2 > 1.0 / (double)num3 || num2 < 0.0)
			{
				this.m_frameCounter += 1U;
				this.m_lastHeartbeat = now;
				if (this.m_frameCounter % 3U == 0U)
				{
					foreach (KeyValuePair<IPEndPoint, NetConnection> keyValuePair in this.m_handshakes)
					{
						NetConnection value = keyValuePair.Value;
						value.UnconnectedHeartbeat(num);
						if (value.m_status == NetConnectionStatus.Connected || value.m_status == NetConnectionStatus.Disconnected)
						{
							break;
						}
					}
				}
				if (this.m_configuration.m_autoFlushSendQueue)
				{
					this.m_executeFlushSendQueue = true;
				}
				lock (this.m_connections)
				{
					foreach (NetConnection netConnection in this.m_connections)
					{
						netConnection.Heartbeat(num, this.m_frameCounter);
						if (netConnection.m_status == NetConnectionStatus.Disconnected)
						{
							this.m_connections.Remove(netConnection);
							this.m_connectionLookup.Remove(netConnection.RemoteEndPoint);
							break;
						}
					}
				}
				this.m_executeFlushSendQueue = false;
				NetTuple<IPEndPoint, NetOutgoingMessage> netTuple;
				while (this.m_unsentUnconnectedMessages.TryDequeue(out netTuple))
				{
					NetOutgoingMessage item = netTuple.Item2;
					int numBytes = item.Encode(this.m_sendBuffer, 0, 0);
					bool flag2;
					this.SendPacket(numBytes, netTuple.Item1, 1, out flag2);
					Interlocked.Decrement(ref item.m_recyclingCount);
					if (item.m_recyclingCount <= 0)
					{
						this.Recycle(item);
					}
				}
			}
			if (this.m_socket == null)
			{
				return;
			}
			if (!this.m_socket.Poll(1000, SelectMode.SelectRead))
			{
				return;
			}
			now = NetTime.Now;
			num = (float)now;
			int num4;
			int num6;
			int num8;
			for (;;)
			{
				num4 = 0;
				try
				{
					num4 = this.m_socket.ReceiveFrom(this.m_receiveBuffer, 0, this.m_receiveBuffer.Length, SocketFlags.None, ref this.m_senderRemote);
				}
				catch (SocketException ex)
				{
					if (ex.SocketErrorCode == SocketError.ConnectionReset)
					{
						this.LogWarning("ConnectionReset");
						break;
					}
					this.LogWarning(ex.ToString());
					break;
				}
				if (num4 < 5)
				{
					break;
				}
				IPEndPoint ipendPoint = (IPEndPoint)this.m_senderRemote;
				if (this.m_upnp != null && num < this.m_upnp.m_discoveryResponseDeadline)
				{
					try
					{
						string text = Encoding.ASCII.GetString(this.m_receiveBuffer, 0, num4);
						if (text.Contains("upnp:rootdevice") || text.Contains("UPnP/1.0"))
						{
							text = text.Substring(text.ToLower().IndexOf("location:") + 9);
							text = text.Substring(0, text.IndexOf("\r")).Trim();
							this.m_upnp.ExtractServiceUrl(text);
							break;
						}
					}
					catch
					{
					}
				}
				NetConnection netConnection2 = null;
				this.m_connectionLookup.TryGetValue(ipendPoint, out netConnection2);
				int num5 = 0;
				num6 = 0;
				while (num4 - num6 >= 5)
				{
					num5++;
					NetMessageType netMessageType = (NetMessageType)this.m_receiveBuffer[num6++];
					byte b = this.m_receiveBuffer[num6++];
					byte b2 = this.m_receiveBuffer[num6++];
					bool isFragment = (b & 1) == 1;
					ushort sequenceNumber = (ushort)(b >> 1 | (int)b2 << 7);
					ushort num7 = (ushort)((int)this.m_receiveBuffer[num6++] | (int)this.m_receiveBuffer[num6++] << 8);
					num8 = NetUtility.BytesToHoldBits((int)num7);
					if (num4 - num6 < num8)
					{
						goto Block_13;
					}
					try
					{
						if (netMessageType >= NetMessageType.LibraryError)
						{
							if (netConnection2 != null)
							{
								netConnection2.ReceivedLibraryMessage(netMessageType, num6, num8);
							}
							else
							{
								this.ReceivedUnconnectedLibraryMessage(now, ipendPoint, netMessageType, num6, num8);
							}
						}
						else
						{
							if (netConnection2 == null && !this.m_configuration.IsMessageTypeEnabled(NetIncomingMessageType.UnconnectedData))
							{
								return;
							}
							NetIncomingMessage netIncomingMessage = this.CreateIncomingMessage(NetIncomingMessageType.Data, num8);
							netIncomingMessage.m_isFragment = isFragment;
							netIncomingMessage.m_receiveTime = now;
							netIncomingMessage.m_sequenceNumber = (int)sequenceNumber;
							netIncomingMessage.m_receivedMessageType = netMessageType;
							netIncomingMessage.m_senderConnection = netConnection2;
							netIncomingMessage.m_senderEndPoint = ipendPoint;
							netIncomingMessage.m_bitLength = (int)num7;
							Buffer.BlockCopy(this.m_receiveBuffer, num6, netIncomingMessage.m_data, 0, num8);
							if (netConnection2 != null)
							{
								if (netMessageType == NetMessageType.Unconnected)
								{
									netIncomingMessage.m_incomingMessageType = NetIncomingMessageType.UnconnectedData;
									this.ReleaseMessage(netIncomingMessage);
								}
								else
								{
									netConnection2.ReceivedMessage(netIncomingMessage);
								}
							}
							else
							{
								netIncomingMessage.m_incomingMessageType = NetIncomingMessageType.UnconnectedData;
								this.ReleaseMessage(netIncomingMessage);
							}
						}
					}
					catch (Exception ex2)
					{
						this.LogError(string.Concat(new object[]
						{
							"Packet parsing error: ",
							ex2.Message,
							" from ",
							ipendPoint
						}));
					}
					num6 += num8;
				}
				if (this.m_socket.Available <= 0)
				{
					return;
				}
			}
			return;
			Block_13:
			this.LogWarning(string.Concat(new object[]
			{
				"Malformed packet; stated payload length ",
				num8,
				", remaining bytes ",
				num4 - num6
			}));
		}

		public void FlushSendQueue()
		{
			this.m_executeFlushSendQueue = true;
		}

		internal void HandleIncomingDiscoveryRequest(double now, IPEndPoint senderEndPoint, int ptr, int payloadByteLength)
		{
			if (this.m_configuration.IsMessageTypeEnabled(NetIncomingMessageType.DiscoveryRequest))
			{
				NetIncomingMessage netIncomingMessage = this.CreateIncomingMessage(NetIncomingMessageType.DiscoveryRequest, payloadByteLength);
				if (payloadByteLength > 0)
				{
					Buffer.BlockCopy(this.m_receiveBuffer, ptr, netIncomingMessage.m_data, 0, payloadByteLength);
				}
				netIncomingMessage.m_receiveTime = now;
				netIncomingMessage.m_bitLength = payloadByteLength * 8;
				netIncomingMessage.m_senderEndPoint = senderEndPoint;
				this.ReleaseMessage(netIncomingMessage);
			}
		}

		internal void HandleIncomingDiscoveryResponse(double now, IPEndPoint senderEndPoint, int ptr, int payloadByteLength)
		{
			if (this.m_configuration.IsMessageTypeEnabled(NetIncomingMessageType.DiscoveryResponse))
			{
				NetIncomingMessage netIncomingMessage = this.CreateIncomingMessage(NetIncomingMessageType.DiscoveryResponse, payloadByteLength);
				if (payloadByteLength > 0)
				{
					Buffer.BlockCopy(this.m_receiveBuffer, ptr, netIncomingMessage.m_data, 0, payloadByteLength);
				}
				netIncomingMessage.m_receiveTime = now;
				netIncomingMessage.m_bitLength = payloadByteLength * 8;
				netIncomingMessage.m_senderEndPoint = senderEndPoint;
				this.ReleaseMessage(netIncomingMessage);
			}
		}

		private void ReceivedUnconnectedLibraryMessage(double now, IPEndPoint senderEndPoint, NetMessageType tp, int ptr, int payloadByteLength)
		{
			NetConnection netConnection;
			if (this.m_handshakes.TryGetValue(senderEndPoint, out netConnection))
			{
				netConnection.ReceivedHandshake(now, tp, ptr, payloadByteLength);
				return;
			}
			switch (tp)
			{
			case NetMessageType.Connect:
			{
				int num = this.m_handshakes.Count + this.m_connections.Count;
				if (num >= this.m_configuration.m_maximumConnections)
				{
					NetOutgoingMessage netOutgoingMessage = this.CreateMessage("Server full");
					netOutgoingMessage.m_messageType = NetMessageType.Disconnect;
					this.SendLibrary(netOutgoingMessage, senderEndPoint);
					return;
				}
				NetConnection netConnection2 = new NetConnection(this, senderEndPoint);
				netConnection2.m_status = NetConnectionStatus.ReceivedInitiation;
				this.m_handshakes.Add(senderEndPoint, netConnection2);
				netConnection2.ReceivedHandshake(now, tp, ptr, payloadByteLength);
				return;
			}
			case NetMessageType.ConnectResponse:
				lock (this.m_handshakes)
				{
					foreach (KeyValuePair<IPEndPoint, NetConnection> keyValuePair in this.m_handshakes)
					{
						if (keyValuePair.Key.Address.Equals(senderEndPoint.Address) && keyValuePair.Value.m_connectionInitiator)
						{
							NetConnection value = keyValuePair.Value;
							this.m_connectionLookup.Remove(keyValuePair.Key);
							this.m_handshakes.Remove(keyValuePair.Key);
							value.MutateEndPoint(senderEndPoint);
							this.m_connectionLookup.Add(senderEndPoint, value);
							this.m_handshakes.Add(senderEndPoint, value);
							value.ReceivedHandshake(now, tp, ptr, payloadByteLength);
							return;
						}
					}
				}
				this.LogWarning(string.Concat(new object[]
				{
					"Received unhandled library message ",
					tp,
					" from ",
					senderEndPoint
				}));
				break;
			case NetMessageType.ConnectionEstablished:
			case NetMessageType.Acknowledge:
				goto IL_193;
			case NetMessageType.Disconnect:
				break;
			case NetMessageType.Discovery:
				this.HandleIncomingDiscoveryRequest(now, senderEndPoint, ptr, payloadByteLength);
				return;
			case NetMessageType.DiscoveryResponse:
				this.HandleIncomingDiscoveryResponse(now, senderEndPoint, ptr, payloadByteLength);
				return;
			case NetMessageType.NatPunchMessage:
				this.HandleNatPunch(ptr, senderEndPoint);
				return;
			case NetMessageType.NatIntroduction:
				this.HandleNatIntroduction(ptr);
				return;
			default:
				goto IL_193;
			}
			return;
			IL_193:
			this.LogWarning(string.Concat(new object[]
			{
				"Received unhandled library message ",
				tp,
				" from ",
				senderEndPoint
			}));
		}

		internal void AcceptConnection(NetConnection conn)
		{
			conn.InitExpandMTU(NetTime.Now);
			if (!this.m_handshakes.Remove(conn.m_remoteEndPoint))
			{
				this.LogWarning("AcceptConnection called but m_handshakes did not contain it!");
			}
			lock (this.m_connections)
			{
				if (this.m_connections.Contains(conn))
				{
					this.LogWarning("AcceptConnection called but m_connection already contains it!");
				}
				else
				{
					this.m_connections.Add(conn);
					this.m_connectionLookup.Add(conn.m_remoteEndPoint, conn);
				}
			}
		}

		[Conditional("DEBUG")]
		internal void VerifyNetworkThread()
		{
			Thread currentThread = Thread.CurrentThread;
			if (Thread.CurrentThread != this.m_networkThread)
			{
				throw new NetException(string.Concat(new object[]
				{
					"Executing on wrong thread! Should be library system thread (is ",
					currentThread.Name,
					" mId ",
					currentThread.ManagedThreadId,
					")"
				}));
			}
		}

		internal NetIncomingMessage SetupReadHelperMessage(int ptr, int payloadLength)
		{
			this.m_readHelperMessage.m_bitLength = (ptr + payloadLength) * 8;
			this.m_readHelperMessage.m_readPosition = ptr * 8;
			return this.m_readHelperMessage;
		}

		[Conditional("DEBUG")]
		internal void LogVerbose(string message)
		{
			if (this.m_configuration.IsMessageTypeEnabled(NetIncomingMessageType.VerboseDebugMessage))
			{
				this.ReleaseMessage(this.CreateIncomingMessage(NetIncomingMessageType.VerboseDebugMessage, message));
			}
		}

		[Conditional("DEBUG")]
		internal void LogDebug(string message)
		{
			if (this.m_configuration.IsMessageTypeEnabled(NetIncomingMessageType.DebugMessage))
			{
				this.ReleaseMessage(this.CreateIncomingMessage(NetIncomingMessageType.DebugMessage, message));
			}
		}

		internal void LogWarning(string message)
		{
			if (this.m_configuration.IsMessageTypeEnabled(NetIncomingMessageType.WarningMessage))
			{
				this.ReleaseMessage(this.CreateIncomingMessage(NetIncomingMessageType.WarningMessage, message));
			}
		}

		internal void LogError(string message)
		{
			if (this.m_configuration.IsMessageTypeEnabled(NetIncomingMessageType.ErrorMessage))
			{
				this.ReleaseMessage(this.CreateIncomingMessage(NetIncomingMessageType.ErrorMessage, message));
			}
		}

		public NetPeerStatus Status
		{
			get
			{
				return this.m_status;
			}
		}

		public AutoResetEvent MessageReceivedEvent
		{
			get
			{
				if (this.m_messageReceivedEvent == null)
				{
					this.m_messageReceivedEvent = new AutoResetEvent(false);
				}
				return this.m_messageReceivedEvent;
			}
		}

		public long UniqueIdentifier
		{
			get
			{
				return this.m_uniqueIdentifier;
			}
		}

		public int Port
		{
			get
			{
				return this.m_listenPort;
			}
		}

		public NetUPnP UPnP
		{
			get
			{
				return this.m_upnp;
			}
		}

		public object Tag
		{
			get
			{
				return this.m_tag;
			}
			set
			{
				this.m_tag = value;
			}
		}

		public List<NetConnection> Connections
		{
			get
			{
				List<NetConnection> result;
				lock (this.m_connections)
				{
					result = new List<NetConnection>(this.m_connections);
				}
				return result;
			}
		}

		public int ConnectionsCount
		{
			get
			{
				return this.m_connections.Count;
			}
		}

		public NetPeerStatistics Statistics
		{
			get
			{
				return this.m_statistics;
			}
		}

		public NetPeerConfiguration Configuration
		{
			get
			{
				return this.m_configuration;
			}
		}

		public NetPeer(NetPeerConfiguration config)
		{
			this.m_configuration = config;
			this.m_statistics = new NetPeerStatistics(this);
			this.m_releasedIncomingMessages = new NetQueue<NetIncomingMessage>(4);
			this.m_unsentUnconnectedMessages = new NetQueue<NetTuple<IPEndPoint, NetOutgoingMessage>>(2);
			this.m_connections = new List<NetConnection>();
			this.m_connectionLookup = new Dictionary<IPEndPoint, NetConnection>();
			this.m_handshakes = new Dictionary<IPEndPoint, NetConnection>();
			this.m_senderRemote = new IPEndPoint(IPAddress.Any, 0);
			this.m_status = NetPeerStatus.NotRunning;
			this.m_receivedFragmentGroups = new Dictionary<NetConnection, Dictionary<int, ReceivedFragmentGroup>>();
		}

		public void Start()
		{
			if (this.m_status != NetPeerStatus.NotRunning)
			{
				this.LogWarning("Start() called on already running NetPeer - ignoring.");
				return;
			}
			this.m_status = NetPeerStatus.Starting;
			if (this.m_configuration.NetworkThreadName == "Lidgren network thread")
			{
				int num = Interlocked.Increment(ref NetPeer.s_initializedPeersCount);
				this.m_configuration.NetworkThreadName = "Lidgren network thread " + num.ToString();
			}
			this.InitializeNetwork();
			this.m_networkThread = new Thread(new ThreadStart(this.NetworkLoop));
			this.m_networkThread.Name = this.m_configuration.NetworkThreadName;
			this.m_networkThread.IsBackground = true;
			this.m_networkThread.Priority = ThreadPriority.BelowNormal;
			this.m_networkThread.Start();
			if (this.m_upnp != null)
			{
				this.m_upnp.Discover(this);
			}
			Thread.Sleep(50);
		}

		public NetConnection GetConnection(IPEndPoint ep)
		{
			NetConnection result;
			this.m_connectionLookup.TryGetValue(ep, out result);
			return result;
		}

		public NetIncomingMessage WaitMessage(int maxMillis)
		{
			NetIncomingMessage netIncomingMessage = this.ReadMessage();
			if (netIncomingMessage != null)
			{
				return netIncomingMessage;
			}
			if (this.m_messageReceivedEvent != null)
			{
				this.m_messageReceivedEvent.WaitOne(maxMillis);
			}
			return this.ReadMessage();
		}

		public NetIncomingMessage ReadMessage()
		{
			NetIncomingMessage netIncomingMessage;
			if (this.m_releasedIncomingMessages.TryDequeue(out netIncomingMessage) && netIncomingMessage.MessageType == NetIncomingMessageType.StatusChanged)
			{
				NetConnectionStatus visibleStatus = (NetConnectionStatus)netIncomingMessage.PeekByte();
				netIncomingMessage.SenderConnection.m_visibleStatus = visibleStatus;
			}
			return netIncomingMessage;
		}

		public int ReadMessages(IList<NetIncomingMessage> addTo)
		{
			int num = this.m_releasedIncomingMessages.TryDrain(addTo);
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					int index = addTo.Count - num + i;
					NetIncomingMessage netIncomingMessage = addTo[index];
					if (netIncomingMessage.MessageType == NetIncomingMessageType.StatusChanged)
					{
						NetConnectionStatus visibleStatus = (NetConnectionStatus)netIncomingMessage.PeekByte();
						netIncomingMessage.SenderConnection.m_visibleStatus = visibleStatus;
					}
				}
			}
			return num;
		}

		internal void SendLibrary(NetOutgoingMessage msg, IPEndPoint recipient)
		{
			int numBytes = msg.Encode(this.m_sendBuffer, 0, 0);
			bool flag;
			this.SendPacket(numBytes, recipient, 1, out flag);
		}

		public NetConnection Connect(string host, int port)
		{
			return this.Connect(new IPEndPoint(NetUtility.Resolve(host), port), null);
		}

		public NetConnection Connect(string host, int port, NetOutgoingMessage hailMessage)
		{
			return this.Connect(new IPEndPoint(NetUtility.Resolve(host), port), hailMessage);
		}

		public NetConnection Connect(IPEndPoint remoteEndPoint)
		{
			return this.Connect(remoteEndPoint, null);
		}

		public virtual NetConnection Connect(IPEndPoint remoteEndPoint, NetOutgoingMessage hailMessage)
		{
			if (remoteEndPoint == null)
			{
				throw new ArgumentNullException("remoteEndPoint");
			}
			NetConnection result;
			lock (this.m_connections)
			{
				if (this.m_status == NetPeerStatus.NotRunning)
				{
					throw new NetException("Must call Start() first");
				}
				if (this.m_connectionLookup.ContainsKey(remoteEndPoint))
				{
					throw new NetException("Already connected to that endpoint!");
				}
				NetConnection netConnection;
				if (this.m_handshakes.TryGetValue(remoteEndPoint, out netConnection))
				{
					NetConnectionStatus status = netConnection.m_status;
					if (status != NetConnectionStatus.InitiatedConnect)
					{
						if (status != NetConnectionStatus.RespondedConnect)
						{
							this.LogWarning("Weird situation; Connect() already in progress to remote endpoint; but hs status is " + netConnection.m_status);
						}
						else
						{
							netConnection.SendConnectResponse((float)NetTime.Now, false);
						}
					}
					else
					{
						netConnection.m_connectRequested = true;
					}
					result = netConnection;
				}
				else
				{
					NetConnection netConnection2 = new NetConnection(this, remoteEndPoint);
					netConnection2.m_status = NetConnectionStatus.InitiatedConnect;
					netConnection2.m_localHailMessage = hailMessage;
					netConnection2.m_connectRequested = true;
					netConnection2.m_connectionInitiator = true;
					this.m_handshakes.Add(remoteEndPoint, netConnection2);
					result = netConnection2;
				}
			}
			return result;
		}

		internal void RawSend(byte[] arr, int offset, int length, IPEndPoint destination)
		{
			Array.Copy(arr, offset, this.m_sendBuffer, 0, length);
			bool flag;
			this.SendPacket(length, destination, 1, out flag);
		}

		public void Shutdown(string bye)
		{
			if (this.m_socket == null)
			{
				return;
			}
			this.m_shutdownReason = bye;
			this.m_status = NetPeerStatus.ShutdownRequested;
		}

		public NetSendResult SendMessage(NetOutgoingMessage msg, NetConnection recipient, NetDeliveryMethod method)
		{
			return this.SendMessage(msg, recipient, method, 0);
		}

		public NetSendResult SendMessage(NetOutgoingMessage msg, NetConnection recipient, NetDeliveryMethod method, int sequenceChannel)
		{
			if (msg == null)
			{
				throw new ArgumentNullException("msg");
			}
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			if (sequenceChannel >= 32)
			{
				throw new ArgumentOutOfRangeException("sequenceChannel");
			}
			if (msg.m_isSent)
			{
				throw new NetException("This message has already been sent! Use NetPeer.SendMessage() to send to multiple recipients efficiently");
			}
			msg.m_isSent = true;
			int num = 5 + msg.LengthBytes;
			if (num <= recipient.m_currentMTU)
			{
				Interlocked.Increment(ref msg.m_recyclingCount);
				return recipient.EnqueueMessage(msg, method, sequenceChannel);
			}
			if (recipient.m_status != NetConnectionStatus.Connected)
			{
				return NetSendResult.FailedNotConnected;
			}
			this.SendFragmentedMessage(msg, new NetConnection[]
			{
				recipient
			}, method, sequenceChannel);
			return NetSendResult.Queued;
		}

		internal int GetMTU(IList<NetConnection> recipients)
		{
			int count = recipients.Count;
			int num = int.MaxValue;
			for (int i = 0; i < count; i++)
			{
				NetConnection netConnection = recipients[i];
				int currentMTU = netConnection.m_currentMTU;
				if (currentMTU < num)
				{
					num = currentMTU;
				}
			}
			return num;
		}

		public void SendMessage(NetOutgoingMessage msg, List<NetConnection> recipients, NetDeliveryMethod method, int sequenceChannel)
		{
			if (msg == null)
			{
				throw new ArgumentNullException("msg");
			}
			if (recipients == null)
			{
				throw new ArgumentNullException("recipients");
			}
			if (recipients.Count < 1)
			{
				throw new NetException("recipients must contain at least one item");
			}
			if (method != NetDeliveryMethod.Unreliable)
			{
			}
			if (msg.m_isSent)
			{
				throw new NetException("This message has already been sent! Use NetPeer.SendMessage() to send to multiple recipients efficiently");
			}
			int mtu = this.GetMTU(recipients);
			msg.m_isSent = true;
			int encodedSize = msg.GetEncodedSize();
			if (encodedSize <= mtu)
			{
				Interlocked.Add(ref msg.m_recyclingCount, recipients.Count);
				using (List<NetConnection>.Enumerator enumerator = recipients.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						NetConnection netConnection = enumerator.Current;
						if (netConnection == null)
						{
							Interlocked.Decrement(ref msg.m_recyclingCount);
						}
						else
						{
							NetSendResult netSendResult = netConnection.EnqueueMessage(msg, method, sequenceChannel);
							if (netSendResult != NetSendResult.Queued && netSendResult != NetSendResult.Sent)
							{
								Interlocked.Decrement(ref msg.m_recyclingCount);
							}
						}
					}
					return;
				}
			}
			this.SendFragmentedMessage(msg, recipients, method, sequenceChannel);
		}

		public void SendUnconnectedMessage(NetOutgoingMessage msg, string host, int port)
		{
			if (msg == null)
			{
				throw new ArgumentNullException("msg");
			}
			if (host == null)
			{
				throw new ArgumentNullException("host");
			}
			if (msg.m_isSent)
			{
				throw new NetException("This message has already been sent! Use NetPeer.SendMessage() to send to multiple recipients efficiently");
			}
			if (msg.LengthBytes > this.m_configuration.MaximumTransmissionUnit)
			{
				throw new NetException("Unconnected messages too long! Must be shorter than NetConfiguration.MaximumTransmissionUnit (currently " + this.m_configuration.MaximumTransmissionUnit + ")");
			}
			IPAddress ipaddress = NetUtility.Resolve(host);
			if (ipaddress == null)
			{
				throw new NetException("Failed to resolve " + host);
			}
			msg.m_messageType = NetMessageType.Unconnected;
			msg.m_isSent = true;
			Interlocked.Increment(ref msg.m_recyclingCount);
			this.m_unsentUnconnectedMessages.Enqueue(new NetTuple<IPEndPoint, NetOutgoingMessage>(new IPEndPoint(ipaddress, port), msg));
		}

		public void SendUnconnectedMessage(NetOutgoingMessage msg, IPEndPoint recipient)
		{
			if (msg == null)
			{
				throw new ArgumentNullException("msg");
			}
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			if (msg.m_isSent)
			{
				throw new NetException("This message has already been sent! Use NetPeer.SendMessage() to send to multiple recipients efficiently");
			}
			if (msg.LengthBytes > this.m_configuration.MaximumTransmissionUnit)
			{
				throw new NetException("Unconnected messages too long! Must be shorter than NetConfiguration.MaximumTransmissionUnit (currently " + this.m_configuration.MaximumTransmissionUnit + ")");
			}
			msg.m_messageType = NetMessageType.Unconnected;
			msg.m_isSent = true;
			Interlocked.Increment(ref msg.m_recyclingCount);
			this.m_unsentUnconnectedMessages.Enqueue(new NetTuple<IPEndPoint, NetOutgoingMessage>(recipient, msg));
		}

		public void SendUnconnectedMessage(NetOutgoingMessage msg, IList<IPEndPoint> recipients)
		{
			if (msg == null)
			{
				throw new ArgumentNullException("msg");
			}
			if (recipients == null)
			{
				throw new ArgumentNullException("recipients");
			}
			if (recipients.Count < 1)
			{
				throw new NetException("recipients must contain at least one item");
			}
			if (msg.m_isSent)
			{
				throw new NetException("This message has already been sent! Use NetPeer.SendMessage() to send to multiple recipients efficiently");
			}
			if (msg.LengthBytes > this.m_configuration.MaximumTransmissionUnit)
			{
				throw new NetException("Unconnected messages too long! Must be shorter than NetConfiguration.MaximumTransmissionUnit (currently " + this.m_configuration.MaximumTransmissionUnit + ")");
			}
			msg.m_messageType = NetMessageType.Unconnected;
			msg.m_isSent = true;
			Interlocked.Add(ref msg.m_recyclingCount, recipients.Count);
			foreach (IPEndPoint item in recipients)
			{
				this.m_unsentUnconnectedMessages.Enqueue(new NetTuple<IPEndPoint, NetOutgoingMessage>(item, msg));
			}
		}

		public void SendUnconnectedToSelf(NetOutgoingMessage msg)
		{
			if (msg == null)
			{
				throw new ArgumentNullException("msg");
			}
			if (msg.m_isSent)
			{
				throw new NetException("This message has already been sent! Use NetPeer.SendMessage() to send to multiple recipients efficiently");
			}
			msg.m_messageType = NetMessageType.Unconnected;
			msg.m_isSent = true;
			if (!this.m_configuration.IsMessageTypeEnabled(NetIncomingMessageType.UnconnectedData))
			{
				return;
			}
			NetIncomingMessage netIncomingMessage = this.CreateIncomingMessage(NetIncomingMessageType.UnconnectedData, msg.LengthBytes);
			netIncomingMessage.m_isFragment = false;
			netIncomingMessage.m_receiveTime = NetTime.Now;
			netIncomingMessage.m_senderConnection = null;
			netIncomingMessage.m_senderEndPoint = (this.m_socket.LocalEndPoint as IPEndPoint);
			netIncomingMessage.m_bitLength = msg.LengthBits;
			this.ReleaseMessage(netIncomingMessage);
		}

		public void DiscoverLocalPeers(int serverPort)
		{
			NetOutgoingMessage netOutgoingMessage = this.CreateMessage(0);
			netOutgoingMessage.m_messageType = NetMessageType.Discovery;
			this.m_unsentUnconnectedMessages.Enqueue(new NetTuple<IPEndPoint, NetOutgoingMessage>(new IPEndPoint(IPAddress.Broadcast, serverPort), netOutgoingMessage));
		}

		public bool DiscoverKnownPeer(string host, int serverPort)
		{
			IPAddress ipaddress = NetUtility.Resolve(host);
			if (ipaddress == null)
			{
				return false;
			}
			this.DiscoverKnownPeer(new IPEndPoint(ipaddress, serverPort));
			return true;
		}

		public void DiscoverKnownPeer(IPEndPoint endPoint)
		{
			NetOutgoingMessage netOutgoingMessage = this.CreateMessage(0);
			netOutgoingMessage.m_messageType = NetMessageType.Discovery;
			this.m_unsentUnconnectedMessages.Enqueue(new NetTuple<IPEndPoint, NetOutgoingMessage>(endPoint, netOutgoingMessage));
		}

		public void DiscoverKnownPeer(IPEndPoint endPoint, NetOutgoingMessage msg)
		{
			msg.m_messageType = NetMessageType.Discovery;
			this.m_unsentUnconnectedMessages.Enqueue(new NetTuple<IPEndPoint, NetOutgoingMessage>(endPoint, msg));
		}

		public void SendDiscoveryResponse(NetOutgoingMessage msg, IPEndPoint recipient)
		{
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			if (msg == null)
			{
				msg = this.CreateMessage(0);
			}
			else if (msg.m_isSent)
			{
				throw new NetException("Message has already been sent!");
			}
			if (msg.LengthBytes >= this.m_configuration.MaximumTransmissionUnit)
			{
				throw new NetException("Cannot send discovery message larger than MTU (currently " + this.m_configuration.MaximumTransmissionUnit + " bytes)");
			}
			msg.m_messageType = NetMessageType.DiscoveryResponse;
			this.m_unsentUnconnectedMessages.Enqueue(new NetTuple<IPEndPoint, NetOutgoingMessage>(recipient, msg));
		}

		internal bool SendMTUPacket(int numBytes, IPEndPoint target)
		{
			try
			{
				this.m_socket.DontFragment = true;
				int num = this.m_socket.SendTo(this.m_sendBuffer, 0, numBytes, SocketFlags.None, target);
				if (numBytes != num)
				{
					this.LogWarning(string.Concat(new object[]
					{
						"Failed to send the full ",
						numBytes,
						"; only ",
						num,
						" bytes sent in packet!"
					}));
				}
			}
			catch (SocketException ex)
			{
				if (ex.SocketErrorCode == SocketError.MessageSize)
				{
					return false;
				}
				if (ex.SocketErrorCode == SocketError.WouldBlock)
				{
					this.LogWarning("Socket threw exception; would block - send buffer full? Increase in NetPeerConfiguration");
					return true;
				}
				if (ex.SocketErrorCode == SocketError.ConnectionReset)
				{
					return true;
				}
				this.LogError(string.Concat(new object[]
				{
					"Failed to send packet: (",
					ex.SocketErrorCode,
					") ",
					ex
				}));
			}
			catch (Exception arg)
			{
				this.LogError("Failed to send packet: " + arg);
			}
			finally
			{
				this.m_socket.DontFragment = false;
			}
			return true;
		}

		internal void SendPacket(int numBytes, IPEndPoint target, int numMessages, out bool connectionReset)
		{
			connectionReset = false;
			try
			{
				if (target.Address == IPAddress.Broadcast)
				{
					this.m_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
				}
				int num = this.m_socket.SendTo(this.m_sendBuffer, 0, numBytes, SocketFlags.None, target);
				if (numBytes != num)
				{
					this.LogWarning(string.Concat(new object[]
					{
						"Failed to send the full ",
						numBytes,
						"; only ",
						num,
						" bytes sent in packet!"
					}));
				}
			}
			catch (SocketException ex)
			{
				if (ex.SocketErrorCode == SocketError.WouldBlock)
				{
					this.LogWarning("Socket threw exception; would block - send buffer full? Increase in NetPeerConfiguration");
				}
				else if (ex.SocketErrorCode == SocketError.ConnectionReset)
				{
					connectionReset = true;
				}
				else
				{
					this.LogError("Failed to send packet: " + ex);
				}
			}
			catch (Exception arg)
			{
				this.LogError("Failed to send packet: " + arg);
			}
			finally
			{
				if (target.Address == IPAddress.Broadcast)
				{
					this.m_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, false);
				}
			}
		}

		private void FlushDelayedPackets()
		{
		}

		private void SendCallBack(IAsyncResult res)
		{
			this.m_socket.EndSendTo(res);
		}
	}
}
