using System;

namespace DNA.Runtime.InteropServices.Win32
{
	public enum DeviceIOFileAccess : uint
	{
		Any,
		Special = 0U,
		Read,
		Write
	}
}
