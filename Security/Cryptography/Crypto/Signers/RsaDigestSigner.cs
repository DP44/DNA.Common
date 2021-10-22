using System;
using System.Collections.Generic;
using DNA.Security.Cryptography.Asn1;
using DNA.Security.Cryptography.Asn1.Nist;
using DNA.Security.Cryptography.Asn1.Pkcs;
using DNA.Security.Cryptography.Asn1.TeleTrust;
using DNA.Security.Cryptography.Asn1.X509;
using DNA.Security.Cryptography.Crypto.Encodings;
using DNA.Security.Cryptography.Crypto.Engines;
using DNA.Security.Cryptography.Crypto.Parameters;
using DNA.Security.Cryptography.Security;

namespace DNA.Security.Cryptography.Crypto.Signers
{
	public class RsaDigestSigner : ISigner
	{
		private readonly IAsymmetricBlockCipher rsaEngine = new Pkcs1Encoding(new RsaBlindedEngine());

		private readonly AlgorithmIdentifier algId;

		private readonly IDigest digest;

		private bool forSigning;

		private static readonly Dictionary<string, DerObjectIdentifier> oidMap = new Dictionary<string, DerObjectIdentifier>();

		static RsaDigestSigner()
		{
			RsaDigestSigner.oidMap["RIPEMD128"] = TeleTrusTObjectIdentifiers.RipeMD128;
			RsaDigestSigner.oidMap["RIPEMD160"] = TeleTrusTObjectIdentifiers.RipeMD160;
			RsaDigestSigner.oidMap["RIPEMD256"] = TeleTrusTObjectIdentifiers.RipeMD256;
			RsaDigestSigner.oidMap["SHA-1"] = X509ObjectIdentifiers.IdSha1;
			RsaDigestSigner.oidMap["SHA-224"] = NistObjectIdentifiers.IdSha224;
			RsaDigestSigner.oidMap["SHA-256"] = NistObjectIdentifiers.IdSha256;
			RsaDigestSigner.oidMap["SHA-384"] = NistObjectIdentifiers.IdSha384;
			RsaDigestSigner.oidMap["SHA-512"] = NistObjectIdentifiers.IdSha512;
			RsaDigestSigner.oidMap["MD2"] = PkcsObjectIdentifiers.MD2;
			RsaDigestSigner.oidMap["MD4"] = PkcsObjectIdentifiers.MD4;
			RsaDigestSigner.oidMap["MD5"] = PkcsObjectIdentifiers.MD5;
		}

		public RsaDigestSigner(IDigest digest)
		{
			this.digest = digest;
			this.algId = new AlgorithmIdentifier(RsaDigestSigner.oidMap[digest.AlgorithmName], DerNull.Instance);
		}

		public string AlgorithmName
		{
			get
			{
				return this.digest.AlgorithmName + "withRSA";
			}
		}

		public void Init(bool forSigning, ICipherParameters parameters)
		{
			this.forSigning = forSigning;
			AsymmetricKeyParameter asymmetricKeyParameter;
			if (parameters is ParametersWithRandom)
			{
				asymmetricKeyParameter = (AsymmetricKeyParameter)((ParametersWithRandom)parameters).Parameters;
			}
			else
			{
				asymmetricKeyParameter = (AsymmetricKeyParameter)parameters;
			}
			if (forSigning && !asymmetricKeyParameter.IsPrivate)
			{
				throw new InvalidKeyException("Signing requires private key.");
			}
			if (!forSigning && asymmetricKeyParameter.IsPrivate)
			{
				throw new InvalidKeyException("Verification requires public key.");
			}
			this.Reset();
			this.rsaEngine.Init(forSigning, parameters);
		}

		public void Update(byte input)
		{
			this.digest.Update(input);
		}

		public void BlockUpdate(byte[] input, int inOff, int length)
		{
			this.digest.BlockUpdate(input, inOff, length);
		}

		public byte[] GenerateSignature()
		{
			if (!this.forSigning)
			{
				throw new InvalidOperationException("RsaDigestSigner not initialised for signature generation.");
			}
			byte[] array = new byte[this.digest.GetDigestSize()];
			this.digest.DoFinal(array, 0);
			byte[] array2 = this.DerEncode(array);
			return this.rsaEngine.ProcessBlock(array2, 0, array2.Length);
		}

		public bool VerifySignature(byte[] signature)
		{
			if (this.forSigning)
			{
				throw new InvalidOperationException("RsaDigestSigner not initialised for verification");
			}
			byte[] array = new byte[this.digest.GetDigestSize()];
			this.digest.DoFinal(array, 0);
			byte[] array2;
			byte[] array3;
			try
			{
				array2 = this.rsaEngine.ProcessBlock(signature, 0, signature.Length);
				array3 = this.DerEncode(array);
			}
			catch (Exception)
			{
				return false;
			}
			if (array2.Length == array3.Length)
			{
				for (int i = 0; i < array2.Length; i++)
				{
					if (array2[i] != array3[i])
					{
						return false;
					}
				}
			}
			else
			{
				if (array2.Length != array3.Length - 2)
				{
					return false;
				}
				int num = array2.Length - array.Length - 2;
				int num2 = array3.Length - array.Length - 2;
				byte[] array4 = array3;
				int num3 = 1;
				array4[num3] -= 2;
				byte[] array5 = array3;
				int num4 = 3;
				array5[num4] -= 2;
				for (int j = 0; j < array.Length; j++)
				{
					if (array2[num + j] != array3[num2 + j])
					{
						return false;
					}
				}
				for (int k = 0; k < num; k++)
				{
					if (array2[k] != array3[k])
					{
						return false;
					}
				}
			}
			return true;
		}

		public void Reset()
		{
			this.digest.Reset();
		}

		private byte[] DerEncode(byte[] hash)
		{
			DigestInfo digestInfo = new DigestInfo(this.algId, hash);
			return digestInfo.GetDerEncoded();
		}
	}
}
