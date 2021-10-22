using System;
using System.IO;

namespace DNA.Security.Cryptography.Asn1
{
	public class Asn1StreamParser
	{
		private readonly Stream _in;

		private readonly int _limit;

		public Asn1StreamParser(Stream inStream) : this(inStream, int.MaxValue)
		{
		}

		public Asn1StreamParser(Stream inStream, int limit)
		{
			if (!inStream.CanRead)
			{
				throw new ArgumentException("Expected stream to be readable", "inStream");
			}
			this._in = inStream;
			this._limit = limit;
		}

		public Asn1StreamParser(byte[] encoding) : this(new MemoryStream(encoding, false), encoding.Length)
		{
		}

		public virtual IAsn1Convertible ReadObject()
		{
			int num = this._in.ReadByte();
			if (num == -1)
			{
				return null;
			}
			this.Set00Check(false);
			int num2 = Asn1InputStream.ReadTagNumber(this._in, num);
			bool flag = (num & 32) != 0;
			int num3 = Asn1InputStream.ReadLength(this._in, this._limit);
			if (num3 < 0)
			{
				if (!flag)
				{
					throw new IOException("indefinite length primitive encoding encountered");
				}
				IndefiniteLengthInputStream indefiniteLengthInputStream = new IndefiniteLengthInputStream(this._in);
				if ((num & 64) != 0)
				{
					Asn1StreamParser parser = new Asn1StreamParser(indefiniteLengthInputStream);
					return new BerApplicationSpecificParser(num2, parser);
				}
				if ((num & 128) != 0)
				{
					return new BerTaggedObjectParser(num, num2, indefiniteLengthInputStream);
				}
				Asn1StreamParser parser2 = new Asn1StreamParser(indefiniteLengthInputStream);
				int num4 = num2;
				if (num4 == 4)
				{
					return new BerOctetStringParser(parser2);
				}
				switch (num4)
				{
				case 16:
					return new BerSequenceParser(parser2);
				case 17:
					return new BerSetParser(parser2);
				default:
					throw new IOException("unknown BER object encountered");
				}
			}
			else
			{
				DefiniteLengthInputStream definiteLengthInputStream = new DefiniteLengthInputStream(this._in, num3);
				if ((num & 64) != 0)
				{
					return new DerApplicationSpecific(flag, num2, definiteLengthInputStream.ToArray());
				}
				if ((num & 128) != 0)
				{
					return new BerTaggedObjectParser(num, num2, definiteLengthInputStream);
				}
				if (flag)
				{
					int num5 = num2;
					if (num5 == 4)
					{
						return new BerOctetStringParser(new Asn1StreamParser(definiteLengthInputStream));
					}
					switch (num5)
					{
					case 16:
						return new DerSequenceParser(new Asn1StreamParser(definiteLengthInputStream));
					case 17:
						return new DerSetParser(new Asn1StreamParser(definiteLengthInputStream));
					default:
						return new DerUnknownTag(true, num2, definiteLengthInputStream.ToArray());
					}
				}
				else
				{
					int num6 = num2;
					if (num6 == 4)
					{
						return new DerOctetStringParser(definiteLengthInputStream);
					}
					return Asn1InputStream.CreatePrimitiveDerObject(num2, definiteLengthInputStream.ToArray());
				}
			}
		}

		private void Set00Check(bool enabled)
		{
			if (this._in is IndefiniteLengthInputStream)
			{
				((IndefiniteLengthInputStream)this._in).SetEofOn00(enabled);
			}
		}

		internal Asn1EncodableVector ReadVector()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			IAsn1Convertible asn1Convertible;
			while ((asn1Convertible = this.ReadObject()) != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					asn1Convertible.ToAsn1Object()
				});
			}
			return asn1EncodableVector;
		}
	}
}
