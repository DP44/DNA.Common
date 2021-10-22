using System;
using System.Runtime.InteropServices;

namespace DNA.Runtime.InteropServices
{
	public class GlobalBuffer : IDisposable
	{
		private IntPtr _buffer;
		private uint _size;
		private bool _disposed;

		public IntPtr Pointer
		{
			get
			{
				return this._buffer;
			}
		}

		public uint Size
		{
			get
			{
				return this._size;
			}
		}

		public static implicit operator IntPtr(GlobalBuffer buffer)
		{
			return buffer.Pointer;
		}

		public GlobalBuffer(uint size)
		{
			this._size = size;
			this._buffer = Marshal.AllocHGlobal((int)this._size);
		}

		public GlobalBuffer(int size)
		{
			this._size = (uint)size;
			this._buffer = Marshal.AllocHGlobal((int)this._size);
		}

		public byte[] ToByteArray()
		{
			byte[] byteArray = new byte[this.Size];
			Marshal.Copy(this._buffer, byteArray, 0, (int)this.Size);
			return byteArray;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				Marshal.FreeHGlobal(this._buffer);
				this._disposed = true;
			}
		}

		~GlobalBuffer()
		{
			this.Dispose(false);
		}
	}
}
