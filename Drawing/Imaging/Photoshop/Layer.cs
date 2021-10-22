using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;

namespace DNA.Drawing.Imaging.Photoshop
{
	public class Layer
	{
		private string blendModeKey;

		private static int protectTransBit = BitVector32.CreateMask();

		private static int visibleBit = BitVector32.CreateMask(Layer.protectTransBit);

		private BitVector32 flags = default(BitVector32);

		internal PsdFile PsdFile { get; private set; }

		public Rectangle Rect { get; set; }

		public ChannelList Channels { get; private set; }

		public Channel AlphaChannel
		{
			get
			{
				if (this.Channels.ContainsId(-1))
				{
					return this.Channels.GetId(-1);
				}
				return null;
			}
		}

		public string BlendModeKey
		{
			get
			{
				return this.blendModeKey;
			}
			set
			{
				if (value.Length != 4)
				{
					throw new ArgumentException("Key length must be 4");
				}
				this.blendModeKey = value;
			}
		}

		public byte Opacity { get; set; }

		public bool Clipping { get; set; }

		public bool Visible
		{
			get
			{
				return !this.flags[Layer.visibleBit];
			}
			set
			{
				this.flags[Layer.visibleBit] = !value;
			}
		}

		public bool ProtectTrans
		{
			get
			{
				return this.flags[Layer.protectTransBit];
			}
			set
			{
				this.flags[Layer.protectTransBit] = value;
			}
		}

		public string Name { get; set; }

		public BlendingRanges BlendingRangesData { get; set; }

		public Mask MaskData { get; set; }

		public List<LayerInfo> AdditionalInfo { get; set; }

		public Layer(PsdFile psdFile)
		{
			this.PsdFile = psdFile;
			this.Rect = Rectangle.Empty;
			this.Channels = new ChannelList();
			this.BlendModeKey = "norm";
			this.AdditionalInfo = new List<LayerInfo>();
		}

		public Layer(PsdBinaryReader reader, PsdFile psdFile)
		{
			this.PsdFile = psdFile;
			Rectangle rect = default(Rectangle);
			rect.Y = reader.ReadInt32();
			rect.X = reader.ReadInt32();
			rect.Height = reader.ReadInt32() - rect.Y;
			rect.Width = reader.ReadInt32() - rect.X;
			this.Rect = rect;
			int num = (int)reader.ReadUInt16();
			this.Channels = new ChannelList();
			for (int i = 0; i < num; i++)
			{
				Channel item = new Channel(reader, this);
				this.Channels.Add(item);
			}
			string a = new string(reader.ReadChars(4));
			if (a != "8BIM")
			{
				throw new PsdInvalidException("Invalid signature in channel header.");
			}
			this.BlendModeKey = new string(reader.ReadChars(4));
			this.Opacity = reader.ReadByte();
			this.Clipping = reader.ReadBoolean();
			byte data = reader.ReadByte();
			this.flags = new BitVector32((int)data);
			reader.ReadByte();
			uint num2 = reader.ReadUInt32();
			long position = reader.BaseStream.Position;
			this.MaskData = new Mask(reader, this);
			this.BlendingRangesData = new BlendingRanges(reader, this);
			long position2 = reader.BaseStream.Position;
			this.Name = reader.ReadPascalString();
			int count = (int)((reader.BaseStream.Position - position2) % 4L);
			reader.ReadBytes(count);
			long num3 = position + (long)((ulong)num2);
			this.AdditionalInfo = new List<LayerInfo>();
			try
			{
				while (reader.BaseStream.Position < num3)
				{
					this.AdditionalInfo.Add(LayerInfoFactory.CreateLayerInfo(reader));
				}
			}
			catch
			{
				reader.BaseStream.Position = num3;
			}
			foreach (LayerInfo layerInfo in this.AdditionalInfo)
			{
				string key;
				if ((key = layerInfo.Key) != null && key == "luni")
				{
					this.Name = ((LayerUnicodeName)layerInfo).Name;
				}
			}
		}

		public unsafe void CreateMissingChannels()
		{
			short num = this.PsdFile.ColorMode.ChannelCount();
			for (short num2 = 0; num2 < num; num2 += 1)
			{
				if (!this.Channels.ContainsId((int)num2))
				{
					int num3 = this.Rect.Height * this.Rect.Width;
					Channel channel = new Channel(num2, this);
					channel.ImageData = new byte[num3];
					fixed (byte* ptr = &channel.ImageData[0])
					{
						Util.Fill(ptr, byte.MaxValue, num3);
					}
					this.Channels.Add(channel);
				}
			}
		}

		public void PrepareSave()
		{
			foreach (Channel channel in this.Channels)
			{
				channel.CompressImageData();
			}
			IEnumerable<LayerInfo> source = from x in this.AdditionalInfo
			where x is LayerUnicodeName
			select x;
			if (source.Count<LayerInfo>() > 1)
			{
				throw new PsdInvalidException("Layer has more than one LayerUnicodeName.");
			}
			LayerUnicodeName layerUnicodeName = (LayerUnicodeName)source.FirstOrDefault<LayerInfo>();
			if (layerUnicodeName == null)
			{
				layerUnicodeName = new LayerUnicodeName(this.Name);
				this.AdditionalInfo.Add(layerUnicodeName);
				return;
			}
			if (layerUnicodeName.Name != this.Name)
			{
				layerUnicodeName.Name = this.Name;
			}
		}

		public void Save(PsdBinaryWriter writer)
		{
			writer.Write(this.Rect.Top);
			writer.Write(this.Rect.Left);
			writer.Write(this.Rect.Bottom);
			writer.Write(this.Rect.Right);
			writer.Write((short)this.Channels.Count);
			foreach (Channel channel in this.Channels)
			{
				channel.Save(writer);
			}
			writer.Write(Util.SIGNATURE_8BIM);
			writer.Write(this.BlendModeKey.ToCharArray());
			writer.Write(this.Opacity);
			writer.Write(this.Clipping);
			writer.Write((byte)this.flags.Data);
			writer.Write(0);
			using (new PsdBlockLengthWriter(writer))
			{
				this.MaskData.Save(writer);
				this.BlendingRangesData.Save(writer);
				long position = writer.BaseStream.Position;
				writer.WritePascalString(this.Name);
				int num = (int)((writer.BaseStream.Position - position) % 4L);
				for (int i = 0; i < num; i++)
				{
					writer.Write(0);
				}
				foreach (LayerInfo layerInfo in this.AdditionalInfo)
				{
					layerInfo.Save(writer);
				}
			}
		}
	}
}
