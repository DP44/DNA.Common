using System;
using DNA.Security.Cryptography.Math;

namespace DNA.Security.Cryptography.Crypto.Parameters
{
	public class RsaKeyParameters : AsymmetricKeyParameter
	{
		private readonly BigInteger modulus;

		private readonly BigInteger exponent;

		public RsaKeyParameters(bool isPrivate, BigInteger modulus, BigInteger exponent) : base(isPrivate)
		{
			this.modulus = modulus;
			this.exponent = exponent;
		}

		public BigInteger Modulus
		{
			get
			{
				return this.modulus;
			}
		}

		public BigInteger Exponent
		{
			get
			{
				return this.exponent;
			}
		}

		public override bool Equals(object obj)
		{
			RsaKeyParameters rsaKeyParameters = obj as RsaKeyParameters;
			return rsaKeyParameters != null && (rsaKeyParameters.IsPrivate == base.IsPrivate && rsaKeyParameters.Modulus.Equals(this.modulus)) && rsaKeyParameters.Exponent.Equals(this.exponent);
		}

		public override int GetHashCode()
		{
			return this.modulus.GetHashCode() ^ this.exponent.GetHashCode() ^ base.IsPrivate.GetHashCode();
		}
	}
}
