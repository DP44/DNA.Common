using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DNA.Input
{
	public class KeyboardInput
	{
		private PlayerIndex? _playerIndex;
		private KeyboardState _lastState;
		private KeyboardState _currentState;

		public KeyboardInput() {}

		public KeyboardInput(Microsoft.Xna.Framework.PlayerIndex index) => 
			this._playerIndex = new Microsoft.Xna.Framework.PlayerIndex?(index);

		public Microsoft.Xna.Framework.PlayerIndex? PlayerIndex => 
			this._playerIndex;

		public KeyboardState CurrentState => 
			this._currentState;

		public KeyboardState LastState => 
			this._lastState;

		public bool IsKeyDown(Keys key) => 
			this._currentState.IsKeyDown(key);

		public bool WasKeyPressed(Keys key) => 
			this._currentState.IsKeyDown(key) && this._lastState.IsKeyUp(key);

		public bool WasKeyReleased(Keys key) => 
			this._currentState.IsKeyUp(key) && this._lastState.IsKeyDown(key);

		public void Update()
		{
			#if DECOMPILED
				this._lastState = this._currentState;
				
				if (this._playerIndex != null)
				{
					this._currentState = Keyboard.GetState(this._playerIndex.Value);
					return;
				}
				
				this._currentState = Keyboard.GetState();
			#else
				this._lastState = this._currentState;

				if (this._playerIndex.HasValue)
				{
					this._currentState = Keyboard.GetState(this._playerIndex.Value);
				}
				else
				{
					this._currentState = Keyboard.GetState();
				}
			#endif
		}
	}
}
