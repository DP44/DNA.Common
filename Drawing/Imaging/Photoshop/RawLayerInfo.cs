using System;

namespace DNA.Drawing.Imaging.Photoshop
{
	public class RawLayerInfo : LayerInfo
	{
		private string key;

		public override string Key
		{
			get
			{
				return this.key;
			}
		}

		public byte[] Data { get; private set; }

		public RawLayerInfo(string key)
		{
			this.key = key;
		}

		public RawLayerInfo(PsdBinaryReader reader, string key, int dataLength)
		{
			this.key = key;
			this.Data = reader.ReadBytes(dataLength);
		}

		protected override void WriteData(PsdBinaryWriter writer)
		{
			writer.Write(this.Data);
		}
	}
}
