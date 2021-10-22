using System;

namespace DNA.Drawing.Imaging.Photoshop
{
	public class LayerSectionInfo : LayerInfo
	{
		public override string Key
		{
			get
			{
				return "lsct";
			}
		}

		public LayerSectionType SectionType { get; set; }

		public string BlendModeKey { get; set; }

		public LayerSectionInfo(PsdBinaryReader reader, int dataLength)
		{
			this.SectionType = (LayerSectionType)reader.ReadInt32();
			if (dataLength == 12)
			{
				string a = new string(reader.ReadChars(4));
				if (a == "8BIM")
				{
					this.BlendModeKey = new string(reader.ReadChars(4));
				}
			}
		}

		protected override void WriteData(PsdBinaryWriter writer)
		{
			writer.Write((int)this.SectionType);
			if (this.BlendModeKey != null)
			{
				writer.Write(Util.SIGNATURE_8BIM);
				writer.Write(this.BlendModeKey.ToCharArray());
			}
		}
	}
}
