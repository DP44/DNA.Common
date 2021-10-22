using System;
using DNA.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI
{
	public class SettingItemElement
	{
		public string Text;

		public Color? OutlineColor;

		public int? OnlineWidth;

		protected OneShotTimer changeValueTimer = new OneShotTimer(TimeSpan.FromSeconds(0.25));

		protected OneShotTimer delayTimer = new OneShotTimer(TimeSpan.FromSeconds(0.25));

		private Color? _textColor;

		private Color? _selectedColor;

		public bool Visible = true;

		private SpriteFont _font;

		public event EventHandler<EventArgs> Clicked;

		public Color? TextColor
		{
			get
			{
				return this._textColor;
			}
			set
			{
				this._textColor = value;
			}
		}

		public Color? SelectedColor
		{
			get
			{
				return this._selectedColor;
			}
			set
			{
				this._selectedColor = value;
			}
		}

		public SpriteFont Font
		{
			get
			{
				return this._font;
			}
			set
			{
				this._font = value;
			}
		}

		public SettingItemElement(string text)
		{
			this.Text = text;
		}

		public virtual void OnClicked()
		{
			if (this.Clicked != null)
			{
				this.Clicked(this, new EventArgs());
			}
		}

		public virtual void Increased()
		{
		}

		public virtual void Decreased()
		{
		}

		public virtual void OnDraw(DNAGame _game, GraphicsDevice device, SpriteBatch spriteBatch, SpriteFont font, Color textColor, Color outlineColor, int outlineWidth, Vector2 loc)
		{
			spriteBatch.DrawOutlinedText(font, this.Text, loc, textColor, outlineColor, outlineWidth);
		}

		public void ResetTimer()
		{
			this.changeValueTimer.Reset();
			this.delayTimer.Reset();
		}

		public virtual bool ChangeValue(TimeSpan elapsedGameTime)
		{
			this.delayTimer.Update(elapsedGameTime);
			if (this.delayTimer.Expired)
			{
				this.changeValueTimer.Update(elapsedGameTime);
				if (this.changeValueTimer.Expired)
				{
					this.changeValueTimer.Reset();
					return true;
				}
			}
			return false;
		}
	}
}
