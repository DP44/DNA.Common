using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DNA.Input
{
	public class MouseInput
	{
		private struct LocalMouseState
		{
			public MouseState MouseState;

			public LocalMouseState(int x, int y, int scrollWheel, ButtonState leftButton, ButtonState middleButton, ButtonState rightButton, ButtonState xButton1, ButtonState xButton2)
			{
				this.MouseState = new MouseState(x, y, scrollWheel, leftButton, middleButton, rightButton, xButton1, xButton2);
			}

			public LocalMouseState(MouseState mouseState)
			{
				this.MouseState = mouseState;
			}

			public void SetMouseState(MouseState mouseState)
			{
				this.MouseState = mouseState;
			}

			public static bool operator != (MouseInput.LocalMouseState left, MouseInput.LocalMouseState right)
			{
				return left.MouseState != right.MouseState;
			}

			public static bool operator == (MouseInput.LocalMouseState left, MouseInput.LocalMouseState right)
			{
				return left.MouseState == right.MouseState;
			}

			public ButtonState LeftButton
			{
				get
				{
					return this.MouseState.LeftButton;
				}
			}

			public ButtonState MiddleButton
			{
				get
				{
					return this.MouseState.MiddleButton;
				}
			}

			public ButtonState RightButton
			{
				get
				{
					return this.MouseState.RightButton;
				}
			}

			public int ScrollWheelValue
			{
				get
				{
					return this.MouseState.ScrollWheelValue;
				}
			}

			public int X
			{
				get
				{
					return this.MouseState.X;
				}
				set
				{
					this.MouseState = new MouseState(value, this.MouseState.Y, this.MouseState.ScrollWheelValue, this.MouseState.LeftButton, this.MouseState.MiddleButton, this.MouseState.RightButton, this.MouseState.XButton1, this.MouseState.XButton2);
				}
			}

			public ButtonState XButton1
			{
				get
				{
					return this.MouseState.XButton1;
				}
			}

			public ButtonState XButton2
			{
				get
				{
					return this.MouseState.XButton2;
				}
			}

			public int Y
			{
				get
				{
					return this.MouseState.Y;
				}
				set
				{
					this.MouseState = new MouseState(this.MouseState.X, value, this.MouseState.ScrollWheelValue, this.MouseState.LeftButton, this.MouseState.MiddleButton, this.MouseState.RightButton, this.MouseState.XButton1, this.MouseState.XButton2);
				}
			}

			public override bool Equals(object obj)
			{
				return this.MouseState.Equals(obj);
			}

			public override int GetHashCode()
			{
				return this.MouseState.GetHashCode();
			}

			public override string ToString()
			{
				return this.MouseState.ToString();
			}
		}

		private DNAGame _game;
		private MouseInput.LocalMouseState _lastState;
		private MouseInput.LocalMouseState _currentState;
		private bool _wheelMovedUpLastFrame;
		private bool _wheelMovedDownLastFrame;

		public MouseState LastState
		{
			get
			{
				return this._lastState.MouseState;
			}
		}

		public MouseState CurrentState
		{
			get
			{
				return this._currentState.MouseState;
			}
		}

		public Point Position
		{
			get
			{
				return this._game.ScreenToBuffer(new Point(this._currentState.X, this._currentState.Y));
			}
		}

		public Point LastPosition
		{
			get
			{
				return this._game.ScreenToBuffer(new Point(this._lastState.X, this._lastState.Y));
			}
		}

		public Vector2 DeltaPosition
		{
			get
			{
				Vector2 vector = this._game.ScreenToBuffer(new Vector2((float)this._currentState.X, (float)this._currentState.Y));
				Vector2 vector2 = this._game.ScreenToBuffer(new Vector2((float)this._lastState.X, (float)this._lastState.Y));
				return new Vector2(vector.X - vector2.X, vector.Y - vector2.Y);
			}
		}

		public int DeltaWheel
		{
			get
			{
				return this._currentState.ScrollWheelValue - this._lastState.ScrollWheelValue;
			}
		}

		public void SetPosition(int x, int y, bool resetDeltas)
		{
			Mouse.SetPosition(x, y);
			this._lastState.X = x;
			this._lastState.Y = y;
			
			if (resetDeltas)
			{
				this._currentState.X = x;
				this._currentState.Y = y;
			}
		}

		public bool LeftButtonDown
		{
			get
			{
				return this._currentState.LeftButton == ButtonState.Pressed;
			}
		}

		public bool MiddleButtonDown
		{
			get
			{
				return this._currentState.MiddleButton == ButtonState.Pressed;
			}
		}

		public bool RightButtonDown
		{
			get
			{
				return this._currentState.RightButton == ButtonState.Pressed;
			}
		}

		public bool XButton1Down
		{
			get
			{
				return this._currentState.XButton1 == ButtonState.Pressed;
			}
		}

		public bool XButton2Down
		{
			get
			{
				return this._currentState.XButton2 == ButtonState.Pressed;
			}
		}

		public bool WheelUpDown
		{
			get
			{
				return this.DeltaWheel > 0;
			}
		}

		public bool WheelDownDown
		{
			get
			{
				return this.DeltaWheel < 0;
			}
		}

		public bool LeftButtonPressed
		{
			get
			{
				return this._currentState.LeftButton == ButtonState.Pressed && this._lastState.LeftButton == ButtonState.Released;
			}
		}

		public bool MiddleButtonPressed
		{
			get
			{
				return this._currentState.MiddleButton == ButtonState.Pressed && this._lastState.MiddleButton == ButtonState.Released;
			}
		}

		public bool RightButtonPressed
		{
			get
			{
				return this._currentState.RightButton == ButtonState.Pressed && this._lastState.RightButton == ButtonState.Released;
			}
		}

		public bool XButton1Pressed
		{
			get
			{
				return this._currentState.XButton1 == ButtonState.Pressed && this._lastState.XButton1 == ButtonState.Released;
			}
		}

		public bool XButton2Pressed
		{
			get
			{
				return this._currentState.XButton2 == ButtonState.Pressed && this._lastState.XButton2 == ButtonState.Released;
			}
		}

		public bool WheelUpPressed
		{
			get
			{
				return this.DeltaWheel > 0;
			}
		}

		public bool WheelDownPressed
		{
			get
			{
				return this.DeltaWheel < 0;
			}
		}

		public bool LeftButtonReleased
		{
			get
			{
				return this._lastState.LeftButton == ButtonState.Pressed && this._currentState.LeftButton == ButtonState.Released;
			}
		}

		public bool MiddleButtonReleased
		{
			get
			{
				return this._lastState.MiddleButton == ButtonState.Pressed && this._currentState.MiddleButton == ButtonState.Released;
			}
		}

		public bool RightButtonReleased
		{
			get
			{
				return this._lastState.RightButton == ButtonState.Pressed && this._currentState.RightButton == ButtonState.Released;
			}
		}

		public bool XButton1Released
		{
			get
			{
				return this._lastState.XButton1 == ButtonState.Pressed && this._currentState.XButton1 == ButtonState.Released;
			}
		}

		public bool XButton2Released
		{
			get
			{
				return this._lastState.XButton2 == ButtonState.Pressed && this._currentState.XButton2 == ButtonState.Released;
			}
		}

		public bool WheelUpReleased
		{
			get
			{
				return this._wheelMovedUpLastFrame && this.DeltaWheel <= 0;
			}
		}

		public bool WheelDownReleased
		{
			get
			{
				return this._wheelMovedDownLastFrame && this.DeltaWheel >= 0;
			}
		}

		public void Update()
		{
			int deltaWheel = this.DeltaWheel;
			this._wheelMovedDownLastFrame = this.WheelDownDown;
			this._wheelMovedUpLastFrame = this.WheelUpDown;
			this._lastState = this._currentState;
			this._currentState.SetMouseState(Mouse.GetState());
		}

		public MouseInput(DNAGame game)
		{
			this._game = game;
		}
	}
}
