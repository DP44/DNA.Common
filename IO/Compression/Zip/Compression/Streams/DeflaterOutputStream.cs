using System;
using System.IO;
using DNA.IO.Checksums;

namespace DNA.IO.Compression.Zip.Compression.Streams
{
	public class DeflaterOutputStream : Stream
	{
		protected byte[] buf;

		protected Deflater def;

		protected Stream baseOutputStream;

		private bool isStreamOwner = true;

		private string password;

		private uint[] keys;

		public bool IsStreamOwner
		{
			get
			{
				return this.isStreamOwner;
			}
			set
			{
				this.isStreamOwner = value;
			}
		}

		public bool CanPatchEntries
		{
			get
			{
				return this.baseOutputStream.CanSeek;
			}
		}

		public override bool CanRead
		{
			get
			{
				return this.baseOutputStream.CanRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.baseOutputStream.CanWrite;
			}
		}

		public override long Length
		{
			get
			{
				return this.baseOutputStream.Length;
			}
		}

		public override long Position
		{
			get
			{
				return this.baseOutputStream.Position;
			}
			set
			{
				throw new NotSupportedException("DefalterOutputStream Position not supported");
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("DeflaterOutputStream Seek not supported");
		}

		public override void SetLength(long val)
		{
			throw new NotSupportedException("DeflaterOutputStream SetLength not supported");
		}

		public override int ReadByte()
		{
			throw new NotSupportedException("DeflaterOutputStream ReadByte not supported");
		}

		public override int Read(byte[] b, int off, int len)
		{
			throw new NotSupportedException("DeflaterOutputStream Read not supported");
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			throw new NotSupportedException("DeflaterOutputStream BeginRead not currently supported");
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			throw new NotSupportedException("DeflaterOutputStream BeginWrite not currently supported");
		}

		protected void Deflate()
		{
			while (!this.def.IsNeedingInput)
			{
				int num = this.def.Deflate(this.buf, 0, this.buf.Length);
				if (num <= 0)
				{
					break;
				}
				if (this.Password != null)
				{
					this.EncryptBlock(this.buf, 0, num);
				}
				this.baseOutputStream.Write(this.buf, 0, num);
			}
			if (!this.def.IsNeedingInput)
			{
				throw new CompressionException("DeflaterOutputStream can't deflate all input?");
			}
		}

		public DeflaterOutputStream(Stream baseOutputStream) : this(baseOutputStream, new Deflater(), 512)
		{
		}

		public DeflaterOutputStream(Stream baseOutputStream, Deflater defl) : this(baseOutputStream, defl, 512)
		{
		}

		public DeflaterOutputStream(Stream baseOutputStream, Deflater deflater, int bufsize)
		{
			if (!baseOutputStream.CanWrite)
			{
				throw new ArgumentException("baseOutputStream", "must support writing");
			}
			if (deflater == null)
			{
				throw new ArgumentNullException("deflater");
			}
			if (bufsize <= 0)
			{
				throw new ArgumentOutOfRangeException("bufsize");
			}
			this.baseOutputStream = baseOutputStream;
			this.buf = new byte[bufsize];
			this.def = deflater;
		}

		public override void Flush()
		{
			this.def.Flush();
			this.Deflate();
			this.baseOutputStream.Flush();
		}

		public virtual void Finish()
		{
			this.def.Finish();
			while (!this.def.IsFinished)
			{
				int num = this.def.Deflate(this.buf, 0, this.buf.Length);
				if (num <= 0)
				{
					break;
				}
				if (this.Password != null)
				{
					this.EncryptBlock(this.buf, 0, num);
				}
				this.baseOutputStream.Write(this.buf, 0, num);
			}
			if (!this.def.IsFinished)
			{
				throw new CompressionException("Can't deflate all input?");
			}
			this.baseOutputStream.Flush();
		}

		public override void Close()
		{
			this.Finish();
			if (this.isStreamOwner)
			{
				this.baseOutputStream.Close();
			}
		}

		public override void WriteByte(byte bval)
		{
			this.Write(new byte[]
			{
				bval
			}, 0, 1);
		}

		public override void Write(byte[] buf, int off, int len)
		{
			this.def.SetInput(buf, off, len);
			this.Deflate();
		}

		public string Password
		{
			get
			{
				return this.password;
			}
			set
			{
				this.password = value;
			}
		}

		protected byte EncryptByte()
		{
			uint num = (this.keys[2] & 65535U) | 2U;
			return (byte)(num * (num ^ 1U) >> 8);
		}

		protected void EncryptBlock(byte[] buffer, int offset, int length)
		{
			for (int i = offset; i < offset + length; i++)
			{
				byte ch = buffer[i];
				int num = i;
				buffer[num] ^= this.EncryptByte();
				this.UpdateKeys(ch);
			}
		}

		protected void InitializePassword(string password)
		{
			this.keys = new uint[]
			{
				305419896U,
				591751049U,
				878082192U
			};
			for (int i = 0; i < password.Length; i++)
			{
				this.UpdateKeys((byte)password[i]);
			}
		}

		protected void UpdateKeys(byte ch)
		{
			this.keys[0] = Crc32.ComputeCrc32(this.keys[0], ch);
			this.keys[1] = this.keys[1] + (uint)((byte)this.keys[0]);
			this.keys[1] = this.keys[1] * 134775813U + 1U;
			this.keys[2] = Crc32.ComputeCrc32(this.keys[2], (byte)(this.keys[1] >> 24));
		}
	}
}
