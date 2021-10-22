using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace DNA.Security.Cryptography.Asn1
{
	public class BerOctetString : DerOctetString, IEnumerable
	{
		private const int MaxLength = 1000;

		private readonly IEnumerable octs;

		private static byte[] ToBytes(IEnumerable octs)
		{
			MemoryStream memoryStream = new MemoryStream();
			foreach (object obj in octs)
			{
				DerOctetString derOctetString = (DerOctetString)obj;
				byte[] octets = derOctetString.GetOctets();
				memoryStream.Write(octets, 0, octets.Length);
			}
			return memoryStream.ToArray();
		}

		public BerOctetString(byte[] str) : base(str)
		{
		}

		public BerOctetString(IEnumerable octets) : base(BerOctetString.ToBytes(octets))
		{
			this.octs = octets;
		}

		public BerOctetString(Asn1Object obj) : base(obj)
		{
		}

		public BerOctetString(Asn1Encodable obj) : base(obj.ToAsn1Object())
		{
		}

		public override byte[] GetOctets()
		{
			return this.str;
		}

		public IEnumerator GetEnumerator()
		{
			if (this.octs == null)
			{
				return this.GenerateOcts().GetEnumerator();
			}
			return this.octs.GetEnumerator();
		}

		[Obsolete("Use GetEnumerator() instead")]
		public IEnumerator GetObjects()
		{
			return this.GetEnumerator();
		}

		private List<DerOctetString> GenerateOcts()
		{
			List<DerOctetString> list = new List<DerOctetString>();
			for (int i = 0; i < this.str.Length; i += 1000)
			{
				int num = Math.Min(this.str.Length, i + 1000);
				byte[] array = new byte[num - i];
				Array.Copy(this.str, i, array, 0, array.Length);
				list.Add(new DerOctetString(array));
			}
			return list;
		}

		internal override void Encode(DerOutputStream derOut)
		{
			if (derOut is Asn1OutputStream || derOut is BerOutputStream)
			{
				derOut.WriteByte(36);
				derOut.WriteByte(128);
				foreach (object obj in this)
				{
					DerOctetString obj2 = (DerOctetString)obj;
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
