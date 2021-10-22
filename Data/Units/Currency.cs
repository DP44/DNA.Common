using System;

namespace DNA.Data.Units
{
	public struct Currency
	{
		private decimal _usDollars;

		public Currency(decimal usDollars)
		{
			this._usDollars = usDollars;
		}

		public override string ToString()
		{
			return "$" + this._usDollars.ToString();
		}

		public override int GetHashCode()
		{
			return this._usDollars.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return this._usDollars == ((Currency)obj)._usDollars;
		}

		public static bool operator == (Currency a, Currency b)
		{
			return a._usDollars == b._usDollars;
		}

		public static bool operator != (Currency a, Currency b)
		{
			return a._usDollars != b._usDollars;
		}
	}
}
