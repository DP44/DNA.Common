using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Drawing2D
{
	public struct LineF2D : IShape2D, ICloneable
	{
		public Vector2 _start;

		public Vector2 _end;

		public Vector2 Start
		{
			get
			{
				return this._start;
			}
			set
			{
				this._start = value;
			}
		}

		public Vector2 End
		{
			get
			{
				return this._end;
			}
			set
			{
				this._end = value;
			}
		}

		public bool IsVertical
		{
			get
			{
				return this._end.X == this._start.X;
			}
		}

		public bool IsHorizonal
		{
			get
			{
				return this._end.Y == this._start.Y;
			}
		}

		public double LengthSq
		{
			get
			{
				return (double)(this.End.X * this.End.X + this.Start.X * this.Start.X);
			}
		}

		public double Length
		{
			get
			{
				return (double)Vector2.Distance(this.Start, this.End);
			}
		}

		public RectangleF BoundingBox
		{
			get
			{
				return this.ComputeBoundingBox();
			}
		}

		private RectangleF ComputeBoundingBox()
		{
			float x;
			float x2;
			if (this._start.X < this._end.X)
			{
				x = this._start.X;
				x2 = this._end.X;
			}
			else
			{
				x = this._end.X;
				x2 = this._start.X;
			}
			float y;
			float y2;
			if (this._start.Y < this._end.Y)
			{
				y = this._start.Y;
				y2 = this._end.Y;
			}
			else
			{
				y = this._end.Y;
				y2 = this._start.Y;
			}
			return new RectangleF(x, y, x2 - x, y2 - y);
		}

		public LineF2D(float sx, float sy, float ex, float ey)
		{
			this._start = new Vector2(sx, sy);
			this._end = new Vector2(ex, ey);
		}

		public LineF2D(Vector2 start, Vector2 end)
		{
			this._start = start;
			this._end = end;
		}

		public Vector2 GetValue(float t)
		{
			float x = (this._end.X - this._start.X) * t + this._start.X;
			float y = (this._end.Y - this._start.Y) * t + this._start.Y;
			return new Vector2(x, y);
		}

		public float DistanceSquare()
		{
			float num = this.End.Y - this.Start.Y;
			float num2 = this.End.X - this.Start.X;
			return num * num + num2 * num2;
		}

		public float Intersect(Vector2 pointOnLine)
		{
			if (this.IsVertical)
			{
				float num = this._end.Y - this._start.Y;
				return (pointOnLine.Y - this._start.Y) / num;
			}
			float num2 = this._end.X - this._start.X;
			return (pointOnLine.X - this._start.X) / num2;
		}

		public bool HorizontalIntersect(float x, out float y)
		{
			float num = this._end.X - this._start.X;
			if (num == 0f)
			{
				y = float.NaN;
				return false;
			}
			float num2 = (x - this._start.X) / num;
			y = this._start.Y + num2 * (this._end.Y - this._start.Y);
			return num2 >= 0f && num2 <= 1f;
		}

		public bool VerticalIntersect(float y, out float x)
		{
			float num = this._end.Y - this._start.Y;
			if (num == 0f)
			{
				x = float.NaN;
				return false;
			}
			float num2 = (y - this._start.Y) / num;
			x = this._start.X + num2 * (this._end.X - this._start.X);
			return num2 >= 0f && num2 <= 1f;
		}

		public bool Intersects(LineF2D line)
		{
			Vector2 vector;
			bool flag;
			return this.Intersects(line, out vector, out flag);
		}

		public bool Intersects(Circle circle, out float t1, out float t2)
		{
			Vector2 center = circle.Center;
			float radius = circle.Radius;
			Vector2 value = this._end - this._start;
			Vector2 value2 = this._start - center;
			double num = (double)value.LengthSquared();
			double num2 = (double)(2f * Vector2.Dot(value, value2));
			double num3 = (double)(center.LengthSquared() + this._start.LengthSquared() - 2f * Vector2.Dot(center, this._start) - radius * radius);
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

		public bool Intersects(Vector2 start, Vector2 end, out float t, out bool coincident)
		{
			coincident = false;
			Vector2 v = new Vector2(this._end.X - this._start.X, this._end.Y - this._start.Y);
			Vector2 vector = new Vector2(end.X - start.X, end.Y - start.Y);
			if (v.LengthSquared() == 0f || vector.LengthSquared() == 0f)
			{
				t = -1f;
				return false;
			}
			Vector2 v2 = new Vector2(this._start.X - start.X, this._start.Y - start.Y);
			float num = vector.Cross(v2);
			float num2 = v.Cross(vector);
			float num3 = v.Cross(v2);
			if (num2 == 0f)
			{
				t = -1f;
				if (num == 0f && num3 == 0f)
				{
					coincident = true;
					return true;
				}
				return false;
			}
			else
			{
				float num4 = num / num2;
				float num5 = num3 / num2;
				if (num4 >= 0f && num4 <= 1f && num5 >= 0f && num5 <= 1f)
				{
					t = num4;
					return true;
				}
				t = -1f;
				return false;
			}
		}

		public bool Intersects(LineF2D line, out float t, out bool coincident)
		{
			coincident = false;
			Vector2 v = new Vector2(this._end.X - this._start.X, this._end.Y - this._start.Y);
			Vector2 vector = new Vector2(line._end.X - line._start.X, line._end.Y - line._start.Y);
			if (v.LengthSquared() == 0f || vector.LengthSquared() == 0f)
			{
				t = -1f;
				return false;
			}
			Vector2 v2 = new Vector2(this._start.X - line._start.X, this._start.Y - line._start.Y);
			float num = vector.Cross(v2);
			float num2 = v.Cross(vector);
			float num3 = v.Cross(v2);
			if (num2 == 0f)
			{
				t = -1f;
				if (num == 0f && num3 == 0f)
				{
					coincident = true;
					return true;
				}
				return false;
			}
			else
			{
				float num4 = num / num2;
				float num5 = num3 / num2;
				if (num4 >= 0f && num4 <= 1f && num5 >= 0f && num5 <= 1f)
				{
					t = num4;
					return true;
				}
				t = -1f;
				return false;
			}
		}

		public bool Intersects(LineF2D line, out Vector2 intersection, out bool coincident)
		{
			coincident = false;
			Vector2 v = new Vector2(this._end.X - this._start.X, this._end.Y - this._start.Y);
			Vector2 vector = new Vector2(line._end.X - line._start.X, line._end.Y - line._start.Y);
			if (v.LengthSquared() == 0f || vector.LengthSquared() == 0f)
			{
				intersection = Vector2.Zero;
				return false;
			}
			Vector2 v2 = new Vector2(this._start.X - line._start.X, this._start.Y - line._start.Y);
			float num = vector.Cross(v2);
			float num2 = v.Cross(vector);
			float num3 = v.Cross(v2);
			if (num2 == 0f)
			{
				intersection = Vector2.Zero;
				if (num == 0f && num3 == 0f)
				{
					coincident = true;
					return true;
				}
				return false;
			}
			else
			{
				float num4 = num / num2;
				float num5 = num3 / num2;
				if (num4 >= 0f && num4 <= 1f && num5 >= 0f && num5 <= 1f)
				{
					intersection = new Vector2(this._start.X + num4 * v.X, this._start.Y + num4 * v.Y);
					return true;
				}
				intersection = Vector2.Zero;
				return false;
			}
		}

		public Vector2 ShortestVectorTo(Vector2 pnt)
		{
			double num = (double)Vector2.DistanceSquared(this._start, this._end);
			double num2;
			if (num != 0.0)
			{
				num2 = (double)((pnt.X - this._start.X) * (this._end.X - this._start.X) + (pnt.Y - this._start.Y) * (this._end.Y - this._start.Y)) / num;
			}
			else
			{
				num2 = 0.0;
			}
			if (num2 < 0.0)
			{
				return this._start - pnt;
			}
			if (num2 > 1.0)
			{
				return this._end - pnt;
			}
			double num3 = (double)this._start.X + num2 * (double)(this._end.X - this._start.X);
			double num4 = (double)this._start.Y + num2 * (double)(this._end.Y - this._start.Y);
			double num5 = num3 - (double)pnt.X;
			double num6 = num4 - (double)pnt.Y;
			return new Vector2((float)num5, (float)num6);
		}

		public double DistanceTo(Vector2 pnt)
		{
			return (double)this.ShortestVectorTo(pnt).Length();
		}

		public override string ToString()
		{
			return this.Start.ToString() + "-" + this.End.ToString();
		}

		public Vector2 Center
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public float Area
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public OrientedBoundingRect GetOrientedBoundingRect()
		{
			throw new NotImplementedException();
		}

		public void Transform(Matrix mat)
		{
			throw new NotImplementedException();
		}

		public bool Touches(IShape2D s)
		{
			throw new NotImplementedException();
		}

		public bool IsAbove(Vector2 p)
		{
			throw new NotImplementedException();
		}

		public float DistanceTo(IShape2D poly)
		{
			throw new NotImplementedException();
		}

		public float DistanceSquaredTo(IShape2D poly)
		{
			throw new NotImplementedException();
		}

		public bool IsBelow(Vector2 p)
		{
			throw new NotImplementedException();
		}

		public bool IsLeftOf(Vector2 p)
		{
			throw new NotImplementedException();
		}

		public bool IsRightOf(Vector2 p)
		{
			throw new NotImplementedException();
		}

		public bool CompletelyContains(IShape2D region)
		{
			throw new NotImplementedException();
		}

		public bool Contains(Vector2 p)
		{
			throw new NotImplementedException();
		}

		public int Contains(IList<Vector2> points)
		{
			throw new NotImplementedException();
		}

		public IShape2D Intersection(IShape2D s)
		{
			throw new NotImplementedException();
		}

		public bool BoundsIntersect(IShape2D shape)
		{
			throw new NotImplementedException();
		}

		public float Contains(IShape2D shape)
		{
			throw new NotImplementedException();
		}

		public object Clone()
		{
			throw new NotImplementedException();
		}
	}
}
