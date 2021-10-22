using System;
using DNA.Security.Cryptography.Math;
using DNA.Security.Cryptography.Security;

namespace DNA.Security.Cryptography.Crypto.Parameters
{
	public class RsaKeyGenerationParameters : KeyGenerationParameters
	{
		private readonly BigInteger publicExponent;

		private readonly int certainty;

		public RsaKeyGenerationParameters(BigInteger publicExponent, SecureRandom random, int strength, int certainty) : base(random, strength)
		{
			this.publicExponent = publicExponent;
			this.certainty = certainty;
		}

		public BigInteger PublicExponent
		{
			get
			{
				return this.publicExponent;
			}
		}

		public int Certainty
		{
			get
			{
				return this.certainty;
			}
		}

		public override bool Equals(object obj)
		{
			RsaKeyGenerationParameters rsaKeyGenerationParameters = obj as RsaKeyGenerationParameters;
			return rsaKeyGenerationParameters != null && this.certainty == rsaKeyGenerationParameters.certainty && this.publicExponent.Equals(rsaKeyGenerationParameters.publicExponent);
		}

		public override int GetHashCode()
		{
			return this.certainty.GetHashCode() ^ this.publicExponent.GetHashCode();
		}
	}
}
