using System;

namespace DNA.Data.Units
{
	[Serializable]
	public struct Area
	{
		private float _squareMeters;

		public float SquareMeters
		{
			get
			{
				return this._squareMeters;
			}
		}

		public static Area Parse(string str)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return this.SquareMeters.ToString() + " M^2";
		}

		public static Area FromSquareMeters(float squareMeters)
		{
			return new Area(squareMeters);
		}

		private Area(float squareMeters)
		{
			this._squareMeters = squareMeters;
		}

		public override int GetHashCode()
		{
			return this._squareMeters.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return this._squareMeters == ((Area)obj)._squareMeters;
		}

		public static bool operator == (Area a, Area b)
		{
			return a._squareMeters == b._squareMeters;
		}

		public static bool operator != (Area a, Area b)
		{
			return a._squareMeters != b._squareMeters;
		}
	}
}
