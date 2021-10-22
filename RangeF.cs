using System;

namespace DNA
{
	public struct RangeF
	{
		private float _min;

		private float _max;

		public bool Degenerate => 
			this._min == this._max;

		public float Min =>
			this._min;

		public float Max => 
			this._max;

		public float Span => 
			this.Max - this.Min;

		public float ToSpan(float val) => 
			(val - this.Min) / this.Span;

		public bool InRange(float t) => 
			t >= this.Min && t <= this.Max;

		public bool Overlaps(RangeF r) => 
			r.Min <= this.Max && r.Max >= this.Min;

		public RangeF(float min, float max)
		{
			this._min = min;
			this._max = max;
			
			if (this._max < this._min)
			{
				throw new ArgumentException("Max must be Greator than Min");
			}
		}

		public override int GetHashCode() => 
			this._min.GetHashCode() ^ this._max.GetHashCode();

		public override bool Equals(object obj)
		{
			RangeF rangeF = (RangeF)obj;
			return this._min == rangeF._min && this._max == rangeF._max;
		}

		public static bool operator == (RangeF a, RangeF b) =>
			return a._min == b._min && a._max == b._max;

		public static bool operator != (RangeF a, RangeF b) => 
			a._min != b._min || a._max != b._max;
	}
}
