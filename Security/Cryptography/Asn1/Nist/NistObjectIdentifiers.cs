using System;

namespace DNA.Security.Cryptography.Asn1.Nist
{
	public sealed class NistObjectIdentifiers
	{
		public static readonly DerObjectIdentifier NistAlgorithm = new DerObjectIdentifier("2.16.840.1.101.3.4");

		public static readonly DerObjectIdentifier IdSha256 = new DerObjectIdentifier(NistObjectIdentifiers.NistAlgorithm + ".2.1");

		public static readonly DerObjectIdentifier IdSha384 = new DerObjectIdentifier(NistObjectIdentifiers.NistAlgorithm + ".2.2");

		public static readonly DerObjectIdentifier IdSha512 = new DerObjectIdentifier(NistObjectIdentifiers.NistAlgorithm + ".2.3");

		public static readonly DerObjectIdentifier IdSha224 = new DerObjectIdentifier(NistObjectIdentifiers.NistAlgorithm + ".2.4");

		public static readonly DerObjectIdentifier Aes = new DerObjectIdentifier(NistObjectIdentifiers.NistAlgorithm + ".1");

		public static readonly DerObjectIdentifier IdAes128Ecb = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".1");

		public static readonly DerObjectIdentifier IdAes128Cbc = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".2");

		public static readonly DerObjectIdentifier IdAes128Ofb = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".3");

		public static readonly DerObjectIdentifier IdAes128Cfb = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".4");

		public static readonly DerObjectIdentifier IdAes128Wrap = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".5");

		public static readonly DerObjectIdentifier IdAes192Ecb = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".21");

		public static readonly DerObjectIdentifier IdAes192Cbc = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".22");

		public static readonly DerObjectIdentifier IdAes192Ofb = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".23");

		public static readonly DerObjectIdentifier IdAes192Cfb = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".24");

		public static readonly DerObjectIdentifier IdAes192Wrap = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".25");

		public static readonly DerObjectIdentifier IdAes256Ecb = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".41");

		public static readonly DerObjectIdentifier IdAes256Cbc = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".42");

		public static readonly DerObjectIdentifier IdAes256Ofb = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".43");

		public static readonly DerObjectIdentifier IdAes256Cfb = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".44");

		public static readonly DerObjectIdentifier IdAes256Wrap = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".45");

		public static readonly DerObjectIdentifier IdDsaWithSha2 = new DerObjectIdentifier(NistObjectIdentifiers.NistAlgorithm + ".3");

		public static readonly DerObjectIdentifier DsaWithSha224 = new DerObjectIdentifier(NistObjectIdentifiers.IdDsaWithSha2 + ".1");

		public static readonly DerObjectIdentifier DsaWithSha256 = new DerObjectIdentifier(NistObjectIdentifiers.IdDsaWithSha2 + ".2");

		public static readonly DerObjectIdentifier DsaWithSha384 = new DerObjectIdentifier(NistObjectIdentifiers.IdDsaWithSha2 + ".3");

		public static readonly DerObjectIdentifier DsaWithSha512 = new DerObjectIdentifier(NistObjectIdentifiers.IdDsaWithSha2 + ".4");

		private NistObjectIdentifiers()
		{
		}
	}
}
