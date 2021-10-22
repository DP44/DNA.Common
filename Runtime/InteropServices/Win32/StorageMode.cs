using System;

namespace DNA.Runtime.InteropServices.Win32
{
	[Flags]
	public enum StorageMode : uint
	{
		Direct = 0U,
		Transacted = 65536U,
		Simple = 134217728U,
		Read = 0U,
		Write = 1U,
		ReadWrite = 2U,
		ShareDenyNone = 64U,
		ShareDenyRead = 48U,
		ShareDenyWrite = 32U,
		ShareExclusive = 16U,
		Priority = 262144U,
		DeleteOnRelease = 67108864U,
		NoScratch = 1048576U,
		Create = 4096U,
		Convert = 131072U,
		NoSnapShot = 2097152U,
		DirectSWMR = 4194304U
	}
}
