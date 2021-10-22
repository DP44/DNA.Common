using System;
using System.IO;
using DNA.Security.Cryptography.Utilities;

namespace DNA.Security.Cryptography.Asn1
{
	public class BerTaggedObjectParser : Asn1TaggedObjectParser, IAsn1Convertible
	{
		private int _baseTag;

		private int _tagNumber;

		private Stream _contentStream;

		private bool _indefiniteLength;

		internal BerTaggedObjectParser(int baseTag, int tagNumber, Stream contentStream)
		{
			if (!contentStream.CanRead)
			{
				throw new ArgumentException("Expected stream to be readable", "contentStream");
			}
			this._baseTag = baseTag;
			this._tagNumber = tagNumber;
			this._contentStream = contentStream;
			this._indefiniteLength = (contentStream is IndefiniteLengthInputStream);
		}

		public bool IsConstructed
		{
			get
			{
				return (this._baseTag & 32) != 0;
			}
		}

		public int TagNo
		{
			get
			{
				return this._tagNumber;
			}
		}

		public IAsn1Convertible GetObjectParser(int tag, bool isExplicit)
		{
			if (isExplicit)
			{
				return new Asn1StreamParser(this._contentStream).ReadObject();
			}
			if (tag != 4)
			{
				switch (tag)
				{
				case 16:
					if (this._indefiniteLength)
					{
						return new BerSequenceParser(new Asn1StreamParser(this._contentStream));
					}
					return new DerSequenceParser(new Asn1StreamParser(this._contentStream));
				case 17:
					if (this._indefiniteLength)
					{
						return new BerSetParser(new Asn1StreamParser(this._contentStream));
					}
					return new DerSetParser(new Asn1StreamParser(this._contentStream));
				default:
					throw Platform.CreateNotImplementedException("implicit tagging");
				}
			}
			else
			{
				if (this._indefiniteLength || this.IsConstructed)
				{
					return new BerOctetStringParser(new Asn1StreamParser(this._contentStream));
				}
				return new DerOctetStringParser((DefiniteLengthInputStream)this._contentStream);
			}
		}

		private Asn1EncodableVector rLoadVector(Stream inStream)
		{
			Asn1EncodableVector result;
			try
			{
				result = new Asn1StreamParser(inStream).ReadVector();
			}
			catch (IOException ex)
			{
				throw new InvalidOperationException(ex.Message, ex);
			}
			return result;
		}

		public Asn1Object ToAsn1Object()
		{
			if (this._indefiniteLength)
			{
				Asn1EncodableVector asn1EncodableVector = this.rLoadVector(this._contentStream);
				if (asn1EncodableVector.Count != 1)
				{
					return new BerTaggedObject(false, this._tagNumber, BerSequence.FromVector(asn1EncodableVector));
				}
				return new BerTaggedObject(true, this._tagNumber, asn1EncodableVector[0]);
			}
			else
			{
				if (!this.IsConstructed)
				{
					Asn1Object result;
					try
					{
						DefiniteLengthInputStream definiteLengthInputStream = (DefiniteLengthInputStream)this._contentStream;
						result = new DerTaggedObject(false, this._tagNumber, new DerOctetString(definiteLengthInputStream.ToArray()));
					}
					catch (IOException ex)
					{
						throw new InvalidOperationException(ex.Message, ex);
					}
					return result;
				}
				Asn1EncodableVector asn1EncodableVector2 = this.rLoadVector(this._contentStream);
				if (asn1EncodableVector2.Count != 1)
				{
					return new DerTaggedObject(false, this._tagNumber, DerSequence.FromVector(asn1EncodableVector2));
				}
				return new DerTaggedObject(true, this._tagNumber, asn1EncodableVector2[0]);
			}
		}
	}
}
