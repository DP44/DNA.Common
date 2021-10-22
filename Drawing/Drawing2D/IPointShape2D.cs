using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Drawing2D
{
	public interface IPointShape2D : IShape2D, ICloneable
	{
		/// <summary>
		/// 
		/// </summary>
		IList<Vector2> Points { get; }

		/// <summary>
		/// 
		/// </summary>
		IList<LineF2D> GetLineSegments();

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		float PointsInside(IShape2D shape);
	}
}
