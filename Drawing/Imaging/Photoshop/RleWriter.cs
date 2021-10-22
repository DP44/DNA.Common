using System;
using System.IO;

namespace DNA.Drawing.Imaging.Photoshop
{
	public class RleWriter
	{
		private int maxPacketLength = 128;

		private object rleLock;

		private Stream stream;

		private byte[] data;

		private int offset;

		private bool rlePacket;

		private int packetLength;

		private int idxDataRawPacket;

		private byte lastValue;

		public RleWriter(Stream stream)
		{
			this.rleLock = new object();
			this.stream = stream;
		}

		public unsafe int Write(byte[] data, int offset, int count)
		{
			if (!Util.CheckBufferBounds(data, offset, count))
			{
				throw new ArgumentOutOfRangeException();
			}
			if (count == 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			int result;
			lock (this.rleLock)
			{
				long position = this.stream.Position;
				this.data = data;
				this.offset = offset;
				fixed (byte* ptr = &data[0])
				{
					byte* ptr2 = ptr + offset;
					byte* ptrEnd = ptr2 + count;
					this.EncodeToStream(ptr2, ptrEnd);
				}
				result = (int)(this.stream.Position - position);
			}
			return result;
		}

		private void ClearPacket()
		{
			this.rlePacket = false;
			this.packetLength = 0;
		}

		private void WriteRlePacket()
		{
			byte value = (byte)(1 - this.packetLength);
			this.stream.WriteByte(value);
			this.stream.WriteByte(this.lastValue);
		}

		private void WriteRawPacket()
		{
			byte value = (byte)(this.packetLength - 1);
			this.stream.WriteByte(value);
			this.stream.Write(this.data, this.idxDataRawPacket, this.packetLength);
		}

		private void WritePacket()
		{
			if (this.rlePacket)
			{
				this.WriteRlePacket();
				return;
			}
			this.WriteRawPacket();
		}

		private unsafe int EncodeToStream(byte* ptr, byte* ptrEnd)
		{
			this.idxDataRawPacket = this.offset;
			this.rlePacket = false;
			this.lastValue = *ptr;
			this.packetLength = 1;
			ptr++;
			int num = 1;
			while (ptr < ptrEnd)
			{
				byte b = *ptr;
				if (this.packetLength == 1)
				{
					this.rlePacket = (b == this.lastValue);
					this.lastValue = b;
					this.packetLength = 2;
				}
				else if (this.packetLength == this.maxPacketLength)
				{
					this.WritePacket();
					this.rlePacket = false;
					this.lastValue = b;
					this.idxDataRawPacket = this.offset + num;
					this.packetLength = 1;
				}
				else if (this.rlePacket)
				{
					if (b == this.lastValue)
					{
						this.packetLength++;
					}
					else
					{
						this.WriteRlePacket();
						this.rlePacket = false;
						this.lastValue = b;
						this.idxDataRawPacket = this.offset + num;
						this.packetLength = 1;
					}
				}
				else if (b == this.lastValue)
				{
					this.packetLength--;
					this.WriteRawPacket();
					this.rlePacket = true;
					this.packetLength = 2;
				}
				else
				{
					this.lastValue = b;
					this.packetLength++;
				}
				ptr++;
				num++;
			}
			this.WritePacket();
			this.ClearPacket();
			return num;
		}
	}
}
