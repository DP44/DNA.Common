using System;

namespace DNA.Data.Units
{
	[Serializable]
	public struct Mass
	{
		private const float lbsPerGram = 0.00220462261f;
		private const float kgsPerGram = 0.001f;
		private const float ouncesPerGram = 0.03527396f;
		private const float milligramsPerGram = 1000f;
		private const float microgramsPerGram = 1000000f;

		private float _grams;

		public float Pounds
		{
			get
			{
				return this._grams * 0.00220462261f;
			}
		}

		public float KiloGrams
		{
			get
			{
				return this._grams * 0.001f;
			}
		}

		public float Grams
		{
			get
			{
				return this._grams;
			}
		}

		public float Milligrams
		{
			get
			{
				return this._grams * 1000f;
			}
		}

		public float Micrograms
		{
			get
			{
				return this._grams * 1000000f;
			}
		}

		public float Ounces
		{
			get
			{
				return this._grams * 0.03527396f;
			}
		}

		public override int GetHashCode()
		{
			return this._grams.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return this._grams == ((Mass)obj)._grams;
		}

		public static bool operator == (Mass a, Mass b)
		{
			return a._grams == b._grams;
		}

		public static bool operator != (Mass a, Mass b)
		{
			return a._grams != b._grams;
		}

		public static Mass Parse(string unit, float amount)
		{
			switch (unit)
			{
				case "oz":
				{
					return Mass.FromOunces(amount);
				}
				
				case "ounce":
				{
					return Mass.FromOunces(amount);
				}
				
				case "ounces":
				{
					return Mass.FromOunces(amount);
				}
				
				case "lbs":
				{
					return Mass.FromPounds(amount);
				}
				
				case "pound":
				{
					return Mass.FromPounds(amount);
				}
				
				case "pounds":
				{
					return Mass.FromPounds(amount);
				}
				
				case "g":
				{
					return Mass.FromGrams(amount);
				}
				
				case "gram":
				{
					return Mass.FromGrams(amount);
				}
				
				case "grams":
				{
					return Mass.FromGrams(amount);
				}
				
				case "kgs":
				{
					return Mass.FromKiloGrams(amount);
				}
				
				case "kilogram":
				{
					return Mass.FromKiloGrams(amount);
				}
				
				case "kilograms":
				{
					return Mass.FromKiloGrams(amount);
				}
			}

			throw new ArgumentException("Unreconized Unit " + unit);
		}

		public static Mass Parse(string str)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return this.Pounds.ToString() + " lbs";
		}

		public static Mass FromOunces(float oz)
		{
			return new Mass(oz / 0.03527396f);
		}

		public static Mass FromGrams(float grams)
		{
			return new Mass(grams);
		}

		public static Mass operator + (Mass m1, Mass m2)
		{
			return Mass.FromGrams(m1.Grams + m2.Grams);
		}

		public static Mass operator - (Mass m1, Mass m2)
		{
			return Mass.FromGrams(m1.Grams - m2.Grams);
		}

		public static double operator / (Mass m1, Mass m2)
		{
			return (double)(m1._grams / m2._grams);
		}

		public static Mass operator * (Mass m1, float scalar)
		{
			return Mass.FromGrams(m1.Grams * scalar);
		}

		public static Mass operator / (Mass m1, float scalar)
		{
			return Mass.FromGrams(m1.Grams / scalar);
		}

		public static Mass FromPounds(float pounds)
		{
			return new Mass(pounds / 0.00220462261f);
		}

		public static Mass FromKiloGrams(float kilograms)
		{
			return new Mass(kilograms / 0.001f);
		}

		private Mass(float grams)
		{
			this._grams = grams;
		}
	}
}
