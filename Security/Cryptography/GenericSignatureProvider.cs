using System;
using System.IO;
using DNA.IO;
using DNA.Security.Cryptography.Crypto;

namespace DNA.Security.Cryptography
{
	public abstract class GenericSignatureProvider : ISignatureProvider
	{
		private ISigner _signer;

		protected ISigner Signer
		{
			get
			{
				return this._signer;
			}
		}

		public abstract Signature Parse(string s);

		public abstract Signature FromByteArray(byte[] data);

		public GenericSignatureProvider(ISigner signer)
		{
			this._signer = signer;
		}

		public virtual Signature GetFileSignature(string path)
		{
			MemoryStream memoryStream = new MemoryStream();
			FileInfo fileInfo = new FileInfo(path);
			using (FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				memoryStream.CopyStream(fileStream, fileInfo.Length);
			}
			return this.Sign(memoryStream.GetBuffer(), fileInfo.Length);
		}

		public Signature Sign(byte[] data)
		{
			return this.Sign(data, 0L, (long)data.Length);
		}

		public Signature Sign(byte[] data, long length)
		{
			return this.Sign(data, 0L, length);
		}

		public Signature Sign(byte[] data, long start, long length)
		{
			this._signer.BlockUpdate(data, (int)start, (int)length);
			return this.FromByteArray(this._signer.GenerateSignature());
		}
	}
}
