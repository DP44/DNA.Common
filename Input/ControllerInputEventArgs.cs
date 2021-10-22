using System;
using Microsoft.Xna.Framework;

namespace DNA.Input
{
	public class ControllerInputEventArgs : EventArgs
	{
		public MouseInput Mouse;
		public KeyboardInput Keyboard;
		public KeyboardInput Chatpad;
		public GameController Controller;
		public GameTime GameTime;
		public bool ContinueProcessing;

		public ControllerInputEventArgs() {}

		public ControllerInputEventArgs(GameController controller, GameTime time, 
										bool continueProcessing)
		{
			this.Controller = controller;
			this.GameTime = time;
			this.ContinueProcessing = continueProcessing;
		}
	}
}
