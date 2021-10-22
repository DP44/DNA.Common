using System;
using DNA.Net.GamerServices;

namespace DNA.Net.MatchMaking
{
	public class CreateSessionInfo
	{
		public NetworkSessionProperties SessionProperties = 
			new NetworkSessionProperties();

		public string Name { get; set; }
		public bool PasswordProtected { get; set; }
		public bool IsPublic { get; set; }
		public JoinGamePolicy JoinGamePolicy { get; set; }
		public int MaxPlayers { get; set; }
		public int NetworkPort { get; set; }
	}
}
