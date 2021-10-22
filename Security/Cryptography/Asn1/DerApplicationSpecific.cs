using System;
using System.IO;
using DNA.Security.Cryptography.Utilities;

namespace DNA.Security.Cryptography.Asn1
{
	public class DerApplicationSpecific : Asn1Object
	{
		private readonly bool isConstructed;

		private readonly int tag;

		private readonly byte[] octets;

		internal DerApplicationSpecific(bool isConstructed, int tag, byte[] octets)
		{
			this.isConstructed = isConstructed;
			this.tag = tag;
			this.octets = octets;
		}

		public DerApplicationSpecific(int tag, byte[] octets) : this(false, tag, octets)
		{
		}

		public DerApplicationSpecific(int tag, Asn1Encodable obj) : this(true, tag, obj)
		{
		}

		public DerApplicationSpecific(bool isExplicit, int tag, Asn1Encodable obj)
		{
			byte[] derEncoded = obj.GetDerEncoded();
			this.isConstructed = isExplicit;
			this.tag = tag;
			if (isExplicit)
			{
				this.octets = derEncoded;
				return;
			}
			int lengthOfLength = this.GetLengthOfLength(derEncoded);
			byte[] array = new byte[derEncoded.Length - lengthOfLength];
			Array.Copy(derEncoded, lengthOfLength, array, 0, array.Length);
			this.octets = array;
		}

		public DerApplicationSpecific(int tagNo, Asn1EncodableVector vec)
		{
			this.tag = tagNo;
			this.isConstructed = true;
			MemoryStream memoryStream = new MemoryStream();
			for (int num = 0; num != vec.Count; num++)
			{
				try
				{
					byte[] encoded = vec[num].GetEncoded();
					memoryStream.Write(encoded, 0, encoded.Length);
				}
				catch (IOException innerException)
				{
					throw new InvalidOperationException("malformed object", innerException);
				}
			}
			this.octets = memoryStream.ToArray();
		}

		private int GetLengthOfLength(byte[] data)
		{
			int num = 2;
			while ((data[num - 1] & 128) != 0)
			{
				num++;
			}
			return num;
		}

		public bool IsConstructed()
		{
			return this.isConstructed;
		}

		public byte[] GetContents()
		{
			return this.octets;
		}

		public int ApplicationTag
		{
			get
			{
				return this.tag;
			}
		}

		public Asn1Object GetObject()
		{
			return Asn1Object.FromByteArray(this.GetContents());
		}

		public Asn1Object GetObject(int derTagNo)
		{
			if (derTagNo >= 31)
			{
				throw new IOException("unsupported tag number");
			}
			byte[] encoded = base.GetEncoded();
			byte[] array = this.ReplaceTagNumber(derTagNo, encoded);
			if ((encoded[0] & 32) != 0)
			{
				byte[] array2 = array;
				int num = 0;
				array2[num] |= 32;
			}
			return Asn1Object.FromByteArray(array);
		}

		internal override void Encode(DerOutputStream derOut)
		{
			int num = 64;
			if (this.isConstructed)
			{
				num |= 32;
			}
			derOut.WriteEncoded(num, this.tag, this.octets);
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerApplicationSpecific derApplicationSpecific = asn1Object as DerApplicationSpecific;
			return derApplicationSpecific != null && (this.isConstructed == derApplicationSpecific.isConstructed && this.tag == derApplicationSpecific.tag) && Arrays.AreEqual(this.octets, derApplicationSpecific.octets);
		}

		protected override int Asn1GetHashCode()
		{
			return this.isConstructed.GetHashCode() ^ this.tag.GetHashCode() ^ Arrays.GetHashCode(this.octets);
		}

		private byte[] ReplaceTagNumber(int newTag, byte[] input)
		{
			int num = (int)(input[0] & 31);
			int num2 = 1;
			if (num == 31)
			{
				num = 0;
				int num3 = (int)(input[num2++] & byte.MaxValue);
				if ((num3 & 127) == 0)
				{
					throw new InvalidOperationException("corrupted stream - invalid high tag number found");
				}
				while (num3 >= 0 && (num3 & 128) != 0)
				{
					num |= (num3 & 127);
					num <<= 7;
					num3 = (int)(input[num2++] & byte.MaxValue);
				}
				num |= (num3 & 127);
			}
			byte[] array = new byte[input.Length - num2 + 1];
			Array.Copy(input, num2, array, 1, array.Length - 1);
			array[0] = (byte)newTag;
			return array;
		}
	}
}
