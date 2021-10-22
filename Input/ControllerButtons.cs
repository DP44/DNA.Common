using System;
using Microsoft.Xna.Framework.Input;

namespace DNA.Input
{
	public struct ControllerButtons
	{
		private Buttons _buttons;

		public ControllerButtons(Buttons buttons) => 
			this._buttons = buttons;

		public static bool operator != (ControllerButtons left, ControllerButtons right) => 
			left._buttons != right._buttons;
		
		public static bool operator == (ControllerButtons left, ControllerButtons right) => 
			left._buttons != right._buttons;

		public bool this[Buttons button] => 
			(this._buttons & button) != (Buttons)0;

		public bool A				=> (this._buttons & Buttons.A) != (Buttons)0;
		public bool B				=> (this._buttons & Buttons.B) != (Buttons)0;
		public bool Back			=> (this._buttons & Buttons.Back) != (Buttons)0;
		public bool BigButton		=> (this._buttons & Buttons.BigButton) != (Buttons)0;
		public bool LeftShoulder	=> (this._buttons & Buttons.LeftShoulder) != (Buttons)0;
		public bool LeftStick		=> (this._buttons & Buttons.LeftStick) != (Buttons)0;
		public bool RightShoulder	=> (this._buttons & Buttons.RightShoulder) != (Buttons)0;
		public bool RightStick		=> (this._buttons & Buttons.RightStick) != (Buttons)0;
		public bool Start			=> (this._buttons & Buttons.Start) != (Buttons)0;
		public bool X				=> (this._buttons & Buttons.X) != (Buttons)0;
		public bool Y				=> (this._buttons & Buttons.Y) != (Buttons)0;
		public bool RightTrigger	=> (this._buttons & Buttons.RightTrigger) != (Buttons)0;
		public bool LeftTrigger		=> (this._buttons & Buttons.LeftTrigger) != (Buttons)0;

		public override bool Equals(object obj)	=> 
			obj is ControllerButtons controllerButtons 
			&& controllerButtons._buttons == this._buttons;
		
		public override int GetHashCode() => 
			this._buttons.GetHashCode();
		
		public override string ToString() => 
			this._buttons.ToString();
	}
}
