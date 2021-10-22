using System;
using DNA.Data.Units;
using DNA.Multimedia.Audio;

namespace DNA.Audio.SignalProcessing.Processors
{
	public class AutoTuner : SignalProcessor<SpectralData>
	{
		private PitchShifter _pitchShifter = new PitchShifter();
		private AnaylzeProcessor _anaylizer = new AnaylzeProcessor();

		public AnaylzeProcessor Anaylizer
		{
			get
			{
				return this._anaylizer;
			}
		}

		public override bool ProcessBlock(SpectralData data)
		{
			this._anaylizer.ProcessBlock(data);
			Frequency freq = this._anaylizer.PrimaryFrequency.Value;
			freq.Hertz = Math.Abs(freq.Hertz);
			Tone tone = Tone.FromFrequency(freq);
			int num = tone.KeyValue;
			
			if (tone.Detune > 0.5f)
			{
				num++;
			}
			
			if (tone.Detune < -0.5f)
			{
				num--;
			}
			
			Tone key = Tone.FromKeyIndex(num);
			
			if (freq.Hertz > 0f)
			{
				float pitch = key.Frequency.Hertz / freq.Hertz;
				this._pitchShifter.Pitch = pitch;
			}
			else
			{
				this._pitchShifter.Pitch = 1f;
			}
			
			this._pitchShifter.ProcessBlock(data);
			
			return true;
		}
	}
}
