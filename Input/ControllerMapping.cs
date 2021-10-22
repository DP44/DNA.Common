using System;

namespace DNA.Input
{
	public abstract class ControllerMapping
	{
		private InputBinding _binding = new InputBinding();

		public InputBinding Binding => this._binding;

		public abstract void ProcessInput(KeyboardInput keyboard, MouseInput mouse, 
										  GameController controller);
	}
}
