using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing 
{
	[Serializable]
	public class Polygon 
	{
		private Vector2[] _points;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="points">A list of points to create the polygon out of.</param>
		public Polygon(Vector2[] points) => 
			this._points = points;

		/// <summary>
		/// A list of the points that make up the polygon.
		/// </summary>
		public Vector2[] Points => 
			this._points;

		/// <summary>
		/// 
		/// </summary>
		public RectangleF Extents => 
			DrawingTools.GetBoundingRect(this._points);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public bool Contains(Vector2 point) 
		{
			#if DECOMPILED
				bool pointInPolygon = false;
				int firstPoint = 0;
				int lastPoint = this.Points.Length - 1;
				
				for (; firstPoint < this.Points.Length; firstPoint++)
				{
					float x1 = this.Points[firstPoint].X;
					float y1 = this.Points[firstPoint].Y;
					float x2 = this.Points[lastPoint].X;
					float y2 = this.Points[lastPoint].Y;
					
					if (((double)y1 <= (double)point.Y && (double)point.Y < (double) y2 || 
						 (double)y2 <= (double)point.Y && 
						 (double)point.Y < (double)y1) 
						&& (double)point.X < ((double)x2 - (double)x1) * 
							((double)point.Y - (double)y1)
							/ ((double)y2 - (double)y1) + (double)x1)
					{
						pointInPolygon = !pointInPolygon;
					}
					
					lastPoint = firstPoint;
				}

				return pointInPolygon;
			#else
				bool pointInPolygon = false;
				
				int firstPoint = 0;
				int lastPoint = this.Points.Length - 1;

				while (firstPoint < this.Points.Length) 
				{
					float x = this.Points[firstPoint].X;
					float y = this.Points[firstPoint].Y;
					float x2 = this.Points[lastPoint].X;
					float y2 = this.Points[lastPoint].Y;

					// Check if the given point is in the polygon.
					if (((y <= point.Y && point.Y < y2) || (y2 <= point.Y && point.Y < y))
						&& point.X < (x2 - x) * (point.Y - y) / (y2 - y) + x) 
					{
						// it's in the polygon's area.
						pointInPolygon = !pointInPolygon;
					}
					
					lastPoint = firstPoint;
					firstPoint++;
				}

				return pointInPolygon;
			#endif
		}
	}
}
