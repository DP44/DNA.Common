using System;
using System.Net;
using System.Net.Sockets;
using DNA.Net.MatchMaking;

namespace DNA.Net.GamerServices
{
	public sealed class AvailableNetworkSession
	{
		private IPEndPoint _endPoint;
		private string _hostGamerTag;
		private string _hostMessage;
		private int _sessionID;
		private int _maxPlayers;
		private int _currentPlayers;
		private bool _passwordProtected;
		private int _friendCount;
		private int _proximity;

		private NetworkSessionProperties _properties;
		private DateTime _dateCreated;

		public ulong LobbySteamID;
		public ulong HostSteamID;

		public IPEndPoint IPEndPoint =>
			this.HostEndPoint;

		internal IPEndPoint HostEndPoint =>
			this._endPoint;

		public void ConvertToIPV4()
		{
			if (this.IPEndPoint != null && 
				this.IPEndPoint.Address.AddressFamily == AddressFamily.InterNetworkV6)
			{
				IPAddress address = IPAddress.Any;
				address = this.IPEndPoint.Address.MaptoIPV4();
				this._endPoint = new IPEndPoint(address, this._endPoint.Port);
			}
		}

		internal AvailableNetworkSession(IPEndPoint endPoint, string hostGamerTag, 
										 string hostMessage, int sessionID, 
										 NetworkSessionProperties props, 
										 int maxplayers, int currentPlayers, 
										 bool passwordProtected)
		{
			this._endPoint = endPoint;
			this._hostGamerTag = hostGamerTag;
			this._hostMessage = hostMessage;
			this._sessionID = sessionID;
			this._properties = props;
			this._maxPlayers = maxplayers;
			this._passwordProtected = passwordProtected;
			this._currentPlayers = currentPlayers;
			this._friendCount = 0;
			this._proximity = 0;
		}

		public AvailableNetworkSession(ClientSessionInfo clientSessionInfo)
		{
			if (clientSessionInfo.IPAddress != null)
			{
				this._endPoint = new IPEndPoint(
					clientSessionInfo.IPAddress, clientSessionInfo.NetworkPort);
			}
			else
			{
				this._endPoint = null;
			}

			this.LobbySteamID = clientSessionInfo.SteamLobbyID;
			this.HostSteamID = clientSessionInfo.SteamHostID;
			this._hostGamerTag = clientSessionInfo.HostUserName;
			this._hostMessage = clientSessionInfo.Name;
			this._sessionID = 0;
			this._properties = clientSessionInfo.SessionProperties;
			this._maxPlayers = clientSessionInfo.MaxPlayers;
			this._passwordProtected = clientSessionInfo.PasswordProtected;
			this._currentPlayers = clientSessionInfo.CurrentPlayers;
			this._dateCreated = clientSessionInfo.DateCreated;
			this._friendCount = clientSessionInfo.NumFriends;
			this._proximity = clientSessionInfo.Proximity;
		}

		public DateTime DateCreated =>
			this._dateCreated;

		public int MaxGamerCount =>
			this._maxPlayers;

		public int CurrentGamerCount =>
			this._currentPlayers;

		public int FriendCount =>
			this._friendCount;

		public int Proximity =>
			this._proximity;

		public string HostGamertag =>
			this._hostGamerTag;

		public string ServerMessage =>
			this._hostMessage;

		public int SessionID =>
			this._sessionID;

		public int OpenPrivateGamerSlots =>
			throw new NotImplementedException();

		public int OpenPublicGamerSlots =>
			throw new NotImplementedException();

		public bool PasswordProtected =>
			this._passwordProtected;

		public QualityOfService QualityOfService =>
			throw new NotImplementedException();

		public NetworkSessionProperties SessionProperties =>
			this._properties;
	}
}
