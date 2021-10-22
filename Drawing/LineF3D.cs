using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing
{
	public struct LineF3D
	{
		public Vector3 Start;
		public Vector3 End;

		public float LengthSquared => 
			(this.End - this.Start).LengthSquared();

		public Vector3 Center => 
			0.5f * (this.Start + this.End);

		public Vector3 Direction
		{
			get
			{
				Vector3 result = this.End - this.Start;
				result.Normalize();
				return result;
			}
		}

		public float Length => 
			(this.End - this.Start).Length();


		public LineF3D(Vector3 start, Vector3 end)
		{
			this.Start = start;
			this.End = end;
		}

		public Vector3 GetValue(float t) => 
			(this.End - this.Start) * t + this.Start;

		public float DistanceTo(Vector3 point) => 
			Vector3.Distance(this.ClosetPointTo(point), point);


		public Vector3 ClosetPointTo(Vector3 point)
		{
			Vector3 vector = this.End - this.Start;
			Vector3 vector2 = point - this.Start;
		
			float num = Vector3.Dot(vector, vector2);
		
			if (num <= 0f)
			{
				return this.Start;
			}
		
			float num2 = Vector3.Dot(vector2, vector2);
			
			if (num2 >= num)
			{
				return this.End;
			}
		
			float scaleFactor = num2 / num;
			return this.Start + vector * scaleFactor;
		}

		public float? Intersects(Plane plane)
		{
			float num = plane.DotCoordinate(this.Start);
			float num2 = num - plane.DotCoordinate(this.End);
		
			if (num2 != 0f)
			{
				float num3 = num / num2;
		
				if ((double)num3 >= 0.0 && num3 <= 1f)
				{
					return new float?(num3);
				}
			}
		
			return null;
		}

		public bool Intersects(Plane plane, out float t, 
							   out bool parallel, int precisionDigits)
		{
			double num = (double)plane.DotCoordinate(this.Start);
			double num2 = num - (double)plane.DotCoordinate(this.End);
			if (Math.Round(num2, precisionDigits) == 0.0)
			{
				t = float.NaN;
				parallel = true;
				return num == 0.0;
			}
			parallel = false;
			double value = num / num2;
			t = (float)Math.Round(value, precisionDigits);
			return (double)t >= 0.0 && t <= 1f;
		}

		public bool Intersects(Triangle3D triangle, out float t, out bool parallel)
		{
			Vector3 vector = triangle.B - triangle.A;
			Vector3 vector2 = triangle.C - triangle.A;
			Vector3 vector3 = Vector3.Cross(vector, vector2);
			Vector3 vector4 = this.End - this.Start;
			Vector3 vector5 = this.Start - triangle.A;
			float num = -Vector3.Dot(vector3, vector5);
			float num2 = Vector3.Dot(vector3, vector4);
			if (num2 == 0f)
			{
				parallel = true;
				if (num == 0f)
				{
					t = 0f;
					return true;
				}
				t = float.NaN;
				return false;
			}
			else
			{
				parallel = false;
				t = num / num2;
				if (t < 0f || t > 1f)
				{
					return false;
				}
				Vector3 value = this.Start + t * vector4;
				float num3 = Vector3.Dot(vector, vector);
				float num4 = Vector3.Dot(vector, vector2);
				float num5 = Vector3.Dot(vector2, vector2);
				Vector3 vector6 = value - triangle.A;
				float num6 = Vector3.Dot(vector6, vector);
				float num7 = Vector3.Dot(vector6, vector2);
				float num8 = num4 * num4 - num3 * num5;
				float num9 = (num4 * num7 - num5 * num6) / num8;
				if ((double)num9 < 0.0 || (double)num9 > 1.0)
				{
					return false;
				}
				float num10 = (num4 * num6 - num3 * num7) / num8;
				return (double)num10 >= 0.0 && (double)(num9 + num10) <= 1.0;
			}
		}

		// TODO: Reverse engineer control flow.
		public int Intersects(Capsule capsule, out float? t1, out float? t2)
		{
			/*
				Ray ray = new Ray(this.Start, this.End - this.Start);
				int num = ray.Intersects(capsule, out t1, out t2);
				if (t1 != null)
				{
					float? num2 = t1;
					if (num2.GetValueOrDefault() < 0f && num2 != null)
					{
						goto IL_74;
					}
				}
				float? num3 = t1;
				if (num3.GetValueOrDefault() <= 1f || num3 == null)
				{
					goto IL_7F;
				}
				IL_74:
				num--;
				t1 = null;
				IL_7F:
				if (t2 != null)
				{
					float? num4 = t2;
					if (num4.GetValueOrDefault() < 0f && num4 != null)
					{
						goto IL_CD;
					}
				}
				float? num5 = t2;
				if (num5.GetValueOrDefault() <= 1f || num5 == null)
				{
					goto IL_D8;
				}
				IL_CD:
				num--;
				t2 = null;
				IL_D8:
				if (t1 == null && t2 != null)
				{
					t1 = t2;
					t2 = null;
				}
				return num;
			*/
		}
		
		public bool Intersects(BoundingSphere sphere) => 
			this.Intersects(sphere, out float _, out float _);

		public bool Intersects(BoundingSphere sphere, out float t1, out float t2)
		{
			Vector3 center = sphere.Center;
			float radius = sphere.Radius;
		
			Vector3 vector = this.End - this.Start;
			Vector3 vector2 = this.Start - center;
		
			double num = (double)vector.LengthSquared();
			double num2 = (double)(2f * Vector3.Dot(vector, vector2));
			
			double num3 = (double)(center.LengthSquared() + 
				this.Start.LengthSquared() - 2f * Vector3.Dot(
					center, this.Start) - radius * radius);
		
			double num4 = num2 * num2 - 4.0 * num * num3;
			
			if (num == 0.0 || num4 < 0.0)
			{
				t1 = 0f;
				t2 = 0f;
				return false;
			}
			
			double num5 = Math.Sqrt(num4);
		
			t1 = (float)((-(float)num2 + num5) / (2.0 * num));
			t2 = (float)((-(float)num2 - num5) / (2.0 * num));
			
			if (t1 >= 0f && t1 <= 1f)
			{
				return true;
			}
		
			if (t2 >= 0f && t2 <= 1f)
			{
				t1 = t2;
				return true;
			}
	
			return false;
		}

		public bool Intersects(Plane plane, out Vector3 pos, 
							   out bool parallel, int precisionDigits)
		{
			float t;
			bool result = this.Intersects(plane, out t, out parallel, precisionDigits);
			
			if (parallel)
			{
				pos = new Vector3(float.NaN, float.NaN, float.NaN);
				return result;
			}
		
			pos = this.GetValue(t);
			return result;
		}

		public bool Intersects(LineF3D line, out float intersection) => 
			throw new NotImplementedException();

		public bool Intersects(LineF3D line, out Vector3 intersection) => 
			throw new NotImplementedException();

		public override string ToString() => 
			"(" + this.Start.X + "," + this.Start.Y + "," + this.Start.Z + ")-(" + 
			this.End.X + "," + this.End.Y + "," + this.End.Z + ")";
	}
}
