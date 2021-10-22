using System;

namespace DNA.Net.GamerServices
{
	public class HostDiscoveryResponseMessage : VersionCheckedMessage
	{
		public NetworkSession.ResultCode Result;
		public string Message;
		public int RequestID;
		public int SessionID;
		public int CurrentPlayers;
		public int MaxPlayers;
		public bool PasswordProtected;
		public NetworkSessionProperties SessionProperties;
		public string HostUsername;
	}
}
