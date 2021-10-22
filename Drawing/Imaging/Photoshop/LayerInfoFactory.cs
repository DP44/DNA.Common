using System;

namespace DNA.Drawing.Imaging.Photoshop
{
	public static class LayerInfoFactory
	{
		public static LayerInfo CreateLayerInfo(PsdBinaryReader reader)
		{
			string a = new string(reader.ReadChars(4));
			if (a != "8BIM")
			{
				throw new PsdInvalidException("Could not read LayerInfo due to signature mismatch.");
			}
			string text = new string(reader.ReadChars(4));
			int num = reader.ReadInt32();
			long position = reader.BaseStream.Position;
			string a2;
			LayerInfo result;
			if ((a2 = text) != null)
			{
				if (a2 == "lsct")
				{
					result = new LayerSectionInfo(reader, num);
					goto IL_88;
				}
				if (a2 == "luni")
				{
					result = new LayerUnicodeName(reader);
					goto IL_88;
				}
			}
			result = new RawLayerInfo(reader, text, num);
			IL_88:
			long num2 = position + (long)num;
			if (reader.BaseStream.Position < num2)
			{
				reader.BaseStream.Position = num2;
			}
			return result;
		}
	}
}
