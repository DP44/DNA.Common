using System;

namespace DNA.Audio.SignalProcessing.Processors
{
	public class AnaylzeProcessor : SignalProcessor<SpectralData>
	{
		private FrequencyPair _primary;

		public FrequencyPair PrimaryFrequency
		{
			get
			{
				return this._primary;
			}
		}

		public override bool ProcessBlock(SpectralData data)
		{
			FrequencyPair[] freq = data.GetData(0);
			float peak = float.MinValue;
		
			for (int i = 0; i < freq.Length; i++)
			{
				if (freq[i].Magnitude > peak)
				{
					peak = freq[i].Magnitude;
					this._primary = freq[i];
				}
			}
		
			this._primary.Value.Hertz = Math.Abs(this._primary.Value.Hertz);
			return true;
		}
	}
}
