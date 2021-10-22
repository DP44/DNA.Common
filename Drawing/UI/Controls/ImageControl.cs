using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI.Controls
{
	public class ImageControl : UIControl
	{
		private Sprite _unselectedSprite;
		public Rectangle? SourceRect;
		public Size _destinationSize;

		/// <summary>
		/// 
		/// </summary>
		public override Size Size
		{
			get
			{
				return this._destinationSize;
			}
			set
			{
				this._destinationSize = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public ImageControl(Sprite image, Rectangle destinationRectangle)
		{
			this._unselectedSprite = image;
			base.LocalPosition = new Point(destinationRectangle.X, destinationRectangle.Y);
			this._destinationSize = new Size(destinationRectangle.Width, destinationRectangle.Height);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public ImageControl(Sprite image, Point position)
		{
			this._unselectedSprite = image;
			base.LocalPosition = position;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public ImageControl(Sprite image)
		{
			this._unselectedSprite = image;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			this._unselectedSprite.Draw(spriteBatch, base.ScreenBounds, Color.White);
		}
	}
}
