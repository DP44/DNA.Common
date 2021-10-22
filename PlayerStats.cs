using System;
using System.IO;

namespace DNA
{
	public abstract class PlayerStats
	{
		private const int FileIdent = 1095783254;

		public string GamerTag;
		public DateTime DateRecorded;

		public abstract int Version { get; }

		public void Save(BinaryWriter writer)
		{
			writer.Write(this.FileIdent);
			writer.Write(this.Version);
			
			if (this.GamerTag != null)
			{
				writer.Write(this.GamerTag);
			}
			else
			{
				writer.Write("<null>");
			}
			
			writer.Write(this.DateRecorded.Ticks);
			this.SaveData(writer);
		}

		protected abstract void SaveData(BinaryWriter writer);

		public void Load(BinaryReader reader)
		{
			int ident = reader.ReadInt32();
			
			// Ensure that the file has the correct identification number.
			if (ident != this.FileIdent)
			{
				throw new Exception();
			}
			
			int version = reader.ReadInt32();
			this.GamerTag = reader.ReadString();
			this.DateRecorded = new DateTime(reader.ReadInt64());
			this.LoadData(reader, version);
		}

		protected abstract void LoadData(BinaryReader reader, int version);
	}
}
