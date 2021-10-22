using System;
using System.IO;
using DNA.IO.Checksums;

namespace DNA.IO.Compression.Zip.Compression.Streams
{
	public class InflaterInputStream : Stream
	{
		protected Inflater inf;

		protected byte[] buf;

		protected int len;

		private byte[] onebytebuffer = new byte[1];

		protected Stream baseInputStream;

		protected long csize;

		private bool isStreamOwner = true;

		private int readChunkSize;

		protected byte[] cryptbuffer;

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

		public override bool CanRead
		{
			get
			{
				return this.baseInputStream.CanRead;
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
				return false;
			}
		}

		public override long Length
		{
			get
			{
				return (long)this.len;
			}
		}

		public override long Position
		{
			get
			{
				return this.baseInputStream.Position;
			}
			set
			{
				throw new NotSupportedException("InflaterInputStream Position not supported");
			}
		}

		public override void Flush()
		{
			this.baseInputStream.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("Seek not supported");
		}

		public override void SetLength(long val)
		{
			throw new NotSupportedException("InflaterInputStream SetLength not supported");
		}

		public override void Write(byte[] array, int offset, int count)
		{
			throw new NotSupportedException("InflaterInputStream Write not supported");
		}

		public override void WriteByte(byte val)
		{
			throw new NotSupportedException("InflaterInputStream WriteByte not supported");
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			throw new NotSupportedException("InflaterInputStream BeginWrite not supported");
		}

		public InflaterInputStream(Stream baseInputStream) : this(baseInputStream, new Inflater(), 4096)
		{
		}

		public InflaterInputStream(Stream baseInputStream, Inflater inf) : this(baseInputStream, inf, 4096)
		{
		}

		public InflaterInputStream(Stream baseInputStream, Inflater inflater, int bufferSize)
		{
			if (baseInputStream == null)
			{
				throw new ArgumentNullException("InflaterInputStream baseInputStream is null");
			}
			if (inflater == null)
			{
				throw new ArgumentNullException("InflaterInputStream Inflater is null");
			}
			if (bufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("bufferSize");
			}
			this.baseInputStream = baseInputStream;
			this.inf = inflater;
			this.buf = new byte[bufferSize];
			if (baseInputStream.CanSeek)
			{
				this.len = (int)baseInputStream.Length;
				return;
			}
			this.len = 0;
		}

		public virtual int Available
		{
			get
			{
				if (!this.inf.IsFinished)
				{
					return 1;
				}
				return 0;
			}
		}

		public override void Close()
		{
			if (this.isStreamOwner)
			{
				this.baseInputStream.Close();
			}
		}

		protected int BufferReadSize
		{
			get
			{
				return this.readChunkSize;
			}
			set
			{
				this.readChunkSize = value;
			}
		}

		protected void FillInputBuffer()
		{
			if (this.readChunkSize <= 0)
			{
				this.len = this.baseInputStream.Read(this.buf, 0, this.buf.Length);
				return;
			}
			this.len = this.baseInputStream.Read(this.buf, 0, this.readChunkSize);
		}

		protected void Fill()
		{
			this.FillInputBuffer();
			if (this.keys != null)
			{
				this.DecryptBlock(this.buf, 0, this.len);
			}
			if (this.len <= 0)
			{
				throw new CompressionException("Deflated stream ends early.");
			}
			this.inf.SetInput(this.buf, 0, this.len);
		}

		public override int ReadByte()
		{
			int num = this.Read(this.onebytebuffer, 0, 1);
			if (num > 0)
			{
				return (int)(this.onebytebuffer[0] & byte.MaxValue);
			}
			return -1;
		}

		public override int Read(byte[] b, int off, int len)
		{
			int num;
			for (;;)
			{
				try
				{
					num = this.inf.Inflate(b, off, len);
				}
				catch (Exception ex)
				{
					throw new CompressionException(ex.ToString());
				}
				if (num > 0)
				{
					break;
				}
				if (this.inf.IsNeedingDictionary)
				{
					goto Block_2;
				}
				if (this.inf.IsFinished)
				{
					return 0;
				}
				if (!this.inf.IsNeedingInput)
				{
					goto IL_60;
				}
				this.Fill();
			}
			return num;
			Block_2:
			throw new CompressionException("Need a dictionary");
			IL_60:
			throw new InvalidOperationException("Don't know what to do");
		}

		public long Skip(long n)
		{
			if (n <= 0L)
			{
				throw new ArgumentOutOfRangeException("n");
			}
			if (this.baseInputStream.CanSeek)
			{
				this.baseInputStream.Seek(n, SeekOrigin.Current);
				return n;
			}
			int num = 2048;
			if (n < (long)num)
			{
				num = (int)n;
			}
			byte[] array = new byte[num];
			return (long)this.baseInputStream.Read(array, 0, array.Length);
		}

		protected byte DecryptByte()
		{
			uint num = (this.keys[2] & 65535U) | 2U;
			return (byte)(num * (num ^ 1U) >> 8);
		}

		protected void DecryptBlock(byte[] buf, int off, int len)
		{
			for (int i = off; i < off + len; i++)
			{
				int num = i;
				buf[num] ^= this.DecryptByte();
				this.UpdateKeys(buf[i]);
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

		protected void StopDecrypting()
		{
			this.keys = null;
			this.cryptbuffer = null;
		}
	}
}
