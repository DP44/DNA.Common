using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Drawing2D
{
	[Serializable]
	public struct Line
	{
		private Point _start;
		private Point _end;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public Line(Point start, Point end)
		{
			this._start = start;
			this._end = end;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public Line(int x1, int y1, int x2, int y2)
		{
			this._start = new Point(x1, y1);
			this._end = new Point(x2, y2);
		}

		/// <summary>
		/// 
		/// </summary>
		public Point Start
		{
			get => 
				this._start;

			set => 
				this._start = value;
		}

		/// <summary>
		/// 
		/// </summary>
		public Point End
		{
			get => 
				this._end;
			
			set => 
				this._end = value;
		}

		/// <summary>
		/// 
		/// </summary>
		public double Length => this.Start.Distance(this.End);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public double DistanceTo(Point pnt)
		{
			double length = this.Length;
			double lineLengthNormalized;
			
			if (length != 0.0)
			{
				lineLengthNormalized = 
					(double)((pnt.X - this._start.X) * (this._end.X - this._start.X) + 
						(pnt.Y - this._start.Y) * (this._end.Y - this._start.Y)) / 
					(length * length);
			}
			else
			{
				lineLengthNormalized = 0.0;
			}
			
			if (lineLengthNormalized >= 0.0 && lineLengthNormalized <= 1.0)
			{
				double x2 = (double)this._start.X + lineLengthNormalized * 
					(double)(this._end.X - this._start.X);
				
				double y2 = (double)this._start.Y + lineLengthNormalized * 
					(double)(this._end.Y - this._start.Y);
				
				double x3 = (double)pnt.X - x2;
				double y3 = (double)pnt.Y - y2;
				return Math.Sqrt(x3 * x3 + y3 * y3);
			}
			
			double distToStart = pnt.Distance(this._start);
			double distToEnd = pnt.Distance(this._end);
			
			if (distToStart >= distToEnd)
			{
				return distToEnd;
			}
			
			return distToStart;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public override bool Equals(object obj) =>
			obj.GetType() == typeof(Line) && this == (Line)obj;

		/// <summary>
		/// 
		/// </summary>
		public override int GetHashCode() =>
			throw new NotImplementedException();

		public static bool operator == (Line a, Line b) =>
			throw new NotImplementedException();

		public static bool operator != (Line a, Line b) =>
			throw new NotImplementedException();
	}
}
