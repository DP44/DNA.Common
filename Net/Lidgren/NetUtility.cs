using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace DNA.Net.Lidgren
{
	public static class NetUtility
	{
		public delegate void ResolveEndPointCallback(IPEndPoint endPoint);
		public delegate void ResolveAddressCallback(IPAddress adr);

		public static SocketError LastResolveResult;

		public static void ResolveAsync(string ipOrHost, int port, 
										NetUtility.ResolveEndPointCallback callback)
		{
			NetUtility.ResolveAsync(ipOrHost, (NetUtility.ResolveAddressCallback)(addr =>
			{
				if (addr == null)
				{
					callback(null);
				}
				else
				{
					callback(new IPEndPoint(adr, port));
				}
			}));
		}

		public static IPEndPoint Resolve(string ipOrHost, int port) => 
			new IPEndPoint(NetUtility.Resolve(ipOrHost), port);

		public static void ResolveAsync(string ipOrHost, 
			NetUtility.ResolveAddressCallback callback)
		{
			NetUtility.LastResolveResult = SocketError.Success;
		
			if (string.IsNullOrEmpty(ipOrHost))
			{
				callback(null);
			}

			ipOrHost = ipOrHost.Trim();
			
			if (string.IsNullOrEmpty(ipOrHost))
			{
				callback(null);
			}

			IPAddress address1 = null;
			
			if (IPAddress.TryParse(ipOrHost, out address1))
			{
				if (address1.AddressFamily != AddressFamily.InterNetwork)
				{
					throw new ArgumentException(
						"This method will not currently resolve other than ipv4 addresses");
				}

				callback(address1);
			}
			else
			{
				try
				{
					IPHostEntry entry;
					
					Dns.BeginGetHostEntry(ipOrHost, (AsyncCallback)(result =>
					{
						try
						{
							entry = Dns.EndGetHostEntry(result);
						}
						catch (SocketException ex)
						{
							NetUtility.LastResolveResult = ex.SocketErrorCode;
							entry = null;
						}
					
						if (entry == null)
						{
							callback(null);
						}
						else
						{
							foreach (IPAddress address5 in entry.AddressList)
							{
								if (address5.AddressFamily == AddressFamily.InterNetwork)
								{
									callback(address5);
									return;
								}
							}
						
							callback(null);
						}
					}), null);
				}
				catch (SocketException ex)
				{
					NetUtility.LastResolveResult = ex.SocketErrorCode;
					callback(null);
				}
			}
		}

		public static IPAddress Resolve(string ipOrHost)
		{
			NetUtility.LastResolveResult = SocketError.Success;
		
			if (string.IsNullOrEmpty(ipOrHost))
			{
				return null;
			}
		
			ipOrHost = ipOrHost.Trim();
		
			if (string.IsNullOrEmpty(ipOrHost))
			{
				return null;
			}
		
			IPAddress ipaddress = null;
		
			if (!IPAddress.TryParse(ipOrHost, out ipaddress))
			{
				IPAddress result;
				
				try
				{
					IPHostEntry hostEntry = Dns.GetHostEntry(ipOrHost);
					
					if (hostEntry == null)
					{
						result = null;
					}
					else
					{
						foreach (IPAddress ipaddress2 in hostEntry.AddressList)
						{
							if (ipaddress2.AddressFamily == AddressFamily.InterNetwork)
							{
								return ipaddress2;
							}
						}

						result = null;
					}
				}
				catch (SocketException ex)
				{
					NetUtility.LastResolveResult = ex.SocketErrorCode;
					result = null;
				}
		
				return result;
			}
		
			if (ipaddress.AddressFamily == AddressFamily.InterNetwork)
			{
				return ipaddress;
			}
		
			return null;
		}

		private static NetworkInterface GetNetworkInterface()
		{
			if (IPGlobalProperties.GetIPGlobalProperties() == null)
			{
				return null;
			}
			
			NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			
			if (allNetworkInterfaces == null || allNetworkInterfaces.Length < 1)
			{
				return null;
			}
			
			NetworkInterface networkInterface = null;
			
			foreach (NetworkInterface networkInterface2 in allNetworkInterfaces)
			{
				if (networkInterface2.NetworkInterfaceType != NetworkInterfaceType.Loopback && 
					networkInterface2.NetworkInterfaceType != NetworkInterfaceType.Unknown && 
					networkInterface2.Supports(NetworkInterfaceComponent.IPv4))
				{
					if (networkInterface == null)
					{
						networkInterface = networkInterface2;
					}
			
					if (networkInterface2.OperationalStatus == OperationalStatus.Up)
					{
						return networkInterface2;
					}
				}
			}
		
			return networkInterface;
		}

		public static PhysicalAddress GetMacAddress() => 
			NetUtility.GetNetworkInterface()?.GetPhysicalAddress();

		public static string ToHexString(long data) => 
			NetUtility.ToHexString(BitConverter.GetBytes(data));

		public static string ToHexString(byte[] data)
		{
			char[] array = new char[data.Length * 2];
			
			for (int i = 0; i < data.Length; i++)
			{
				byte b = (byte)(data[i] >> 4);
				array[i * 2] = (char)((b > 9) ? (b + 55) : (b + 48));
				b = (data[i] & 15);
				array[i * 2 + 1] = (char)((b > 9) ? (b + 55) : (b + 48));
			}
			
			return new string(array);
		}

		public static IPAddress GetBroadcastAddress()
		{
			try
			{
				NetworkInterface networkInterface = NetUtility.GetNetworkInterface();

				if (networkInterface == null)
				{
					return null;
				}
			
				IPInterfaceProperties ipproperties = networkInterface.GetIPProperties();
			
				foreach (UnicastIPAddressInformation unicastIPAddressInformation 
					in ipproperties.UnicastAddresses)
				{
					if (unicastIPAddressInformation != null && 
						unicastIPAddressInformation.Address != null && 
						unicastIPAddressInformation.Address.AddressFamily == 
							AddressFamily.InterNetwork)
					{
						IPAddress ipv4Mask = unicastIPAddressInformation.IPv4Mask;
						
						byte[] addressBytes = 
							unicastIPAddressInformation.Address.GetAddressBytes();
						
						byte[] addressBytes2 = ipv4Mask.GetAddressBytes();
						
						if (addressBytes.Length != addressBytes2.Length)
						{
							throw new ArgumentException(
								"Lengths of IP address and subnet mask do not match.");
						}
					
						byte[] array = new byte[addressBytes.Length];
						
						for (int i = 0; i < array.Length; i++)
						{
							array[i] = (addressBytes[i] | (addressBytes2[i] ^ byte.MaxValue));
						}
					
						return new IPAddress(array);
					}
				}
			}
			catch
			{
				return IPAddress.Broadcast;
			}
			
			return IPAddress.Broadcast;
		}

		public static IPAddress GetMyAddress(out IPAddress mask)
		{
			mask = null;
			NetworkInterface networkInterface = NetUtility.GetNetworkInterface();
		
			if (networkInterface == null)
			{
				mask = null;
				return null;
			}
		
			IPInterfaceProperties ipproperties = networkInterface.GetIPProperties();

			foreach (UnicastIPAddressInformation unicastIPAddressInformation 
				in ipproperties.UnicastAddresses)
			{
				if (unicastIPAddressInformation != null && 
					unicastIPAddressInformation.Address != null && 
					unicastIPAddressInformation.Address.AddressFamily == 
						AddressFamily.InterNetwork)
				{
					mask = unicastIPAddressInformation.IPv4Mask;
					return unicastIPAddressInformation.Address;
				}
			}
	
			return null;
		}

		public static bool IsLocal(IPEndPoint endPoint) => 
			endPoint != null && NetUtility.IsLocal(endPoint.Address);

		public static bool IsLocal(IPAddress remote)
		{
			IPAddress mask;
			IPAddress myAddress = NetUtility.GetMyAddress(out mask);
		
			if (mask == null)
			{
				return false;
			}
		
			uint uMask = BitConverter.ToUInt32(mask.GetAddressBytes(), 0);
			uint uRemoteAddr = BitConverter.ToUInt32(remote.GetAddressBytes(), 0);
			uint uSourceAddr = BitConverter.ToUInt32(myAddress.GetAddressBytes(), 0);
			return (uRemoteAddr & uMask) == (uSourceAddr & uMask);
		}

		[CLSCompliant(false)]
		public static int BitsToHoldUInt(uint value)
		{
			int bitsToHold = 1;
			
			while ((value >>= 1) != 0U)
			{
				bitsToHold++;
			}

			return bitsToHold;
		}

		public static int BytesToHoldBits(int numBits) =>
			(numBits + 7) / 8;

		internal static uint SwapByteOrder(uint value) =>
			(value & 4278190080U) >> 24 | (value & 16711680U) >> 8 | 
			(value & 65280U) << 8 | (value & 255U) << 24;

		internal static ulong SwapByteOrder(ulong value) =>
			(value & 18374686479671623680UL) >> 56 | (value & 71776119061217280UL) >> 40 | 
			(value & 280375465082880UL) >> 24 | (value & 1095216660480UL) >> 8 | 
			(value & (ulong)-16777216) << 8 | (value & 16711680UL) << 24 | 
			(value & 65280UL) << 40 | (value & 255UL) << 56;

		internal static bool CompareElements(byte[] one, byte[] two)
		{
			if (one.Length != two.Length)
			{
				return false;
			}
			
			for (int i = 0; i < one.Length; i++)
			{
				if (one[i] != two[i])
				{
					return false;
				}
			}

			return true;
		}

		public static byte[] ToByteArray(string hexString)
		{
			byte[] array = new byte[hexString.Length / 2];
			
			for (int i = 0; i < hexString.Length; i += 2)
			{
				array[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
			}
			
			return array;
		}

		public static string ToHumanReadable(long bytes)
		{
			if (bytes < 4000L)
			{
				return bytes + " bytes";
			}
			
			if (bytes < 1000000L)
			{
				return Math.Round((double)bytes / 1000.0, 2) + " kilobytes";
			}
		
			return Math.Round((double)bytes / 1000000.0, 2) + " megabytes";
		}

		internal static int RelativeSequenceNumber(int nr, int expected) =>
			(nr - expected + 1024 + 512) % 1024 - 512;

		public static int GetWindowSize(NetDeliveryMethod method)
		{
			switch (method)
			{
				case NetDeliveryMethod.Unknown:
				{
					return 0;
				}

				case NetDeliveryMethod.Unreliable:
				case NetDeliveryMethod.UnreliableSequenced:
				{
					return 128;
				}

				case NetDeliveryMethod.ReliableOrdered:
				{
					return 64;
				}

				default:
				{
					return 64;
				}
			}
		}

		internal static void SortMembersList(MemberInfo[] list)
		{
			int i = 1;
			
			while (i * 3 + 1 <= list.Length)
			{
				i = 3 * i + 1;
			}
			
			while (i > 0)
			{
				for (int j = i - 1; j < list.Length; j++)
				{
					MemberInfo memberInfo = list[j];
					int num = j;
				
					while (num >= i && string.Compare(list[num - i].Name, 
						memberInfo.Name, StringComparison.InvariantCulture) > 0)
					{
						list[num] = list[num - i];
						num -= i;
					}
			
					list[num] = memberInfo;
				}
			
				i /= 3;
			}
		}

		internal static NetDeliveryMethod GetDeliveryMethod(NetMessageType mtp)
		{
			if (mtp >= NetMessageType.UserReliableOrdered1)
			{
				return NetDeliveryMethod.ReliableOrdered;
			}
			
			if (mtp >= NetMessageType.UserReliableSequenced1)
			{
				return NetDeliveryMethod.ReliableSequenced;
			}
			
			if (mtp >= NetMessageType.UserReliableUnordered)
			{
				return NetDeliveryMethod.ReliableUnordered;
			}
		
			if (mtp >= NetMessageType.UserSequenced1)
			{
				return NetDeliveryMethod.UnreliableSequenced;
			}
		
			return NetDeliveryMethod.Unreliable;
		}

		public static string MakeCommaDelimitedList<T>(IList<T> list)
		{
			int count = list.Count;
			StringBuilder stringBuilder = new StringBuilder(count * 5);
			
			for (int i = 0; i < count; i++)
			{
				StringBuilder stringBuilder2 = stringBuilder;
				T t = list[i];
				stringBuilder2.Append(t.ToString());
			
				if (i != count - 1)
				{
					stringBuilder.Append(", ");
				}
			}
		
			return stringBuilder.ToString();
		}
	}
}
