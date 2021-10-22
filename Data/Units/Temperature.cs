using System;

namespace DNA.Data.Units
{
	[Serializable]
	public struct Temperature
	{
		private float _celsiusDegrees;

		public float Fahrenheit
		{
			get
			{
				return this._celsiusDegrees * 9f / 5f + 32f;
			}
		}

		public float Celsius
		{
			get
			{
				return this._celsiusDegrees;
			}
		}

		public float Kelvin
		{
			get
			{
				return this._celsiusDegrees + 273f;
			}
		}

		public static Temperature Parse(string str)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return this.Celsius.ToString() + " C";
		}

		public static Temperature FromCelsius(float celsius)
		{
			return new Temperature(celsius);
		}

		private Temperature(float celsius)
		{
			this._celsiusDegrees = celsius;
		}

		public override int GetHashCode()
		{
			return this._celsiusDegrees.GetHashCode();
		}

		public bool Equals(Temperature other)
		{
			return this._celsiusDegrees == other._celsiusDegrees;
		}

		public override bool Equals(object obj)
		{
			return obj.GetType() == typeof(Temperature) && this.Equals((Temperature)obj);
		}

		public static bool operator == (Temperature a, Temperature b)
		{
			return a.Equals(b);
		}

		public static bool operator != (Temperature a, Temperature b)
		{
			return !a.Equals(b);
		}
	}
}
