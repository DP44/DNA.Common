using System;

namespace DNA.Data.Units
{
	[Serializable]
	public struct Frequency
	{
		public static readonly Frequency Zero = Frequency.FromHertz(0f);
		private float _hertz;

		public float Hertz
		{
			get
			{
				return this._hertz;
			}
			set
			{
				this._hertz = value;
			}
		}

		public TimeSpan Period
		{
			get
			{
				return TimeSpan.FromSeconds(1.0 / (double)this._hertz);
			}
		}

		public static Frequency FromHertz(float hertz)
		{
			return new Frequency(hertz);
		}

		public static Frequency FromPeriod(TimeSpan span)
		{
			return new Frequency((float)(1.0 / span.TotalSeconds));
		}

		private Frequency(float hertz)
		{
			this._hertz = hertz;
		}

		public override int GetHashCode()
		{
			return this._hertz.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return this._hertz == ((Frequency)obj)._hertz;
		}

		public static bool operator == (Frequency a, Frequency b)
		{
			return a._hertz == b._hertz;
		}

		public static bool operator != (Frequency a, Frequency b)
		{
			return a._hertz != b._hertz;
		}

		public override string ToString()
		{
			return this.Hertz.ToString();
		}
	}
}
