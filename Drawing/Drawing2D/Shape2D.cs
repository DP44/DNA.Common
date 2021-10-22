using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Drawing2D
{
	[Serializable]
	public abstract class Shape2D : IShape2D, ICloneable
	{
		/// <summary>
		/// 
		/// </summary>
		public abstract RectangleF BoundingBox { get; }

		/// <summary>
		/// 
		/// </summary>
		public virtual Vector2 Center
		{
			get
			{
				RectangleF boundingBox = this.BoundingBox;
				
				return new Vector2(boundingBox.X + boundingBox.Width / 2f, 
								   boundingBox.Y + boundingBox.Height / 2f);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public abstract float Area { get; }

		/// <summary>
		/// 
		/// </summary>
		public abstract OrientedBoundingRect GetOrientedBoundingRect();
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public abstract void Transform(Matrix mat);

		public static IShape2D Transform(IShape2D poly, Matrix mat)
		{
			IShape2D newShape = (IShape2D)poly.Clone();
			newShape.Transform(mat);
			return newShape;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public abstract bool Touches(IShape2D s);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public abstract bool IsAbove(Vector2 p);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public float DistanceTo(IShape2D poly) => 
			(float)Math.Sqrt((double)this.DistanceSquaredTo(poly));

		public abstract float DistanceSquaredTo(IShape2D poly);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public abstract bool IsBelow(Vector2 p);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public abstract bool IsLeftOf(Vector2 p);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public abstract bool IsRightOf(Vector2 p);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public abstract bool CompletelyContains(IShape2D region);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public abstract bool Contains(Vector2 p);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public abstract IShape2D Intersection(IShape2D s);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public int Contains(IList<Vector2> points)
		{
			int containedPoints = 0;
			
			for (int i = 0; i < points.Count; i++)
			{
				if (this.Contains(points[i]))
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
		public bool BoundsIntersect(IShape2D shape) => 
			this.BoundingBox.IntersectsWith(shape.BoundingBox);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public abstract float Contains(IShape2D shape);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static int CompareByArea(IShape2D a, IShape2D b)
		{
			if (b.Area == a.Area)
			{
				return 0;
			}
			
			if (b.Area <= a.Area)
			{
				return -1;
			}
			
			return 1;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static int CompareByHeight(IShape2D p1, IShape2D p2)
		{
			if (p1.BoundingBox.Height > p2.BoundingBox.Height)
			{
				return -1;
			}
			
			if (p1.BoundingBox.Height < p2.BoundingBox.Height)
			{
				return 1;
			}
			
			return 0;
		}

		/// <summary>
		/// 
		/// </summary>
		public abstract object Clone();
	}
}
