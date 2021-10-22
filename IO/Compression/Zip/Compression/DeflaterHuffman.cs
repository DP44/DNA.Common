using System;

namespace DNA.IO.Compression.Zip.Compression
{
	public class DeflaterHuffman
	{
		public class Tree
		{
			public short[] freqs;

			public byte[] length;

			public int minNumCodes;

			public int numCodes;

			private short[] codes;

			private int[] bl_counts;

			private int maxLength;

			private DeflaterHuffman dh;

			public Tree(DeflaterHuffman dh, int elems, int minCodes, int maxLength)
			{
				this.dh = dh;
				this.minNumCodes = minCodes;
				this.maxLength = maxLength;
				this.freqs = new short[elems];
				this.bl_counts = new int[maxLength];
			}

			public void Reset()
			{
				for (int i = 0; i < this.freqs.Length; i++)
				{
					this.freqs[i] = 0;
				}
				this.codes = null;
				this.length = null;
			}

			public void WriteSymbol(int code)
			{
				this.dh.pending.WriteBits((int)this.codes[code] & 65535, (int)this.length[code]);
			}

			public void CheckEmpty()
			{
				bool flag = true;
				for (int i = 0; i < this.freqs.Length; i++)
				{
					if (this.freqs[i] != 0)
					{
						flag = false;
					}
				}
				if (!flag)
				{
					throw new CompressionException("!Empty");
				}
			}

			public void SetStaticCodes(short[] stCodes, byte[] stLength)
			{
				this.codes = stCodes;
				this.length = stLength;
			}

			public void BuildCodes()
			{
				int[] array = new int[this.maxLength];
				int num = 0;
				this.codes = new short[this.freqs.Length];
				for (int i = 0; i < this.maxLength; i++)
				{
					array[i] = num;
					num += this.bl_counts[i] << 15 - i;
				}
				for (int j = 0; j < this.numCodes; j++)
				{
					int num2 = (int)this.length[j];
					if (num2 > 0)
					{
						this.codes[j] = DeflaterHuffman.BitReverse(array[num2 - 1]);
						array[num2 - 1] += 1 << 16 - num2;
					}
				}
			}

			private void BuildLength(int[] childs)
			{
				this.length = new byte[this.freqs.Length];
				int num = childs.Length / 2;
				int num2 = (num + 1) / 2;
				int num3 = 0;
				for (int i = 0; i < this.maxLength; i++)
				{
					this.bl_counts[i] = 0;
				}
				int[] array = new int[num];
				array[num - 1] = 0;
				for (int j = num - 1; j >= 0; j--)
				{
					if (childs[2 * j + 1] != -1)
					{
						int num4 = array[j] + 1;
						if (num4 > this.maxLength)
						{
							num4 = this.maxLength;
							num3++;
						}
						array[childs[2 * j]] = (array[childs[2 * j + 1]] = num4);
					}
					else
					{
						int num5 = array[j];
						this.bl_counts[num5 - 1]++;
						this.length[childs[2 * j]] = (byte)array[j];
					}
				}
				if (num3 == 0)
				{
					return;
				}
				int num6 = this.maxLength - 1;
				for (;;)
				{
					if (this.bl_counts[--num6] != 0)
					{
						do
						{
							this.bl_counts[num6]--;
							this.bl_counts[++num6]++;
							num3 -= 1 << this.maxLength - 1 - num6;
						}
						while (num3 > 0 && num6 < this.maxLength - 1);
						if (num3 <= 0)
						{
							break;
						}
					}
				}
				this.bl_counts[this.maxLength - 1] += num3;
				this.bl_counts[this.maxLength - 2] -= num3;
				int num7 = 2 * num2;
				for (int num8 = this.maxLength; num8 != 0; num8--)
				{
					int k = this.bl_counts[num8 - 1];
					while (k > 0)
					{
						int num9 = 2 * childs[num7++];
						if (childs[num9 + 1] == -1)
						{
							this.length[childs[num9]] = (byte)num8;
							k--;
						}
					}
				}
			}

			public void BuildTree()
			{
				int num = this.freqs.Length;
				int[] array = new int[num];
				int i = 0;
				int num2 = 0;
				for (int j = 0; j < num; j++)
				{
					int num3 = (int)this.freqs[j];
					if (num3 != 0)
					{
						int num4 = i++;
						int num5;
						while (num4 > 0 && (int)this.freqs[array[num5 = (num4 - 1) / 2]] > num3)
						{
							array[num4] = array[num5];
							num4 = num5;
						}
						array[num4] = j;
						num2 = j;
					}
				}
				while (i < 2)
				{
					int num6 = (num2 < 2) ? (++num2) : 0;
					array[i++] = num6;
				}
				this.numCodes = Math.Max(num2 + 1, this.minNumCodes);
				int num7 = i;
				int[] array2 = new int[4 * i - 2];
				int[] array3 = new int[2 * i - 1];
				int num8 = num7;
				for (int k = 0; k < i; k++)
				{
					int num9 = array[k];
					array2[2 * k] = num9;
					array2[2 * k + 1] = -1;
					array3[k] = (int)this.freqs[num9] << 8;
					array[k] = k;
				}
				do
				{
					int num10 = array[0];
					int num11 = array[--i];
					int num12 = 0;
					int l;
					for (l = 1; l < i; l = l * 2 + 1)
					{
						if (l + 1 < i && array3[array[l]] > array3[array[l + 1]])
						{
							l++;
						}
						array[num12] = array[l];
						num12 = l;
					}
					int num13 = array3[num11];
					while ((l = num12) > 0 && array3[array[num12 = (l - 1) / 2]] > num13)
					{
						array[l] = array[num12];
					}
					array[l] = num11;
					int num14 = array[0];
					num11 = num8++;
					array2[2 * num11] = num10;
					array2[2 * num11 + 1] = num14;
					int num15 = Math.Min(array3[num10] & 255, array3[num14] & 255);
					num13 = (array3[num11] = array3[num10] + array3[num14] - num15 + 1);
					num12 = 0;
					for (l = 1; l < i; l = num12 * 2 + 1)
					{
						if (l + 1 < i && array3[array[l]] > array3[array[l + 1]])
						{
							l++;
						}
						array[num12] = array[l];
						num12 = l;
					}
					while ((l = num12) > 0 && array3[array[num12 = (l - 1) / 2]] > num13)
					{
						array[l] = array[num12];
					}
					array[l] = num11;
				}
				while (i > 1);
				if (array[0] != array2.Length / 2 - 1)
				{
					throw new CompressionException("Heap invariant violated");
				}
				this.BuildLength(array2);
			}

			public int GetEncodedLength()
			{
				int num = 0;
				for (int i = 0; i < this.freqs.Length; i++)
				{
					num += (int)(this.freqs[i] * (short)this.length[i]);
				}
				return num;
			}

			public void CalcBLFreq(DeflaterHuffman.Tree blTree)
			{
				int num = -1;
				int i = 0;
				while (i < this.numCodes)
				{
					int num2 = 1;
					int num3 = (int)this.length[i];
					int num4;
					int num5;
					if (num3 == 0)
					{
						num4 = 138;
						num5 = 3;
					}
					else
					{
						num4 = 6;
						num5 = 3;
						if (num != num3)
						{
							short[] array = blTree.freqs;
							int num6 = num3;
							array[num6] += 1;
							num2 = 0;
						}
					}
					num = num3;
					i++;
					while (i < this.numCodes && num == (int)this.length[i])
					{
						i++;
						if (++num2 >= num4)
						{
							break;
						}
					}
					if (num2 < num5)
					{
						short[] array2 = blTree.freqs;
						int num7 = num;
						array2[num7] += (short)num2;
					}
					else if (num != 0)
					{
						short[] array3 = blTree.freqs;
						int rep_3_ = DeflaterHuffman.REP_3_6;
						array3[rep_3_] += 1;
					}
					else if (num2 <= 10)
					{
						short[] array4 = blTree.freqs;
						int rep_3_2 = DeflaterHuffman.REP_3_10;
						array4[rep_3_2] += 1;
					}
					else
					{
						short[] array5 = blTree.freqs;
						int rep_11_ = DeflaterHuffman.REP_11_138;
						array5[rep_11_] += 1;
					}
				}
			}

			public void WriteTree(DeflaterHuffman.Tree blTree)
			{
				int num = -1;
				int i = 0;
				while (i < this.numCodes)
				{
					int num2 = 1;
					int num3 = (int)this.length[i];
					int num4;
					int num5;
					if (num3 == 0)
					{
						num4 = 138;
						num5 = 3;
					}
					else
					{
						num4 = 6;
						num5 = 3;
						if (num != num3)
						{
							blTree.WriteSymbol(num3);
							num2 = 0;
						}
					}
					num = num3;
					i++;
					while (i < this.numCodes && num == (int)this.length[i])
					{
						i++;
						if (++num2 >= num4)
						{
							break;
						}
					}
					if (num2 < num5)
					{
						while (num2-- > 0)
						{
							blTree.WriteSymbol(num);
						}
					}
					else if (num != 0)
					{
						blTree.WriteSymbol(DeflaterHuffman.REP_3_6);
						this.dh.pending.WriteBits(num2 - 3, 2);
					}
					else if (num2 <= 10)
					{
						blTree.WriteSymbol(DeflaterHuffman.REP_3_10);
						this.dh.pending.WriteBits(num2 - 3, 3);
					}
					else
					{
						blTree.WriteSymbol(DeflaterHuffman.REP_11_138);
						this.dh.pending.WriteBits(num2 - 11, 7);
					}
				}
			}
		}

		private static int BUFSIZE = 16384;

		private static int LITERAL_NUM = 286;

		private static int DIST_NUM = 30;

		private static int BITLEN_NUM = 19;

		private static int REP_3_6 = 16;

		private static int REP_3_10 = 17;

		private static int REP_11_138 = 18;

		private static int EOF_SYMBOL = 256;

		private static int[] BL_ORDER = new int[]
		{
			16,
			17,
			18,
			0,
			8,
			7,
			9,
			6,
			10,
			5,
			11,
			4,
			12,
			3,
			13,
			2,
			14,
			1,
			15
		};

		private static byte[] bit4Reverse = new byte[]
		{
			0,
			8,
			4,
			12,
			2,
			10,
			6,
			14,
			1,
			9,
			5,
			13,
			3,
			11,
			7,
			15
		};

		public DeflaterPending pending;

		private DeflaterHuffman.Tree literalTree;

		private DeflaterHuffman.Tree distTree;

		private DeflaterHuffman.Tree blTree;

		private short[] d_buf;

		private byte[] l_buf;

		private int last_lit;

		private int extra_bits;

		private static short[] staticLCodes = new short[DeflaterHuffman.LITERAL_NUM];

		private static byte[] staticLLength = new byte[DeflaterHuffman.LITERAL_NUM];

		private static short[] staticDCodes;

		private static byte[] staticDLength;

		public static short BitReverse(int toReverse)
		{
			return (short)((int)DeflaterHuffman.bit4Reverse[toReverse & 15] << 12 | (int)DeflaterHuffman.bit4Reverse[toReverse >> 4 & 15] << 8 | (int)DeflaterHuffman.bit4Reverse[toReverse >> 8 & 15] << 4 | (int)DeflaterHuffman.bit4Reverse[toReverse >> 12]);
		}

		static DeflaterHuffman()
		{
			int i = 0;
			while (i < 144)
			{
				DeflaterHuffman.staticLCodes[i] = DeflaterHuffman.BitReverse(48 + i << 8);
				DeflaterHuffman.staticLLength[i++] = 8;
			}
			while (i < 256)
			{
				DeflaterHuffman.staticLCodes[i] = DeflaterHuffman.BitReverse(256 + i << 7);
				DeflaterHuffman.staticLLength[i++] = 9;
			}
			while (i < 280)
			{
				DeflaterHuffman.staticLCodes[i] = DeflaterHuffman.BitReverse(-256 + i << 9);
				DeflaterHuffman.staticLLength[i++] = 7;
			}
			while (i < DeflaterHuffman.LITERAL_NUM)
			{
				DeflaterHuffman.staticLCodes[i] = DeflaterHuffman.BitReverse(-88 + i << 8);
				DeflaterHuffman.staticLLength[i++] = 8;
			}
			DeflaterHuffman.staticDCodes = new short[DeflaterHuffman.DIST_NUM];
			DeflaterHuffman.staticDLength = new byte[DeflaterHuffman.DIST_NUM];
			for (i = 0; i < DeflaterHuffman.DIST_NUM; i++)
			{
				DeflaterHuffman.staticDCodes[i] = DeflaterHuffman.BitReverse(i << 11);
				DeflaterHuffman.staticDLength[i] = 5;
			}
		}

		public DeflaterHuffman(DeflaterPending pending)
		{
			this.pending = pending;
			this.literalTree = new DeflaterHuffman.Tree(this, DeflaterHuffman.LITERAL_NUM, 257, 15);
			this.distTree = new DeflaterHuffman.Tree(this, DeflaterHuffman.DIST_NUM, 1, 15);
			this.blTree = new DeflaterHuffman.Tree(this, DeflaterHuffman.BITLEN_NUM, 4, 7);
			this.d_buf = new short[DeflaterHuffman.BUFSIZE];
			this.l_buf = new byte[DeflaterHuffman.BUFSIZE];
		}

		public void Reset()
		{
			this.last_lit = 0;
			this.extra_bits = 0;
			this.literalTree.Reset();
			this.distTree.Reset();
			this.blTree.Reset();
		}

		private int Lcode(int len)
		{
			if (len == 255)
			{
				return 285;
			}
			int num = 257;
			while (len >= 8)
			{
				num += 4;
				len >>= 1;
			}
			return num + len;
		}

		private int Dcode(int distance)
		{
			int num = 0;
			while (distance >= 4)
			{
				num += 2;
				distance >>= 1;
			}
			return num + distance;
		}

		public void SendAllTrees(int blTreeCodes)
		{
			this.blTree.BuildCodes();
			this.literalTree.BuildCodes();
			this.distTree.BuildCodes();
			this.pending.WriteBits(this.literalTree.numCodes - 257, 5);
			this.pending.WriteBits(this.distTree.numCodes - 1, 5);
			this.pending.WriteBits(blTreeCodes - 4, 4);
			for (int i = 0; i < blTreeCodes; i++)
			{
				this.pending.WriteBits((int)this.blTree.length[DeflaterHuffman.BL_ORDER[i]], 3);
			}
			this.literalTree.WriteTree(this.blTree);
			this.distTree.WriteTree(this.blTree);
		}

		public void CompressBlock()
		{
			for (int i = 0; i < this.last_lit; i++)
			{
				int num = (int)(this.l_buf[i] & byte.MaxValue);
				int num2 = (int)this.d_buf[i];
				if (num2-- != 0)
				{
					int num3 = this.Lcode(num);
					this.literalTree.WriteSymbol(num3);
					int num4 = (num3 - 261) / 4;
					if (num4 > 0 && num4 <= 5)
					{
						this.pending.WriteBits(num & (1 << num4) - 1, num4);
					}
					int num5 = this.Dcode(num2);
					this.distTree.WriteSymbol(num5);
					num4 = num5 / 2 - 1;
					if (num4 > 0)
					{
						this.pending.WriteBits(num2 & (1 << num4) - 1, num4);
					}
				}
				else
				{
					this.literalTree.WriteSymbol(num);
				}
			}
			this.literalTree.WriteSymbol(DeflaterHuffman.EOF_SYMBOL);
		}

		public void FlushStoredBlock(byte[] stored, int storedOffset, int storedLength, bool lastBlock)
		{
			this.pending.WriteBits(lastBlock ? 1 : 0, 3);
			this.pending.AlignToByte();
			this.pending.WriteShort(storedLength);
			this.pending.WriteShort(~storedLength);
			this.pending.WriteBlock(stored, storedOffset, storedLength);
			this.Reset();
		}

		public void FlushBlock(byte[] stored, int storedOffset, int storedLength, bool lastBlock)
		{
			short[] freqs = this.literalTree.freqs;
			int eof_SYMBOL = DeflaterHuffman.EOF_SYMBOL;
			freqs[eof_SYMBOL] += 1;
			this.literalTree.BuildTree();
			this.distTree.BuildTree();
			this.literalTree.CalcBLFreq(this.blTree);
			this.distTree.CalcBLFreq(this.blTree);
			this.blTree.BuildTree();
			int num = 4;
			for (int i = 18; i > num; i--)
			{
				if (this.blTree.length[DeflaterHuffman.BL_ORDER[i]] > 0)
				{
					num = i + 1;
				}
			}
			int num2 = 14 + num * 3 + this.blTree.GetEncodedLength() + this.literalTree.GetEncodedLength() + this.distTree.GetEncodedLength() + this.extra_bits;
			int num3 = this.extra_bits;
			for (int j = 0; j < DeflaterHuffman.LITERAL_NUM; j++)
			{
				num3 += (int)(this.literalTree.freqs[j] * (short)DeflaterHuffman.staticLLength[j]);
			}
			for (int k = 0; k < DeflaterHuffman.DIST_NUM; k++)
			{
				num3 += (int)(this.distTree.freqs[k] * (short)DeflaterHuffman.staticDLength[k]);
			}
			if (num2 >= num3)
			{
				num2 = num3;
			}
			if (storedOffset >= 0 && storedLength + 4 < num2 >> 3)
			{
				this.FlushStoredBlock(stored, storedOffset, storedLength, lastBlock);
				return;
			}
			if (num2 == num3)
			{
				this.pending.WriteBits(2 + (lastBlock ? 1 : 0), 3);
				this.literalTree.SetStaticCodes(DeflaterHuffman.staticLCodes, DeflaterHuffman.staticLLength);
				this.distTree.SetStaticCodes(DeflaterHuffman.staticDCodes, DeflaterHuffman.staticDLength);
				this.CompressBlock();
				this.Reset();
				return;
			}
			this.pending.WriteBits(4 + (lastBlock ? 1 : 0), 3);
			this.SendAllTrees(num);
			this.CompressBlock();
			this.Reset();
		}

		public bool IsFull()
		{
			return this.last_lit >= DeflaterHuffman.BUFSIZE;
		}

		public bool TallyLit(int lit)
		{
			this.d_buf[this.last_lit] = 0;
			this.l_buf[this.last_lit++] = (byte)lit;
			short[] freqs = this.literalTree.freqs;
			freqs[lit] += 1;
			return this.IsFull();
		}

		public bool TallyDist(int dist, int len)
		{
			this.d_buf[this.last_lit] = (short)dist;
			this.l_buf[this.last_lit++] = (byte)(len - 3);
			int num = this.Lcode(len - 3);
			short[] freqs = this.literalTree.freqs;
			int num2 = num;
			freqs[num2] += 1;
			if (num >= 265 && num < 285)
			{
				this.extra_bits += (num - 261) / 4;
			}
			int num3 = this.Dcode(dist - 1);
			short[] freqs2 = this.distTree.freqs;
			int num4 = num3;
			freqs2[num4] += 1;
			if (num3 >= 4)
			{
				this.extra_bits += num3 / 2 - 1;
			}
			return this.IsFull();
		}
	}
}
