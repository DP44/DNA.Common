using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing
{
	public class BasicPhysics : Physics
	{
		public static Vector3 Gravity = new Vector3(0f, -20f, 0f);
	
		private Vector3 _worldVelocity = Vector3.Zero;
	
		public Vector3 LocalAcceleration = Vector3.Zero;
		public Vector3 WorldAcceleration = Vector3.Zero;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name=""></param>
		public BasicPhysics(Entity owner) : base(owner) {}

		/// <summary>
		/// 
		/// </summary>
		public Vector3 WorldVelocity
		{
			get => 
				this._worldVelocity;

			set => 
				this._worldVelocity = value;
		}

		/// <summary>
		/// 
		/// </summary>
		public Vector3 LocalVelocity
		{
			get => 
				Vector3.TransformNormal(this.WorldVelocity, this.Owner.WorldToLocal);
		
			set => 
				this.WorldVelocity = Vector3.TransformNormal(value, this.Owner.LocalToWorld);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void Reflect(Vector3 normal, float elasticity) => 
			this.WorldVelocity = 
				Vector3.Reflect(this.WorldVelocity, Vector3.Normalize(normal)) * elasticity;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public override void Accelerate(TimeSpan dt)
		{
			float scaleFactor = (float)dt.TotalSeconds;
			Entity owner = base.Owner;
			Vector3 worldVelocity = this.WorldVelocity;

			Vector3 acceleration = 
				Vector3.TransformNormal(this.LocalAcceleration, owner.LocalToParent) + 
					this.WorldAcceleration;

			worldVelocity.LengthSquared();
			this.WorldVelocity += acceleration * scaleFactor;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public override void Move(TimeSpan dt)
		{
			#if DECOMPILED
				Entity owner = base.Owner;
				float scaleFactor = (float)dt.TotalSeconds;
				float num = this.WorldVelocity.Length();
				
				if (num != 0f)
				{
					owner.LocalPosition += this.WorldVelocity * scaleFactor;
				}
			#else
				Entity owner = this.Owner;
				float scaleFactor = (float)dt.TotalSeconds;
				
				if (this.WorldVelocity.Length() == 0.0f)
				{
					return;
				}

				owner.LocalPosition += this.WorldVelocity * scaleFactor;
			#endif
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public override void Simulate(TimeSpan dt) {}
	}
}
