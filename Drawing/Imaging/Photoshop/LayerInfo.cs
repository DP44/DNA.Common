using System;

namespace DNA.Drawing.Imaging.Photoshop
{
	public abstract class LayerInfo
	{
		public abstract string Key { get; }

		protected abstract void WriteData(PsdBinaryWriter writer);

		public void Save(PsdBinaryWriter writer)
		{
			writer.Write(Util.SIGNATURE_8BIM);
			writer.Write(this.Key.ToCharArray());
			using (new PsdBlockLengthWriter(writer))
			{
				this.WriteData(writer);
			}
		}
	}
}
