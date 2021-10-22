using System;
using System.Collections.Generic;
using System.IO;
using DNA.IO;
using DNA.Security.Cryptography.Crypto;
using DNA.Security.Cryptography.Crypto.Generators;
using DNA.Security.Cryptography.Crypto.Parameters;
using DNA.Security.Cryptography.Math;
using DNA.Security.Cryptography.Security;

namespace DNA.Security.Cryptography
{
	public class RSAKey
	{
		private RsaPrivateCrtKeyParameters _privateKeyParams;

		private RsaKeyParameters _publicKeyParams;

		internal RsaKeyParameters Key
		{
			get
			{
				if (this._privateKeyParams != null)
				{
					return this._privateKeyParams;
				}
				return this._publicKeyParams;
			}
		}

		public bool IsPrivate
		{
			get
			{
				return this._privateKeyParams != null;
			}
		}

		public RSAKey PublicKey
		{
			get
			{
				if (this._privateKeyParams == null)
				{
					return this;
				}
				return new RSAKey(this._publicKeyParams);
			}
		}

		private RSAKey(RsaKeyParameters pubKey)
		{
			this._publicKeyParams = pubKey;
		}

		private RSAKey()
		{
		}

		public static RSAKey Generate(int size)
		{
			RSAKey rsakey = new RSAKey();
			RsaKeyPairGenerator rsaKeyPairGenerator = new RsaKeyPairGenerator();
			rsaKeyPairGenerator.Init(new KeyGenerationParameters(new SecureRandom(), size));
			AsymmetricCipherKeyPair asymmetricCipherKeyPair = rsaKeyPairGenerator.GenerateKeyPair();
			rsakey._privateKeyParams = (RsaPrivateCrtKeyParameters)asymmetricCipherKeyPair.Private;
			rsakey._publicKeyParams = (RsaKeyParameters)asymmetricCipherKeyPair.Public;
			return rsakey;
		}

		public override string ToString()
		{
			HTFDocument htfdocument = new HTFDocument();
			if (this._privateKeyParams != null)
			{
				htfdocument.Children.Add(new HTFElement("Modulus", this._privateKeyParams.Modulus.ToString(16)));
				htfdocument.Children.Add(new HTFElement("PublicExponent", this._privateKeyParams.PublicExponent.ToString(16)));
				htfdocument.Children.Add(new HTFElement("PrivateExponent", this._privateKeyParams.Exponent.ToString(16)));
				htfdocument.Children.Add(new HTFElement("P", this._privateKeyParams.P.ToString(16)));
				htfdocument.Children.Add(new HTFElement("Q", this._privateKeyParams.Q.ToString(16)));
				htfdocument.Children.Add(new HTFElement("DP", this._privateKeyParams.DP.ToString(16)));
				htfdocument.Children.Add(new HTFElement("dQ", this._privateKeyParams.DQ.ToString(16)));
				htfdocument.Children.Add(new HTFElement("qInv", this._privateKeyParams.QInv.ToString(16)));
			}
			else
			{
				htfdocument.Children.Add(new HTFElement("Modulus", this._publicKeyParams.Modulus.ToString(16)));
				htfdocument.Children.Add(new HTFElement("PublicExponent", this._publicKeyParams.Exponent.ToString(16)));
			}
			return htfdocument.ToString();
		}

		public static RSAKey Parse(string str)
		{
			RSAKey rsakey = new RSAKey();
			HTFDocument htfdocument = new HTFDocument();
			htfdocument.LoadFromString(str);
			Dictionary<string, BigInteger> dictionary = new Dictionary<string, BigInteger>();
			foreach (HTFElement htfelement in htfdocument.Children)
			{
				dictionary[htfelement.ID] = new BigInteger(htfelement.Value, 16);
			}
			if (dictionary.ContainsKey("PrivateExponent"))
			{
				rsakey._privateKeyParams = new RsaPrivateCrtKeyParameters(dictionary["Modulus"], dictionary["PublicExponent"], dictionary["PrivateExponent"], dictionary["P"], dictionary["Q"], dictionary["DP"], dictionary["dQ"], dictionary["qInv"]);
				rsakey._publicKeyParams = new RsaKeyParameters(false, rsakey._privateKeyParams.Modulus, rsakey._privateKeyParams.PublicExponent);
			}
			else
			{
				rsakey._publicKeyParams = new RsaKeyParameters(false, dictionary["Modulus"], dictionary["PublicExponent"]);
			}
			return rsakey;
		}

		private static void WriteBigInt(BinaryWriter writer, BigInteger bigInt)
		{
			byte[] array = bigInt.ToByteArray();
			writer.Write(array.Length);
			writer.Write(array);
		}

		private static BigInteger ReadBigInt(BinaryReader reader)
		{
			int count = reader.ReadInt32();
			return new BigInteger(reader.ReadBytes(count));
		}

		public void Write(BinaryWriter writer)
		{
			if (this._privateKeyParams != null)
			{
				writer.Write(true);
				RSAKey.WriteBigInt(writer, this._privateKeyParams.Modulus);
				RSAKey.WriteBigInt(writer, this._privateKeyParams.PublicExponent);
				RSAKey.WriteBigInt(writer, this._privateKeyParams.Exponent);
				RSAKey.WriteBigInt(writer, this._privateKeyParams.P);
				RSAKey.WriteBigInt(writer, this._privateKeyParams.Q);
				RSAKey.WriteBigInt(writer, this._privateKeyParams.DP);
				RSAKey.WriteBigInt(writer, this._privateKeyParams.DQ);
				RSAKey.WriteBigInt(writer, this._privateKeyParams.QInv);
				return;
			}
			writer.Write(false);
			RSAKey.WriteBigInt(writer, this._publicKeyParams.Modulus);
			RSAKey.WriteBigInt(writer, this._publicKeyParams.Exponent);
		}

		public static RSAKey Read(BinaryReader reader)
		{
			RSAKey rsakey = new RSAKey();
			if (reader.ReadBoolean())
			{
				rsakey._privateKeyParams = new RsaPrivateCrtKeyParameters(RSAKey.ReadBigInt(reader), RSAKey.ReadBigInt(reader), RSAKey.ReadBigInt(reader), RSAKey.ReadBigInt(reader), RSAKey.ReadBigInt(reader), RSAKey.ReadBigInt(reader), RSAKey.ReadBigInt(reader), RSAKey.ReadBigInt(reader));
				rsakey._publicKeyParams = new RsaKeyParameters(false, rsakey._privateKeyParams.Modulus, rsakey._privateKeyParams.PublicExponent);
			}
			else
			{
				rsakey._publicKeyParams = new RsaKeyParameters(false, RSAKey.ReadBigInt(reader), RSAKey.ReadBigInt(reader));
			}
			return rsakey;
		}

		public byte[] ToByteArray()
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			this.Write(binaryWriter);
			binaryWriter.Flush();
			return memoryStream.ToArray();
		}

		public static RSAKey FromByteArray(byte[] data)
		{
			MemoryStream input = new MemoryStream(data);
			BinaryReader reader = new BinaryReader(input);
			return RSAKey.Read(reader);
		}
	}
}
