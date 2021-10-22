using System;

namespace DNA.Net.Lidgren
{
	public enum NetIncomingMessageType
	{
		Error,
		StatusChanged,
		UnconnectedData,
		ConnectionApproval = 4,
		Data = 8,
		Receipt = 16,
		DiscoveryRequest = 32,
		DiscoveryResponse = 64,
		VerboseDebugMessage = 128,
		DebugMessage = 256,
		WarningMessage = 512,
		ErrorMessage = 1024,
		NatIntroductionSuccess = 2048,
		ConnectionLatencyUpdated = 4096
	}
}
