using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace DNA.Drawing.Imaging.Photoshop
{
	public class Thumbnail : ImageResource
	{
		public override ResourceID ID
		{
			get
			{
				return ResourceID.ThumbnailRgb;
			}
		}

		public Bitmap Image { get; set; }

		public Thumbnail(string name) : base(name)
		{
		}

		public Thumbnail(PsdBinaryReader reader, ResourceID id, string name, int numBytes) : base(name)
		{
			uint num = reader.ReadUInt32();
			uint width = reader.ReadUInt32();
			uint height = reader.ReadUInt32();
			reader.ReadUInt32();
			reader.ReadUInt32();
			reader.ReadUInt32();
			reader.ReadUInt16();
			reader.ReadUInt16();
			if (num == 0U)
			{
				this.Image = new Bitmap((int)width, (int)height, PixelFormat.Format24bppRgb);
				return;
			}
			if (num != 1U)
			{
				throw new PsdInvalidException("Unknown thumbnail format.");
			}
			byte[] buffer = reader.ReadBytes(numBytes - 28);
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				Bitmap bitmap = new Bitmap(memoryStream);
				this.Image = (Bitmap)bitmap.Clone();
			}
			if (id == ResourceID.ThumbnailBgr)
			{
				return;
			}
		}

		protected override void WriteData(PsdBinaryWriter writer)
		{
			throw new NotImplementedException();
		}
	}
}
