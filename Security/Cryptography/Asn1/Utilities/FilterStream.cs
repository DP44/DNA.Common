using System;
using System.IO;

namespace DNA.Security.Cryptography.Asn1.Utilities
{
	public class FilterStream : Stream
	{
		private readonly Stream s;

		public FilterStream(Stream s)
		{
			this.s = s;
		}

		public override bool CanRead
		{
			get
			{
				return this.s.CanRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return this.s.CanSeek;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.s.CanWrite;
			}
		}

		public override long Length
		{
			get
			{
				return this.s.Length;
			}
		}

		public override long Position
		{
			get
			{
				return this.s.Position;
			}
			set
			{
				this.s.Position = value;
			}
		}

		public override void Close()
		{
			this.s.Close();
		}

		public override void Flush()
		{
			this.s.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return this.s.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			this.s.SetLength(value);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return this.s.Read(buffer, offset, count);
		}

		public override int ReadByte()
		{
			return this.s.ReadByte();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.s.Write(buffer, offset, count);
		}

		public override void WriteByte(byte value)
		{
			this.s.WriteByte(value);
		}
	}
}
