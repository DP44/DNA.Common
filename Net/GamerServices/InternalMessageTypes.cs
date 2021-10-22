using System;

namespace DNA.Net.GamerServices
{
	public enum InternalMessageTypes
	{
		NewPeer,
		ResponseToConnection,
		DropPeer,
		ForwardMessage,
		BroadcastMessage,
		SessionPropertiesChanged
	}
}
