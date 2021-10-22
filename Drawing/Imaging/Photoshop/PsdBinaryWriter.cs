using System;
using System.IO;
using System.Text;

namespace DNA.Drawing.Imaging.Photoshop
{
	public class PsdBinaryWriter : IDisposable
	{
		private BinaryWriter writer;

		private bool disposed;

		public Stream BaseStream
		{
			get
			{
				return this.writer.BaseStream;
			}
		}

		public bool AutoFlush { get; set; }

		public PsdBinaryWriter(Stream stream)
		{
			this.writer = new BinaryWriter(stream);
		}

		public void Flush()
		{
			this.writer.Flush();
		}

		public void WritePascalString(string s)
		{
			string s2 = (s.Length > 255) ? s.Substring(0, 255) : s;
			byte[] bytes = Encoding.Default.GetBytes(s2);
			this.Write((byte)bytes.Length);
			this.Write(bytes);
			if (bytes.Length % 2 == 0)
			{
				this.Write(0);
			}
			if (this.AutoFlush)
			{
				this.Flush();
			}
		}

		public void WriteUnicodeString(string s)
		{
			this.Write(s.Length);
			byte[] bytes = Encoding.BigEndianUnicode.GetBytes(s);
			this.Write(bytes);
		}

		public void Write(bool value)
		{
			this.writer.Write(value);
			if (this.AutoFlush)
			{
				this.Flush();
			}
		}

		public void Write(char[] value)
		{
			this.writer.Write(value);
			if (this.AutoFlush)
			{
				this.Flush();
			}
		}

		public void Write(byte[] value)
		{
			this.writer.Write(value);
			if (this.AutoFlush)
			{
				this.Flush();
			}
		}

		public void Write(byte value)
		{
			this.writer.Write(value);
			if (this.AutoFlush)
			{
				this.Flush();
			}
		}

		public unsafe void Write(short value)
		{
			Util.SwapBytes2((byte*)(&value));
			this.writer.Write(value);
			if (this.AutoFlush)
			{
				this.Flush();
			}
		}

		public unsafe void Write(int value)
		{
			Util.SwapBytes4((byte*)(&value));
			this.writer.Write(value);
			if (this.AutoFlush)
			{
				this.Flush();
			}
		}

		public unsafe void Write(long value)
		{
			Util.SwapBytes((byte*)(&value), 8);
			this.writer.Write(value);
			if (this.AutoFlush)
			{
				this.Flush();
			}
		}

		public unsafe void Write(ushort value)
		{
			Util.SwapBytes2((byte*)(&value));
			this.writer.Write(value);
			if (this.AutoFlush)
			{
				this.Flush();
			}
		}

		public unsafe void Write(uint value)
		{
			Util.SwapBytes4((byte*)(&value));
			this.writer.Write(value);
			if (this.AutoFlush)
			{
				this.Flush();
			}
		}

		public unsafe void Write(ulong value)
		{
			Util.SwapBytes((byte*)(&value), 8);
			this.writer.Write(value);
			if (this.AutoFlush)
			{
				this.Flush();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}
			if (disposing && this.writer != null)
			{
				this.writer.Close();
				this.writer = null;
			}
			this.disposed = true;
		}
	}
}
