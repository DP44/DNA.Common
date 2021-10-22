using System;
using DNA.Security.Cryptography.Utilities;

namespace DNA.Security.Cryptography.Asn1
{
	public class DerT61String : DerStringBase
	{
		private readonly string str;

		public static DerT61String GetInstance(object obj)
		{
			if (obj == null || obj is DerT61String)
			{
				return (DerT61String)obj;
			}
			if (obj is Asn1OctetString)
			{
				return new DerT61String(((Asn1OctetString)obj).GetOctets());
			}
			if (obj is Asn1TaggedObject)
			{
				return DerT61String.GetInstance(((Asn1TaggedObject)obj).GetObject());
			}
			throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name);
		}

		public static DerT61String GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return DerT61String.GetInstance(obj.GetObject());
		}

		public DerT61String(byte[] str) : this(Strings.FromByteArray(str))
		{
		}

		public DerT61String(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			this.str = str;
		}

		public override string GetString()
		{
			return this.str;
		}

		internal override void Encode(DerOutputStream derOut)
		{
			derOut.WriteEncoded(20, this.GetOctets());
		}

		public byte[] GetOctets()
		{
			return Strings.ToByteArray(this.str);
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerT61String derT61String = asn1Object as DerT61String;
			return derT61String != null && this.str.Equals(derT61String.str);
		}
	}
}
