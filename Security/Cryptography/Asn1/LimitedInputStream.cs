using System;
using System.IO;
using DNA.Security.Cryptography.Utilities.IO;

namespace DNA.Security.Cryptography.Asn1
{
	internal abstract class LimitedInputStream : BaseInputStream
	{
		protected readonly Stream _in;

		internal LimitedInputStream(Stream inStream)
		{
			this._in = inStream;
		}

		protected virtual void SetParentEofDetect(bool on)
		{
			if (this._in is IndefiniteLengthInputStream)
			{
				((IndefiniteLengthInputStream)this._in).SetEofOn00(on);
			}
		}
	}
}
