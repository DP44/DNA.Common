using System;

namespace DNA.IO.Checksums
{
	public class XOR8Checksum : IChecksum<byte>
	{
		private byte _value;

		public byte Value
		{
			get
			{
				return this._value;
			}
		}

		public void Reset()
		{
			this._value = 0;
		}

		public void Update(byte bval)
		{
			this._value ^= bval;
		}

		public void Update(byte[] buffer)
		{
			for (int i = 0; i < buffer.Length; i++)
			{
				this._value ^= buffer[i];
			}
		}

		public void Update(byte[] buf, int off, int len)
		{
			for (int i = 0; i < len; i++)
			{
				this._value ^= buf[off + i];
			}
		}
	}
}
