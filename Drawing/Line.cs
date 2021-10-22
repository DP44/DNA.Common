using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing
{
	public struct Line
	{
		private Point _start;
		private Point _end;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="start">Where the line should start.</param>
		/// <param name="end">Where the line should end.</param>
		public Line(Point start, Point end)
		{
			this._start = start;
			this._end = end;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="x1">The x position on the screen to where the line should start.</param>
		/// <param name="y1">The y position on the screen to where the line should start.</param>
		/// <param name="x2">The x position on the screen to where the line should end.</param>
		/// <param name="y2">The y position on the screen to where the line should end.</param>
		public Line(int x1, int y1, int x2, int y2)
		{
			this._start = new Point(x1, y1);
			this._end = new Point(x2, y2);
		}

		/// <summary>
		/// The start of the line.
		/// </summary>
		public Point Start
		{
			get => 
				this._start;
			
			set => 
				this._start = value;
		}

		/// <summary>
		/// The end point of the line.
		/// </summary>
		public Point End
		{
			get => 
				this._end;
			
			set => 
				this._end = value;
		}

		/// <summary>
		/// The length of the line.
		/// </summary>
		public double Length => 
			MathTools.Distance(this.Start, this.End);

		/// <summary>
		/// Get the distance to a point.
		/// </summary>
		/// <param name="pnt">The point to check for.</param>
		public double DistanceTo(Point pnt)
		{
			double length = this.Length;
			
			double k = length == 0.0 ? 0.0
				: (double)((pnt.X - this._start.X) * (this._end.X - this._start.X) + 
						   (pnt.Y - this._start.Y) * (this._end.Y - this._start.Y)) / 
								(length * length);

			if (k < 0.0 || k > 1.0)
			{
				double distToStart = MathTools.Distance(pnt, this._start);
				double distToEnd = MathTools.Distance(pnt, this._end);
				
				return distToStart >= distToEnd ? distToEnd : distToStart;
			}

			double xCalc = (double)this._start.X + k * (double)(this._end.X - this._start.X);
			double yCalc = (double)this._start.Y + k * (double)(this._end.Y - this._start.Y);
			
			double distToCalcX = (double)pnt.X - xCalc;
			double distToCalcY = (double)pnt.Y - yCalc;

			return Math.Sqrt(distToCalcX * distToCalcX + distToCalcY * distToCalcY);
		}
	}
}
