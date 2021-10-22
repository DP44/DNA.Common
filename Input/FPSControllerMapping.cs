using System;
using Microsoft.Xna.Framework;

namespace DNA.Input
{
	public class FPSControllerMapping : ControllerMapping
	{
		public enum FPSControllerFunction
		{
			Fire,
			Jump,
			MoveForward,
			MoveBackward,
			StrafeLeft,
			StrafeRight,
			Sprint
		}

		public bool InvertY;
		public float Sensitivity = 1f;

		public Vector2 Movement;
		public Vector2 Aiming;

		public Trigger Fire;
		public Trigger Jump;
		public Trigger MoveForward;
		public Trigger MoveBackward;
		public Trigger StrafeLeft;
		public Trigger StrafeRight;
		public Trigger Sprint;

		public virtual void SetToDefault()
		{
			base.Binding.Clear();
			
			base.Binding.Bind(6, InputBinding.Bindable.KeyLShift, InputBinding.Bindable.None, 
								 InputBinding.Bindable.ButtonLStick);
			
			base.Binding.Bind(0, InputBinding.Bindable.MouseButtonLeft, 
								 InputBinding.Bindable.None, 
								 InputBinding.Bindable.ButtonRTrigger);
			
			base.Binding.Bind(1, InputBinding.Bindable.KeySpace, InputBinding.Bindable.None, 
								 InputBinding.Bindable.ButtonA);

			base.Binding.Bind(4, InputBinding.Slot.KeyMouse1, InputBinding.Bindable.KeyA);
			base.Binding.Bind(5, InputBinding.Slot.KeyMouse1, InputBinding.Bindable.KeyD);
			base.Binding.Bind(2, InputBinding.Slot.KeyMouse1, InputBinding.Bindable.KeyW);
			base.Binding.Bind(3, InputBinding.Slot.KeyMouse1, InputBinding.Bindable.KeyS);
			
			base.Binding.Initialized = true;
		}

		public override void ProcessInput(KeyboardInput keyboard, MouseInput mouse, 
										  GameController controller)
		{
			if (!base.Binding.Initialized)
			{
				this.SetToDefault();
			}
			
			this.Movement = controller.CurrentState.ThumbSticks.Left;
			this.Aiming = controller.CurrentState.ThumbSticks.Right;
			
			this.Aiming.X = this.Aiming.X + mouse.DeltaPosition.X / 20f;
			this.Aiming.Y = this.Aiming.Y - mouse.DeltaPosition.Y / 20f;
			
			if (this.InvertY)
			{
				this.Aiming.Y = -this.Aiming.Y;
			}
			
			this.Aiming *= this.Sensitivity;
			
			if (this.MoveForward.Held)
			{
				this.Movement.Y = 1f;
			}
			
			if (this.MoveBackward.Held)
			{
				this.Movement.Y = -1f;
			}
			
			if (this.StrafeRight.Held)
			{
				this.Movement.X = 1f;
			}
			
			if (this.StrafeLeft.Held)
			{
				this.Movement.X = -1f;
			}

			this.Jump = base.Binding.GetFunction(1, keyboard, mouse, controller);
			this.Fire = base.Binding.GetFunction(0, keyboard, mouse, controller);
			this.MoveForward = base.Binding.GetFunction(2, keyboard, mouse, controller);
			this.MoveBackward = base.Binding.GetFunction(3, keyboard, mouse, controller);
			this.StrafeRight = base.Binding.GetFunction(5, keyboard, mouse, controller);
			this.StrafeLeft = base.Binding.GetFunction(4, keyboard, mouse, controller);
			this.Sprint = base.Binding.GetFunction(6, keyboard, mouse, controller);
		}

		public virtual void ClearAllControls()
		{
			this.Jump.Released = false;
			this.Jump.Pressed = false;
			this.Jump.Held = false;
			this.MoveForward.Released = false;
			this.MoveForward.Pressed = false;
			this.MoveForward.Held = false;
			this.MoveBackward.Released = false;
			this.MoveBackward.Pressed = false;
			this.MoveBackward.Held = false;
			this.StrafeLeft.Released = false;
			this.StrafeLeft.Pressed = false;
			this.StrafeLeft.Held = false;
			this.StrafeRight.Released = false;
			this.StrafeRight.Pressed = false;
			this.StrafeRight.Held = false;
			this.Sprint.Released = false;
			this.Sprint.Pressed = false;
			this.Sprint.Held = false;
			this.Movement = Vector2.Zero;
		}
	}
}
