using System;
using System.Diagnostics;
using System.Text;

namespace DNA.Net.Lidgren
{
	public sealed class NetConnectionStatistics
	{
		private readonly NetConnection m_connection;

		internal int m_sentPackets;
		internal int m_receivedPackets;
		internal int m_sentMessages;
		internal int m_receivedMessages;
		internal int m_sentBytes;
		internal int m_receivedBytes;
		internal int m_resentMessagesDueToDelay;
		internal int m_resentMessagesDueToHole;

		internal NetConnectionStatistics(NetConnection conn)
		{
			this.m_connection = conn;
			this.Reset();
		}

		internal void Reset()
		{
			this.m_sentPackets = 0;
			this.m_receivedPackets = 0;
			this.m_sentBytes = 0;
			this.m_receivedBytes = 0;
		}

		public int SentPackets =>
			this.m_sentPackets;

		public int ReceivedPackets =>
			this.m_receivedPackets;

		public int SentBytes =>
			this.m_sentBytes;

		public int ReceivedBytes =>
			this.m_receivedBytes;

		public int ResentMessages =>
			this.m_resentMessagesDueToHole + this.m_resentMessagesDueToDelay;

		[Conditional("DEBUG")]
		internal void PacketSent(int numBytes, int numMessages)
		{
			this.m_sentPackets++;
			this.m_sentBytes += numBytes;
			this.m_sentMessages += numMessages;
		}

		[Conditional("DEBUG")]
		internal void PacketReceived(int numBytes, int numMessages)
		{
			this.m_receivedPackets++;
			this.m_receivedBytes += numBytes;
			this.m_receivedMessages += numMessages;
		}

		[Conditional("DEBUG")]
		internal void MessageResent(MessageResendReason reason)
		{
			if (reason == MessageResendReason.Delay)
			{
				this.m_resentMessagesDueToDelay++;
			}
			else
			{
				this.m_resentMessagesDueToHole++;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(
				string.Concat("Sent ", this.m_sentBytes, " bytes in ", this.m_sentMessages, 
					" messages in ", this.m_sentPackets, " packets"));
			
			stringBuilder.AppendLine(
				string.Concat("Received ", this.m_receivedBytes, " bytes in ", 
					this.m_receivedMessages, " messages in ", this.m_receivedPackets, " packets"));
		
			if (this.m_resentMessagesDueToDelay > 0)
			{
				stringBuilder.AppendLine(
					"Resent messages (delay): " + this.m_resentMessagesDueToDelay);
			}

			if (this.m_resentMessagesDueToDelay > 0)
			{
				stringBuilder.AppendLine(
					"Resent messages (holes): " + this.m_resentMessagesDueToHole);
			}

			int num = 0;
			int num2 = 0;
			
			foreach (NetSenderChannelBase netSenderChannelBase in this.m_connection.m_sendChannels)
			{
				if (netSenderChannelBase != null)
				{
					num += netSenderChannelBase.m_queuedSends.Count;
					
					NetReliableSenderChannel netReliableSenderChannel = 
						netSenderChannelBase as NetReliableSenderChannel;
			
					if (netReliableSenderChannel != null)
					{
						for (int j = 0; j < netReliableSenderChannel.m_storedMessages.Length; j++)
						{
							if (netReliableSenderChannel.m_storedMessages[j].Message != null)
							{
								num2++;
							}
						}
					}
				}
			}
			
			int num3 = 0;
			
			foreach (NetReceiverChannelBase netReceiverChannelBase 
				in this.m_connection.m_receiveChannels)
			{
				NetReliableOrderedReceiver netReliableOrderedReceiver = 
					netReceiverChannelBase as NetReliableOrderedReceiver;
				
				if (netReliableOrderedReceiver != null)
				{
					for (int l = 0; l < netReliableOrderedReceiver.m_withheldMessages.Length; l++)
					{
						if (netReliableOrderedReceiver.m_withheldMessages[l] != null)
						{
							num3++;
						}
					}
				}
			}

			stringBuilder.AppendLine("Unsent messages: " + num);
			stringBuilder.AppendLine("Stored messages: " + num2);
			stringBuilder.AppendLine("Withheld messages: " + num3);

			return stringBuilder.ToString();
		}
	}
}
