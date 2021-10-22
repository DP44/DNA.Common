using Microsoft.Xna.Framework;
using System;

namespace DNA
{
	public struct Angle
	{
		public static readonly Angle Zero = Angle.FromRadians(0.0f);
		public static readonly Angle UnitCircle = Angle.FromRevolutions(1f);
		
		private float _radians;

		public float Radians
		{
			get => 
				this._radians;
			
			set => 
				this._radians = value;
		}

		public float Degrees
		{
			get => 
				(float)((double)this._radians * 180.0 / 3.14159274101257);
			
			set => 
				this._radians = (float)((double)value * 3.14159274101257 / 180.0);
		}

		public double Sin => Math.Sin((double)this._radians);
		public double Cos => Math.Cos((double)this._radians);
		public double Tan => Math.Tan((double)this._radians);

		public static Angle ASin(double value)			=> new Angle((float)Math.Asin(value));
		public static Angle ACos(double value)			=> new Angle((float)Math.Acos(value));
		public static Angle ATan(double value)			=> new Angle((float)Math.Atan(value));
		public static Angle ATan2(double y, double x)	=> new Angle((float)Math.Atan2(y, x));
		
		public static Angle Lerp(Angle a, Angle b, float t)	=> 
			Angle.FromRadians((b.Radians - a.Radians) * t + a.Radians);

		public float Revolutions
		{
			get => 
				this._radians / 6.283185f;
		
			set => 
				this._radians = value * 6.283185f;
		}

		public void Normalize() => 
			this._radians = MathTools.Mod(this._radians, 6.283185f);

		public static double DegreesToRadians(double degs) => 
			degs * Math.PI / 180.0;
		
		public static float DegreesToRadians(float degs) => 
			(float)((double)degs * 3.14159274101257 / 180.0);
		
		public static float RadiansToDegrees(float rads) => 
			(float)((double)rads * 180.0 / 3.14159274101257);

		public static Angle FromLocations(Point pivot, Point point)
		{
			int num1 = point.X - pivot.X;
			int num2 = point.Y - pivot.Y;
			float num3 = (float)num2 / (float)num1;
			float rads;
			
			if (num1 == 0)
			{
				rads = num2 <= 0 ? (num2 >= 0 ? 0.0f : -1.570796f) : 1.570796f;
			}
			else
			{
				rads = (float)Math.Atan((double)num3);
				
				if (num1 < 0)
				{
					rads += 3.141593f;
				}
			}
			
			Angle angle = Angle.FromRadians(rads);
			angle.Normalize();
			return angle;
		}

		public static Angle FromRadians(double rads)	=> new Angle((float) rads);
		public static Angle FromDegrees(double degs)	=> new Angle(Angle.DegreesToRadians(degs));
		public static Angle FromRadians(float rads)		=> new Angle(rads);
		public static Angle FromDegrees(float degs)		=> new Angle(Angle.DegreesToRadians(degs));
		public static Angle FromRevolutions(float revs)	=> new Angle(revs * 6.283185f);

		public static Angle operator + (Angle a, Angle b)	=> new Angle(a._radians + b._radians);
		public static Angle operator - (Angle a)			=> new Angle(-a._radians);
		public static Angle operator - (Angle a, Angle b)	=> new Angle(a._radians - b._radians);
		public static Angle operator / (Angle a, float b)	=> new Angle(a._radians / b);
		public static Angle operator / (Angle a, double b)	=> new Angle((double)a._radians / b);
		public static float operator / (Angle a, Angle b)	=> a._radians / b._radians;

		public static bool operator < (Angle a, Angle b) => 
			(double)a._radians < (double)b._radians;

		public static bool operator > (Angle a, Angle b) => 
			(double)a._radians > (double)b._radians;

		public static Angle operator * (float b, Angle a)	=> new Angle(a._radians * b);
		public static Angle operator * (double b, Angle a)	=> new Angle((double)a._radians * b);
		public static Angle operator * (Angle a, float b)	=> new Angle(a._radians * b);
		public static Angle operator * (Angle a, double b)	=> new Angle((double)a._radians * b);

		private Angle(double radians)		=> this._radians = (float)radians;
		private Angle(float radians)		=> this._radians = radians;
		public override string ToString()	=> this.Degrees.ToString();
		public static Angle Parse(string s)	=> Angle.FromDegrees(float.Parse(s));
		public override int GetHashCode()	=> this._radians.GetHashCode();

		public override bool Equals(object obj) => 
			(double)this._radians == (double)((Angle) obj)._radians;

		public static bool operator == (Angle a, Angle b) => 
			(double)a._radians == (double)b._radians;
	
		public static bool operator != (Angle a, Angle b) => 
			(double)a._radians != (double)b._radians;
	}
}