using System;

namespace DNA.Audio.SignalProcessing.Processors
{
	public class RealToSpectralGroup : SignalProcessorGroup<RealPCMData, SpectralData>
	{
		private const int MAX_FRAME_LENGTH = 4096;

		private float[] gInFIFO = new float[4096];
		private float[] gOutFIFO = new float[4096];
		private float[] gFFTworksp = new float[8192];
		private float[] gLastPhase = new float[2049];
		private float[] gSumPhase = new float[2049];
		private float[] gOutputAccum = new float[8192];

		private int gRover;
		public int OverSampling = 2;
		public int FFTSize = 256;
		private float[] _windowVals;

		private ShortTimeFFT _fft = new ShortTimeFFT(1024);
		private SpectralData _specbuffer = new SpectralData(1, 1024);

		private void Alloc()
		{
			this._specbuffer = new SpectralData(1, this.FFTSize / 2 + 1);
			this._fft = new ShortTimeFFT(this.FFTSize);
			this._windowVals = new float[this.FFTSize];
		
			for (int i = 0; i < this.FFTSize; i++)
			{
				this._windowVals[i] = -0.5f * (float)Math.Cos(6.2831853071795862 * (double)((float)i) / (double)((float)this.FFTSize)) + 0.5f;
			}
		}

		public override bool ProcessBlock(RealPCMData data)
		{
			if (this._windowVals == null)
			{
				this.Alloc();
			}
		
			int samples = data.Samples;
			int sampleRate = data.SampleRate;
			float[] data2 = data.GetData(0);
			int fftsize = this.FFTSize;
			int overSampling = this.OverSampling;
			float num = 3.14159274f;
			float num2 = 6.28318548f;
			float[] dst = data2;
			int num3 = fftsize / 2;
			int num4 = fftsize / overSampling;
			float num5 = (float)sampleRate / (float)fftsize;
			float num6 = (float)(6.2831853071795862 * (double)((float)num4) / (double)((float)fftsize));
			int num7 = fftsize - num4;
		
			if (this.gRover == 0)
			{
				this.gRover = num7;
			}
		
			int i = 0;
			int num8 = 0;
			FrequencyPair[] data3 = this._specbuffer.GetData(0);
		
			while (i < samples)
			{
				int num9 = samples - i;
				int num10 = fftsize - this.gRover;
		
				if (num10 > num9)
				{
					num10 = num9;
				}
		
				Buffer.BlockCopy(data2, i * 4, this.gInFIFO, this.gRover * 4, num10 * 4);
				Buffer.BlockCopy(this.gOutFIFO, (this.gRover - num7) * 4, dst, num8 * 4, num10 * 4);
				i += num10;
				num8 += num10;
				this.gRover += num10;
		
				if (this.gRover >= fftsize)
				{
					this.gRover = num7;
		
					for (int j = 0; j < fftsize; j++)
					{
						this.gFFTworksp[2 * j] = this.gInFIFO[j] * this._windowVals[j];
						this.gFFTworksp[2 * j + 1] = 0f;
					}
		
					this._fft.Transform(this.gFFTworksp, -1);
		
					for (int j = 0; j <= num3; j++)
					{
						float num11 = this.gFFTworksp[2 * j];
						float num12 = this.gFFTworksp[2 * j + 1];
						float num13 = 2f * (float)Math.Sqrt((double)(num11 * num11 + num12 * num12));
						float num14 = (float)Math.Atan2((double)num12, (double)num11);
						float num15 = num14 - this.gLastPhase[j];
						this.gLastPhase[j] = num14;
						num15 -= (float)j * num6;
						int num16 = (int)(num15 / num);
		
						if (num16 >= 0)
						{
							num16 += (num16 & 1);
						}
						else
						{
							num16 -= (num16 & 1);
						}
						
						num15 -= num * (float)num16;
						num15 = (float)overSampling * num15 / num2;
						num15 = (float)j * num5 + num15 * num5;
						
						data3[j].Magnitude = num13;
						data3[j].Value.Hertz = num15;
					}

					for (int k = 0; k < base.Count; k++)
					{
						SignalProcessor<SpectralData> signalProcessor = base[k];
						
						if (signalProcessor.Active)
						{
							signalProcessor.ProcessBlock(this._specbuffer);
						}
					}

					for (int j = 0; j <= num3; j++)
					{
						float num13 = data3[j].Magnitude;
						float num15 = data3[j].Value.Hertz;
						num15 -= (float)j * num5;
						num15 /= num5;
						num15 = num2 * num15 / (float)overSampling;
						num15 += (float)j * num6;
						this.gSumPhase[j] += num15;
						float num14 = this.gSumPhase[j];
						this.gFFTworksp[2 * j] = num13 * (float)Math.Cos((double)num14);
						this.gFFTworksp[2 * j + 1] = num13 * (float)Math.Sin((double)num14);
					}

					for (int j = fftsize + 2; j < 2 * fftsize; j++)
					{
						this.gFFTworksp[j] = 0f;
					}

					this._fft.Transform(this.gFFTworksp, 1);
					float num17 = 2f / (float)(num3 * overSampling);
					
					for (int j = 0; j < fftsize; j++)
					{
						this.gOutputAccum[j] += num17 * this._windowVals[j] * this.gFFTworksp[2 * j];
					}
					
					Buffer.BlockCopy(this.gOutputAccum, 0, this.gOutFIFO, 0, num4 * 4);
					Buffer.BlockCopy(this.gOutputAccum, num4 * 4, this.gOutputAccum, 0, fftsize * 4);
					Buffer.BlockCopy(this.gInFIFO, num4 * 4, this.gInFIFO, 0, num7 * 4);
				}
			}
			
			return true;
		}
	}
}
