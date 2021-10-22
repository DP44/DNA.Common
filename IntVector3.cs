using System;
using System.IO;
using Microsoft.Xna.Framework;

namespace DNA 
{
	public struct IntVector3 
	{
		public static IntVector3 Zero = new IntVector3(0, 0, 0);

		public int X;
		public int Y;
		public int Z;

		public IntVector3(int x, int y, int z) 
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		public void SetValues(int x, int y, int z) 
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		public void Write(BinaryWriter writer) 
		{
			writer.Write(this.X);
			writer.Write(this.Y);
			writer.Write(this.Z);
		}

		public static IntVector3 Read(BinaryReader reader) =>
			new IntVector3(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());

		public static IntVector3 Subtract(IntVector3 a, int x, int y, int z) =>
			new IntVector3(a.X - x, a.Y - y, a.Z - z);

		public static IntVector3 Subtract(int x, int y, int z, IntVector3 b) =>
			new IntVector3(x - b.X, y - b.Y, z - b.Z);

		public static IntVector3 Subtract(IntVector3 a, IntVector3 b) =>
			new IntVector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

		public static void Subtract(ref IntVector3 a, ref IntVector3 b, 
									out IntVector3 result) =>
			result = new IntVector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

		public static IntVector3 Add(IntVector3 a, int x, int y, int z) =>
			new IntVector3(a.X + x, a.Y + y, a.Z + z);

		public static IntVector3 Add(IntVector3 a, IntVector3 b) =>
			new IntVector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

		public static void Add(ref IntVector3 a, ref IntVector3 b, 
							   out IntVector3 result) =>
			result = new IntVector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

		public static IntVector3 FromVector3(Vector3 v) =>
			new IntVector3((int)Math.Floor((double)v.X), 
						   (int)Math.Floor((double)v.Y), 
						   (int)Math.Floor((double)v.Z));

		public static Vector3 ToVector3(IntVector3 a) =>
			new Vector3((float)a.X, (float)a.Y, (float)a.Z);

		public static bool operator == (IntVector3 v1, IntVector3 v2) =>
			v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z;

		public static bool operator != (IntVector3 v1, IntVector3 v2) =>
			v1.X != v2.X || v1.Y != v2.Y || v1.Z != v2.Z;

		public static bool operator == (IntVector3 v1, Vector3 v2) =>
			(float)v1.X == v2.X && (float)v1.Y == v2.Y && (float)v1.Z == v2.Z;

		public static bool operator != (IntVector3 v1, Vector3 v2) =>
			(float)v1.X != v2.X || (float)v1.Y != v2.Y || (float)v1.Z != v2.Z;

		public int LengthSquared() =>
			this.X * this.X + this.Y * this.Y + this.Z * this.Z;

		public static int DistanceSquared(IntVector3 v1, IntVector3 v2) =>
			MathTools.Square(v1.X - v2.X) + 
				MathTools.Square(v1.Y - v2.Y) + 
				MathTools.Square(v1.Z - v2.Z);

		public override bool Equals(object obj) 
		{
			if (obj is IntVector3) 
			{
				return this == (IntVector3)obj;
			}

			return obj is Vector3 && this == (Vector3)obj;
		}

		public IntVector3 Abs() =>
			new IntVector3(Math.Abs(this.X), Math.Abs(this.Y), Math.Abs(this.Z));

		public override int GetHashCode() =>
			this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Z.GetHashCode();

		public static IntVector3 Abs(IntVector3 vec) =>
			vec.Abs();

		public static Vector3 operator * (float v, IntVector3 vect) =>
			new Vector3((float)vect.X * v, (float)vect.Y * v, (float)vect.Z * v);

		public static Vector3 operator * (IntVector3 vect, float v) =>
			new Vector3((float)vect.X * v, (float)vect.Y * v, (float)vect.Z * v);

		public static Vector3 operator / (IntVector3 vect, float v) =>
			new Vector3((float)vect.X / v, (float)vect.Y / v, (float)vect.Z / v);

		public static IntVector3 operator + (IntVector3 v1, IntVector3 v2) =>
			new IntVector3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);

		public static IntVector3 operator - (IntVector3 v1, IntVector3 v2) =>
			new IntVector3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);

		public static IntVector3 operator * (IntVector3 vect, int v) =>
			new IntVector3(vect.X * v, vect.Y * v, vect.Z * v);

		public static IntVector3 operator / (IntVector3 vect, int v) =>
			new IntVector3(vect.X / v, vect.Y / v, vect.Z / v);

		public static implicit operator Vector3(IntVector3 vect) =>
			new Vector3((float)vect.X, (float)vect.Y, (float)vect.Z);

		public static explicit operator IntVector3(Vector3 vect) =>
			new IntVector3((int)Math.Floor((double)vect.X), 
						   (int)Math.Floor((double)vect.Y), 
						   (int)Math.Floor((double)vect.Z));

		public bool Equals(IntVector3 other) =>
			other.X == this.X && other.Y == this.Y && other.Z == this.Z;

		public void SetToMin(IntVector3 a) 
		{
			this.X = ((a.X < this.X) ? a.X : this.X);
			this.Y = ((a.Y < this.Y) ? a.Y : this.Y);
			this.Z = ((a.Z < this.Z) ? a.Z : this.Z);
		}

		public void SetToMax(IntVector3 a) 
		{
			this.X = ((a.X > this.X) ? a.X : this.X);
			this.Y = ((a.Y > this.Y) ? a.Y : this.Y);
			this.Z = ((a.Z > this.Z) ? a.Z : this.Z);
		}

		public static IntVector3 Min(IntVector3 a, IntVector3 b) =>
			new IntVector3((a.X <= b.X) ? a.X : b.X, 
						   (a.Y <= b.Y) ? a.Y : b.Y, 
						   (a.Z <= b.Z) ? a.Z : b.Z);

		public static IntVector3 Max(IntVector3 a, IntVector3 b) =>
			new IntVector3((a.X >= b.X) ? a.X : b.X, 
						   (a.Y >= b.Y) ? a.Y : b.Y, 
						   (a.Z >= b.Z) ? a.Z : b.Z);

		public static IntVector3 Clamp(IntVector3 value, IntVector3 min, IntVector3 max) =>
			IntVector3.Min(IntVector3.Max(value, min), max);

		public static void FillBoundingCorners(IntVector3 min, IntVector3 max, 
											   ref Vector3[] bounds)
		{
			bounds[0] = new Vector3((float)min.X, (float)min.Y, (float)min.Z);
			bounds[1] = new Vector3((float)min.X, (float)min.Y, (float)max.Z);
			bounds[2] = new Vector3((float)max.X, (float)min.Y, (float)min.Z);
			bounds[3] = new Vector3((float)max.X, (float)min.Y, (float)max.Z);
			bounds[4] = new Vector3((float)min.X, (float)max.Y, (float)min.Z);
			bounds[5] = new Vector3((float)min.X, (float)max.Y, (float)max.Z);
			bounds[6] = new Vector3((float)max.X, (float)max.Y, (float)min.Z);
			bounds[7] = new Vector3((float)max.X, (float)max.Y, (float)max.Z);
		}

		public static void FillBoundingCorners(IntVector3 min, IntVector3 max, 
											   ref IntVector3[] bounds)
		{
			bounds[0] = new IntVector3(min.X, min.Y, min.Z);
			bounds[1] = new IntVector3(min.X, min.Y, max.Z);
			bounds[2] = new IntVector3(max.X, min.Y, min.Z);
			bounds[3] = new IntVector3(max.X, min.Y, max.Z);
			bounds[4] = new IntVector3(min.X, max.Y, min.Z);
			bounds[5] = new IntVector3(min.X, max.Y, max.Z);
			bounds[6] = new IntVector3(max.X, max.Y, min.Z);
			bounds[7] = new IntVector3(max.X, max.Y, max.Z);
		}

		public override string ToString() =>
			string.Concat(this.X.ToString(), ",", this.Y.ToString(), ",", this.Z.ToString());

		public int Dot(IntVector3 o) =>
			this.X * o.X + this.Y * o.Y + this.Z * o.Z;
	}
}
