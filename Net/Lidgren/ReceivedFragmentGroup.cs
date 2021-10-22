using System;

namespace DNA.Net.Lidgren
{
	internal class ReceivedFragmentGroup
	{
		public float LastReceived;
		public byte[] Data;
		public NetBitVector ReceivedChunks;
	}
}
