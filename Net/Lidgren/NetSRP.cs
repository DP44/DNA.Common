using System;
using System.Security.Cryptography;
using System.Text;

namespace DNA.Net.Lidgren
{
	public static class NetSRP
	{
		private static readonly NetBigInteger N = new NetBigInteger("0115b8b692e0e045692cf280b436735c77a5a9e8a9e7ed56c965f87db5b2a2ece3", 16);

		private static readonly NetBigInteger g = NetBigInteger.Two;

		private static readonly NetBigInteger k = NetSRP.ComputeMultiplier();

		private static HashAlgorithm GetHashAlgorithm()
		{
			return SHA256.Create();
		}

		private static NetBigInteger ComputeMultiplier()
		{
			string text = NetUtility.ToHexString(NetSRP.N.ToByteArrayUnsigned());
			string text2 = NetUtility.ToHexString(NetSRP.g.ToByteArrayUnsigned());
			string hexString = text + text2.PadLeft(text.Length, '0');
			byte[] buffer = NetUtility.ToByteArray(hexString);
			HashAlgorithm hashAlgorithm = NetSRP.GetHashAlgorithm();
			byte[] data = hashAlgorithm.ComputeHash(buffer);
			return new NetBigInteger(NetUtility.ToHexString(data), 16);
		}

		public static byte[] CreateRandomSalt()
		{
			byte[] array = new byte[16];
			NetRandom.Instance.NextBytes(array);
			return array;
		}

		public static byte[] CreateRandomEphemeral()
		{
			byte[] array = new byte[32];
			NetRandom.Instance.NextBytes(array);
			return array;
		}

		public static byte[] ComputePrivateKey(string username, string password, byte[] salt)
		{
			HashAlgorithm hashAlgorithm = NetSRP.GetHashAlgorithm();
			byte[] bytes = Encoding.UTF8.GetBytes(username + ":" + password);
			byte[] array = hashAlgorithm.ComputeHash(bytes);
			byte[] array2 = new byte[array.Length + salt.Length];
			Buffer.BlockCopy(salt, 0, array2, 0, salt.Length);
			Buffer.BlockCopy(array, 0, array2, salt.Length, array.Length);
			return new NetBigInteger(NetUtility.ToHexString(hashAlgorithm.ComputeHash(array2)), 16).ToByteArrayUnsigned();
		}

		public static byte[] ComputeServerVerifier(byte[] privateKey)
		{
			NetBigInteger exponent = new NetBigInteger(NetUtility.ToHexString(privateKey), 16);
			NetBigInteger netBigInteger = NetSRP.g.ModPow(exponent, NetSRP.N);
			return netBigInteger.ToByteArrayUnsigned();
		}

		public static byte[] Hash(byte[] data)
		{
			HashAlgorithm hashAlgorithm = NetSRP.GetHashAlgorithm();
			return hashAlgorithm.ComputeHash(data);
		}

		public static byte[] ComputeClientEphemeral(byte[] clientPrivateEphemeral)
		{
			NetBigInteger exponent = new NetBigInteger(NetUtility.ToHexString(clientPrivateEphemeral), 16);
			NetBigInteger netBigInteger = NetSRP.g.ModPow(exponent, NetSRP.N);
			return netBigInteger.ToByteArrayUnsigned();
		}

		public static byte[] ComputeServerEphemeral(byte[] serverPrivateEphemeral, byte[] verifier)
		{
			NetBigInteger exponent = new NetBigInteger(NetUtility.ToHexString(serverPrivateEphemeral), 16);
			NetBigInteger netBigInteger = new NetBigInteger(NetUtility.ToHexString(verifier), 16);
			NetBigInteger value = NetSRP.g.ModPow(exponent, NetSRP.N);
			NetBigInteger netBigInteger2 = netBigInteger.Multiply(NetSRP.k);
			NetBigInteger netBigInteger3 = netBigInteger2.Add(value).Mod(NetSRP.N);
			return netBigInteger3.ToByteArrayUnsigned();
		}

		public static byte[] ComputeU(byte[] clientPublicEphemeral, byte[] serverPublicEphemeral)
		{
			string text = NetUtility.ToHexString(clientPublicEphemeral);
			string text2 = NetUtility.ToHexString(serverPublicEphemeral);
			int totalWidth = 66;
			string hexString = text.PadLeft(totalWidth, '0') + text2.PadLeft(totalWidth, '0');
			byte[] buffer = NetUtility.ToByteArray(hexString);
			HashAlgorithm hashAlgorithm = NetSRP.GetHashAlgorithm();
			byte[] data = hashAlgorithm.ComputeHash(buffer);
			return new NetBigInteger(NetUtility.ToHexString(data), 16).ToByteArrayUnsigned();
		}

		public static byte[] ComputeServerSessionValue(byte[] clientPublicEphemeral, byte[] verifier, byte[] udata, byte[] serverPrivateEphemeral)
		{
			NetBigInteger val = new NetBigInteger(NetUtility.ToHexString(clientPublicEphemeral), 16);
			NetBigInteger netBigInteger = new NetBigInteger(NetUtility.ToHexString(verifier), 16);
			NetBigInteger exponent = new NetBigInteger(NetUtility.ToHexString(udata), 16);
			NetBigInteger exponent2 = new NetBigInteger(NetUtility.ToHexString(serverPrivateEphemeral), 16);
			NetBigInteger netBigInteger2 = netBigInteger.ModPow(exponent, NetSRP.N).Multiply(val).Mod(NetSRP.N).ModPow(exponent2, NetSRP.N).Mod(NetSRP.N);
			return netBigInteger2.ToByteArrayUnsigned();
		}

		public static byte[] ComputeClientSessionValue(byte[] serverPublicEphemeral, byte[] xdata, byte[] udata, byte[] clientPrivateEphemeral)
		{
			NetBigInteger netBigInteger = new NetBigInteger(NetUtility.ToHexString(serverPublicEphemeral), 16);
			NetBigInteger netBigInteger2 = new NetBigInteger(NetUtility.ToHexString(xdata), 16);
			NetBigInteger val = new NetBigInteger(NetUtility.ToHexString(udata), 16);
			NetBigInteger value = new NetBigInteger(NetUtility.ToHexString(clientPrivateEphemeral), 16);
			NetBigInteger netBigInteger3 = NetSRP.g.ModPow(netBigInteger2, NetSRP.N);
			NetBigInteger netBigInteger4 = netBigInteger.Add(NetSRP.N.Multiply(NetSRP.k)).Subtract(netBigInteger3.Multiply(NetSRP.k)).Mod(NetSRP.N);
			return netBigInteger4.ModPow(netBigInteger2.Multiply(val).Add(value), NetSRP.N).ToByteArrayUnsigned();
		}

		public static NetXtea CreateEncryption(byte[] sessionValue)
		{
			HashAlgorithm hashAlgorithm = NetSRP.GetHashAlgorithm();
			byte[] array = hashAlgorithm.ComputeHash(sessionValue);
			byte[] array2 = new byte[16];
			for (int i = 0; i < 16; i++)
			{
				array2[i] = array[i];
				for (int j = 1; j < array.Length / 16; j++)
				{
					byte[] array3 = array2;
					int num = i;
					array3[num] ^= array[i + j * 16];
				}
			}
			return new NetXtea(array2);
		}
	}
}
