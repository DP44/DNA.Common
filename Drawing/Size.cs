using System;

namespace DNA.Drawing
{
	public struct Size
	{
		public int Width;
		public int Height;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public Size(int width, int height)
		{
			this.Width = width;
			this.Height = height;
		}

		/// <summary>
		/// 
		/// </summary>
		public override int GetHashCode() => 
			this.Width.GetHashCode() ^ this.Height.GetHashCode();

		public static bool operator != (Size a, Size b) => 
			a.Width != b.Width || a.Height != b.Height;
		
		public static bool operator == (Size a, Size b) => 
			a.Width == b.Width && a.Height == b.Height;
	}
}
