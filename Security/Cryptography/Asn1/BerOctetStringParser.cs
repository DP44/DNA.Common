using System;
using System.IO;
using DNA.Security.Cryptography.Utilities.IO;

namespace DNA.Security.Cryptography.Asn1
{
	public class BerOctetStringParser : Asn1OctetStringParser, IAsn1Convertible
	{
		private readonly Asn1StreamParser _parser;

		internal BerOctetStringParser(Asn1StreamParser parser)
		{
			this._parser = parser;
		}

		public Stream GetOctetStream()
		{
			return new ConstructedOctetStream(this._parser);
		}

		public Asn1Object ToAsn1Object()
		{
			Asn1Object result;
			try
			{
				result = new BerOctetString(Streams.ReadAll(this.GetOctetStream()));
			}
			catch (IOException ex)
			{
				throw new InvalidOperationException("IOException converting stream to byte array: " + ex.Message, ex);
			}
			return result;
		}
	}
}
