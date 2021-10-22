using System;
using System.Collections.Generic;
using System.IO;

namespace DNA.Audio
{
	public class RawPCMData
	{
		private enum WavCompression
		{
			Unknown,
			PCM,
			MicrosoftADPCM,
			ITUG711alaw = 6,
			ITUG711Alaw,
			IMAADPCM = 17,
			ITUG723ADPCM = 20,
			GSM610 = 49,
			ITUG721ADPCM = 64,
			MPEG = 80
		}

		private byte[] _channelData = new byte[0];

		public byte[] ChannelData
		{
			get
			{
				return this._channelData;
			}
		}

		public int Channels { get; private set; }
		public int SampleRate { get; private set; }
		public int BitsPerSample { get; private set; }

		public int Samples
		{
			get
			{
				return this._channelData.Length / (this.Channels * (this.BitsPerSample >> 3));
			}
		}

		public TimeSpan Time
		{
			get
			{
				return TimeSpan.FromSeconds((double)this.Samples / (double)this.SampleRate);
			}
		}

		public RawPCMData()
		{
			this.Channels = 1;
		}

		public RawPCMData(int channels, int samples, int sampleRate, int bitsPerSample)
		{
			this.Channels = channels;
			this.SampleRate = sampleRate;
			this.BitsPerSample = bitsPerSample;
			this._channelData = new byte[channels * samples * (this.BitsPerSample >> 3)];
		}

		public RawPCMData(int channels, int sampleRate, int bitsPerSample, byte[] channelData)
		{
			this.Channels = channels;
			this.SampleRate = sampleRate;
			this.BitsPerSample = bitsPerSample;
			this._channelData = channelData;
		}

		public RawPCMData(RealPCMData data, int bitsPerSample)
		{
			this.Convert(data, bitsPerSample);
		}

		public void Alloc(int channels, int samples)
		{
			this._channelData = new byte[channels * samples * (this.BitsPerSample >> 3)];
		}

		public void Convert(RealPCMData data, int bitsPerSample)
		{
			if (data.Channels != this.Channels || data.Samples != data.Samples || this.BitsPerSample != bitsPerSample)
			{
				this.BitsPerSample = bitsPerSample;
				this.Alloc(data.Channels, data.Samples);
			}
			int bitsPerSample2 = this.BitsPerSample;
			if (bitsPerSample2 == 16)
			{
				for (int i = 0; i < this.Channels; i++)
				{
					int num = 2;
					int num2 = this.Channels * num;
					float[] data2 = data.GetData(i);
					int num3 = num * i;
					foreach (float num4 in data2)
					{
						short num5 = (short)(num4 * 32768f);
						this._channelData[num3] = (byte)(num5 & 255);
						this._channelData[num3 + 1] = (byte)(num5 >> 8);
						num3 += num2;
					}
				}
				return;
			}
			throw new NotImplementedException();
		}

		private void LoadWavInternal(Stream stream)
		{
			this.LoadWav(new BinaryReader(stream));
		}

		public static RawPCMData LoadWav(string path)
		{
			RawPCMData result;
			using (FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				result = RawPCMData.LoadWav(fileStream);
			}
			return result;
		}

		public static RawPCMData LoadWav(Stream stream)
		{
			RawPCMData rawPCMData = new RawPCMData();
			rawPCMData.LoadWavInternal(stream);
			return rawPCMData;
		}

		private int LoadWav(BinaryReader reader)
		{
			this._channelData = null;
			if (reader.ReadUInt32() != 1179011410U)
			{
				throw new Exception("Not a valid Wav File");
			}
			int num = reader.ReadInt32();
			long position = reader.BaseStream.Position;
			if (reader.ReadUInt32() != 1163280727U)
			{
				throw new Exception("Not a valid Wav File");
			}
			long num2 = position + (long)num;
			new List<List<float>>();
			while (reader.BaseStream.Position < num2)
			{
				uint num3 = reader.ReadUInt32();
				int num4 = reader.ReadInt32();
				long position2 = reader.BaseStream.Position;
				long num5 = position2 + (long)num4;
				uint num6 = num3;
				if (num6 != 544501094U)
				{
					if (num6 != 1635017060U)
					{
						if (num6 == 1953393779U)
						{
							throw new Exception("Silence Chunks not Supported");
						}
					}
					else
					{
						if (this._channelData != null)
						{
							throw new Exception("Mutiple Wav Chunks Not Supported");
						}
						this._channelData = reader.ReadBytes(num4);
						int bitsPerSample = this.BitsPerSample;
					}
				}
				else
				{
					RawPCMData.WavCompression wavCompression = (RawPCMData.WavCompression)reader.ReadUInt16();
					reader.ReadUInt16();
					this.SampleRate = reader.ReadInt32();
					reader.ReadInt32();
					reader.ReadUInt16();
					this.BitsPerSample = (int)reader.ReadUInt16();
					if (wavCompression != RawPCMData.WavCompression.PCM)
					{
						throw new Exception("Unsupported Wav Compression " + wavCompression.ToString());
					}
				}
				if (reader.BaseStream.Position < num5)
				{
					reader.BaseStream.Position = num5;
				}
			}
			return this.BitsPerSample;
		}

		public void SaveWav(string path)
		{
			using (FileStream fileStream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				this.SaveWav(fileStream);
			}
		}

		public void SaveWav(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			this.SaveWav(writer);
		}

		public void SaveWav(BinaryWriter writer)
		{
			writer.Write(1179011410U);
			int num = this.BitsPerSample * this.Channels >> 3;
			int num2 = num * this.Samples;
			writer.Write(36 + num2);
			writer.Write(1163280727U);
			writer.Write(544501094U);
			writer.Write(16);
			writer.Write(1);
			writer.Write((short)this.Channels);
			writer.Write(this.SampleRate);
			writer.Write(this.SampleRate * num);
			writer.Write((short)num);
			writer.Write((short)this.BitsPerSample);
			writer.Write(1635017060U);
			writer.Write(num2);
			int samples = this.Samples;
			int channels = this.Channels;
			byte[] buffer;
			if (this.BitsPerSample == 16)
			{
				buffer = new byte[this._channelData.Length];
			}
			else
			{
				buffer = this._channelData;
			}
			writer.Write(buffer);
		}
	}
}
