using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Drawing2D
{
	public interface IShape2D : ICloneable
	{
		/// <summary>
		/// 
		/// </summary>
		RectangleF BoundingBox { get; }

		/// <summary>
		/// 
		/// </summary>
		Vector2 Center { get; }

		/// <summary>
		/// 
		/// </summary>
		float Area { get; }

		/// <summary>
		/// 
		/// </summary>
		OrientedBoundingRect GetOrientedBoundingRect();
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		void Transform(Matrix mat);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		bool Touches(IShape2D s);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		bool IsAbove(Vector2 p);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		float DistanceTo(IShape2D poly);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		float DistanceSquaredTo(IShape2D poly);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		bool IsBelow(Vector2 p);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		bool IsLeftOf(Vector2 p);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		bool IsRightOf(Vector2 p);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		bool CompletelyContains(IShape2D region);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		bool Contains(Vector2 p);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		int Contains(IList<Vector2> points);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		IShape2D Intersection(IShape2D s);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		bool BoundsIntersect(IShape2D shape);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		float Contains(IShape2D shape);
	}
}
