using System;
using DNA.Security.Cryptography.Math;

namespace DNA.Security.Cryptography.Crypto.Parameters
{
	public class RsaPrivateCrtKeyParameters : RsaKeyParameters
	{
		private readonly BigInteger e;

		private readonly BigInteger p;

		private readonly BigInteger q;

		private readonly BigInteger dP;

		private readonly BigInteger dQ;

		private readonly BigInteger qInv;

		public RsaPrivateCrtKeyParameters(BigInteger modulus, BigInteger publicExponent, BigInteger privateExponent, BigInteger p, BigInteger q, BigInteger dP, BigInteger dQ, BigInteger qInv) : base(true, modulus, privateExponent)
		{
			this.e = publicExponent;
			this.p = p;
			this.q = q;
			this.dP = dP;
			this.dQ = dQ;
			this.qInv = qInv;
		}

		public BigInteger PublicExponent
		{
			get
			{
				return this.e;
			}
		}

		public BigInteger P
		{
			get
			{
				return this.p;
			}
		}

		public BigInteger Q
		{
			get
			{
				return this.q;
			}
		}

		public BigInteger DP
		{
			get
			{
				return this.dP;
			}
		}

		public BigInteger DQ
		{
			get
			{
				return this.dQ;
			}
		}

		public BigInteger QInv
		{
			get
			{
				return this.qInv;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			RsaPrivateCrtKeyParameters rsaPrivateCrtKeyParameters = obj as RsaPrivateCrtKeyParameters;
			return rsaPrivateCrtKeyParameters != null && (rsaPrivateCrtKeyParameters.DP.Equals(this.dP) && rsaPrivateCrtKeyParameters.DQ.Equals(this.dQ) && rsaPrivateCrtKeyParameters.Exponent.Equals(base.Exponent) && rsaPrivateCrtKeyParameters.Modulus.Equals(base.Modulus) && rsaPrivateCrtKeyParameters.P.Equals(this.p) && rsaPrivateCrtKeyParameters.Q.Equals(this.q) && rsaPrivateCrtKeyParameters.PublicExponent.Equals(this.e)) && rsaPrivateCrtKeyParameters.QInv.Equals(this.qInv);
		}

		public override int GetHashCode()
		{
			return this.DP.GetHashCode() ^ this.DQ.GetHashCode() ^ base.Exponent.GetHashCode() ^ base.Modulus.GetHashCode() ^ this.P.GetHashCode() ^ this.Q.GetHashCode() ^ this.PublicExponent.GetHashCode() ^ this.QInv.GetHashCode();
		}
	}
}
