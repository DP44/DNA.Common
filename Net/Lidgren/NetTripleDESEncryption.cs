using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DNA.Net.Lidgren
{
	public class NetTripleDESEncryption : INetEncryption
	{
		private readonly byte[] m_key;

		private readonly byte[] m_iv;

		private readonly int m_bitSize;

		private static readonly List<int> m_keysizes;

		private static readonly List<int> m_blocksizes;

		static NetTripleDESEncryption()
		{
			TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
			List<int> list = new List<int>();
			foreach (KeySizes keySizes in tripleDESCryptoServiceProvider.LegalKeySizes)
			{
				for (int j = keySizes.MinSize; j <= keySizes.MaxSize; j += keySizes.SkipSize)
				{
					if (!list.Contains(j))
					{
						list.Add(j);
					}
					if (j == keySizes.MaxSize)
					{
						break;
					}
				}
			}
			NetTripleDESEncryption.m_keysizes = list;
			list = new List<int>();
			foreach (KeySizes keySizes2 in tripleDESCryptoServiceProvider.LegalBlockSizes)
			{
				for (int l = keySizes2.MinSize; l <= keySizes2.MaxSize; l += keySizes2.SkipSize)
				{
					if (!list.Contains(l))
					{
						list.Add(l);
					}
					if (l == keySizes2.MaxSize)
					{
						break;
					}
				}
			}
			NetTripleDESEncryption.m_blocksizes = list;
		}

		public NetTripleDESEncryption(byte[] key, byte[] iv)
		{
			if (!NetTripleDESEncryption.m_keysizes.Contains(key.Length * 8))
			{
				throw new NetException(string.Format("Not a valid key size. (Valid values are: {0})", NetUtility.MakeCommaDelimitedList<int>(NetTripleDESEncryption.m_keysizes)));
			}
			if (!NetTripleDESEncryption.m_blocksizes.Contains(iv.Length * 8))
			{
				throw new NetException(string.Format("Not a valid iv size. (Valid values are: {0})", NetUtility.MakeCommaDelimitedList<int>(NetTripleDESEncryption.m_blocksizes)));
			}
			this.m_key = key;
			this.m_iv = iv;
			this.m_bitSize = this.m_key.Length * 8;
		}

		public NetTripleDESEncryption(string key, int bitsize)
		{
			if (!NetTripleDESEncryption.m_keysizes.Contains(bitsize))
			{
				throw new NetException(string.Format("Not a valid key size. (Valid values are: {0})", NetUtility.MakeCommaDelimitedList<int>(NetTripleDESEncryption.m_keysizes)));
			}
			byte[] array = Encoding.UTF32.GetBytes(key);
			HMACSHA512 hmacsha = new HMACSHA512(Convert.FromBase64String("i88NEiez3c50bHqr3YGasDc4p8jRrxJAaiRiqixpvp4XNAStP5YNoC2fXnWkURtkha6M8yY901Gj07IRVIRyGL=="));
			hmacsha.Initialize();
			for (int i = 0; i < 1000; i++)
			{
				array = hmacsha.ComputeHash(array);
			}
			int num = bitsize / 8;
			this.m_key = new byte[num];
			Buffer.BlockCopy(array, 0, this.m_key, 0, num);
			this.m_iv = new byte[NetTripleDESEncryption.m_blocksizes[0] / 8];
			Buffer.BlockCopy(array, array.Length - this.m_iv.Length - 1, this.m_iv, 0, this.m_iv.Length);
			this.m_bitSize = bitsize;
		}

		public NetTripleDESEncryption(string key) : this(key, NetTripleDESEncryption.m_keysizes[0])
		{
		}

		public bool Encrypt(NetOutgoingMessage msg)
		{
			try
			{
				using (TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider
				{
					KeySize = this.m_bitSize,
					Mode = CipherMode.CBC
				})
				{
					using (ICryptoTransform cryptoTransform = tripleDESCryptoServiceProvider.CreateEncryptor(this.m_key, this.m_iv))
					{
						using (MemoryStream memoryStream = new MemoryStream())
						{
							using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
							{
								cryptoStream.Write(msg.m_data, 0, msg.m_data.Length);
							}
							msg.m_data = memoryStream.ToArray();
						}
					}
				}
			}
			catch
			{
				return false;
			}
			return true;
		}

		public bool Decrypt(NetIncomingMessage msg)
		{
			try
			{
				using (TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider
				{
					KeySize = this.m_bitSize,
					Mode = CipherMode.CBC
				})
				{
					using (ICryptoTransform cryptoTransform = tripleDESCryptoServiceProvider.CreateDecryptor(this.m_key, this.m_iv))
					{
						using (MemoryStream memoryStream = new MemoryStream())
						{
							using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
							{
								cryptoStream.Write(msg.m_data, 0, msg.m_data.Length);
							}
							msg.m_data = memoryStream.ToArray();
						}
					}
				}
			}
			catch
			{
				return false;
			}
			return true;
		}
	}
}
