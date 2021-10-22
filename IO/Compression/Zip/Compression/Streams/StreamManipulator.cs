using System;

namespace DNA.IO.Compression.Zip.Compression.Streams
{
	public class StreamManipulator
	{
		private byte[] window;

		private int window_start;

		private int window_end;

		private uint buffer;

		private int bits_in_buffer;

		public int PeekBits(int n)
		{
			if (this.bits_in_buffer < n)
			{
				if (this.window_start == this.window_end)
				{
					return -1;
				}
				this.buffer |= (uint)((uint)((int)(this.window[this.window_start++] & byte.MaxValue) | (int)(this.window[this.window_start++] & byte.MaxValue) << 8) << this.bits_in_buffer);
				this.bits_in_buffer += 16;
			}
			return (int)((ulong)this.buffer & (ulong)((long)((1 << n) - 1)));
		}

		public void DropBits(int n)
		{
			this.buffer >>= n;
			this.bits_in_buffer -= n;
		}

		public int GetBits(int n)
		{
			int num = this.PeekBits(n);
			if (num >= 0)
			{
				this.DropBits(n);
			}
			return num;
		}

		public int AvailableBits
		{
			get
			{
				return this.bits_in_buffer;
			}
		}

		public int AvailableBytes
		{
			get
			{
				return this.window_end - this.window_start + (this.bits_in_buffer >> 3);
			}
		}

		public void SkipToByteBoundary()
		{
			this.buffer >>= (this.bits_in_buffer & 7);
			this.bits_in_buffer &= -8;
		}

		public bool IsNeedingInput
		{
			get
			{
				return this.window_start == this.window_end;
			}
		}

		public int CopyBytes(byte[] output, int offset, int length)
		{
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			if ((this.bits_in_buffer & 7) != 0)
			{
				throw new InvalidOperationException("Bit buffer is not byte aligned!");
			}
			int num = 0;
			while (this.bits_in_buffer > 0 && length > 0)
			{
				output[offset++] = (byte)this.buffer;
				this.buffer >>= 8;
				this.bits_in_buffer -= 8;
				length--;
				num++;
			}
			if (length == 0)
			{
				return num;
			}
			int num2 = this.window_end - this.window_start;
			if (length > num2)
			{
				length = num2;
			}
			Array.Copy(this.window, this.window_start, output, offset, length);
			this.window_start += length;
			if ((this.window_start - this.window_end & 1) != 0)
			{
				this.buffer = (uint)(this.window[this.window_start++] & byte.MaxValue);
				this.bits_in_buffer = 8;
			}
			return num + length;
		}

		public void Reset()
		{
			this.buffer = (uint)(this.window_start = (this.window_end = (this.bits_in_buffer = 0)));
		}

		public void SetInput(byte[] buf, int off, int len)
		{
			if (this.window_start < this.window_end)
			{
				throw new InvalidOperationException("Old input was not completely processed");
			}
			int num = off + len;
			if (0 > off || off > num || num > buf.Length)
			{
				throw new ArgumentOutOfRangeException();
			}
			if ((len & 1) != 0)
			{
				this.buffer |= (uint)((uint)(buf[off++] & byte.MaxValue) << this.bits_in_buffer);
				this.bits_in_buffer += 8;
			}
			this.window = buf;
			this.window_start = off;
			this.window_end = num;
		}
	}
}
