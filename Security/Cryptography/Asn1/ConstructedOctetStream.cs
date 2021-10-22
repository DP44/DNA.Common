using System;
using System.IO;
using DNA.Security.Cryptography.Utilities.IO;

namespace DNA.Security.Cryptography.Asn1
{
	internal class ConstructedOctetStream : BaseInputStream
	{
		private readonly Asn1StreamParser _parser;

		private bool _first = true;

		private Stream _currentStream;

		internal ConstructedOctetStream(Asn1StreamParser parser)
		{
			this._parser = parser;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (this._currentStream == null)
			{
				if (!this._first)
				{
					return 0;
				}
				Asn1OctetStringParser asn1OctetStringParser = (Asn1OctetStringParser)this._parser.ReadObject();
				if (asn1OctetStringParser == null)
				{
					return 0;
				}
				this._first = false;
				this._currentStream = asn1OctetStringParser.GetOctetStream();
			}
			int num = 0;
			for (;;)
			{
				int num2 = this._currentStream.Read(buffer, offset + num, count - num);
				if (num2 > 0)
				{
					num += num2;
					if (num == count)
					{
						break;
					}
				}
				else
				{
					Asn1OctetStringParser asn1OctetStringParser2 = (Asn1OctetStringParser)this._parser.ReadObject();
					if (asn1OctetStringParser2 == null)
					{
						goto Block_6;
					}
					this._currentStream = asn1OctetStringParser2.GetOctetStream();
				}
			}
			return num;
			Block_6:
			this._currentStream = null;
			return num;
		}

		public override int ReadByte()
		{
			if (this._currentStream == null)
			{
				if (!this._first)
				{
					return 0;
				}
				Asn1OctetStringParser asn1OctetStringParser = (Asn1OctetStringParser)this._parser.ReadObject();
				if (asn1OctetStringParser == null)
				{
					return 0;
				}
				this._first = false;
				this._currentStream = asn1OctetStringParser.GetOctetStream();
			}
			int num;
			for (;;)
			{
				num = this._currentStream.ReadByte();
				if (num >= 0)
				{
					break;
				}
				Asn1OctetStringParser asn1OctetStringParser2 = (Asn1OctetStringParser)this._parser.ReadObject();
				if (asn1OctetStringParser2 == null)
				{
					goto Block_5;
				}
				this._currentStream = asn1OctetStringParser2.GetOctetStream();
			}
			return num;
			Block_5:
			this._currentStream = null;
			return -1;
		}
	}
}
