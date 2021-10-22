using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Drawing2D
{
	[Serializable]
	public class PolygonF
	{
		private Vector2[] _points;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public PolygonF(Vector2[] points) => 
			this._points = points;

		/// <summary>
		/// 
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
			bool doesContainPoint = false;
			int i = 0;
			int previousPoint = this.Points.Length - 1;
			
			// Iterate through all of the points.
			while (i < this.Points.Length)
			{
				float x = this.Points[i].X;
				float y = this.Points[i].Y;
				float x2 = this.Points[previousPoint].X;
				float y2 = this.Points[previousPoint].Y;
			
				if (((y <= point.Y && point.Y < y2) || (y2 <= point.Y && point.Y < y)) && 
					point.X < (x2 - x) * (point.Y - y) / (y2 - y) + x)
				{
					// Toggle our bool.
					doesContainPoint = !doesContainPoint;
				}
			
				// Store our previous point.
				previousPoint = i;
				i++;
			}
			
			return doesContainPoint;
		}
	}
}
