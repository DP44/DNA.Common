using System;

namespace DNA.Security.Cryptography.Crypto.Digests
{
	public class MD5Digest : GeneralDigest
	{
		private const int DigestLength = 16;

		private int H1;

		private int H2;

		private int H3;

		private int H4;

		private int[] X = new int[16];

		private int xOff;

		private static readonly int S11 = 7;

		private static readonly int S12 = 12;

		private static readonly int S13 = 17;

		private static readonly int S14 = 22;

		private static readonly int S21 = 5;

		private static readonly int S22 = 9;

		private static readonly int S23 = 14;

		private static readonly int S24 = 20;

		private static readonly int S31 = 4;

		private static readonly int S32 = 11;

		private static readonly int S33 = 16;

		private static readonly int S34 = 23;

		private static readonly int S41 = 6;

		private static readonly int S42 = 10;

		private static readonly int S43 = 15;

		private static readonly int S44 = 21;

		public MD5Digest()
		{
			this.Reset();
		}

		public MD5Digest(MD5Digest t) : base(t)
		{
			this.H1 = t.H1;
			this.H2 = t.H2;
			this.H3 = t.H3;
			this.H4 = t.H4;
			Array.Copy(t.X, 0, this.X, 0, t.X.Length);
			this.xOff = t.xOff;
		}

		public override string AlgorithmName
		{
			get
			{
				return "MD5";
			}
		}

		public override int GetDigestSize()
		{
			return 16;
		}

		internal override void ProcessWord(byte[] input, int inOff)
		{
			this.X[this.xOff++] = ((int)(input[inOff] & byte.MaxValue) | (int)(input[inOff + 1] & byte.MaxValue) << 8 | (int)(input[inOff + 2] & byte.MaxValue) << 16 | (int)(input[inOff + 3] & byte.MaxValue) << 24);
			if (this.xOff == 16)
			{
				this.ProcessBlock();
			}
		}

		internal override void ProcessLength(long bitLength)
		{
			if (this.xOff > 14)
			{
				this.ProcessBlock();
			}
			this.X[14] = (int)(bitLength & (long)((ulong)-1));
			this.X[15] = (int)((ulong)bitLength >> 32);
		}

		private void UnpackWord(int word, byte[] outBytes, int outOff)
		{
			outBytes[outOff] = (byte)word;
			outBytes[outOff + 1] = (byte)((uint)word >> 8);
			outBytes[outOff + 2] = (byte)((uint)word >> 16);
			outBytes[outOff + 3] = (byte)((uint)word >> 24);
		}

		public override int DoFinal(byte[] output, int outOff)
		{
			base.Finish();
			this.UnpackWord(this.H1, output, outOff);
			this.UnpackWord(this.H2, output, outOff + 4);
			this.UnpackWord(this.H3, output, outOff + 8);
			this.UnpackWord(this.H4, output, outOff + 12);
			this.Reset();
			return 16;
		}

		public override void Reset()
		{
			base.Reset();
			this.H1 = 1732584193;
			this.H2 = -271733879;
			this.H3 = -1732584194;
			this.H4 = 271733878;
			this.xOff = 0;
			for (int num = 0; num != this.X.Length; num++)
			{
				this.X[num] = 0;
			}
		}

		private int RotateLeft(int x, int n)
		{
			return x << n | (int)((uint)x >> 32 - n);
		}

		private int F(int u, int v, int w)
		{
			return (u & v) | (~u & w);
		}

		private int G(int u, int v, int w)
		{
			return (u & w) | (v & ~w);
		}

		private int H(int u, int v, int w)
		{
			return u ^ v ^ w;
		}

		private int K(int u, int v, int w)
		{
			return v ^ (u | ~w);
		}

		internal override void ProcessBlock()
		{
			int num = this.H1;
			int num2 = this.H2;
			int num3 = this.H3;
			int num4 = this.H4;
			num = this.RotateLeft(num + this.F(num2, num3, num4) + this.X[0] + -680876936, MD5Digest.S11) + num2;
			num4 = this.RotateLeft(num4 + this.F(num, num2, num3) + this.X[1] + -389564586, MD5Digest.S12) + num;
			num3 = this.RotateLeft(num3 + this.F(num4, num, num2) + this.X[2] + 606105819, MD5Digest.S13) + num4;
			num2 = this.RotateLeft(num2 + this.F(num3, num4, num) + this.X[3] + -1044525330, MD5Digest.S14) + num3;
			num = this.RotateLeft(num + this.F(num2, num3, num4) + this.X[4] + -176418897, MD5Digest.S11) + num2;
			num4 = this.RotateLeft(num4 + this.F(num, num2, num3) + this.X[5] + 1200080426, MD5Digest.S12) + num;
			num3 = this.RotateLeft(num3 + this.F(num4, num, num2) + this.X[6] + -1473231341, MD5Digest.S13) + num4;
			num2 = this.RotateLeft(num2 + this.F(num3, num4, num) + this.X[7] + -45705983, MD5Digest.S14) + num3;
			num = this.RotateLeft(num + this.F(num2, num3, num4) + this.X[8] + 1770035416, MD5Digest.S11) + num2;
			num4 = this.RotateLeft(num4 + this.F(num, num2, num3) + this.X[9] + -1958414417, MD5Digest.S12) + num;
			num3 = this.RotateLeft(num3 + this.F(num4, num, num2) + this.X[10] + -42063, MD5Digest.S13) + num4;
			num2 = this.RotateLeft(num2 + this.F(num3, num4, num) + this.X[11] + -1990404162, MD5Digest.S14) + num3;
			num = this.RotateLeft(num + this.F(num2, num3, num4) + this.X[12] + 1804603682, MD5Digest.S11) + num2;
			num4 = this.RotateLeft(num4 + this.F(num, num2, num3) + this.X[13] + -40341101, MD5Digest.S12) + num;
			num3 = this.RotateLeft(num3 + this.F(num4, num, num2) + this.X[14] + -1502002290, MD5Digest.S13) + num4;
			num2 = this.RotateLeft(num2 + this.F(num3, num4, num) + this.X[15] + 1236535329, MD5Digest.S14) + num3;
			num = this.RotateLeft(num + this.G(num2, num3, num4) + this.X[1] + -165796510, MD5Digest.S21) + num2;
			num4 = this.RotateLeft(num4 + this.G(num, num2, num3) + this.X[6] + -1069501632, MD5Digest.S22) + num;
			num3 = this.RotateLeft(num3 + this.G(num4, num, num2) + this.X[11] + 643717713, MD5Digest.S23) + num4;
			num2 = this.RotateLeft(num2 + this.G(num3, num4, num) + this.X[0] + -373897302, MD5Digest.S24) + num3;
			num = this.RotateLeft(num + this.G(num2, num3, num4) + this.X[5] + -701558691, MD5Digest.S21) + num2;
			num4 = this.RotateLeft(num4 + this.G(num, num2, num3) + this.X[10] + 38016083, MD5Digest.S22) + num;
			num3 = this.RotateLeft(num3 + this.G(num4, num, num2) + this.X[15] + -660478335, MD5Digest.S23) + num4;
			num2 = this.RotateLeft(num2 + this.G(num3, num4, num) + this.X[4] + -405537848, MD5Digest.S24) + num3;
			num = this.RotateLeft(num + this.G(num2, num3, num4) + this.X[9] + 568446438, MD5Digest.S21) + num2;
			num4 = this.RotateLeft(num4 + this.G(num, num2, num3) + this.X[14] + -1019803690, MD5Digest.S22) + num;
			num3 = this.RotateLeft(num3 + this.G(num4, num, num2) + this.X[3] + -187363961, MD5Digest.S23) + num4;
			num2 = this.RotateLeft(num2 + this.G(num3, num4, num) + this.X[8] + 1163531501, MD5Digest.S24) + num3;
			num = this.RotateLeft(num + this.G(num2, num3, num4) + this.X[13] + -1444681467, MD5Digest.S21) + num2;
			num4 = this.RotateLeft(num4 + this.G(num, num2, num3) + this.X[2] + -51403784, MD5Digest.S22) + num;
			num3 = this.RotateLeft(num3 + this.G(num4, num, num2) + this.X[7] + 1735328473, MD5Digest.S23) + num4;
			num2 = this.RotateLeft(num2 + this.G(num3, num4, num) + this.X[12] + -1926607734, MD5Digest.S24) + num3;
			num = this.RotateLeft(num + this.H(num2, num3, num4) + this.X[5] + -378558, MD5Digest.S31) + num2;
			num4 = this.RotateLeft(num4 + this.H(num, num2, num3) + this.X[8] + -2022574463, MD5Digest.S32) + num;
			num3 = this.RotateLeft(num3 + this.H(num4, num, num2) + this.X[11] + 1839030562, MD5Digest.S33) + num4;
			num2 = this.RotateLeft(num2 + this.H(num3, num4, num) + this.X[14] + -35309556, MD5Digest.S34) + num3;
			num = this.RotateLeft(num + this.H(num2, num3, num4) + this.X[1] + -1530992060, MD5Digest.S31) + num2;
			num4 = this.RotateLeft(num4 + this.H(num, num2, num3) + this.X[4] + 1272893353, MD5Digest.S32) + num;
			num3 = this.RotateLeft(num3 + this.H(num4, num, num2) + this.X[7] + -155497632, MD5Digest.S33) + num4;
			num2 = this.RotateLeft(num2 + this.H(num3, num4, num) + this.X[10] + -1094730640, MD5Digest.S34) + num3;
			num = this.RotateLeft(num + this.H(num2, num3, num4) + this.X[13] + 681279174, MD5Digest.S31) + num2;
			num4 = this.RotateLeft(num4 + this.H(num, num2, num3) + this.X[0] + -358537222, MD5Digest.S32) + num;
			num3 = this.RotateLeft(num3 + this.H(num4, num, num2) + this.X[3] + -722521979, MD5Digest.S33) + num4;
			num2 = this.RotateLeft(num2 + this.H(num3, num4, num) + this.X[6] + 76029189, MD5Digest.S34) + num3;
			num = this.RotateLeft(num + this.H(num2, num3, num4) + this.X[9] + -640364487, MD5Digest.S31) + num2;
			num4 = this.RotateLeft(num4 + this.H(num, num2, num3) + this.X[12] + -421815835, MD5Digest.S32) + num;
			num3 = this.RotateLeft(num3 + this.H(num4, num, num2) + this.X[15] + 530742520, MD5Digest.S33) + num4;
			num2 = this.RotateLeft(num2 + this.H(num3, num4, num) + this.X[2] + -995338651, MD5Digest.S34) + num3;
			num = this.RotateLeft(num + this.K(num2, num3, num4) + this.X[0] + -198630844, MD5Digest.S41) + num2;
			num4 = this.RotateLeft(num4 + this.K(num, num2, num3) + this.X[7] + 1126891415, MD5Digest.S42) + num;
			num3 = this.RotateLeft(num3 + this.K(num4, num, num2) + this.X[14] + -1416354905, MD5Digest.S43) + num4;
			num2 = this.RotateLeft(num2 + this.K(num3, num4, num) + this.X[5] + -57434055, MD5Digest.S44) + num3;
			num = this.RotateLeft(num + this.K(num2, num3, num4) + this.X[12] + 1700485571, MD5Digest.S41) + num2;
			num4 = this.RotateLeft(num4 + this.K(num, num2, num3) + this.X[3] + -1894986606, MD5Digest.S42) + num;
			num3 = this.RotateLeft(num3 + this.K(num4, num, num2) + this.X[10] + -1051523, MD5Digest.S43) + num4;
			num2 = this.RotateLeft(num2 + this.K(num3, num4, num) + this.X[1] + -2054922799, MD5Digest.S44) + num3;
			num = this.RotateLeft(num + this.K(num2, num3, num4) + this.X[8] + 1873313359, MD5Digest.S41) + num2;
			num4 = this.RotateLeft(num4 + this.K(num, num2, num3) + this.X[15] + -30611744, MD5Digest.S42) + num;
			num3 = this.RotateLeft(num3 + this.K(num4, num, num2) + this.X[6] + -1560198380, MD5Digest.S43) + num4;
			num2 = this.RotateLeft(num2 + this.K(num3, num4, num) + this.X[13] + 1309151649, MD5Digest.S44) + num3;
			num = this.RotateLeft(num + this.K(num2, num3, num4) + this.X[4] + -145523070, MD5Digest.S41) + num2;
			num4 = this.RotateLeft(num4 + this.K(num, num2, num3) + this.X[11] + -1120210379, MD5Digest.S42) + num;
			num3 = this.RotateLeft(num3 + this.K(num4, num, num2) + this.X[2] + 718787259, MD5Digest.S43) + num4;
			num2 = this.RotateLeft(num2 + this.K(num3, num4, num) + this.X[9] + -343485551, MD5Digest.S44) + num3;
			this.H1 += num;
			this.H2 += num2;
			this.H3 += num3;
			this.H4 += num4;
			this.xOff = 0;
			for (int num5 = 0; num5 != this.X.Length; num5++)
			{
				this.X[num5] = 0;
			}
		}
	}
}
