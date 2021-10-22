using System;
using System.Threading;

namespace DNA.Net.Lidgren
{
	public class NetRandom
	{
		private const double REAL_UNIT_INT = 4.6566128730773926E-10;

		private const double REAL_UNIT_UINT = 2.3283064365386963E-10;

		private const uint Y = 842502087U;

		private const uint Z = 3579807591U;

		private const uint W = 273326509U;

		public static readonly NetRandom Instance = new NetRandom();

		private static int s_extraSeed = 42;

		private uint x;

		private uint y;

		private uint z;

		private uint w;

		private uint bitBuffer;

		private uint bitMask = 1U;

		public NetRandom()
		{
			this.Reinitialise(this.GetSeed(this));
		}

		public NetRandom(int seed)
		{
			this.Reinitialise(seed);
		}

		public int GetSeed(object forObject)
		{
			int num = Environment.TickCount;
			num ^= forObject.GetHashCode();
			int num2 = Interlocked.Increment(ref NetRandom.s_extraSeed);
			return num + num2;
		}

		public void Reinitialise(int seed)
		{
			this.x = (uint)seed;
			this.y = 842502087U;
			this.z = 3579807591U;
			this.w = 273326509U;
		}

		public int Next()
		{
			uint num = this.x ^ this.x << 11;
			this.x = this.y;
			this.y = this.z;
			this.z = this.w;
			this.w = (this.w ^ this.w >> 19 ^ (num ^ num >> 8));
			uint num2 = this.w & 2147483647U;
			if (num2 == 2147483647U)
			{
				return this.Next();
			}
			return (int)num2;
		}

		public int Next(int upperBound)
		{
			if (upperBound < 0)
			{
				throw new ArgumentOutOfRangeException("upperBound", upperBound, "upperBound must be >=0");
			}
			uint num = this.x ^ this.x << 11;
			this.x = this.y;
			this.y = this.z;
			this.z = this.w;
			return (int)(4.6566128730773926E-10 * (double)(2147483647U & (this.w = (this.w ^ this.w >> 19 ^ (num ^ num >> 8)))) * (double)upperBound);
		}

		public int Next(int lowerBound, int upperBound)
		{
			if (lowerBound > upperBound)
			{
				throw new ArgumentOutOfRangeException("upperBound", upperBound, "upperBound must be >=lowerBound");
			}
			uint num = this.x ^ this.x << 11;
			this.x = this.y;
			this.y = this.z;
			this.z = this.w;
			int num2 = upperBound - lowerBound;
			if (num2 < 0)
			{
				return lowerBound + (int)(2.3283064365386963E-10 * (this.w = (this.w ^ this.w >> 19 ^ (num ^ num >> 8))) * (double)((long)upperBound - (long)lowerBound));
			}
			return lowerBound + (int)(4.6566128730773926E-10 * (double)(2147483647U & (this.w = (this.w ^ this.w >> 19 ^ (num ^ num >> 8)))) * (double)num2);
		}

		public double NextDouble()
		{
			uint num = this.x ^ this.x << 11;
			this.x = this.y;
			this.y = this.z;
			this.z = this.w;
			return 4.6566128730773926E-10 * (double)(2147483647U & (this.w = (this.w ^ this.w >> 19 ^ (num ^ num >> 8))));
		}

		public float NextSingle()
		{
			return (float)this.NextDouble();
		}

		public void NextBytes(byte[] buffer)
		{
			uint num = this.x;
			uint num2 = this.y;
			uint num3 = this.z;
			uint num4 = this.w;
			int i = 0;
			int num5 = buffer.Length - 3;
			while (i < num5)
			{
				uint num6 = num ^ num << 11;
				num = num2;
				num2 = num3;
				num3 = num4;
				num4 = (num4 ^ num4 >> 19 ^ (num6 ^ num6 >> 8));
				buffer[i++] = (byte)num4;
				buffer[i++] = (byte)(num4 >> 8);
				buffer[i++] = (byte)(num4 >> 16);
				buffer[i++] = (byte)(num4 >> 24);
			}
			if (i < buffer.Length)
			{
				uint num6 = num ^ num << 11;
				num = num2;
				num2 = num3;
				num3 = num4;
				num4 = (num4 ^ num4 >> 19 ^ (num6 ^ num6 >> 8));
				buffer[i++] = (byte)num4;
				if (i < buffer.Length)
				{
					buffer[i++] = (byte)(num4 >> 8);
					if (i < buffer.Length)
					{
						buffer[i++] = (byte)(num4 >> 16);
						if (i < buffer.Length)
						{
							buffer[i] = (byte)(num4 >> 24);
						}
					}
				}
			}
			this.x = num;
			this.y = num2;
			this.z = num3;
			this.w = num4;
		}

		[CLSCompliant(false)]
		public uint NextUInt()
		{
			uint num = this.x ^ this.x << 11;
			this.x = this.y;
			this.y = this.z;
			this.z = this.w;
			return this.w = (this.w ^ this.w >> 19 ^ (num ^ num >> 8));
		}

		public int NextInt()
		{
			uint num = this.x ^ this.x << 11;
			this.x = this.y;
			this.y = this.z;
			this.z = this.w;
			return (int)(2147483647U & (this.w = (this.w ^ this.w >> 19 ^ (num ^ num >> 8))));
		}

		public bool NextBool()
		{
			if (this.bitMask == 1U)
			{
				uint num = this.x ^ this.x << 11;
				this.x = this.y;
				this.y = this.z;
				this.z = this.w;
				this.bitBuffer = (this.w = (this.w ^ this.w >> 19 ^ (num ^ num >> 8)));
				this.bitMask = 2147483648U;
				return (this.bitBuffer & this.bitMask) == 0U;
			}
			return (this.bitBuffer & (this.bitMask >>= 1)) == 0U;
		}
	}
}
