using System;

namespace DNA.Drawing 
{
	public abstract class Physics 
	{
		private Entity _owner;

		/// <summary>
		/// 
		/// </summary>
		public Entity Owner => 
			this._owner;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public Physics(Entity owner) => 
			this._owner = owner;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public abstract void Accelerate(TimeSpan dt);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public abstract void Move(TimeSpan dt);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public abstract void Simulate(TimeSpan dt);
	}
}
