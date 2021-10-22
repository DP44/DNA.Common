using System;
using Microsoft.Xna.Framework;

namespace DNA
{
	public struct Polar3
	{
		private float _radius;

		private Angle _zenith;

		private Angle _azimuth;

		public float Radius
		{
			get
			{
				return this._radius;
			}
		}

		public Angle Zenith
		{
			get
			{
				return this._zenith;
			}
		}

		public Angle Azimuth
		{
			get
			{
				return this._azimuth;
			}
		}

		public Vector3 GetCartesian()
		{
			float x = (float)((double)this.Radius * Math.Sin((double)this.Zenith.Radians) * Math.Cos((double)this.Azimuth.Radians));
			float y = (float)((double)this.Radius * Math.Sin((double)this.Zenith.Radians) * Math.Sin((double)this.Azimuth.Radians));
			float z = (float)((double)this.Radius * Math.Cos((double)this.Zenith.Radians));
			return new Vector3(x, y, z);
		}

		public float DistanceTo(Polar3 p)
		{
			Vector3 cartesian = this.GetCartesian();
			Vector3 cartesian2 = p.GetCartesian();
			return Vector3.Distance(cartesian, cartesian2);
		}

		public float ArcLength(Polar3 p3)
		{
			Vector3 cartesian = this.GetCartesian();
			Vector3 cartesian2 = p3.GetCartesian();
			Angle angle = cartesian.AngleBetween(cartesian2);
			float num = (float)((double)(2f * p3.Radius) * 3.1415926535897931);
			return angle.Revolutions * num;
		}

		public Polar3(Angle zenith, Angle azimuth, float radius)
		{
			this._radius = radius;
			this._zenith = zenith;
			this._azimuth = azimuth;
		}

		public override bool Equals(object obj)
		{
			return obj.GetType() == typeof(Polar3) && this == (Polar3)obj;
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}

		public static bool operator == (Polar3 a, Polar3 b)
		{
			throw new NotImplementedException();
		}

		public static bool operator != (Polar3 a, Polar3 b)
		{
			throw new NotImplementedException();
		}
	}
}
