using System;

namespace DNA.Net.Lidgren
{
	public enum NetConnectionStatus
	{
		None,
		InitiatedConnect,
		ReceivedInitiation,
		RespondedAwaitingApproval,
		RespondedConnect,
		Connected,
		Disconnecting,
		Disconnected
	}
}
