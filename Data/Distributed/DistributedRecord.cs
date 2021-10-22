using System;
using System.Collections.Generic;
using System.IO;
using DNA.Collections;
using DNA.Reflection;
using DNA.Security.Cryptography;

namespace DNA.Data.Distributed
{
	public abstract class DistributedRecord
	{
		private IHashProvider hasher = new MD5HashProvider();
		private Guid _id = Guid.NewGuid();
		private Hash _hash;
		private string Name;
		private static Type[] _recordTypes;
		private static Dictionary<Type, int> _recordIDs;

		/// <summary>
		/// 
		/// </summary>
		public int RecordTypeID
		{
			get
			{
				if (DistributedRecord._recordIDs == null)
				{
					DistributedRecord.PopulateMessageTypes();
				}

				return DistributedRecord._recordIDs[base.GetType()];
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Guid ID
		{
			get
			{
				return this._id;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected abstract void SerializeData(Stream stream);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected abstract void DeserializeData(Stream stream);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void Serialize(Stream stream, string user)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			MemoryStream memoryStream = new MemoryStream();
			this.SerializeData(memoryStream);
			Hash hash = this.hasher.Compute(memoryStream.GetBuffer(), 0L, memoryStream.Position);
			hash != this._hash;
			this._hash = hash;
			binaryWriter.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Position);
			binaryWriter.Write(this._hash.Data.Length);
			binaryWriter.Write(this._hash.Data);
			binaryWriter.Write(this.Name);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public DistributedRecord(string name, string creator)
		{
			this.Name = name;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		private static bool TypeFilter(Type type)
		{
			return type.IsSubclassOf(typeof(DistributedRecord)) && !type.IsAbstract;
		}

		/// <summary>
		/// 
		/// </summary>
		private static void PopulateMessageTypes()
		{
			DistributedRecord._recordTypes = ReflectionTools.GetTypes(new Filter<Type>(DistributedRecord.TypeFilter));
			DistributedRecord._recordIDs = new Dictionary<Type, int>();
			
			for (int i = 0; i < DistributedRecord._recordTypes.Length; i++)
			{
				DistributedRecord._recordIDs[DistributedRecord._recordTypes[i]] = i;
			}
		}
	}
}
