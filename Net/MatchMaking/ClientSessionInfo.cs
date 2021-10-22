using System;
using System.Net;
using DNA.Net.GamerServices;

namespace DNA.Net.MatchMaking
{
	public class ClientSessionInfo
	{
		private NetworkSessionProperties _props =
			new NetworkSessionProperties();

		public string Name { get; set; }
		public Guid SessionID { get; set; }
		public bool PasswordProtected { get; set; }
		public bool IsPublic { get; set; }
		public JoinGamePolicy JoinGamePolicy { get; set; }
		public IPAddress IPAddress { get; set; }
		public int NetworkPort { get; set; }
		public string HostUserName { get; set; }
		public ulong SteamLobbyID { get; set; }
		public ulong SteamHostID { get; set; }
		public int MaxPlayers { get; set; }
		public int CurrentPlayers { get; set; }
		public int NumFriends { get; set; }
		public int Proximity { get; set; }
		public DateTime DateCreated { get; set; }

		public NetworkSessionProperties SessionProperties =>
			this._props;
	}
}
