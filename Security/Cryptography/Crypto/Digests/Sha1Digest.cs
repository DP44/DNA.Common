using System;

namespace DNA.Security.Cryptography.Crypto.Digests
{
	public class Sha1Digest : GeneralDigest
	{
		private const int DigestLength = 20;

		private const int Y1 = 1518500249;

		private const int Y2 = 1859775393;

		private const int Y3 = -1894007588;

		private const int Y4 = -899497514;

		private int H1;

		private int H2;

		private int H3;

		private int H4;

		private int H5;

		private int[] X = new int[80];

		private int xOff;

		public Sha1Digest()
		{
			this.Reset();
		}

		public Sha1Digest(Sha1Digest t) : base(t)
		{
			this.H1 = t.H1;
			this.H2 = t.H2;
			this.H3 = t.H3;
			this.H4 = t.H4;
			this.H5 = t.H5;
			Array.Copy(t.X, 0, this.X, 0, t.X.Length);
			this.xOff = t.xOff;
		}

		public override string AlgorithmName
		{
			get
			{
				return "SHA-1";
			}
		}

		public override int GetDigestSize()
		{
			return 20;
		}

		internal override void ProcessWord(byte[] input, int inOff)
		{
			this.X[this.xOff++] = ((int)(input[inOff] & byte.MaxValue) << 24 | (int)(input[inOff + 1] & byte.MaxValue) << 16 | (int)(input[inOff + 2] & byte.MaxValue) << 8 | (int)(input[inOff + 3] & byte.MaxValue));
			if (this.xOff == 16)
			{
				this.ProcessBlock();
			}
		}

		private static void UnpackWord(int word, byte[] outBytes, int outOff)
		{
			outBytes[outOff++] = (byte)((uint)word >> 24);
			outBytes[outOff++] = (byte)((uint)word >> 16);
			outBytes[outOff++] = (byte)((uint)word >> 8);
			outBytes[outOff++] = (byte)word;
		}

		internal override void ProcessLength(long bitLength)
		{
			if (this.xOff > 14)
			{
				this.ProcessBlock();
			}
			this.X[14] = (int)((ulong)bitLength >> 32);
			this.X[15] = (int)(bitLength & (long)((ulong)-1));
		}

		public override int DoFinal(byte[] output, int outOff)
		{
			base.Finish();
			Sha1Digest.UnpackWord(this.H1, output, outOff);
			Sha1Digest.UnpackWord(this.H2, output, outOff + 4);
			Sha1Digest.UnpackWord(this.H3, output, outOff + 8);
			Sha1Digest.UnpackWord(this.H4, output, outOff + 12);
			Sha1Digest.UnpackWord(this.H5, output, outOff + 16);
			this.Reset();
			return 20;
		}

		public override void Reset()
		{
			base.Reset();
			this.H1 = 1732584193;
			this.H2 = -271733879;
			this.H3 = -1732584194;
			this.H4 = 271733878;
			this.H5 = -1009589776;
			this.xOff = 0;
			for (int num = 0; num != this.X.Length; num++)
			{
				this.X[num] = 0;
			}
		}

		private static int F(int u, int v, int w)
		{
			return (u & v) | (~u & w);
		}

		private static int H(int u, int v, int w)
		{
			return u ^ v ^ w;
		}

		private static int G(int u, int v, int w)
		{
			return (u & v) | (u & w) | (v & w);
		}

		internal override void ProcessBlock()
		{
			for (int i = 16; i < 80; i++)
			{
				int num = this.X[i - 3] ^ this.X[i - 8] ^ this.X[i - 14] ^ this.X[i - 16];
				this.X[i] = (num << 1 | (int)((uint)num >> 31));
			}
			int num2 = this.H1;
			int num3 = this.H2;
			int num4 = this.H3;
			int num5 = this.H4;
			int num6 = this.H5;
			int num7 = 0;
			for (int j = 0; j < 4; j++)
			{
				num6 += (num2 << 5 | (int)((uint)num2 >> 27)) + Sha1Digest.F(num3, num4, num5) + this.X[num7++] + 1518500249;
				num3 = (num3 << 30 | (int)((uint)num3 >> 2));
				num5 += (num6 << 5 | (int)((uint)num6 >> 27)) + Sha1Digest.F(num2, num3, num4) + this.X[num7++] + 1518500249;
				num2 = (num2 << 30 | (int)((uint)num2 >> 2));
				num4 += (num5 << 5 | (int)((uint)num5 >> 27)) + Sha1Digest.F(num6, num2, num3) + this.X[num7++] + 1518500249;
				num6 = (num6 << 30 | (int)((uint)num6 >> 2));
				num3 += (num4 << 5 | (int)((uint)num4 >> 27)) + Sha1Digest.F(num5, num6, num2) + this.X[num7++] + 1518500249;
				num5 = (num5 << 30 | (int)((uint)num5 >> 2));
				num2 += (num3 << 5 | (int)((uint)num3 >> 27)) + Sha1Digest.F(num4, num5, num6) + this.X[num7++] + 1518500249;
				num4 = (num4 << 30 | (int)((uint)num4 >> 2));
			}
			for (int k = 0; k < 4; k++)
			{
				num6 += (num2 << 5 | (int)((uint)num2 >> 27)) + Sha1Digest.H(num3, num4, num5) + this.X[num7++] + 1859775393;
				num3 = (num3 << 30 | (int)((uint)num3 >> 2));
				num5 += (num6 << 5 | (int)((uint)num6 >> 27)) + Sha1Digest.H(num2, num3, num4) + this.X[num7++] + 1859775393;
				num2 = (num2 << 30 | (int)((uint)num2 >> 2));
				num4 += (num5 << 5 | (int)((uint)num5 >> 27)) + Sha1Digest.H(num6, num2, num3) + this.X[num7++] + 1859775393;
				num6 = (num6 << 30 | (int)((uint)num6 >> 2));
				num3 += (num4 << 5 | (int)((uint)num4 >> 27)) + Sha1Digest.H(num5, num6, num2) + this.X[num7++] + 1859775393;
				num5 = (num5 << 30 | (int)((uint)num5 >> 2));
				num2 += (num3 << 5 | (int)((uint)num3 >> 27)) + Sha1Digest.H(num4, num5, num6) + this.X[num7++] + 1859775393;
				num4 = (num4 << 30 | (int)((uint)num4 >> 2));
			}
			for (int l = 0; l < 4; l++)
			{
				num6 += (num2 << 5 | (int)((uint)num2 >> 27)) + Sha1Digest.G(num3, num4, num5) + this.X[num7++] + -1894007588;
				num3 = (num3 << 30 | (int)((uint)num3 >> 2));
				num5 += (num6 << 5 | (int)((uint)num6 >> 27)) + Sha1Digest.G(num2, num3, num4) + this.X[num7++] + -1894007588;
				num2 = (num2 << 30 | (int)((uint)num2 >> 2));
				num4 += (num5 << 5 | (int)((uint)num5 >> 27)) + Sha1Digest.G(num6, num2, num3) + this.X[num7++] + -1894007588;
				num6 = (num6 << 30 | (int)((uint)num6 >> 2));
				num3 += (num4 << 5 | (int)((uint)num4 >> 27)) + Sha1Digest.G(num5, num6, num2) + this.X[num7++] + -1894007588;
				num5 = (num5 << 30 | (int)((uint)num5 >> 2));
				num2 += (num3 << 5 | (int)((uint)num3 >> 27)) + Sha1Digest.G(num4, num5, num6) + this.X[num7++] + -1894007588;
				num4 = (num4 << 30 | (int)((uint)num4 >> 2));
			}
			for (int m = 0; m <= 3; m++)
			{
				num6 += (num2 << 5 | (int)((uint)num2 >> 27)) + Sha1Digest.H(num3, num4, num5) + this.X[num7++] + -899497514;
				num3 = (num3 << 30 | (int)((uint)num3 >> 2));
				num5 += (num6 << 5 | (int)((uint)num6 >> 27)) + Sha1Digest.H(num2, num3, num4) + this.X[num7++] + -899497514;
				num2 = (num2 << 30 | (int)((uint)num2 >> 2));
				num4 += (num5 << 5 | (int)((uint)num5 >> 27)) + Sha1Digest.H(num6, num2, num3) + this.X[num7++] + -899497514;
				num6 = (num6 << 30 | (int)((uint)num6 >> 2));
				num3 += (num4 << 5 | (int)((uint)num4 >> 27)) + Sha1Digest.H(num5, num6, num2) + this.X[num7++] + -899497514;
				num5 = (num5 << 30 | (int)((uint)num5 >> 2));
				num2 += (num3 << 5 | (int)((uint)num3 >> 27)) + Sha1Digest.H(num4, num5, num6) + this.X[num7++] + -899497514;
				num4 = (num4 << 30 | (int)((uint)num4 >> 2));
			}
			this.H1 += num2;
			this.H2 += num3;
			this.H3 += num4;
			this.H4 += num5;
			this.H5 += num6;
			this.xOff = 0;
			for (int n = 0; n < 16; n++)
			{
				this.X[n] = 0;
			}
		}
	}
}
