using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;
using DNA.Diagnostics.Instrumentation;

namespace DNA.Runtime.InteropServices.Win32.IPHelper
{
	public static class IPHelperAPI
	{
		private struct IPAddressString
		{
			public IntPtr Next;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
			public string IPAddress;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
			public string IpMask;
		}

		private struct IPAdapterInfo
		{
			public IntPtr Next;
			public uint ComboIndex;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string AdapterName;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 132)]
			public string Description;

			public uint AddressLength;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
			public byte[] Address;

			public uint Index;
			public uint Type;
			public uint DhcpEnabled;
			public IntPtr CurrentIpAddress;
			public IPHelperAPI.IPAddressString IpAddressList;
			public IPHelperAPI.IPAddressString GatewayList;
			public IPHelperAPI.IPAddressString DhcpServer;
			public bool HaveWins;
			public IPHelperAPI.IPAddressString PrimaryWinsServer;
			public IPHelperAPI.IPAddressString SecondaryWinsServer;
			public uint LeaseObtained;
			public uint LeaseExpires;
		}

		[DllImport("Iphlpapi.dll")]
		private static extern Win32ErrorCode GetAdaptersInfo(IntPtr data, ref uint pOutBufLen);

		public static NetworkAdapterInfo[] GetAdaptersInfo()
		{
			List<NetworkAdapterInfo> list = new List<NetworkAdapterInfo>();
			uint size = 0U;
			
			if (IPHelperAPI.GetAdaptersInfo(IntPtr.Zero, ref size) == Win32ErrorCode.Success)
			{
				return new NetworkAdapterInfo[0];
			}
			
			GlobalBuffer globalBuffer = new GlobalBuffer(size);
			Win32ErrorCode adaptersInfo = IPHelperAPI.GetAdaptersInfo(globalBuffer.Pointer, ref size);
			
			if (adaptersInfo != Win32ErrorCode.Success)
			{
				throw new Win32Exception((int)adaptersInfo);
			}
			
			Marshal.SizeOf(typeof(IPHelperAPI.IPAdapterInfo));
			IntPtr intPtr = globalBuffer.Pointer;
			
			do
			{
				IPHelperAPI.IPAdapterInfo ipadapterInfo = (IPHelperAPI.IPAdapterInfo)Marshal.PtrToStructure(intPtr, typeof(IPHelperAPI.IPAdapterInfo));
				
				ulong num = 0UL;
				int num2 = 0;
				
				while ((long)num2 < (long)((ulong)ipadapterInfo.AddressLength))
				{
					ulong num3 = (ulong)ipadapterInfo.Address[num2] << num2 * 8;
					num |= num3;
					num2++;
				}
				
				string text = "";
				
				for (int i = 0; i < 6; i++)
				{
					text += (num >> 8 * i & 255UL).ToString("X2");
				
					if (i != 5)
					{
						text += ":";
					}
				}
				
				list.Add(new NetworkAdapterInfo(text, new IPAddress[0], true));
				intPtr = ipadapterInfo.Next;
			}
			while (intPtr != IntPtr.Zero);
			
			return list.ToArray();
		}
	}
}
