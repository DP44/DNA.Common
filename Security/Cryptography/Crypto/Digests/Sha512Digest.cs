using System;

namespace DNA.Security.Cryptography.Crypto.Digests
{
	public class Sha512Digest : LongDigest
	{
		private const int DigestLength = 64;

		public Sha512Digest()
		{
		}

		public Sha512Digest(Sha512Digest t) : base(t)
		{
		}

		public override string AlgorithmName
		{
			get
			{
				return "SHA-512";
			}
		}

		public override int GetDigestSize()
		{
			return 64;
		}

		public override int DoFinal(byte[] output, int outOff)
		{
			base.Finish();
			LongDigest.UnpackWord(this.H1, output, outOff);
			LongDigest.UnpackWord(this.H2, output, outOff + 8);
			LongDigest.UnpackWord(this.H3, output, outOff + 16);
			LongDigest.UnpackWord(this.H4, output, outOff + 24);
			LongDigest.UnpackWord(this.H5, output, outOff + 32);
			LongDigest.UnpackWord(this.H6, output, outOff + 40);
			LongDigest.UnpackWord(this.H7, output, outOff + 48);
			LongDigest.UnpackWord(this.H8, output, outOff + 56);
			this.Reset();
			return 64;
		}

		public override void Reset()
		{
			base.Reset();
			this.H1 = 7640891576956012808L;
			this.H2 = -4942790177534073029L;
			this.H3 = 4354685564936845355L;
			this.H4 = -6534734903238641935L;
			this.H5 = 5840696475078001361L;
			this.H6 = -7276294671716946913L;
			this.H7 = 2270897969802886507L;
			this.H8 = 6620516959819538809L;
		}
	}
}
