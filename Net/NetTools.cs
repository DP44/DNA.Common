using System;
using System.Net;
using System.Net.Sockets;

namespace DNA.Net
{
	public static class NetTools
	{
		public static IPAddress MaptoIP6(this IPAddress adr)
		{
			if (adr == null)
			{
				return null;
			}
			
			if (adr.AddressFamily == AddressFamily.InterNetworkV6)
			{
				return adr;
			}

			if (adr.AddressFamily == AddressFamily.InterNetwork)
			{
				byte[] addressBytes = adr.GetAddressBytes();
				byte[] array = new byte[16];
				array[10] = byte.MaxValue;
				array[11] = byte.MaxValue;
				byte[] array2 = array;
				array2[12] = addressBytes[0];
				array2[13] = addressBytes[1];
				array2[14] = addressBytes[2];
				array2[15] = addressBytes[3];
				return new IPAddress(array2);
			}

			throw new ApplicationException("Cannot convert this address");
		}

		public static IPAddress MaptoIPV4(this IPAddress adr)
		{
			if (adr == null)
			{
				return null;
			}

			if (adr.AddressFamily == AddressFamily.InterNetwork)
			{
				return adr;
			}

			if (adr.AddressFamily == AddressFamily.InterNetworkV6)
			{
				byte[] addressBytes = adr.GetAddressBytes();
				
				return new IPAddress(new byte[]
				{
					addressBytes[12],
					addressBytes[13],
					addressBytes[14],
					addressBytes[15]
				});
			}

			throw new ApplicationException("Cannot convert this address");
		}
	}
}
