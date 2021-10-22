using System;
using System.IO;
using DNA.Collections;

namespace DNA.Audio
{
	public class RealPCMData
	{
		private float[][] _channelData = new float[][]
		{
			new float[0]
		};
		
		/// <summary>
		/// 
		/// </summary>
		public int SampleRate { get; private set; }
		
		/// <summary>
		/// 
		/// </summary>
		public int Channels
		{
			get
			{
				return this._channelData.Length;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public int Samples
		{
			get
			{
				return this._channelData[0].Length;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public TimeSpan Time
		{
			get
			{
				return TimeSpan.FromSeconds((double)this.Samples / (double)this.SampleRate);
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public float[] GetData(int channel)
		{
			return this._channelData[channel];
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void Convert(RawPCMData data)
		{
			if (data.Samples != this.Samples || this.Channels != data.Channels)
			{
				this.Alloc(this.Channels, data.Samples);
			}
			
			this.SampleRate = data.SampleRate;
			int bitsPerSample = data.BitsPerSample;
			
			if (bitsPerSample == 16)
			{
				int num = 2 * this.Channels;
				byte[] channelData = data.ChannelData;
				
				for (int i = 0; i < data.Channels; i++)
				{
					float[] array = this._channelData[i];
					int num2 = 0;
					
					for (int j = i * 2; j < data.ChannelData.Length; j += num)
					{
						short num3 = (short)channelData[j];
						short num4 = (short)(channelData[j + 1] << 8);
						short num5 = num3 | num4;
						float num6 = (float)num5 * 3.05175781E-05f;
						array[num2++] = num6;
					}
				}

				return;
			}

			throw new NotImplementedException();
		}
		
		/// <summary>
		/// 
		/// </summary>
		private RealPCMData() {}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void Alloc(int channels, int samples)
		{
			this._channelData = ArrayTools.AllocSquareJaggedArray<float>(channels, samples);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public RealPCMData(int channels, int samples, int sampleRate)
		{
			this.SampleRate = sampleRate;
			this.Alloc(channels, samples);
		}
		
		/// <summary>
		/// 
		/// </summary>
		public void CombineChannels()
		{
			if (this.Channels == 1)
			{
				return;
			}
			
			float[][] array = ArrayTools.AllocSquareJaggedArray<float>(1, this.Samples);
			int samples = this.Samples;
			int channels = this.Channels;
			
			for (int i = 0; i < samples; i++)
			{
				array[0][i] = 0f;
				
				for (int j = 0; j < this.Channels; j++)
				{
					array[0][i] += this._channelData[j][i];
				}
				
				array[0][i] /= (float)channels;
			}

			this._channelData = array;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void AdjustVolume(float modifier)
		{
			int samples = this.Samples;
			
			for (int i = 0; i < this.Channels; i++)
			{
				for (int j = 0; j < samples; j++)
				{
					this._channelData[i][j] *= modifier;
				}
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void TrimEndSilence(float threshold)
		{
			int num = 0;
			int length = this._channelData.GetLength(1);
			
			for (int i = 0; i < this.Channels; i++)
			{
				for (int j = length - 1; j > num; j--)
				{
					if (Math.Abs(this._channelData[i][j]) > threshold)
					{
						num = j;
						break;
					}
				}
			}
			
			if (num == 0)
			{
				return;
			}
			
			float[][] array = ArrayTools.AllocSquareJaggedArray<float>(this.Channels, num);
			
			for (int k = 0; k < this.Channels; k++)
			{
				for (int l = 0; l < num; l++)
				{
					array[k][l] = this._channelData[k][l];
				}
			}
			
			this._channelData = array;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void TrimBeginSilence(float threshold)
		{
			int num = this.Samples;
			this._channelData.GetLength(1);
			
			for (int i = 0; i < this.Channels; i++)
			{
				for (int j = 0; j < num; j++)
				{
					if (Math.Abs(this._channelData[i][j]) > threshold)
					{
						num = j;
						break;
					}
				}
			}
			
			int num2 = this.Samples - num;
			float[][] array = ArrayTools.AllocSquareJaggedArray<float>(this.Channels, num2);
			
			for (int k = 0; k < this.Channels; k++)
			{
				for (int l = 0; l < num2; l++)
				{
					array[k][l] = this._channelData[k][l + num];
				}
			}
			
			this._channelData = array;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void AdjustSpeed(float modifier)
		{
			this.SampleRate = (int)Math.Ceiling((double)((float)this.SampleRate * modifier));
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void Resample(int newSampleRate)
		{
			float num = 1f / (float)newSampleRate;
			float num2 = 1f / (float)this.SampleRate;
			int samples = this.Samples;
			int num3 = (int)((long)this.Samples * (long)newSampleRate / (long)this.SampleRate);
			float[][] array = ArrayTools.AllocSquareJaggedArray<float>(this.Channels, num3);
			
			for (int i = 0; i < this.Channels; i++)
			{
				for (int j = 0; j < num3; j++)
				{
					int num4 = (int)Math.Floor((double)((float)j * (float)(samples - 1) / (float)num3));
					array[i][j] = this._channelData[i][num4];
				}
			}
			
			this._channelData = array;
			this.SampleRate = newSampleRate;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public float GetAverageAmplitude(int sampleStart, int sampleEnd)
		{
			float num = 0f;
			
			for (int i = 0; i < this.Channels; i++)
			{
				for (int j = sampleStart; j <= sampleEnd; j++)
				{
					num += Math.Abs(this._channelData[i][j]);
				}
			}
			
			return num / (float)(sampleEnd - sampleStart + 1);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public float GetMaxAmplitude(int sampleStart, int sampleEnd)
		{
			float num = float.MinValue;
			
			for (int i = 0; i < this.Channels; i++)
			{
				for (int j = sampleStart; j <= sampleEnd; j++)
				{
					float val = Math.Abs(this._channelData[i][j]);
					num = Math.Max(num, val);
				}
			}
			
			return num;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public float GetMaxAmplitude()
		{
			return this.GetMaxAmplitude(0, this.Samples - 1);
		}
		
		/// <summary>
		/// 
		/// </summary>
		public void Normalize()
		{
			float num = this.GetMaxAmplitude();
			
			if (num == 0f)
			{
				num = 1f;
			}
			
			this.AdjustVolume(1f / num);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		private void LoadWavInternal(Stream stream)
		{
			RawPCMData data = RawPCMData.LoadWav(stream);
			this.Convert(data);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static RealPCMData LoadWav(string path)
		{
			RealPCMData result;
			
			using (FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				result = RealPCMData.LoadWav(fileStream);
			}
			
			return result;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static RealPCMData LoadWav(Stream stream)
		{
			RealPCMData data = new RealPCMData();
			data.LoadWavInternal(stream);
			return data;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void SaveWav(string path, int BitsPerSample)
		{
			using (FileStream fileStream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				this.SaveWav(fileStream, BitsPerSample);
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void SaveWav(Stream stream, int BitsPerSample)
		{
			new BinaryWriter(stream);
			RawPCMData data = new RawPCMData();
			data.Convert(this, BitsPerSample);
			data.SaveWav(stream);
		}
	}
}
