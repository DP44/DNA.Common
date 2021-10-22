using System;

namespace DNA.Security.Cryptography.Checksums
{
	public sealed class Adler32 : IChecksum
	{
		private static readonly uint BASE = 65521U;

		private uint checksum;

		public uint Value
		{
			get
			{
				return this.checksum;
			}
		}

		public Adler32()
		{
			this.Reset();
		}

		public void Reset()
		{
			this.checksum = 1U;
		}

		public void Update(int bval)
		{
			uint num = this.checksum & 65535U;
			uint num2 = this.checksum >> 16;
			num = (num + (uint)(bval & 255)) % Adler32.BASE;
			num2 = (num + num2) % Adler32.BASE;
			this.checksum = (num2 << 16) + num;
		}

		public void Update(byte[] buffer)
		{
			this.Update(buffer, 0, buffer.Length);
		}

		public void Update(byte[] buf, int off, int len)
		{
			if (buf == null)
			{
				throw new ArgumentNullException("buf");
			}
			if (off < 0 || len < 0 || off + len > buf.Length)
			{
				throw new ArgumentOutOfRangeException();
			}
			uint num = this.checksum & 65535U;
			uint num2 = this.checksum >> 16;
			while (len > 0)
			{
				int num3 = 3800;
				if (num3 > len)
				{
					num3 = len;
				}
				len -= num3;
				while (--num3 >= 0)
				{
					num += (uint)(buf[off++] & byte.MaxValue);
					num2 += num;
				}
				num %= Adler32.BASE;
				num2 %= Adler32.BASE;
			}
			this.checksum = (num2 << 16 | num);
		}
	}
}
