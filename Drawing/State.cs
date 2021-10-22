using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing
{
	public class State
	{
		private bool _started;

		/// <summary>
		/// 
		/// </summary>
		public bool Started => 
			this._started;

		/// <summary>
		/// 
		/// </summary>
		public virtual bool Complete => 
			this._started;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void Start(Entity entity)
		{
			this._started = true;
			this.OnStart(entity);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void End(Entity entity) =>
			this.OnEnd(entity);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void Tick(DNAGame game, Entity entity, GameTime time) =>
			this.OnTick(game, entity, time);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected virtual void OnStart(Entity entity) {}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected virtual void OnEnd(Entity entity) {}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected virtual void OnTick(DNAGame game, Entity entity, GameTime deltaT) {}
	}
}
