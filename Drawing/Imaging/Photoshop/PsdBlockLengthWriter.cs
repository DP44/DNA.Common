using System;

namespace DNA.Drawing.Imaging.Photoshop
{
	internal class PsdBlockLengthWriter : IDisposable
	{
		private bool disposed;

		private long lengthPosition;

		private long startPosition;

		private PsdBinaryWriter writer;

		public PsdBlockLengthWriter(PsdBinaryWriter writer)
		{
			this.writer = writer;
			this.lengthPosition = writer.BaseStream.Position;
			writer.Write(4277010157U);
			this.startPosition = writer.BaseStream.Position;
		}

		public void Write()
		{
			long position = this.writer.BaseStream.Position;
			this.writer.BaseStream.Position = this.lengthPosition;
			long num = position - this.startPosition;
			this.writer.Write((uint)num);
			this.writer.BaseStream.Position = position;
		}

		public void Dispose()
		{
			if (!this.disposed)
			{
				this.Write();
				this.disposed = true;
			}
		}
	}
}
