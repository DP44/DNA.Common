using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI.Controls
{
	public class TextControl : UIControl
	{
		private bool _dirtyText = true;

		private StringBuilder _textToDraw = new StringBuilder();

		public float Scale = 1f;

		private bool _pulseDir;

		private TimeSpan _currenPulseTime = TimeSpan.FromSeconds(0.0);

		private TimeSpan _pulseTime = TimeSpan.FromSeconds(0.0);

		private float _pulseSize = 0.1f;

		private string _text = "<Text>";

		public SpriteFont Font;

		private Color _outLineColor = Color.Black;

		private int _outLineWidth;

		public TimeSpan PulseTime
		{
			get
			{
				return this._pulseTime;
			}
			set
			{
				this._pulseTime = value;
			}
		}

		public Color Color { get; set; }

		public float PulseSize
		{
			get
			{
				return this._pulseSize;
			}
			set
			{
				this._pulseSize = value;
			}
		}

		public string Text
		{
			get
			{
				return this._text;
			}
			set
			{
				if (this._text != value)
				{
					this._text = value;
					this._dirtyText = true;
				}
			}
		}

		public Color OutlineColor
		{
			get
			{
				return this._outLineColor;
			}
			set
			{
				this._outLineColor = value;
			}
		}

		public int OutlineWidth
		{
			get
			{
				return this._outLineWidth;
			}
			set
			{
				this._outLineWidth = value;
			}
		}

		public TextControl(SpriteFont font, string text, Point position, Color color, Color outlineColor, int outlineWidth)
		{
			this.Text = text;
			this.Font = font;
			base.LocalPosition = position;
			this.Color = color;
			this.OutlineColor = Color.Black;
			this.OutlineWidth = outlineWidth;
		}

		public TextControl(SpriteFont font, string text, Point position, Color color)
		{
			this.Text = text;
			this.Font = font;
			base.LocalPosition = position;
			this.Color = color;
			this.OutlineWidth = 0;
		}

		public TextControl(string text, SpriteFont font)
		{
			this.Text = text;
			this.Font = font;
			this.Color = Color.White;
		}

		public TextControl(SpriteFont font)
		{
			this.Font = font;
			this.Color = Color.White;
		}

		public override Size Size
		{
			get
			{
				Vector2 vector = this.Font.MeasureString(this.Text) * this.Scale;
				return new Size((int)vector.X, (int)vector.Y);
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		protected virtual void ProcessText(string text, StringBuilder builder)
		{
			builder.Append(text);
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			if (this._dirtyText)
			{
				this._textToDraw.Length = 0;
				this.ProcessText(this._text, this._textToDraw);
				this._dirtyText = false;
			}
			Vector2 vector = new Vector2((float)base.ScreenPosition.X, (float)base.ScreenPosition.Y);
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
				float scale = (float)(1.0 + (double)this.PulseSize * this._currenPulseTime.TotalSeconds / this._pulseTime.TotalSeconds) * this.Scale;
				Vector2 vector2 = new Vector2((float)this.Size.Width / 2f, (float)this.Size.Height / 2f);
				if (this.OutlineWidth > 0)
				{
					spriteBatch.DrawOutlinedText(this.Font, this._textToDraw, vector + vector2, this.Color, this.OutlineColor, this.OutlineWidth, scale, 0f, vector2);
					return;
				}
				spriteBatch.DrawString(this.Font, this._textToDraw, vector + vector2, this.Color, 0f, vector2, scale, SpriteEffects.None, 1f);
				return;
			}
			else
			{
				if (this.OutlineWidth > 0)
				{
					spriteBatch.DrawOutlinedText(this.Font, this._textToDraw, vector, this.Color, this.OutlineColor, this.OutlineWidth, this.Scale, 0f, Vector2.Zero);
					return;
				}
				spriteBatch.DrawString(this.Font, this._textToDraw, vector, this.Color, 0f, Vector2.Zero, this.Scale, SpriteEffects.None, 0f);
				return;
			}
		}
	}
}
