using System;
using System.Diagnostics;
using System.Text;

namespace DNA.Net.Lidgren
{
	public sealed class NetPeerStatistics
	{
		private readonly NetPeer m_peer;

		internal int m_sentPackets;

		internal int m_receivedPackets;

		internal int m_sentMessages;

		internal int m_receivedMessages;

		internal int m_sentBytes;

		internal int m_receivedBytes;

		internal long m_bytesAllocated;

		internal NetPeerStatistics(NetPeer peer)
		{
			this.m_peer = peer;
			this.Reset();
		}

		internal void Reset()
		{
			this.m_sentPackets = 0;
			this.m_receivedPackets = 0;
			this.m_sentMessages = 0;
			this.m_receivedMessages = 0;
			this.m_sentBytes = 0;
			this.m_receivedBytes = 0;
			this.m_bytesAllocated = 0L;
		}

		public int SentPackets
		{
			get
			{
				return this.m_sentPackets;
			}
		}

		public int ReceivedPackets
		{
			get
			{
				return this.m_receivedPackets;
			}
		}

		public int SentMessages
		{
			get
			{
				return this.m_sentMessages;
			}
		}

		public int ReceivedMessages
		{
			get
			{
				return this.m_receivedMessages;
			}
		}

		public int SentBytes
		{
			get
			{
				return this.m_sentBytes;
			}
		}

		public int ReceivedBytes
		{
			get
			{
				return this.m_receivedBytes;
			}
		}

		public long StorageBytesAllocated
		{
			get
			{
				return this.m_bytesAllocated;
			}
		}

		public int BytesInRecyclePool
		{
			get
			{
				return this.m_peer.m_storagePoolBytes;
			}
		}

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

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(this.m_peer.ConnectionsCount.ToString() + " connections");
			stringBuilder.AppendLine("Sent (n/a) bytes in (n/a) messages in (n/a) packets");
			stringBuilder.AppendLine("Received (n/a) bytes in (n/a) messages in (n/a) packets");
			stringBuilder.AppendLine("Storage allocated " + this.m_bytesAllocated + " bytes");
			stringBuilder.AppendLine("Recycled pool " + this.m_peer.m_storagePoolBytes + " bytes");
			return stringBuilder.ToString();
		}
	}
}
