using System;
using System.IO;
using System.Text;

namespace DNA.Drawing.Imaging.Photoshop
{
	public class PsdBinaryReader : IDisposable
	{
		private BinaryReader reader;

		private bool disposed;

		public Stream BaseStream
		{
			get
			{
				return this.reader.BaseStream;
			}
		}

		public PsdBinaryReader(Stream stream)
		{
			this.reader = new BinaryReader(stream, Encoding.Default);
		}

		public byte ReadByte()
		{
			return this.reader.ReadByte();
		}

		public byte[] ReadBytes(int count)
		{
			return this.reader.ReadBytes(count);
		}

		public char[] ReadChars(int count)
		{
			return this.reader.ReadChars(count);
		}

		public bool ReadBoolean()
		{
			return this.reader.ReadBoolean();
		}

		public unsafe short ReadInt16()
		{
			short result = this.reader.ReadInt16();
			Util.SwapBytes((byte*)(&result), 2);
			return result;
		}

		public unsafe int ReadInt32()
		{
			int result = this.reader.ReadInt32();
			Util.SwapBytes((byte*)(&result), 4);
			return result;
		}

		public unsafe long ReadInt64()
		{
			long result = this.reader.ReadInt64();
			Util.SwapBytes((byte*)(&result), 8);
			return result;
		}

		public unsafe ushort ReadUInt16()
		{
			ushort result = this.reader.ReadUInt16();
			Util.SwapBytes((byte*)(&result), 2);
			return result;
		}

		public unsafe uint ReadUInt32()
		{
			uint result = this.reader.ReadUInt32();
			Util.SwapBytes((byte*)(&result), 4);
			return result;
		}

		public unsafe ulong ReadUInt64()
		{
			ulong result = this.reader.ReadUInt64();
			Util.SwapBytes((byte*)(&result), 8);
			return result;
		}

		public string ReadPascalString()
		{
			byte b = this.ReadByte();
			char[] value = this.ReadChars((int)b);
			if (b % 2 == 0)
			{
				this.ReadByte();
			}
			return new string(value);
		}

		public string ReadUnicodeString()
		{
			int num = this.ReadInt32();
			int count = 2 * num;
			byte[] bytes = this.ReadBytes(count);
			return Encoding.BigEndianUnicode.GetString(bytes, 0, count);
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
			if (disposing && this.reader != null)
			{
				this.reader.Close();
				this.reader = null;
			}
			this.disposed = true;
		}
	}
}
