using System;

namespace DNA.Runtime.InteropServices.Win32
{
	[Flags]
	public enum Win32FileFlags : uint
	{
		None = 0U,
		ReadOnly = 1U,
		Hidden = 2U,
		System = 4U,
		Directory = 16U,
		Archive = 32U,
		Device = 64U,
		Normal = 128U,
		Temporary = 256U,
		SparseFile = 512U,
		ReParsePoint = 1024U,
		Compressed = 2048U,
		OffLine = 4096U,
		NotContentIndexed = 8192U,
		Encrypted = 16384U,
		WriteThrough = 2147483648U,
		Overlapped = 1073741824U,
		NoBuffering = 536870912U,
		RandomAccess = 268435456U,
		SequentialScan = 134217728U,
		DeleteOnClose = 67108864U,
		BackupSemantics = 33554432U,
		PoxisSemantics = 16777216U,
		OpenReparsePoint = 2097152U,
		OpenNorecall = 1048576U,
		FirstPipeInstance = 524288U
	}
}
