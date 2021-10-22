using System;

namespace DNA.Audio.SignalProcessing.Processors
{
	public class VocoderProcessor : SignalProcessor<RealPCMData>
	{
		private class FFT
		{
			private float[] _sinTable;

			private float[] _cosTable;

			private int[] _revTable;

			private static int ilog2(int n)
			{
				int num = -1;
				while (n != 0)
				{
					num++;
					n >>= 1;
				}
				return num;
			}

			private static int BitReverse(int k, int nu)
			{
				int num = 0;
				for (int i = 0; i < nu; i++)
				{
					num <<= 1;
					num |= (k & 1);
					k >>= 1;
				}
				return num;
			}

			private void CreateArrays(int n)
			{
				int nu = VocoderProcessor.FFT.ilog2(n);
				this._sinTable = new float[n];
				this._cosTable = new float[n];
				this._revTable = new int[n];
				for (int i = 0; i < n; i++)
				{
					double num = 6.2831853071795862 * (double)i / (double)n;
					this._cosTable[i] = (float)Math.Cos(num);
					this._sinTable[i] = (float)Math.Sin(num);
					this._revTable[i] = VocoderProcessor.FFT.BitReverse(i, nu);
				}
			}

			public FFT(int windowSize)
			{
				int nu = VocoderProcessor.FFT.ilog2(windowSize);
				this._sinTable = new float[windowSize];
				this._cosTable = new float[windowSize];
				this._revTable = new int[windowSize];
				for (int i = 0; i < windowSize; i++)
				{
					double num = 6.2831853071795862 * (double)i / (double)windowSize;
					this._cosTable[i] = (float)Math.Cos(num);
					this._sinTable[i] = (float)Math.Sin(num);
					this._revTable[i] = VocoderProcessor.FFT.BitReverse(i, nu);
				}
			}

			public void DoFFT(float[,] data, int windowSize, int sign)
			{
				int num = VocoderProcessor.FFT.ilog2(windowSize);
				int num2 = windowSize;
				int num3 = num;
				for (int i = 0; i < num; i++)
				{
					num2 /= 2;
					num3--;
					for (int j = 0; j < windowSize; j += num2)
					{
						int k = 0;
						while (k < num2)
						{
							int num4 = this._revTable[j >> num3];
							float num5 = data[j + num2, 0] * this._cosTable[num4] + data[j + num2, 1] * this._sinTable[num4] * (float)sign;
							float num6 = data[j + num2, 1] * this._cosTable[num4] - data[j + num2, 0] * this._sinTable[num4] * (float)sign;
							data[j + num2, 0] = data[j, 0] - num5;
							data[j + num2, 1] = data[j, 1] - num6;
							data[j, 0] += num5;
							data[j, 1] += num6;
							k++;
							j++;
						}
					}
				}
				for (int j = 0; j < windowSize; j++)
				{
					int k = this._revTable[j];
					if (k > j)
					{
						float num7 = data[j, 0];
						float num8 = data[j, 1];
						data[j, 0] = data[k, 0];
						data[j, 1] = data[k, 1];
						data[k, 0] = num7;
						data[k, 1] = num8;
					}
				}
			}
		}

		private const int MaxWindowSize = 4096;

		private int _windowLength = 512;

		private int _windowOverlap = 256;

		private int _bandCount = 16;

		private bool _Normalize = true;

		private float[] CarrierBuffer;

		private float[] _modulatorBuffer = new float[4096];

		private float[,] _loopedCarrierBuffer;

		private float[,] _outputBuffer = new float[4096, 2];

		private float[] _outputOld = new float[4096];

		private float[] _outputNew = new float[4096];

		private VocoderProcessor.FFT _fft1;

		private VocoderProcessor.FFT _fft2;

		public VocoderProcessor(RealPCMData carrier)
		{
			this._fft1 = new VocoderProcessor.FFT(this._windowLength);
			this._fft2 = new VocoderProcessor.FFT(this._windowLength / 2);
			this.ProcessCarrier(carrier);
		}

		private void ProcessCarrier(RealPCMData carrier)
		{
			this.CarrierBuffer = new float[this._windowLength];
			this.CarrierBuffer = carrier.GetData(0);
			float[] array = new float[this._windowLength];
			this._loopedCarrierBuffer = new float[4096, 2];
			VocoderProcessor.ReadLooped(this.CarrierBuffer, array, 0, this._windowLength);
			VocoderProcessor.ToComplexArray(array, this._loopedCarrierBuffer, this._windowLength);
			this._fft1.DoFFT(this._loopedCarrierBuffer, this._windowLength, 1);
			this.NormalizeFFT(this._loopedCarrierBuffer, this._windowLength);
		}

		private static int ReadBuffer(float[] buffer, int sourcePos, float[] dest, int destPos, int length)
		{
			int num = buffer.Length - sourcePos;
			if (num > length)
			{
				num = length;
			}
			int num2 = length - num;
			for (int i = 0; i < num; i++)
			{
				dest[i + destPos] = buffer[i + sourcePos];
			}
			for (int j = 0; j < num2; j++)
			{
				dest[j + destPos] = 0f;
			}
			return num;
		}

		private static void ReadLooped(float[] buffer, float[] dest, int dpos, int length)
		{
			int num = 0;
			while (length > 0)
			{
				if (num >= buffer.Length)
				{
					num = 0;
				}
				dest[dpos] = buffer[num];
				dpos++;
				num++;
				length--;
			}
		}

		private static void CopyBuffer(float[] source, float[] dest, int length)
		{
			for (int i = 0; i < length; i++)
			{
				dest[i] = source[i];
			}
		}

		private static void ToComplexArray(float[] source, float[,] dest, int length)
		{
			for (int i = 0; i < length; i++)
			{
				dest[i, 0] = source[i];
				dest[i, 1] = 0f;
			}
		}

		private static void ComplexToSample(float[,] complex_array, float[] sample_array, int length)
		{
			for (int i = 0; i < length; i++)
			{
				sample_array[i] = complex_array[i, 0];
			}
		}

		private static int WriteBuffer(float[] output, int outputPos, float[] source, int sourcePos, int length)
		{
			for (int i = 0; i < length; i++)
			{
				output[outputPos + i] = source[sourcePos + i];
			}
			return length;
		}

		private void DoVocode(float[] modulatorBuffer)
		{
			int num = modulatorBuffer.Length;
			int num2 = (num - this._windowOverlap) / (this._windowLength - this._windowOverlap);
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			do
			{
				int num6 = VocoderProcessor.ReadBuffer(modulatorBuffer, num4, this._modulatorBuffer, this._windowOverlap, this._windowLength - this._windowOverlap);
				num4 += num6;
				this.VocodeWindow(this._modulatorBuffer, this._loopedCarrierBuffer, this._outputBuffer);
				VocoderProcessor.ComplexToSample(this._outputBuffer, this._outputNew, this._windowLength);
				for (int i = 0; i < this._windowOverlap; i++)
				{
					this._outputNew[i] = this._outputNew[i] * ((float)i / (float)this._windowOverlap) + this._outputOld[this._windowLength - this._windowOverlap + i] * ((float)(this._windowOverlap - i) / (float)this._windowOverlap);
				}
				int num7 = VocoderProcessor.WriteBuffer(modulatorBuffer, num5, this._outputNew, 0, this._windowLength - this._windowOverlap);
				num5 += num7;
				for (int i = 0; i < this._windowOverlap; i++)
				{
					this._modulatorBuffer[i] = this._modulatorBuffer[this._windowLength - this._windowOverlap + i];
				}
				float[] outputOld = this._outputOld;
				this._outputOld = this._outputNew;
				this._outputNew = outputOld;
				num3++;
			}
			while (num4 < modulatorBuffer.Length);
		}

		private void VocodeWindow(float[] modulator, float[,] carrier, float[,] output)
		{
			int num = this._windowLength / (this._bandCount * 2);
			int num2 = this._windowLength / 2 - num * (this._bandCount - 1);
			this.RealFFTMag(modulator, this._windowLength);
			for (int i = 0; i < this._bandCount; i++)
			{
				int num3 = (i == this._bandCount - 1) ? num2 : num;
				float num4 = 0f;
				float num5 = 0f;
				int j = 0;
				int num6 = i * num;
				int num7 = this._windowLength - num6 - 1;
				while (j < num3)
				{
					if (this._Normalize)
					{
						float num8 = carrier[num6, 0] * carrier[num6, 0] + carrier[num6, 1] * carrier[num6, 1];
						float num9 = carrier[num7, 0] * carrier[num7, 0] + carrier[num7, 1] * carrier[num7, 1];
						num5 += (float)(Math.Sqrt((double)num8) + Math.Sqrt((double)num9));
					}
					num4 += modulator[num6];
					j++;
					num6++;
					num7--;
				}
				if (!this._Normalize)
				{
					num5 = 1f;
				}
				if (num5 == 0f)
				{
					num5 = 0.0001f;
				}
				float num10 = num4 / num5;
				j = 0;
				num6 = i * num;
				num7 = this._windowLength - num6 - 1;
				while (j < num3)
				{
					output[num6, 0] = carrier[num6, 0] * num10;
					output[num6, 1] = carrier[num6, 1] * num10;
					output[num7, 0] = carrier[num7, 0] * num10;
					output[num7, 1] = carrier[num7, 1] * num10;
					j++;
					num6++;
					num7--;
				}
			}
			this._fft1.DoFFT(output, this._windowLength, -1);
		}

		private void RealFFTMag(float[] data, int windowlength)
		{
			float[,] array = new float[windowlength / 2, 2];
			for (int i = 0; i < windowlength; i += 2)
			{
				array[i >> 1, 0] = data[i];
				array[i >> 1, 1] = data[i + 1];
			}
			this._fft2.DoFFT(array, windowlength / 2, 1);
			data[0] = (array[0, 0] + array[0, 1]) / (float)windowlength;
			for (int i = 1; i < windowlength / 2; i++)
			{
				double num = 6.2831853071795862 * (double)i / (double)windowlength;
				double num2 = Math.Cos(num);
				double num3 = Math.Sin(num);
				double num4 = (double)((array[i, 1] + array[windowlength / 2 - i, 1]) / 2f);
				double num5 = (double)((array[i, 0] - array[windowlength / 2 - i, 0]) / 2f);
				double num6 = (double)((array[i, 0] + array[windowlength / 2 - i, 0]) / 2f) + num2 * num4 - num3 * num5;
				double num7 = (double)((array[i, 1] - array[windowlength / 2 - i, 1]) / 2f) - num3 * num4 - num2 * num5;
				num6 /= (double)(windowlength / 2);
				num7 /= (double)(windowlength / 2);
				data[i] = (float)Math.Sqrt(num6 * num6 + num7 * num7);
			}
			data[windowlength / 2] = (array[0, 0] - array[0, 1]) / (float)windowlength;
		}

		private void NormalizeFFT(float[,] x, int n)
		{
			for (int i = 0; i < n; i++)
			{
				x[i, 0] /= (float)n;
				x[i, 1] /= (float)n;
			}
		}

		private void Initalize(int size)
		{
			this._modulatorBuffer = new float[size];
			this._outputBuffer = new float[size, 2];
			this._outputOld = new float[size];
			this._outputNew = new float[size];
		}

		public override bool ProcessBlock(RealPCMData data)
		{
			return true;
		}
	}
}
