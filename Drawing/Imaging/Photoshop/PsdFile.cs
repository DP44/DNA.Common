using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DNA.Drawing.Imaging.Photoshop
{
	public class PsdFile
	{
		private class DecompressChannelContext
		{
			private Channel ch;

			private Rectangle rect;

			public DecompressChannelContext(Channel ch, Rectangle rect)
			{
				this.ch = ch;
				this.rect = rect;
			}

			public void DecompressChannel(object context)
			{
				this.ch.DecompressImageData(this.rect);
			}
		}

		private short channelCount;

		private int bitDepth;

		public byte[] ColorModeData = new byte[0];

		private byte[] GlobalLayerMaskData = new byte[0];

		public Layer BaseLayer { get; set; }

		public ImageCompression ImageCompression { get; set; }

		public PsdFile()
		{
			this.Version = 1;
			this.BaseLayer = new Layer(this);
			this.BaseLayer.Rect = new Rectangle(0, 0, 0, 0);
			this.ImageResources = new List<ImageResource>();
			this.Layers = new List<Layer>();
		}

		public void Load(string fileName)
		{
			using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
			{
				this.Load(fileStream);
			}
		}

		public void Load(Stream stream)
		{
			PsdBinaryReader reader = new PsdBinaryReader(stream);
			this.LoadHeader(reader);
			this.LoadColorModeData(reader);
			this.LoadImageResources(reader);
			this.LoadLayerAndMaskInfo(reader);
			this.LoadImage(reader);
			this.DecompressImages();
		}

		public void Save(string fileName)
		{
			using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
			{
				this.Save(fileStream);
			}
		}

		public void Save(Stream stream)
		{
			if (this.BitDepth != 8)
			{
				throw new NotImplementedException("Only 8-bit color has been implemented for saving.");
			}
			PsdBinaryWriter psdBinaryWriter = new PsdBinaryWriter(stream);
			psdBinaryWriter.AutoFlush = true;
			this.PrepareSave();
			this.SaveHeader(psdBinaryWriter);
			this.SaveColorModeData(psdBinaryWriter);
			this.SaveImageResources(psdBinaryWriter);
			this.SaveLayerAndMaskInfo(psdBinaryWriter);
			this.SaveImage(psdBinaryWriter);
		}

		public short Version { get; private set; }

		public short ChannelCount
		{
			get
			{
				return this.channelCount;
			}
			set
			{
				if (value < 1 || value > 56)
				{
					throw new ArgumentException("Number of channels must be from 1 to 56.");
				}
				this.channelCount = value;
			}
		}

		public int RowCount
		{
			get
			{
				return this.BaseLayer.Rect.Height;
			}
			set
			{
				if (value < 0 || value > 30000)
				{
					throw new ArgumentException("Number of rows must be from 1 to 30000.");
				}
				this.BaseLayer.Rect = new Rectangle(0, 0, this.BaseLayer.Rect.Width, value);
			}
		}

		public int ColumnCount
		{
			get
			{
				return this.BaseLayer.Rect.Width;
			}
			set
			{
				if (value < 0 || value > 30000)
				{
					throw new ArgumentException("Number of columns must be from 1 to 30000.");
				}
				this.BaseLayer.Rect = new Rectangle(0, 0, value, this.BaseLayer.Rect.Height);
			}
		}

		public int BitDepth
		{
			get
			{
				return this.bitDepth;
			}
			set
			{
				if (value <= 8)
				{
					if (value != 1 && value != 8)
					{
						goto IL_22;
					}
				}
				else if (value != 16 && value != 32)
				{
					goto IL_22;
				}
				this.bitDepth = value;
				return;
				IL_22:
				throw new NotImplementedException("Invalid bit depth.");
			}
		}

		public PsdColorMode ColorMode { get; set; }

		private void LoadHeader(PsdBinaryReader reader)
		{
			string a = new string(reader.ReadChars(4));
			if (a != "8BPS")
			{
				throw new PsdInvalidException("The given stream is not a valid PSD file");
			}
			this.Version = reader.ReadInt16();
			if (this.Version != 1)
			{
				throw new PsdInvalidException("The PSD file has an unknown version");
			}
			reader.BaseStream.Position += 6L;
			this.ChannelCount = reader.ReadInt16();
			this.RowCount = reader.ReadInt32();
			this.ColumnCount = reader.ReadInt32();
			this.BitDepth = (int)reader.ReadInt16();
			this.ColorMode = (PsdColorMode)reader.ReadInt16();
		}

		private void SaveHeader(PsdBinaryWriter writer)
		{
			string text = "8BPS";
			writer.Write(text.ToCharArray());
			writer.Write(this.Version);
			byte[] value = new byte[6];
			writer.Write(value);
			writer.Write(this.ChannelCount);
			writer.Write(this.RowCount);
			writer.Write(this.ColumnCount);
			writer.Write((short)this.BitDepth);
			writer.Write((short)this.ColorMode);
		}

		private void LoadColorModeData(PsdBinaryReader reader)
		{
			uint num = reader.ReadUInt32();
			if (num > 0U)
			{
				this.ColorModeData = reader.ReadBytes((int)num);
			}
		}

		private void SaveColorModeData(PsdBinaryWriter writer)
		{
			writer.Write((uint)this.ColorModeData.Length);
			writer.Write(this.ColorModeData);
		}

		public List<ImageResource> ImageResources { get; set; }

		public ResolutionInfo Resolution
		{
			get
			{
				return (ResolutionInfo)this.ImageResources.Find((ImageResource x) => x.ID == ResourceID.ResolutionInfo);
			}
			set
			{
				this.ImageResources.RemoveAll((ImageResource x) => x.ID == ResourceID.ResolutionInfo);
				this.ImageResources.Add(value);
			}
		}

		private void LoadImageResources(PsdBinaryReader reader)
		{
			this.ImageResources.Clear();
			uint num = reader.ReadUInt32();
			if (num <= 0U)
			{
				return;
			}
			long position = reader.BaseStream.Position;
			long num2 = position + (long)((ulong)num);
			while (reader.BaseStream.Position < num2)
			{
				ImageResource item = ImageResourceFactory.CreateImageResource(reader);
				this.ImageResources.Add(item);
			}
			reader.BaseStream.Position = position + (long)((ulong)num);
		}

		private void SaveImageResources(PsdBinaryWriter writer)
		{
			using (new PsdBlockLengthWriter(writer))
			{
				foreach (ImageResource imageResource in this.ImageResources)
				{
					imageResource.Save(writer);
				}
			}
		}

		public List<Layer> Layers { get; private set; }

		public bool AbsoluteAlpha { get; set; }

		private void LoadLayerAndMaskInfo(PsdBinaryReader reader)
		{
			uint num = reader.ReadUInt32();
			if (num <= 0U)
			{
				return;
			}
			long position = reader.BaseStream.Position;
			this.LoadLayers(reader);
			this.LoadGlobalLayerMask(reader);
			if (this.BitDepth > 8 && reader.BaseStream.Position < position + (long)((ulong)num))
			{
				string a = new string(reader.ReadChars(8));
				if (a == "8BIMLr16" || a == "8BIMLr32")
				{
					this.LoadLayers(reader);
					this.LoadGlobalLayerMask(reader);
				}
			}
			reader.BaseStream.Position = position + (long)((ulong)num);
		}

		private void SaveLayerAndMaskInfo(PsdBinaryWriter writer)
		{
			using (new PsdBlockLengthWriter(writer))
			{
				this.SaveLayers(writer);
				this.SaveGlobalLayerMask(writer);
			}
		}

		private void LoadLayers(PsdBinaryReader reader)
		{
			uint num = reader.ReadUInt32();
			if (num <= 0U)
			{
				return;
			}
			long position = reader.BaseStream.Position;
			short num2 = reader.ReadInt16();
			if (num2 < 0)
			{
				this.AbsoluteAlpha = true;
				num2 = Math.Abs(num2);
			}
			this.Layers.Clear();
			if (num2 == 0)
			{
				return;
			}
			for (int i = 0; i < (int)num2; i++)
			{
				this.Layers.Add(new Layer(reader, this));
			}
			foreach (Layer layer in this.Layers)
			{
				foreach (Channel channel in layer.Channels)
				{
					Rectangle rect = (channel.ID == -2) ? layer.MaskData.Rect : layer.Rect;
					channel.LoadPixelData(reader, rect);
				}
			}
			if (reader.BaseStream.Position % 2L == 1L)
			{
				reader.ReadByte();
			}
			reader.BaseStream.Position = position + (long)((ulong)num);
		}

		private void DecompressImages()
		{
			IEnumerable<Layer> enumerable = this.Layers.Concat(new List<Layer>
			{
				this.BaseLayer
			});
			foreach (Layer layer in enumerable)
			{
				foreach (Channel channel in layer.Channels)
				{
					Rectangle rect = (channel.ID == -2) ? layer.MaskData.Rect : layer.Rect;
					PsdFile.DecompressChannelContext decompressChannelContext = new PsdFile.DecompressChannelContext(channel, rect);
					decompressChannelContext.DecompressChannel(null);
				}
			}
			foreach (Layer layer2 in this.Layers)
			{
				if (layer2.Channels.ContainsId(-2))
				{
					layer2.MaskData.ImageData = layer2.Channels.GetId(-2).ImageData;
				}
			}
		}

		public void PrepareSave()
		{
			List<Layer> list = this.Layers.Concat(new List<Layer>
			{
				this.BaseLayer
			}).ToList<Layer>();
			foreach (Layer layer in list)
			{
				layer.PrepareSave();
			}
			this.SetVersionInfo();
			this.VerifyLayerSections();
		}

		internal void VerifyLayerSections()
		{
			int num = 0;
			foreach (Layer layer in this.Layers.Reverse<Layer>())
			{
				IEnumerable<LayerInfo> source = from info in layer.AdditionalInfo
				where info.Key == "lsct"
				select info;
				int num2 = source.Count<LayerInfo>();
				if (num2 > 1)
				{
					throw new PsdInvalidException("Layer has more than one section info block.");
				}
				if (num2 != 0)
				{
					LayerSectionInfo layerSectionInfo = (LayerSectionInfo)source.Single<LayerInfo>();
					switch (layerSectionInfo.SectionType)
					{
					case LayerSectionType.OpenFolder:
					case LayerSectionType.ClosedFolder:
						num++;
						break;
					case LayerSectionType.SectionDivider:
						num--;
						if (num < 0)
						{
							throw new PsdInvalidException("Layer section ended without matching start marker.");
						}
						break;
					}
				}
			}
			if (num != 0)
			{
				throw new PsdInvalidException("Layer section not closed by end marker.");
			}
		}

		public void SetVersionInfo()
		{
			IEnumerable<ImageResource> source = from x in this.ImageResources
			where x.ID == ResourceID.VersionInfo
			select x;
			if (source.Count<ImageResource>() > 1)
			{
				throw new PsdInvalidException("Image has more than one VersionInfo resource.");
			}
			VersionInfo versionInfo = (VersionInfo)source.SingleOrDefault<ImageResource>();
			if (versionInfo == null)
			{
				versionInfo = new VersionInfo();
				this.ImageResources.Add(versionInfo);
			}
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			Version version = executingAssembly.GetName().Version;
			string str = string.Concat(new object[]
			{
				version.Major,
				".",
				version.Minor,
				".",
				version.Build
			});
			versionInfo.Version = 1U;
			versionInfo.HasRealMergedData = true;
			versionInfo.ReaderName = "Paint.NET PSD Plugin";
			versionInfo.WriterName = "Paint.NET PSD Plugin " + str;
			versionInfo.FileVersion = 1U;
		}

		private void SaveLayers(PsdBinaryWriter writer)
		{
			using (new PsdBlockLengthWriter(writer))
			{
				short num = (short)this.Layers.Count;
				if (this.AbsoluteAlpha)
				{
					num = -num;
				}
				writer.Write(num);
				foreach (Layer layer in this.Layers)
				{
					layer.Save(writer);
				}
				foreach (Layer layer2 in this.Layers)
				{
					foreach (Channel channel in layer2.Channels)
					{
						channel.SavePixelData(writer);
					}
				}
				if (writer.BaseStream.Position % 2L == 1L)
				{
					writer.Write(0);
				}
			}
		}

		private void LoadGlobalLayerMask(PsdBinaryReader reader)
		{
			uint num = reader.ReadUInt32();
			if (num <= 0U)
			{
				return;
			}
			this.GlobalLayerMaskData = reader.ReadBytes((int)num);
		}

		private void SaveGlobalLayerMask(PsdBinaryWriter writer)
		{
			writer.Write((uint)this.GlobalLayerMaskData.Length);
			writer.Write(this.GlobalLayerMaskData);
		}

		private void LoadImage(PsdBinaryReader reader)
		{
			this.BaseLayer.Rect = new Rectangle(0, 0, this.ColumnCount, this.RowCount);
			this.ImageCompression = (ImageCompression)reader.ReadInt16();
			switch (this.ImageCompression)
			{
			case ImageCompression.Raw:
			{
				int num = this.RowCount * Util.BytesPerRow(this.BaseLayer.Rect, this.BitDepth);
				for (short num2 = 0; num2 < this.ChannelCount; num2 += 1)
				{
					Channel channel = new Channel(num2, this.BaseLayer);
					channel.ImageCompression = this.ImageCompression;
					channel.Length = num;
					channel.ImageData = reader.ReadBytes(num);
					this.BaseLayer.Channels.Add(channel);
				}
				break;
			}
			case ImageCompression.Rle:
				for (short num3 = 0; num3 < this.ChannelCount; num3 += 1)
				{
					int num4 = 0;
					for (int i = 0; i < this.RowCount; i++)
					{
						num4 += (int)reader.ReadUInt16();
					}
					Channel channel2 = new Channel(num3, this.BaseLayer);
					channel2.ImageCompression = this.ImageCompression;
					channel2.Length = num4;
					this.BaseLayer.Channels.Add(channel2);
				}
				foreach (Channel channel3 in this.BaseLayer.Channels)
				{
					channel3.Data = reader.ReadBytes(channel3.Length);
				}
				break;
			}
			if (this.ChannelCount == this.ColorMode.ChannelCount() + 1)
			{
				Channel channel4 = this.BaseLayer.Channels.Last<Channel>();
				channel4.ID = -1;
			}
		}

		private void SaveImage(PsdBinaryWriter writer)
		{
			writer.Write((short)this.ImageCompression);
			if (this.ImageCompression == ImageCompression.Rle)
			{
				foreach (Channel channel in this.BaseLayer.Channels)
				{
					writer.Write(channel.RleHeader);
				}
			}
			foreach (Channel channel2 in this.BaseLayer.Channels)
			{
				writer.Write(channel2.Data);
			}
		}
	}
}
