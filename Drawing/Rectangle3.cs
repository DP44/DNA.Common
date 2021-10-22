using System;

namespace DNA.Drawing
{
	[Serializable]
	public struct Rectangle3 : IEquatable<Rectangle3>
	{
		public int Height;
		public int Width;
		public int Depth;
		public int X;
		public int Y;
		public int Z;

		public Rectangle3(int x, int y, int z, int width, int height, int depth)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
			this.Width = width;
			this.Height = height;
			this.Depth = depth;
		}

		public static bool operator != (Rectangle3 a, Rectangle3 b) =>
			throw new NotImplementedException();

		public static bool operator == (Rectangle3 a, Rectangle3 b) =>
			throw new NotImplementedException();

		public IntVector3 Center =>
			new IntVector3(this.X + this.Width / 2, 
						   this.Y + this.Height / 2, 
						   this.Z + this.Depth / 2);

		public static Rectangle3 Empty =>
			new Rectangle3(0, 0, 0, 0, 0, 0);

		public bool IsEmpty =>
			throw new NotImplementedException();

		public IntVector3 Location
		{
			get =>
				new IntVector3(this.X, this.Y, this.Z);

			set
			{
				this.X = value.X;
				this.Y = value.Y;
				this.Z = value.Z;
			}
		}

		public int Top => this.Y;

		public int Bottom =>
			this.Y + this.Height;

		public int Left => this.X;

		public int Right =>
			this.X + this.Width;

		public int Front => this.Z;

		public int Back =>
			this.Z + this.Depth;

		public bool Contains(IntVector3 value) =>
			throw new NotImplementedException();

		public bool Contains(Rectangle3 value) =>
			throw new NotImplementedException();

		public bool Contains(int x, int y, int z) =>
			throw new NotImplementedException();

		public void Contains(ref IntVector3 value, out bool result) =>
			throw new NotImplementedException();

		public void Contains(ref Rectangle3 value, out bool result) =>
			throw new NotImplementedException();

		public override bool Equals(object obj) =>
			throw new NotImplementedException();

		public bool Equals(Rectangle3 other) =>
			throw new NotImplementedException();

		public override int GetHashCode() =>
			throw new NotImplementedException();

		public void Inflate(int horizontalAmount, int verticalAmount, int depthAmount) =>
			throw new NotImplementedException();

		public static Rectangle3 Intersect(Rectangle3 value1, Rectangle3 value2) =>
			throw new NotImplementedException();

		public static void Intersect(ref Rectangle3 value1, ref Rectangle3 value2, 
									 out Rectangle3 result) =>
			throw new NotImplementedException();

		public bool Intersects(Rectangle3 value) =>
			throw new NotImplementedException();

		public void Intersects(ref Rectangle3 value, out bool result) =>
			throw new NotImplementedException();

		public void Offset(IntVector3 amount) =>
			throw new NotImplementedException();

		public void Offset(int offsetX, int offsetY) =>
			throw new NotImplementedException();

		public override string ToString() =>
			throw new NotImplementedException();

		public static Rectangle3 Union(Rectangle3 value1, Rectangle3 value2) =>
			throw new NotImplementedException();

		public static void Union(ref Rectangle3 value1, ref Rectangle3 value2, 
								 out Rectangle3 result) =>
			throw new NotImplementedException();
	}
}
