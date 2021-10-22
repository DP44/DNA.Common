using System;
using System.Collections.Generic;
using System.IO;
using DNA.Security.Cryptography.Utilities;
using DNA.Security.Cryptography.Utilities.Encoders;
using DNA.Text;

namespace DNA.Security.Cryptography.Asn1
{
	public abstract class Asn1OctetString : Asn1Object, Asn1OctetStringParser, IAsn1Convertible
	{
		internal byte[] str;

		public static Asn1OctetString GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return Asn1OctetString.GetInstance(obj.GetObject());
		}

		public static Asn1OctetString GetInstance(object obj)
		{
			if (obj == null || obj is Asn1OctetString)
			{
				return (Asn1OctetString)obj;
			}
			if (obj is Asn1TaggedObject)
			{
				return Asn1OctetString.GetInstance(((Asn1TaggedObject)obj).GetObject());
			}
			if (obj is Asn1Sequence)
			{
				List<object> list = new List<object>();
				foreach (object item in ((Asn1Sequence)obj))
				{
					list.Add(item);
				}
				return new BerOctetString(list);
			}
			throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name);
		}

		internal Asn1OctetString(byte[] str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			this.str = str;
		}

		internal Asn1OctetString(Asn1Encodable obj)
		{
			try
			{
				this.str = obj.GetDerEncoded();
			}
			catch (IOException ex)
			{
				throw new ArgumentException("Error processing object : " + ex.ToString());
			}
		}

		public Stream GetOctetStream()
		{
			return new MemoryStream(this.str, false);
		}

		public Asn1OctetStringParser Parser
		{
			get
			{
				return this;
			}
		}

		public virtual byte[] GetOctets()
		{
			return this.str;
		}

		protected override int Asn1GetHashCode()
		{
			return Arrays.GetHashCode(this.GetOctets());
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerOctetString derOctetString = asn1Object as DerOctetString;
			return derOctetString != null && Arrays.AreEqual(this.GetOctets(), derOctetString.GetOctets());
		}

		public override string ToString()
		{
			byte[] array = Hex.Encode(this.str);
			return "#" + ASCIIEncoder.GetString(array, 0, array.Length);
		}
	}
}
