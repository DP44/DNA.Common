using System;
using DNA.Security.Cryptography.Crypto;

namespace DNA.Security.Cryptography
{
	public abstract class HashProcess
	{
		private IDigest _hashAlg;

		public HashProcess(IDigest hashAlg)
		{
			this._hashAlg = hashAlg;
		}

		public void ComputeBlock(byte[] buffer, int offset, int count)
		{
			this._hashAlg.BlockUpdate(buffer, offset, count);
		}

		protected abstract Hash CreateHash(byte[] data);

		public Hash EndHash(byte[] buffer, int offset, int count)
		{
			this._hashAlg.BlockUpdate(buffer, offset, count);
			return this.EndHash();
		}

		public Hash EndHash()
		{
			byte[] array = new byte[this._hashAlg.GetDigestSize()];
			this._hashAlg.DoFinal(array, 0);
			return this.CreateHash(array);
		}
	}
}
