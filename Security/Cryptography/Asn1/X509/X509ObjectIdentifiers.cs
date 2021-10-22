﻿using System;

namespace DNA.Security.Cryptography.Asn1.X509
{
	public abstract class X509ObjectIdentifiers
	{
		internal const string ID = "2.5.4";

		public static readonly DerObjectIdentifier CommonName = new DerObjectIdentifier("2.5.4.3");

		public static readonly DerObjectIdentifier CountryName = new DerObjectIdentifier("2.5.4.6");

		public static readonly DerObjectIdentifier LocalityName = new DerObjectIdentifier("2.5.4.7");

		public static readonly DerObjectIdentifier StateOrProvinceName = new DerObjectIdentifier("2.5.4.8");

		public static readonly DerObjectIdentifier Organization = new DerObjectIdentifier("2.5.4.10");

		public static readonly DerObjectIdentifier OrganizationalUnitName = new DerObjectIdentifier("2.5.4.11");

		public static readonly DerObjectIdentifier id_at_telephoneNumber = new DerObjectIdentifier("2.5.4.20");

		public static readonly DerObjectIdentifier id_at_name = new DerObjectIdentifier("2.5.4.41");

		public static readonly DerObjectIdentifier IdSha1 = new DerObjectIdentifier("1.3.14.3.2.26");

		public static readonly DerObjectIdentifier RipeMD160 = new DerObjectIdentifier("1.3.36.3.2.1");

		public static readonly DerObjectIdentifier RipeMD160WithRsaEncryption = new DerObjectIdentifier("1.3.36.3.3.1.2");

		public static readonly DerObjectIdentifier IdEARsa = new DerObjectIdentifier("2.5.8.1.1");

		public static readonly DerObjectIdentifier IdPkix = new DerObjectIdentifier("1.3.6.1.5.5.7");

		public static readonly DerObjectIdentifier IdPE = new DerObjectIdentifier(X509ObjectIdentifiers.IdPkix + ".1");

		public static readonly DerObjectIdentifier IdAD = new DerObjectIdentifier(X509ObjectIdentifiers.IdPkix + ".48");

		public static readonly DerObjectIdentifier IdADCAIssuers = new DerObjectIdentifier(X509ObjectIdentifiers.IdAD + ".2");

		public static readonly DerObjectIdentifier IdADOcsp = new DerObjectIdentifier(X509ObjectIdentifiers.IdAD + ".1");

		public static readonly DerObjectIdentifier OcspAccessMethod = X509ObjectIdentifiers.IdADOcsp;

		public static readonly DerObjectIdentifier CrlAccessMethod = X509ObjectIdentifiers.IdADCAIssuers;
	}
}
