using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing
{
	public struct RectangleF
	{
		private Vector2 _location;

		private Vector2 _size;

		public static readonly RectangleF Empty = new RectangleF(new Vector2(0f, 0f), new Vector2(0f, 0f));

		public RectangleF(Vector2 location, Vector2 size)
		{
			this._location = location;
			this._size = size;
		}

		public RectangleF(float x, float y, float width, float height)
		{
			this._location = new Vector2(x, y);
			this._size = new Vector2(width, height);
		}

		public static bool operator !=(RectangleF left, RectangleF right)
		{
			throw new NotImplementedException();
		}

		public static bool operator ==(RectangleF left, RectangleF right)
		{
			throw new NotImplementedException();
		}

		public static implicit operator RectangleF(Rectangle r)
		{
			return new RectangleF((float)r.X, (float)r.Y, (float)r.Width, (float)r.Height);
		}

		public float Bottom
		{
			get
			{
				return this._location.Y + this._size.Y;
			}
		}

		public float Height
		{
			get
			{
				return this._size.Y;
			}
			set
			{
				this._size.Y = value;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this._size.X == 0f || this._size.Y == 0f;
			}
		}

		public float Left
		{
			get
			{
				return this.X;
			}
		}

		public Vector2 Location
		{
			get
			{
				return this._location;
			}
			set
			{
				this._location = value;
			}
		}

		public float Right
		{
			get
			{
				return this._location.X + this._size.X;
			}
		}

		public Vector2 Size
		{
			get
			{
				return this._size;
			}
			set
			{
				this._size = value;
			}
		}

		public float Top
		{
			get
			{
				return this._location.Y;
			}
		}

		public float Width
		{
			get
			{
				return this._size.X;
			}
			set
			{
				this._size.X = value;
			}
		}

		public float X
		{
			get
			{
				return this._location.X;
			}
			set
			{
				this._location.X = value;
			}
		}

		public float Y
		{
			get
			{
				return this._location.Y;
			}
			set
			{
				this._location.Y = value;
			}
		}

		public bool Contains(Vector2 pt)
		{
			return pt.X >= this._location.X && pt.Y >= this._location.Y && pt.X <= this.Right && pt.Y <= this.Bottom;
		}

		public bool Contains(RectangleF rect)
		{
			return rect.Left >= this.Left && rect.Right <= this.Right && rect.Top >= this.Top && rect.Bottom <= this.Bottom;
		}

		public bool Contains(float x, float y)
		{
			throw new NotImplementedException();
		}

		public override bool Equals(object obj)
		{
			throw new NotImplementedException();
		}

		public static RectangleF FromLTRB(float left, float top, float right, float bottom)
		{
			throw new NotImplementedException();
		}

		public void Inflate(Vector2 size)
		{
			throw new NotImplementedException();
		}

		public void Inflate(float x, float y)
		{
			throw new NotImplementedException();
		}

		public static RectangleF Inflate(RectangleF rect, float x, float y)
		{
			throw new NotImplementedException();
		}

		public void Intersect(RectangleF rect)
		{
			throw new NotImplementedException();
		}

		public static RectangleF Intersect(RectangleF a, RectangleF b)
		{
			throw new NotImplementedException();
		}

		public bool IntersectsWith(RectangleF rect)
		{
			return rect.Right >= this.Left && rect.Left <= this.Right && rect.Bottom >= this.Top && rect.Top <= this.Bottom;
		}

		public void Offset(Vector2 pos)
		{
			throw new NotImplementedException();
		}

		public void Offset(float x, float y)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				this.X.ToString(),
				",",
				this.Y.ToString(),
				",",
				this.Width.ToString(),
				",",
				this.Height.ToString()
			});
		}

		public static RectangleF Union(RectangleF a, RectangleF b)
		{
			throw new NotImplementedException();
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}
	}
}
