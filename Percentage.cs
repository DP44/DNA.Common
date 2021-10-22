using System;

namespace DNA
{
	[Serializable]
	public struct Percentage
	{
		public static readonly Percentage Zero = 
			Percentage.FromFraction(0f);
		
		public static readonly Percentage OneHundred = 
			Percentage.FromFraction(1f);

		private float _fraction;

		public float Fraction
		{
			get => 
				this._fraction;
			
			set => 
				this._fraction = value;
		}

		public float Percent
		{
			get => 
				this._fraction * 100f;
			
			set => 
				this._fraction = value / 100f;
		}

		public override int GetHashCode() => 
			this._fraction.GetHashCode();

		public override bool Equals(object obj) => 
			this._fraction == ((Percentage)obj)._fraction;

		public static bool operator == (Percentage a, Percentage b) => 
			a._fraction == b._fraction;

		public static bool operator != (Percentage a, Percentage b) => 
			a._fraction != b._fraction;

		public string ToString(int decmialplaces) => 
			this.Fraction.ToString("P" + decmialplaces.ToString());

		public override string ToString() => 
			this.Fraction.ToString("P");

		public static Percentage FromFraction(float fraction) => 
			new Percentage(fraction);

		public static Percentage FromPercent(float percent) => 
			new Percentage(percent / 100f);

		public static Percentage Parse(string str)
		{
			// str = str.TrimEnd(new char[] { ' ', '\t', '\n', '%' });
			str = str.TrimEnd(' ', '\t', '\n', '%');
			
			return new Percentage(float.Parse(str) / 100f);
		}

		public static bool operator < (Percentage p1, Percentage p2) => 
			p1._fraction < p2._fraction;

		public static bool operator > (Percentage p1, Percentage p2) => 
			p1._fraction > p2._fraction;

		public static Percentage operator + (Percentage p1, Percentage p2) => 
			new Percentage(p1._fraction + p2._fraction);

		public static Percentage operator - (Percentage p1, Percentage p2) => 
			new Percentage(p1._fraction - p2._fraction);

		private Percentage(float fraction) => 
			this._fraction = fraction;
	}
}
