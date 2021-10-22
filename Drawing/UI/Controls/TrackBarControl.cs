using System;
using DNA.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI.Controls
{
	public class TrackBarControl : UIControl
	{
		private const int ControlHeight = 20;

		private const int SliderWidth = 12;

		private const int TrackWidth = 8;

		public Color FillColor = Color.Gray;

		private int _minValue;

		private int _maxValue = 100;

		private Size _size = new Size(100, 20);

		private int _value = 50;

		private bool _hovering;

		public int MinValue
		{
			get
			{
				return this._minValue;
			}
			set
			{
				this._minValue = value;
			}
		}

		public int MaxValue
		{
			get
			{
				return this._maxValue;
			}
			set
			{
				this._maxValue = value;
			}
		}

		public override Size Size
		{
			get
			{
				return this._size;
			}
			set
			{
				this._size = new Size(value.Width, 20);
			}
		}

		public int Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this._value = value;
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

		protected override void OnInput(InputManager inputManager, GameController controller, KeyboardInput chatPad, GameTime gameTime)
		{
			bool flag = this.GetSliderLocation(base.ScreenBounds).Contains(inputManager.Mouse.Position);
			if (flag)
			{
				this.Hovering = true;
				if (inputManager.Mouse.LeftButtonPressed)
				{
					base.CaptureInput = true;
				}
			}
			else
			{
				this.Hovering = false;
			}
			if (inputManager.Mouse.LeftButtonReleased)
			{
				base.CaptureInput = false;
			}
			if (base.CaptureInput)
			{
				Rectangle trackLocation = this.GetTrackLocation(base.ScreenBounds);
				int x = inputManager.Mouse.Position.X;
				int num = (x - trackLocation.Left) * (this.MaxValue - this.MinValue) / (trackLocation.Width - 1);
				num += this.MinValue;
				if (num > this.MaxValue)
				{
					this.Value = this.MaxValue;
				}
				else if (num < this.MinValue)
				{
					this.Value = this.MinValue;
				}
				else
				{
					this.Value = num;
				}
			}
			base.OnInput(inputManager, controller, chatPad, gameTime);
		}

		private Rectangle GetSliderLocation(Rectangle screenBounds)
		{
			Rectangle trackLocation = this.GetTrackLocation(screenBounds);
			int num = this._value * (trackLocation.Width - 1) / (this.MaxValue - this.MinValue) + trackLocation.Left;
			return new Rectangle(num - 6, screenBounds.Y, 12, screenBounds.Height);
		}

		private Rectangle GetTrackLocation(Rectangle screenBounds)
		{
			return new Rectangle(screenBounds.Left, screenBounds.Center.Y - 4, screenBounds.Width - 6, 8);
		}

		public override bool HitTest(Point screenPoint)
		{
			return this.GetSliderLocation(base.ScreenBounds).Contains(screenPoint);
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			Rectangle screenBounds = base.ScreenBounds;
			Rectangle sliderLocation = this.GetSliderLocation(screenBounds);
			Rectangle trackLocation = this.GetTrackLocation(screenBounds);
			Rectangle destinationRectangle = new Rectangle(trackLocation.X, trackLocation.Y, sliderLocation.Center.X - trackLocation.X, trackLocation.Height);
			Color color = Color.White;
			if (base.CaptureInput)
			{
				color = Color.Black;
			}
			else if (this.Hovering)
			{
				color = Color.Gray;
			}
			spriteBatch.Draw(UIControl.DummyTexture, trackLocation, Color.Black);
			trackLocation.Inflate(-1, -1);
			spriteBatch.Draw(UIControl.DummyTexture, trackLocation, Color.White);
			destinationRectangle.Inflate(-1, -1);
			spriteBatch.Draw(UIControl.DummyTexture, destinationRectangle, this.FillColor);
			spriteBatch.Draw(UIControl.DummyTexture, sliderLocation, Color.Black);
			sliderLocation.Inflate(-1, -1);
			spriteBatch.Draw(UIControl.DummyTexture, sliderLocation, color);
		}
	}
}
