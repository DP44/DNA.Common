using System;

namespace DNA.Net.GamerServices
{
	public class HostDiscoveryRequestMessage : VersionCheckedMessage
	{
		public int RequestID;
		public PlayerID PlayerID;
	}
}
