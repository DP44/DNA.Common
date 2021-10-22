using System;

namespace DNA.Runtime.InteropServices.Win32
{
	public struct Win32MemoryStatusEx
	{
		public uint Length;
		public uint MemoryLoad;
		public ulong TotalPhysical;
		public ulong AvailiblePhysical;
		public ulong TotalPageFile;
		public ulong AvailiblePageFile;
		public ulong TotalVirtual;
		public ulong AvailibleVirtual;
		public ulong AvailibleExtendedVirtual;
	}
}
