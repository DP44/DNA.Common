using System;

namespace DNA.Net.Lidgren
{
	internal sealed class NetUnreliableUnorderedReceiver : NetReceiverChannelBase
	{
		public NetUnreliableUnorderedReceiver(NetConnection connection) 
			: base(connection)
		{
		}

		internal override void ReceiveMessage(NetIncomingMessage msg)
		{
			this.m_connection.QueueAck(
				msg.m_receivedMessageType, msg.m_sequenceNumber);
			
			this.m_peer.ReleaseMessage(msg);
		}
	}
}
