using System;

namespace DNA.Drawing.Effects
{
	public interface IEffectTime
	{
		TimeSpan ElaspedTime { get; set; }
		TimeSpan TotalTime { get; set; }
	}
}
