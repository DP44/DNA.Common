using System;
using DNA.Input;
using DNA.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI
{
	public class MouseOverText
	{
		private Vector2 _location;

		private Rectangle _locationRectangle = Rectangle.Empty;

		private string _text = "";

		private Texture2D _icon;

		private Sprite _sprite;

		private Vector2 _iconWidthHeight = Vector2.Zero;

		private SpriteFont _font;

		private Color _color = Color.White;

		private Color _selectedColor = Color.Red;

		private bool _selected;

		private bool _flashDir;

		private OneShotTimer _flashTimer = new OneShotTimer(TimeSpan.FromSeconds(0.25));

		public Vector2 Location
		{
			get
			{
				return this._location;
			}
			set
			{
				this._location = value;
				Vector2 vector = this._font.MeasureString(this._text);
				this._locationRectangle = new Rectangle((int)this._location.X, (int)this._location.Y, (int)(vector.X + this._iconWidthHeight.X), (int)vector.Y);
			}
		}

		public MouseOverText(string text, Vector2 location, SpriteFont font)
		{
			this._text = text;
			this._location = location;
			this._font = font;
			this._locationRectangle = new Rectangle((int)this._location.X, (int)this._location.Y, (int)this._font.MeasureString(this._text).X, (int)this._font.MeasureString(this._text).Y);
		}

		public MouseOverText(string text, Vector2 location, SpriteFont font, Texture2D icon)
		{
			this._text = text;
			this._location = location;
			this._font = font;
			this._icon = icon;
			Vector2 vector = this._font.MeasureString(this._text);
			float num = vector.Y / (float)this._icon.Height;
			this._iconWidthHeight = new Vector2((float)this._icon.Width * num, vector.Y);
			this._locationRectangle = new Rectangle((int)this._location.X, (int)this._location.Y, (int)(vector.X + this._iconWidthHeight.X), (int)vector.Y);
		}

		public MouseOverText(string text, Vector2 location, SpriteFont font, Sprite icon)
		{
			this._text = text;
			this._location = location;
			this._font = font;
			this._sprite = icon;
			Vector2 vector = this._font.MeasureString(this._text);
			float num = vector.Y / (float)this._sprite.Height;
			this._iconWidthHeight = new Vector2((float)this._sprite.Width * num, vector.Y);
			this._locationRectangle = new Rectangle((int)this._location.X, (int)this._location.Y, (int)(vector.X + this._iconWidthHeight.X), (int)vector.Y);
		}

		public MouseOverText(string text, Vector2 location, SpriteFont font, Sprite icon, Color color, Color selectedColor, float scaleImage)
		{
			this._text = text;
			this._location = location;
			this._font = font;
			this._sprite = icon;
			this._color = color;
			this._selectedColor = selectedColor;
			Vector2 vector = this._font.MeasureString(this._text);
			float num = vector.Y / (float)this._sprite.Height;
			this._iconWidthHeight = new Vector2((float)this._sprite.Width * num, vector.Y) * scaleImage;
			this._locationRectangle = new Rectangle((int)this._location.X, (int)this._location.Y, (int)(vector.X + this._iconWidthHeight.X), (int)vector.Y);
		}

		public MouseOverText(string text, Vector2 location, SpriteFont font, Texture2D icon, Color color, Color selectedColor)
		{
			this._text = text;
			this._location = location;
			this._font = font;
			this._icon = icon;
			this._color = color;
			this._selectedColor = selectedColor;
			Vector2 vector = this._font.MeasureString(this._text);
			float num = vector.Y / (float)this._icon.Height;
			this._iconWidthHeight = new Vector2((float)this._icon.Width * num, vector.Y);
			this._locationRectangle = new Rectangle((int)this._location.X, (int)this._location.Y, (int)(vector.X + this._iconWidthHeight.X), (int)vector.Y);
		}

		public MouseOverText(string text, Vector2 location, SpriteFont font, Sprite icon, Color color, Color selectedColor)
		{
			this._text = text;
			this._location = location;
			this._font = font;
			this._sprite = icon;
			this._color = color;
			this._selectedColor = selectedColor;
			Vector2 vector = this._font.MeasureString(this._text);
			float num = vector.Y / (float)this._sprite.Height;
			this._iconWidthHeight = new Vector2((float)this._sprite.Width * num, vector.Y);
			this._locationRectangle = new Rectangle((int)this._location.X, (int)this._location.Y, (int)(vector.X + this._iconWidthHeight.X), (int)vector.Y);
		}

		public MouseOverText(string text, Vector2 location, SpriteFont font, Color color, Color selectedColor)
		{
			this._text = text;
			this._location = location;
			this._font = font;
			this._color = color;
			this._selectedColor = selectedColor;
			this._locationRectangle = new Rectangle((int)this._location.X, (int)this._location.Y, (int)this._font.MeasureString(this._text).X, (int)this._font.MeasureString(this._text).Y);
		}

		public bool CheckMouse(MouseInput mouse)
		{
			if (this._locationRectangle.Contains(mouse.Position))
			{
				this._selected = true;
				if (mouse.LeftButtonPressed)
				{
					return true;
				}
			}
			else
			{
				this._selected = false;
			}
			return false;
		}

		public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
		{
			this._flashTimer.Update(gameTime.ElapsedGameTime);
			if (this._flashTimer.Expired)
			{
				this._flashTimer.Reset();
				this._flashDir = !this._flashDir;
			}
			float amount = this._flashDir ? this._flashTimer.PercentComplete : (1f - this._flashTimer.PercentComplete);
			Color color = Color.Lerp(this._color, this._selectedColor, amount);
			if (this._icon != null)
			{
				spriteBatch.Draw(this._icon, new Rectangle((int)this._location.X, (int)(this._location.Y + (float)(this._font.LineSpacing / 2) - this._iconWidthHeight.Y / 2f), (int)this._iconWidthHeight.X, (int)this._iconWidthHeight.Y), Color.White);
			}
			else if (this._sprite != null)
			{
				spriteBatch.Draw(this._sprite, new Rectangle((int)this._location.X, (int)(this._location.Y + (float)(this._font.LineSpacing / 2) - this._iconWidthHeight.Y / 2f), (int)this._iconWidthHeight.X, (int)this._iconWidthHeight.Y), Color.White);
			}
			spriteBatch.DrawOutlinedText(this._font, this._text, new Vector2(this._location.X + this._iconWidthHeight.X, this._location.Y), this._selected ? color : this._color, Color.Black, 1);
		}
	}
}
