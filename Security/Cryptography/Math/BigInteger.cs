using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DNA.Security.Cryptography.Math
{
	[Serializable]
	public class BigInteger
	{
		private const long IMASK = 4294967295L;

		private const int BitsPerByte = 8;

		private const int BitsPerInt = 32;

		private const int BytesPerInt = 4;

		private static readonly int[][] primeLists = new int[][]
		{
			new int[]
			{
				3,
				5,
				7,
				11,
				13,
				17,
				19,
				23
			},
			new int[]
			{
				29,
				31,
				37,
				41,
				43
			},
			new int[]
			{
				47,
				53,
				59,
				61,
				67
			},
			new int[]
			{
				71,
				73,
				79,
				83
			},
			new int[]
			{
				89,
				97,
				101,
				103
			},
			new int[]
			{
				107,
				109,
				113,
				127
			},
			new int[]
			{
				131,
				137,
				139,
				149
			},
			new int[]
			{
				151,
				157,
				163,
				167
			},
			new int[]
			{
				173,
				179,
				181,
				191
			},
			new int[]
			{
				193,
				197,
				199,
				211
			},
			new int[]
			{
				223,
				227,
				229
			},
			new int[]
			{
				233,
				239,
				241
			},
			new int[]
			{
				251,
				257,
				263
			},
			new int[]
			{
				269,
				271,
				277
			},
			new int[]
			{
				281,
				283,
				293
			},
			new int[]
			{
				307,
				311,
				313
			},
			new int[]
			{
				317,
				331,
				337
			},
			new int[]
			{
				347,
				349,
				353
			},
			new int[]
			{
				359,
				367,
				373
			},
			new int[]
			{
				379,
				383,
				389
			},
			new int[]
			{
				397,
				401,
				409
			},
			new int[]
			{
				419,
				421,
				431
			},
			new int[]
			{
				433,
				439,
				443
			},
			new int[]
			{
				449,
				457,
				461
			},
			new int[]
			{
				463,
				467,
				479
			},
			new int[]
			{
				487,
				491,
				499
			},
			new int[]
			{
				503,
				509,
				521
			},
			new int[]
			{
				523,
				541,
				547
			},
			new int[]
			{
				557,
				563,
				569
			},
			new int[]
			{
				571,
				577,
				587
			},
			new int[]
			{
				593,
				599,
				601
			},
			new int[]
			{
				607,
				613,
				617
			},
			new int[]
			{
				619,
				631,
				641
			},
			new int[]
			{
				643,
				647,
				653
			},
			new int[]
			{
				659,
				661,
				673
			},
			new int[]
			{
				677,
				683,
				691
			},
			new int[]
			{
				701,
				709,
				719
			},
			new int[]
			{
				727,
				733,
				739
			},
			new int[]
			{
				743,
				751,
				757
			},
			new int[]
			{
				761,
				769,
				773
			},
			new int[]
			{
				787,
				797,
				809
			},
			new int[]
			{
				811,
				821,
				823
			},
			new int[]
			{
				827,
				829,
				839
			},
			new int[]
			{
				853,
				857,
				859
			},
			new int[]
			{
				863,
				877,
				881
			},
			new int[]
			{
				883,
				887,
				907
			},
			new int[]
			{
				911,
				919,
				929
			},
			new int[]
			{
				937,
				941,
				947
			},
			new int[]
			{
				953,
				967,
				971
			},
			new int[]
			{
				977,
				983,
				991
			},
			new int[]
			{
				997,
				1009,
				1013
			},
			new int[]
			{
				1019,
				1021,
				1031
			}
		};

		private static readonly int[] primeProducts;

		private static readonly ulong UIMASK = (ulong)-1;

		private static readonly int[] ZeroMagnitude = new int[0];

		private static readonly byte[] ZeroEncoding = new byte[0];

		public static readonly BigInteger Zero = new BigInteger(0, BigInteger.ZeroMagnitude, false);

		public static readonly BigInteger One = BigInteger.createUValueOf(1UL);

		public static readonly BigInteger Two = BigInteger.createUValueOf(2UL);

		public static readonly BigInteger Three = BigInteger.createUValueOf(3UL);

		public static readonly BigInteger Ten = BigInteger.createUValueOf(10UL);

		private static readonly int chunk2 = 1;

		private static readonly BigInteger radix2 = BigInteger.ValueOf(2L);

		private static readonly BigInteger radix2E = BigInteger.radix2.Pow(BigInteger.chunk2);

		private static readonly int chunk10 = 19;

		private static readonly BigInteger radix10 = BigInteger.ValueOf(10L);

		private static readonly BigInteger radix10E = BigInteger.radix10.Pow(BigInteger.chunk10);

		private static readonly int chunk16 = 16;

		private static readonly BigInteger radix16 = BigInteger.ValueOf(16L);

		private static readonly BigInteger radix16E = BigInteger.radix16.Pow(BigInteger.chunk16);

		private static readonly Random RandomSource = new Random();

		private int sign;

		private int[] magnitude;

		private int nBits = -1;

		private int nBitLength = -1;

		private long mQuote = -1L;

		private static readonly byte[] rndMask = new byte[]
		{
			byte.MaxValue,
			127,
			63,
			31,
			15,
			7,
			3,
			1
		};

		private static readonly byte[] bitCounts = new byte[]
		{
			0,
			1,
			1,
			2,
			1,
			2,
			2,
			3,
			1,
			2,
			2,
			3,
			2,
			3,
			3,
			4,
			1,
			2,
			2,
			3,
			2,
			3,
			3,
			4,
			2,
			3,
			3,
			4,
			3,
			4,
			4,
			5,
			1,
			2,
			2,
			3,
			2,
			3,
			3,
			4,
			2,
			3,
			3,
			4,
			3,
			4,
			4,
			5,
			2,
			3,
			3,
			4,
			3,
			4,
			4,
			5,
			3,
			4,
			4,
			5,
			4,
			5,
			5,
			6,
			1,
			2,
			2,
			3,
			2,
			3,
			3,
			4,
			2,
			3,
			3,
			4,
			3,
			4,
			4,
			5,
			2,
			3,
			3,
			4,
			3,
			4,
			4,
			5,
			3,
			4,
			4,
			5,
			4,
			5,
			5,
			6,
			2,
			3,
			3,
			4,
			3,
			4,
			4,
			5,
			3,
			4,
			4,
			5,
			4,
			5,
			5,
			6,
			3,
			4,
			4,
			5,
			4,
			5,
			5,
			6,
			4,
			5,
			5,
			6,
			5,
			6,
			6,
			7,
			1,
			2,
			2,
			3,
			2,
			3,
			3,
			4,
			2,
			3,
			3,
			4,
			3,
			4,
			4,
			5,
			2,
			3,
			3,
			4,
			3,
			4,
			4,
			5,
			3,
			4,
			4,
			5,
			4,
			5,
			5,
			6,
			2,
			3,
			3,
			4,
			3,
			4,
			4,
			5,
			3,
			4,
			4,
			5,
			4,
			5,
			5,
			6,
			3,
			4,
			4,
			5,
			4,
			5,
			5,
			6,
			4,
			5,
			5,
			6,
			5,
			6,
			6,
			7,
			2,
			3,
			3,
			4,
			3,
			4,
			4,
			5,
			3,
			4,
			4,
			5,
			4,
			5,
			5,
			6,
			3,
			4,
			4,
			5,
			4,
			5,
			5,
			6,
			4,
			5,
			5,
			6,
			5,
			6,
			6,
			7,
			3,
			4,
			4,
			5,
			4,
			5,
			5,
			6,
			4,
			5,
			5,
			6,
			5,
			6,
			6,
			7,
			4,
			5,
			5,
			6,
			5,
			6,
			6,
			7,
			5,
			6,
			6,
			7,
			6,
			7,
			7,
			8
		};

		static BigInteger()
		{
			BigInteger.primeProducts = new int[BigInteger.primeLists.Length];
			for (int i = 0; i < BigInteger.primeLists.Length; i++)
			{
				int[] array = BigInteger.primeLists[i];
				int num = 1;
				for (int j = 0; j < array.Length; j++)
				{
					num *= array[j];
				}
				BigInteger.primeProducts[i] = num;
			}
		}

		private static int GetByteLength(int nBits)
		{
			return (nBits + 8 - 1) / 8;
		}

		private BigInteger()
		{
		}

		private BigInteger(int signum, int[] mag, bool checkMag)
		{
			if (!checkMag)
			{
				this.sign = signum;
				this.magnitude = mag;
				return;
			}
			int num = 0;
			while (num < mag.Length && mag[num] == 0)
			{
				num++;
			}
			if (num == mag.Length)
			{
				this.magnitude = BigInteger.ZeroMagnitude;
				return;
			}
			this.sign = signum;
			if (num == 0)
			{
				this.magnitude = mag;
				return;
			}
			this.magnitude = new int[mag.Length - num];
			Array.Copy(mag, num, this.magnitude, 0, this.magnitude.Length);
		}

		public BigInteger(string value) : this(value, 10)
		{
		}

		public BigInteger(string str, int radix)
		{
			if (str.Length == 0)
			{
				throw new FormatException("Zero length BigInteger");
			}
			NumberStyles style;
			int num;
			BigInteger bigInteger;
			BigInteger val;
			if (radix != 2)
			{
				if (radix != 10)
				{
					if (radix != 16)
					{
						throw new FormatException("Only bases 2, 10, or 16 allowed");
					}
					style = NumberStyles.AllowHexSpecifier;
					num = BigInteger.chunk16;
					bigInteger = BigInteger.radix16;
					val = BigInteger.radix16E;
				}
				else
				{
					style = NumberStyles.Integer;
					num = BigInteger.chunk10;
					bigInteger = BigInteger.radix10;
					val = BigInteger.radix10E;
				}
			}
			else
			{
				style = NumberStyles.Integer;
				num = BigInteger.chunk2;
				bigInteger = BigInteger.radix2;
				val = BigInteger.radix2E;
			}
			int num2 = 0;
			this.sign = 1;
			if (str[0] == '-')
			{
				if (str.Length == 1)
				{
					throw new FormatException("Zero length BigInteger");
				}
				this.sign = -1;
				num2 = 1;
			}
			while (num2 < str.Length && int.Parse(str[num2].ToString(), style) == 0)
			{
				num2++;
			}
			if (num2 >= str.Length)
			{
				this.sign = 0;
				this.magnitude = BigInteger.ZeroMagnitude;
				return;
			}
			BigInteger bigInteger2 = BigInteger.Zero;
			int num3 = num2 + num;
			if (num3 <= str.Length)
			{
				string text;
				for (;;)
				{
					text = str.Substring(num2, num);
					ulong num4 = ulong.Parse(text, style);
					BigInteger value = BigInteger.createUValueOf(num4);
					if (radix != 2)
					{
						if (radix != 16)
						{
							bigInteger2 = bigInteger2.Multiply(val);
						}
						else
						{
							bigInteger2 = bigInteger2.ShiftLeft(64);
						}
					}
					else
					{
						if (num4 > 1UL)
						{
							break;
						}
						bigInteger2 = bigInteger2.ShiftLeft(1);
					}
					bigInteger2 = bigInteger2.Add(value);
					num2 = num3;
					num3 += num;
					if (num3 > str.Length)
					{
						goto IL_1B6;
					}
				}
				throw new FormatException("Bad character in radix 2 string: " + text);
			}
			IL_1B6:
			if (num2 < str.Length)
			{
				string text2 = str.Substring(num2);
				ulong value2 = ulong.Parse(text2, style);
				BigInteger bigInteger3 = BigInteger.createUValueOf(value2);
				if (bigInteger2.sign > 0)
				{
					if (radix != 2)
					{
						if (radix == 16)
						{
							bigInteger2 = bigInteger2.ShiftLeft(text2.Length << 2);
						}
						else
						{
							bigInteger2 = bigInteger2.Multiply(bigInteger.Pow(text2.Length));
						}
					}
					bigInteger2 = bigInteger2.Add(bigInteger3);
				}
				else
				{
					bigInteger2 = bigInteger3;
				}
			}
			this.magnitude = bigInteger2.magnitude;
		}

		public BigInteger(byte[] bytes) : this(bytes, 0, bytes.Length)
		{
		}

		public BigInteger(byte[] bytes, int offset, int length)
		{
			if (length == 0)
			{
				throw new FormatException("Zero length BigInteger");
			}
			if ((sbyte)bytes[offset] >= 0)
			{
				this.magnitude = BigInteger.MakeMagnitude(bytes, offset, length);
				this.sign = ((this.magnitude.Length > 0) ? 1 : 0);
				return;
			}
			this.sign = -1;
			int num = offset + length;
			int num2 = offset;
			while (num2 < num && (sbyte)bytes[num2] == -1)
			{
				num2++;
			}
			if (num2 >= num)
			{
				this.magnitude = BigInteger.One.magnitude;
				return;
			}
			int num3 = num - num2;
			byte[] array = new byte[num3];
			int i = 0;
			while (i < num3)
			{
				array[i++] = ~bytes[num2++];
			}
			while (array[--i] == 255)
			{
				array[i] = 0;
			}
			byte[] array2 = array;
			int num4 = i;
			array2[num4] += 1;
			this.magnitude = BigInteger.MakeMagnitude(array, 0, array.Length);
		}

		private static int[] MakeMagnitude(byte[] bytes, int offset, int length)
		{
			int num = offset + length;
			int num2 = offset;
			while (num2 < num && bytes[num2] == 0)
			{
				num2++;
			}
			if (num2 >= num)
			{
				return BigInteger.ZeroMagnitude;
			}
			int num3 = (num - num2 + 3) / 4;
			int num4 = (num - num2) % 4;
			if (num4 == 0)
			{
				num4 = 4;
			}
			if (num3 < 1)
			{
				return BigInteger.ZeroMagnitude;
			}
			int[] array = new int[num3];
			int num5 = 0;
			int num6 = 0;
			for (int i = num2; i < num; i++)
			{
				num5 <<= 8;
				num5 |= (int)(bytes[i] & byte.MaxValue);
				num4--;
				if (num4 <= 0)
				{
					array[num6] = num5;
					num6++;
					num4 = 4;
					num5 = 0;
				}
			}
			if (num6 < array.Length)
			{
				array[num6] = num5;
			}
			return array;
		}

		public BigInteger(int sign, byte[] bytes) : this(sign, bytes, 0, bytes.Length)
		{
		}

		public BigInteger(int sign, byte[] bytes, int offset, int length)
		{
			if (sign < -1 || sign > 1)
			{
				throw new FormatException("Invalid sign value");
			}
			if (sign == 0)
			{
				this.magnitude = BigInteger.ZeroMagnitude;
				return;
			}
			this.magnitude = BigInteger.MakeMagnitude(bytes, offset, length);
			this.sign = ((this.magnitude.Length < 1) ? 0 : sign);
		}

		public BigInteger(int sizeInBits, Random random)
		{
			if (sizeInBits < 0)
			{
				throw new ArgumentException("sizeInBits must be non-negative");
			}
			this.nBits = -1;
			this.nBitLength = -1;
			if (sizeInBits == 0)
			{
				this.magnitude = BigInteger.ZeroMagnitude;
				return;
			}
			int byteLength = BigInteger.GetByteLength(sizeInBits);
			byte[] array = new byte[byteLength];
			random.NextBytes(array);
			byte[] array2 = array;
			int num = 0;
			array2[num] &= BigInteger.rndMask[8 * byteLength - sizeInBits];
			this.magnitude = BigInteger.MakeMagnitude(array, 0, array.Length);
			this.sign = ((this.magnitude.Length < 1) ? 0 : 1);
		}

		public BigInteger(int bitLength, int certainty, Random random)
		{
			if (bitLength < 2)
			{
				throw new ArithmeticException("bitLength < 2");
			}
			this.sign = 1;
			this.nBitLength = bitLength;
			if (bitLength == 2)
			{
				this.magnitude = ((random.Next(2) == 0) ? BigInteger.Two.magnitude : BigInteger.Three.magnitude);
				return;
			}
			int byteLength = BigInteger.GetByteLength(bitLength);
			byte[] array = new byte[byteLength];
			int num = 8 * byteLength - bitLength;
			byte b = BigInteger.rndMask[num];
			for (;;)
			{
				random.NextBytes(array);
				byte[] array2 = array;
				int num2 = 0;
				array2[num2] &= b;
				byte[] array3 = array;
				int num3 = 0;
				array3[num3] |= (byte)(1 << 7 - num);
				byte[] array4 = array;
				int num4 = byteLength - 1;
				array4[num4] |= 1;
				this.magnitude = BigInteger.MakeMagnitude(array, 0, array.Length);
				this.nBits = -1;
				this.mQuote = -1L;
				if (certainty < 1)
				{
					break;
				}
				if (this.CheckProbablePrime(certainty, random))
				{
					return;
				}
				if (bitLength > 32)
				{
					for (int i = 0; i < 10000; i++)
					{
						int num5 = 33 + random.Next(bitLength - 2);
						this.magnitude[this.magnitude.Length - (num5 >> 5)] ^= 1 << num5;
						this.magnitude[this.magnitude.Length - 1] ^= random.Next() + 1 << 1;
						this.mQuote = -1L;
						if (this.CheckProbablePrime(certainty, random))
						{
							return;
						}
					}
				}
			}
		}

		public BigInteger Abs()
		{
			if (this.sign < 0)
			{
				return this.Negate();
			}
			return this;
		}

		private static int[] AddMagnitudes(int[] a, int[] b)
		{
			int num = a.Length - 1;
			int i = b.Length - 1;
			long num2 = 0L;
			while (i >= 0)
			{
				num2 += (long)((ulong)a[num] + (ulong)b[i--]);
				a[num--] = (int)num2;
				num2 = (long)((ulong)num2 >> 32);
			}
			if (num2 != 0L)
			{
				while (num >= 0 && ++a[num--] == 0)
				{
				}
			}
			return a;
		}

		public BigInteger Add(BigInteger value)
		{
			if (this.sign == 0)
			{
				return value;
			}
			if (this.sign == value.sign)
			{
				return this.AddToMagnitude(value.magnitude);
			}
			if (value.sign == 0)
			{
				return this;
			}
			if (value.sign < 0)
			{
				return this.Subtract(value.Negate());
			}
			return value.Subtract(this.Negate());
		}

		private BigInteger AddToMagnitude(int[] magToAdd)
		{
			int[] array;
			int[] array2;
			if (this.magnitude.Length < magToAdd.Length)
			{
				array = magToAdd;
				array2 = this.magnitude;
			}
			else
			{
				array = this.magnitude;
				array2 = magToAdd;
			}
			uint num = uint.MaxValue;
			if (array.Length == array2.Length)
			{
				num -= (uint)array2[0];
			}
			bool flag = array[0] >= (int)num;
			int[] array3;
			if (flag)
			{
				array3 = new int[array.Length + 1];
				array.CopyTo(array3, 1);
			}
			else
			{
				array3 = (int[])array.Clone();
			}
			array3 = BigInteger.AddMagnitudes(array3, array2);
			return new BigInteger(this.sign, array3, flag);
		}

		public BigInteger And(BigInteger value)
		{
			if (this.sign == 0 || value.sign == 0)
			{
				return BigInteger.Zero;
			}
			int[] array = (this.sign > 0) ? this.magnitude : this.Add(BigInteger.One).magnitude;
			int[] array2 = (value.sign > 0) ? value.magnitude : value.Add(BigInteger.One).magnitude;
			bool flag = this.sign < 0 && value.sign < 0;
			int num = Math.Max(array.Length, array2.Length);
			int[] array3 = new int[num];
			int num2 = array3.Length - array.Length;
			int num3 = array3.Length - array2.Length;
			for (int i = 0; i < array3.Length; i++)
			{
				int num4 = (i >= num2) ? array[i - num2] : 0;
				int num5 = (i >= num3) ? array2[i - num3] : 0;
				if (this.sign < 0)
				{
					num4 = ~num4;
				}
				if (value.sign < 0)
				{
					num5 = ~num5;
				}
				array3[i] = (num4 & num5);
				if (flag)
				{
					array3[i] = ~array3[i];
				}
			}
			BigInteger bigInteger = new BigInteger(1, array3, true);
			if (flag)
			{
				bigInteger = bigInteger.Not();
			}
			return bigInteger;
		}

		public BigInteger AndNot(BigInteger val)
		{
			return this.And(val.Not());
		}

		public int BitCount
		{
			get
			{
				if (this.nBits == -1)
				{
					if (this.sign < 0)
					{
						this.nBits = this.Not().BitCount;
					}
					else
					{
						int num = 0;
						for (int i = 0; i < this.magnitude.Length; i++)
						{
							num += (int)BigInteger.bitCounts[(int)((byte)this.magnitude[i])];
							num += (int)BigInteger.bitCounts[(int)((byte)(this.magnitude[i] >> 8))];
							num += (int)BigInteger.bitCounts[(int)((byte)(this.magnitude[i] >> 16))];
							num += (int)BigInteger.bitCounts[(int)((byte)(this.magnitude[i] >> 24))];
						}
						this.nBits = num;
					}
				}
				return this.nBits;
			}
		}

		private int calcBitLength(int indx, int[] mag)
		{
			while (indx < mag.Length)
			{
				if (mag[indx] != 0)
				{
					int num = 32 * (mag.Length - indx - 1);
					int num2 = mag[indx];
					num += BigInteger.BitLen(num2);
					if (this.sign < 0 && (num2 & -num2) == num2)
					{
						while (++indx < mag.Length)
						{
							if (mag[indx] != 0)
							{
								return num;
							}
						}
						num--;
					}
					return num;
				}
				indx++;
			}
			return 0;
		}

		public int BitLength
		{
			get
			{
				if (this.nBitLength == -1)
				{
					this.nBitLength = ((this.sign == 0) ? 0 : this.calcBitLength(0, this.magnitude));
				}
				return this.nBitLength;
			}
		}

		private static int BitLen(int w)
		{
			if (w >= 32768)
			{
				if (w >= 8388608)
				{
					if (w >= 134217728)
					{
						if (w >= 536870912)
						{
							if (w >= 1073741824)
							{
								return 31;
							}
							return 30;
						}
						else
						{
							if (w >= 268435456)
							{
								return 29;
							}
							return 28;
						}
					}
					else if (w >= 33554432)
					{
						if (w >= 67108864)
						{
							return 27;
						}
						return 26;
					}
					else
					{
						if (w >= 16777216)
						{
							return 25;
						}
						return 24;
					}
				}
				else if (w >= 524288)
				{
					if (w >= 2097152)
					{
						if (w >= 4194304)
						{
							return 23;
						}
						return 22;
					}
					else
					{
						if (w >= 1048576)
						{
							return 21;
						}
						return 20;
					}
				}
				else if (w >= 131072)
				{
					if (w >= 262144)
					{
						return 19;
					}
					return 18;
				}
				else
				{
					if (w >= 65536)
					{
						return 17;
					}
					return 16;
				}
			}
			else if (w >= 128)
			{
				if (w >= 2048)
				{
					if (w >= 8192)
					{
						if (w >= 16384)
						{
							return 15;
						}
						return 14;
					}
					else
					{
						if (w >= 4096)
						{
							return 13;
						}
						return 12;
					}
				}
				else if (w >= 512)
				{
					if (w >= 1024)
					{
						return 11;
					}
					return 10;
				}
				else
				{
					if (w >= 256)
					{
						return 9;
					}
					return 8;
				}
			}
			else if (w >= 8)
			{
				if (w >= 32)
				{
					if (w >= 64)
					{
						return 7;
					}
					return 6;
				}
				else
				{
					if (w >= 16)
					{
						return 5;
					}
					return 4;
				}
			}
			else if (w >= 2)
			{
				if (w >= 4)
				{
					return 3;
				}
				return 2;
			}
			else
			{
				if (w >= 1)
				{
					return 1;
				}
				if (w >= 0)
				{
					return 0;
				}
				return 32;
			}
		}

		private bool QuickPow2Check()
		{
			return this.sign > 0 && this.nBits == 1;
		}

		public int CompareTo(object obj)
		{
			return this.CompareTo((BigInteger)obj);
		}

		private static int CompareTo(int xIndx, int[] x, int yIndx, int[] y)
		{
			while (xIndx != x.Length)
			{
				if (x[xIndx] != 0)
				{
					break;
				}
				xIndx++;
			}
			while (yIndx != y.Length && y[yIndx] == 0)
			{
				yIndx++;
			}
			return BigInteger.CompareNoLeadingZeroes(xIndx, x, yIndx, y);
		}

		private static int CompareNoLeadingZeroes(int xIndx, int[] x, int yIndx, int[] y)
		{
			int num = x.Length - y.Length - (xIndx - yIndx);
			if (num == 0)
			{
				while (xIndx < x.Length)
				{
					uint num2 = (uint)x[xIndx++];
					uint num3 = (uint)y[yIndx++];
					if (num2 != num3)
					{
						if (num2 >= num3)
						{
							return 1;
						}
						return -1;
					}
				}
				return 0;
			}
			if (num >= 0)
			{
				return 1;
			}
			return -1;
		}

		public int CompareTo(BigInteger value)
		{
			if (this.sign < value.sign)
			{
				return -1;
			}
			if (this.sign > value.sign)
			{
				return 1;
			}
			if (this.sign != 0)
			{
				return this.sign * BigInteger.CompareNoLeadingZeroes(0, this.magnitude, 0, value.magnitude);
			}
			return 0;
		}

		private int[] Divide(int[] x, int[] y)
		{
			int num = 0;
			while (num < x.Length && x[num] == 0)
			{
				num++;
			}
			int num2 = 0;
			while (num2 < y.Length && y[num2] == 0)
			{
				num2++;
			}
			int num3 = BigInteger.CompareNoLeadingZeroes(num, x, num2, y);
			int[] array3;
			if (num3 > 0)
			{
				int num4 = this.calcBitLength(num2, y);
				int num5 = this.calcBitLength(num, x);
				int num6 = num5 - num4;
				int num7 = 0;
				int num8 = 0;
				int num9 = num4;
				int[] array;
				int[] array2;
				if (num6 > 0)
				{
					array = new int[(num6 >> 5) + 1];
					array[0] = 1 << num6 % 32;
					array2 = BigInteger.ShiftLeft(y, num6);
					num9 += num6;
				}
				else
				{
					array = new int[]
					{
						1
					};
					int num10 = y.Length - num2;
					array2 = new int[num10];
					Array.Copy(y, num2, array2, 0, num10);
				}
				array3 = new int[array.Length];
				for (;;)
				{
					if (num9 < num5 || BigInteger.CompareNoLeadingZeroes(num, x, num8, array2) >= 0)
					{
						BigInteger.Subtract(num, x, num8, array2);
						BigInteger.AddMagnitudes(array3, array);
						while (x[num] == 0)
						{
							if (++num == x.Length)
							{
								return array3;
							}
						}
						num5 = 32 * (x.Length - num - 1) + BigInteger.BitLen(x[num]);
						if (num5 <= num4)
						{
							if (num5 < num4)
							{
								return array3;
							}
							num3 = BigInteger.CompareNoLeadingZeroes(num, x, num2, y);
							if (num3 <= 0)
							{
								goto IL_1CA;
							}
						}
					}
					num6 = num9 - num5;
					if (num6 == 1)
					{
						uint num11 = (uint)array2[num8] >> 1;
						uint num12 = (uint)x[num];
						if (num11 > num12)
						{
							num6++;
						}
					}
					if (num6 < 2)
					{
						array2 = BigInteger.ShiftRightOneInPlace(num8, array2);
						num9--;
						array = BigInteger.ShiftRightOneInPlace(num7, array);
					}
					else
					{
						array2 = BigInteger.ShiftRightInPlace(num8, array2, num6);
						num9 -= num6;
						array = BigInteger.ShiftRightInPlace(num7, array, num6);
					}
					while (array2[num8] == 0)
					{
						num8++;
					}
					while (array[num7] == 0)
					{
						num7++;
					}
				}
				return array3;
			}
			array3 = new int[1];
			IL_1CA:
			if (num3 == 0)
			{
				BigInteger.AddMagnitudes(array3, BigInteger.One.magnitude);
				Array.Clear(x, num, x.Length - num);
			}
			return array3;
		}

		public BigInteger Divide(BigInteger val)
		{
			if (val.sign == 0)
			{
				throw new ArithmeticException("Division by zero error");
			}
			if (this.sign == 0)
			{
				return BigInteger.Zero;
			}
			if (!val.QuickPow2Check())
			{
				int[] x = (int[])this.magnitude.Clone();
				return new BigInteger(this.sign * val.sign, this.Divide(x, val.magnitude), true);
			}
			BigInteger bigInteger = this.Abs().ShiftRight(val.Abs().BitLength - 1);
			if (val.sign != this.sign)
			{
				return bigInteger.Negate();
			}
			return bigInteger;
		}

		public BigInteger[] DivideAndRemainder(BigInteger val)
		{
			if (val.sign == 0)
			{
				throw new ArithmeticException("Division by zero error");
			}
			BigInteger[] array = new BigInteger[2];
			if (this.sign == 0)
			{
				array[0] = BigInteger.Zero;
				array[1] = BigInteger.Zero;
			}
			else if (val.QuickPow2Check())
			{
				int n = val.Abs().BitLength - 1;
				BigInteger bigInteger = this.Abs().ShiftRight(n);
				int[] mag = this.LastNBits(n);
				array[0] = ((val.sign == this.sign) ? bigInteger : bigInteger.Negate());
				array[1] = new BigInteger(this.sign, mag, true);
			}
			else
			{
				int[] array2 = (int[])this.magnitude.Clone();
				int[] mag2 = this.Divide(array2, val.magnitude);
				array[0] = new BigInteger(this.sign * val.sign, mag2, true);
				array[1] = new BigInteger(this.sign, array2, true);
			}
			return array;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			BigInteger bigInteger = obj as BigInteger;
			if (bigInteger == null)
			{
				return false;
			}
			if (bigInteger.sign != this.sign || bigInteger.magnitude.Length != this.magnitude.Length)
			{
				return false;
			}
			for (int i = 0; i < this.magnitude.Length; i++)
			{
				if (bigInteger.magnitude[i] != this.magnitude[i])
				{
					return false;
				}
			}
			return true;
		}

		public BigInteger Gcd(BigInteger value)
		{
			if (value.sign == 0)
			{
				return this.Abs();
			}
			if (this.sign == 0)
			{
				return value.Abs();
			}
			BigInteger bigInteger = this;
			BigInteger bigInteger2 = value;
			while (bigInteger2.sign != 0)
			{
				BigInteger bigInteger3 = bigInteger.Mod(bigInteger2);
				bigInteger = bigInteger2;
				bigInteger2 = bigInteger3;
			}
			return bigInteger;
		}

		public override int GetHashCode()
		{
			int num = this.magnitude.Length;
			if (this.magnitude.Length > 0)
			{
				num ^= this.magnitude[0];
				if (this.magnitude.Length > 1)
				{
					num ^= this.magnitude[this.magnitude.Length - 1];
				}
			}
			if (this.sign >= 0)
			{
				return num;
			}
			return ~num;
		}

		private BigInteger Inc()
		{
			if (this.sign == 0)
			{
				return BigInteger.One;
			}
			if (this.sign < 0)
			{
				return new BigInteger(-1, BigInteger.doSubBigLil(this.magnitude, BigInteger.One.magnitude), true);
			}
			return this.AddToMagnitude(BigInteger.One.magnitude);
		}

		public int IntValue
		{
			get
			{
				if (this.sign == 0)
				{
					return 0;
				}
				if (this.sign <= 0)
				{
					return -this.magnitude[this.magnitude.Length - 1];
				}
				return this.magnitude[this.magnitude.Length - 1];
			}
		}

		public bool IsProbablePrime(int certainty)
		{
			if (certainty <= 0)
			{
				return true;
			}
			BigInteger bigInteger = this.Abs();
			if (!bigInteger.TestBit(0))
			{
				return bigInteger.Equals(BigInteger.Two);
			}
			return !bigInteger.Equals(BigInteger.One) && bigInteger.CheckProbablePrime(certainty, BigInteger.RandomSource);
		}

		private bool CheckProbablePrime(int certainty, Random random)
		{
			int num = Math.Min(this.BitLength - 1, BigInteger.primeLists.Length);
			for (int i = 0; i < num; i++)
			{
				int num2 = this.Remainder(BigInteger.primeProducts[i]);
				foreach (int num3 in BigInteger.primeLists[i])
				{
					if (num2 % num3 == 0)
					{
						return this.BitLength < 16 && this.IntValue == num3;
					}
				}
			}
			return this.RabinMillerTest(certainty, random);
		}

		internal bool RabinMillerTest(int certainty, Random random)
		{
			BigInteger bigInteger = this.Subtract(BigInteger.One);
			int lowestSetBit = bigInteger.GetLowestSetBit();
			BigInteger exponent = bigInteger.ShiftRight(lowestSetBit);
			for (;;)
			{
				BigInteger bigInteger2 = new BigInteger(this.BitLength, random);
				if (bigInteger2.CompareTo(BigInteger.One) > 0 && bigInteger2.CompareTo(bigInteger) < 0)
				{
					BigInteger bigInteger3 = bigInteger2.ModPow(exponent, this);
					if (!bigInteger3.Equals(BigInteger.One))
					{
						int num = 0;
						while (!bigInteger3.Equals(bigInteger))
						{
							if (++num == lowestSetBit)
							{
								return false;
							}
							bigInteger3 = bigInteger3.ModPow(BigInteger.Two, this);
							if (bigInteger3.Equals(BigInteger.One))
							{
								return false;
							}
						}
					}
					certainty -= 2;
					if (certainty <= 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		public long LongValue
		{
			get
			{
				if (this.sign == 0)
				{
					return 0L;
				}
				long num;
				if (this.magnitude.Length > 1)
				{
					num = ((long)this.magnitude[this.magnitude.Length - 2] << 32 | ((long)this.magnitude[this.magnitude.Length - 1] & (long)((ulong)-1)));
				}
				else
				{
					num = ((long)this.magnitude[this.magnitude.Length - 1] & (long)((ulong)-1));
				}
				if (this.sign >= 0)
				{
					return num;
				}
				return -num;
			}
		}

		public BigInteger Max(BigInteger value)
		{
			if (this.CompareTo(value) <= 0)
			{
				return value;
			}
			return this;
		}

		public BigInteger Min(BigInteger value)
		{
			if (this.CompareTo(value) >= 0)
			{
				return value;
			}
			return this;
		}

		public BigInteger Mod(BigInteger m)
		{
			if (m.sign < 1)
			{
				throw new ArithmeticException("Modulus must be positive");
			}
			BigInteger bigInteger = this.Remainder(m);
			if (bigInteger.sign < 0)
			{
				return bigInteger.Add(m);
			}
			return bigInteger;
		}

		public BigInteger ModInverse(BigInteger m)
		{
			if (m.sign < 1)
			{
				throw new ArithmeticException("Modulus must be positive");
			}
			BigInteger bigInteger = new BigInteger();
			BigInteger bigInteger2 = BigInteger.ExtEuclid(this.Mod(m), m, bigInteger, null);
			if (!bigInteger2.Equals(BigInteger.One))
			{
				throw new ArithmeticException("Numbers not relatively prime.");
			}
			if (bigInteger.sign < 0)
			{
				bigInteger.sign = 1;
				bigInteger.magnitude = BigInteger.doSubBigLil(m.magnitude, bigInteger.magnitude);
			}
			return bigInteger;
		}

		private static BigInteger ExtEuclid(BigInteger a, BigInteger b, BigInteger u1Out, BigInteger u2Out)
		{
			BigInteger bigInteger = BigInteger.One;
			BigInteger bigInteger2 = a;
			BigInteger bigInteger3 = BigInteger.Zero;
			BigInteger bigInteger4 = b;
			while (bigInteger4.sign > 0)
			{
				BigInteger[] array = bigInteger2.DivideAndRemainder(bigInteger4);
				BigInteger n = bigInteger3.Multiply(array[0]);
				BigInteger bigInteger5 = bigInteger.Subtract(n);
				bigInteger = bigInteger3;
				bigInteger3 = bigInteger5;
				bigInteger2 = bigInteger4;
				bigInteger4 = array[1];
			}
			if (u1Out != null)
			{
				u1Out.sign = bigInteger.sign;
				u1Out.magnitude = bigInteger.magnitude;
			}
			if (u2Out != null)
			{
				BigInteger bigInteger6 = bigInteger.Multiply(a);
				bigInteger6 = bigInteger2.Subtract(bigInteger6);
				BigInteger bigInteger7 = bigInteger6.Divide(b);
				u2Out.sign = bigInteger7.sign;
				u2Out.magnitude = bigInteger7.magnitude;
			}
			return bigInteger2;
		}

		private static void ZeroOut(int[] x)
		{
			Array.Clear(x, 0, x.Length);
		}

		public BigInteger ModPow(BigInteger exponent, BigInteger m)
		{
			if (m.sign < 1)
			{
				throw new ArithmeticException("Modulus must be positive");
			}
			if (m.Equals(BigInteger.One))
			{
				return BigInteger.Zero;
			}
			if (exponent.sign == 0)
			{
				return BigInteger.One;
			}
			if (this.sign == 0)
			{
				return BigInteger.Zero;
			}
			int[] array = null;
			int[] array2 = null;
			bool flag = (m.magnitude[m.magnitude.Length - 1] & 1) == 1;
			long num = 0L;
			if (flag)
			{
				num = m.GetMQuote();
				BigInteger bigInteger = this.ShiftLeft(32 * m.magnitude.Length).Mod(m);
				array = bigInteger.magnitude;
				flag = (array.Length <= m.magnitude.Length);
				if (flag)
				{
					array2 = new int[m.magnitude.Length + 1];
					if (array.Length < m.magnitude.Length)
					{
						int[] array3 = new int[m.magnitude.Length];
						array.CopyTo(array3, array3.Length - array.Length);
						array = array3;
					}
				}
			}
			if (!flag)
			{
				if (this.magnitude.Length <= m.magnitude.Length)
				{
					array = new int[m.magnitude.Length];
					this.magnitude.CopyTo(array, array.Length - this.magnitude.Length);
				}
				else
				{
					BigInteger bigInteger2 = this.Remainder(m);
					array = new int[m.magnitude.Length];
					bigInteger2.magnitude.CopyTo(array, array.Length - bigInteger2.magnitude.Length);
				}
				array2 = new int[m.magnitude.Length * 2];
			}
			int[] array4 = new int[m.magnitude.Length];
			for (int i = 0; i < exponent.magnitude.Length; i++)
			{
				int j = exponent.magnitude[i];
				int k = 0;
				if (i == 0)
				{
					while (j > 0)
					{
						j <<= 1;
						k++;
					}
					array.CopyTo(array4, 0);
					j <<= 1;
					k++;
				}
				while (j != 0)
				{
					if (flag)
					{
						BigInteger.MultiplyMonty(array2, array4, array4, m.magnitude, num);
					}
					else
					{
						BigInteger.Square(array2, array4);
						this.Remainder(array2, m.magnitude);
						Array.Copy(array2, array2.Length - array4.Length, array4, 0, array4.Length);
						BigInteger.ZeroOut(array2);
					}
					k++;
					if (j < 0)
					{
						if (flag)
						{
							BigInteger.MultiplyMonty(array2, array4, array, m.magnitude, num);
						}
						else
						{
							BigInteger.Multiply(array2, array4, array);
							this.Remainder(array2, m.magnitude);
							Array.Copy(array2, array2.Length - array4.Length, array4, 0, array4.Length);
							BigInteger.ZeroOut(array2);
						}
					}
					j <<= 1;
				}
				while (k < 32)
				{
					if (flag)
					{
						BigInteger.MultiplyMonty(array2, array4, array4, m.magnitude, num);
					}
					else
					{
						BigInteger.Square(array2, array4);
						this.Remainder(array2, m.magnitude);
						Array.Copy(array2, array2.Length - array4.Length, array4, 0, array4.Length);
						BigInteger.ZeroOut(array2);
					}
					k++;
				}
			}
			if (flag)
			{
				BigInteger.ZeroOut(array);
				array[array.Length - 1] = 1;
				BigInteger.MultiplyMonty(array2, array4, array, m.magnitude, num);
			}
			BigInteger bigInteger3 = new BigInteger(1, array4, true);
			if (exponent.sign <= 0)
			{
				return bigInteger3.ModInverse(m);
			}
			return bigInteger3;
		}

		private static int[] Square(int[] w, int[] x)
		{
			int num = w.Length - 1;
			ulong num4;
			ulong num5;
			for (int num2 = x.Length - 1; num2 != 0; num2--)
			{
				ulong num3 = (ulong)x[num2];
				num4 = num3 * num3;
				num5 = num4 >> 32;
				num4 = (ulong)((uint)num4);
				num4 += (ulong)w[num];
				w[num] = (int)((uint)num4);
				ulong num6 = num5 + (num4 >> 32);
				for (int i = num2 - 1; i >= 0; i--)
				{
					num--;
					num4 = num3 * (ulong)x[i];
					num5 = num4 >> 31;
					num4 = (ulong)((uint)((uint)num4 << 1));
					num4 += num6 + (ulong)w[num];
					w[num] = (int)((uint)num4);
					num6 = num5 + (num4 >> 32);
				}
				num6 += (ulong)w[--num];
				w[num] = (int)((uint)num6);
				if (--num >= 0)
				{
					w[num] = (int)((uint)(num6 >> 32));
				}
				num += num2;
			}
			num4 = (ulong)x[0];
			num4 *= num4;
			num5 = num4 >> 32;
			num4 &= (ulong)-1;
			num4 += (ulong)w[num];
			w[num] = (int)((uint)num4);
			if (--num >= 0)
			{
				w[num] = (int)((uint)(num5 + (num4 >> 32) + (ulong)w[num]));
			}
			return w;
		}

		private static int[] Multiply(int[] x, int[] y, int[] z)
		{
			int num = z.Length;
			if (num < 1)
			{
				return x;
			}
			int num2 = x.Length - y.Length;
			long num4;
			for (;;)
			{
				long num3 = (long)z[--num] & (long)((ulong)-1);
				num4 = 0L;
				for (int i = y.Length - 1; i >= 0; i--)
				{
					num4 += num3 * ((long)y[i] & (long)((ulong)-1)) + ((long)x[num2 + i] & (long)((ulong)-1));
					x[num2 + i] = (int)num4;
					num4 = (long)((ulong)num4 >> 32);
				}
				num2--;
				if (num < 1)
				{
					break;
				}
				x[num2] = (int)num4;
			}
			if (num2 >= 0)
			{
				x[num2] = (int)num4;
			}
			return x;
		}

		private static long FastExtEuclid(long a, long b, long[] uOut)
		{
			long num = 1L;
			long num2 = a;
			long num3 = 0L;
			long num6;
			for (long num4 = b; num4 > 0L; num4 = num6)
			{
				long num5 = num2 / num4;
				num6 = num - num3 * num5;
				num = num3;
				num3 = num6;
				num6 = num2 - num4 * num5;
				num2 = num4;
			}
			uOut[0] = num;
			uOut[1] = (num2 - num * a) / b;
			return num2;
		}

		private static long FastModInverse(long v, long m)
		{
			if (m < 1L)
			{
				throw new ArithmeticException("Modulus must be positive");
			}
			long[] array = new long[2];
			long num = BigInteger.FastExtEuclid(v, m, array);
			if (num != 1L)
			{
				throw new ArithmeticException("Numbers not relatively prime.");
			}
			if (array[0] < 0L)
			{
				array[0] += m;
			}
			return array[0];
		}

		private long GetMQuote()
		{
			if (this.mQuote != -1L)
			{
				return this.mQuote;
			}
			if (this.magnitude.Length == 0 || (this.magnitude[this.magnitude.Length - 1] & 1) == 0)
			{
				return -1L;
			}
			long v = (long)(~this.magnitude[this.magnitude.Length - 1] | 1) & (long)((ulong)-1);
			this.mQuote = BigInteger.FastModInverse(v, 4294967296L);
			return this.mQuote;
		}

		private static void MultiplyMonty(int[] a, int[] x, int[] y, int[] m, long mQuote)
		{
			if (m.Length == 1)
			{
				x[0] = (int)BigInteger.MultiplyMontyNIsOne((uint)x[0], (uint)y[0], (uint)m[0], (ulong)mQuote);
				return;
			}
			int num = m.Length;
			int num2 = num - 1;
			long num3 = (long)y[num2] & (long)((ulong)-1);
			Array.Clear(a, 0, num + 1);
			for (int i = num; i > 0; i--)
			{
				long num4 = (long)x[i - 1] & (long)((ulong)-1);
				long num5 = (((long)a[num] & (long)((ulong)-1)) + (num4 * num3 & (long)((ulong)-1)) & (long)((ulong)-1)) * mQuote & (long)((ulong)-1);
				long num6 = num4 * num3;
				long num7 = num5 * ((long)m[num2] & (long)((ulong)-1));
				long num8 = ((long)a[num] & (long)((ulong)-1)) + (num6 & (long)((ulong)-1)) + (num7 & (long)((ulong)-1));
				long num9 = (long)(((ulong)num6 >> 32) + ((ulong)num7 >> 32) + ((ulong)num8 >> 32));
				for (int j = num2; j > 0; j--)
				{
					num6 = num4 * ((long)y[j - 1] & (long)((ulong)-1));
					num7 = num5 * ((long)m[j - 1] & (long)((ulong)-1));
					num8 = ((long)a[j] & (long)((ulong)-1)) + (num6 & (long)((ulong)-1)) + (num7 & (long)((ulong)-1)) + (num9 & (long)((ulong)-1));
					num9 = (long)(((ulong)num9 >> 32) + ((ulong)num6 >> 32) + ((ulong)num7 >> 32) + ((ulong)num8 >> 32));
					a[j + 1] = (int)num8;
				}
				num9 += ((long)a[0] & (long)((ulong)-1));
				a[1] = (int)num9;
				a[0] = (int)((ulong)num9 >> 32);
			}
			if (BigInteger.CompareTo(0, a, 0, m) >= 0)
			{
				BigInteger.Subtract(0, a, 0, m);
			}
			Array.Copy(a, 1, x, 0, num);
		}

		private static uint MultiplyMontyNIsOne(uint x, uint y, uint m, ulong mQuote)
		{
			ulong num = (ulong)m;
			ulong num2 = (ulong)x * (ulong)y;
			ulong num3 = num2 * mQuote & BigInteger.UIMASK;
			ulong num4 = num3 * num;
			ulong num5 = (num2 & BigInteger.UIMASK) + (num4 & BigInteger.UIMASK);
			ulong num6 = (num2 >> 32) + (num4 >> 32) + (num5 >> 32);
			if (num6 > num)
			{
				num6 -= num;
			}
			return (uint)(num6 & BigInteger.UIMASK);
		}

		public BigInteger Multiply(BigInteger val)
		{
			if (this.sign == 0 || val.sign == 0)
			{
				return BigInteger.Zero;
			}
			if (val.QuickPow2Check())
			{
				BigInteger bigInteger = this.ShiftLeft(val.Abs().BitLength - 1);
				if (val.sign <= 0)
				{
					return bigInteger.Negate();
				}
				return bigInteger;
			}
			else
			{
				if (!this.QuickPow2Check())
				{
					int num = (this.BitLength + val.BitLength) / 32 + 1;
					int[] array = new int[num];
					if (val == this)
					{
						BigInteger.Square(array, this.magnitude);
					}
					else
					{
						BigInteger.Multiply(array, this.magnitude, val.magnitude);
					}
					return new BigInteger(this.sign * val.sign, array, true);
				}
				BigInteger bigInteger2 = val.ShiftLeft(this.Abs().BitLength - 1);
				if (this.sign <= 0)
				{
					return bigInteger2.Negate();
				}
				return bigInteger2;
			}
		}

		public BigInteger Negate()
		{
			if (this.sign == 0)
			{
				return this;
			}
			return new BigInteger(-this.sign, this.magnitude, false);
		}

		public BigInteger NextProbablePrime()
		{
			if (this.sign < 0)
			{
				throw new ArithmeticException("Cannot be called on value < 0");
			}
			if (this.CompareTo(BigInteger.Two) < 0)
			{
				return BigInteger.Two;
			}
			BigInteger bigInteger = this.Inc().SetBit(0);
			while (!bigInteger.CheckProbablePrime(100, BigInteger.RandomSource))
			{
				bigInteger = bigInteger.Add(BigInteger.Two);
			}
			return bigInteger;
		}

		public BigInteger Not()
		{
			return this.Inc().Negate();
		}

		public BigInteger Pow(int exp)
		{
			if (exp < 0)
			{
				throw new ArithmeticException("Negative exponent");
			}
			if (exp == 0)
			{
				return BigInteger.One;
			}
			if (this.sign == 0 || this.Equals(BigInteger.One))
			{
				return this;
			}
			BigInteger bigInteger = BigInteger.One;
			BigInteger bigInteger2 = this;
			for (;;)
			{
				if ((exp & 1) == 1)
				{
					bigInteger = bigInteger.Multiply(bigInteger2);
				}
				exp >>= 1;
				if (exp == 0)
				{
					break;
				}
				bigInteger2 = bigInteger2.Multiply(bigInteger2);
			}
			return bigInteger;
		}

		public static BigInteger ProbablePrime(int bitLength, Random random)
		{
			return new BigInteger(bitLength, 100, random);
		}

		private int Remainder(int m)
		{
			long num = 0L;
			for (int i = 0; i < this.magnitude.Length; i++)
			{
				long num2 = (long)((ulong)this.magnitude[i]);
				num = (num << 32 | num2) % (long)m;
			}
			return (int)num;
		}

		private int[] Remainder(int[] x, int[] y)
		{
			int num = 0;
			while (num < x.Length && x[num] == 0)
			{
				num++;
			}
			int num2 = 0;
			while (num2 < y.Length && y[num2] == 0)
			{
				num2++;
			}
			int num3 = BigInteger.CompareNoLeadingZeroes(num, x, num2, y);
			if (num3 > 0)
			{
				int num4 = this.calcBitLength(num2, y);
				int num5 = this.calcBitLength(num, x);
				int num6 = num5 - num4;
				int num7 = 0;
				int num8 = num4;
				int[] array;
				if (num6 > 0)
				{
					array = BigInteger.ShiftLeft(y, num6);
					num8 += num6;
				}
				else
				{
					int num9 = y.Length - num2;
					array = new int[num9];
					Array.Copy(y, num2, array, 0, num9);
				}
				for (;;)
				{
					if (num8 < num5 || BigInteger.CompareNoLeadingZeroes(num, x, num7, array) >= 0)
					{
						BigInteger.Subtract(num, x, num7, array);
						while (x[num] == 0)
						{
							if (++num == x.Length)
							{
								return x;
							}
						}
						num5 = 32 * (x.Length - num - 1) + BigInteger.BitLen(x[num]);
						if (num5 <= num4)
						{
							if (num5 < num4)
							{
								return x;
							}
							num3 = BigInteger.CompareNoLeadingZeroes(num, x, num2, y);
							if (num3 <= 0)
							{
								goto IL_152;
							}
						}
					}
					num6 = num8 - num5;
					if (num6 == 1)
					{
						uint num10 = (uint)array[num7] >> 1;
						uint num11 = (uint)x[num];
						if (num10 > num11)
						{
							num6++;
						}
					}
					if (num6 < 2)
					{
						array = BigInteger.ShiftRightOneInPlace(num7, array);
						num8--;
					}
					else
					{
						array = BigInteger.ShiftRightInPlace(num7, array, num6);
						num8 -= num6;
					}
					while (array[num7] == 0)
					{
						num7++;
					}
				}
				return x;
			}
			IL_152:
			if (num3 == 0)
			{
				Array.Clear(x, num, x.Length - num);
			}
			return x;
		}

		public BigInteger Remainder(BigInteger n)
		{
			if (n.sign == 0)
			{
				throw new ArithmeticException("Division by zero error");
			}
			if (this.sign == 0)
			{
				return BigInteger.Zero;
			}
			if (n.magnitude.Length == 1)
			{
				int num = n.magnitude[0];
				if (num > 0)
				{
					if (num == 1)
					{
						return BigInteger.Zero;
					}
					int num2 = this.Remainder(num);
					if (num2 != 0)
					{
						return new BigInteger(this.sign, new int[]
						{
							num2
						}, false);
					}
					return BigInteger.Zero;
				}
			}
			if (BigInteger.CompareNoLeadingZeroes(0, this.magnitude, 0, n.magnitude) < 0)
			{
				return this;
			}
			int[] array;
			if (n.QuickPow2Check())
			{
				array = this.LastNBits(n.Abs().BitLength - 1);
			}
			else
			{
				array = (int[])this.magnitude.Clone();
				array = this.Remainder(array, n.magnitude);
			}
			return new BigInteger(this.sign, array, true);
		}

		private int[] LastNBits(int n)
		{
			if (n < 1)
			{
				return BigInteger.ZeroMagnitude;
			}
			int num = (n + 32 - 1) / 32;
			num = Math.Min(num, this.magnitude.Length);
			int[] array = new int[num];
			Array.Copy(this.magnitude, this.magnitude.Length - num, array, 0, num);
			int num2 = n % 32;
			if (num2 != 0)
			{
				array[0] &= ~(-1 << num2);
			}
			return array;
		}

		private static int[] ShiftLeft(int[] mag, int n)
		{
			int num = (int)((uint)n >> 5);
			int num2 = n & 31;
			int num3 = mag.Length;
			int[] array;
			if (num2 == 0)
			{
				array = new int[num3 + num];
				mag.CopyTo(array, 0);
			}
			else
			{
				int num4 = 0;
				int num5 = 32 - num2;
				int num6 = (int)((uint)mag[0] >> num5);
				if (num6 != 0)
				{
					array = new int[num3 + num + 1];
					array[num4++] = num6;
				}
				else
				{
					array = new int[num3 + num];
				}
				int num7 = mag[0];
				for (int i = 0; i < num3 - 1; i++)
				{
					int num8 = mag[i + 1];
					array[num4++] = (num7 << num2 | (int)((uint)num8 >> num5));
					num7 = num8;
				}
				array[num4] = mag[num3 - 1] << num2;
			}
			return array;
		}

		public BigInteger ShiftLeft(int n)
		{
			if (this.sign == 0 || this.magnitude.Length == 0)
			{
				return BigInteger.Zero;
			}
			if (n == 0)
			{
				return this;
			}
			if (n < 0)
			{
				return this.ShiftRight(-n);
			}
			BigInteger bigInteger = new BigInteger(this.sign, BigInteger.ShiftLeft(this.magnitude, n), true);
			if (this.nBits != -1)
			{
				bigInteger.nBits = ((this.sign > 0) ? this.nBits : (this.nBits + n));
			}
			if (this.nBitLength != -1)
			{
				bigInteger.nBitLength = this.nBitLength + n;
			}
			return bigInteger;
		}

		private static int[] ShiftRightInPlace(int start, int[] mag, int n)
		{
			int num = (int)(((uint)n >> 5) + (uint)start);
			int num2 = n & 31;
			int num3 = mag.Length - 1;
			if (num != start)
			{
				int num4 = num - start;
				for (int i = num3; i >= num; i--)
				{
					mag[i] = mag[i - num4];
				}
				for (int j = num - 1; j >= start; j--)
				{
					mag[j] = 0;
				}
			}
			if (num2 != 0)
			{
				int num5 = 32 - num2;
				int num6 = mag[num3];
				for (int k = num3; k > num; k--)
				{
					int num7 = mag[k - 1];
					mag[k] = (int)((uint)num6 >> num2 | (uint)((uint)num7 << num5));
					num6 = num7;
				}
				mag[num] = (int)((uint)mag[num] >> num2);
			}
			return mag;
		}

		private static int[] ShiftRightOneInPlace(int start, int[] mag)
		{
			int num = mag.Length;
			int num2 = mag[num - 1];
			while (--num > start)
			{
				int num3 = mag[num - 1];
				mag[num] = (int)((uint)num2 >> 1 | (uint)((uint)num3 << 31));
				num2 = num3;
			}
			mag[start] = (int)((uint)mag[start] >> 1);
			return mag;
		}

		public BigInteger ShiftRight(int n)
		{
			if (n == 0)
			{
				return this;
			}
			if (n < 0)
			{
				return this.ShiftLeft(-n);
			}
			if (n < this.BitLength)
			{
				int num = this.BitLength - n + 31 >> 5;
				int[] array = new int[num];
				int num2 = n >> 5;
				int num3 = n & 31;
				if (num3 == 0)
				{
					Array.Copy(this.magnitude, 0, array, 0, array.Length);
				}
				else
				{
					int num4 = 32 - num3;
					int num5 = this.magnitude.Length - 1 - num2;
					for (int i = num - 1; i >= 0; i--)
					{
						array[i] = (int)((uint)this.magnitude[num5--] >> (num3 & 31));
						if (num5 >= 0)
						{
							array[i] |= this.magnitude[num5] << num4;
						}
					}
				}
				return new BigInteger(this.sign, array, false);
			}
			if (this.sign >= 0)
			{
				return BigInteger.Zero;
			}
			return BigInteger.One.Negate();
		}

		public int SignValue
		{
			get
			{
				return this.sign;
			}
		}

		private static int[] Subtract(int xStart, int[] x, int yStart, int[] y)
		{
			int num = x.Length;
			int num2 = y.Length;
			int num3 = 0;
			do
			{
				long num4 = ((long)x[--num] & (long)((ulong)-1)) - ((long)y[--num2] & (long)((ulong)-1)) + (long)num3;
				x[num] = (int)num4;
				num3 = (int)(num4 >> 63);
			}
			while (num2 > yStart);
			if (num3 != 0)
			{
				while (--x[--num] == -1)
				{
				}
			}
			return x;
		}

		public BigInteger Subtract(BigInteger n)
		{
			if (n.sign == 0)
			{
				return this;
			}
			if (this.sign == 0)
			{
				return n.Negate();
			}
			if (this.sign != n.sign)
			{
				return this.Add(n.Negate());
			}
			int num = BigInteger.CompareNoLeadingZeroes(0, this.magnitude, 0, n.magnitude);
			if (num == 0)
			{
				return BigInteger.Zero;
			}
			BigInteger bigInteger;
			BigInteger bigInteger2;
			if (num < 0)
			{
				bigInteger = n;
				bigInteger2 = this;
			}
			else
			{
				bigInteger = this;
				bigInteger2 = n;
			}
			return new BigInteger(this.sign * num, BigInteger.doSubBigLil(bigInteger.magnitude, bigInteger2.magnitude), true);
		}

		private static int[] doSubBigLil(int[] bigMag, int[] lilMag)
		{
			int[] x = (int[])bigMag.Clone();
			return BigInteger.Subtract(0, x, 0, lilMag);
		}

		public byte[] ToByteArray()
		{
			return this.ToByteArray(false);
		}

		public byte[] ToByteArrayUnsigned()
		{
			return this.ToByteArray(true);
		}

		private byte[] ToByteArray(bool unsigned)
		{
			if (this.sign != 0)
			{
				int num = (unsigned && this.sign > 0) ? this.BitLength : (this.BitLength + 1);
				int byteLength = BigInteger.GetByteLength(num);
				byte[] array = new byte[byteLength];
				int i = this.magnitude.Length;
				int num2 = array.Length;
				if (this.sign > 0)
				{
					while (i > 1)
					{
						uint num3 = (uint)this.magnitude[--i];
						array[--num2] = (byte)num3;
						array[--num2] = (byte)(num3 >> 8);
						array[--num2] = (byte)(num3 >> 16);
						array[--num2] = (byte)(num3 >> 24);
					}
					uint num4;
					for (num4 = (uint)this.magnitude[0]; num4 > 255U; num4 >>= 8)
					{
						array[--num2] = (byte)num4;
					}
					array[num2 - 1] = (byte)num4;
				}
				else
				{
					bool flag = true;
					while (i > 1)
					{
						uint num5 = (uint)(~(uint)this.magnitude[--i]);
						if (flag)
						{
							flag = ((num5 += 1U) == 0U);
						}
						array[--num2] = (byte)num5;
						array[--num2] = (byte)(num5 >> 8);
						array[--num2] = (byte)(num5 >> 16);
						array[--num2] = (byte)(num5 >> 24);
					}
					uint num6 = (uint)this.magnitude[0];
					if (flag)
					{
						num6 -= 1U;
					}
					while (num6 > 255U)
					{
						array[--num2] = (byte)(~(byte)num6);
						num6 >>= 8;
					}
					array[--num2] = (byte)(~(byte)num6);
					if (num2 > 0)
					{
						array[num2 - 1] = byte.MaxValue;
					}
				}
				return array;
			}
			if (!unsigned)
			{
				return new byte[1];
			}
			return BigInteger.ZeroEncoding;
		}

		public override string ToString()
		{
			return this.ToString(10);
		}

		public string ToString(int radix)
		{
			if (radix != 2 && radix != 10 && radix != 16)
			{
				throw new FormatException("Only bases 2, 10, 16 are allowed");
			}
			if (this.magnitude == null)
			{
				return "null";
			}
			if (this.sign == 0)
			{
				return "0";
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (radix == 16)
			{
				stringBuilder.Append(this.magnitude[0].ToString("x"));
				for (int i = 1; i < this.magnitude.Length; i++)
				{
					stringBuilder.Append(this.magnitude[i].ToString("x8"));
				}
			}
			else if (radix == 2)
			{
				stringBuilder.Append('1');
				for (int j = this.BitLength - 2; j >= 0; j--)
				{
					stringBuilder.Append(this.TestBit(j) ? '1' : '0');
				}
			}
			else
			{
				Stack<string> stack = new Stack<string>();
				BigInteger bigInteger = BigInteger.ValueOf((long)radix);
				BigInteger bigInteger2 = this.Abs();
				while (bigInteger2.sign != 0)
				{
					BigInteger bigInteger3 = bigInteger2.Mod(bigInteger);
					if (bigInteger3.sign == 0)
					{
						stack.Push("0");
					}
					else
					{
						stack.Push(bigInteger3.magnitude[0].ToString("d"));
					}
					bigInteger2 = bigInteger2.Divide(bigInteger);
				}
				while (stack.Count != 0)
				{
					stringBuilder.Append(stack.Pop());
				}
			}
			string text = stringBuilder.ToString();
			if (text[0] == '0')
			{
				int num = 0;
				while (text[++num] == '0')
				{
				}
				text = text.Substring(num);
			}
			if (this.sign == -1)
			{
				text = "-" + text;
			}
			return text;
		}

		private static BigInteger createUValueOf(ulong value)
		{
			int num = (int)(value >> 32);
			int num2 = (int)value;
			if (num != 0)
			{
				return new BigInteger(1, new int[]
				{
					num,
					num2
				}, false);
			}
			if (num2 != 0)
			{
				BigInteger bigInteger = new BigInteger(1, new int[]
				{
					num2
				}, false);
				if ((num2 & -num2) == num2)
				{
					bigInteger.nBits = 1;
				}
				return bigInteger;
			}
			return BigInteger.Zero;
		}

		private static BigInteger createValueOf(long value)
		{
			if (value >= 0L)
			{
				return BigInteger.createUValueOf((ulong)value);
			}
			if (value == -9223372036854775808L)
			{
				return BigInteger.createValueOf(~value).Not();
			}
			return BigInteger.createValueOf(-value).Negate();
		}

		public static BigInteger ValueOf(long value)
		{
			if (value <= 3L)
			{
				if (value < 0L)
				{
					goto IL_49;
				}
				switch ((int)value)
				{
				case 0:
					return BigInteger.Zero;
				case 1:
					return BigInteger.One;
				case 2:
					return BigInteger.Two;
				case 3:
					return BigInteger.Three;
				}
			}
			if (value == 10L)
			{
				return BigInteger.Ten;
			}
			IL_49:
			return BigInteger.createValueOf(value);
		}

		public int GetLowestSetBit()
		{
			if (this.sign == 0)
			{
				return -1;
			}
			int num = this.magnitude.Length;
			while (--num > 0 && this.magnitude[num] == 0)
			{
			}
			int num2 = this.magnitude[num];
			int num3 = ((num2 & 65535) == 0) ? (((num2 & 16711680) == 0) ? 7 : 15) : (((num2 & 255) == 0) ? 23 : 31);
			while (num3 > 0 && num2 << num3 != -2147483648)
			{
				num3--;
			}
			return (this.magnitude.Length - num) * 32 - (num3 + 1);
		}

		public bool TestBit(int n)
		{
			if (n < 0)
			{
				throw new ArithmeticException("Bit position must not be negative");
			}
			if (this.sign < 0)
			{
				return !this.Not().TestBit(n);
			}
			int num = n / 32;
			if (num >= this.magnitude.Length)
			{
				return false;
			}
			int num2 = this.magnitude[this.magnitude.Length - 1 - num];
			return (num2 >> n % 32 & 1) > 0;
		}

		public BigInteger Or(BigInteger value)
		{
			if (this.sign == 0)
			{
				return value;
			}
			if (value.sign == 0)
			{
				return this;
			}
			int[] array = (this.sign > 0) ? this.magnitude : this.Add(BigInteger.One).magnitude;
			int[] array2 = (value.sign > 0) ? value.magnitude : value.Add(BigInteger.One).magnitude;
			bool flag = this.sign < 0 || value.sign < 0;
			int num = Math.Max(array.Length, array2.Length);
			int[] array3 = new int[num];
			int num2 = array3.Length - array.Length;
			int num3 = array3.Length - array2.Length;
			for (int i = 0; i < array3.Length; i++)
			{
				int num4 = (i >= num2) ? array[i - num2] : 0;
				int num5 = (i >= num3) ? array2[i - num3] : 0;
				if (this.sign < 0)
				{
					num4 = ~num4;
				}
				if (value.sign < 0)
				{
					num5 = ~num5;
				}
				array3[i] = (num4 | num5);
				if (flag)
				{
					array3[i] = ~array3[i];
				}
			}
			BigInteger bigInteger = new BigInteger(1, array3, true);
			if (flag)
			{
				bigInteger = bigInteger.Not();
			}
			return bigInteger;
		}

		public BigInteger Xor(BigInteger value)
		{
			if (this.sign == 0)
			{
				return value;
			}
			if (value.sign == 0)
			{
				return this;
			}
			int[] array = (this.sign > 0) ? this.magnitude : this.Add(BigInteger.One).magnitude;
			int[] array2 = (value.sign > 0) ? value.magnitude : value.Add(BigInteger.One).magnitude;
			bool flag = (this.sign < 0 && value.sign >= 0) || (this.sign >= 0 && value.sign < 0);
			int num = Math.Max(array.Length, array2.Length);
			int[] array3 = new int[num];
			int num2 = array3.Length - array.Length;
			int num3 = array3.Length - array2.Length;
			for (int i = 0; i < array3.Length; i++)
			{
				int num4 = (i >= num2) ? array[i - num2] : 0;
				int num5 = (i >= num3) ? array2[i - num3] : 0;
				if (this.sign < 0)
				{
					num4 = ~num4;
				}
				if (value.sign < 0)
				{
					num5 = ~num5;
				}
				array3[i] = (num4 ^ num5);
				if (flag)
				{
					array3[i] = ~array3[i];
				}
			}
			BigInteger bigInteger = new BigInteger(1, array3, true);
			if (flag)
			{
				bigInteger = bigInteger.Not();
			}
			return bigInteger;
		}

		public BigInteger SetBit(int n)
		{
			if (n < 0)
			{
				throw new ArithmeticException("Bit address less than zero");
			}
			if (this.TestBit(n))
			{
				return this;
			}
			if (this.sign > 0 && n < this.BitLength - 1)
			{
				return this.FlipExistingBit(n);
			}
			return this.Or(BigInteger.One.ShiftLeft(n));
		}

		public BigInteger ClearBit(int n)
		{
			if (n < 0)
			{
				throw new ArithmeticException("Bit address less than zero");
			}
			if (!this.TestBit(n))
			{
				return this;
			}
			if (this.sign > 0 && n < this.BitLength - 1)
			{
				return this.FlipExistingBit(n);
			}
			return this.AndNot(BigInteger.One.ShiftLeft(n));
		}

		public BigInteger FlipBit(int n)
		{
			if (n < 0)
			{
				throw new ArithmeticException("Bit address less than zero");
			}
			if (this.sign > 0 && n < this.BitLength - 1)
			{
				return this.FlipExistingBit(n);
			}
			return this.Xor(BigInteger.One.ShiftLeft(n));
		}

		private BigInteger FlipExistingBit(int n)
		{
			int[] array = (int[])this.magnitude.Clone();
			array[array.Length - 1 - (n >> 5)] ^= 1 << n;
			return new BigInteger(this.sign, array, false);
		}
	}
}
