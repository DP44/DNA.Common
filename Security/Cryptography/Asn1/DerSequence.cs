using System;
using System.IO;

namespace DNA.Security.Cryptography.Asn1
{
	public class DerSequence : Asn1Sequence
	{
		public static readonly DerSequence Empty = new DerSequence();

		public static DerSequence FromVector(Asn1EncodableVector v)
		{
			if (v.Count >= 1)
			{
				return new DerSequence(v);
			}
			return DerSequence.Empty;
		}

		public DerSequence() : base(0)
		{
		}

		public DerSequence(Asn1Encodable obj) : base(1)
		{
			base.AddObject(obj);
		}

		public DerSequence(params Asn1Encodable[] v) : base(v.Length)
		{
			foreach (Asn1Encodable obj in v)
			{
				base.AddObject(obj);
			}
		}

		public DerSequence(Asn1EncodableVector v) : base(v.Count)
		{
			foreach (object obj in v)
			{
				Asn1Encodable obj2 = (Asn1Encodable)obj;
				base.AddObject(obj2);
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
			derOut.WriteEncoded(48, bytes);
		}
	}
}
