using System;
using System.Diagnostics;
using System.Net;

namespace DNA.Net.Lidgren
{
	[DebuggerDisplay("RemoteUniqueIdentifier={RemoteUniqueIdentifier} RemoteEndPoint={remoteEndPoint}")]
	public class NetConnection
	{
		private enum ExpandMTUStatus
		{
			None,
			InProgress,
			Finished
		}

		public const string sConnectionTimeoutErrorString = "Failed to establish connection - no response from remote host";

		private const int c_protocolMaxMTU = 8190;

		private const int m_infrequentEventsSkipFrames = 8;

		private const int m_messageCoalesceFrames = 3;

		private float m_sentPingTime;

		private int m_sentPingNumber;

		private float m_averageRoundtripTime;

		private float m_timeoutDeadline = float.MaxValue;

		internal double m_remoteTimeOffset;

		internal bool m_connectRequested;

		internal bool m_disconnectRequested;

		internal bool m_connectionInitiator;

		internal string m_disconnectMessage;

		internal NetIncomingMessage m_remoteHailMessage;

		internal float m_lastHandshakeSendTime;

		internal int m_handshakeAttempts;

		private NetConnection.ExpandMTUStatus m_expandMTUStatus;

		private int m_largestSuccessfulMTU;

		private int m_smallestFailedMTU;

		private int m_lastSentMTUAttemptSize;

		private double m_lastSentMTUAttemptTime;

		private int m_mtuAttemptFails;

		internal int m_currentMTU;

		internal NetPeer m_peer;

		internal NetPeerConfiguration m_peerConfiguration;

		internal NetConnectionStatus m_status;

		internal NetConnectionStatus m_visibleStatus;

		internal IPEndPoint m_remoteEndPoint;

		internal NetSenderChannelBase[] m_sendChannels;

		internal NetReceiverChannelBase[] m_receiveChannels;

		internal NetOutgoingMessage m_localHailMessage;

		internal long m_remoteUniqueIdentifier;

		internal NetQueue<NetTuple<NetMessageType, int>> m_queuedOutgoingAcks;

		internal NetQueue<NetTuple<NetMessageType, int>> m_queuedIncomingAcks;

		private int m_sendBufferWritePtr;

		private int m_sendBufferNumMessages;

		private object m_tag;

		internal NetConnectionStatistics m_statistics;

		public float AverageRoundtripTime
		{
			get
			{
				return this.m_averageRoundtripTime;
			}
		}

		public float RemoteTimeOffset
		{
			get
			{
				return (float)this.m_remoteTimeOffset;
			}
		}

		internal void InitializeRemoteTimeOffset(float remoteSendTime)
		{
			this.m_remoteTimeOffset = (double)remoteSendTime + (double)this.m_averageRoundtripTime / 2.0 - NetTime.Now;
		}

		public double GetLocalTime(double remoteTimestamp)
		{
			return remoteTimestamp - this.m_remoteTimeOffset;
		}

		public double GetRemoteTime(double localTimestamp)
		{
			return localTimestamp + this.m_remoteTimeOffset;
		}

		internal void InitializePing()
		{
			float num = (float)NetTime.Now;
			this.m_sentPingTime = num;
			this.m_sentPingTime -= this.m_peerConfiguration.PingInterval * 0.25f;
			this.m_sentPingTime -= NetRandom.Instance.NextSingle() * (this.m_peerConfiguration.PingInterval * 0.75f);
			this.m_timeoutDeadline = num + this.m_peerConfiguration.m_connectionTimeout * 2f;
			this.SendPing();
		}

		internal void SendPing()
		{
			this.m_sentPingNumber++;
			this.m_sentPingTime = (float)NetTime.Now;
			NetOutgoingMessage netOutgoingMessage = this.m_peer.CreateMessage(1);
			netOutgoingMessage.Write((byte)this.m_sentPingNumber);
			netOutgoingMessage.m_messageType = NetMessageType.Ping;
			int numBytes = netOutgoingMessage.Encode(this.m_peer.m_sendBuffer, 0, 0);
			bool flag;
			this.m_peer.SendPacket(numBytes, this.m_remoteEndPoint, 1, out flag);
		}

		internal void SendPong(int pingNumber)
		{
			NetOutgoingMessage netOutgoingMessage = this.m_peer.CreateMessage(5);
			netOutgoingMessage.Write((byte)pingNumber);
			netOutgoingMessage.Write((float)NetTime.Now);
			netOutgoingMessage.m_messageType = NetMessageType.Pong;
			int numBytes = netOutgoingMessage.Encode(this.m_peer.m_sendBuffer, 0, 0);
			bool flag;
			this.m_peer.SendPacket(numBytes, this.m_remoteEndPoint, 1, out flag);
		}

		internal void ReceivedPong(float now, int pongNumber, float remoteSendTime)
		{
			if ((byte)pongNumber != (byte)this.m_sentPingNumber)
			{
				return;
			}
			this.m_timeoutDeadline = now + this.m_peerConfiguration.m_connectionTimeout;
			float num = now - this.m_sentPingTime;
			double num2 = (double)remoteSendTime + (double)num / 2.0 - (double)now;
			if (this.m_averageRoundtripTime < 0f)
			{
				this.m_remoteTimeOffset = num2;
				this.m_averageRoundtripTime = num;
			}
			else
			{
				this.m_averageRoundtripTime = this.m_averageRoundtripTime * 0.7f + num * 0.3f;
				this.m_remoteTimeOffset = (this.m_remoteTimeOffset * (double)(this.m_sentPingNumber - 1) + num2) / (double)this.m_sentPingNumber;
			}
			float resendDelay = this.GetResendDelay();
			foreach (NetSenderChannelBase netSenderChannelBase in this.m_sendChannels)
			{
				NetReliableSenderChannel netReliableSenderChannel = netSenderChannelBase as NetReliableSenderChannel;
				if (netReliableSenderChannel != null)
				{
					netReliableSenderChannel.m_resendDelay = resendDelay;
				}
			}
			if (this.m_peer.m_configuration.IsMessageTypeEnabled(NetIncomingMessageType.ConnectionLatencyUpdated))
			{
				NetIncomingMessage netIncomingMessage = this.m_peer.CreateIncomingMessage(NetIncomingMessageType.ConnectionLatencyUpdated, 4);
				netIncomingMessage.m_senderConnection = this;
				netIncomingMessage.m_senderEndPoint = this.m_remoteEndPoint;
				netIncomingMessage.Write(num);
				this.m_peer.ReleaseMessage(netIncomingMessage);
			}
		}

		public NetIncomingMessage RemoteHailMessage
		{
			get
			{
				return this.m_remoteHailMessage;
			}
		}

		internal void UnconnectedHeartbeat(float now)
		{
			if (this.m_disconnectRequested)
			{
				this.ExecuteDisconnect(this.m_disconnectMessage, true);
			}
			if (this.m_connectRequested)
			{
				switch (this.m_status)
				{
				case NetConnectionStatus.InitiatedConnect:
					this.SendConnect(now);
					return;
				case NetConnectionStatus.RespondedConnect:
				case NetConnectionStatus.Connected:
					this.ExecuteDisconnect("Reconnecting", true);
					return;
				case NetConnectionStatus.Disconnecting:
					return;
				case NetConnectionStatus.Disconnected:
					throw new NetException("This connection is Disconnected; spent. A new one should have been created");
				}
				this.SendConnect(now);
				return;
			}
			if (now - this.m_lastHandshakeSendTime > this.m_peerConfiguration.m_resendHandshakeInterval)
			{
				if (this.m_handshakeAttempts >= this.m_peerConfiguration.m_maximumHandshakeAttempts)
				{
					this.ExecuteDisconnect("Failed to establish connection - no response from remote host", true);
					return;
				}
				switch (this.m_status)
				{
				case NetConnectionStatus.None:
				case NetConnectionStatus.ReceivedInitiation:
					this.m_peer.LogWarning("Time to resend handshake, but status is " + this.m_status);
					return;
				case NetConnectionStatus.InitiatedConnect:
					this.SendConnect(now);
					return;
				case NetConnectionStatus.RespondedAwaitingApproval:
					this.m_lastHandshakeSendTime = now;
					return;
				case NetConnectionStatus.RespondedConnect:
					this.SendConnectResponse(now, true);
					return;
				default:
					this.m_peer.LogWarning("Time to resend handshake, but status is " + this.m_status);
					break;
				}
			}
		}

		internal void ExecuteDisconnect(string reason, bool sendByeMessage)
		{
			for (int i = 0; i < this.m_sendChannels.Length; i++)
			{
				NetSenderChannelBase netSenderChannelBase = this.m_sendChannels[i];
				if (netSenderChannelBase != null)
				{
					netSenderChannelBase.Reset();
				}
			}
			if (sendByeMessage)
			{
				this.SendDisconnect(reason, true);
			}
			this.SetStatus(NetConnectionStatus.Disconnected, reason);
			lock (this.m_peer.m_handshakes)
			{
				this.m_peer.m_handshakes.Remove(this.m_remoteEndPoint);
			}
			this.m_disconnectRequested = false;
			this.m_connectRequested = false;
			this.m_handshakeAttempts = 0;
		}

		internal void SendConnect(float now)
		{
			int num = 13 + this.m_peerConfiguration.AppIdentifier.Length;
			num += ((this.m_localHailMessage == null) ? 0 : this.m_localHailMessage.LengthBytes);
			NetOutgoingMessage netOutgoingMessage = this.m_peer.CreateMessage(num);
			netOutgoingMessage.m_messageType = NetMessageType.Connect;
			netOutgoingMessage.Write(this.m_peerConfiguration.AppIdentifier);
			netOutgoingMessage.Write(this.m_peer.m_uniqueIdentifier);
			netOutgoingMessage.Write(now);
			this.WriteLocalHail(netOutgoingMessage);
			this.m_peer.SendLibrary(netOutgoingMessage, this.m_remoteEndPoint);
			this.m_connectRequested = false;
			this.m_lastHandshakeSendTime = now;
			this.m_handshakeAttempts++;
			int handshakeAttempts = this.m_handshakeAttempts;
			this.SetStatus(NetConnectionStatus.InitiatedConnect, "Locally requested connect");
		}

		internal void SendConnectResponse(float now, bool onLibraryThread)
		{
			NetOutgoingMessage netOutgoingMessage = this.m_peer.CreateMessage(this.m_peerConfiguration.AppIdentifier.Length + 13 + ((this.m_localHailMessage == null) ? 0 : this.m_localHailMessage.LengthBytes));
			netOutgoingMessage.m_messageType = NetMessageType.ConnectResponse;
			netOutgoingMessage.Write(this.m_peerConfiguration.AppIdentifier);
			netOutgoingMessage.Write(this.m_peer.m_uniqueIdentifier);
			netOutgoingMessage.Write(now);
			this.WriteLocalHail(netOutgoingMessage);
			if (onLibraryThread)
			{
				this.m_peer.SendLibrary(netOutgoingMessage, this.m_remoteEndPoint);
			}
			else
			{
				this.m_peer.m_unsentUnconnectedMessages.Enqueue(new NetTuple<IPEndPoint, NetOutgoingMessage>(this.m_remoteEndPoint, netOutgoingMessage));
			}
			this.m_lastHandshakeSendTime = now;
			this.m_handshakeAttempts++;
			int handshakeAttempts = this.m_handshakeAttempts;
			this.SetStatus(NetConnectionStatus.RespondedConnect, "Remotely requested connect");
		}

		internal void SendDisconnect(string reason, bool onLibraryThread)
		{
			NetOutgoingMessage netOutgoingMessage = this.m_peer.CreateMessage(reason);
			netOutgoingMessage.m_messageType = NetMessageType.Disconnect;
			if (onLibraryThread)
			{
				this.m_peer.SendLibrary(netOutgoingMessage, this.m_remoteEndPoint);
				return;
			}
			this.m_peer.m_unsentUnconnectedMessages.Enqueue(new NetTuple<IPEndPoint, NetOutgoingMessage>(this.m_remoteEndPoint, netOutgoingMessage));
		}

		private void WriteLocalHail(NetOutgoingMessage om)
		{
			if (this.m_localHailMessage != null)
			{
				byte[] data = this.m_localHailMessage.Data;
				if (data != null && data.Length >= this.m_localHailMessage.LengthBytes)
				{
					if (om.LengthBytes + this.m_localHailMessage.LengthBytes > this.m_peerConfiguration.m_maximumTransmissionUnit - 10)
					{
						throw new NetException("Hail message too large; can maximally be " + (this.m_peerConfiguration.m_maximumTransmissionUnit - 10 - om.LengthBytes));
					}
					om.Write(this.m_localHailMessage.Data, 0, this.m_localHailMessage.LengthBytes);
				}
			}
		}

		internal void SendConnectionEstablished()
		{
			NetOutgoingMessage netOutgoingMessage = this.m_peer.CreateMessage(4);
			netOutgoingMessage.m_messageType = NetMessageType.ConnectionEstablished;
			netOutgoingMessage.Write((float)NetTime.Now);
			this.m_peer.SendLibrary(netOutgoingMessage, this.m_remoteEndPoint);
			this.m_handshakeAttempts = 0;
			this.InitializePing();
			if (this.m_status != NetConnectionStatus.Connected)
			{
				this.SetStatus(NetConnectionStatus.Connected, "Connected to " + NetUtility.ToHexString(this.m_remoteUniqueIdentifier));
			}
		}

		public void Approve()
		{
			if (this.m_status != NetConnectionStatus.RespondedAwaitingApproval)
			{
				this.m_peer.LogWarning("Approve() called in wrong status; expected RespondedAwaitingApproval; got " + this.m_status);
				return;
			}
			this.m_localHailMessage = null;
			this.m_handshakeAttempts = 0;
			this.SendConnectResponse((float)NetTime.Now, false);
		}

		public void Approve(NetOutgoingMessage localHail)
		{
			if (this.m_status != NetConnectionStatus.RespondedAwaitingApproval)
			{
				this.m_peer.LogWarning("Approve() called in wrong status; expected RespondedAwaitingApproval; got " + this.m_status);
				return;
			}
			this.m_localHailMessage = localHail;
			this.m_handshakeAttempts = 0;
			this.SendConnectResponse((float)NetTime.Now, false);
		}

		public void Deny()
		{
			this.Deny(string.Empty);
		}

		public void Deny(string reason)
		{
			this.SendDisconnect(reason, false);
			this.m_peer.m_handshakes.Remove(this.m_remoteEndPoint);
		}

		internal void ReceivedHandshake(double now, NetMessageType tp, int ptr, int payloadLength)
		{
			switch (tp)
			{
			case NetMessageType.Ping:
			case NetMessageType.Pong:
			case NetMessageType.Acknowledge:
				break;
			case NetMessageType.Connect:
				if (this.m_status == NetConnectionStatus.ReceivedInitiation)
				{
					byte[] array;
					bool flag = this.ValidateHandshakeData(ptr, payloadLength, out array);
					if (flag)
					{
						if (array != null)
						{
							this.m_remoteHailMessage = this.m_peer.CreateIncomingMessage(NetIncomingMessageType.Data, array);
							this.m_remoteHailMessage.LengthBits = array.Length * 8;
						}
						else
						{
							this.m_remoteHailMessage = null;
						}
						if (this.m_peerConfiguration.IsMessageTypeEnabled(NetIncomingMessageType.ConnectionApproval))
						{
							NetIncomingMessage netIncomingMessage = this.m_peer.CreateIncomingMessage(NetIncomingMessageType.ConnectionApproval, (this.m_remoteHailMessage == null) ? 0 : this.m_remoteHailMessage.LengthBytes);
							netIncomingMessage.m_receiveTime = now;
							netIncomingMessage.m_senderConnection = this;
							netIncomingMessage.m_senderEndPoint = this.m_remoteEndPoint;
							if (this.m_remoteHailMessage != null)
							{
								netIncomingMessage.Write(this.m_remoteHailMessage.m_data, 0, this.m_remoteHailMessage.LengthBytes);
							}
							this.SetStatus(NetConnectionStatus.RespondedAwaitingApproval, "Awaiting approval");
							this.m_peer.ReleaseMessage(netIncomingMessage);
							return;
						}
						this.SendConnectResponse((float)now, true);
					}
					return;
				}
				if (this.m_status == NetConnectionStatus.RespondedAwaitingApproval)
				{
					this.m_peer.LogWarning("Ignoring multiple Connect() most likely due to a delayed Approval");
					return;
				}
				if (this.m_status == NetConnectionStatus.RespondedConnect)
				{
					this.SendConnectResponse((float)now, true);
					return;
				}
				break;
			case NetMessageType.ConnectResponse:
				switch (this.m_status)
				{
				case NetConnectionStatus.None:
				case NetConnectionStatus.ReceivedInitiation:
				case NetConnectionStatus.RespondedAwaitingApproval:
				case NetConnectionStatus.RespondedConnect:
				case NetConnectionStatus.Disconnecting:
				case NetConnectionStatus.Disconnected:
					break;
				case NetConnectionStatus.InitiatedConnect:
				{
					byte[] array;
					bool flag2 = this.ValidateHandshakeData(ptr, payloadLength, out array);
					if (flag2)
					{
						if (array != null)
						{
							this.m_remoteHailMessage = this.m_peer.CreateIncomingMessage(NetIncomingMessageType.Data, array);
							this.m_remoteHailMessage.LengthBits = array.Length * 8;
						}
						else
						{
							this.m_remoteHailMessage = null;
						}
						this.m_peer.AcceptConnection(this);
						this.SendConnectionEstablished();
						return;
					}
					break;
				}
				case NetConnectionStatus.Connected:
					this.SendConnectionEstablished();
					return;
				default:
					return;
				}
				break;
			case NetMessageType.ConnectionEstablished:
				switch (this.m_status)
				{
				case NetConnectionStatus.None:
				case NetConnectionStatus.InitiatedConnect:
				case NetConnectionStatus.ReceivedInitiation:
				case NetConnectionStatus.RespondedAwaitingApproval:
				case NetConnectionStatus.Connected:
				case NetConnectionStatus.Disconnecting:
				case NetConnectionStatus.Disconnected:
					break;
				case NetConnectionStatus.RespondedConnect:
				{
					NetIncomingMessage netIncomingMessage2 = this.m_peer.SetupReadHelperMessage(ptr, payloadLength);
					this.InitializeRemoteTimeOffset(netIncomingMessage2.ReadSingle());
					this.m_peer.AcceptConnection(this);
					this.InitializePing();
					this.SetStatus(NetConnectionStatus.Connected, "Connected to " + NetUtility.ToHexString(this.m_remoteUniqueIdentifier));
					return;
				}
				default:
					return;
				}
				break;
			case NetMessageType.Disconnect:
			{
				string reason = "Ouch";
				try
				{
					NetIncomingMessage netIncomingMessage3 = this.m_peer.SetupReadHelperMessage(ptr, payloadLength);
					reason = netIncomingMessage3.ReadString();
				}
				catch
				{
				}
				this.ExecuteDisconnect(reason, false);
				return;
			}
			case NetMessageType.Discovery:
				this.m_peer.HandleIncomingDiscoveryRequest(now, this.m_remoteEndPoint, ptr, payloadLength);
				return;
			case NetMessageType.DiscoveryResponse:
				this.m_peer.HandleIncomingDiscoveryResponse(now, this.m_remoteEndPoint, ptr, payloadLength);
				break;
			default:
				return;
			}
		}

		private bool ValidateHandshakeData(int ptr, int payloadLength, out byte[] hail)
		{
			hail = null;
			NetIncomingMessage netIncomingMessage = this.m_peer.SetupReadHelperMessage(ptr, payloadLength);
			try
			{
				string a = netIncomingMessage.ReadString();
				long remoteUniqueIdentifier = netIncomingMessage.ReadInt64();
				this.InitializeRemoteTimeOffset(netIncomingMessage.ReadSingle());
				int num = payloadLength - (netIncomingMessage.PositionInBytes - ptr);
				if (num > 0)
				{
					hail = netIncomingMessage.ReadBytes(num);
				}
				if (a != this.m_peer.m_configuration.AppIdentifier)
				{
					this.ExecuteDisconnect("Wrong application identifier!", true);
					return false;
				}
				this.m_remoteUniqueIdentifier = remoteUniqueIdentifier;
			}
			catch (Exception ex)
			{
				this.ExecuteDisconnect("Handshake data validation failed", true);
				this.m_peer.LogWarning("ReadRemoteHandshakeData failed: " + ex.Message);
				return false;
			}
			return true;
		}

		public void Disconnect(string byeMessage)
		{
			if (this.m_status == NetConnectionStatus.None || this.m_status == NetConnectionStatus.Disconnected)
			{
				return;
			}
			this.m_disconnectMessage = byeMessage;
			if (this.m_status != NetConnectionStatus.Disconnected && this.m_status != NetConnectionStatus.None)
			{
				this.SetStatus(NetConnectionStatus.Disconnecting, byeMessage);
			}
			this.m_handshakeAttempts = 0;
			this.m_disconnectRequested = true;
		}

		internal void InitExpandMTU(double now)
		{
			this.m_lastSentMTUAttemptTime = now + (double)this.m_peerConfiguration.m_expandMTUFrequency + 1.5 + (double)this.m_averageRoundtripTime;
			this.m_largestSuccessfulMTU = 512;
			this.m_smallestFailedMTU = -1;
			this.m_currentMTU = this.m_peerConfiguration.MaximumTransmissionUnit;
		}

		private void MTUExpansionHeartbeat(double now)
		{
			if (this.m_expandMTUStatus == NetConnection.ExpandMTUStatus.Finished)
			{
				return;
			}
			if (this.m_expandMTUStatus != NetConnection.ExpandMTUStatus.None)
			{
				if (now > this.m_lastSentMTUAttemptTime + (double)this.m_peerConfiguration.ExpandMTUFrequency)
				{
					this.m_mtuAttemptFails++;
					if (this.m_mtuAttemptFails == 3)
					{
						this.FinalizeMTU(this.m_currentMTU);
						return;
					}
					this.m_smallestFailedMTU = this.m_lastSentMTUAttemptSize;
					this.ExpandMTU(now, false);
				}
				return;
			}
			if (!this.m_peerConfiguration.m_autoExpandMTU)
			{
				this.FinalizeMTU(this.m_currentMTU);
				return;
			}
			this.ExpandMTU(now, true);
		}

		private void ExpandMTU(double now, bool succeeded)
		{
			int num;
			if (this.m_smallestFailedMTU == -1)
			{
				num = (int)((float)this.m_currentMTU * 1.25f);
			}
			else
			{
				num = (int)(((float)this.m_smallestFailedMTU + (float)this.m_largestSuccessfulMTU) / 2f);
			}
			if (num > 8190)
			{
				num = 8190;
			}
			if (num == this.m_largestSuccessfulMTU)
			{
				this.FinalizeMTU(this.m_largestSuccessfulMTU);
				return;
			}
			this.SendExpandMTU(now, num);
		}

		private void SendExpandMTU(double now, int size)
		{
			NetOutgoingMessage netOutgoingMessage = this.m_peer.CreateMessage(size);
			byte[] source = new byte[size];
			netOutgoingMessage.Write(source);
			netOutgoingMessage.m_messageType = NetMessageType.ExpandMTURequest;
			int numBytes = netOutgoingMessage.Encode(this.m_peer.m_sendBuffer, 0, 0);
			if (!this.m_peer.SendMTUPacket(numBytes, this.m_remoteEndPoint))
			{
				if (this.m_smallestFailedMTU == -1 || size < this.m_smallestFailedMTU)
				{
					this.m_smallestFailedMTU = size;
					this.m_mtuAttemptFails++;
					if (this.m_mtuAttemptFails >= this.m_peerConfiguration.ExpandMTUFailAttempts)
					{
						this.FinalizeMTU(this.m_largestSuccessfulMTU);
						return;
					}
				}
				this.ExpandMTU(now, false);
				return;
			}
			this.m_lastSentMTUAttemptSize = size;
			this.m_lastSentMTUAttemptTime = now;
		}

		private void FinalizeMTU(int size)
		{
			if (this.m_expandMTUStatus == NetConnection.ExpandMTUStatus.Finished)
			{
				return;
			}
			this.m_expandMTUStatus = NetConnection.ExpandMTUStatus.Finished;
			this.m_currentMTU = size;
			int currentMTU = this.m_currentMTU;
			int maximumTransmissionUnit = this.m_peerConfiguration.m_maximumTransmissionUnit;
		}

		private void SendMTUSuccess(int size)
		{
			NetOutgoingMessage netOutgoingMessage = this.m_peer.CreateMessage(4);
			netOutgoingMessage.Write(size);
			netOutgoingMessage.m_messageType = NetMessageType.ExpandMTUSuccess;
			int numBytes = netOutgoingMessage.Encode(this.m_peer.m_sendBuffer, 0, 0);
			bool flag;
			this.m_peer.SendPacket(numBytes, this.m_remoteEndPoint, 1, out flag);
		}

		private void HandleExpandMTUSuccess(double now, int size)
		{
			if (size > this.m_largestSuccessfulMTU)
			{
				this.m_largestSuccessfulMTU = size;
			}
			if (size < this.m_currentMTU)
			{
				return;
			}
			this.m_currentMTU = size;
			this.ExpandMTU(now, true);
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

		public NetPeer Peer
		{
			get
			{
				return this.m_peer;
			}
		}

		public NetConnectionStatus Status
		{
			get
			{
				return this.m_visibleStatus;
			}
		}

		public NetConnectionStatistics Statistics
		{
			get
			{
				return this.m_statistics;
			}
		}

		public IPEndPoint RemoteEndPoint
		{
			get
			{
				return this.m_remoteEndPoint;
			}
		}

		public long RemoteUniqueIdentifier
		{
			get
			{
				return this.m_remoteUniqueIdentifier;
			}
		}

		public NetOutgoingMessage LocalHailMessage
		{
			get
			{
				return this.m_localHailMessage;
			}
		}

		internal float GetResendDelay()
		{
			float num = this.m_averageRoundtripTime;
			if (num <= 0f)
			{
				num = 0.1f;
			}
			return 0.02f + num * 2f;
		}

		internal NetConnection(NetPeer peer, IPEndPoint remoteEndPoint)
		{
			this.m_peer = peer;
			this.m_peerConfiguration = this.m_peer.Configuration;
			this.m_status = NetConnectionStatus.None;
			this.m_visibleStatus = NetConnectionStatus.None;
			this.m_remoteEndPoint = remoteEndPoint;
			this.m_sendChannels = new NetSenderChannelBase[99];
			this.m_receiveChannels = new NetReceiverChannelBase[99];
			this.m_queuedOutgoingAcks = new NetQueue<NetTuple<NetMessageType, int>>(4);
			this.m_queuedIncomingAcks = new NetQueue<NetTuple<NetMessageType, int>>(4);
			this.m_statistics = new NetConnectionStatistics(this);
			this.m_averageRoundtripTime = -1f;
			this.m_currentMTU = this.m_peerConfiguration.MaximumTransmissionUnit;
		}

		internal void MutateEndPoint(IPEndPoint endPoint)
		{
			this.m_remoteEndPoint = endPoint;
		}

		internal void SetStatus(NetConnectionStatus status, string reason)
		{
			if (status == this.m_status)
			{
				return;
			}
			this.m_status = status;
			if (reason == null)
			{
				reason = string.Empty;
			}
			if (this.m_status == NetConnectionStatus.Connected)
			{
				this.m_timeoutDeadline = (float)NetTime.Now + this.m_peerConfiguration.m_connectionTimeout;
			}
			if (this.m_peerConfiguration.IsMessageTypeEnabled(NetIncomingMessageType.StatusChanged))
			{
				NetIncomingMessage netIncomingMessage = this.m_peer.CreateIncomingMessage(NetIncomingMessageType.StatusChanged, 4 + reason.Length + ((reason.Length > 126) ? 2 : 1));
				netIncomingMessage.m_senderConnection = this;
				netIncomingMessage.m_senderEndPoint = this.m_remoteEndPoint;
				netIncomingMessage.Write((byte)this.m_status);
				netIncomingMessage.Write(reason);
				this.m_peer.ReleaseMessage(netIncomingMessage);
				return;
			}
			this.m_visibleStatus = this.m_status;
		}

		internal void Heartbeat(float now, uint frameCounter)
		{
			if (frameCounter % 8U == 0U)
			{
				if (now > this.m_timeoutDeadline)
				{
					this.ExecuteDisconnect("Connection timed out", true);
				}
				if (this.m_status == NetConnectionStatus.Connected)
				{
					if (now > this.m_sentPingTime + this.m_peer.m_configuration.m_pingInterval)
					{
						this.SendPing();
					}
					this.MTUExpansionHeartbeat((double)now);
				}
				if (this.m_disconnectRequested)
				{
					this.ExecuteDisconnect(this.m_disconnectMessage, true);
					return;
				}
			}
			byte[] sendBuffer = this.m_peer.m_sendBuffer;
			int currentMTU = this.m_currentMTU;
			if (frameCounter % 3U == 0U)
			{
				while (this.m_queuedOutgoingAcks.Count > 0)
				{
					int num = (currentMTU - (this.m_sendBufferWritePtr + 5)) / 3;
					if (num > this.m_queuedOutgoingAcks.Count)
					{
						num = this.m_queuedOutgoingAcks.Count;
					}
					this.m_sendBufferNumMessages++;
					sendBuffer[this.m_sendBufferWritePtr++] = 134;
					sendBuffer[this.m_sendBufferWritePtr++] = 0;
					sendBuffer[this.m_sendBufferWritePtr++] = 0;
					int num2 = num * 3 * 8;
					sendBuffer[this.m_sendBufferWritePtr++] = (byte)num2;
					sendBuffer[this.m_sendBufferWritePtr++] = (byte)(num2 >> 8);
					for (int i = 0; i < num; i++)
					{
						NetTuple<NetMessageType, int> netTuple;
						this.m_queuedOutgoingAcks.TryDequeue(out netTuple);
						sendBuffer[this.m_sendBufferWritePtr++] = (byte)netTuple.Item1;
						sendBuffer[this.m_sendBufferWritePtr++] = (byte)netTuple.Item2;
						sendBuffer[this.m_sendBufferWritePtr++] = (byte)(netTuple.Item2 >> 8);
					}
					if (this.m_queuedOutgoingAcks.Count > 0)
					{
						bool flag;
						this.m_peer.SendPacket(this.m_sendBufferWritePtr, this.m_remoteEndPoint, this.m_sendBufferNumMessages, out flag);
						this.m_sendBufferWritePtr = 0;
						this.m_sendBufferNumMessages = 0;
					}
				}
				NetTuple<NetMessageType, int> netTuple2;
				while (this.m_queuedIncomingAcks.TryDequeue(out netTuple2))
				{
					NetSenderChannelBase netSenderChannelBase = this.m_sendChannels[(int)(netTuple2.Item1 - NetMessageType.UserUnreliable)];
					if (netSenderChannelBase == null)
					{
						netSenderChannelBase = this.CreateSenderChannel(netTuple2.Item1);
					}
					netSenderChannelBase.ReceiveAcknowledge(now, netTuple2.Item2);
				}
			}
			if (this.m_peer.m_executeFlushSendQueue)
			{
				for (int j = this.m_sendChannels.Length - 1; j >= 0; j--)
				{
					NetSenderChannelBase netSenderChannelBase2 = this.m_sendChannels[j];
					if (netSenderChannelBase2 != null)
					{
						netSenderChannelBase2.SendQueuedMessages(now);
					}
				}
			}
			if (this.m_sendBufferWritePtr > 0)
			{
				bool flag;
				this.m_peer.SendPacket(this.m_sendBufferWritePtr, this.m_remoteEndPoint, this.m_sendBufferNumMessages, out flag);
				this.m_sendBufferWritePtr = 0;
				this.m_sendBufferNumMessages = 0;
			}
		}

		internal void QueueSendMessage(NetOutgoingMessage om, int seqNr)
		{
			int encodedSize = om.GetEncodedSize();
			if (encodedSize > this.m_currentMTU)
			{
				this.m_peer.LogWarning("Message larger than MTU! Fragmentation must have failed!");
			}
			if (this.m_sendBufferWritePtr + encodedSize > this.m_currentMTU)
			{
				bool flag;
				this.m_peer.SendPacket(this.m_sendBufferWritePtr, this.m_remoteEndPoint, this.m_sendBufferNumMessages, out flag);
				this.m_sendBufferWritePtr = 0;
				this.m_sendBufferNumMessages = 0;
			}
			this.m_sendBufferWritePtr = om.Encode(this.m_peer.m_sendBuffer, this.m_sendBufferWritePtr, seqNr);
			this.m_sendBufferNumMessages++;
		}

		public NetSendResult SendMessage(NetOutgoingMessage msg, NetDeliveryMethod method, int sequenceChannel)
		{
			return this.m_peer.SendMessage(msg, this, method, sequenceChannel);
		}

		internal NetSendResult EnqueueMessage(NetOutgoingMessage msg, NetDeliveryMethod method, int sequenceChannel)
		{
			if (this.m_status != NetConnectionStatus.Connected)
			{
				return NetSendResult.FailedNotConnected;
			}
			NetMessageType netMessageType = (NetMessageType)(method + (byte)sequenceChannel);
			msg.m_messageType = netMessageType;
			int num = (int)(method - NetDeliveryMethod.Unreliable) + sequenceChannel;
			NetSenderChannelBase netSenderChannelBase = this.m_sendChannels[num];
			if (netSenderChannelBase == null)
			{
				netSenderChannelBase = this.CreateSenderChannel(netMessageType);
			}
			if (msg.GetEncodedSize() > this.m_currentMTU)
			{
				throw new NetException("Message too large! Fragmentation failure?");
			}
			NetSendResult netSendResult = netSenderChannelBase.Enqueue(msg);
			if (netSendResult == NetSendResult.Sent && !this.m_peerConfiguration.m_autoFlushSendQueue)
			{
				netSendResult = NetSendResult.Queued;
			}
			return netSendResult;
		}

		private NetSenderChannelBase CreateSenderChannel(NetMessageType tp)
		{
			NetSenderChannelBase netSenderChannelBase;
			lock (this.m_sendChannels)
			{
				NetDeliveryMethod deliveryMethod = NetUtility.GetDeliveryMethod(tp);
				int num = (int)(tp - (NetMessageType)deliveryMethod);
				int num2 = (int)(deliveryMethod - NetDeliveryMethod.Unreliable) + num;
				if (this.m_sendChannels[num2] != null)
				{
					netSenderChannelBase = this.m_sendChannels[num2];
				}
				else
				{
					NetDeliveryMethod netDeliveryMethod = deliveryMethod;
					switch (netDeliveryMethod)
					{
					case NetDeliveryMethod.Unreliable:
					case NetDeliveryMethod.UnreliableSequenced:
						netSenderChannelBase = new NetUnreliableSenderChannel(this, NetUtility.GetWindowSize(deliveryMethod));
						break;
					default:
						switch (netDeliveryMethod)
						{
						case NetDeliveryMethod.ReliableUnordered:
						case NetDeliveryMethod.ReliableSequenced:
							break;
						default:
							if (netDeliveryMethod == NetDeliveryMethod.ReliableOrdered)
							{
								netSenderChannelBase = new NetReliableSenderChannel(this, NetUtility.GetWindowSize(deliveryMethod));
								goto IL_92;
							}
							break;
						}
						netSenderChannelBase = new NetReliableSenderChannel(this, NetUtility.GetWindowSize(deliveryMethod));
						break;
					}
					IL_92:
					this.m_sendChannels[num2] = netSenderChannelBase;
				}
			}
			return netSenderChannelBase;
		}

		internal void ReceivedLibraryMessage(NetMessageType tp, int ptr, int payloadLength)
		{
			float num = (float)NetTime.Now;
			switch (tp)
			{
			case NetMessageType.Ping:
			{
				int pingNumber = (int)this.m_peer.m_receiveBuffer[ptr++];
				this.SendPong(pingNumber);
				return;
			}
			case NetMessageType.Pong:
			{
				NetIncomingMessage netIncomingMessage = this.m_peer.SetupReadHelperMessage(ptr, payloadLength);
				int pongNumber = (int)netIncomingMessage.ReadByte();
				float remoteSendTime = netIncomingMessage.ReadSingle();
				this.ReceivedPong(num, pongNumber, remoteSendTime);
				return;
			}
			case NetMessageType.Acknowledge:
				for (int i = 0; i < payloadLength; i += 3)
				{
					NetMessageType item = (NetMessageType)this.m_peer.m_receiveBuffer[ptr++];
					int num2 = (int)this.m_peer.m_receiveBuffer[ptr++];
					num2 |= (int)this.m_peer.m_receiveBuffer[ptr++] << 8;
					this.m_queuedIncomingAcks.Enqueue(new NetTuple<NetMessageType, int>(item, num2));
				}
				return;
			case NetMessageType.Disconnect:
			{
				NetIncomingMessage netIncomingMessage2 = this.m_peer.SetupReadHelperMessage(ptr, payloadLength);
				this.ExecuteDisconnect(netIncomingMessage2.ReadString(), false);
				return;
			}
			case NetMessageType.NatIntroduction:
				this.m_peer.HandleNatIntroduction(ptr);
				return;
			case NetMessageType.ExpandMTURequest:
				this.SendMTUSuccess(payloadLength);
				return;
			case NetMessageType.ExpandMTUSuccess:
			{
				NetIncomingMessage netIncomingMessage3 = this.m_peer.SetupReadHelperMessage(ptr, payloadLength);
				int size = netIncomingMessage3.ReadInt32();
				this.HandleExpandMTUSuccess((double)num, size);
				return;
			}
			}
			this.m_peer.LogWarning("Connection received unhandled library message: " + tp);
		}

		internal void ReceivedMessage(NetIncomingMessage msg)
		{
			NetMessageType receivedMessageType = msg.m_receivedMessageType;
			int num = (int)(receivedMessageType - NetMessageType.UserUnreliable);
			NetReceiverChannelBase netReceiverChannelBase = this.m_receiveChannels[num];
			if (netReceiverChannelBase == null)
			{
				netReceiverChannelBase = this.CreateReceiverChannel(receivedMessageType);
			}
			netReceiverChannelBase.ReceiveMessage(msg);
		}

		private NetReceiverChannelBase CreateReceiverChannel(NetMessageType tp)
		{
			NetDeliveryMethod deliveryMethod = NetUtility.GetDeliveryMethod(tp);
			NetDeliveryMethod netDeliveryMethod = deliveryMethod;
			NetReceiverChannelBase netReceiverChannelBase;
			switch (netDeliveryMethod)
			{
			case NetDeliveryMethod.Unreliable:
				netReceiverChannelBase = new NetUnreliableUnorderedReceiver(this);
				break;
			case NetDeliveryMethod.UnreliableSequenced:
				netReceiverChannelBase = new NetUnreliableSequencedReceiver(this);
				break;
			default:
				switch (netDeliveryMethod)
				{
				case NetDeliveryMethod.ReliableUnordered:
					netReceiverChannelBase = new NetReliableUnorderedReceiver(this, 64);
					break;
				case NetDeliveryMethod.ReliableSequenced:
					netReceiverChannelBase = new NetReliableSequencedReceiver(this, 64);
					break;
				default:
					if (netDeliveryMethod != NetDeliveryMethod.ReliableOrdered)
					{
						throw new NetException("Unhandled NetDeliveryMethod!");
					}
					netReceiverChannelBase = new NetReliableOrderedReceiver(this, 64);
					break;
				}
				break;
			}
			int num = (int)(tp - NetMessageType.UserUnreliable);
			this.m_receiveChannels[num] = netReceiverChannelBase;
			return netReceiverChannelBase;
		}

		internal void QueueAck(NetMessageType tp, int sequenceNumber)
		{
			this.m_queuedOutgoingAcks.Enqueue(new NetTuple<NetMessageType, int>(tp, sequenceNumber));
		}

		public void GetSendQueueInfo(NetDeliveryMethod method, int sequenceChannel, out int windowSize, out int freeWindowSlots)
		{
			int num = (int)(method - NetDeliveryMethod.Unreliable) + sequenceChannel;
			NetSenderChannelBase netSenderChannelBase = this.m_sendChannels[num];
			if (netSenderChannelBase == null)
			{
				windowSize = NetUtility.GetWindowSize(method);
				freeWindowSlots = windowSize;
				return;
			}
			windowSize = netSenderChannelBase.WindowSize;
			freeWindowSlots = netSenderChannelBase.GetAllowedSends() - netSenderChannelBase.m_queuedSends.Count;
		}

		internal void Shutdown(string reason)
		{
			this.ExecuteDisconnect(reason, true);
		}

		public override string ToString()
		{
			return "[NetConnection to " + this.m_remoteEndPoint + "]";
		}
	}
}
