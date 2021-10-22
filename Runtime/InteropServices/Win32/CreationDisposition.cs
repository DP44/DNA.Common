using System;

namespace DNA.Runtime.InteropServices.Win32
{
	[Flags]
	public enum CreationDisposition : uint
	{
		CreateNew = 1U,
		CreateAlways = 2U,
		OpenExisting = 3U,
		OpenAlways = 4U,
		TruncateExisting = 5U
	}
}
