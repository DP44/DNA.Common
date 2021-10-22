using System;
using DNA.Input;
using DNA.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI
{
	public class BarSettingItem : SettingItemElement
	{
		public float Value;
		
		private Rectangle _location = Rectangle.Empty;

		public BarSettingItem(string text, float defaultValue) : base(text)
		{
			this.changeValueTimer = new OneShotTimer(TimeSpan.FromSeconds(0.1));
			this.Value = defaultValue;
		}

		public bool SetBarValue(MouseInput mouse)
		{
			if (!this._location.Contains(mouse.Position) || !mouse.LeftButtonDown)
			{
				return false;
			}
			
			this.Value = 
				(float)(mouse.Position.X - this._location.X) / (float)this._location.Width;
		
			return true;
		}

		public override void OnClicked()
		{
			if ((double)this.Value < 0.25)
			{
				this.Value = 0.25f;
			}
			else if ((double)this.Value < 0.5)
			{
				this.Value = 0.5f;
			}
			else if ((double)this.Value < 0.75)
			{
				this.Value = 0.75f;
			}
			else if (this.Value < 1f)
			{
				this.Value = 1f;
			}
			else
			{
				this.Value = 0f;
			}

			base.OnClicked();
		}

		public override void Increased()
		{
			this.Value += 0.05f;
			
			if (this.Value > 1f)
			{
				this.Value = 1f;
			}
		}

		public override void Decreased()
		{
			this.Value -= 0.05f;
			
			if (this.Value < 0f)
			{
				this.Value = 0f;
			}
		}

		public override void OnDraw(DNAGame _game, GraphicsDevice device, SpriteBatch spriteBatch, 
									SpriteFont font, Color textColor, Color outlineColor, 
									int outlineWidth, Vector2 loc)
		{
			Rectangle screenRect = Screen.Adjuster.ScreenRect;
			Vector2 vector = font.MeasureString(">");
			float num = (float)(screenRect.Right - screenRect.Center.X - 50) - vector.X * 2f;
			float num2 = vector.Y / 2f;
			float num3 = loc.Y + (vector.Y - num2) / 2f - 3f;
			
			spriteBatch.DrawOutlinedText(font, "<", 
				new Vector2((float)(screenRect.Center.X + 15), loc.Y), 
				textColor, outlineColor, outlineWidth);
			
			spriteBatch.DrawOutlinedText(font, ">", 
				new Vector2((float)(screenRect.Right - 15) - vector.X, loc.Y), 
				textColor, outlineColor, outlineWidth);
			
			this._location = 
				new Rectangle((int)((float)(screenRect.Center.X + 25) + vector.X), 
							  (int)num3, (int)num, (int)num2);
			
			spriteBatch.Draw(_game.DummyTexture, 
				new Rectangle((int)((float)(screenRect.Center.X + 25) + vector.X), 
					(int)num3, (int)num, (int)num2), 
				Color.Black);
		
			spriteBatch.Draw(_game.DummyTexture, 
				new Rectangle((int)((float)(screenRect.Center.X + 26) + vector.X), 
					(int)num3 + 1, (int)((num - 2f) * this.Value), (int)(num2 - 2f)), 
				Color.White);
			
			base.OnDraw(_game, device, spriteBatch, font, textColor, 
						outlineColor, outlineWidth, loc);
		}
	}
}
