using System;
using DNA.Input;
using Microsoft.Xna.Framework;

namespace DNA.Drawing
{
	public class FPSRig : Entity
	{
		public const float EyePointHeight = 1.4f;

		private Entity pitchPiviot = new Entity();
		private Entity recoilPiviot = new Entity();
		public PerspectiveCamera FPSCamera = new PerspectiveCamera();
		public Angle TorsoPitch = Angle.Zero;
		public float Speed = 5f;
		public bool InContact = true;
		public Vector3 GroundNormal = Vector3.Up;
		public float JumpImpulse = 10f;
		public float ControlSensitivity = 1f;
		public int JumpCountLimit = 1;
		protected int m_jumpCount;

		/// <summary>
		/// The physics object for the player.
		/// </summary>
		public BasicPhysics PlayerPhysics => 
			(BasicPhysics)this.Physics;

		/// <summary>
		/// How much to update the player's rotation from recoil.
		/// </summary>
		public Quaternion RecoilRotation
		{
			get => 
				this.recoilPiviot.LocalRotation;
			
			set => 
				this.recoilPiviot.LocalRotation = value;
		}

		/// <summary>
		/// If the player can jump or not.
		/// </summary>
		protected virtual bool CanJump => 
			this.InContact || this.m_jumpCount < this.JumpCountLimit;

		/// <summary>
		/// Sets the FPS controller's status on if it's making contact with anything.
		/// </summary>
		/// <param name="contact">What to set the value to.</param>
		protected virtual void SetInContact(bool contact)
		{
			this.InContact = contact;

			// If we're setting it to false we don't need to reset the jump timer.
			if (!this.InContact)
			{
				return;
			}

			this.m_jumpCount = 0;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public FPSRig()
		{
			this.FPSCamera.FieldOfView = Angle.FromDegrees(73f);
			base.Physics = new BasicPhysics(this);
			new GameTime(TimeSpan.FromSeconds(0.01), TimeSpan.FromSeconds(0.01));
			base.Children.Add(this.pitchPiviot);
			this.pitchPiviot.LocalPosition = new Vector3(0f, 1.4f, 0f);
			this.pitchPiviot.Children.Add(this.recoilPiviot);
			this.recoilPiviot.Children.Add(this.FPSCamera);
		}

		/// <summary>
		/// Handles jumping.
		/// </summary>
		public virtual void Jump()
		{
			if (!this.CanJump)
			{
				return;
			}

			float num = Vector3.Dot(this.GroundNormal, Vector3.Up);
			Vector3 worldVelocity = this.PlayerPhysics.WorldVelocity;
			worldVelocity.Y += this.JumpImpulse * num;
			this.PlayerPhysics.WorldVelocity = worldVelocity;
			this.m_jumpCount++;
		}

		/// <summary>
		/// Called every frame.
		/// </summary>
		/// <param name="gameTime">The time elapsed.</param>
		protected override void OnUpdate(GameTime gameTime)
		{
			this.pitchPiviot.LocalRotation = 
				Quaternion.CreateFromAxisAngle(Vector3.UnitX, this.TorsoPitch.Radians);
			
			base.OnUpdate(gameTime);
		}

		/// <summary>
		/// Updates the player rotation.
		/// </summary>
		/// <param name="input">The player's current input.</param>
		/// <param name="gameTime">The time elapsed.</param>
		protected virtual void UpdateRotation(FPSControllerMapping input, GameTime gameTime)
		{
			float num = (float)gameTime.ElapsedGameTime.TotalSeconds;
			
			this.TorsoPitch += Angle.FromRadians(
				3.14159274f * input.Aiming.Y * num * this.ControlSensitivity);
			
			if (this.TorsoPitch > Angle.FromDegrees(89f))
			{
				this.TorsoPitch = Angle.FromDegrees(89f);
			}
			
			if (this.TorsoPitch < Angle.FromDegrees(-89f))
			{
				this.TorsoPitch = Angle.FromDegrees(-89f);
			}
			
			base.LocalRotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, 
				-4.712389f * input.Aiming.X * num * this.ControlSensitivity);
		}

		/// <summary>
		/// Updates the player's velocity.
		/// </summary>
		/// <param name="input">The player's current input.</param>
		/// <param name="gameTime">The time elapsed.</param>
		protected virtual void UpdateVelocity(FPSControllerMapping input, GameTime gameTime)
		{
			double totalSeconds = gameTime.ElapsedGameTime.TotalSeconds;
			
			this.PlayerPhysics.LocalVelocity = new Vector3(input.Movement.X * this.Speed, 
				this.PlayerPhysics.LocalVelocity.Y, -input.Movement.Y * this.Speed);
		}

		/// <summary>
		/// Processes movement such as view rotation and player velocity.
		/// </summary>
		/// <param name="input">The player's current input.</param>
		/// <param name="gameTime">The time elapsed.</param>
		public virtual void ProcessInput(FPSControllerMapping input, GameTime gameTime)
		{
			// Call our other functions.
			this.UpdateRotation(input, gameTime);
			this.UpdateVelocity(input, gameTime);
			
			// Handle jumping.
			if (input.Jump.Pressed)
			{
				this.Jump();
			}
		}
	}
}
