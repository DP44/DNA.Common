using System;
using System.Security.Cryptography;
using System.Text;

namespace DNA.Net.Lidgren
{
	public sealed class NetXtea : NetBlockEncryptionBase
	{
		private const int c_blockSize = 8;

		private const int c_keySize = 16;

		private const int c_delta = -1640531527;

		private readonly int m_numRounds;

		private readonly uint[] m_sum0;

		private readonly uint[] m_sum1;

		public override int BlockSize
		{
			get
			{
				return 8;
			}
		}

		public NetXtea(byte[] key, int rounds)
		{
			if (key.Length < 16)
			{
				throw new NetException("Key too short!");
			}
			this.m_numRounds = rounds;
			this.m_sum0 = new uint[this.m_numRounds];
			this.m_sum1 = new uint[this.m_numRounds];
			uint[] array = new uint[8];
			int i;
			int num = i = 0;
			while (i < 4)
			{
				array[i] = BitConverter.ToUInt32(key, num);
				i++;
				num += 4;
			}
			num = (i = 0);
			while (i < 32)
			{
				this.m_sum0[i] = (uint)(num + (int)array[num & 3]);
				num += -1640531527;
				this.m_sum1[i] = (uint)(num + (int)array[num >> 11 & 3]);
				i++;
			}
		}

		public NetXtea(byte[] key) : this(key, 32)
		{
		}

		public NetXtea(string key) : this(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(key)), 32)
		{
		}

		protected override void EncryptBlock(byte[] source, int sourceOffset, byte[] destination)
		{
			uint num = NetXtea.BytesToUInt(source, sourceOffset);
			uint num2 = NetXtea.BytesToUInt(source, sourceOffset + 4);
			for (int num3 = 0; num3 != this.m_numRounds; num3++)
			{
				num += ((num2 << 4 ^ num2 >> 5) + num2 ^ this.m_sum0[num3]);
				num2 += ((num << 4 ^ num >> 5) + num ^ this.m_sum1[num3]);
			}
			NetXtea.UIntToBytes(num, destination, 0);
			NetXtea.UIntToBytes(num2, destination, 4);
		}

		protected override void DecryptBlock(byte[] source, int sourceOffset, byte[] destination)
		{
			uint num = NetXtea.BytesToUInt(source, sourceOffset);
			uint num2 = NetXtea.BytesToUInt(source, sourceOffset + 4);
			for (int i = this.m_numRounds - 1; i >= 0; i--)
			{
				num2 -= ((num << 4 ^ num >> 5) + num ^ this.m_sum1[i]);
				num -= ((num2 << 4 ^ num2 >> 5) + num2 ^ this.m_sum0[i]);
			}
			NetXtea.UIntToBytes(num, destination, 0);
			NetXtea.UIntToBytes(num2, destination, 4);
		}

		private static uint BytesToUInt(byte[] bytes, int offset)
		{
			uint num = (uint)((uint)bytes[offset] << 24);
			num |= (uint)((uint)bytes[++offset] << 16);
			num |= (uint)((uint)bytes[++offset] << 8);
			return num | (uint)bytes[++offset];
		}

		private static void UIntToBytes(uint value, byte[] destination, int destinationOffset)
		{
			destination[destinationOffset++] = (byte)(value >> 24);
			destination[destinationOffset++] = (byte)(value >> 16);
			destination[destinationOffset++] = (byte)(value >> 8);
			destination[destinationOffset++] = (byte)value;
		}
	}
}
