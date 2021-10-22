using System;
using DNA.Security.Cryptography.Crypto.Parameters;
using DNA.Security.Cryptography.Security;
using DNA.Security.Cryptography.Utilities;

namespace DNA.Security.Cryptography.Crypto.Encodings
{
	public class Pkcs1Encoding : IAsymmetricBlockCipher
	{
		public const string StrictLengthEnabledProperty = "DNA.Security.Cryptography.Pkcs1.Strict";

		private const int HeaderLength = 10;

		private static readonly bool[] strictLengthEnabled;

		private SecureRandom random;

		private IAsymmetricBlockCipher engine;

		private bool forEncryption;

		private bool forPrivateKey;

		private bool useStrictLength;

		public static bool StrictLengthEnabled
		{
			get
			{
				return Pkcs1Encoding.strictLengthEnabled[0];
			}
			set
			{
				Pkcs1Encoding.strictLengthEnabled[0] = value;
			}
		}

		static Pkcs1Encoding()
		{
			string environmentVariable = Platform.GetEnvironmentVariable("DNA.Security.Cryptography.Pkcs1.Strict");
			Pkcs1Encoding.strictLengthEnabled = new bool[]
			{
				environmentVariable == null || environmentVariable.Equals("true")
			};
		}

		public Pkcs1Encoding(IAsymmetricBlockCipher cipher)
		{
			this.engine = cipher;
			this.useStrictLength = Pkcs1Encoding.StrictLengthEnabled;
		}

		public IAsymmetricBlockCipher GetUnderlyingCipher()
		{
			return this.engine;
		}

		public string AlgorithmName
		{
			get
			{
				return this.engine.AlgorithmName + "/PKCS1Padding";
			}
		}

		public void Init(bool forEncryption, ICipherParameters parameters)
		{
			AsymmetricKeyParameter asymmetricKeyParameter;
			if (parameters is ParametersWithRandom)
			{
				ParametersWithRandom parametersWithRandom = (ParametersWithRandom)parameters;
				this.random = parametersWithRandom.Random;
				asymmetricKeyParameter = (AsymmetricKeyParameter)parametersWithRandom.Parameters;
			}
			else
			{
				this.random = new SecureRandom();
				asymmetricKeyParameter = (AsymmetricKeyParameter)parameters;
			}
			this.engine.Init(forEncryption, parameters);
			this.forPrivateKey = asymmetricKeyParameter.IsPrivate;
			this.forEncryption = forEncryption;
		}

		public int GetInputBlockSize()
		{
			int inputBlockSize = this.engine.GetInputBlockSize();
			if (!this.forEncryption)
			{
				return inputBlockSize;
			}
			return inputBlockSize - 10;
		}

		public int GetOutputBlockSize()
		{
			int outputBlockSize = this.engine.GetOutputBlockSize();
			if (!this.forEncryption)
			{
				return outputBlockSize - 10;
			}
			return outputBlockSize;
		}

		public byte[] ProcessBlock(byte[] input, int inOff, int length)
		{
			if (!this.forEncryption)
			{
				return this.DecodeBlock(input, inOff, length);
			}
			return this.EncodeBlock(input, inOff, length);
		}

		private byte[] EncodeBlock(byte[] input, int inOff, int inLen)
		{
			if (inLen > this.GetInputBlockSize())
			{
				throw new ArgumentException("input data too large", "inLen");
			}
			byte[] array = new byte[this.engine.GetInputBlockSize()];
			if (this.forPrivateKey)
			{
				array[0] = 1;
				for (int num = 1; num != array.Length - inLen - 1; num++)
				{
					array[num] = byte.MaxValue;
				}
			}
			else
			{
				this.random.NextBytes(array);
				array[0] = 2;
				for (int num2 = 1; num2 != array.Length - inLen - 1; num2++)
				{
					while (array[num2] == 0)
					{
						array[num2] = (byte)this.random.NextInt();
					}
				}
			}
			array[array.Length - inLen - 1] = 0;
			Array.Copy(input, inOff, array, array.Length - inLen, inLen);
			return this.engine.ProcessBlock(array, 0, array.Length);
		}

		private byte[] DecodeBlock(byte[] input, int inOff, int inLen)
		{
			byte[] array = this.engine.ProcessBlock(input, inOff, inLen);
			if (array.Length < this.GetOutputBlockSize())
			{
				throw new InvalidCipherTextException("block truncated");
			}
			byte b = array[0];
			if (b != 1 && b != 2)
			{
				throw new InvalidCipherTextException("unknown block type");
			}
			if (this.useStrictLength && array.Length != this.engine.GetOutputBlockSize())
			{
				throw new InvalidCipherTextException("block incorrect size");
			}
			int num;
			for (num = 1; num != array.Length; num++)
			{
				byte b2 = array[num];
				if (b2 == 0)
				{
					break;
				}
				if (b == 1 && b2 != 255)
				{
					throw new InvalidCipherTextException("block padding incorrect");
				}
			}
			num++;
			if (num >= array.Length || num < 10)
			{
				throw new InvalidCipherTextException("no data in block");
			}
			byte[] array2 = new byte[array.Length - num];
			Array.Copy(array, num, array2, 0, array2.Length);
			return array2;
		}
	}
}
