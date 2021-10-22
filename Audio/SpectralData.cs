using System;
using DNA.Collections;
using DNA.Data.Units;

namespace DNA.Audio
{
	public class SpectralData
	{
		private FrequencyPair[][] _channelData = new FrequencyPair[][]
		{
			new FrequencyPair[0]
		};
		
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
		public int FrequencyCount
		{
			get
			{
				return this._channelData[0].Length;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public FrequencyPair[] GetData(int channel)
		{
			return this._channelData[channel];
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void CopyTo(SpectralData data)
		{
			if (this.Channels != data.Channels || this.FrequencyCount != data.FrequencyCount)
			{
				data._channelData = ArrayTools.AllocSquareJaggedArray<FrequencyPair>(this.Channels, this.FrequencyCount);
			}
			
			for (int i = 0; i < this.Channels; i++)
			{
				this._channelData[i].CopyTo(data._channelData[i], 0);
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public void SetZero()
		{
			for (int i = 0; i < this.Channels; i++)
			{
				for (int j = 0; j < this._channelData[i].Length; j++)
				{
					this._channelData[i][j].Magnitude = 0f;
					this._channelData[i][j].Value = Frequency.Zero;
				}
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public SpectralData(int channels, int frequencies)
		{
			this._channelData = ArrayTools.AllocSquareJaggedArray<FrequencyPair>(channels, frequencies);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void Convert(RealPCMData data)
		{
			throw new NotImplementedException();
		}
	}
}
