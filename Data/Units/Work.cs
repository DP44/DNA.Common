using System;

namespace DNA.Data.Units
{
	[Serializable]
	public struct Work
	{
		private float _watts;

		public float Watts
		{
			get
			{
				return this._watts;
			}
			set
			{
				this._watts = value;
			}
		}

		public override int GetHashCode()
		{
			return this._watts.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			Work work = (Work)obj;
			return this._watts == work._watts;
		}

		public static bool operator == (Work a, Work b)
		{
			return a._watts == b._watts;
		}

		public static bool operator != (Work a, Work b)
		{
			return a._watts != b._watts;
		}
	}
}
