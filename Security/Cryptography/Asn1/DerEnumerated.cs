using System;
using DNA.Security.Cryptography.Math;
using DNA.Security.Cryptography.Utilities;

namespace DNA.Security.Cryptography.Asn1
{
	public class DerEnumerated : Asn1Object
	{
		private readonly byte[] bytes;

		public static DerEnumerated GetInstance(object obj)
		{
			if (obj == null || obj is DerEnumerated)
			{
				return (DerEnumerated)obj;
			}
			if (obj is Asn1OctetString)
			{
				return new DerEnumerated(((Asn1OctetString)obj).GetOctets());
			}
			if (obj is Asn1TaggedObject)
			{
				return DerEnumerated.GetInstance(((Asn1TaggedObject)obj).GetObject());
			}
			throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name);
		}

		public static DerEnumerated GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return DerEnumerated.GetInstance(obj.GetObject());
		}

		public DerEnumerated(int value)
		{
			this.bytes = BigInteger.ValueOf((long)value).ToByteArray();
		}

		public DerEnumerated(BigInteger value)
		{
			this.bytes = value.ToByteArray();
		}

		public DerEnumerated(byte[] bytes)
		{
			this.bytes = bytes;
		}

		public BigInteger Value
		{
			get
			{
				return new BigInteger(this.bytes);
			}
		}

		internal override void Encode(DerOutputStream derOut)
		{
			derOut.WriteEncoded(10, this.bytes);
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerEnumerated derEnumerated = asn1Object as DerEnumerated;
			return derEnumerated != null && Arrays.AreEqual(this.bytes, derEnumerated.bytes);
		}

		protected override int Asn1GetHashCode()
		{
			return Arrays.GetHashCode(this.bytes);
		}
	}
}
