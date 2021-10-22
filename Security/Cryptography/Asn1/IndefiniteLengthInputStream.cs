using System;
using System.IO;

namespace DNA.Security.Cryptography.Asn1
{
	internal class IndefiniteLengthInputStream : LimitedInputStream
	{
		private int _b1;

		private int _b2;

		private bool _eofReached;

		private bool _eofOn00 = true;

		internal IndefiniteLengthInputStream(Stream inStream) : base(inStream)
		{
			this._b1 = inStream.ReadByte();
			this._b2 = inStream.ReadByte();
			if (this._b2 < 0)
			{
				throw new EndOfStreamException();
			}
			this.CheckForEof();
		}

		internal void SetEofOn00(bool eofOn00)
		{
			this._eofOn00 = eofOn00;
			this.CheckForEof();
		}

		private bool CheckForEof()
		{
			if (!this._eofReached && this._eofOn00 && this._b1 == 0 && this._b2 == 0)
			{
				this._eofReached = true;
				this.SetParentEofDetect(true);
			}
			return this._eofReached;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (this._eofOn00 || count < 3)
			{
				return base.Read(buffer, offset, count);
			}
			if (this._eofReached)
			{
				return 0;
			}
			int num = this._in.Read(buffer, offset + 2, count - 2);
			if (num <= 0)
			{
				throw new EndOfStreamException();
			}
			buffer[offset] = (byte)this._b1;
			buffer[offset + 1] = (byte)this._b2;
			this._b1 = this._in.ReadByte();
			this._b2 = this._in.ReadByte();
			if (this._b2 < 0)
			{
				throw new EndOfStreamException();
			}
			return num + 2;
		}

		public override int ReadByte()
		{
			if (this.CheckForEof())
			{
				return -1;
			}
			int num = this._in.ReadByte();
			if (num < 0)
			{
				throw new EndOfStreamException();
			}
			int b = this._b1;
			this._b1 = this._b2;
			this._b2 = num;
			return b;
		}
	}
}
