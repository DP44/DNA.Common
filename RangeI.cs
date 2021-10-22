using System;

namespace DNA
{
	public struct RangeI
	{
		private int _min;
		private int _max;

		public bool Degenerate
		{
			get
			{
				return this._min == this._max;
			}
		}

		public int Min
		{
			get
			{
				return this._min;
			}
		}

		public int Max
		{
			get
			{
				return this._max;
			}
		}

		public int Span
		{
			get
			{
				return this.Max - this.Min;
			}
		}

		public override int GetHashCode()
		{
			return this._min.GetHashCode() ^ this._max.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			RangeI rangeI = (RangeI)obj;
			return rangeI._max == rangeI.Min;
		}

		public static bool operator == (RangeI a, RangeI b)
		{
			return a._max == b._max && a._min == b._min;
		}

		public static bool operator != (RangeI a, RangeI b)
		{
			return a._max != b._max && a._min != b._min;
		}

		public bool InRange(int t)
		{
			return t >= this.Min && t <= this.Max;
		}

		public bool Overlaps(RangeI r)
		{
			return r.Min <= this.Max && r.Max >= this.Min;
		}

		public RangeI(int min, int max)
		{
			this._min = min;
			this._max = max;
			
			if (this._max < this._min)
			{
				throw new ArgumentException("Max must be Greator than Min");
			}
		}
	}
}
