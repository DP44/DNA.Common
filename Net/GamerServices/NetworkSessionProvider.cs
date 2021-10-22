using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using DNA.Net.MatchMaking;

namespace DNA.Net.GamerServices
{
	public abstract class NetworkSessionProvider : IDisposable
	{
		protected const string sDisconnectReasonShutdown = "Session Ended Normally";
		protected const string sDisconnectReasonKicked = "Host Kicked Us";
		protected const string sDisconnectReasonDropped = "Connection Dropped";

		protected NetworkSessionStaticProvider _staticProvider;
		protected NetworkSession _networkSession;

		public ManualResetEvent LocalPlayerJoinedEvent = new ManualResetEvent(false);

		protected Dictionary<byte, NetworkGamer> _idToGamer = 
			new Dictionary<byte, NetworkGamer>();

		protected List<NetworkGamer> _allGamers;

		protected GamerCollection<NetworkGamer> _allGamerCollection;

		protected List<NetworkGamer> _remoteGamers;

		protected GamerCollection<NetworkGamer> _remoteGamerCollection = 
			new GamerCollection<NetworkGamer>();

		protected List<LocalNetworkGamer> _localGamers;

		protected GamerCollection<LocalNetworkGamer> _localGamerCollection;

		protected List<SignedInGamer> _signedInGamers;

		protected NetworkSessionProperties _properties = new NetworkSessionProperties();

		protected int _maxPlayers;
		protected string _gameName;
		protected string _password;
		protected string _serverMessage;
		protected string _externalIPString;
		protected string _internalIPString;
		protected int _version;

		protected NetworkSession.ResultCode _hostConnectionResult;
		protected string _hostConnectionResultString;

		public NetworkSession.AllowConnectionCallbackDelegate AllowConnectionCallback;
		public NetworkSession.AllowConnectionCallbackDelegateAlt AllowConnectionCallbackAlt;

		protected bool _isHost;
		protected byte _nextPlayerGID = 1;
		protected byte _localPlayerGID;
		protected bool _disposed;
		protected bool _allowHostMigration;
		protected bool _allowJoinInProgress;
		protected NetworkGamer _host;
		protected NetworkSessionType _sessionType;

		public HostSessionInfo HostSessionInfo;

		public GamerCollection<NetworkGamer> AllGamers => 
			this._allGamerCollection;

		public GamerCollection<NetworkGamer> RemoteGamers => 
			this._remoteGamerCollection;

		public virtual string ExternalIPString => 
			this._externalIPString;

		public virtual string InternalIPString => 
			this._internalIPString;

		public virtual string Password
		{
			get => 
				this._password;
			
			set => 
				this._password = value;
		}

		public virtual NetworkSession.ResultCode HostConnectionResult => 
			this._hostConnectionResult;

		public virtual string HostConnectionResultString => 
			this._hostConnectionResultString;

		public void ResetHostConnectionResult()
		{
			this._hostConnectionResult = NetworkSession.ResultCode.Pending;
			this._hostConnectionResultString = "";
		}

		public string ServerMessage
		{
			get => 
				this._serverMessage;
		
			set => 
				this._serverMessage = value;
		}

		private event EventHandler<GamerJoinedEventArgs> _gamerJoined;

		public virtual event EventHandler<GamerJoinedEventArgs> GamerJoined
		{
			add
			{
				this._gamerJoined += value;
				
				GamerCollection<LocalNetworkGamer> localGamers = this.LocalGamers;
				
				foreach (LocalNetworkGamer gamer in localGamers)
				{
					value(this, new GamerJoinedEventArgs(gamer));
				}
			}
			
			remove => 
				this._gamerJoined -= value;
		}

		public event EventHandler<GameEndedEventArgs> GameEnded;
		public event EventHandler<GamerLeftEventArgs> GamerLeft;
		public event EventHandler<GameStartedEventArgs> GameStarted;
		public event EventHandler<HostChangedEventArgs> HostChanged;
		public event EventHandler<NetworkSessionEndedEventArgs> SessionEnded;

		public NetworkSessionProvider(NetworkSessionStaticProvider statics)
		{
			this._staticProvider = statics;
			this._localGamers = new List<LocalNetworkGamer>();
			this._allGamers = new List<NetworkGamer>();
			this._remoteGamers = new List<NetworkGamer>();
			
			this._localGamerCollection = 
				new GamerCollection<LocalNetworkGamer>(this._localGamers);
			
			this._allGamerCollection = 
				new GamerCollection<NetworkGamer>(this._allGamers);
			
			this._remoteGamerCollection = 
				new GamerCollection<NetworkGamer>(this._remoteGamers);

			this._nextPlayerGID = 1;
		}

		protected static DNAGame CurrentGame
		{
			get
			{
				return (DNAGame)GamerServicesComponent.Instance.Game;
			}
		}

		protected void HandleDisconnection(string disconnectReason)
		{
			if (this._hostConnectionResult == NetworkSession.ResultCode.Pending)
			{
				this._hostConnectionResult = 
					this._staticProvider.ParseResultCode(disconnectReason);
				
				this._hostConnectionResultString = disconnectReason;
			}
			else
			{
				if (this.SessionEnded == null)
				{
					return;
				}

				NetworkSessionEndReason endReason;
				
				switch (disconnectReason)
				{
					case "Session Ended Normally":
					{
						endReason = NetworkSessionEndReason.HostEndedSession;
						break;
					}

					case "Host Kicked Us":
					{
						endReason = NetworkSessionEndReason.RemovedByHost;
						break;
					}

					case "Connection Dropped":
					{
						endReason = NetworkSessionEndReason.Disconnected;
						break;
					}

					case "Failed to establish connection - no response from remote host":
					{
						endReason = NetworkSessionEndReason.Disconnected;
						break;
					}

					default:
					{
						endReason = NetworkSessionEndReason.Disconnected;
						break;
					}
				}

				this.SessionEnded((object)this, new NetworkSessionEndedEventArgs(endReason));
			}
		}

		protected void AddLocalGamer(SignedInGamer sig, bool isHost, 
									 byte globalID, ulong steamID)
		{
			LocalNetworkGamer newGamer = new LocalNetworkGamer(sig, 
				this._networkSession, true, isHost, globalID, steamID);
			
			this._localPlayerGID = globalID;
			
			if (isHost)
			{
				this._host = newGamer;
			}
			
			this._idToGamer.Add(globalID, newGamer);
			this._allGamers.Add(newGamer);
			this._localGamers.Add(newGamer);
			this._allGamerCollection = new GamerCollection<NetworkGamer>(this._allGamers);
			
			lock (this._localGamerCollection)
			{
				this._localGamerCollection = 
					new GamerCollection<LocalNetworkGamer>(this._localGamers);
			}
			
			this.LocalPlayerJoinedEvent.Set();
			
			if (this._gamerJoined != null)
			{
				this._gamerJoined(this, new GamerJoinedEventArgs(newGamer));
			}
		}

		protected void AddLocalGamer(SignedInGamer sig, bool isHost, byte globalID)
		{
			this.AddLocalGamer(sig, isHost, globalID, 0UL);
		}

		protected NetworkGamer AddProxyGamer(Gamer gmr, bool isHost, byte globalID)
		{
			NetworkGamer networkGamer = new NetworkGamer(gmr, 
				this._networkSession, false, isHost, globalID);
			
			if (isHost)
			{
				this._host = networkGamer;
			}
			this._idToGamer.Add(networkGamer.Id, networkGamer);
			this._allGamers.Add(networkGamer);
			
			this._allGamerCollection = 
				new GamerCollection<NetworkGamer>(this._allGamers);
			
			this._remoteGamers.Add(networkGamer);
			
			this._remoteGamerCollection = 
				new GamerCollection<NetworkGamer>(this._remoteGamers);
			
			if (this._gamerJoined != null)
			{
				this._gamerJoined(this, new GamerJoinedEventArgs(networkGamer));
			}
			
			return networkGamer;
		}

		private void FinishAddingRemoteGamer(NetworkGamer ng, bool isHost, byte playerGID)
		{
			if (isHost)
			{
				this._host = ng;
			}
			
			this._idToGamer.Add(playerGID, ng);
			this._allGamers.Add(ng);
			
			this._allGamerCollection = 
				new GamerCollection<NetworkGamer>(this._allGamers);
			
			this._remoteGamers.Add(ng);
			
			this._remoteGamerCollection = 
				new GamerCollection<NetworkGamer>(this._remoteGamers);
			
			if (this._gamerJoined != null)
			{
				this._gamerJoined(this, new GamerJoinedEventArgs(ng));
			}
		}

		protected NetworkGamer AddRemoteGamer(Gamer gmr, IPAddress endPoint, 
											  bool isHost, byte playerGID)
		{
			NetworkGamer networkGamer = new NetworkGamer(gmr, 
				this._networkSession, false, isHost, playerGID, endPoint);
			
			this.FinishAddingRemoteGamer(networkGamer, isHost, playerGID);
			
			return networkGamer;
		}

		protected NetworkGamer AddRemoteGamer(Gamer gmr, ulong steamId, 
											  bool isHost, byte playerGID)
		{
			NetworkGamer networkGamer = new NetworkGamer(gmr, 
				this._networkSession, false, isHost, playerGID, steamId);
			
			this.FinishAddingRemoteGamer(networkGamer, isHost, playerGID);
			
			return networkGamer;
		}

		protected void RemoveGamer(NetworkGamer gamer)
		{
			if (this.GamerLeft != null)
			{
				this.GamerLeft(this, new GamerLeftEventArgs(gamer));
			}
			
			gamer.NetConnectionObject = null;
			this._idToGamer.Remove(gamer.Id);
			
			if (this._allGamers.Remove(gamer))
			{
				this._allGamerCollection = new GamerCollection<NetworkGamer>(this._allGamers);
			}
			
			if (gamer is LocalNetworkGamer)
			{
				if (!this._localGamers.Remove(gamer as LocalNetworkGamer))
				{
					return;
				}
				
				lock (this._localGamerCollection)
				{
					this._localGamerCollection = 
						new GamerCollection<LocalNetworkGamer>(this._localGamers);
					
					return;
				}
			}

			if (this._remoteGamers.Remove(gamer))
			{
				this._remoteGamerCollection = 
					new GamerCollection<NetworkGamer>(this._remoteGamers);
			}
		}

		public virtual NetworkGamer FindGamerById(byte gamerId)
		{
			NetworkGamer networkGamer;
			
			return this._idToGamer.TryGetValue(gamerId, out networkGamer) 
				? networkGamer 
				: (NetworkGamer)null;
		}

		public virtual bool AllowHostMigration
		{
			get => 
				this._allowHostMigration;
			
			set => 
				this._allowHostMigration = value;
		}

		public virtual bool AllowJoinInProgress
		{
			get => 
				this._allowJoinInProgress;
			
			set => 
				this._allowJoinInProgress = value;
		}

		public virtual NetworkGamer Host => 
			this._host;

		public virtual bool IsDisposed => 
			this._disposed;

		public virtual bool IsHost => 
			this._isHost;

		public virtual int MaxGamers
		{
			get => 
				this._maxPlayers;
			
			set => 
				throw new NotImplementedException();
		}

		public virtual NetworkSessionProperties SessionProperties => 
			this._properties;

		public virtual NetworkSessionType SessionType => 
			this._sessionType;

		public abstract void StartHost(NetworkSessionStaticProvider.BeginCreateSessionState sqs);
		public abstract void StartClient(NetworkSessionStaticProvider.BeginJoinSessionState sqs);

		public virtual void StartClientInvited(ulong lobbyId, 
			NetworkSessionStaticProvider.BeginJoinSessionState sqs, 
			GetPasswordForInvitedGameCallback getPasswordCallback)
		{
			throw new NotImplementedException();
		}

		public abstract void BroadcastRemoteData(byte[] data, SendDataOptions options);

		public abstract void BroadcastRemoteData(byte[] data, int offset, 
												 int length, SendDataOptions options);

		public void Dispose()
		{
			// Make sure it isn't already disposed of.
			if (this._disposed)
			{
				return;
			}

			this.Dispose(true);
		}

		public virtual GamerCollection<LocalNetworkGamer> LocalGamers
		{
			get
			{
				lock (this._localGamerCollection)
				{
					return this._localGamerCollection;
				}
			}
		}

		public abstract void SendRemoteData(byte[] data, 
			SendDataOptions options, NetworkGamer recipient);

		public abstract void SendRemoteData(byte[] data, int offset, 
			int length, SendDataOptions options, NetworkGamer recipient);

		public abstract void ReportClientJoined(string username);

		public abstract void ReportClientLeft(string username);

		public abstract void ReportSessionAlive();

		public abstract void UpdateHostSession(string serverName, 
			bool? passwordProtected, bool? isPublic, 
			NetworkSessionProperties sessionProps);

		public abstract void UpdateHostSessionJoinPolicy(JoinGamePolicy joinGamePolicy);

		public abstract void CloseNetworkSession();

		public abstract void Update();

		public virtual void AddLocalGamer(SignedInGamer gamer) => 
			throw new NotImplementedException();

		public virtual int BytesPerSecondReceived => 
			throw new NotImplementedException();

		public virtual int BytesPerSecondSent => 
			throw new NotImplementedException();

		public virtual void Dispose(bool disposeManagedObjects) => 
			this._disposed = true;

		public virtual void EndGame() => 
			throw new NotImplementedException();

		public virtual bool IsEveryoneReady => 
			throw new NotImplementedException();

		public virtual GamerCollection<NetworkGamer> PreviousGamers => 
			throw new NotImplementedException();

		public virtual int PrivateGamerSlots
		{
			get => 
				throw new NotImplementedException();
			
			set => 
				throw new NotImplementedException();
		}

		public virtual NetworkSessionState SessionState => 
			throw new NotImplementedException();

		public virtual TimeSpan SimulatedLatency
		{
			get => 
				throw new NotImplementedException();
			
			set => 
				throw new NotImplementedException();
		}

		public virtual float SimulatedPacketLoss
		{
			get => 
				throw new NotImplementedException();
			
			set => 
				throw new NotImplementedException();
		}

		public virtual void StartGame() => 
			throw new NotImplementedException();

		public virtual void ResetReady() => 
			throw new NotImplementedException();
	}
}
