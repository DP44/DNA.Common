using System;
using System.Collections;
using System.Globalization;
using System.Text;

namespace DNA.Net.Lidgren
{
	internal class NetBigInteger
	{
		private const long IMASK = 4294967295L;

		private const int BitsPerByte = 8;

		private const int BitsPerInt = 32;

		private const int BytesPerInt = 4;

		private static readonly ulong UIMASK = (ulong)-1;

		private static readonly int[] ZeroMagnitude = new int[0];

		private static readonly byte[] ZeroEncoding = new byte[0];

		public static readonly NetBigInteger Zero = new NetBigInteger(0, NetBigInteger.ZeroMagnitude, false);

		public static readonly NetBigInteger One = NetBigInteger.createUValueOf(1UL);

		public static readonly NetBigInteger Two = NetBigInteger.createUValueOf(2UL);

		public static readonly NetBigInteger Three = NetBigInteger.createUValueOf(3UL);

		public static readonly NetBigInteger Ten = NetBigInteger.createUValueOf(10UL);

		private static readonly int chunk2 = 1;

		private static readonly NetBigInteger radix2 = NetBigInteger.ValueOf(2L);

		private static readonly NetBigInteger radix2E = NetBigInteger.radix2.Pow(NetBigInteger.chunk2);

		private static readonly int chunk10 = 19;

		private static readonly NetBigInteger radix10 = NetBigInteger.ValueOf(10L);

		private static readonly NetBigInteger radix10E = NetBigInteger.radix10.Pow(NetBigInteger.chunk10);

		private static readonly int chunk16 = 16;

		private static readonly NetBigInteger radix16 = NetBigInteger.ValueOf(16L);

		private static readonly NetBigInteger radix16E = NetBigInteger.radix16.Pow(NetBigInteger.chunk16);

		private int m_sign;

		private int[] m_magnitude;

		private int m_numBits = -1;

		private int m_numBitLength = -1;

		private long m_quote = -1L;

		private static int GetByteLength(int nBits)
		{
			return (nBits + 8 - 1) / 8;
		}

		private NetBigInteger()
		{
		}

		private NetBigInteger(int signum, int[] mag, bool checkMag)
		{
			if (!checkMag)
			{
				this.m_sign = signum;
				this.m_magnitude = mag;
				return;
			}
			int num = 0;
			while (num < mag.Length && mag[num] == 0)
			{
				num++;
			}
			if (num == mag.Length)
			{
				this.m_magnitude = NetBigInteger.ZeroMagnitude;
				return;
			}
			this.m_sign = signum;
			if (num == 0)
			{
				this.m_magnitude = mag;
				return;
			}
			this.m_magnitude = new int[mag.Length - num];
			Array.Copy(mag, num, this.m_magnitude, 0, this.m_magnitude.Length);
		}

		public NetBigInteger(string value) : this(value, 10)
		{
		}

		public NetBigInteger(string str, int radix)
		{
			if (str.Length == 0)
			{
				throw new FormatException("Zero length BigInteger");
			}
			NumberStyles style;
			int num;
			NetBigInteger netBigInteger;
			NetBigInteger val;
			if (radix != 2)
			{
				if (radix != 10)
				{
					if (radix != 16)
					{
						throw new FormatException("Only bases 2, 10, or 16 allowed");
					}
					style = NumberStyles.AllowHexSpecifier;
					num = NetBigInteger.chunk16;
					netBigInteger = NetBigInteger.radix16;
					val = NetBigInteger.radix16E;
				}
				else
				{
					style = NumberStyles.Integer;
					num = NetBigInteger.chunk10;
					netBigInteger = NetBigInteger.radix10;
					val = NetBigInteger.radix10E;
				}
			}
			else
			{
				style = NumberStyles.Integer;
				num = NetBigInteger.chunk2;
				netBigInteger = NetBigInteger.radix2;
				val = NetBigInteger.radix2E;
			}
			int num2 = 0;
			this.m_sign = 1;
			if (str[0] == '-')
			{
				if (str.Length == 1)
				{
					throw new FormatException("Zero length BigInteger");
				}
				this.m_sign = -1;
				num2 = 1;
			}
			while (num2 < str.Length && int.Parse(str[num2].ToString(), style) == 0)
			{
				num2++;
			}
			if (num2 >= str.Length)
			{
				this.m_sign = 0;
				this.m_magnitude = NetBigInteger.ZeroMagnitude;
				return;
			}
			NetBigInteger netBigInteger2 = NetBigInteger.Zero;
			int num3 = num2 + num;
			if (num3 <= str.Length)
			{
				string text;
				for (;;)
				{
					text = str.Substring(num2, num);
					ulong num4 = ulong.Parse(text, style);
					NetBigInteger value = NetBigInteger.createUValueOf(num4);
					if (radix != 2)
					{
						if (radix != 16)
						{
							netBigInteger2 = netBigInteger2.Multiply(val);
						}
						else
						{
							netBigInteger2 = netBigInteger2.ShiftLeft(64);
						}
					}
					else
					{
						if (num4 > 1UL)
						{
							break;
						}
						netBigInteger2 = netBigInteger2.ShiftLeft(1);
					}
					netBigInteger2 = netBigInteger2.Add(value);
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
				NetBigInteger netBigInteger3 = NetBigInteger.createUValueOf(value2);
				if (netBigInteger2.m_sign > 0)
				{
					if (radix != 2)
					{
						if (radix == 16)
						{
							netBigInteger2 = netBigInteger2.ShiftLeft(text2.Length << 2);
						}
						else
						{
							netBigInteger2 = netBigInteger2.Multiply(netBigInteger.Pow(text2.Length));
						}
					}
					netBigInteger2 = netBigInteger2.Add(netBigInteger3);
				}
				else
				{
					netBigInteger2 = netBigInteger3;
				}
			}
			this.m_magnitude = netBigInteger2.m_magnitude;
		}

		public NetBigInteger(byte[] bytes) : this(bytes, 0, bytes.Length)
		{
		}

		public NetBigInteger(byte[] bytes, int offset, int length)
		{
			if (length == 0)
			{
				throw new FormatException("Zero length BigInteger");
			}
			if ((sbyte)bytes[offset] >= 0)
			{
				this.m_magnitude = NetBigInteger.MakeMagnitude(bytes, offset, length);
				this.m_sign = ((this.m_magnitude.Length > 0) ? 1 : 0);
				return;
			}
			this.m_sign = -1;
			int num = offset + length;
			int num2 = offset;
			while (num2 < num && (sbyte)bytes[num2] == -1)
			{
				num2++;
			}
			if (num2 >= num)
			{
				this.m_magnitude = NetBigInteger.One.m_magnitude;
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
			this.m_magnitude = NetBigInteger.MakeMagnitude(array, 0, array.Length);
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
				return NetBigInteger.ZeroMagnitude;
			}
			int num3 = (num - num2 + 3) / 4;
			int num4 = (num - num2) % 4;
			if (num4 == 0)
			{
				num4 = 4;
			}
			if (num3 < 1)
			{
				return NetBigInteger.ZeroMagnitude;
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

		public NetBigInteger(int sign, byte[] bytes) : this(sign, bytes, 0, bytes.Length)
		{
		}

		public NetBigInteger(int sign, byte[] bytes, int offset, int length)
		{
			if (sign < -1 || sign > 1)
			{
				throw new FormatException("Invalid sign value");
			}
			if (sign == 0)
			{
				this.m_magnitude = NetBigInteger.ZeroMagnitude;
				return;
			}
			this.m_magnitude = NetBigInteger.MakeMagnitude(bytes, offset, length);
			this.m_sign = ((this.m_magnitude.Length < 1) ? 0 : sign);
		}

		public NetBigInteger Abs()
		{
			if (this.m_sign < 0)
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

		public NetBigInteger Add(NetBigInteger value)
		{
			if (this.m_sign == 0)
			{
				return value;
			}
			if (this.m_sign == value.m_sign)
			{
				return this.AddToMagnitude(value.m_magnitude);
			}
			if (value.m_sign == 0)
			{
				return this;
			}
			if (value.m_sign < 0)
			{
				return this.Subtract(value.Negate());
			}
			return value.Subtract(this.Negate());
		}

		private NetBigInteger AddToMagnitude(int[] magToAdd)
		{
			int[] array;
			int[] array2;
			if (this.m_magnitude.Length < magToAdd.Length)
			{
				array = magToAdd;
				array2 = this.m_magnitude;
			}
			else
			{
				array = this.m_magnitude;
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
			array3 = NetBigInteger.AddMagnitudes(array3, array2);
			return new NetBigInteger(this.m_sign, array3, flag);
		}

		public NetBigInteger And(NetBigInteger value)
		{
			if (this.m_sign == 0 || value.m_sign == 0)
			{
				return NetBigInteger.Zero;
			}
			int[] array = (this.m_sign > 0) ? this.m_magnitude : this.Add(NetBigInteger.One).m_magnitude;
			int[] array2 = (value.m_sign > 0) ? value.m_magnitude : value.Add(NetBigInteger.One).m_magnitude;
			bool flag = this.m_sign < 0 && value.m_sign < 0;
			int num = Math.Max(array.Length, array2.Length);
			int[] array3 = new int[num];
			int num2 = array3.Length - array.Length;
			int num3 = array3.Length - array2.Length;
			for (int i = 0; i < array3.Length; i++)
			{
				int num4 = (i >= num2) ? array[i - num2] : 0;
				int num5 = (i >= num3) ? array2[i - num3] : 0;
				if (this.m_sign < 0)
				{
					num4 = ~num4;
				}
				if (value.m_sign < 0)
				{
					num5 = ~num5;
				}
				array3[i] = (num4 & num5);
				if (flag)
				{
					array3[i] = ~array3[i];
				}
			}
			NetBigInteger netBigInteger = new NetBigInteger(1, array3, true);
			if (flag)
			{
				netBigInteger = netBigInteger.Not();
			}
			return netBigInteger;
		}

		private int calcBitLength(int indx, int[] mag)
		{
			while (indx < mag.Length)
			{
				if (mag[indx] != 0)
				{
					int num = 32 * (mag.Length - indx - 1);
					int num2 = mag[indx];
					num += NetBigInteger.BitLen(num2);
					if (this.m_sign < 0 && (num2 & -num2) == num2)
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
				if (this.m_numBitLength == -1)
				{
					this.m_numBitLength = ((this.m_sign == 0) ? 0 : this.calcBitLength(0, this.m_magnitude));
				}
				return this.m_numBitLength;
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
			return this.m_sign > 0 && this.m_numBits == 1;
		}

		public int CompareTo(object obj)
		{
			return this.CompareTo((NetBigInteger)obj);
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
			return NetBigInteger.CompareNoLeadingZeroes(xIndx, x, yIndx, y);
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

		public int CompareTo(NetBigInteger value)
		{
			if (this.m_sign < value.m_sign)
			{
				return -1;
			}
			if (this.m_sign > value.m_sign)
			{
				return 1;
			}
			if (this.m_sign != 0)
			{
				return this.m_sign * NetBigInteger.CompareNoLeadingZeroes(0, this.m_magnitude, 0, value.m_magnitude);
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
			int num3 = NetBigInteger.CompareNoLeadingZeroes(num, x, num2, y);
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
					array2 = NetBigInteger.ShiftLeft(y, num6);
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
					if (num9 < num5 || NetBigInteger.CompareNoLeadingZeroes(num, x, num8, array2) >= 0)
					{
						NetBigInteger.Subtract(num, x, num8, array2);
						NetBigInteger.AddMagnitudes(array3, array);
						while (x[num] == 0)
						{
							if (++num == x.Length)
							{
								return array3;
							}
						}
						num5 = 32 * (x.Length - num - 1) + NetBigInteger.BitLen(x[num]);
						if (num5 <= num4)
						{
							if (num5 < num4)
							{
								return array3;
							}
							num3 = NetBigInteger.CompareNoLeadingZeroes(num, x, num2, y);
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
						array2 = NetBigInteger.ShiftRightOneInPlace(num8, array2);
						num9--;
						array = NetBigInteger.ShiftRightOneInPlace(num7, array);
					}
					else
					{
						array2 = NetBigInteger.ShiftRightInPlace(num8, array2, num6);
						num9 -= num6;
						array = NetBigInteger.ShiftRightInPlace(num7, array, num6);
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
				NetBigInteger.AddMagnitudes(array3, NetBigInteger.One.m_magnitude);
				Array.Clear(x, num, x.Length - num);
			}
			return array3;
		}

		public NetBigInteger Divide(NetBigInteger val)
		{
			if (val.m_sign == 0)
			{
				throw new ArithmeticException("Division by zero error");
			}
			if (this.m_sign == 0)
			{
				return NetBigInteger.Zero;
			}
			if (!val.QuickPow2Check())
			{
				int[] x = (int[])this.m_magnitude.Clone();
				return new NetBigInteger(this.m_sign * val.m_sign, this.Divide(x, val.m_magnitude), true);
			}
			NetBigInteger netBigInteger = this.Abs().ShiftRight(val.Abs().BitLength - 1);
			if (val.m_sign != this.m_sign)
			{
				return netBigInteger.Negate();
			}
			return netBigInteger;
		}

		public NetBigInteger[] DivideAndRemainder(NetBigInteger val)
		{
			if (val.m_sign == 0)
			{
				throw new ArithmeticException("Division by zero error");
			}
			NetBigInteger[] array = new NetBigInteger[2];
			if (this.m_sign == 0)
			{
				array[0] = NetBigInteger.Zero;
				array[1] = NetBigInteger.Zero;
			}
			else if (val.QuickPow2Check())
			{
				int n = val.Abs().BitLength - 1;
				NetBigInteger netBigInteger = this.Abs().ShiftRight(n);
				int[] mag = this.LastNBits(n);
				array[0] = ((val.m_sign == this.m_sign) ? netBigInteger : netBigInteger.Negate());
				array[1] = new NetBigInteger(this.m_sign, mag, true);
			}
			else
			{
				int[] array2 = (int[])this.m_magnitude.Clone();
				int[] mag2 = this.Divide(array2, val.m_magnitude);
				array[0] = new NetBigInteger(this.m_sign * val.m_sign, mag2, true);
				array[1] = new NetBigInteger(this.m_sign, array2, true);
			}
			return array;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			NetBigInteger netBigInteger = obj as NetBigInteger;
			if (netBigInteger == null)
			{
				return false;
			}
			if (netBigInteger.m_sign != this.m_sign || netBigInteger.m_magnitude.Length != this.m_magnitude.Length)
			{
				return false;
			}
			for (int i = 0; i < this.m_magnitude.Length; i++)
			{
				if (netBigInteger.m_magnitude[i] != this.m_magnitude[i])
				{
					return false;
				}
			}
			return true;
		}

		public NetBigInteger Gcd(NetBigInteger value)
		{
			if (value.m_sign == 0)
			{
				return this.Abs();
			}
			if (this.m_sign == 0)
			{
				return value.Abs();
			}
			NetBigInteger netBigInteger = this;
			NetBigInteger netBigInteger2 = value;
			while (netBigInteger2.m_sign != 0)
			{
				NetBigInteger netBigInteger3 = netBigInteger.Mod(netBigInteger2);
				netBigInteger = netBigInteger2;
				netBigInteger2 = netBigInteger3;
			}
			return netBigInteger;
		}

		public override int GetHashCode()
		{
			int num = this.m_magnitude.Length;
			if (this.m_magnitude.Length > 0)
			{
				num ^= this.m_magnitude[0];
				if (this.m_magnitude.Length > 1)
				{
					num ^= this.m_magnitude[this.m_magnitude.Length - 1];
				}
			}
			if (this.m_sign >= 0)
			{
				return num;
			}
			return ~num;
		}

		private NetBigInteger Inc()
		{
			if (this.m_sign == 0)
			{
				return NetBigInteger.One;
			}
			if (this.m_sign < 0)
			{
				return new NetBigInteger(-1, NetBigInteger.doSubBigLil(this.m_magnitude, NetBigInteger.One.m_magnitude), true);
			}
			return this.AddToMagnitude(NetBigInteger.One.m_magnitude);
		}

		public int IntValue
		{
			get
			{
				if (this.m_sign == 0)
				{
					return 0;
				}
				if (this.m_sign <= 0)
				{
					return -this.m_magnitude[this.m_magnitude.Length - 1];
				}
				return this.m_magnitude[this.m_magnitude.Length - 1];
			}
		}

		public NetBigInteger Max(NetBigInteger value)
		{
			if (this.CompareTo(value) <= 0)
			{
				return value;
			}
			return this;
		}

		public NetBigInteger Min(NetBigInteger value)
		{
			if (this.CompareTo(value) >= 0)
			{
				return value;
			}
			return this;
		}

		public NetBigInteger Mod(NetBigInteger m)
		{
			if (m.m_sign < 1)
			{
				throw new ArithmeticException("Modulus must be positive");
			}
			NetBigInteger netBigInteger = this.Remainder(m);
			if (netBigInteger.m_sign < 0)
			{
				return netBigInteger.Add(m);
			}
			return netBigInteger;
		}

		public NetBigInteger ModInverse(NetBigInteger m)
		{
			if (m.m_sign < 1)
			{
				throw new ArithmeticException("Modulus must be positive");
			}
			NetBigInteger netBigInteger = new NetBigInteger();
			NetBigInteger netBigInteger2 = NetBigInteger.ExtEuclid(this, m, netBigInteger, null);
			if (!netBigInteger2.Equals(NetBigInteger.One))
			{
				throw new ArithmeticException("Numbers not relatively prime.");
			}
			if (netBigInteger.m_sign < 0)
			{
				netBigInteger.m_sign = 1;
				netBigInteger.m_magnitude = NetBigInteger.doSubBigLil(m.m_magnitude, netBigInteger.m_magnitude);
			}
			return netBigInteger;
		}

		private static NetBigInteger ExtEuclid(NetBigInteger a, NetBigInteger b, NetBigInteger u1Out, NetBigInteger u2Out)
		{
			NetBigInteger netBigInteger = NetBigInteger.One;
			NetBigInteger netBigInteger2 = a;
			NetBigInteger netBigInteger3 = NetBigInteger.Zero;
			NetBigInteger netBigInteger4 = b;
			while (netBigInteger4.m_sign > 0)
			{
				NetBigInteger[] array = netBigInteger2.DivideAndRemainder(netBigInteger4);
				NetBigInteger n = netBigInteger3.Multiply(array[0]);
				NetBigInteger netBigInteger5 = netBigInteger.Subtract(n);
				netBigInteger = netBigInteger3;
				netBigInteger3 = netBigInteger5;
				netBigInteger2 = netBigInteger4;
				netBigInteger4 = array[1];
			}
			if (u1Out != null)
			{
				u1Out.m_sign = netBigInteger.m_sign;
				u1Out.m_magnitude = netBigInteger.m_magnitude;
			}
			if (u2Out != null)
			{
				NetBigInteger netBigInteger6 = netBigInteger.Multiply(a);
				netBigInteger6 = netBigInteger2.Subtract(netBigInteger6);
				NetBigInteger netBigInteger7 = netBigInteger6.Divide(b);
				u2Out.m_sign = netBigInteger7.m_sign;
				u2Out.m_magnitude = netBigInteger7.m_magnitude;
			}
			return netBigInteger2;
		}

		private static void ZeroOut(int[] x)
		{
			Array.Clear(x, 0, x.Length);
		}

		public NetBigInteger ModPow(NetBigInteger exponent, NetBigInteger m)
		{
			if (m.m_sign < 1)
			{
				throw new ArithmeticException("Modulus must be positive");
			}
			if (m.Equals(NetBigInteger.One))
			{
				return NetBigInteger.Zero;
			}
			if (exponent.m_sign == 0)
			{
				return NetBigInteger.One;
			}
			if (this.m_sign == 0)
			{
				return NetBigInteger.Zero;
			}
			int[] array = null;
			int[] array2 = null;
			bool flag = (m.m_magnitude[m.m_magnitude.Length - 1] & 1) == 1;
			long mQuote = 0L;
			if (flag)
			{
				mQuote = m.GetMQuote();
				NetBigInteger netBigInteger = this.ShiftLeft(32 * m.m_magnitude.Length).Mod(m);
				array = netBigInteger.m_magnitude;
				flag = (array.Length <= m.m_magnitude.Length);
				if (flag)
				{
					array2 = new int[m.m_magnitude.Length + 1];
					if (array.Length < m.m_magnitude.Length)
					{
						int[] array3 = new int[m.m_magnitude.Length];
						array.CopyTo(array3, array3.Length - array.Length);
						array = array3;
					}
				}
			}
			if (!flag)
			{
				if (this.m_magnitude.Length <= m.m_magnitude.Length)
				{
					array = new int[m.m_magnitude.Length];
					this.m_magnitude.CopyTo(array, array.Length - this.m_magnitude.Length);
				}
				else
				{
					NetBigInteger netBigInteger2 = this.Remainder(m);
					array = new int[m.m_magnitude.Length];
					netBigInteger2.m_magnitude.CopyTo(array, array.Length - netBigInteger2.m_magnitude.Length);
				}
				array2 = new int[m.m_magnitude.Length * 2];
			}
			int[] array4 = new int[m.m_magnitude.Length];
			for (int i = 0; i < exponent.m_magnitude.Length; i++)
			{
				int j = exponent.m_magnitude[i];
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
						NetBigInteger.MultiplyMonty(array2, array4, array4, m.m_magnitude, mQuote);
					}
					else
					{
						NetBigInteger.Square(array2, array4);
						this.Remainder(array2, m.m_magnitude);
						Array.Copy(array2, array2.Length - array4.Length, array4, 0, array4.Length);
						NetBigInteger.ZeroOut(array2);
					}
					k++;
					if (j < 0)
					{
						if (flag)
						{
							NetBigInteger.MultiplyMonty(array2, array4, array, m.m_magnitude, mQuote);
						}
						else
						{
							NetBigInteger.Multiply(array2, array4, array);
							this.Remainder(array2, m.m_magnitude);
							Array.Copy(array2, array2.Length - array4.Length, array4, 0, array4.Length);
							NetBigInteger.ZeroOut(array2);
						}
					}
					j <<= 1;
				}
				while (k < 32)
				{
					if (flag)
					{
						NetBigInteger.MultiplyMonty(array2, array4, array4, m.m_magnitude, mQuote);
					}
					else
					{
						NetBigInteger.Square(array2, array4);
						this.Remainder(array2, m.m_magnitude);
						Array.Copy(array2, array2.Length - array4.Length, array4, 0, array4.Length);
						NetBigInteger.ZeroOut(array2);
					}
					k++;
				}
			}
			if (flag)
			{
				NetBigInteger.ZeroOut(array);
				array[array.Length - 1] = 1;
				NetBigInteger.MultiplyMonty(array2, array4, array, m.m_magnitude, mQuote);
			}
			NetBigInteger netBigInteger3 = new NetBigInteger(1, array4, true);
			if (exponent.m_sign <= 0)
			{
				return netBigInteger3.ModInverse(m);
			}
			return netBigInteger3;
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
			long num = NetBigInteger.FastExtEuclid(v, m, array);
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
			if (this.m_quote != -1L)
			{
				return this.m_quote;
			}
			if (this.m_magnitude.Length == 0 || (this.m_magnitude[this.m_magnitude.Length - 1] & 1) == 0)
			{
				return -1L;
			}
			long v = (long)(~this.m_magnitude[this.m_magnitude.Length - 1] | 1) & (long)((ulong)-1);
			this.m_quote = NetBigInteger.FastModInverse(v, 4294967296L);
			return this.m_quote;
		}

		private static void MultiplyMonty(int[] a, int[] x, int[] y, int[] m, long mQuote)
		{
			if (m.Length == 1)
			{
				x[0] = (int)NetBigInteger.MultiplyMontyNIsOne((uint)x[0], (uint)y[0], (uint)m[0], (ulong)mQuote);
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
			if (NetBigInteger.CompareTo(0, a, 0, m) >= 0)
			{
				NetBigInteger.Subtract(0, a, 0, m);
			}
			Array.Copy(a, 1, x, 0, num);
		}

		private static uint MultiplyMontyNIsOne(uint x, uint y, uint m, ulong mQuote)
		{
			ulong num = (ulong)m;
			ulong num2 = (ulong)x * (ulong)y;
			ulong num3 = num2 * mQuote & NetBigInteger.UIMASK;
			ulong num4 = num3 * num;
			ulong num5 = (num2 & NetBigInteger.UIMASK) + (num4 & NetBigInteger.UIMASK);
			ulong num6 = (num2 >> 32) + (num4 >> 32) + (num5 >> 32);
			if (num6 > num)
			{
				num6 -= num;
			}
			return (uint)(num6 & NetBigInteger.UIMASK);
		}

		public NetBigInteger Modulus(NetBigInteger val)
		{
			return this.Mod(val);
		}

		public NetBigInteger Multiply(NetBigInteger val)
		{
			if (this.m_sign == 0 || val.m_sign == 0)
			{
				return NetBigInteger.Zero;
			}
			if (val.QuickPow2Check())
			{
				NetBigInteger netBigInteger = this.ShiftLeft(val.Abs().BitLength - 1);
				if (val.m_sign <= 0)
				{
					return netBigInteger.Negate();
				}
				return netBigInteger;
			}
			else
			{
				if (!this.QuickPow2Check())
				{
					int num = this.BitLength + val.BitLength;
					int num2 = (num + 32 - 1) / 32;
					int[] array = new int[num2];
					if (val == this)
					{
						NetBigInteger.Square(array, this.m_magnitude);
					}
					else
					{
						NetBigInteger.Multiply(array, this.m_magnitude, val.m_magnitude);
					}
					return new NetBigInteger(this.m_sign * val.m_sign, array, true);
				}
				NetBigInteger netBigInteger2 = val.ShiftLeft(this.Abs().BitLength - 1);
				if (this.m_sign <= 0)
				{
					return netBigInteger2.Negate();
				}
				return netBigInteger2;
			}
		}

		public NetBigInteger Negate()
		{
			if (this.m_sign == 0)
			{
				return this;
			}
			return new NetBigInteger(-this.m_sign, this.m_magnitude, false);
		}

		public NetBigInteger Not()
		{
			return this.Inc().Negate();
		}

		public NetBigInteger Pow(int exp)
		{
			if (exp < 0)
			{
				throw new ArithmeticException("Negative exponent");
			}
			if (exp == 0)
			{
				return NetBigInteger.One;
			}
			if (this.m_sign == 0 || this.Equals(NetBigInteger.One))
			{
				return this;
			}
			NetBigInteger netBigInteger = NetBigInteger.One;
			NetBigInteger netBigInteger2 = this;
			for (;;)
			{
				if ((exp & 1) == 1)
				{
					netBigInteger = netBigInteger.Multiply(netBigInteger2);
				}
				exp >>= 1;
				if (exp == 0)
				{
					break;
				}
				netBigInteger2 = netBigInteger2.Multiply(netBigInteger2);
			}
			return netBigInteger;
		}

		private int Remainder(int m)
		{
			long num = 0L;
			for (int i = 0; i < this.m_magnitude.Length; i++)
			{
				long num2 = (long)((ulong)this.m_magnitude[i]);
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
			int num3 = NetBigInteger.CompareNoLeadingZeroes(num, x, num2, y);
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
					array = NetBigInteger.ShiftLeft(y, num6);
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
					if (num8 < num5 || NetBigInteger.CompareNoLeadingZeroes(num, x, num7, array) >= 0)
					{
						NetBigInteger.Subtract(num, x, num7, array);
						while (x[num] == 0)
						{
							if (++num == x.Length)
							{
								return x;
							}
						}
						num5 = 32 * (x.Length - num - 1) + NetBigInteger.BitLen(x[num]);
						if (num5 <= num4)
						{
							if (num5 < num4)
							{
								return x;
							}
							num3 = NetBigInteger.CompareNoLeadingZeroes(num, x, num2, y);
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
						array = NetBigInteger.ShiftRightOneInPlace(num7, array);
						num8--;
					}
					else
					{
						array = NetBigInteger.ShiftRightInPlace(num7, array, num6);
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

		public NetBigInteger Remainder(NetBigInteger n)
		{
			if (n.m_sign == 0)
			{
				throw new ArithmeticException("Division by zero error");
			}
			if (this.m_sign == 0)
			{
				return NetBigInteger.Zero;
			}
			if (n.m_magnitude.Length == 1)
			{
				int num = n.m_magnitude[0];
				if (num > 0)
				{
					if (num == 1)
					{
						return NetBigInteger.Zero;
					}
					int num2 = this.Remainder(num);
					if (num2 != 0)
					{
						return new NetBigInteger(this.m_sign, new int[]
						{
							num2
						}, false);
					}
					return NetBigInteger.Zero;
				}
			}
			if (NetBigInteger.CompareNoLeadingZeroes(0, this.m_magnitude, 0, n.m_magnitude) < 0)
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
				array = (int[])this.m_magnitude.Clone();
				array = this.Remainder(array, n.m_magnitude);
			}
			return new NetBigInteger(this.m_sign, array, true);
		}

		private int[] LastNBits(int n)
		{
			if (n < 1)
			{
				return NetBigInteger.ZeroMagnitude;
			}
			int num = (n + 32 - 1) / 32;
			num = Math.Min(num, this.m_magnitude.Length);
			int[] array = new int[num];
			Array.Copy(this.m_magnitude, this.m_magnitude.Length - num, array, 0, num);
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

		public NetBigInteger ShiftLeft(int n)
		{
			if (this.m_sign == 0 || this.m_magnitude.Length == 0)
			{
				return NetBigInteger.Zero;
			}
			if (n == 0)
			{
				return this;
			}
			if (n < 0)
			{
				return this.ShiftRight(-n);
			}
			NetBigInteger netBigInteger = new NetBigInteger(this.m_sign, NetBigInteger.ShiftLeft(this.m_magnitude, n), true);
			if (this.m_numBits != -1)
			{
				netBigInteger.m_numBits = ((this.m_sign > 0) ? this.m_numBits : (this.m_numBits + n));
			}
			if (this.m_numBitLength != -1)
			{
				netBigInteger.m_numBitLength = this.m_numBitLength + n;
			}
			return netBigInteger;
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

		public NetBigInteger ShiftRight(int n)
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
					Array.Copy(this.m_magnitude, 0, array, 0, array.Length);
				}
				else
				{
					int num4 = 32 - num3;
					int num5 = this.m_magnitude.Length - 1 - num2;
					for (int i = num - 1; i >= 0; i--)
					{
						array[i] = (int)((uint)this.m_magnitude[num5--] >> (num3 & 31));
						if (num5 >= 0)
						{
							array[i] |= this.m_magnitude[num5] << num4;
						}
					}
				}
				return new NetBigInteger(this.m_sign, array, false);
			}
			if (this.m_sign >= 0)
			{
				return NetBigInteger.Zero;
			}
			return NetBigInteger.One.Negate();
		}

		public int SignValue
		{
			get
			{
				return this.m_sign;
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

		public NetBigInteger Subtract(NetBigInteger n)
		{
			if (n.m_sign == 0)
			{
				return this;
			}
			if (this.m_sign == 0)
			{
				return n.Negate();
			}
			if (this.m_sign != n.m_sign)
			{
				return this.Add(n.Negate());
			}
			int num = NetBigInteger.CompareNoLeadingZeroes(0, this.m_magnitude, 0, n.m_magnitude);
			if (num == 0)
			{
				return NetBigInteger.Zero;
			}
			NetBigInteger netBigInteger;
			NetBigInteger netBigInteger2;
			if (num < 0)
			{
				netBigInteger = n;
				netBigInteger2 = this;
			}
			else
			{
				netBigInteger = this;
				netBigInteger2 = n;
			}
			return new NetBigInteger(this.m_sign * num, NetBigInteger.doSubBigLil(netBigInteger.m_magnitude, netBigInteger2.m_magnitude), true);
		}

		private static int[] doSubBigLil(int[] bigMag, int[] lilMag)
		{
			int[] x = (int[])bigMag.Clone();
			return NetBigInteger.Subtract(0, x, 0, lilMag);
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
			if (this.m_sign != 0)
			{
				int nBits = (unsigned && this.m_sign > 0) ? this.BitLength : (this.BitLength + 1);
				int byteLength = NetBigInteger.GetByteLength(nBits);
				byte[] array = new byte[byteLength];
				int i = this.m_magnitude.Length;
				int num = array.Length;
				if (this.m_sign > 0)
				{
					while (i > 1)
					{
						uint num2 = (uint)this.m_magnitude[--i];
						array[--num] = (byte)num2;
						array[--num] = (byte)(num2 >> 8);
						array[--num] = (byte)(num2 >> 16);
						array[--num] = (byte)(num2 >> 24);
					}
					uint num3;
					for (num3 = (uint)this.m_magnitude[0]; num3 > 255U; num3 >>= 8)
					{
						array[--num] = (byte)num3;
					}
					array[num - 1] = (byte)num3;
				}
				else
				{
					bool flag = true;
					while (i > 1)
					{
						uint num4 = (uint)(~(uint)this.m_magnitude[--i]);
						if (flag)
						{
							flag = ((num4 += 1U) == 0U);
						}
						array[--num] = (byte)num4;
						array[--num] = (byte)(num4 >> 8);
						array[--num] = (byte)(num4 >> 16);
						array[--num] = (byte)(num4 >> 24);
					}
					uint num5 = (uint)this.m_magnitude[0];
					if (flag)
					{
						num5 -= 1U;
					}
					while (num5 > 255U)
					{
						array[--num] = (byte)(~(byte)num5);
						num5 >>= 8;
					}
					array[--num] = (byte)(~(byte)num5);
					if (num > 0)
					{
						array[num - 1] = byte.MaxValue;
					}
				}
				return array;
			}
			if (!unsigned)
			{
				return new byte[1];
			}
			return NetBigInteger.ZeroEncoding;
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
			if (this.m_magnitude == null)
			{
				return "null";
			}
			if (this.m_sign == 0)
			{
				return "0";
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (radix == 16)
			{
				stringBuilder.Append(this.m_magnitude[0].ToString("x"));
				for (int i = 1; i < this.m_magnitude.Length; i++)
				{
					stringBuilder.Append(this.m_magnitude[i].ToString("x8"));
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
				Stack stack = new Stack();
				NetBigInteger netBigInteger = NetBigInteger.ValueOf((long)radix);
				NetBigInteger netBigInteger2 = this.Abs();
				while (netBigInteger2.m_sign != 0)
				{
					NetBigInteger netBigInteger3 = netBigInteger2.Mod(netBigInteger);
					if (netBigInteger3.m_sign == 0)
					{
						stack.Push("0");
					}
					else
					{
						stack.Push(netBigInteger3.m_magnitude[0].ToString("d"));
					}
					netBigInteger2 = netBigInteger2.Divide(netBigInteger);
				}
				while (stack.Count != 0)
				{
					stringBuilder.Append((string)stack.Pop());
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
			if (this.m_sign == -1)
			{
				text = "-" + text;
			}
			return text;
		}

		private static NetBigInteger createUValueOf(ulong value)
		{
			int num = (int)(value >> 32);
			int num2 = (int)value;
			if (num != 0)
			{
				return new NetBigInteger(1, new int[]
				{
					num,
					num2
				}, false);
			}
			if (num2 != 0)
			{
				NetBigInteger netBigInteger = new NetBigInteger(1, new int[]
				{
					num2
				}, false);
				if ((num2 & -num2) == num2)
				{
					netBigInteger.m_numBits = 1;
				}
				return netBigInteger;
			}
			return NetBigInteger.Zero;
		}

		private static NetBigInteger createValueOf(long value)
		{
			if (value >= 0L)
			{
				return NetBigInteger.createUValueOf((ulong)value);
			}
			if (value == -9223372036854775808L)
			{
				return NetBigInteger.createValueOf(~value).Not();
			}
			return NetBigInteger.createValueOf(-value).Negate();
		}

		public static NetBigInteger ValueOf(long value)
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
					return NetBigInteger.Zero;
				case 1:
					return NetBigInteger.One;
				case 2:
					return NetBigInteger.Two;
				case 3:
					return NetBigInteger.Three;
				}
			}
			if (value == 10L)
			{
				return NetBigInteger.Ten;
			}
			IL_49:
			return NetBigInteger.createValueOf(value);
		}

		public int GetLowestSetBit()
		{
			if (this.m_sign == 0)
			{
				return -1;
			}
			int num = this.m_magnitude.Length;
			while (--num > 0 && this.m_magnitude[num] == 0)
			{
			}
			int num2 = this.m_magnitude[num];
			int num3 = ((num2 & 65535) == 0) ? (((num2 & 16711680) == 0) ? 7 : 15) : (((num2 & 255) == 0) ? 23 : 31);
			while (num3 > 0 && num2 << num3 != -2147483648)
			{
				num3--;
			}
			return (this.m_magnitude.Length - num) * 32 - (num3 + 1);
		}

		public bool TestBit(int n)
		{
			if (n < 0)
			{
				throw new ArithmeticException("Bit position must not be negative");
			}
			if (this.m_sign < 0)
			{
				return !this.Not().TestBit(n);
			}
			int num = n / 32;
			if (num >= this.m_magnitude.Length)
			{
				return false;
			}
			int num2 = this.m_magnitude[this.m_magnitude.Length - 1 - num];
			return (num2 >> n % 32 & 1) > 0;
		}
	}
}
