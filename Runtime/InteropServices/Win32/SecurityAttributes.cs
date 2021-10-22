using System;
using System.Runtime.InteropServices;

namespace DNA.Runtime.InteropServices.Win32
{
	[StructLayout(LayoutKind.Sequential)]
	public class SecurityAttributes
	{
		public uint Length;
		public IntPtr SecurityDescriptor;
		public bool InheritHandle;
	}
}
