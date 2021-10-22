using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace DNA.Runtime.InteropServices.Win32
{
	public static class Win32API
	{
		public const int MaxPath = 260;

		private static readonly IntPtr InvalidFileHandle = new IntPtr(-1);

		[DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
		private static extern void CopyMemory(byte[] Destination, byte[] Source, uint Length);

		[DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
		private static extern void CopyMemoryUint(uint[] Destination, uint[] Source, uint Length);

		[DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
		private static extern void CopyMemoryUint(uint[] Destination, IntPtr Source, uint Length);

		[DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
		private static extern void CopyMemoryUint(IntPtr Destination, uint[] Source, uint Length);

		[DllImport("kernel32.dll")]
		public static extern IntPtr LoadLibrary(string lpFileName);

		[DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

		[DllImport("kernel32.dll", EntryPoint = "GlobalMemoryStatusEx", ExactSpelling = true)]
		private static extern bool _GlobalMemoryStatusEx(ref Win32MemoryStatusEx lpBuffer);

		[DllImport("Kernel32.dll", CharSet = CharSet.Ansi, EntryPoint = "CreateFileA", SetLastError = true)]
		private static extern IntPtr _CreateFile(string fileName, Win32FileAccess access, Win32FileShare share, SecurityAttributes security, CreationDisposition dwCreationDisposition, Win32FileFlags flags, IntPtr templateFile);

		[DllImport("Kernel32.dll", EntryPoint = "CloseHandle", SetLastError = true)]
		private static extern bool _CloseHandle(IntPtr Handle);

		[DllImport("Kernel32.dll", EntryPoint = "DeviceIoControl", SetLastError = true)]
		private static extern bool _DeviceIoControl(IntPtr hDevice, IOControlCode dwIoControlCode, IntPtr InBuffer, uint InBufferSize, IntPtr OutBuffer, uint OutBufferSize, out uint BytesReturned, Overlapped overlapped);

		public static void CopyMemory(uint[] Destination, uint[] Source, uint Length)
		{
			Win32API.CopyMemoryUint(Destination, Source, Length * 4U);
		}

		public static void CopyMemory(IntPtr Destination, uint[] Source, uint Length)
		{
			Win32API.CopyMemoryUint(Destination, Source, Length * 4U);
		}

		public static void CopyMemory(uint[] Destination, IntPtr Source, uint Length)
		{
			Win32API.CopyMemoryUint(Destination, Source, Length * 4U);
		}

		public static Win32MemoryStatusEx GlobalMemoryStatus()
		{
			Win32MemoryStatusEx memoryStats = default(Win32MemoryStatusEx);
			memoryStats.Length = (uint)Marshal.SizeOf(typeof(Win32MemoryStatusEx));
			
			if (!Win32API._GlobalMemoryStatusEx(ref memoryStats))
			{
				throw new Win32Exception();
			}

			return memoryStats;
		}

		public static IntPtr CreateFile(string fileName, Win32FileAccess access, Win32FileShare share, SecurityAttributes security, CreationDisposition dwCreationDisposition, Win32FileFlags flags, IntPtr templateFile)
		{
			IntPtr fileHandle = Win32API._CreateFile(fileName, access, share, security, dwCreationDisposition, flags, templateFile);
			
			if (fileHandle == Win32API.InvalidFileHandle)
			{
				throw new Win32Exception();
			}
			
			return fileHandle;
		}

		public static void CloseHandle(IntPtr handle)
		{
			if (!Win32API._CloseHandle(handle))
			{
				throw new Win32Exception();
			}
		}

		public static uint DeviceIoControl(IntPtr hDevice, IOControlCode dwIoControlCode, IntPtr InBuffer, uint InBufferSize, IntPtr OutBuffer, uint OutBufferSize, Overlapped overlapped)
		{
			uint result;
			
			if (!Win32API._DeviceIoControl(hDevice, dwIoControlCode, InBuffer, InBufferSize, OutBuffer, OutBufferSize, out result, overlapped))
			{
				throw new Win32Exception();
			}
			
			return result;
		}
	}
}
