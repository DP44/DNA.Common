using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI
{
	public class ScalableFrame
	{
		private enum ImageTypes
		{
			TopLeft,
			TopRight,
			BottomLeft,
			BottomRight,
			Top,
			Bottom,
			Left,
			Right,
			Center
		}

		private static readonly string[] _suffixs = new string[]
		{
			"_tl",
			"_tr",
			"_bl",
			"_br",
			"_t",
			"_b",
			"_l",
			"_r",
			"_c"
		};

		private Sprite[] _sprites = new Sprite[9];

		private Sprite TopLeft
		{
			get
			{
				return this._sprites[0];
			}
		}

		private Sprite TopRight
		{
			get
			{
				return this._sprites[1];
			}
		}

		private Sprite BottomLeft
		{
			get
			{
				return this._sprites[2];
			}
		}

		private Sprite BottomRight
		{
			get
			{
				return this._sprites[3];
			}
		}

		private Sprite Top
		{
			get
			{
				return this._sprites[4];
			}
		}

		private Sprite Bottom
		{
			get
			{
				return this._sprites[5];
			}
		}

		private Sprite Left
		{
			get
			{
				return this._sprites[6];
			}
		}

		private Sprite Right
		{
			get
			{
				return this._sprites[7];
			}
		}

		private Sprite Center
		{
			get
			{
				return this._sprites[8];
			}
		}

		public ScalableFrame(SpriteManager spriteManager, string name)
		{
			for (int i = 0; i < this._sprites.Length; i++)
			{
				this._sprites[i] = spriteManager[name + ScalableFrame._suffixs[i]];
			}
		}

		public Rectangle CenterRegion(Rectangle drawRegion)
		{
			return new Rectangle(drawRegion.Left + this.Right.Width, drawRegion.Top + this.Top.Height, drawRegion.Width - (this.Left.Width + this.Right.Width), drawRegion.Height - (this.Top.Height + this.Bottom.Height));
		}

		public void Draw(SpriteBatch spriteBatch, Rectangle rect, Color color)
		{
			this.TopLeft.Draw(spriteBatch, new Vector2((float)rect.X, (float)rect.Y), color);
			this.TopRight.Draw(spriteBatch, new Vector2((float)(rect.Right - this.TopRight.Width), (float)rect.Y), color);
			this.BottomLeft.Draw(spriteBatch, new Vector2((float)rect.X, (float)(rect.Bottom - this.BottomLeft.Height)), color);
			this.BottomRight.Draw(spriteBatch, new Vector2((float)(rect.Right - this.BottomRight.Width), (float)(rect.Bottom - this.BottomRight.Height)), color);
			this.Top.Draw(spriteBatch, new Rectangle(rect.X + this.TopLeft.Width, rect.Top, rect.Width - (this.TopLeft.Width + this.TopRight.Width), this.Top.Height), color);
			this.Bottom.Draw(spriteBatch, new Rectangle(rect.X + this.BottomLeft.Width, rect.Bottom - this.Bottom.Height, rect.Width - (this.BottomLeft.Width + this.BottomRight.Width), this.Bottom.Height), color);
			this.Left.Draw(spriteBatch, new Rectangle(rect.Left, rect.Top + this.TopLeft.Height, this.Left.Width, rect.Height - (this.TopLeft.Height + this.BottomLeft.Height)), color);
			this.Right.Draw(spriteBatch, new Rectangle(rect.Right - this.Right.Width, rect.Top + this.TopRight.Height, this.Right.Width, rect.Height - (this.TopRight.Height + this.BottomRight.Height)), color);
			this.Center.Draw(spriteBatch, new Rectangle(rect.Left + this.Right.Width, rect.Top + this.Top.Height, rect.Width - (this.Left.Width + this.Right.Width), rect.Height - (this.Top.Height + this.Bottom.Height)), color);
		}
	}
}
