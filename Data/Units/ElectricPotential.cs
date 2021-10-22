using System;

namespace DNA.Data.Units
{
	[Serializable]
	public struct ElectricPotential
	{
		private float _volts;

		public float Volts
		{
			get
			{
				return this._volts;
			}
			set
			{
				this._volts = value;
			}
		}

		public override int GetHashCode()
		{
			return this._volts.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return this._volts == ((ElectricPotential)obj)._volts;
		}

		public static bool operator == (ElectricPotential a, ElectricPotential b)
		{
			return a._volts == b._volts;
		}

		public static bool operator != (ElectricPotential a, ElectricPotential b)
		{
			return a._volts != b._volts;
		}
	}
}
