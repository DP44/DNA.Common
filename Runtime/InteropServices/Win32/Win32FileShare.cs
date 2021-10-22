using System;

namespace DNA.Runtime.InteropServices.Win32
{
	[Flags]
	public enum Win32FileShare : uint
	{
		ShareRead = 1U,
		ShareWrite = 2U,
		ShareDelete = 4U
	}
}
