using System;

namespace DNA.IO.Compression.Zip.Compression
{
	public class PendingBuffer
	{
		protected byte[] buf;

		private int start;

		private int end;

		private uint bits;

		private int bitCount;

		public PendingBuffer() : this(4096)
		{
		}

		public PendingBuffer(int bufsize)
		{
			this.buf = new byte[bufsize];
		}

		public void Reset()
		{
			this.start = (this.end = (this.bitCount = 0));
		}

		public void WriteByte(int b)
		{
			this.buf[this.end++] = (byte)b;
		}

		public void WriteShort(int s)
		{
			this.buf[this.end++] = (byte)s;
			this.buf[this.end++] = (byte)(s >> 8);
		}

		public void WriteInt(int s)
		{
			this.buf[this.end++] = (byte)s;
			this.buf[this.end++] = (byte)(s >> 8);
			this.buf[this.end++] = (byte)(s >> 16);
			this.buf[this.end++] = (byte)(s >> 24);
		}

		public void WriteBlock(byte[] block, int offset, int len)
		{
			Array.Copy(block, offset, this.buf, this.end, len);
			this.end += len;
		}

		public int BitCount
		{
			get
			{
				return this.bitCount;
			}
		}

		public void AlignToByte()
		{
			if (this.bitCount > 0)
			{
				this.buf[this.end++] = (byte)this.bits;
				if (this.bitCount > 8)
				{
					this.buf[this.end++] = (byte)(this.bits >> 8);
				}
			}
			this.bits = 0U;
			this.bitCount = 0;
		}

		public void WriteBits(int b, int count)
		{
			this.bits |= (uint)((uint)b << this.bitCount);
			this.bitCount += count;
			if (this.bitCount >= 16)
			{
				this.buf[this.end++] = (byte)this.bits;
				this.buf[this.end++] = (byte)(this.bits >> 8);
				this.bits >>= 16;
				this.bitCount -= 16;
			}
		}

		public void WriteShortMSB(int s)
		{
			this.buf[this.end++] = (byte)(s >> 8);
			this.buf[this.end++] = (byte)s;
		}

		public bool IsFlushed
		{
			get
			{
				return this.end == 0;
			}
		}

		public int Flush(byte[] output, int offset, int length)
		{
			if (this.bitCount >= 8)
			{
				this.buf[this.end++] = (byte)this.bits;
				this.bits >>= 8;
				this.bitCount -= 8;
			}
			if (length > this.end - this.start)
			{
				length = this.end - this.start;
				Array.Copy(this.buf, this.start, output, offset, length);
				this.start = 0;
				this.end = 0;
			}
			else
			{
				Array.Copy(this.buf, this.start, output, offset, length);
				this.start += length;
			}
			return length;
		}

		public byte[] ToByteArray()
		{
			byte[] array = new byte[this.end - this.start];
			Array.Copy(this.buf, this.start, array, 0, array.Length);
			this.start = 0;
			this.end = 0;
			return array;
		}
	}
}
