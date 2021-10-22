using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI
{
	public class TextElement : UIElement
	{
		private bool _dirtyText = true;
		private StringBuilder _textToDraw = new StringBuilder();
		private bool _pulseDir;
		private TimeSpan _currenPulseTime = TimeSpan.FromSeconds(0.0);
		private TimeSpan _pulseTime = TimeSpan.FromSeconds(0.0);
		private float _pulseSize = 0.1f;
		private string _text = "<Text>";
		public SpriteFont Font;
		private Color _outLineColor = Color.Black;
		private int _outLineWidth = 2;

		public bool ScaleOnScreenResize = true;

		/// <summary>
		/// 
		/// </summary>
		public TimeSpan PulseTime
		{
			get => 
				this._pulseTime;
		
			set => 
				this._pulseTime = value;
		}

		/// <summary>
		/// 
		/// </summary>
		public float PulseSize
		{
			get => 
				this._pulseSize;
	
			set => 
				this._pulseSize = value;
		}

		/// <summary>
		/// 
		/// </summary>
		public string Text
		{
			get => 
				this._text;
			
			set
			{
				// Make sure it isn't already set to that.
				// 
				// NOTE: We do this as overwriting the text elements with the same thing
				//       can sometimes lead to weird issues with flickering text.
				if (this._text == value)
				{
					return;
				}

				this._text = value;
				this._dirtyText = true;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Color OutlineColor
		{
			get => 
				this._outLineColor;
			
			set => 
				this._outLineColor = value;
		}

		/// <summary>
		/// 
		/// </summary>
		public int OutlineWidth
		{
			get => 
				this._outLineWidth;
		
			set => 
				this._outLineWidth = value;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public TextElement(SpriteFont font, string text, Vector2 position, 
						   Color color, Color outlineColor, int outlineWidth)
		{
			this.Text = text;
			this.Font = font;
			base.Location = position;
			base.Color = color;
			this.OutlineColor = Color.Black;
			this.OutlineWidth = outlineWidth;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public TextElement(SpriteFont font, string text, Vector2 position, Color color)
		{
			this.Text = text;
			this.Font = font;
			base.Location = position;
			base.Color = color;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public TextElement(string text, SpriteFont font)
		{
			this.Text = text;
			this.Font = font;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public TextElement(SpriteFont font) =>
			this.Font = font;

		/// <summary>
		/// 
		/// </summary>
		public override Vector2 Size
		{
			get =>
				this.Font.MeasureString(this.Text);

			set =>
				throw new NotSupportedException();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected virtual Color GetForColor(bool selected) =>
			base.Color;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected virtual void ProcessText(string text, StringBuilder builder) =>
			builder.Append(text);

		/// <summary>
		/// 
		/// </summary>
		protected void DirtyText() =>
			this._dirtyText = true;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, 
									   GameTime gameTime, bool selected)
		{
			if (this._dirtyText)
			{
				this._textToDraw.Length = 0;
				this.ProcessText(this._text, this._textToDraw);
				this._dirtyText = false;
			}

			if (this._pulseTime > TimeSpan.Zero)
			{
				if (this._pulseDir)
				{
					this._currenPulseTime += gameTime.ElapsedGameTime;
				
					if (this._currenPulseTime > this._pulseTime)
					{
						this._currenPulseTime = this._pulseTime;
						this._pulseDir = !this._pulseDir;
					}
				}
				else
				{
					this._currenPulseTime -= gameTime.ElapsedGameTime;
				
					if (this._currenPulseTime < TimeSpan.Zero)
					{
						this._currenPulseTime = TimeSpan.Zero;
						this._pulseDir = !this._pulseDir;
					}
				}
				
				float scale = (float)(1.0 + (double)this.PulseSize * 
					this._currenPulseTime.TotalSeconds / this._pulseTime.TotalSeconds) * 
						(this.ScaleOnScreenResize ? Screen.Adjuster.ScaleFactor.Y : 1f);
				
				Vector2 vector = new Vector2(this.Size.X / 2f, this.Size.Y / 2f);
				
				if (this.OutlineWidth > 0)
				{
					spriteBatch.DrawOutlinedText(this.Font, this._textToDraw, 
						base.Location + vector, this.GetForColor(selected), this.OutlineColor, 
						this.OutlineWidth, scale, 0f, vector);
				}
				else
				{
					spriteBatch.DrawString(this.Font, this._textToDraw, base.Location + vector, 
						this.GetForColor(selected), 0f, vector, scale, SpriteEffects.None, 1f);
				}
			}
			else
			{
				float num = this.ScaleOnScreenResize ? Screen.Adjuster.ScaleFactor.Y : 1f;
				
				if (this.OutlineWidth > 0)
				{
					spriteBatch.DrawOutlinedText(this.Font, this._textToDraw, base.Location, 
						this.GetForColor(selected), this.OutlineColor, 
						(int)Math.Ceiling((double)((float)this.OutlineWidth * num)), 
						num, 0f, new Vector2(0f, 0f));
				} 
				else
				{
					spriteBatch.DrawString(this.Font, this._textToDraw, base.Location, 
						this.GetForColor(selected), 0f, new Vector2(0f, 0f), num, 
						SpriteEffects.None, 0f);
				}
			}
		}
	}
}
