using System;

namespace DNA.Drawing.Imaging.Photoshop
{
	public static class ImageResourceFactory
	{
		public static ImageResource CreateImageResource(PsdBinaryReader reader)
		{
			new string(reader.ReadChars(4));
			ushort num = reader.ReadUInt16();
			string name = reader.ReadPascalString();
			int num2 = (int)reader.ReadUInt32();
			long num3 = reader.BaseStream.Position + (long)num2;
			ResourceID resourceID = (ResourceID)num;
			ResourceID resourceID2 = resourceID;
			ImageResource result;
			if (resourceID2 <= ResourceID.ThumbnailBgr)
			{
				switch (resourceID2)
				{
				case ResourceID.ResolutionInfo:
					result = new ResolutionInfo(reader, name);
					goto IL_B4;
				case ResourceID.AlphaChannelNames:
					result = new AlphaChannelNames(reader, name, num2);
					goto IL_B4;
				default:
					if (resourceID2 != ResourceID.ThumbnailBgr)
					{
						goto IL_A8;
					}
					break;
				}
			}
			else if (resourceID2 != ResourceID.ThumbnailRgb)
			{
				if (resourceID2 != ResourceID.VersionInfo)
				{
					goto IL_A8;
				}
				result = new VersionInfo(reader, name);
				goto IL_B4;
			}
			result = new Thumbnail(reader, resourceID, name, num2);
			goto IL_B4;
			IL_A8:
			result = new RawImageResource(reader, name, resourceID, num2);
			IL_B4:
			if (reader.BaseStream.Position % 2L == 1L)
			{
				reader.ReadByte();
			}
			if (reader.BaseStream.Position < num3)
			{
				reader.BaseStream.Position = num3;
			}
			return result;
		}
	}
}
