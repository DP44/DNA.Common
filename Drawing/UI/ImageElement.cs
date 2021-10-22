using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI
{
	public class ImageElement : UIElement
	{
		private Sprite _selectedSprite;

		private Sprite _unselectedSprite;

		public Rectangle? SourceRect;

		public Vector2 _destinationSize;

		public override Vector2 Size
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

		public ImageElement(Sprite image, Rectangle destinationRectangle)
		{
			this._unselectedSprite = image;
			base.Location = new Vector2((float)destinationRectangle.X, (float)destinationRectangle.Y);
			this._destinationSize = new Vector2((float)destinationRectangle.Width, (float)destinationRectangle.Height);
		}

		public ImageElement(Sprite image, Vector2 position)
		{
			this._unselectedSprite = image;
			base.Location = position;
		}

		public ImageElement(Sprite image)
		{
			this._unselectedSprite = image;
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime, bool selected)
		{
			Vector2 destinationSize = this._destinationSize;
			if (selected && this._selectedSprite != null)
			{
				this._selectedSprite.Draw(spriteBatch, new Rectangle((int)base.Location.X, (int)base.Location.Y, (int)destinationSize.X, (int)destinationSize.Y), base.Color);
				return;
			}
			this._unselectedSprite.Draw(spriteBatch, new Rectangle((int)base.Location.X, (int)base.Location.Y, (int)destinationSize.X, (int)destinationSize.Y), base.Color);
		}
	}
}
