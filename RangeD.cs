using System;

namespace DNA
{
	public struct RangeD
	{
		private double _min;

		private double _max;

		public bool Degenerate
		{
			get
			{
				return this._min == this._max;
			}
		}

		public double Min
		{
			get
			{
				return this._min;
			}
		}

		public double Max
		{
			get
			{
				return this._max;
			}
		}

		public double Span
		{
			get
			{
				return this.Max - this.Min;
			}
		}

		public double ToSpan(double val)
		{
			return (val - this.Min) / this.Span;
		}

		public bool InRange(double t)
		{
			return t >= this.Min && t <= this.Max;
		}

		public bool Overlaps(RangeF r)
		{
			return (double)r.Min <= this.Max && (double)r.Max >= this.Min;
		}

		public RangeD(double min, double max)
		{
			this._min = min;
			this._max = max;
			
			if (this._max < this._min)
			{
				throw new ArgumentException("Max must be Greator than Min");
			}
		}

		public override int GetHashCode()
		{
			return this._min.GetHashCode() ^ this._max.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			RangeD rangeD = (RangeD)obj;
			return this._min == rangeD._min && this._max == rangeD._max;
		}

		public static bool operator == (RangeD a, RangeD b)
		{
			return a._min == b._min && a._max == b._max;
		}

		public static bool operator != (RangeD a, RangeD b)
		{
			return a._min != b._min || a._max != b._max;
		}
	}
}
