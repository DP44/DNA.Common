using System;
using System.IO;
using DNA.IO;
using DNA.Security.Cryptography.Crypto;

namespace DNA.Security.Cryptography
{
	public abstract class GenericHashProvider : IHashProvider
	{
		internal abstract IDigest GetHashAlgorythim();

		public abstract Hash CreateHash(byte[] data);

		public abstract Hash Parse(string s);

		public abstract HashProcess BeginHash();

		public GenericHashProvider()
		{
		}

		public int HashLength
		{
			get
			{
				return this.GetHashAlgorythim().GetDigestSize();
			}
		}

		public Hash Compute(byte[] data)
		{
			if (data.Length > 2147483647)
			{
				throw new Exception("Data over 4GB not supported yet");
			}
			IDigest hashAlgorythim = this.GetHashAlgorythim();
			hashAlgorythim.BlockUpdate(data, 0, data.Length);
			byte[] array = new byte[hashAlgorythim.GetDigestSize()];
			hashAlgorythim.DoFinal(array, 0);
			return this.CreateHash(array);
		}

		public Hash Compute(byte[] data, long length)
		{
			if (length > 2147483647L)
			{
				throw new Exception("Data over 4GB not supported yet");
			}
			IDigest hashAlgorythim = this.GetHashAlgorythim();
			hashAlgorythim.BlockUpdate(data, 0, (int)length);
			byte[] array = new byte[hashAlgorythim.GetDigestSize()];
			hashAlgorythim.DoFinal(array, 0);
			return this.CreateHash(array);
		}

		public Hash Compute(byte[] data, long start, long length)
		{
			if (length > 2147483647L)
			{
				throw new Exception("Data over 4GB not supported yet");
			}
			IDigest hashAlgorythim = this.GetHashAlgorythim();
			hashAlgorythim.BlockUpdate(data, (int)start, (int)length);
			byte[] array = new byte[hashAlgorythim.GetDigestSize()];
			hashAlgorythim.DoFinal(array, 0);
			return this.CreateHash(array);
		}

		public Hash Read(BinaryReader reader)
		{
			return this.CreateHash(reader.ReadBytes(this.HashLength));
		}

		public virtual Hash GetFileHash(string path)
		{
			MemoryStream memoryStream = new MemoryStream();
			FileInfo fileInfo = new FileInfo(path);
			using (FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				memoryStream.CopyStream(fileStream, fileInfo.Length);
			}
			return this.Compute(memoryStream.GetBuffer(), fileInfo.Length);
		}
	}
}
