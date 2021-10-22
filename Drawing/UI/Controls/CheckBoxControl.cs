using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI.Controls
{
	public class CheckBoxControl : ButtonControl
	{
		private const int CheckSpace = 5;

		public new float Scale;

		public SpriteFont Font { get; set; }
		public string Text { get; set; }
		public bool TextOnRight { get; set; }
		public bool Checked { get; set; }
		public Sprite UncheckedImage { get; set; }
		public Sprite CheckedImage { get; set; }
		public Color TextColor { get; set; }
		public Color CheckColor { get; set; }
		public Color CheckPressedColor { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public CheckBoxControl()
		{
			this.TextColor = Color.Black;
			this.CheckColor = Color.White;
			this.CheckPressedColor = Color.Gray;
			this.TextOnRight = true;
			this.Checked = false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public CheckBoxControl(Sprite box, Sprite check)
		{
			this.TextColor = Color.Black;
			this.CheckColor = Color.White;
			this.CheckPressedColor = Color.Gray;
			this.UncheckedImage = box;
			this.CheckedImage = check;
		}

		/// <summary>
		/// 
		/// </summary>
		public override Size Size
		{
			get
			{
				Vector2 vector = Vector2.Zero;
				vector = this.Font.MeasureString(this.Text);
				return new Size((int)((vector.X + (float)this.UncheckedImage.Width + 5f) * this.Scale), (int)(((vector.Y > (float)this.UncheckedImage.Height) ? vector.Y : ((float)this.UncheckedImage.Height)) * this.Scale));
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			Rectangle screenBounds = base.ScreenBounds;
			Vector2 vector = this.Font.MeasureString(this.Text) * this.Scale;
			Color color = this.CheckColor;
			
			if (base.Hovering || base.CaptureInput)
			{
				color = this.CheckPressedColor;
			}
			
			float num = (float)this.UncheckedImage.Height * this.Scale;
			float num2 = (float)this.Font.LineSpacing * this.Scale;
			float num3 = 2f * this.Scale;
			float num4 = 5f * this.Scale;
			
			if (this.TextOnRight)
			{
				spriteBatch.DrawString(this.Font, this.Text, new Vector2((float)(screenBounds.X + this.UncheckedImage.Width) + num4, (float)screenBounds.Y), this.TextColor, 0f, Vector2.Zero, this.Scale, SpriteEffects.None, 0f);
				spriteBatch.Draw(this.UncheckedImage, new Vector2((float)screenBounds.X, (float)screenBounds.Y + num3 + num2 / 2f - num / 2f), this.Scale, color);
				
				if (this.Checked)
				{
					spriteBatch.Draw(this.CheckedImage, new Vector2((float)screenBounds.X, (float)screenBounds.Y + num3 + num2 / 2f - num / 2f), this.Scale, color);
					return;
				}
			}
			else
			{
				spriteBatch.DrawString(this.Font, this.Text, new Vector2((float)screenBounds.Left, (float)screenBounds.Y), this.TextColor, 0f, Vector2.Zero, this.Scale, SpriteEffects.None, 0f);
				spriteBatch.Draw(this.UncheckedImage, new Vector2((float)screenBounds.X + vector.X + num4, (float)screenBounds.Y + num3 + num2 / 2f - num / 2f), this.Scale, color);
				
				if (this.Checked)
				{
					spriteBatch.Draw(this.CheckedImage, new Vector2((float)screenBounds.X + vector.X + num4, (float)screenBounds.Y + num3 + num2 / 2f - num / 2f), this.Scale, color);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public override void OnPressed()
		{
			this.Checked = !this.Checked;
			base.OnPressed();
		}
	}
}
