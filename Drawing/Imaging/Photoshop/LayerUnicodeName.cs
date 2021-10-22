using System;

namespace DNA.Drawing.Imaging.Photoshop
{
	public class LayerUnicodeName : LayerInfo
	{
		public override string Key
		{
			get
			{
				return "luni";
			}
		}

		public string Name { get; set; }

		public LayerUnicodeName(string name)
		{
			this.Name = name;
		}

		public LayerUnicodeName(PsdBinaryReader reader)
		{
			this.Name = reader.ReadUnicodeString();
		}

		protected override void WriteData(PsdBinaryWriter writer)
		{
			writer.WriteUnicodeString(this.Name);
		}
	}
}
