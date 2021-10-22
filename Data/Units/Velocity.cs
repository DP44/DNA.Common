using System;

namespace DNA.Data.Units
{
	[Serializable]
	public struct Velocity
	{
		private const float mpsPerKnots = 0.5144445f;
		private const float mpsPerMph = 0.44704f;
		private const float mpsPerKph = 0.2777778f;

		private float _mps;

		public float Knots
		{
			get
			{
				return this._mps / 0.5144445f;
			}
			set
			{
				this._mps = value * 0.5144445f;
			}
		}

		public float MilesPerHour
		{
			get
			{
				return this._mps / 0.44704f;
			}
			set
			{
				this._mps = value * 0.44704f;
			}
		}

		public float MetersPerSecond
		{
			get
			{
				return this._mps;
			}
			set
			{
				this._mps = value;
			}
		}

		public float KilometersPerHour
		{
			get
			{
				return this._mps / 0.5144445f;
			}
			set
			{
				this._mps = value * 0.5144445f;
			}
		}

		public static Velocity FromMetersPerSecond(float mps)
		{
			return new Velocity(mps);
		}

		public static Velocity FromKnots(float knots)
		{
			return new Velocity(knots * 0.5144445f);
		}

		public static Velocity FromMilesPerHour(float mph)
		{
			return new Velocity(mph * 0.44704f);
		}

		public static Velocity FromKilometersPerHour(float kph)
		{
			return new Velocity(kph * 0.2777778f);
		}

		private Velocity(float mps)
		{
			this._mps = mps;
		}

		public override string ToString()
		{
			return this.MilesPerHour.ToString() + " Mph";
		}

		public override int GetHashCode()
		{
			return this._mps.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return this._mps == ((Velocity)obj)._mps;
		}

		public static bool operator == (Velocity a, Velocity b)
		{
			return a._mps == b._mps;
		}

		public static bool operator != (Velocity a, Velocity b)
		{
			return a._mps != b._mps;
		}
	}
}
