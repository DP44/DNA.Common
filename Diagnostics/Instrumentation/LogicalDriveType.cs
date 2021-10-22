using System;

namespace DNA.Diagnostics.Instrumentation
{
	public enum LogicalDriveType
	{
		Unknown,
		NoRootDirectory,
		RemovableDisk,
		LocalDisk,
		NetworkDrive,
		CompactDisc,
		RAMDisk
	}
}
