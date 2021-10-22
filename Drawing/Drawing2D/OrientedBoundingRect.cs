using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Drawing2D
{
	public struct OrientedBoundingRect
	{
		public static readonly OrientedBoundingRect Empty = new OrientedBoundingRect(new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f));

		private Vector2 _size;

		private Vector2 _location;

		private Vector2 _axis;

		public Vector2 Size
		{
			get
			{
				return this._size;
			}
		}

		public Vector2 Location
		{
			get
			{
				return this._location;
			}
		}

		public Angle Rotation
		{
			get
			{
				return Angle.FromDegrees(90f) + Angle.ATan2((double)this._axis.Y, (double)this._axis.X);
			}
		}

		public Vector2 Axis
		{
			get
			{
				return this._axis;
			}
		}

		public Vector2 Center
		{
			get
			{
				Vector2 vector = new Vector2(this._axis.Y, -this._axis.X);
				return new Vector2(this._location.X + this._axis.X * this._size.Y / 2f + vector.X * this._size.X / 2f, this._location.Y + this._axis.Y * this._size.Y / 2f + vector.Y * this._size.X / 2f);
			}
		}

		public Matrix Orientation
		{
			get
			{
				return Matrix.CreateScale(this._size.X, this._size.Y, 1f) * Matrix.CreateRotationZ(this.Rotation.Radians) * Matrix.CreateTranslation(this._location.X, this._location.Y, 0f);
			}
		}

		private OrientedBoundingRect(Vector2 location, Vector2 size, Vector2 axis)
		{
			this._size = size;
			this._location = location;
			this._axis = axis;
		}

		public static OrientedBoundingRect FromPoints(IList<Vector2> opoints)
		{
			Vector2[] convexHull = DrawingTools.GetConvexHull(opoints);
			if (convexHull.Length <= 0)
			{
				return OrientedBoundingRect.Empty;
			}
			RectangleF rectangleF = DrawingTools.FindBounds(convexHull);
			Vector2 location = new Vector2(rectangleF.Left + rectangleF.Width / 2f, rectangleF.Top + rectangleF.Height / 2f);
			Vector2 vector = new Vector2(0f, 0f);
			foreach (Vector2 vector2 in convexHull)
			{
				Vector2 vector3 = new Vector2(vector2.X - location.X, vector2.Y - location.Y);
				if (vector3.X < 0f)
				{
					vector3 = -vector3;
				}
				vector += vector3;
			}
			Vector2 vector4 = new Vector2(0f, 0f);
			foreach (Vector2 vector5 in convexHull)
			{
				Vector2 vector6 = new Vector2(vector5.X - location.X, vector5.Y - location.Y);
				if (vector6.Y < 0f)
				{
					vector6 = -vector6;
				}
				vector4 += vector6;
			}
			if (vector4.Length() > vector.Length())
			{
				vector = vector4;
			}
			vector.Normalize();
			Vector2 value = new Vector2(vector.Y, -vector.X);
			float num = float.MaxValue;
			float num2 = float.MaxValue;
			float num3 = float.MinValue;
			float num4 = float.MinValue;
			foreach (Vector2 vector7 in convexHull)
			{
				Vector2 vector8 = new Vector2(location.X - vector7.X, location.Y - vector7.Y);
				float val = vector.X * vector8.Y - vector8.X * vector.Y;
				float val2 = -(value.X * vector8.Y - vector8.X * value.Y);
				num = Math.Min(num, val);
				num2 = Math.Min(num2, val2);
				num3 = Math.Max(num3, val);
				num4 = Math.Max(num4, val2);
			}
			float x = num3 - num;
			float y = num4 - num2;
			Vector2 vector9 = num * value;
			Vector2 vector10 = num2 * vector;
			location.X = location.X + vector9.X + vector10.X;
			location.Y = location.Y + vector9.Y + vector10.Y;
			return new OrientedBoundingRect(location, new Vector2(x, y), vector);
		}

		public static OrientedBoundingRect FromPoints(IList<Point> opoints)
		{
			Vector2[] array = new Vector2[opoints.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new Vector2((float)opoints[i].X, (float)opoints[i].Y);
			}
			return OrientedBoundingRect.FromPoints(array);
		}

		public static OrientedBoundingRect FromPoints(IList<Vector3> points)
		{
			throw new Exception();
		}
	}
}
