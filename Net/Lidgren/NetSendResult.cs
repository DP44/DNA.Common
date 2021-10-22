using System;

namespace DNA.Net.Lidgren
{
	public enum NetSendResult
	{
		FailedNotConnected,
		Sent,
		Queued,
		Dropped
	}
}
