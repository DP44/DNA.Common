using System;

namespace DNA.Net.Lidgren
{
	public enum NetDeliveryMethod : byte
	{
		Unknown,
		Unreliable,
		UnreliableSequenced,
		ReliableUnordered = 34,
		ReliableSequenced,
		ReliableOrdered = 67
	}
}
