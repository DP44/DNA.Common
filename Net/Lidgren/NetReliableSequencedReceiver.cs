using System;

namespace DNA.Net.Lidgren
{
	internal sealed class NetReliableSequencedReceiver : NetReceiverChannelBase
	{
		private int m_windowStart;

		private int m_windowSize;

		public NetReliableSequencedReceiver(NetConnection connection, int windowSize) : base(connection)
		{
			this.m_windowSize = windowSize;
		}

		private void AdvanceWindow()
		{
			this.m_windowStart = (this.m_windowStart + 1) % 1024;
		}

		internal override void ReceiveMessage(NetIncomingMessage message)
		{
			int sequenceNumber = message.m_sequenceNumber;
			int num = NetUtility.RelativeSequenceNumber(sequenceNumber, this.m_windowStart);
			this.m_connection.QueueAck(message.m_receivedMessageType, sequenceNumber);
			if (num == 0)
			{
				this.AdvanceWindow();
				this.m_peer.ReleaseMessage(message);
				return;
			}
			if (num < 0)
			{
				return;
			}
			if (num > this.m_windowSize)
			{
				return;
			}
			this.m_windowStart = (this.m_windowStart + num) % 1024;
			this.m_peer.ReleaseMessage(message);
		}
	}
}
