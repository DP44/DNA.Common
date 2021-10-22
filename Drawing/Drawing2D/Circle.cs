using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Drawing2D
{
	public struct Circle : IShape2D, ICloneable
	{
		public Vector2 _center;
		public float Radius;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public Circle(Vector2 center, float radius)
		{
			this._center = center;
			this.Radius = radius;
		}

		/// <summary>
		/// 
		/// </summary>
		public RectangleF BoundingBox => 
			new RectangleF(this._center.X - this.Radius, 
						   this._center.Y - this.Radius, 
						   this.Radius * 2f, this.Radius * 2f);

		/// <summary>
		/// 
		/// </summary>
		public Vector2 Center
		{
			get => 
				this._center;
			
			set => 
				this._center = value;
		}

		/// <summary>
		/// 
		/// </summary>
		public float Area => 
			3.141593f * this.Radius * this.Radius;

		/// <summary>
		/// 
		/// </summary>
		public OrientedBoundingRect GetOrientedBoundingRect() => 
			throw new NotImplementedException();

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void Transform(Matrix mat) => 
			this._center = Vector2.Transform(this._center, mat);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public bool Touches(IShape2D s) =>
			throw new NotImplementedException();

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public bool IsAbove(Vector2 p) => 
			throw new NotImplementedException();

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public float DistanceTo(IShape2D poly) => 
			throw new NotImplementedException();

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public float DistanceSquaredTo(IShape2D poly) => 
			throw new NotImplementedException();

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public bool IsBelow(Vector2 p) => 
			throw new NotImplementedException();

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public bool IsLeftOf(Vector2 p) => 
			throw new NotImplementedException();

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public bool IsRightOf(Vector2 p) => 
			throw new NotImplementedException();

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public bool CompletelyContains(IShape2D region) => 
			throw new NotImplementedException();

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public bool Contains(Vector2 p) => 
			(p - this._center).Length() <= this.Radius;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public int Contains(IList<Vector2> points)
		{
			int containedPoints = 0;
			
			foreach (Vector2 point in points)
			{
				if (!this.Contains(point))
				{
					containedPoints++;
				}
			}
			
			return containedPoints;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public IShape2D Intersection(IShape2D s) => 
			throw new NotImplementedException();

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public bool BoundsIntersect(IShape2D shape) => 
			throw new NotImplementedException();

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public float Contains(IShape2D shape) => 
			throw new NotImplementedException();

		/// <summary>
		/// 
		/// </summary>
		public object Clone() => 
			new Circle(this._center, this.Radius);
	}
}
