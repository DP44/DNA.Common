using System;
using System.Globalization;

namespace DNA.Drawing.Imaging.Photoshop
{
	public abstract class ImageResource
	{
		public string Name { get; set; }

		public abstract ResourceID ID { get; }

		protected ImageResource(string name)
		{
			this.Name = name;
		}

		public void Save(PsdBinaryWriter writer)
		{
			writer.Write(Util.SIGNATURE_8BIM);
			writer.Write((ushort)this.ID);
			writer.WritePascalString(this.Name);
			writer.Write(0U);
			long position = writer.BaseStream.Position;
			this.WriteData(writer);
			long position2 = writer.BaseStream.Position;
			long num = position2 - position;
			writer.BaseStream.Position = position - 4L;
			writer.Write((uint)num);
			writer.BaseStream.Position = position2;
			if (writer.BaseStream.Position % 2L == 1L)
			{
				writer.Write(0);
			}
		}

		protected abstract void WriteData(PsdBinaryWriter writer);

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
			{
				this.ID,
				this.Name
			});
		}
	}
}
