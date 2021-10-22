using System;
using DNA.Security.Cryptography.Crypto.Parameters;
using DNA.Security.Cryptography.Math;
using DNA.Security.Cryptography.Security;

namespace DNA.Security.Cryptography.Crypto.Engines
{
	internal class RsaCoreEngine
	{
		private RsaKeyParameters key;

		private bool forEncryption;

		private int bitSize;

		public void Init(bool forEncryption, ICipherParameters parameters)
		{
			if (parameters is ParametersWithRandom)
			{
				parameters = ((ParametersWithRandom)parameters).Parameters;
			}
			if (!(parameters is RsaKeyParameters))
			{
				throw new InvalidKeyException("Not an RSA key");
			}
			this.key = (RsaKeyParameters)parameters;
			this.forEncryption = forEncryption;
			this.bitSize = this.key.Modulus.BitLength;
		}

		public int GetInputBlockSize()
		{
			if (this.forEncryption)
			{
				return (this.bitSize - 1) / 8;
			}
			return (this.bitSize + 7) / 8;
		}

		public int GetOutputBlockSize()
		{
			if (this.forEncryption)
			{
				return (this.bitSize + 7) / 8;
			}
			return (this.bitSize - 1) / 8;
		}

		public BigInteger ConvertInput(byte[] inBuf, int inOff, int inLen)
		{
			int num = (this.bitSize + 7) / 8;
			if (inLen > num)
			{
				throw new DataLengthException("input too large for RSA cipher.");
			}
			BigInteger bigInteger = new BigInteger(1, inBuf, inOff, inLen);
			if (bigInteger.CompareTo(this.key.Modulus) >= 0)
			{
				throw new DataLengthException("input too large for RSA cipher.");
			}
			return bigInteger;
		}

		public byte[] ConvertOutput(BigInteger result)
		{
			byte[] array = result.ToByteArrayUnsigned();
			if (this.forEncryption)
			{
				int outputBlockSize = this.GetOutputBlockSize();
				if (array.Length < outputBlockSize)
				{
					byte[] array2 = new byte[outputBlockSize];
					array.CopyTo(array2, array2.Length - array.Length);
					array = array2;
				}
			}
			return array;
		}

		public BigInteger ProcessBlock(BigInteger input)
		{
			if (this.key is RsaPrivateCrtKeyParameters)
			{
				RsaPrivateCrtKeyParameters rsaPrivateCrtKeyParameters = (RsaPrivateCrtKeyParameters)this.key;
				BigInteger p = rsaPrivateCrtKeyParameters.P;
				BigInteger q = rsaPrivateCrtKeyParameters.Q;
				BigInteger dp = rsaPrivateCrtKeyParameters.DP;
				BigInteger dq = rsaPrivateCrtKeyParameters.DQ;
				BigInteger qinv = rsaPrivateCrtKeyParameters.QInv;
				BigInteger bigInteger = input.Remainder(p).ModPow(dp, p);
				BigInteger bigInteger2 = input.Remainder(q).ModPow(dq, q);
				BigInteger bigInteger3 = bigInteger.Subtract(bigInteger2);
				bigInteger3 = bigInteger3.Multiply(qinv);
				bigInteger3 = bigInteger3.Mod(p);
				BigInteger bigInteger4 = bigInteger3.Multiply(q);
				return bigInteger4.Add(bigInteger2);
			}
			return input.ModPow(this.key.Exponent, this.key.Modulus);
		}
	}
}
