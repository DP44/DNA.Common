using System;
using System.Threading;

namespace DNA.Net.Lidgren
{
	internal sealed class NetReliableSenderChannel : NetSenderChannelBase
	{
		private NetConnection m_connection;

		private int m_windowStart;

		private int m_windowSize;

		private int m_sendStart;

		private NetBitVector m_receivedAcks;

		internal NetStoredReliableMessage[] m_storedMessages;

		internal float m_resendDelay;

		internal override int WindowSize
		{
			get
			{
				return this.m_windowSize;
			}
		}

		internal NetReliableSenderChannel(NetConnection connection, int windowSize)
		{
			this.m_connection = connection;
			this.m_windowSize = windowSize;
			this.m_windowStart = 0;
			this.m_sendStart = 0;
			this.m_receivedAcks = new NetBitVector(1024);
			this.m_storedMessages = new NetStoredReliableMessage[this.m_windowSize];
			this.m_queuedSends = new NetQueue<NetOutgoingMessage>(8);
			this.m_resendDelay = this.m_connection.GetResendDelay();
		}

		internal override int GetAllowedSends()
		{
			return this.m_windowSize - (this.m_sendStart + 1024 - this.m_windowStart) % 1024;
		}

		internal override void Reset()
		{
			this.m_receivedAcks.Clear();
			for (int i = 0; i < this.m_storedMessages.Length; i++)
			{
				this.m_storedMessages[i].Reset();
			}
			this.m_queuedSends.Clear();
			this.m_windowStart = 0;
			this.m_sendStart = 0;
		}

		internal override NetSendResult Enqueue(NetOutgoingMessage message)
		{
			this.m_queuedSends.Enqueue(message);
			int count = this.m_queuedSends.Count;
			int num = this.m_windowSize - (this.m_sendStart + 1024 - this.m_windowStart) % 1024;
			if (count <= num)
			{
				return NetSendResult.Sent;
			}
			return NetSendResult.Queued;
		}

		internal override void SendQueuedMessages(float now)
		{
			for (int i = 0; i < this.m_storedMessages.Length; i++)
			{
				NetOutgoingMessage message = this.m_storedMessages[i].Message;
				if (message != null)
				{
					float lastSent = this.m_storedMessages[i].LastSent;
					if (lastSent > 0f && now - lastSent > this.m_resendDelay)
					{
						int num = this.m_windowStart % this.m_windowSize;
						int num2 = this.m_windowStart;
						while (num != i)
						{
							num--;
							if (num < 0)
							{
								num = this.m_windowSize - 1;
							}
							num2--;
						}
						this.m_connection.QueueSendMessage(message, num2);
						this.m_storedMessages[i].LastSent = now;
						NetStoredReliableMessage[] storedMessages = this.m_storedMessages;
						int num3 = i;
						storedMessages[num3].NumSent = storedMessages[num3].NumSent + 1;
					}
				}
			}
			int num4 = this.GetAllowedSends();
			if (num4 < 1)
			{
				return;
			}
			while (this.m_queuedSends.Count > 0 && num4 > 0)
			{
				NetOutgoingMessage message2;
				if (this.m_queuedSends.TryDequeue(out message2))
				{
					this.ExecuteSend(now, message2);
				}
				num4--;
			}
		}

		private void ExecuteSend(float now, NetOutgoingMessage message)
		{
			int sendStart = this.m_sendStart;
			this.m_sendStart = (this.m_sendStart + 1) % 1024;
			this.m_connection.QueueSendMessage(message, sendStart);
			int num = sendStart % this.m_windowSize;
			NetStoredReliableMessage[] storedMessages = this.m_storedMessages;
			int num2 = num;
			storedMessages[num2].NumSent = storedMessages[num2].NumSent + 1;
			this.m_storedMessages[num].Message = message;
			this.m_storedMessages[num].LastSent = now;
		}

		private void DestoreMessage(int storeIndex)
		{
			NetOutgoingMessage message = this.m_storedMessages[storeIndex].Message;
			if (message != null)
			{
				Interlocked.Decrement(ref message.m_recyclingCount);
				if (message.m_recyclingCount <= 0)
				{
					this.m_connection.m_peer.Recycle(message);
				}
			}
			this.m_storedMessages[storeIndex] = default(NetStoredReliableMessage);
		}

		internal override void ReceiveAcknowledge(float now, int seqNr)
		{
			int num = NetUtility.RelativeSequenceNumber(seqNr, this.m_windowStart);
			if (num < 0)
			{
				return;
			}
			if (num == 0)
			{
				this.m_receivedAcks[this.m_windowStart] = false;
				this.DestoreMessage(this.m_windowStart % this.m_windowSize);
				this.m_windowStart = (this.m_windowStart + 1) % 1024;
				while (this.m_receivedAcks.Get(this.m_windowStart))
				{
					this.m_receivedAcks[this.m_windowStart] = false;
					this.DestoreMessage(this.m_windowStart % this.m_windowSize);
					this.m_windowStart = (this.m_windowStart + 1) % 1024;
				}
				return;
			}
			int num2 = NetUtility.RelativeSequenceNumber(seqNr, this.m_sendStart);
			if (num2 <= 0)
			{
				if (!this.m_receivedAcks[seqNr])
				{
					this.m_receivedAcks[seqNr] = true;
				}
			}
			else if (num2 > 0)
			{
				return;
			}
			int num3 = seqNr;
			do
			{
				num3--;
				if (num3 < 0)
				{
					num3 = 1023;
				}
				if (!this.m_receivedAcks[num3])
				{
					int num4 = num3 % this.m_windowSize;
					if (this.m_storedMessages[num4].NumSent == 1)
					{
						NetOutgoingMessage message = this.m_storedMessages[num4].Message;
						if (now - this.m_storedMessages[num4].LastSent >= this.m_resendDelay * 0.35f)
						{
							this.m_storedMessages[num4].LastSent = now;
							NetStoredReliableMessage[] storedMessages = this.m_storedMessages;
							int num5 = num4;
							storedMessages[num5].NumSent = storedMessages[num5].NumSent + 1;
							this.m_connection.QueueSendMessage(message, num3);
						}
					}
				}
			}
			while (num3 != this.m_windowStart);
		}
	}
}
