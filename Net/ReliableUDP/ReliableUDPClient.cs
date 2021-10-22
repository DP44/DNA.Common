using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using DNA.Net.GamerServices;

namespace DNA.Net.ReliableUDP
{
	public class ReliableUDPClient : IDisposable
	{
		private enum CommandWords : byte
		{
			Data,
			DataReliable,
			DataInOrder,
			DataInOrderReliable,
			ACK,
			ACKRIO,
			NACK,
			NACKRIO,
			HeartBeat,
			Disconnect
		}

		private class Host
		{
			public IPEndPoint EndPoint = new IPEndPoint(IPAddress.Any, 0);

			public DateTime LastSentTo = DateTime.Now;

			public DateTime LastHeardFrom = DateTime.Now;

			public int LastInOrderPacketNumberReceived;

			public LinkedList<int> ReliablePacketsReceived = new LinkedList<int>();

			public LinkedList<ReliableUDPClient.Packet> ReliableInOrderPacketsReceived = new LinkedList<ReliableUDPClient.Packet>();

			public List<ReliableUDPClient.Packet> InOrderBuffer = new List<ReliableUDPClient.Packet>();

			public MemoryStream _sendStream = new MemoryStream(65535);

			public BinaryWriter _sendWriter;

			public int NextInOrderPacketNumberSent = 1;

			public int NextReliablePacketNumberSent = 1;

			public int NextReliableInOrderPacketNumberSent = 1;

			public Dictionary<int, ReliableUDPClient.Packet> _unacknolegedReliablePackets = new Dictionary<int, ReliableUDPClient.Packet>();

			public Dictionary<int, ReliableUDPClient.Packet> _unacknolegedReliableInOrderPackets = new Dictionary<int, ReliableUDPClient.Packet>();

			public int LastRIOPacketDispatched;

			public Host()
			{
				this.ReliablePacketsReceived.AddLast(0);
				this._sendWriter = new BinaryWriter(this._sendStream);
			}
		}

		private class Packet
		{
			private static Queue<ReliableUDPClient.Packet> PacketGrave = new Queue<ReliableUDPClient.Packet>();

			private ReliableUDPClient.CommandWords _packetType;

			public ReliableUDPClient.Host Sender;

			public DateTime LastSent;

			public int SequenceNumber;

			private MemoryStream _playloadStream;

			public int AckNumber;

			public ReliableUDPClient.CommandWords PacketType
			{
				get
				{
					return this._packetType;
				}
			}

			public byte[] PayloadBuffer
			{
				get
				{
					return this._playloadStream.GetBuffer();
				}
			}

			public int PayloadLength
			{
				get
				{
					return (int)this._playloadStream.Length;
				}
			}

			public static ReliableUDPClient.Packet Read(ReliableUDPClient.Host sender, BinaryReader reader)
			{
				ReliableUDPClient.CommandWords packetType = (ReliableUDPClient.CommandWords)reader.ReadByte();
				ReliableUDPClient.Packet packet = ReliableUDPClient.Packet.Create(packetType);
				packet.Receive(sender, reader);
				return packet;
			}

			public static ReliableUDPClient.Packet Create(ReliableUDPClient.CommandWords packetType)
			{
				ReliableUDPClient.Packet packet;
				lock (ReliableUDPClient.Packet.PacketGrave)
				{
					if (ReliableUDPClient.Packet.PacketGrave.Count > 0)
					{
						packet = ReliableUDPClient.Packet.PacketGrave.Dequeue();
					}
					else
					{
						packet = new ReliableUDPClient.Packet();
					}
				}
				packet._packetType = packetType;
				return packet;
			}

			private Packet()
			{
				this._playloadStream = new MemoryStream(4096);
			}

			private void Receive(ReliableUDPClient.Host sender, BinaryReader reader)
			{
				this.Sender = sender;
				switch (this._packetType)
				{
				case ReliableUDPClient.CommandWords.Data:
					this.ReadPayload(reader);
					return;
				case ReliableUDPClient.CommandWords.DataReliable:
					this.SequenceNumber = reader.ReadInt32();
					this.ReadPayload(reader);
					return;
				case ReliableUDPClient.CommandWords.DataInOrder:
					this.SequenceNumber = reader.ReadInt32();
					this.ReadPayload(reader);
					return;
				case ReliableUDPClient.CommandWords.DataInOrderReliable:
					this.SequenceNumber = reader.ReadInt32();
					this.ReadPayload(reader);
					return;
				case ReliableUDPClient.CommandWords.ACK:
					this.AckNumber = reader.ReadInt32();
					return;
				case ReliableUDPClient.CommandWords.ACKRIO:
					this.AckNumber = reader.ReadInt32();
					return;
				case ReliableUDPClient.CommandWords.NACK:
				case ReliableUDPClient.CommandWords.NACKRIO:
					return;
				default:
					throw new NotImplementedException();
				}
			}

			public override string ToString()
			{
				return "Packet #" + this.SequenceNumber.ToString();
			}

			private void ReadPayload(BinaryReader reader)
			{
				int num = (int)reader.ReadUInt16();
				this._playloadStream.SetLength((long)num);
				reader.Read(this._playloadStream.GetBuffer(), 0, (int)this._playloadStream.Length);
			}

			private void WritePayload(BinaryWriter writer)
			{
				if (this._playloadStream.Position > 65535L)
				{
					throw new Exception("Packet too Big");
				}
				ushort value = (ushort)this._playloadStream.Position;
				writer.Write(value);
				writer.Write(this._playloadStream.GetBuffer(), 0, (int)this._playloadStream.Position);
			}

			public void Send(ReliableUDPClient.Host sendTo)
			{
				this.LastSent = DateTime.Now;
				sendTo.LastSentTo = DateTime.Now;
				BinaryWriter sendWriter = sendTo._sendWriter;
				lock (sendWriter)
				{
					sendWriter.Write((byte)this._packetType);
					switch (this.PacketType)
					{
					case ReliableUDPClient.CommandWords.Data:
						this.WritePayload(sendTo._sendWriter);
						break;
					case ReliableUDPClient.CommandWords.DataReliable:
						sendWriter.Write(this.SequenceNumber);
						this.WritePayload(sendTo._sendWriter);
						break;
					case ReliableUDPClient.CommandWords.DataInOrder:
						sendWriter.Write(this.SequenceNumber);
						this.WritePayload(sendTo._sendWriter);
						break;
					case ReliableUDPClient.CommandWords.DataInOrderReliable:
						sendWriter.Write(this.SequenceNumber);
						this.WritePayload(sendTo._sendWriter);
						break;
					case ReliableUDPClient.CommandWords.ACK:
						sendWriter.Write(this.AckNumber);
						break;
					case ReliableUDPClient.CommandWords.ACKRIO:
						sendWriter.Write(this.AckNumber);
						break;
					case ReliableUDPClient.CommandWords.NACK:
					case ReliableUDPClient.CommandWords.NACKRIO:
						break;
					default:
						throw new NotImplementedException();
					}
				}
			}

			private void Reset()
			{
				this.AckNumber = -1;
				this._playloadStream.Position = 0L;
				this.LastSent = DateTime.Now;
			}

			public void SetData(byte[] data, int offset, int length)
			{
				this._playloadStream.Position = 0L;
				this._playloadStream.Write(data, offset, length);
			}

			public void Release()
			{
				this.Reset();
				lock (ReliableUDPClient.Packet.PacketGrave)
				{
					ReliableUDPClient.Packet.PacketGrave.Enqueue(this);
				}
			}
		}

		public const int MaxPacketSize = 65535;

		public float SimulatePacketLoss;

		public Random _pakectLossRand = new Random();

		private Socket _socket;

		private MemoryStream _receiveStream;

		private BinaryReader _receiveReader;

		private List<ReliableUDPClient.Packet> ReliablePackets = new List<ReliableUDPClient.Packet>();

		private List<ReliableUDPClient.Host> Hosts = new List<ReliableUDPClient.Host>();

		private Queue<ReliableUDPClient.Packet> DispatchQueue = new Queue<ReliableUDPClient.Packet>();

		private bool _disposed;

		public int PacketsReady
		{
			get
			{
				return this.DispatchQueue.Count;
			}
		}

		public int ReadPacket(byte[] buffer, int offset, out IPEndPoint endpoint)
		{
			if (this.PacketsReady == 0)
			{
				endpoint = null;
				return -1;
			}
			ReliableUDPClient.Packet packet = this.DispatchQueue.Dequeue();
			int payloadLength = packet.PayloadLength;
			Array.Copy(packet.PayloadBuffer, 0, buffer, offset, payloadLength);
			endpoint = packet.Sender.EndPoint;
			packet.Release();
			return payloadLength;
		}

		private ReliableUDPClient.Host GetHostFromEndpoint(IPEndPoint endPoint)
		{
			for (int i = 0; i < this.Hosts.Count; i++)
			{
				if (this.Hosts[i].EndPoint.Equals(endPoint))
				{
					return this.Hosts[i];
				}
			}
			ReliableUDPClient.Host host = new ReliableUDPClient.Host();
			host.EndPoint = endPoint;
			this.Hosts.Add(host);
			return host;
		}

		public ReliableUDPClient(int port)
		{
			IPEndPoint localEP = new IPEndPoint(IPAddress.Any, port);
			this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			this._socket.Bind(localEP);
			this._receiveStream = new MemoryStream(65535);
			this._receiveStream.SetLength((long)this._receiveStream.Capacity);
			this._receiveReader = new BinaryReader(this._receiveStream);
		}

		private static ReliableUDPClient.CommandWords ToCommandWord(SendDataOptions option)
		{
			switch (option)
			{
			case SendDataOptions.None:
				return ReliableUDPClient.CommandWords.Data;
			case SendDataOptions.Reliable:
				return ReliableUDPClient.CommandWords.DataReliable;
			case SendDataOptions.InOrder:
				return ReliableUDPClient.CommandWords.DataInOrder;
			case SendDataOptions.ReliableInOrder:
				return ReliableUDPClient.CommandWords.DataInOrderReliable;
			case SendDataOptions.Chat:
				return ReliableUDPClient.CommandWords.DataInOrderReliable;
			default:
				throw new Exception("Bad option");
			}
		}

		public void SendPacket(byte[] sendData, int offset, int length, IPEndPoint endPoint, SendDataOptions option)
		{
			ReliableUDPClient.Packet packet = ReliableUDPClient.Packet.Create(ReliableUDPClient.ToCommandWord(option));
			packet.SetData(sendData, offset, length);
			ReliableUDPClient.Host hostFromEndpoint = this.GetHostFromEndpoint(endPoint);
			switch (option)
			{
				case SendDataOptions.None:
					packet.Send(hostFromEndpoint);
					packet.Release();
					break;
				case SendDataOptions.Reliable:
					packet.SequenceNumber = hostFromEndpoint.NextReliablePacketNumberSent++;
					lock (hostFromEndpoint._unacknolegedReliablePackets)
						hostFromEndpoint._unacknolegedReliablePackets[packet.SequenceNumber] = packet;
					packet.Send(hostFromEndpoint);
					break;
				case SendDataOptions.InOrder:
					packet.SequenceNumber = hostFromEndpoint.NextInOrderPacketNumberSent++;
					packet.Send(hostFromEndpoint);
					packet.Release();
					break;
				case SendDataOptions.ReliableInOrder:
					packet.SequenceNumber = hostFromEndpoint.NextReliableInOrderPacketNumberSent++;
					lock (hostFromEndpoint._unacknolegedReliableInOrderPackets)
						hostFromEndpoint._unacknolegedReliableInOrderPackets[packet.SequenceNumber] = packet;
					packet.Send(hostFromEndpoint);
					break;
			}
		}

		private void SendNACK(ReliableUDPClient.Host sendTo)
		{
			ReliableUDPClient.Packet packet = ReliableUDPClient.Packet.Create(ReliableUDPClient.CommandWords.NACK);
			packet.Send(sendTo);
			packet.Release();
		}

		private void SendNACKRIO(ReliableUDPClient.Host sendTo)
		{
			ReliableUDPClient.Packet packet = ReliableUDPClient.Packet.Create(ReliableUDPClient.CommandWords.NACKRIO);
			packet.Send(sendTo);
			packet.Release();
		}

		public void FlushSendBuffers()
		{
			for (int i = 0; i < this.Hosts.Count; i++)
			{
				lock (this.Hosts[i]._sendWriter)
				{
					this.Hosts[i]._sendWriter.Flush();
					if (this.Hosts[i]._sendStream.Position > 0L)
					{
						this._socket.SendTo(this.Hosts[i]._sendStream.GetBuffer(), 0, (int)this.Hosts[i]._sendStream.Position, SocketFlags.None, this.Hosts[i].EndPoint);
						this.Hosts[i]._sendStream.Position = 0L;
					}
				}
			}
		}

		public void ReceivePackets()
		{
			lock (this._receiveStream)
			{
				this._receiveStream.Position = 0L;
				EndPoint remoteEP = (EndPoint) new IPEndPoint(0L, 0);
				int from = this._socket.ReceiveFrom(this._receiveStream.GetBuffer(), ref remoteEP);
				ReliableUDPClient.Host hostFromEndpoint = this.GetHostFromEndpoint((IPEndPoint) remoteEP);
				bool flag1 = false;
				bool flag2 = false;
				bool flag3 = false;
				bool flag4 = false;
label_47:
				while (this._receiveStream.Position < (long) from)
				{
					ReliableUDPClient.Packet packet1 = ReliableUDPClient.Packet.Read(hostFromEndpoint, this._receiveReader);
					if (this._pakectLossRand.Decide(this.SimulatePacketLoss))
					{
						packet1.Release();
					}
					else
					{
						switch (packet1.PacketType)
						{
							case ReliableUDPClient.CommandWords.Data:
								this.DispatchQueue.Enqueue(packet1);
								continue;
							case ReliableUDPClient.CommandWords.DataReliable:
								flag1 = true;
								LinkedListNode<int> linkedListNode1 = hostFromEndpoint.ReliablePacketsReceived.First;
								ReliableUDPClient.Packet packet2 = ReliableUDPClient.Packet.Create(ReliableUDPClient.CommandWords.ACK);
								packet2.AckNumber = packet1.SequenceNumber;
								packet2.Send(hostFromEndpoint);
								packet2.Release();
								LinkedListNode<int> next;
								while (true)
								{
									next = linkedListNode1.Next;
									if (packet1.SequenceNumber > linkedListNode1.Value)
									{
										if (next != null)
										{
											if (next.Value <= packet1.SequenceNumber)
												linkedListNode1 = next;
											else
												goto label_15;
										}
										else
											goto label_13;
									}
									else
										break;
								}
								packet1.Release();
								goto label_18;
label_13:
								LinkedListNode<int> linkedListNode2 = hostFromEndpoint.ReliablePacketsReceived.AddLast(packet1.SequenceNumber);
								this.DispatchQueue.Enqueue(packet1);
								goto label_18;
label_15:
								linkedListNode2 = hostFromEndpoint.ReliablePacketsReceived.AddBefore(next, packet1.SequenceNumber);
								this.DispatchQueue.Enqueue(packet1);
label_18:
								while (true)
								{
									if (hostFromEndpoint.ReliablePacketsReceived.First.Next != null && hostFromEndpoint.ReliablePacketsReceived.First.Value == hostFromEndpoint.ReliablePacketsReceived.First.Next.Value - 1)
										hostFromEndpoint.ReliablePacketsReceived.RemoveFirst();
									else
										goto label_47;
								}
							case ReliableUDPClient.CommandWords.DataInOrder:
								if (packet1.SequenceNumber > packet1.Sender.LastInOrderPacketNumberReceived)
								{
									packet1.Sender.LastInOrderPacketNumberReceived = packet1.SequenceNumber;
									this.DispatchQueue.Enqueue(packet1);
									continue;
								}
								packet1.Release();
								continue;
							case ReliableUDPClient.CommandWords.DataInOrderReliable:
								ReliableUDPClient.Packet packet3 = ReliableUDPClient.Packet.Create(ReliableUDPClient.CommandWords.ACKRIO);
								packet3.AckNumber = packet1.SequenceNumber;
								packet3.Send(hostFromEndpoint);
								packet3.Release();
								flag2 = true;
								LinkedListNode<ReliableUDPClient.Packet> node = hostFromEndpoint.ReliableInOrderPacketsReceived.First;
								if (packet1.SequenceNumber <= hostFromEndpoint.LastRIOPacketDispatched)
								{
									packet1.Release();
									continue;
								}
								for (; node != null; node = node.Next)
								{
									if (packet1.SequenceNumber == node.Value.SequenceNumber)
									{
										packet1.Release();
										goto label_29;
									}
									else if (packet1.SequenceNumber < node.Value.SequenceNumber)
									{
										hostFromEndpoint.ReliableInOrderPacketsReceived.AddBefore(node, packet1);
										goto label_29;
									}
								}
								hostFromEndpoint.ReliableInOrderPacketsReceived.AddLast(packet1);
label_29:
								while (true)
								{
									if (hostFromEndpoint.ReliableInOrderPacketsReceived.First != null && hostFromEndpoint.ReliableInOrderPacketsReceived.First.Value.SequenceNumber == hostFromEndpoint.LastRIOPacketDispatched + 1)
									{
										ReliableUDPClient.Packet packet4 = hostFromEndpoint.ReliableInOrderPacketsReceived.First.Value;
										hostFromEndpoint.LastRIOPacketDispatched = packet4.SequenceNumber;
										this.DispatchQueue.Enqueue(packet4);
										hostFromEndpoint.ReliableInOrderPacketsReceived.RemoveFirst();
									}
									else
										goto label_47;
								}
							case ReliableUDPClient.CommandWords.ACK:
								lock (hostFromEndpoint._unacknolegedReliablePackets)
								{
									ReliableUDPClient.Packet packet5;
									if (hostFromEndpoint._unacknolegedReliablePackets.TryGetValue(packet1.AckNumber, out packet5))
									{
										hostFromEndpoint._unacknolegedReliablePackets.Remove(packet1.AckNumber);
										packet5.Release();
									}
									packet1.Release();
									continue;
								}
							case ReliableUDPClient.CommandWords.ACKRIO:
								lock (hostFromEndpoint._unacknolegedReliableInOrderPackets)
								{
									ReliableUDPClient.Packet packet6;
									if (hostFromEndpoint._unacknolegedReliableInOrderPackets.TryGetValue(packet1.AckNumber, out packet6))
									{
										hostFromEndpoint._unacknolegedReliableInOrderPackets.Remove(packet1.AckNumber);
										packet6.Release();
									}
									packet1.Release();
									continue;
								}
							case ReliableUDPClient.CommandWords.NACK:
								flag3 = true;
								packet1.Release();
								continue;
							case ReliableUDPClient.CommandWords.NACKRIO:
								flag4 = true;
								packet1.Release();
								continue;
							default:
								throw new NotImplementedException();
						}
					}
				}
				if (flag1 && hostFromEndpoint.ReliablePacketsReceived.Count > 1)
					this.SendNACK(hostFromEndpoint);
				if (flag2 && hostFromEndpoint.ReliableInOrderPacketsReceived.Count > 0)
					this.SendNACKRIO(hostFromEndpoint);
				if (flag3)
				{
					lock (hostFromEndpoint._unacknolegedReliablePackets)
					{
						foreach (KeyValuePair<int, ReliableUDPClient.Packet> unacknolegedReliablePacket in hostFromEndpoint._unacknolegedReliablePackets)
							unacknolegedReliablePacket.Value.Send(hostFromEndpoint);
					}
				}
				if (!flag4)
					return;
				lock (hostFromEndpoint._unacknolegedReliableInOrderPackets)
				{
					foreach (KeyValuePair<int, ReliableUDPClient.Packet> reliableInOrderPacket in hostFromEndpoint._unacknolegedReliableInOrderPackets)
						reliableInOrderPacket.Value.Send(hostFromEndpoint);
				}
			}
		}

		~ReliableUDPClient()
		{
			if (!this._disposed)
			{
				this.Dispose();
			}
			GC.SuppressFinalize(this);
		}

		public void Dispose()
		{
			if (!this._disposed)
			{
				this._socket.Dispose();
				this._disposed = true;
			}
		}
	}
}
