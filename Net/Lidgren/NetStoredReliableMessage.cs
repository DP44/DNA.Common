using System;

namespace DNA.Net.Lidgren
{
	internal struct NetStoredReliableMessage
	{
		public int NumSent;
		public float LastSent;
		public NetOutgoingMessage Message;

		public void Reset()
		{
			this.NumSent = 0;
			this.LastSent = 0f;
			this.Message = null;
		}
	}
}
