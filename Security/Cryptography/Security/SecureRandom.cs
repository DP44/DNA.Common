using System;
using System.Globalization;
using DNA.Security.Cryptography.Crypto;
using DNA.Security.Cryptography.Crypto.Digests;
using DNA.Security.Cryptography.Crypto.Prng;

namespace DNA.Security.Cryptography.Security
{
	public class SecureRandom : Random
	{
		private static readonly SecureRandom[] master;

		protected IRandomGenerator generator;

		private static readonly double DoubleScale;

		private static SecureRandom Master
		{
			get
			{
				if (SecureRandom.master[0] == null)
				{
					IRandomGenerator randomGenerator = new DigestRandomGenerator(new Sha256Digest());
					randomGenerator = new ReversedWindowGenerator(randomGenerator, 32);
					SecureRandom secureRandom = SecureRandom.master[0] = new SecureRandom(randomGenerator);
					secureRandom.SetSeed(DateTime.Now.Ticks);
					secureRandom.SetSeed(new ThreadedSeedGenerator().GenerateSeed(24, true));
					secureRandom.GenerateSeed(1 + secureRandom.Next(32));
				}
				return SecureRandom.master[0];
			}
		}

		public static SecureRandom GetInstance(string algorithm)
		{
			IDigest digest = null;
			string a;
			if ((a = algorithm.ToUpper(CultureInfo.InvariantCulture)) != null)
			{
				if (!(a == "SHA1PRNG"))
				{
					if (a == "SHA256PRNG")
					{
						digest = new Sha256Digest();
					}
				}
				else
				{
					digest = new Sha1Digest();
				}
			}
			if (digest != null)
			{
				return new SecureRandom(new DigestRandomGenerator(digest));
			}
			throw new ArgumentException("Unrecognised PRNG algorithm: " + algorithm, "algorithm");
		}

		public static byte[] GetSeed(int length)
		{
			return SecureRandom.Master.GenerateSeed(length);
		}

		public SecureRandom() : base(0)
		{
			this.generator = new DigestRandomGenerator(new Sha1Digest());
			this.SetSeed(SecureRandom.GetSeed(8));
		}

		public SecureRandom(byte[] inSeed) : base(0)
		{
			this.generator = new DigestRandomGenerator(new Sha1Digest());
			this.SetSeed(inSeed);
		}

		public SecureRandom(IRandomGenerator generator) : base(0)
		{
			this.generator = generator;
		}

		public virtual byte[] GenerateSeed(int length)
		{
			this.SetSeed(DateTime.Now.Ticks);
			byte[] array = new byte[length];
			this.NextBytes(array);
			return array;
		}

		public virtual void SetSeed(byte[] inSeed)
		{
			this.generator.AddSeedMaterial(inSeed);
		}

		public virtual void SetSeed(long seed)
		{
			this.generator.AddSeedMaterial(seed);
		}

		public override int Next()
		{
			int num;
			do
			{
				num = (this.NextInt() & int.MaxValue);
			}
			while (num == 2147483647);
			return num;
		}

		public override int Next(int maxValue)
		{
			if (maxValue < 2)
			{
				if (maxValue < 0)
				{
					throw new ArgumentOutOfRangeException("maxValue < 0");
				}
				return 0;
			}
			else
			{
				if ((maxValue & -maxValue) == maxValue)
				{
					int num = this.NextInt() & int.MaxValue;
					long num2 = (long)maxValue * (long)num >> 31;
					return (int)num2;
				}
				int num3;
				int num4;
				do
				{
					num3 = (this.NextInt() & int.MaxValue);
					num4 = num3 % maxValue;
				}
				while (num3 - num4 + (maxValue - 1) < 0);
				return num4;
			}
		}

		public override int Next(int minValue, int maxValue)
		{
			if (maxValue <= minValue)
			{
				if (maxValue == minValue)
				{
					return minValue;
				}
				throw new ArgumentException("maxValue cannot be less than minValue");
			}
			else
			{
				int num = maxValue - minValue;
				if (num > 0)
				{
					return minValue + this.Next(num);
				}
				int num2;
				do
				{
					num2 = this.NextInt();
				}
				while (num2 < minValue || num2 >= maxValue);
				return num2;
			}
		}

		public override void NextBytes(byte[] buffer)
		{
			this.generator.NextBytes(buffer);
		}

		public virtual void NextBytes(byte[] buffer, int start, int length)
		{
			this.generator.NextBytes(buffer, start, length);
		}

		public override double NextDouble()
		{
			return Convert.ToDouble((ulong)this.NextLong()) / SecureRandom.DoubleScale;
		}

		public virtual int NextInt()
		{
			byte[] array = new byte[4];
			this.NextBytes(array);
			int num = 0;
			for (int i = 0; i < 4; i++)
			{
				num = (num << 8) + (int)(array[i] & byte.MaxValue);
			}
			return num;
		}

		public virtual long NextLong()
		{
			return (long)((ulong)this.NextInt() << 32 | (ulong)this.NextInt());
		}

		// Note: this type is marked as 'beforefieldinit'.
		static SecureRandom()
		{
			SecureRandom[] array = new SecureRandom[1];
			SecureRandom.master = array;
			SecureRandom.DoubleScale = Math.Pow(2.0, 64.0);
		}
	}
}
