using System;

namespace DNA.IO.Compression.Zip.Compression.Streams
{
	public class OutputWindow
	{
		private static int WINDOW_SIZE = 32768;

		private static int WINDOW_MASK = OutputWindow.WINDOW_SIZE - 1;

		private byte[] window = new byte[OutputWindow.WINDOW_SIZE];

		private int windowEnd;

		private int windowFilled;

		public void Write(int abyte)
		{
			if (this.windowFilled++ == OutputWindow.WINDOW_SIZE)
			{
				throw new InvalidOperationException("Window full");
			}
			this.window[this.windowEnd++] = (byte)abyte;
			this.windowEnd &= OutputWindow.WINDOW_MASK;
		}

		private void SlowRepeat(int repStart, int len, int dist)
		{
			while (len-- > 0)
			{
				this.window[this.windowEnd++] = this.window[repStart++];
				this.windowEnd &= OutputWindow.WINDOW_MASK;
				repStart &= OutputWindow.WINDOW_MASK;
			}
		}

		public void Repeat(int len, int dist)
		{
			if ((this.windowFilled += len) > OutputWindow.WINDOW_SIZE)
			{
				throw new InvalidOperationException("Window full");
			}
			int num = this.windowEnd - dist & OutputWindow.WINDOW_MASK;
			int num2 = OutputWindow.WINDOW_SIZE - len;
			if (num > num2 || this.windowEnd >= num2)
			{
				this.SlowRepeat(num, len, dist);
				return;
			}
			if (len <= dist)
			{
				Array.Copy(this.window, num, this.window, this.windowEnd, len);
				this.windowEnd += len;
				return;
			}
			while (len-- > 0)
			{
				this.window[this.windowEnd++] = this.window[num++];
			}
		}

		public int CopyStored(StreamManipulator input, int len)
		{
			len = Math.Min(Math.Min(len, OutputWindow.WINDOW_SIZE - this.windowFilled), input.AvailableBytes);
			int num = OutputWindow.WINDOW_SIZE - this.windowEnd;
			int num2;
			if (len > num)
			{
				num2 = input.CopyBytes(this.window, this.windowEnd, num);
				if (num2 == num)
				{
					num2 += input.CopyBytes(this.window, 0, len - num);
				}
			}
			else
			{
				num2 = input.CopyBytes(this.window, this.windowEnd, len);
			}
			this.windowEnd = (this.windowEnd + num2 & OutputWindow.WINDOW_MASK);
			this.windowFilled += num2;
			return num2;
		}

		public void CopyDict(byte[] dict, int offset, int len)
		{
			if (this.windowFilled > 0)
			{
				throw new InvalidOperationException();
			}
			if (len > OutputWindow.WINDOW_SIZE)
			{
				offset += len - OutputWindow.WINDOW_SIZE;
				len = OutputWindow.WINDOW_SIZE;
			}
			Array.Copy(dict, offset, this.window, 0, len);
			this.windowEnd = (len & OutputWindow.WINDOW_MASK);
		}

		public int GetFreeSpace()
		{
			return OutputWindow.WINDOW_SIZE - this.windowFilled;
		}

		public int GetAvailable()
		{
			return this.windowFilled;
		}

		public int CopyOutput(byte[] output, int offset, int len)
		{
			int num = this.windowEnd;
			if (len > this.windowFilled)
			{
				len = this.windowFilled;
			}
			else
			{
				num = (this.windowEnd - this.windowFilled + len & OutputWindow.WINDOW_MASK);
			}
			int num2 = len;
			int num3 = len - num;
			if (num3 > 0)
			{
				Array.Copy(this.window, OutputWindow.WINDOW_SIZE - num3, output, offset, num3);
				offset += num3;
				len = num;
			}
			Array.Copy(this.window, num - len, output, offset, len);
			this.windowFilled -= num2;
			if (this.windowFilled < 0)
			{
				throw new InvalidOperationException();
			}
			return num2;
		}

		public void Reset()
		{
			this.windowFilled = (this.windowEnd = 0);
		}
	}
}
