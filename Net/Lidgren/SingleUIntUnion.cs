using System;
using System.Runtime.InteropServices;

namespace DNA.Net.Lidgren
{
	[StructLayout(LayoutKind.Explicit)]
	public struct SingleUIntUnion
	{
		[FieldOffset(0)]
		public float SingleValue;

		[CLSCompliant(false), FieldOffset(0)]
		public uint UIntValue;
	}
}
