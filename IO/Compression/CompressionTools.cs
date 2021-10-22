using System;
using System.IO;
using DNA.IO.Compression.Zip.Compression;
using DNA.IO.Compression.Zip.Compression.Streams;

namespace DNA.IO.Compression
{
	public class CompressionTools
	{
		private bool UseHeaders;
		private Inflater inflater;
		private Deflater deflater;
		private MemoryStream outStream = new MemoryStream();

		public CompressionTools()
		{
			this.deflater = new Deflater(Deflater.DefaultCompression, !this.UseHeaders);
			this.inflater = new Inflater(!this.UseHeaders);
		}

		public CompressionTools(bool useHeaders)
		{
			this.UseHeaders = useHeaders;
		}

		public byte[] Compress(byte[] data)
		{
			byte[] compressedData;
			
			lock (this.deflater)
			{
				this.deflater.Reset();
				this.outStream.Position = 0L;
				this.outStream.SetLength(0L);

				DeflaterOutputStream output = new DeflaterOutputStream(this.outStream, this.deflater);
				
				BinaryWriter writer = new BinaryWriter(output);
				writer.Write(data.Length);
				writer.Write(data, 0, data.Length);
				writer.Flush();
				
				output.Finish();
				
				compressedData = this.outStream.ToArray();
			}

			return compressedData;
		}

		public byte[] Decompress(byte[] data)
		{
			byte[] decompressedData;
			
			lock (this.inflater)
			{
				MemoryStream baseInputStream = new MemoryStream(data);
				
				this.inflater.Reset();
				
				InflaterInputStream input = new InflaterInputStream(baseInputStream, this.inflater);
				BinaryReader reader = new BinaryReader(input);
				
				int count = reader.ReadInt32();
				decompressedData = reader.ReadBytes(count);
			}

			return decompressedData;
		}
	}
}
