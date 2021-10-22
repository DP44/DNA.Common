using System;

namespace DNA.Net.GamerServices
{
	[Flags]
	public enum SendDataOptions
	{
		None = 0,
		Reliable = 1,
		InOrder = 2,
		ReliableInOrder = 3,
		Chat = 4
	}
}
