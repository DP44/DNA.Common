using System;
using System.Text;
using DNA.Input;
using DNA.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI.Controls
{
	public class TextEditControl : UIControl
	{
		private bool _hideInput;

		public float Scale = 1f;

		private string _visibleText;

		private string _text;

		private int _curPos;

		private Size _size = new Size(200, 40);

		private StringBuilder _builder = new StringBuilder();

		private OneShotTimer _cursorTimer = new OneShotTimer(TimeSpan.FromSeconds(0.5), true);

		private StringBuilder _drawBuilder = new StringBuilder();

		public ScalableFrame Frame { get; set; }

		public SpriteFont Font { get; set; }

		public int MaxChars { get; set; }

		public bool HideInput
		{
			get
			{
				return this._hideInput;
			}
			set
			{
				this._hideInput = value;
				this.Text = this._text;
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
				this._text = value;
				this._curPos = this._text.Length;
				if (this.HideInput)
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append('*', this._text.Length);
					this._visibleText = stringBuilder.ToString();
					return;
				}
				this._visibleText = this._text;
			}
		}

		public int CursorPos
		{
			get
			{
				return this._curPos;
			}
			set
			{
				this._curPos = value;
				if (this._curPos < 0)
				{
					this._curPos = 0;
				}
				if (this._curPos > this.Text.Length)
				{
					this._curPos = this.Text.Length;
				}
			}
		}

		public Color FrameColor { get; set; }

		public Color TextColor { get; set; }

		public TextEditControl()
		{
			this.Text = "";
			this.HideInput = false;
			this.MaxChars = -1;
			this.FrameColor = Color.White;
			this.TextColor = Color.Black;
			base.IsTabStop = true;
		}

		public override Size Size
		{
			get
			{
				return new Size((int)(this.Scale * (float)this._size.Width), (int)(this.Scale * (float)this.Font.LineSpacing));
			}
			set
			{
				this._size = value;
			}
		}

		protected override void OnInput(InputManager inputManager, GameController controller, KeyboardInput chatPad, GameTime gameTime)
		{
			bool flag = this.HitTest(inputManager.Mouse.Position);
			if (flag && inputManager.Mouse.LeftButtonPressed)
			{
				base.CaptureInput = true;
			}
			if (base.CaptureInput)
			{
				this._curPos = this.LetterHitTest(inputManager.Mouse.Position.X);
			}
			if (inputManager.Mouse.LeftButtonReleased)
			{
				base.CaptureInput = false;
			}
			base.OnInput(inputManager, controller, chatPad, gameTime);
		}

		public int LetterHitTest(int xloc)
		{
			Rectangle rectangle = this.Frame.CenterRegion(base.ScreenBounds);
			if (xloc <= rectangle.Left)
			{
				return 0;
			}
			lock (this._builder)
			{
				this._builder.Clear();
				for (int i = 0; i < this._visibleText.Length; i++)
				{
					this._builder.Append(this._visibleText, i, 1);
					Vector2 vector = this.Font.MeasureString(this._builder) * this.Scale;
					if ((float)xloc <= (float)rectangle.Left + vector.X)
					{
						return i;
					}
				}
			}
			return (int)((float)this.Text.Length * this.Scale);
		}

		public event EventHandler EnterPressed;

		protected override void OnChar(char c)
		{
			StringBuilder stringBuilder = new StringBuilder(this.Text);
			switch (c)
			{
			case '\b':
				if (stringBuilder.Length > 0 && this._curPos > 0)
				{
					stringBuilder.Remove(this._curPos - 1, 1);
					this._curPos--;
				}
				break;
			case '\t':
				break;
			default:
				if (c != '\r')
				{
					if (c != '\u0016' && (char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsWhiteSpace(c)) && (this.MaxChars < 0 || this.Text.Length < this.MaxChars))
					{
						StringBuilder stringBuilder2 = new StringBuilder(stringBuilder.ToString());
						stringBuilder2.Insert(this._curPos, c);
						Vector2 vector = this.Font.MeasureString(stringBuilder2) * this.Scale;
						Rectangle rectangle = this.Frame.CenterRegion(base.ScreenBounds);
						if (vector.X < (float)rectangle.Width)
						{
							stringBuilder.Insert(this._curPos, c);
							this._curPos++;
						}
					}
				}
				else
				{
					if (this.EnterPressed != null)
					{
						this.EnterPressed(this, new EventArgs());
					}
					stringBuilder = new StringBuilder(this.Text);
				}
				break;
			}
			this.Text = stringBuilder.ToString();
			base.OnChar(c);
		}

		protected override void OnUpdate(DNAGame game, GameTime gameTime)
		{
			this._cursorTimer.Update(gameTime.ElapsedGameTime);
			base.OnUpdate(game, gameTime);
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			Rectangle screenBounds = base.ScreenBounds;
			this.Frame.Draw(spriteBatch, screenBounds, this.FrameColor);
			Rectangle rectangle = this.Frame.CenterRegion(base.ScreenBounds);
			spriteBatch.DrawString(this.Font, this._visibleText, new Vector2((float)rectangle.Left, (float)screenBounds.Top), this.TextColor, 0f, Vector2.Zero, this.Scale, SpriteEffects.None, 0f);
			if (base.HasFocus && this._cursorTimer.PercentComplete < 0.5f)
			{
				Vector2 vector = this.Font.MeasureString(this._visibleText.Substring(0, this._curPos)) * this.Scale;
				spriteBatch.DrawString(this.Font, "|", new Vector2((float)(rectangle.Left + (int)vector.X), (float)(screenBounds.Top - 2)), this.TextColor, 0f, Vector2.Zero, this.Scale, SpriteEffects.None, 0f);
			}
		}
	}
}
