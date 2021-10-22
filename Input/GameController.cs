using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DNA.Input
{
	public class GameController
	{
		private PlayerIndex _playerIndex;

		private GamePadState _lastState;

		private GamePadState _currentState;

		private ControllerButtons _pressedButtons;

		private ControllerButtons _releasedButtons;

		private ControllerDPad _pressedDPad;

		private ControllerDPad _releasedDPad;

		private GamePadCapabilities _caps;

		public float LeftTriggerPullThreshhold = 0.25f;

		public float RightTriggerPullThreshhold = 0.25f;

		public float LeftStickThreshold = 0.1f;

		public float RightStickThreshold = 0.1f;

		private static readonly GamePadDPad _unpressedDPad = new GamePadDPad(ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released);

		public bool ButtonPressed
		{
			get
			{
				return this._pressedButtons.A || this._pressedButtons.B || this._pressedButtons.Back || this._pressedButtons.BigButton || this._pressedButtons.LeftShoulder || this._pressedButtons.LeftStick || this._pressedButtons.LeftTrigger || this._pressedButtons.RightShoulder || this._pressedButtons.RightStick || this._pressedButtons.RightTrigger || this._pressedButtons.Start || this._pressedButtons.X || this._pressedButtons.Y;
			}
		}

		public bool Activity
		{
			get
			{
				return this.ButtonPressed || this._currentState.ThumbSticks.Left.LengthSquared() > this.LeftStickThreshold * this.LeftStickThreshold || this._currentState.ThumbSticks.Right.LengthSquared() > this.RightStickThreshold * this.RightStickThreshold || this._currentState.DPad != GameController._unpressedDPad;
			}
		}

		public bool GetButtonPressed(Buttons btn)
		{
			switch (btn)
			{
				case Buttons.DPadUp:
				case Buttons.DPadDown:

				case Buttons.DPadLeft:
				{
					break;
				}

				case Buttons.DPadUp | Buttons.DPadDown:
				{
					return this._pressedButtons[btn];
				}

				default:
				{
					if (btn != Buttons.DPadRight)
					{
						return this._pressedButtons[btn];
					}

					break;
				}
			}

			return this._pressedDPad[btn];
		}

		public bool GetButtonReleased(Buttons btn)
		{
			switch (btn)
			{
				case Buttons.DPadUp:
				case Buttons.DPadDown:

				case Buttons.DPadLeft:
				{
					break;
				}
				
				case Buttons.DPadUp | Buttons.DPadDown:
				{
					return this._releasedButtons[btn];
				}

				default:
				{
					if (btn != Buttons.DPadRight)
					{
						return this._releasedButtons[btn];
					}

					break;
				}
			}

			return this._releasedDPad[btn];
		}

		public bool GetButtonHeld(Buttons btn)
		{
			return this._currentState.IsButtonDown(btn);
		}

		public PlayerIndex PlayerIndex
		{
			get
			{
				return this._playerIndex;
			}
		}

		public GamePadState CurrentState
		{
			get
			{
				return this._currentState;
			}
		}

		public GamePadState LastState
		{
			get
			{
				return this._lastState;
			}
		}

		public ControllerButtons PressedButtons
		{
			get
			{
				return this._pressedButtons;
			}
		}

		public ControllerButtons ReleasedButtons
		{
			get
			{
				return this._releasedButtons;
			}
		}

		public ControllerDPad PressedDPad
		{
			get
			{
				return this._pressedDPad;
			}
		}

		public ControllerDPad ReleasedDPad
		{
			get
			{
				return this._releasedDPad;
			}
		}

		public GameController(PlayerIndex index)
		{
			this._playerIndex = index;
		}

		public void SetVibration(float left, float right)
		{
			GamePad.SetVibration(this._playerIndex, left, right);
		}

		public void Update()
		{
			this._lastState = this._currentState;
			this._currentState = GamePad.GetState(this._playerIndex);
			this._caps = GamePad.GetCapabilities(this._playerIndex);
			Buttons buttons = (Buttons)0;

			if (this._lastState.Triggers.Left <= this.LeftTriggerPullThreshhold && this._currentState.Triggers.Left > this.LeftTriggerPullThreshhold)
			{
				buttons |= Buttons.LeftTrigger;
			}
			
			if (this._lastState.Triggers.Right <= this.RightTriggerPullThreshhold && this._currentState.Triggers.Right > this.RightTriggerPullThreshhold)
			{
				buttons |= Buttons.RightTrigger;
			}
		
			if (this._lastState.Buttons.A == ButtonState.Released && this._currentState.Buttons.A == ButtonState.Pressed)
			{
				buttons |= Buttons.A;
			}
			
			if (this._lastState.Buttons.X == ButtonState.Released && this._currentState.Buttons.X == ButtonState.Pressed)
			{
				buttons |= Buttons.X;
			}
			
			if (this._lastState.Buttons.B == ButtonState.Released && this._currentState.Buttons.B == ButtonState.Pressed)
			{
				buttons |= Buttons.B;
			}
			
			if (this._lastState.Buttons.Y == ButtonState.Released && this._currentState.Buttons.Y == ButtonState.Pressed)
			{
				buttons |= Buttons.Y;
			}
			
			if (this._lastState.Buttons.Start == ButtonState.Released && this._currentState.Buttons.Start == ButtonState.Pressed)
			{
				buttons |= Buttons.Start;
			}
			
			if (this._lastState.Buttons.Back == ButtonState.Released && this._currentState.Buttons.Back == ButtonState.Pressed)
			{
				buttons |= Buttons.Back;
			}
			
			if (this._lastState.Buttons.BigButton == ButtonState.Released && this._currentState.Buttons.BigButton == ButtonState.Pressed)
			{
				buttons |= Buttons.BigButton;
			}
			
			if (this._lastState.Buttons.LeftShoulder == ButtonState.Released && this._currentState.Buttons.LeftShoulder == ButtonState.Pressed)
			{
				buttons |= Buttons.LeftShoulder;
			}
			
			if (this._lastState.Buttons.LeftStick == ButtonState.Released && this._currentState.Buttons.LeftStick == ButtonState.Pressed)
			{
				buttons |= Buttons.LeftStick;
			}
			
			if (this._lastState.Buttons.RightShoulder == ButtonState.Released && this._currentState.Buttons.RightShoulder == ButtonState.Pressed)
			{
				buttons |= Buttons.RightShoulder;
			}
			
			if (this._lastState.Buttons.RightStick == ButtonState.Released && this._currentState.Buttons.RightStick == ButtonState.Pressed)
			{
				buttons |= Buttons.RightStick;
			}
			
			this._pressedButtons = new ControllerButtons(buttons);
			Buttons buttons2 = (Buttons)0;
			
			if (this._lastState.Triggers.Left > this.LeftTriggerPullThreshhold && this._currentState.Triggers.Left <= this.LeftTriggerPullThreshhold)
			{
				buttons2 |= Buttons.LeftTrigger;
			}
			
			if (this._lastState.Triggers.Right > this.RightTriggerPullThreshhold && this._currentState.Triggers.Right <= this.RightTriggerPullThreshhold)
			{
				buttons2 |= Buttons.RightTrigger;
			}
			
			if (this._lastState.Buttons.A == ButtonState.Pressed && this._currentState.Buttons.A == ButtonState.Released)
			{
				buttons2 |= Buttons.A;
			}
			
			if (this._lastState.Buttons.X == ButtonState.Pressed && this._currentState.Buttons.X == ButtonState.Released)
			{
				buttons2 |= Buttons.X;
			}
			
			if (this._lastState.Buttons.B == ButtonState.Pressed && this._currentState.Buttons.B == ButtonState.Released)
			{
				buttons2 |= Buttons.B;
			}
			
			if (this._lastState.Buttons.Y == ButtonState.Pressed && this._currentState.Buttons.Y == ButtonState.Released)
			{
				buttons2 |= Buttons.Y;
			}
			
			if (this._lastState.Buttons.Start == ButtonState.Pressed && this._currentState.Buttons.Start == ButtonState.Released)
			{
				buttons2 |= Buttons.Start;
			}
			
			if (this._lastState.Buttons.Back == ButtonState.Pressed && this._currentState.Buttons.Back == ButtonState.Released)
			{
				buttons2 |= Buttons.Back;
			}
			
			if (this._lastState.Buttons.BigButton == ButtonState.Pressed && this._currentState.Buttons.BigButton == ButtonState.Released)
			{
				buttons2 |= Buttons.BigButton;
			}
			
			if (this._lastState.Buttons.LeftShoulder == ButtonState.Pressed && this._currentState.Buttons.LeftShoulder == ButtonState.Released)
			{
				buttons2 |= Buttons.LeftShoulder;
			}
			
			if (this._lastState.Buttons.LeftStick == ButtonState.Pressed && this._currentState.Buttons.LeftStick == ButtonState.Released)
			{
				buttons2 |= Buttons.LeftStick;
			}
			
			if (this._lastState.Buttons.RightShoulder == ButtonState.Pressed && this._currentState.Buttons.RightShoulder == ButtonState.Released)
			{
				buttons2 |= Buttons.RightShoulder;
			}
			
			if (this._lastState.Buttons.RightStick == ButtonState.Pressed && this._currentState.Buttons.RightStick == ButtonState.Released)
			{
				buttons2 |= Buttons.RightStick;
			}
			
			this._releasedButtons = new ControllerButtons(buttons2);
			
			bool upValue = false;
			bool downValue = false;
			bool leftValue = false;
			bool rightValue = false;
			
			if (this._lastState.DPad.Up == ButtonState.Released && this._currentState.DPad.Up == ButtonState.Pressed)
			{
				upValue = true;
			}
			
			if (this._lastState.DPad.Down == ButtonState.Released && this._currentState.DPad.Down == ButtonState.Pressed)
			{
				downValue = true;
			}
			
			if (this._lastState.DPad.Left == ButtonState.Released && this._currentState.DPad.Left == ButtonState.Pressed)
			{
				leftValue = true;
			}
			
			if (this._lastState.DPad.Right == ButtonState.Released && this._currentState.DPad.Right == ButtonState.Pressed)
			{
				rightValue = true;
			}
			
			this._pressedDPad = new ControllerDPad(upValue, downValue, leftValue, rightValue);
			
			if (this._lastState.DPad.Up == ButtonState.Pressed && this._currentState.DPad.Up == ButtonState.Released)
			{
				upValue = true;
			}
			
			if (this._lastState.DPad.Down == ButtonState.Pressed && this._currentState.DPad.Down == ButtonState.Released)
			{
				downValue = true;
			}
			
			if (this._lastState.DPad.Left == ButtonState.Pressed && this._currentState.DPad.Left == ButtonState.Released)
			{
				leftValue = true;
			}
			
			if (this._lastState.DPad.Right == ButtonState.Pressed && this._currentState.DPad.Right == ButtonState.Released)
			{
				rightValue = true;
			}
			
			this._releasedDPad = new ControllerDPad(upValue, downValue, leftValue, rightValue);
		}
	}
}
