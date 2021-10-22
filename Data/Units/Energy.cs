using System;

namespace DNA.Data.Units
{
	[Serializable]
	public struct Energy
	{
		private const float joulesPerCalorie = 4.184f;
		private const float btusPerCalorie = 0.003965667f;
		
		private float _calories;

		public float KiloCalories
		{
			get
			{
				return this._calories * 0.001f;
			}
		}

		public float Calories
		{
			get
			{
				return this._calories;
			}
		}

		public static Energy Parse(string str)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return this.Calories.ToString() + " Cals";
		}

		public override int GetHashCode()
		{
			return this._calories.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return this._calories == ((Energy)obj)._calories;
		}

		public static bool operator == (Energy a, Energy b)
		{
			return a._calories == b._calories;
		}

		public static bool operator != (Energy a, Energy b)
		{
			return a._calories != b._calories;
		}

		public static Energy FromKilocalories(float kilocals)
		{
			return new Energy(kilocals * 1000f);
		}

		public static Energy FromCalories(float calories)
		{
			return new Energy(calories);
		}

		public static Energy operator + (Energy m1, Energy m2)
		{
			return Energy.FromCalories(m1._calories + m2._calories);
		}

		public static Energy operator - (Energy m1, Energy m2)
		{
			return Energy.FromCalories(m1._calories - m2._calories);
		}

		public static double operator / (Energy m1, Energy m2)
		{
			return (double)(m1._calories / m2._calories);
		}

		public static Energy operator * (Energy m1, float scalar)
		{
			return Energy.FromCalories(m1._calories * scalar);
		}

		public static Energy operator / (Energy m1, float scalar)
		{
			return Energy.FromCalories(m1._calories / scalar);
		}

		private Energy(float calories)
		{
			this._calories = calories;
		}
	}
}
