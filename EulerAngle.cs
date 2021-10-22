using System;
using Microsoft.Xna.Framework;

namespace DNA
{
	public struct EulerAngle
	{
		private Angle _yaw;

		private Angle _pitch;

		private Angle _roll;

		public Angle Yaw
		{
			get
			{
				return this._yaw;
			}
			set
			{
				this._yaw = value;
			}
		}

		public Angle Pitch
		{
			get
			{
				return this._pitch;
			}
			set
			{
				this._pitch = value;
			}
		}

		public Angle Roll
		{
			get
			{
				return this._roll;
			}
			set
			{
				this._roll = value;
			}
		}

		public EulerAngle(Angle yaw, Angle pitch, Angle roll)
		{
			this._yaw = yaw;
			this._pitch = pitch;
			this._roll = roll;
		}

		public EulerAngle(Quaternion q)
		{
			Vector3 zero = Vector3.Zero;
			float rads = (float)Math.Atan2((double)(2f * q.Y * q.W - 2f * q.X * q.Z), 1.0 - 2.0 * Math.Pow((double)q.Y, 2.0) - 2.0 * Math.Pow((double)q.Z, 2.0));
			float rads2 = (float)Math.Asin((double)(2f * q.X * q.Y + 2f * q.Z * q.W));
			float rads3 = (float)Math.Atan2((double)(2f * q.X * q.W - 2f * q.Y * q.Z), 1.0 - 2.0 * Math.Pow((double)q.X, 2.0) - 2.0 * Math.Pow((double)q.Z, 2.0));
			if ((double)(q.X * q.Y + q.Z * q.W) == 0.5)
			{
				rads = (float)(2.0 * Math.Atan2((double)q.X, (double)q.W));
				rads3 = 0f;
			}
			else if ((double)(q.X * q.Y + q.Z * q.W) == -0.5)
			{
				rads = (float)(-2.0 * Math.Atan2((double)q.X, (double)q.W));
				rads3 = 0f;
			}
			this._yaw = Angle.FromRadians(rads);
			this._pitch = Angle.FromRadians(rads3);
			this._roll = Angle.FromRadians(rads2);
		}

		public override bool Equals(object obj)
		{
			return obj.GetType() == typeof(EulerAngle) && this == (EulerAngle)obj;
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}

		public static bool operator ==(EulerAngle a, EulerAngle b)
		{
			throw new NotImplementedException();
		}

		public static bool operator !=(EulerAngle a, EulerAngle b)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"yaw:",
				this._yaw.ToString(),
				" pitch:",
				this._pitch.ToString(),
				" roll:",
				this._roll.ToString()
			});
		}
	}
}
