using System;

namespace DNA.Net.GamerServices
{
	public enum NetworkSessionEndReason
	{
		ClientSignedOut,
		HostEndedSession,
		RemovedByHost,
		Disconnected
	}
}
