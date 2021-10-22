using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DNA.Net.Lidgren
{
	public class NetRC2Encryption : INetEncryption
	{
		private readonly byte[] m_key;

		private readonly byte[] m_iv;

		private readonly int m_bitSize;

		private static readonly List<int> m_keysizes;

		private static readonly List<int> m_blocksizes;

		static NetRC2Encryption()
		{
			RC2CryptoServiceProvider rc2CryptoServiceProvider = new RC2CryptoServiceProvider();
			List<int> list = new List<int>();
			foreach (KeySizes keySizes in rc2CryptoServiceProvider.LegalKeySizes)
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
			NetRC2Encryption.m_keysizes = list;
			list = new List<int>();
			foreach (KeySizes keySizes2 in rc2CryptoServiceProvider.LegalBlockSizes)
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
			NetRC2Encryption.m_blocksizes = list;
		}

		public NetRC2Encryption(byte[] key, byte[] iv)
		{
			if (!NetRC2Encryption.m_keysizes.Contains(key.Length * 8))
			{
				throw new NetException(string.Format("Not a valid key size. (Valid values are: {0})", NetUtility.MakeCommaDelimitedList<int>(NetRC2Encryption.m_keysizes)));
			}
			if (!NetRC2Encryption.m_blocksizes.Contains(iv.Length * 8))
			{
				throw new NetException(string.Format("Not a valid iv size. (Valid values are: {0})", NetUtility.MakeCommaDelimitedList<int>(NetRC2Encryption.m_blocksizes)));
			}
			this.m_key = key;
			this.m_iv = iv;
			this.m_bitSize = this.m_key.Length * 8;
		}

		public NetRC2Encryption(string key, int bitsize)
		{
			if (!NetRC2Encryption.m_keysizes.Contains(bitsize))
			{
				throw new NetException(string.Format("Not a valid key size. (Valid values are: {0})", NetUtility.MakeCommaDelimitedList<int>(NetRC2Encryption.m_keysizes)));
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
			this.m_iv = new byte[NetRC2Encryption.m_blocksizes[0] / 8];
			Buffer.BlockCopy(array, array.Length - this.m_iv.Length - 1, this.m_iv, 0, this.m_iv.Length);
			this.m_bitSize = bitsize;
		}

		public NetRC2Encryption(string key) : this(key, NetRC2Encryption.m_keysizes[0])
		{
		}

		public bool Encrypt(NetOutgoingMessage msg)
		{
			try
			{
				using (RC2CryptoServiceProvider rc2CryptoServiceProvider = new RC2CryptoServiceProvider
				{
					KeySize = this.m_bitSize,
					Mode = CipherMode.CBC
				})
				{
					using (ICryptoTransform cryptoTransform = rc2CryptoServiceProvider.CreateEncryptor(this.m_key, this.m_iv))
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
				using (RC2CryptoServiceProvider rc2CryptoServiceProvider = new RC2CryptoServiceProvider
				{
					KeySize = this.m_bitSize,
					Mode = CipherMode.CBC
				})
				{
					using (ICryptoTransform cryptoTransform = rc2CryptoServiceProvider.CreateDecryptor(this.m_key, this.m_iv))
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
