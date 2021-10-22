using System;
using System.Collections.Generic;
using System.Threading;
using DNA.Net.MatchMaking;
using DNA.Threading;

namespace DNA.Net.GamerServices
{
	public abstract class NetworkSessionStaticProvider
	{
		public abstract class BaseAsyncResult : IAsyncResult
		{
			private object _state;

			public ManualResetEvent Event = new ManualResetEvent(false);

			public AsyncCallback Callback;

			public BaseAsyncResult(AsyncCallback callback, object state)
			{
				this.Callback = callback;
				this._state = state;
			}

			public object AsyncState
			{
				get
				{
					return this._state;
				}
			}

			public WaitHandle AsyncWaitHandle
			{
				get
				{
					return this.Event;
				}
			}

			public bool CompletedSynchronously
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public virtual bool IsCompleted
			{
				get
				{
					return true;
				}
			}
		}

		public class BeginCreateSessionState : NetworkSessionStaticProvider.BaseAsyncResult
		{
			public NetworkSession Session;

			public NetworkSessionType SessionType;

			public int MaxPlayers;

			public IEnumerable<SignedInGamer> LocalGamers;

			public NetworkSessionProperties Properties;

			public string NetworkGameName;

			public int Version;

			public string ServerMessage;

			public string Password;

			public Exception ExceptionEncountered;

			public BeginCreateSessionState(AsyncCallback callback, object state) : base(callback, state)
			{
			}

			public override bool IsCompleted
			{
				get
				{
					return this.ExceptionEncountered == null;
				}
			}
		}

		public class BeginJoinSessionState : NetworkSessionStaticProvider.BaseAsyncResult
		{
			public NetworkSession Session;

			public NetworkSessionType SessionType;

			public AvailableNetworkSession AvailableSession;

			public IEnumerable<SignedInGamer> LocalGamers;

			public string NetworkGameName;

			public int Version;

			public NetworkSession.ResultCode HostConnectionResult;

			public string HostConnectionResultString;

			public string Password;

			public BeginJoinSessionState(AsyncCallback callback, object state) : base(callback, state)
			{
			}
		}

		public class SessionQueryState : NetworkSessionStaticProvider.BaseAsyncResult
		{
			public AvailableNetworkSessionCollection Sessions;

			public QuerySessionInfo SearchProperties;

			public List<ClientSessionInfo> ClientSessionsFound;

			public SessionQueryState(QuerySessionInfo searchProps, AsyncCallback callback, object state) : base(callback, state)
			{
				this.SearchProperties = searchProps;
			}
		}

		public class JoinSessionState : NetworkSessionStaticProvider.BaseAsyncResult
		{
			public AvailableNetworkSession SessionToJoin;

			public NetworkSession JoinedSession;

			public JoinSessionState(AsyncCallback callback, object state) : base(callback, state)
			{
			}
		}

		private TaskScheduler _taskScheduler;

		private string[] _sResultCodeNames = new string[27];

		protected int _defaultPort = 61903;

		public NetworkSessionServices NetworkSessionServices;

		public TaskScheduler TaskScheduler
		{
			get
			{
				return this._taskScheduler;
			}
		}

		protected static DNAGame CurrentGame
		{
			get
			{
				return (DNAGame)GamerServicesComponent.Instance.Game;
			}
		}

		public virtual int DefaultPort
		{
			get
			{
				return this._defaultPort;
			}
			set
			{
				this._defaultPort = value;
			}
		}

		protected event EventHandler<InviteAcceptedEventArgs> _inviteAccepted;

		public virtual event EventHandler<InviteAcceptedEventArgs> InviteAccepted
		{
			add
			{
				this._inviteAccepted += value;
			}
			remove
			{
				this._inviteAccepted -= value;
			}
		}

		protected void CallInviteAccepted(InviteAcceptedEventArgs args)
		{
			if (this._inviteAccepted != null)
			{
				this._inviteAccepted(this, args);
			}
		}

		private void _taskScheduler_ThreadException(object sender, TaskScheduler.ExceptionEventArgs e)
		{
			NetworkSessionStaticProvider.CurrentGame.CrashGame(e.InnerException);
		}

		public NetworkSessionStaticProvider()
		{
			for (NetworkSession.ResultCode resultCode = NetworkSession.ResultCode.Succeeded; resultCode < NetworkSession.ResultCode.Count; resultCode++)
			{
				this._sResultCodeNames[(int)resultCode] = resultCode.ToString();
			}
			this._taskScheduler = new TaskScheduler();
			this._taskScheduler.ThreadException += this._taskScheduler_ThreadException;
		}

		public NetworkSession.ResultCode ParseResultCode(string name)
		{
			for (int i = 0; i < 27; i++)
			{
				if (this._sResultCodeNames[i] == name)
				{
					return (NetworkSession.ResultCode)i;
				}
			}
			return NetworkSession.ResultCode.UnknownResult;
		}

		public virtual IAsyncResult BeginCreate(NetworkSessionType sessionType, IEnumerable<SignedInGamer> localGamers, int maxGamers, int privateGamerSlots, NetworkSessionProperties sessionProperties, string gameName, int networkVersion, string serverMessage, string password, AsyncCallback callback, object asyncState)
		{
			NetworkSessionStaticProvider.BeginCreateSessionState beginCreateSessionState = new NetworkSessionStaticProvider.BeginCreateSessionState(callback, asyncState);
			beginCreateSessionState.LocalGamers = localGamers;
			beginCreateSessionState.Properties = sessionProperties;
			beginCreateSessionState.MaxPlayers = maxGamers;
			beginCreateSessionState.SessionType = sessionType;
			beginCreateSessionState.NetworkGameName = gameName;
			beginCreateSessionState.ServerMessage = serverMessage;
			beginCreateSessionState.Version = networkVersion;
			beginCreateSessionState.Password = password;
			beginCreateSessionState.Session = this.CreateSession();
			this.FinishBeginCreate(beginCreateSessionState);
			return beginCreateSessionState;
		}

		public virtual NetworkSession EndCreate(IAsyncResult result)
		{
			NetworkSessionStaticProvider.BeginCreateSessionState beginCreateSessionState = (NetworkSessionStaticProvider.BeginCreateSessionState)result;
			beginCreateSessionState.AsyncWaitHandle.WaitOne();
			return beginCreateSessionState.Session;
		}

		public virtual IAsyncResult BeginFind(NetworkSessionType sessionType, IEnumerable<SignedInGamer> localGamers, QuerySessionInfo searchProperties, AsyncCallback callback, object asyncState)
		{
			NetworkSessionStaticProvider.SessionQueryState sessionQueryState = new NetworkSessionStaticProvider.SessionQueryState(searchProperties, callback, asyncState);
			this.FinishBeginFind(sessionQueryState);
			return sessionQueryState;
		}

		public virtual AvailableNetworkSessionCollection EndFind(IAsyncResult result)
		{
			NetworkSessionStaticProvider.SessionQueryState sessionQueryState = (NetworkSessionStaticProvider.SessionQueryState)result;
			sessionQueryState.AsyncWaitHandle.WaitOne();
			return sessionQueryState.Sessions;
		}

		public virtual IAsyncResult BeginJoinInvited(ulong lobbyId, int version, string gameName, IEnumerable<SignedInGamer> localGamers, AsyncCallback callback, object asyncState, GetPasswordForInvitedGameCallback getPasswordCallback)
		{
			NetworkSessionStaticProvider.BeginJoinSessionState beginJoinSessionState = new NetworkSessionStaticProvider.BeginJoinSessionState(callback, asyncState);
			beginJoinSessionState.AvailableSession = null;
			beginJoinSessionState.SessionType = NetworkSessionType.PlayerMatch;
			beginJoinSessionState.LocalGamers = localGamers;
			beginJoinSessionState.Version = version;
			beginJoinSessionState.NetworkGameName = gameName;
			beginJoinSessionState.Session = this.CreateSession();
			this.FinishBeginJoinInvited(lobbyId, beginJoinSessionState, getPasswordCallback);
			return beginJoinSessionState;
		}

		public virtual IAsyncResult BeginJoin(AvailableNetworkSession availableSession, string gameName, int version, string password, IEnumerable<SignedInGamer> localGamers, AsyncCallback callback, object asyncState)
		{
			NetworkSessionStaticProvider.BeginJoinSessionState beginJoinSessionState = new NetworkSessionStaticProvider.BeginJoinSessionState(callback, asyncState);
			beginJoinSessionState.AvailableSession = availableSession;
			beginJoinSessionState.SessionType = NetworkSessionType.PlayerMatch;
			beginJoinSessionState.LocalGamers = localGamers;
			beginJoinSessionState.NetworkGameName = gameName;
			beginJoinSessionState.Version = version;
			beginJoinSessionState.Password = password;
			beginJoinSessionState.Session = this.CreateSession();
			this.FinishBeginJoin(beginJoinSessionState);
			return beginJoinSessionState;
		}

		public virtual NetworkSession EndJoin(IAsyncResult result)
		{
			NetworkSessionStaticProvider.BeginJoinSessionState beginJoinSessionState = (NetworkSessionStaticProvider.BeginJoinSessionState)result;
			if (beginJoinSessionState.HostConnectionResult != NetworkSession.ResultCode.Succeeded)
			{
				string empty = string.Empty;
				beginJoinSessionState.Session.Dispose();
				beginJoinSessionState.Session = null;
				throw new Exception("Connection failed: " + beginJoinSessionState.HostConnectionResultString);
			}
			beginJoinSessionState.AsyncWaitHandle.WaitOne();
			return beginJoinSessionState.Session;
		}

		protected abstract NetworkSession CreateSession();

		protected abstract void FinishBeginCreate(NetworkSessionStaticProvider.BeginCreateSessionState state);

		protected abstract void FinishBeginJoin(NetworkSessionStaticProvider.BeginJoinSessionState state);

		protected abstract void FinishBeginFind(NetworkSessionStaticProvider.SessionQueryState state);

		protected virtual void FinishBeginJoinInvited(ulong lobbyId, NetworkSessionStaticProvider.BeginJoinSessionState state, GetPasswordForInvitedGameCallback getPasswordCallback)
		{
			throw new NotImplementedException();
		}

		public abstract HostDiscovery GetHostDiscoveryObject(string gamename, int version, PlayerID playerID);

		public virtual IAsyncResult BeginJoinInvited(int maxLocalGamers, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public virtual NetworkSession Create(NetworkSessionType sessionType, int maxLocalGamers, int maxGamers)
		{
			throw new NotImplementedException();
		}

		public virtual NetworkSession Create(NetworkSessionType sessionType, IEnumerable<SignedInGamer> localGamers, int maxGamers, int privateGamerSlots, NetworkSessionProperties sessionProperties)
		{
			throw new NotImplementedException();
		}

		public virtual NetworkSession Create(NetworkSessionType sessionType, int maxLocalGamers, int maxGamers, int privateGamerSlots, NetworkSessionProperties sessionProperties)
		{
			throw new NotImplementedException();
		}

		public virtual NetworkSession EndJoinInvited(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public virtual AvailableNetworkSessionCollection Find(NetworkSessionType sessionType, IEnumerable<SignedInGamer> localGamers, NetworkSessionProperties searchProperties)
		{
			throw new NotImplementedException();
		}

		public virtual AvailableNetworkSessionCollection Find(NetworkSessionType sessionType, int maxLocalGamers, NetworkSessionProperties searchProperties)
		{
			throw new NotImplementedException();
		}

		public virtual NetworkSession Join(AvailableNetworkSession availableSession)
		{
			throw new NotImplementedException();
		}

		public virtual NetworkSession JoinInvited(IEnumerable<SignedInGamer> localGamers)
		{
			throw new NotImplementedException();
		}

		public virtual NetworkSession JoinInvited(int maxLocalGamers)
		{
			throw new NotImplementedException();
		}

		public virtual IAsyncResult BeginCreate(NetworkSessionType sessionType, int maxLocalGamers, int maxGamers, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public virtual IAsyncResult BeginCreate(NetworkSessionType sessionType, int maxLocalGamers, int maxGamers, int privateGamerSlots, NetworkSessionProperties sessionProperties, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public virtual IAsyncResult BeginFind(NetworkSessionType sessionType, int maxLocalGamers, NetworkSessionProperties searchProperties, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}
	}
}
