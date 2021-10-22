using System;

namespace DNA.Runtime.InteropServices.Win32
{
	public enum DeviceIOMethod : uint
	{
		Buffered,
		InDirect,
		OutDirect,
		Neither,
		DirectToHardware = 2U,
		DirectFromHardware = 1U
	}
}
