using System;

namespace DNA.Security.Cryptography.Asn1
{
	public class BerSequence : DerSequence
	{
		public new static readonly BerSequence Empty = new BerSequence();

		public new static BerSequence FromVector(Asn1EncodableVector v)
		{
			if (v.Count >= 1)
			{
				return new BerSequence(v);
			}
			return BerSequence.Empty;
		}

		public BerSequence()
		{
		}

		public BerSequence(Asn1Encodable obj) : base(obj)
		{
		}

		public BerSequence(params Asn1Encodable[] v) : base(v)
		{
		}

		public BerSequence(Asn1EncodableVector v) : base(v)
		{
		}

		internal override void Encode(DerOutputStream derOut)
		{
			if (derOut is Asn1OutputStream || derOut is BerOutputStream)
			{
				derOut.WriteByte(48);
				derOut.WriteByte(128);
				foreach (object obj in this)
				{
					Asn1Encodable obj2 = (Asn1Encodable)obj;
					derOut.WriteObject(obj2);
				}
				derOut.WriteByte(0);
				derOut.WriteByte(0);
				return;
			}
			base.Encode(derOut);
		}
	}
}
