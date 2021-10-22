using System;
using System.Collections.Generic;

namespace DNA.Drawing.Imaging.Photoshop
{
	public class AlphaChannelNames : ImageResource
	{
		private List<string> channelNames = new List<string>();

		public override ResourceID ID =>
			ResourceID.AlphaChannelNames;

		public List<string> ChannelNames =>
			this.channelNames;

		public AlphaChannelNames() : base(string.Empty) {}

		public AlphaChannelNames(PsdBinaryReader reader, 
								 string name, int resourceDataLength) 
			: base(name)
		{
			long num = reader.BaseStream.Position + (long)resourceDataLength;
			
			while (reader.BaseStream.Position < num)
			{
				byte count = reader.ReadByte();
				string text = new string(reader.ReadChars((int)count));
			
				if (text.Length > 0)
				{
					this.channelNames.Add(text);
				}
			}
		}

		protected override void WriteData(PsdBinaryWriter writer)
		{
			foreach (string text in this.channelNames)
			{
				writer.Write((byte)text.Length);
				writer.Write(text.ToCharArray());
			}
		}
	}
}
