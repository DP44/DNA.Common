using System;

namespace DNA.Audio.SignalProcessing.Processors
{
	public class PitchShifter : SignalProcessor<SpectralData>
	{
		public float Pitch = 1f;
		private SpectralData _buffer = new SpectralData(1, 0);

		public override bool ProcessBlock(SpectralData data)
		{
			data.CopyTo(this._buffer);
			data.SetZero();
			
			for (int i = 0; i < data.Channels; i++)
			{
				FrequencyPair[] data2 = data.GetData(i);
				FrequencyPair[] data3 = this._buffer.GetData(i);
				
				for (int j = 0; j < data2.Length; j++)
				{
					int num = (int)((float)j * this.Pitch);
					
					if (num < data2.Length)
					{
						FrequencyPair[] array = data2;
						int num2 = num;
						array[num2].Magnitude = array[num2].Magnitude + data3[j].Magnitude;
						data2[num].Value.Hertz = data3[j].Value.Hertz * this.Pitch;
					}
				}
			}
			
			return true;
		}
	}
}
