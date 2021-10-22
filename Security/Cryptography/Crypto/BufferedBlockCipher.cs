﻿using System;
using DNA.Security.Cryptography.Crypto.Parameters;

namespace DNA.Security.Cryptography.Crypto
{
	public class BufferedBlockCipher : BufferedCipherBase
	{
		internal byte[] buf;

		internal int bufOff;

		internal bool forEncryption;

		internal IBlockCipher cipher;

		protected BufferedBlockCipher()
		{
		}

		public BufferedBlockCipher(IBlockCipher cipher)
		{
			if (cipher == null)
			{
				throw new ArgumentNullException("cipher");
			}
			this.cipher = cipher;
			this.buf = new byte[cipher.GetBlockSize()];
			this.bufOff = 0;
		}

		public override string AlgorithmName
		{
			get
			{
				return this.cipher.AlgorithmName;
			}
		}

		public override void Init(bool forEncryption, ICipherParameters parameters)
		{
			this.forEncryption = forEncryption;
			if (parameters is ParametersWithRandom)
			{
				parameters = ((ParametersWithRandom)parameters).Parameters;
			}
			this.Reset();
			this.cipher.Init(forEncryption, parameters);
		}

		public override int GetBlockSize()
		{
			return this.cipher.GetBlockSize();
		}

		public override int GetUpdateOutputSize(int length)
		{
			int num = length + this.bufOff;
			int num2 = num % this.buf.Length;
			return num - num2;
		}

		public override int GetOutputSize(int length)
		{
			return length + this.bufOff;
		}

		public override int ProcessByte(byte input, byte[] output, int outOff)
		{
			this.buf[this.bufOff++] = input;
			if (this.bufOff != this.buf.Length)
			{
				return 0;
			}
			if (outOff + this.buf.Length > output.Length)
			{
				throw new DataLengthException("output buffer too short");
			}
			this.bufOff = 0;
			return this.cipher.ProcessBlock(this.buf, 0, output, outOff);
		}

		public override byte[] ProcessByte(byte input)
		{
			int updateOutputSize = this.GetUpdateOutputSize(1);
			byte[] array = (updateOutputSize > 0) ? new byte[updateOutputSize] : null;
			int num = this.ProcessByte(input, array, 0);
			if (updateOutputSize > 0 && num < updateOutputSize)
			{
				byte[] array2 = new byte[num];
				Array.Copy(array, 0, array2, 0, num);
				array = array2;
			}
			return array;
		}

		public override byte[] ProcessBytes(byte[] input, int inOff, int length)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (length < 1)
			{
				return null;
			}
			int updateOutputSize = this.GetUpdateOutputSize(length);
			byte[] array = (updateOutputSize > 0) ? new byte[updateOutputSize] : null;
			int num = this.ProcessBytes(input, inOff, length, array, 0);
			if (updateOutputSize > 0 && num < updateOutputSize)
			{
				byte[] array2 = new byte[num];
				Array.Copy(array, 0, array2, 0, num);
				array = array2;
			}
			return array;
		}

		public override int ProcessBytes(byte[] input, int inOff, int length, byte[] output, int outOff)
		{
			if (length < 1)
			{
				if (length < 0)
				{
					throw new ArgumentException("Can't have a negative input length!");
				}
				return 0;
			}
			else
			{
				int blockSize = this.GetBlockSize();
				int updateOutputSize = this.GetUpdateOutputSize(length);
				if (updateOutputSize > 0 && outOff + updateOutputSize > output.Length)
				{
					throw new DataLengthException("output buffer too short");
				}
				int num = 0;
				int num2 = this.buf.Length - this.bufOff;
				if (length > num2)
				{
					Array.Copy(input, inOff, this.buf, this.bufOff, num2);
					num += this.cipher.ProcessBlock(this.buf, 0, output, outOff);
					this.bufOff = 0;
					length -= num2;
					inOff += num2;
					while (length > this.buf.Length)
					{
						num += this.cipher.ProcessBlock(input, inOff, output, outOff + num);
						length -= blockSize;
						inOff += blockSize;
					}
				}
				Array.Copy(input, inOff, this.buf, this.bufOff, length);
				this.bufOff += length;
				if (this.bufOff == this.buf.Length)
				{
					num += this.cipher.ProcessBlock(this.buf, 0, output, outOff + num);
					this.bufOff = 0;
				}
				return num;
			}
		}

		public override byte[] DoFinal()
		{
			byte[] array = BufferedCipherBase.EmptyBuffer;
			int outputSize = this.GetOutputSize(0);
			if (outputSize > 0)
			{
				array = new byte[outputSize];
				int num = this.DoFinal(array, 0);
				if (num < array.Length)
				{
					byte[] array2 = new byte[num];
					Array.Copy(array, 0, array2, 0, num);
					array = array2;
				}
			}
			else
			{
				this.Reset();
			}
			return array;
		}

		public override byte[] DoFinal(byte[] input, int inOff, int inLen)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			int outputSize = this.GetOutputSize(inLen);
			byte[] array = BufferedCipherBase.EmptyBuffer;
			if (outputSize > 0)
			{
				array = new byte[outputSize];
				int num = (inLen > 0) ? this.ProcessBytes(input, inOff, inLen, array, 0) : 0;
				num += this.DoFinal(array, num);
				if (num < array.Length)
				{
					byte[] array2 = new byte[num];
					Array.Copy(array, 0, array2, 0, num);
					array = array2;
				}
			}
			else
			{
				this.Reset();
			}
			return array;
		}

		public override int DoFinal(byte[] output, int outOff)
		{
			if (this.bufOff != 0)
			{
				if (!this.cipher.IsPartialBlockOkay)
				{
					throw new DataLengthException("data not block size aligned");
				}
				if (outOff + this.bufOff > output.Length)
				{
					throw new DataLengthException("output buffer too short for DoFinal()");
				}
				this.cipher.ProcessBlock(this.buf, 0, this.buf, 0);
				Array.Copy(this.buf, 0, output, outOff, this.bufOff);
			}
			int result = this.bufOff;
			this.Reset();
			return result;
		}

		public override void Reset()
		{
			Array.Clear(this.buf, 0, this.buf.Length);
			this.bufOff = 0;
			this.cipher.Reset();
		}
	}
}
