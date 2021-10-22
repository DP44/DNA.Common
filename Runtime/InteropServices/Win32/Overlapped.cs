using System;
using System.Runtime.InteropServices;

namespace DNA.Runtime.InteropServices.Win32
{
	[StructLayout(LayoutKind.Sequential)]
	public class Overlapped
	{
		public IntPtr Internal;
		public IntPtr InternalHigh;
		public ulong Offset;
		public IntPtr hEvent;
	}
}
