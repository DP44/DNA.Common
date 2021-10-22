using System;

namespace DNA.Drawing.Imaging.Photoshop
{
	public class RawImageResource : ImageResource
	{
		private ResourceID id;

		public byte[] Data { get; private set; }

		public override ResourceID ID
		{
			get
			{
				return this.id;
			}
		}

		public RawImageResource(string name) : base(name)
		{
		}

		public RawImageResource(PsdBinaryReader reader, string name, ResourceID resourceId, int numBytes) : base(name)
		{
			this.id = resourceId;
			this.Data = reader.ReadBytes(numBytes);
		}

		protected override void WriteData(PsdBinaryWriter writer)
		{
			writer.Write(this.Data);
		}
	}
}
