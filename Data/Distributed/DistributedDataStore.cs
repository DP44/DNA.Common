using System;
using System.Collections.Generic;
using System.IO;

namespace DNA.Data.Distributed
{
	public class DistributedDataStore
	{
		private Dictionary<Guid, DistributedRecord> _records = new Dictionary<Guid, DistributedRecord>();

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void Set(DistributedRecord record)
		{
			this._records[record.ID] = record;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void Remove(DistributedRecord record)
		{
			this._records.Remove(record.ID);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void Remove(Guid id)
		{
			this._records.Remove(id);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void Commit(Stream stream, string user)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(this._records.Count);
			MemoryStream memory = new MemoryStream();
			
			foreach (KeyValuePair<Guid, DistributedRecord> keyValuePair in this._records)
			{
				memory.Position = 0L;
				DistributedRecord value = keyValuePair.Value;
				writer.Write(value.RecordTypeID);
				
				value.Serialize(memory, user);
				byte[] buffer = memory.GetBuffer();
				writer.Write(buffer, 0, (int)memory.Position);
			}
		}
	}
}
