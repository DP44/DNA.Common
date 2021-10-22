using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace DNA.Net.Lidgren
{
	public sealed class NetBitVector
	{
		private readonly int m_capacity;

		private readonly int[] m_data;

		private int m_numBitsSet;

		public int Capacity
		{
			get
			{
				return this.m_capacity;
			}
		}

		public NetBitVector(int bitsCapacity)
		{
			this.m_capacity = bitsCapacity;
			this.m_data = new int[(bitsCapacity + 31) / 32];
		}

		public bool IsEmpty()
		{
			return this.m_numBitsSet == 0;
		}

		public int Count()
		{
			return this.m_numBitsSet;
		}

		public void RotateDown()
		{
			int num = this.m_data.Length - 1;
			int num2 = this.m_data[0] & 1;
			for (int i = 0; i < num; i++)
			{
				this.m_data[i] = ((this.m_data[i] >> 1 & int.MaxValue) | this.m_data[i + 1] << 31);
			}
			int num3 = this.m_capacity - 1 - 32 * num;
			int num4 = this.m_data[num];
			num4 >>= 1;
			num4 |= num2 << num3;
			this.m_data[num] = num4;
		}

		public int GetFirstSetIndex()
		{
			int num = 0;
			int num2;
			for (num2 = this.m_data[0]; num2 == 0; num2 = this.m_data[num])
			{
				num++;
			}
			int num3 = 0;
			while ((num2 >> num3 & 1) == 0)
			{
				num3++;
			}
			return num * 32 + num3;
		}

		public bool Get(int bitIndex)
		{
			return (this.m_data[bitIndex / 32] & 1 << bitIndex % 32) != 0;
		}

		public void Set(int bitIndex, bool value)
		{
			int num = bitIndex / 32;
			if (value)
			{
				if ((this.m_data[num] & 1 << bitIndex % 32) == 0)
				{
					this.m_numBitsSet++;
				}
				this.m_data[num] |= 1 << bitIndex % 32;
				return;
			}
			if ((this.m_data[num] & 1 << bitIndex % 32) != 0)
			{
				this.m_numBitsSet--;
			}
			this.m_data[num] &= ~(1 << bitIndex % 32);
		}

		[IndexerName("Bit")]
		public bool this[int index]
		{
			get
			{
				return this.Get(index);
			}
			set
			{
				this.Set(index, value);
			}
		}

		public void Clear()
		{
			Array.Clear(this.m_data, 0, this.m_data.Length);
			this.m_numBitsSet = 0;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(this.m_capacity + 2);
			stringBuilder.Append('[');
			for (int i = 0; i < this.m_capacity; i++)
			{
				stringBuilder.Append(this.Get(this.m_capacity - i - 1) ? '1' : '0');
			}
			stringBuilder.Append(']');
			return stringBuilder.ToString();
		}
	}
}
