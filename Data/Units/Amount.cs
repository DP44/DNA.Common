using System;

namespace DNA.Data.Units
{
	[Serializable]
	public struct Amount
	{
		public const float Mole = 6.02214136E+23f;

		private float _value;

		public static Amount FromMoles(float moles)
		{
			return new Amount(moles * 6.02214136E+23f);
		}

		public Amount(float value)
		{
			this._value = value;
		}

		public float Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this._value = value;
			}
		}

		public float Moles
		{
			get
			{
				return this._value / 6.02214136E+23f;
			}
			set
			{
				this._value = value * 6.02214136E+23f;
			}
		}

		public override int GetHashCode()
		{
			return this._value.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return this._value == ((Amount)obj)._value;
		}

		public static bool operator == (Amount a, Amount b)
		{
			return a._value == b._value;
		}

		public static bool operator != (Amount a, Amount b)
		{
			return a._value != b._value;
		}
	}
}
