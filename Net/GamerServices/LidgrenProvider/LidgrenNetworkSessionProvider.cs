using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using DNA.Net.Lidgren;
using DNA.Net.MatchMaking;

namespace DNA.Net.GamerServices.LidgrenProvider
{
	public class LidgrenNetworkSessionProvider : NetworkSessionProvider
	{
		protected NetPeer _netSession;
		protected int _sessionID;

		private List<NetworkGamer> _hostConnectedGamerList = 
			new List<NetworkGamer>();

		private Stopwatch debugStopwatch;

		public static NetworkSession CreateNetworkSession(
			NetworkSessionStaticProvider staticprovider)
		{
			LidgrenNetworkSessionProvider lidgrenNetworkSessionProvider = 
				new LidgrenNetworkSessionProvider(staticprovider);
			NetworkSession networkSession = new NetworkSession(lidgrenNetworkSessionProvider);
			lidgrenNetworkSessionProvider._networkSession = networkSession;
			return networkSession;
		}

		protected LidgrenNetworkSessionProvider(NetworkSessionStaticProvider staticProvider) 
			: base(staticProvider) {}

		public override void ReportClientJoined(string username)
		{
			try
			{
				this._staticProvider.NetworkSessionServices.ReportClientJoined(
					this.HostSessionInfo, username);
			}
			catch
			{
				// Swallow exceptions.
			}
		}

		public override void ReportClientLeft(string username)
		{
			try
			{
				this._staticProvider.NetworkSessionServices.ReportClientLeft(
					this.HostSessionInfo, username);
			}
			catch
			{
				// Swallow exceptions.
			}
		}

		public override void ReportSessionAlive()
		{
			try
			{
				this._staticProvider.NetworkSessionServices.ReportSessionAlive(
					this.HostSessionInfo);
			}
			catch
			{
				// Swallow exceptions.
			}
		}

		public override void UpdateHostSessionJoinPolicy(JoinGamePolicy joinGamePolicy)
		{
			this.UpdateHostSession(null, null, new bool?(
				joinGamePolicy == JoinGamePolicy.Anyone), null);
		}

		public override void UpdateHostSession(string serverName, 
											   bool? passwordProtected, bool? isPublic, 
											   NetworkSessionProperties sessionProps)
		{
			try
			{
				if (!string.IsNullOrWhiteSpace(serverName))
				{
					this.HostSessionInfo.Name = serverName;
				}
				
				if (passwordProtected != null)
				{
					this.HostSessionInfo.PasswordProtected = passwordProtected.Value;
				}
				
				if (isPublic != null)
				{
					this.HostSessionInfo.IsPublic = isPublic.Value;
				}

				if (sessionProps != null)
				{
					this.HostSessionInfo.SessionProperties = sessionProps;
				}

				this._staticProvider.NetworkSessionServices.UpdateHostSession(
					this.HostSessionInfo);
			}
			catch
			{
				// Swallow exceptions.
			}
		}

		public override void CloseNetworkSession()
		{
			try
			{
				this._staticProvider.NetworkSessionServices.CloseNetworkSession(
					this.HostSessionInfo);
			}
			catch
			{
				// Swallow exceptions.
			}
		}

		protected NetworkGamer AddRemoteGamer(Gamer gmr, NetConnection connection, 
											  bool isHost, byte playerGID)
		{
			NetworkGamer networkGamer = base.AddRemoteGamer(
				gmr, connection.m_remoteEndPoint.Address, isHost, playerGID);
			
			connection.Tag = networkGamer;
			networkGamer.NetConnectionObject = connection;
			return networkGamer;
		}

		public override void StartHost(NetworkSessionStaticProvider.BeginCreateSessionState sqs)
		{
			this._isHost = true;
			this._sessionID = MathTools.RandomInt();
			this._sessionType = sqs.SessionType;
			this._maxPlayers = sqs.MaxPlayers;
			this._signedInGamers = new List<SignedInGamer>(sqs.LocalGamers);
			this._gameName = sqs.NetworkGameName;
			this._properties = sqs.Properties;
			this._version = sqs.Version;
		
			if (!string.IsNullOrWhiteSpace(this._password))
			{
				this._password = sqs.Password;
			}
		
			if (this._sessionType != NetworkSessionType.Local)
			{
				NetPeerConfiguration netPeerConfiguration = 
					new NetPeerConfiguration(this._gameName);
				
				netPeerConfiguration.Port = this._staticProvider.DefaultPort;
				netPeerConfiguration.AcceptIncomingConnections = true;
				netPeerConfiguration.MaximumConnections = sqs.MaxPlayers;
				netPeerConfiguration.NetworkThreadName = "Lidgren Network Host Thread";
				netPeerConfiguration.UseMessageRecycling = true;
				netPeerConfiguration.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
				netPeerConfiguration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
				netPeerConfiguration.EnableMessageType(NetIncomingMessageType.StatusChanged);
				netPeerConfiguration.EnableUPnP = true;
				
				try
				{
					this._netSession = new NetPeer(netPeerConfiguration);
					this._netSession.Start();
				}
				catch (Exception exceptionEncountered)
				{
					sqs.ExceptionEncountered = exceptionEncountered;
					this._hostConnectionResult = NetworkSession.ResultCode.ExceptionThrown;
					this._netSession = null;
					return;
				}
				
				this._netSession.UPnP.ForwardPort(this._netSession.Port, this._gameName);
				
				try
				{
					IPAddress ipaddress = null;
				
					if (this._netSession.UPnP != null && 
						this._netSession.UPnP.Status != UPnPStatus.NotAvailable)
					{
						ipaddress = this._netSession.UPnP.GetExternalIP();
					}
				
					if (ipaddress == null)
					{
						this._externalIPString = LidgrenExtensions.GetPublicIP();
						
						if (IPAddress.TryParse(this._externalIPString, out ipaddress))
						{
							this._externalIPString = 
								this._externalIPString + ":" + this._netSession.Port.ToString();
						}
					}
					else
					{
						this._externalIPString = 
							ipaddress.ToString() + ":" + this._netSession.Port.ToString();
					}
				}
				catch
				{
					this._externalIPString = CommonResources.Address_not_available;
				}
				
				try
				{
					IPAddress ipaddress = null;
					this._internalIPString = LidgrenExtensions.GetLanIPAddress();
				
					if (IPAddress.TryParse(this._internalIPString, out ipaddress))
					{
						this._internalIPString = 
							this._internalIPString + ":" + this._netSession.Port.ToString();
					}
				}
				catch
				{
					this._internalIPString = CommonResources.Address_not_available;
				}
				
				CreateSessionInfo createSessionInfo = new CreateSessionInfo();
				createSessionInfo.MaxPlayers = this._maxPlayers;
				createSessionInfo.Name = sqs.ServerMessage;
				createSessionInfo.NetworkPort = this._netSession.Port;
				
				createSessionInfo.PasswordProtected = 
					!string.IsNullOrWhiteSpace(this._password);
				
				createSessionInfo.SessionProperties = this.SessionProperties;
				createSessionInfo.JoinGamePolicy = JoinGamePolicy.Anyone;
				createSessionInfo.IsPublic = true;
				
				try
				{
					this.HostSessionInfo = 
						this._staticProvider.NetworkSessionServices.CreateNetworkSession(
							createSessionInfo);
					
					this._hostConnectionResult = NetworkSession.ResultCode.Succeeded;
					base.AddLocalGamer(this._signedInGamers[0], true, 0);
				}
				catch (Exception exceptionEncountered2)
				{
					sqs.ExceptionEncountered = exceptionEncountered2;
					this._hostConnectionResult = NetworkSession.ResultCode.ExceptionThrown;
					this._netSession = null;
					return;
				}
			}

			this._netSession = null;
			this._hostConnectionResult = NetworkSession.ResultCode.Succeeded;
			base.AddLocalGamer(this._signedInGamers[0], true, 0);
		}

		public override void StartClient(NetworkSessionStaticProvider.BeginJoinSessionState sqs)
		{
			this._isHost = false;
			this._sessionType = sqs.SessionType;
			this._sessionID = sqs.AvailableSession.SessionID;
			this._properties = sqs.AvailableSession.SessionProperties;
			this._maxPlayers = sqs.AvailableSession.MaxGamerCount;
			this._signedInGamers = new List<SignedInGamer>(sqs.LocalGamers);
			this._gameName = sqs.NetworkGameName;
			this._version = sqs.Version;
			this._hostConnectionResult = NetworkSession.ResultCode.Pending;
			
			if (this._sessionType != NetworkSessionType.Local)
			{
				NetPeerConfiguration netPeerConfiguration = 
					new NetPeerConfiguration(this._gameName);
			
				netPeerConfiguration.AcceptIncomingConnections = false;
				netPeerConfiguration.MaximumConnections = 1;
				netPeerConfiguration.NetworkThreadName = "Lidgren Network Client Thread";
				netPeerConfiguration.UseMessageRecycling = true;
				netPeerConfiguration.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
				netPeerConfiguration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
				netPeerConfiguration.EnableMessageType(NetIncomingMessageType.StatusChanged);
				this._netSession = new NetPeer(netPeerConfiguration);
				this._netSession.Start();
				Thread.Sleep(100);
				
				RequestConnectToHostMessage requestConnectToHostMessage = 
					new RequestConnectToHostMessage();
				
				requestConnectToHostMessage.SessionID = this._sessionID;
				requestConnectToHostMessage.SessionProperties = this._properties;
				requestConnectToHostMessage.Password = sqs.Password;
				requestConnectToHostMessage.Gamer = this._signedInGamers[0];
				NetOutgoingMessage netOutgoingMessage = this._netSession.CreateMessage();
				
				netOutgoingMessage.Write(requestConnectToHostMessage, 
					this._gameName, this._version);
				
				this._netSession.Connect(sqs.AvailableSession.HostEndPoint, netOutgoingMessage);
			}
			else
			{
				// We're on a local session.
				this._netSession = null;
			}
		}

		private void SendRemoteData(NetOutgoingMessage msg,
									NetDeliveryMethod flags, 
									NetworkGamer recipient)
		{
			NetConnection netConnection = 
				(NetConnection)recipient.NetConnectionObject;

			if (netConnection != null)
			{
				netConnection.SendMessage(msg, flags, 0);
			}
		}

		private NetDeliveryMethod GetDeliveryMethodFromOptions(SendDataOptions options)
		{
			NetDeliveryMethod result = NetDeliveryMethod.Unknown;

			switch (options)
			{
				case SendDataOptions.None:
				{
					result = NetDeliveryMethod.Unreliable;
					break;
				}

				case SendDataOptions.Reliable:
				{
					result = NetDeliveryMethod.ReliableUnordered;
					break;
				}

				case SendDataOptions.InOrder:
				{
					result = NetDeliveryMethod.UnreliableSequenced;
					break;
				}

				case SendDataOptions.ReliableInOrder:
				{
					result = NetDeliveryMethod.ReliableOrdered;
					break;
				}
			}

			return result;
		}

		private void PrepareMessageForSending(SendDataOptions options, NetworkGamer recipient, 
											  out NetOutgoingMessage msg, out int channel, 
											  out NetConnection netConnection, 
											  out NetDeliveryMethod flags)
		{
			if (recipient.NetProxyObject)
			{
				msg = this._netSession.CreateMessage();
				flags = this.GetDeliveryMethodFromOptions(options);
				channel = 1;
				netConnection = (NetConnection)this._host.NetConnectionObject;
				msg.Write(3);
				msg.Write(recipient.Id);
				msg.Write((byte)flags);
				msg.Write(this._localPlayerGID);
				
				if (flags == NetDeliveryMethod.ReliableUnordered)
				{
					flags = NetDeliveryMethod.ReliableOrdered;
					return;
				}
			}
			else
			{
				NetConnection netConnection2 = (NetConnection)recipient.NetConnectionObject;
				
				if (netConnection2 != null)
				{
					msg = this._netSession.CreateMessage();
					flags = this.GetDeliveryMethodFromOptions(options);
					msg.Write(recipient.Id);
					msg.Write(this._localPlayerGID);
					channel = 0;
					netConnection = netConnection2;
					return;
				}
				
				msg = null;
				channel = 0;
				flags = NetDeliveryMethod.Unknown;
				netConnection = null;
			}
		}

		public override void SendRemoteData(byte[] data, SendDataOptions options, 
											NetworkGamer recipient)
		{
			if (this._netSession != null)
			{
				NetOutgoingMessage msg;
				int sequenceChannel;
				NetConnection netConnection;
				NetDeliveryMethod method;
				
				this.PrepareMessageForSending(options, recipient, out msg, 
					out sequenceChannel, out netConnection, out method);
				
				if (netConnection != null)
				{
					msg.WriteArray(data);
					netConnection.SendMessage(msg, method, sequenceChannel);
				}
			}
		}

		public override void SendRemoteData(byte[] data, int offset, int length, 
											SendDataOptions options, NetworkGamer recipient)
		{
			if (this._netSession != null)
			{
				NetOutgoingMessage msg;
				int sequenceChannel;
				NetConnection netConnection;
				NetDeliveryMethod method;

				this.PrepareMessageForSending(options, recipient, out msg, out sequenceChannel, 
											  out netConnection, out method);

				if (netConnection != null)
				{
					msg.WriteArray(data, offset, length);
					netConnection.SendMessage(msg, method, sequenceChannel);
				}
			}
		}

		private void PrepareBroadcastMessageForSending(SendDataOptions options, 
													   out NetOutgoingMessage msg, 
													   out NetDeliveryMethod flags)
		{
			msg = this._netSession.CreateMessage();
			flags = this.GetDeliveryMethodFromOptions(options);
			msg.Write(4);
			msg.Write((byte)flags);
			msg.Write(this._localPlayerGID);
			
			if (flags == NetDeliveryMethod.ReliableUnordered)
			{
				flags = NetDeliveryMethod.ReliableOrdered;
			}
		}

		public override void BroadcastRemoteData(byte[] data, SendDataOptions options)
		{
			if (this._netSession != null)
			{
				NetConnection netConnection = this._host.NetConnectionObject as NetConnection;
				
				if (netConnection != null)
				{
					NetOutgoingMessage msg;
					NetDeliveryMethod method;
					this.PrepareBroadcastMessageForSending(options, out msg, out method);
					msg.WriteArray(data);
					netConnection.SendMessage(msg, method, 1);
				}
			}
		}

		public override void BroadcastRemoteData(byte[] data, int offset, int length, 
												SendDataOptions options)
		{
			if (this._netSession != null)
			{
				NetConnection netConnection = this._host.NetConnectionObject as NetConnection;

				if (netConnection != null)
				{
					NetOutgoingMessage msg;
					NetDeliveryMethod method;
					this.PrepareBroadcastMessageForSending(options, out msg, out method);
					msg.WriteArray(data, offset, length);
					netConnection.SendMessage(msg, method, 1);
				}
			}
		}

		private bool HandleHostStatusChangedMessage(NetIncomingMessage msg)
		{
			bool flag = true;
			
			switch (msg.ReadByte())
			{
				case 5:
				{
					ConnectedMessage cm = new ConnectedMessage();
					cm.PlayerGID = this._nextPlayerGID;
					cm.SetPeerList(this._allGamers);
					NetOutgoingMessage message1 = this._netSession.CreateMessage();
					message1.Write((byte) 1);
					message1.Write(cm);
					
					int num1 = (int) msg.SenderConnection.SendMessage(
						message1, NetDeliveryMethod.ReliableOrdered, 1);
					
					NetworkGamer networkGamer = this.AddRemoteGamer(
						(Gamer)msg.SenderConnection.Tag, msg.SenderConnection, 
						false, this._nextPlayerGID);
					
					this._nextPlayerGID++;
				
					using (List<NetConnection>.Enumerator enumerator = 
						this._netSession.Connections.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							NetConnection current = enumerator.Current;
							if (current != msg.SenderConnection)
							{
								NetOutgoingMessage message2 = this._netSession.CreateMessage();
								message2.Write((byte) 0);
								message2.Write(networkGamer.Id);
								message2.Write((Gamer) networkGamer);
								
								int num2 = (int)current.SendMessage(
									message2, NetDeliveryMethod.ReliableOrdered, 1);
							}
						}
						break;
					}
				}

				case 7:
				{
					if (msg.SenderConnection.Tag != null && 
						msg.SenderConnection.Tag is NetworkGamer tag2)
					{
						DropPeerMessage data = new DropPeerMessage();
						data.PlayerGID = tag2.Id;
						foreach (NetConnection connection in this._netSession.Connections)
						{
							if (connection != msg.SenderConnection)
							{
								NetOutgoingMessage message3 = this._netSession.CreateMessage();
								message3.Write((byte) 2);
								message3.Write(data);
								
								int num3 = (int)connection.SendMessage(
									message3, NetDeliveryMethod.ReliableOrdered, 1);
							}
						}
				
						this.RemoveGamer(tag2);
						break;
					}
				
					break;
				}
				
				default:
				{
					flag = false;
					break;
				}
			}

			return flag;
		}

		private bool HandleHostSystemMessages(NetIncomingMessage msg)
		{
			bool result = true;
		
			switch (msg.ReadByte())
			{
				case 3:
				{
					byte b = msg.ReadByte();
					NetDeliveryMethod method = (NetDeliveryMethod)msg.ReadByte();
					NetworkGamer networkGamer = this.FindGamerById(b);
					
					if (networkGamer != null)
					{
						byte b2 = msg.ReadByte();
						
						NetOutgoingMessage netOutgoingMessage = 
							this._netSession.CreateMessage();
						
						netOutgoingMessage.Write(b);
						netOutgoingMessage.Write(b2);
						netOutgoingMessage.CopyByteArrayFrom(msg);
					
						NetConnection netConnection = 
							(NetConnection)networkGamer.NetConnectionObject;
					
						netConnection.SendMessage(netOutgoingMessage, method, 0);
					}
					
					break;
				}
				
				case 4:
				{
					NetDeliveryMethod method = (NetDeliveryMethod)msg.ReadByte();
					byte b2 = msg.ReadByte();
					byte[] array = null;
					int num = msg.ReadInt32();
					int num2 = 0;
					bool flag = false;
					if (num > 0)
					{
						flag = msg.GetAlignedData(out array, out num2);
						if (!flag)
						{
							array = msg.ReadBytes(num);
						}
					}
					
					LocalNetworkGamer localNetworkGamer = 
						this.FindGamerById(0) as LocalNetworkGamer;
					
					if (localNetworkGamer != null)
					{
						NetworkGamer sender = this.FindGamerById(b2);
						
						if (flag)
						{
							localNetworkGamer.AppendNewDataPacket(array, num2, num, sender);
						}
						else
						{
							localNetworkGamer.AppendNewDataPacket(array, sender);
						}
						
						for (int i = 0; i < this._remoteGamers.Count; i++)
						{
							if (this._remoteGamers[i].Id != b2)
							{
								NetConnection netConnection2 = 
									this._remoteGamers[i].NetConnectionObject as NetConnection;
								
								if (netConnection2 != null)
								{
									NetOutgoingMessage netOutgoingMessage2 = 
										this._netSession.CreateMessage();
									
									netOutgoingMessage2.Write(this._remoteGamers[i].Id);
									netOutgoingMessage2.Write(b2);
									netOutgoingMessage2.Write(num);
									
									if (num > 0)
									{
										netOutgoingMessage2.Write(array, num2, num);
									}
									
									netConnection2.SendMessage(netOutgoingMessage2, method, 0);
								}
							}
						}
					}

					break;
				}
			}
			return result;
		}

		private void HandleHostDiscoveryRequest(NetIncomingMessage msg)
		{
			HostDiscoveryRequestMessage hostDiscoveryRequestMessage = 
				msg.ReadDiscoveryRequestMessage(this._gameName, this._version);
			
			HostDiscoveryResponseMessage hostDiscoveryResponseMessage = 
				new HostDiscoveryResponseMessage();
		
			switch (hostDiscoveryRequestMessage.ReadResult)
			{
				case VersionCheckedMessage.ReadResultCode.Success:
				{
					if (this.AllowConnectionCallback == null || 
						this.AllowConnectionCallback(
							hostDiscoveryRequestMessage.PlayerID, msg.SenderEndPoint.Address))
					{
						hostDiscoveryResponseMessage.Result = NetworkSession.ResultCode.Succeeded;
					}
					else
					{
						hostDiscoveryResponseMessage.Result = 
							NetworkSession.ResultCode.ConnectionDenied;
					}

					break;
				}

				case VersionCheckedMessage.ReadResultCode.GameNameInvalid:
				{
					hostDiscoveryResponseMessage.Result = 
						NetworkSession.ResultCode.GameNamesDontMatch;
					
					break;
				}

				case VersionCheckedMessage.ReadResultCode.LocalVersionIsHigher:
				{
					hostDiscoveryResponseMessage.Result = 
						NetworkSession.ResultCode.ServerHasNewerVersion;
					
					break;
				}

				case VersionCheckedMessage.ReadResultCode.LocalVersionIsLower:
				{
					hostDiscoveryResponseMessage.Result = 
						NetworkSession.ResultCode.ServerHasOlderVersion;
					
					break;
				}
			}
			
			if (hostDiscoveryResponseMessage.Result == NetworkSession.ResultCode.Succeeded)
			{
				hostDiscoveryResponseMessage.RequestID = hostDiscoveryRequestMessage.RequestID;
				hostDiscoveryResponseMessage.SessionID = this._sessionID;
				hostDiscoveryResponseMessage.SessionProperties = this._properties;
				hostDiscoveryResponseMessage.CurrentPlayers = this._allGamers.Count;
				hostDiscoveryResponseMessage.MaxPlayers = this._maxPlayers;
				hostDiscoveryResponseMessage.Message = this._serverMessage;
				hostDiscoveryResponseMessage.HostUsername = this._host.Gamertag;
			
				hostDiscoveryResponseMessage.PasswordProtected = 
					!string.IsNullOrWhiteSpace(this._password);
			}
			
			NetOutgoingMessage msg2 = this._netSession.CreateMessage();
			msg2.Write(hostDiscoveryResponseMessage, this._gameName, this._version);
			this._netSession.SendDiscoveryResponse(msg2, msg.SenderEndPoint);
		}

		private void HandleHostConnectionApproval(NetIncomingMessage msg)
		{
			RequestConnectToHostMessage requestConnectToHostMessage = 
				msg.ReadRequestConnectToHostMessage(this._gameName, this._version);
			
			if (requestConnectToHostMessage.ReadResult == 
					VersionCheckedMessage.ReadResultCode.GameNameInvalid)
			{
				this.FailConnection(msg.SenderConnection, 
					NetworkSession.ResultCode.GameNamesDontMatch);
				
				return;
			}
			
			if (requestConnectToHostMessage.ReadResult == 
					VersionCheckedMessage.ReadResultCode.VersionInvalid)
			{
				this.FailConnection(msg.SenderConnection, 
					NetworkSession.ResultCode.ServerHasOlderVersion);
				
				return;
			}
			
			if (requestConnectToHostMessage.ReadResult == 
					VersionCheckedMessage.ReadResultCode.LocalVersionIsLower)
			{
				this.FailConnection(msg.SenderConnection, 
					NetworkSession.ResultCode.ServerHasOlderVersion);
				
				return;
			}
			
			if (requestConnectToHostMessage.ReadResult == 
					VersionCheckedMessage.ReadResultCode.LocalVersionIsHIgher)
			{
				this.FailConnection(msg.SenderConnection, 
					NetworkSession.ResultCode.ServerHasNewerVersion);
				
				return;
			}

			if (!string.IsNullOrWhiteSpace(this._password) && 
				(string.IsNullOrWhiteSpace(requestConnectToHostMessage.Password) || 
					!requestConnectToHostMessage.Password.Equals(this._password)))
			{
				this.FailConnection(msg.SenderConnection, 
					NetworkSession.ResultCode.IncorrectPassword);
				return;
			}

			if (this.AllowConnectionCallback != null && 
				!this.AllowConnectionCallback(
					requestConnectToHostMessage.Gamer.PlayerID, msg.SenderEndPoint.Address))
			{
				this.FailConnection(msg.SenderConnection, 
					NetworkSession.ResultCode.ConnectionDenied);
				return;
			}

			if (requestConnectToHostMessage.SessionProperties.Count != this._properties.Count)
			{
				this.FailConnection(msg.SenderConnection, 
					NetworkSession.ResultCode.SessionPropertiesDontMatch);
				return;
			}

			for (int i = 0; i < requestConnectToHostMessage.SessionProperties.Count; i++)
			{
				if (requestConnectToHostMessage.SessionProperties[i] != this._properties[i])
				{
					this.FailConnection(msg.SenderConnection, 
						NetworkSession.ResultCode.SessionPropertiesDontMatch);
					return;
				}
			}

			GamerCollection<NetworkGamer> allGamers = base.AllGamers;
			
			for (int j = 0; j < allGamers.Count; j++)
			{
				if (allGamers[j] != null && 
					allGamers[j].Gamertag == requestConnectToHostMessage.Gamer.Gamertag)
				{
					this.FailConnection(msg.SenderConnection, 
						NetworkSession.ResultCode.GamerAlreadyConnected);
					
					return;
				}
			}

			msg.SenderConnection.Tag = requestConnectToHostMessage.Gamer;
			msg.SenderConnection.Approve();
		}

		private bool HandleHostMessages(NetIncomingMessage msg)
		{
			bool result = true;
			NetIncomingMessageType messageType = msg.MessageType;
		
			if (messageType <= NetIncomingMessageType.ConnectionApproval)
			{
				if (messageType == NetIncomingMessageType.StatusChanged)
				{
					return this.HandleHostStatusChangedMessage(msg);
				}
				
				if (messageType == NetIncomingMessageType.ConnectionApproval)
				{
					this.HandleHostConnectionApproval(msg);
					return result;
				}
			}
			else if (messageType != NetIncomingMessageType.Data)
			{
				if (messageType == NetIncomingMessageType.DiscoveryRequest)
				{
					this.HandleHostDiscoveryRequest(msg);
					return result;
				}
			}
			else
			{
				if (msg.SequenceChannel == 1)
				{
					return this.HandleHostSystemMessages(msg);
				}

				return false;
			}
			
			result = false;
			return result;
		}

		private void AddNewPeer(NetIncomingMessage msg)
		{
			byte globalID = msg.ReadByte();
			Gamer gmr = msg.ReadGamer();
			base.AddProxyGamer(gmr, false, globalID);
		}

		private bool HandleClientSystemMessages(NetIncomingMessage msg)
		{
			bool result = true;
		
			InternalMessageTypes internalMessageTypes = 
				(InternalMessageTypes)msg.ReadByte();
		
			NetworkGamer gamer = null;
			
			switch (internalMessageTypes)
			{
				case InternalMessageTypes.NewPeer:
				{
					this.AddNewPeer(msg);
					break;
				}

				case InternalMessageTypes.ResponseToConnection:
				{
					ConnectedMessage connectedMessage = msg.ReadConnectedMessage();
					
					base.AddLocalGamer(this._signedInGamers[0], 
						false, connectedMessage.PlayerGID);
					
					for (int i = 0; i < connectedMessage.Peers.Length; i++)
					{
						if (connectedMessage.ids[i] == 0)
						{
							this.AddRemoteGamer(connectedMessage.Peers[i], 
								msg.SenderConnection, true, 0);
						}
						else
						{
							base.AddProxyGamer(connectedMessage.Peers[i], 
								false, connectedMessage.ids[i]);
						}
					}
					
					break;
				}
				
				case InternalMessageTypes.DropPeer:
				{
					DropPeerMessage dropPeerMessage = msg.ReadDropPeerMessage();
					
					if (this._idToGamer.TryGetValue(dropPeerMessage.PlayerGID, out gamer))
					{
						base.RemoveGamer(gamer);
					}
					
					break;
				}
				
				default:
				{
					result = false;
					break;
				}
			}

			return result;
		}

		private void HandleClientStatusChangedMessage(NetIncomingMessage msg)
		{
			switch (msg.ReadByte())
			{
				case 5:
				{
					this._hostConnectionResult = 
						NetworkSession.ResultCode.Succeeded;
					
					this._hostConnectionResultString = 
						this._hostConnectionResult.ToString();
					
					break;
				}

				case 7:
				{
					this.HandleDisconnection(msg.ReadString());
					break;
				}
			}
		}

		private bool HandleClientMessages(NetIncomingMessage msg)
		{
			bool flag = true;
			
			switch (msg.MessageType)
			{
				case NetIncomingMessageType.StatusChanged:
				{
					this.HandleClientStatusChangedMessage(msg);
					break;
				}

				case NetIncomingMessageType.Data:
				{
					if (msg.SequenceChannel == 1)
					{
						return this.HandleClientSystemMessages(msg);
					}

					flag = false;
					
					break;
				}
				
				default:
				{
					flag = false;
					break;
				}
			}
			
			return flag;
		}

		private void FailConnection(NetConnection c, NetworkSession.ResultCode reason) =>
			c.Deny(reason.ToString());

		private bool HandleCommonMessages(NetIncomingMessage msg)
		{
			bool result = true;
			NetIncomingMessageType messageType = msg.MessageType;
			
			if (messageType <= NetIncomingMessageType.VerboseDebugMessage)
			{
				if (messageType != NetIncomingMessageType.Data)
				{
					if (messageType == NetIncomingMessageType.VerboseDebugMessage)
					{
						return result;
					}
				}
				else
				{
					byte gamerId = msg.ReadByte();
					NetworkGamer networkGamer = this.FindGamerById(gamerId);
					
					if (networkGamer == null)
					{
						return result;
					}
					
					LocalNetworkGamer localNetworkGamer = networkGamer as LocalNetworkGamer;
					
					if (localNetworkGamer == null)
					{
						return result;
					}
					
					byte gamerId2 = msg.ReadByte();
					NetworkGamer networkGamer2 = this.FindGamerById(gamerId2);
					
					if (networkGamer2 != null)
					{
						byte[] data = msg.ReadByteArray();
						localNetworkGamer.AppendNewDataPacket(data, networkGamer2);
						return result;
					}
					
					return result;
				}
			}
			else if (messageType == NetIncomingMessageType.DebugMessage || 
					 messageType == NetIncomingMessageType.WarningMessage || 
					 messageType == NetIncomingMessageType.ErrorMessage)
			{
				return result;
			}

			result = false;
			return result;
		}

		public override void Update()
		{
			if (this._netSession != null)
			{
				NetIncomingMessage msg;
				
				while ((msg = this._netSession.ReadMessage()) != null)
				{
					if (!(this._isHost ? this.HandleHostMessages(msg) 
						: this.HandleClientMessages(msg)))
					{
						bool flag = this.HandleCommonMessages(msg);
					}

					if (this._netSession == null)
					{
						return;
					}

					this._netSession.Recycle(msg);
				}
			}
		}

		public override void Dispose(bool disposeManagedObjects)
		{
			this._staticProvider.TaskScheduler.Exit();
			
			if (this._netSession != null)
			{
				int port = this._netSession.Port;
				this._netSession.Shutdown("Session Ended Normally");
				
				if (this.IsHost && this._netSession.UPnP != null)
				{
					this._netSession.UPnP.DeleteForwardingRule(port);
				}
				
				this._netSession = null;
			}
			
			base.Dispose(disposeManagedObjects);
		}
	}
}
