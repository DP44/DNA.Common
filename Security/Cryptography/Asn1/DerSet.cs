using System;
using System.IO;

namespace DNA.Security.Cryptography.Asn1
{
	public class DerSet : Asn1Set
	{
		public static readonly DerSet Empty = new DerSet();

		public static DerSet FromVector(Asn1EncodableVector v)
		{
			if (v.Count >= 1)
			{
				return new DerSet(v);
			}
			return DerSet.Empty;
		}

		internal static DerSet FromVector(Asn1EncodableVector v, bool needsSorting)
		{
			if (v.Count >= 1)
			{
				return new DerSet(v, needsSorting);
			}
			return DerSet.Empty;
		}

		public DerSet() : base(0)
		{
		}

		public DerSet(Asn1Encodable obj) : base(1)
		{
			base.AddObject(obj);
		}

		public DerSet(params Asn1Encodable[] v) : base(v.Length)
		{
			foreach (Asn1Encodable obj in v)
			{
				base.AddObject(obj);
			}
			base.Sort();
		}

		public DerSet(Asn1EncodableVector v) : this(v, true)
		{
		}

		internal DerSet(Asn1EncodableVector v, bool needsSorting) : base(v.Count)
		{
			foreach (object obj in v)
			{
				Asn1Encodable obj2 = (Asn1Encodable)obj;
				base.AddObject(obj2);
			}
			if (needsSorting)
			{
				base.Sort();
			}
		}

		internal override void Encode(DerOutputStream derOut)
		{
			MemoryStream memoryStream = new MemoryStream();
			DerOutputStream derOutputStream = new DerOutputStream(memoryStream);
			foreach (object obj in this)
			{
				Asn1Encodable obj2 = (Asn1Encodable)obj;
				derOutputStream.WriteObject(obj2);
			}
			derOutputStream.Close();
			byte[] bytes = memoryStream.ToArray();
			derOut.WriteEncoded(49, bytes);
		}
	}
}
