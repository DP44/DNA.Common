using System;

namespace DNA.Net.GamerServices
{
	public class RequestConnectToHostMessage : VersionCheckedMessage
	{
		public int SessionID;
		public string Password;
		public Gamer Gamer;
		public NetworkSessionProperties SessionProperties;
	}
}
