using System;
using DNA.Data.Units;

namespace DNA.Audio
{
	public struct FrequencyPair
	{
		public Frequency Value;
		public float Magnitude;
		
		/// <summary>
		/// 
		/// </summary>
		public override string ToString()
		{
			return this.Value.ToString() + " " + this.Magnitude.ToString();
		}
	}
}
