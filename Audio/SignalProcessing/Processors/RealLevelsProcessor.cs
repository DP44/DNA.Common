using System;

namespace DNA.Audio.SignalProcessing.Processors
{
	public class RealLevelsProcessor : SignalProcessor<RealPCMData>
	{
		public float PeakLevel;
		public float averageLevel;
		public float Volume = 1f;

		public override bool ProcessBlock(RealPCMData data)
		{
			float[] data2 = data.GetData(0);
			this.PeakLevel = 0f;
			this.averageLevel = 0f;
			
			for (int i = 0; i < data2.Length; i++)
			{
				float num = Math.Abs(data2[i]);
				this.PeakLevel = ((num > this.PeakLevel) ? num : this.PeakLevel);
				this.averageLevel += num;
			}
			
			this.averageLevel /= (float)data2.Length;
			return true;
		}
	}
}
