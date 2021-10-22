using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI.Controls
{
	public class ListItemControl : ButtonControl
	{
		public bool Selected;

		public bool Active = true;

		private SpriteFont _font;

		private Size _size = new Size(100, 100);

		public Color ButtonColor = Color.White;

		public Color ButtonHoverColor = Color.Gray;

		public Color ButtonPressedColor = Color.Black;

		public Color TextColor = Color.Black;

		public Color TextHoverColor = Color.Black;

		public Color TextPressedColor = Color.White;

		public string Text { get; set; }

		public override Size Size
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

		public ListItemControl(Size size)
		{
			this._size = size;
		}

		public ListItemControl(SpriteFont font, Size size, string text)
		{
			this._font = font;
			this._size = size;
			this.Text = text;
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			Rectangle screenBounds = base.ScreenBounds;
			if (base.CaptureInput || this.Selected)
			{
				spriteBatch.Draw(UIControl.DummyTexture, screenBounds, this.ButtonPressedColor * 0.75f);
				if (this._font != null && this.Text != null)
				{
					spriteBatch.DrawString(this._font, this.Text, new Vector2((float)(screenBounds.X + 5), (float)(screenBounds.Y + 5)), this.TextPressedColor);
					return;
				}
			}
			else if (base.Hovering)
			{
				spriteBatch.Draw(UIControl.DummyTexture, screenBounds, this.ButtonHoverColor * 0.75f);
				if (this._font != null && this.Text != null)
				{
					spriteBatch.DrawString(this._font, this.Text, new Vector2((float)(screenBounds.X + 5), (float)(screenBounds.Y + 5)), this.TextHoverColor);
					return;
				}
			}
			else
			{
				spriteBatch.Draw(UIControl.DummyTexture, screenBounds, this.ButtonColor * 0.75f);
				if (this._font != null && this.Text != null)
				{
					spriteBatch.DrawString(this._font, this.Text, new Vector2((float)(screenBounds.X + 5), (float)(screenBounds.Y + 5)), this.TextColor);
				}
			}
		}
	}
}
