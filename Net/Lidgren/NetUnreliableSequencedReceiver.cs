using System;

namespace DNA.Net.Lidgren
{
	internal sealed class NetUnreliableSequencedReceiver 
		: NetReceiverChannelBase
	{
		private int m_lastReceivedSequenceNumber;

		public NetUnreliableSequencedReceiver(NetConnection connection) 
			: base(connection) {}

		internal override void ReceiveMessage(NetIncomingMessage msg)
		{
			int sequenceNumber = msg.m_sequenceNumber;
			
			this.m_connection.QueueAck(
				msg.m_receivedMessageType, sequenceNumber);
			
			int num = NetUtility.RelativeSequenceNumber(
				sequenceNumber, this.m_lastReceivedSequenceNumber);
			
			if (num < 0)
			{
				return;
			}
			
			this.m_lastReceivedSequenceNumber = sequenceNumber;
			this.m_peer.ReleaseMessage(msg);
		}
	}
}
