using System;
using Microsoft.Xna.Framework.Media;

namespace DNA.Audio
{
	public class BeatFinder
	{
		private TimeSpan[] beats = new TimeSpan[30];
		private TimeSpan timeSinceLastPeek = TimeSpan.Zero;
		private float averageBMP;
		private float[] historicalAverages = new float[50];
		private bool beatFound;
		
		/// <summary>
		/// 
		/// </summary>
		public bool BeatFound
		{
			get
			{
				return this.beatFound;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public float AverageBMP
		{
			get
			{
				return this.averageBMP;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void Update(VisualizationData visData, TimeSpan elaspedTime)
		{
			this.timeSinceLastPeek += elaspedTime;
			float num = 0f;
			
			for (int i = 0; i < visData.Samples.Count; i++)
			{
				float num2 = visData.Samples[i];
				num2 *= num2;
				num += num2;
			}
			
			num /= (float)visData.Samples.Count;
			
			if (num != this.historicalAverages[this.historicalAverages.Length - 1])
			{
				float num3 = 0f;
				
				for (int j = 0; j < this.historicalAverages.Length; j++)
				{
					num3 += this.historicalAverages[j];
				}
				
				num3 /= (float)this.historicalAverages.Length;
				float num4 = 0f;
				
				for (int k = 0; k < this.historicalAverages.Length; k++)
				{
					num4 += Math.Abs(this.historicalAverages[k] - num3);
				}
				
				num4 /= (float)this.historicalAverages.Length;
				float num5 = num - num3;
				
				if (num5 >= num4 && this.timeSinceLastPeek.TotalSeconds > 0.20000000298023224)
				{
					this.beatFound = true;
					TimeSpan t = TimeSpan.Zero;
					
					for (int l = 0; l < this.beats.Length - 1; l++)
					{
						t += this.beats[l];
						this.beats[l] = this.beats[l + 1];
					}
					
					this.averageBMP = 60f / (float)TimeSpan.FromSeconds(
						t.TotalSeconds / (double)(this.beats.Length - 1)).TotalSeconds;
					
					this.beats[this.beats.Length - 1] = this.timeSinceLastPeek;
					this.timeSinceLastPeek = TimeSpan.Zero;
				}
				else
				{
					this.beatFound = false;
				}
				
				for (int m = 0; m < this.historicalAverages.Length - 1; m++)
				{
					this.historicalAverages[m] = this.historicalAverages[m + 1];
				}
				
				this.historicalAverages[this.historicalAverages.Length - 1] = num;
			}
		}
	}
}
