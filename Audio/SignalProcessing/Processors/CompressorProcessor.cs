using System;

namespace DNA.Audio.SignalProcessing.Processors
{
	public class CompressorProcessor : RealLevelsProcessor
	{
		public float targetLevel = 1f;
		private float[] levelHistory = new float[20];
		private int index;

		public override bool ProcessBlock(RealPCMData data)
		{
			base.ProcessBlock(data);
			this.levelHistory[this.index++] = this.PeakLevel;
			
			if (this.index >= this.levelHistory.Length - 1)
			{
				this.index = 0;
			}
			
			float num = 0f;
			
			for (int i = 0; i < this.levelHistory.Length; i++)
			{
				num += this.levelHistory[i];
			}
			
			num /= (float)this.levelHistory.Length;
			float num2 = this.targetLevel / num;
			float[] data2 = data.GetData(0);
			
			if (this.PeakLevel * num2 > 0.8f)
			{
				num2 = 0.8f / this.PeakLevel;
			}
			
			if (this.PeakLevel > 0.15f)
			{
				for (int j = 0; j < data2.Length; j++)
				{
					data2[j] *= num2;
				}
			}
			
			return true;
		}
	}
}
