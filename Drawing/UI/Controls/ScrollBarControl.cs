using System;
using DNA.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI.Controls
{
	public class ScrollBarControl : UIControl
	{
		public Color TrackColor = Color.LightGray;

		public Color SliderColor = Color.DarkGray;

		public Color HoverColor = Color.Gray;

		public Color SelectedColor = Color.DimGray;

		public int SliderHeight = 50;

		private float _sliderTop;

		public int ArrowSize = 20;

		private bool _hovering;

		private bool _trackHover;

		private bool _upperArrowHover;

		private bool _lowerArrowHover;

		private bool _upperArrowCaptureInput;

		private bool _lowerArrowCaptureInput;

		private bool _sliderCaptureInput;

		public float Value
		{
			get
			{
				int num = base.ScreenBounds.Height - (this.SliderHeight + this.ArrowSize * 2) - 1;
				return this._sliderTop / (float)num;
			}
			set
			{
				int num = base.ScreenBounds.Height - (this.SliderHeight + this.ArrowSize * 2) - 1;
				this._sliderTop = value * (float)num;
			}
		}

		protected bool Hovering
		{
			get
			{
				return this._hovering;
			}
			set
			{
				this._hovering = value;
			}
		}

		public override Size Size { get; set; }

		public event EventHandler UpArrowPressed;

		public void OnUpArrowPressed()
		{
			if (this.UpArrowPressed != null)
			{
				this.UpArrowPressed(this, new EventArgs());
			}
		}

		public event EventHandler DownArrowPressed;

		public void OnDownArrowPressed()
		{
			if (this.DownArrowPressed != null)
			{
				this.DownArrowPressed(this, new EventArgs());
			}
		}

		protected override void OnInput(InputManager inputManager, GameController controller, KeyboardInput chatPad, GameTime gameTime)
		{
			bool flag = this.GetSliderLocation(base.ScreenBounds).Contains(inputManager.Mouse.Position);
			Rectangle rectangle = new Rectangle(base.ScreenBounds.X + 1, base.ScreenBounds.Y + 1, base.ScreenBounds.Width - 2, this.ArrowSize - 2);
			Rectangle rectangle2 = new Rectangle(base.ScreenBounds.X + 1, base.ScreenBounds.Bottom - this.ArrowSize, base.ScreenBounds.Width - 2, this.ArrowSize - 2);
			if (flag && !this._upperArrowCaptureInput && !this._lowerArrowCaptureInput)
			{
				this.Hovering = true;
				if (inputManager.Mouse.LeftButtonPressed)
				{
					this._sliderCaptureInput = true;
				}
			}
			else
			{
				this.Hovering = false;
			}
			if (inputManager.Mouse.LeftButtonReleased)
			{
				this._sliderCaptureInput = false;
				this._lowerArrowCaptureInput = false;
				this._upperArrowCaptureInput = false;
			}
			if (this._sliderCaptureInput)
			{
				this._sliderTop += inputManager.Mouse.DeltaPosition.Y;
				if (this._sliderTop < 0f)
				{
					this._sliderTop = 0f;
				}
				if (this._sliderTop > (float)(base.ScreenBounds.Height - (this.SliderHeight + this.ArrowSize * 2) - 1))
				{
					this._sliderTop = (float)(base.ScreenBounds.Height - (this.SliderHeight + this.ArrowSize * 2) - 1);
				}
				this._upperArrowCaptureInput = false;
				this._lowerArrowCaptureInput = false;
				this._upperArrowHover = false;
				this._lowerArrowHover = false;
			}
			else if (!this._lowerArrowHover && !this._lowerArrowCaptureInput && rectangle.Contains(inputManager.Mouse.Position))
			{
				this._lowerArrowCaptureInput = false;
				this._lowerArrowHover = false;
				this._upperArrowHover = true;
				if (inputManager.Mouse.LeftButtonPressed)
				{
					this._upperArrowCaptureInput = true;
					this.OnUpArrowPressed();
				}
			}
			else if (!this._upperArrowHover && !this._upperArrowCaptureInput && rectangle2.Contains(inputManager.Mouse.Position))
			{
				this._upperArrowCaptureInput = false;
				this._upperArrowHover = false;
				this._lowerArrowHover = true;
				if (inputManager.Mouse.LeftButtonPressed)
				{
					this._lowerArrowCaptureInput = true;
					this.OnDownArrowPressed();
				}
			}
			else
			{
				this._upperArrowHover = false;
				this._lowerArrowHover = false;
			}
			if (this._sliderCaptureInput || this._lowerArrowCaptureInput || this._upperArrowCaptureInput || base.ScreenBounds.Contains(inputManager.Mouse.Position))
			{
				this._trackHover = true;
			}
			else
			{
				this._trackHover = false;
			}
			base.CaptureInput = (this._sliderCaptureInput || this._lowerArrowCaptureInput || this._upperArrowCaptureInput);
			base.OnInput(inputManager, controller, chatPad, gameTime);
		}

		private Rectangle GetSliderLocation(Rectangle screenBounds)
		{
			return new Rectangle(screenBounds.X + 1, (int)this._sliderTop + base.ScreenBounds.Y + this.ArrowSize, screenBounds.Width - 2, this.SliderHeight);
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			spriteBatch.Draw(UIControl.DummyTexture, base.ScreenBounds, this.TrackColor);
			Color color = this.SliderColor;
			if (this._sliderCaptureInput)
			{
				color = this.SelectedColor;
			}
			else if (this._hovering)
			{
				color = this.HoverColor;
			}
			Rectangle sliderLocation = this.GetSliderLocation(base.ScreenBounds);
			spriteBatch.Draw(UIControl.DummyTexture, sliderLocation, Color.Black);
			sliderLocation.Inflate(-1, -1);
			spriteBatch.Draw(UIControl.DummyTexture, sliderLocation, color);
			Rectangle destinationRectangle = new Rectangle(base.ScreenBounds.X + 1, base.ScreenBounds.Y + 1, base.ScreenBounds.Width - 2, this.ArrowSize - 2);
			Rectangle destinationRectangle2 = new Rectangle(base.ScreenBounds.X + 1, base.ScreenBounds.Bottom - this.ArrowSize, base.ScreenBounds.Width - 2, this.ArrowSize - 2);
			if (this._trackHover)
			{
				spriteBatch.Draw(UIControl.DummyTexture, destinationRectangle, Color.Black);
				destinationRectangle.Inflate(-1, -1);
				if (this._upperArrowCaptureInput)
				{
					spriteBatch.Draw(UIControl.DummyTexture, destinationRectangle, this.SelectedColor);
				}
				else if (this._upperArrowHover)
				{
					spriteBatch.Draw(UIControl.DummyTexture, destinationRectangle, this.HoverColor);
				}
				else
				{
					spriteBatch.Draw(UIControl.DummyTexture, destinationRectangle, this.SliderColor);
				}
				spriteBatch.Draw(UIControl.DummyTexture, destinationRectangle2, Color.Black);
				destinationRectangle2.Inflate(-1, -1);
				if (this._lowerArrowCaptureInput)
				{
					spriteBatch.Draw(UIControl.DummyTexture, destinationRectangle2, this.SelectedColor);
				}
				else if (this._lowerArrowHover)
				{
					spriteBatch.Draw(UIControl.DummyTexture, destinationRectangle2, this.HoverColor);
				}
				else
				{
					spriteBatch.Draw(UIControl.DummyTexture, destinationRectangle2, this.SliderColor);
				}
			}
			spriteBatch.Draw(UIControl.DummyTexture, new Rectangle(base.ScreenBounds.Center.X, base.ScreenBounds.Y + 5, 3, 10), null, Color.Black, Angle.FromDegrees(45f).Radians, Vector2.Zero, SpriteEffects.None, 0f);
			spriteBatch.Draw(UIControl.DummyTexture, new Rectangle(base.ScreenBounds.Center.X - 1, base.ScreenBounds.Y + 7, 3, 10), null, Color.Black, Angle.FromDegrees(-45f).Radians, Vector2.Zero, SpriteEffects.None, 0f);
			spriteBatch.Draw(UIControl.DummyTexture, new Rectangle(base.ScreenBounds.Center.X + 6, base.ScreenBounds.Bottom - 15, 3, 10), null, Color.Black, Angle.FromDegrees(45f).Radians, Vector2.Zero, SpriteEffects.None, 0f);
			spriteBatch.Draw(UIControl.DummyTexture, new Rectangle(base.ScreenBounds.Center.X - 7, base.ScreenBounds.Bottom - 12, 3, 10), null, Color.Black, Angle.FromDegrees(-45f).Radians, Vector2.Zero, SpriteEffects.None, 0f);
		}
	}
}
