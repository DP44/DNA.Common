using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Actions
{
	public class MoveToState : State
	{
		private Vector3 _startPosition;
		private Vector3 _endPosition;

		private TimeSpan _totalTime;
		private TimeSpan _currentTime;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public MoveToState(Vector3 finalLocaiton, TimeSpan time)
		{
			this._totalTime = time;
			this._endPosition = finalLocaiton;
		}

		/// <summary>
		/// 
		/// </summary>
		public override bool Complete => 
			this._currentTime >= this._totalTime;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected override void OnStart(Entity entity)
		{
			this._currentTime = TimeSpan.Zero;
			this._startPosition = entity.LocalPosition;
			base.OnStart(entity);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected override void OnTick(DNAGame game, Entity entity, GameTime deltaT)
		{
			this._currentTime += deltaT.ElapsedGameTime;
			
			if (this._currentTime > this._totalTime)
			{
				this._currentTime = this._totalTime;
			}
			
			float amount = 
				(float)((this._totalTime.TotalSeconds - this._currentTime.TotalSeconds) / 
					this._totalTime.TotalSeconds);
			
			entity.LocalPosition = Vector3.Lerp(this._endPosition, this._startPosition, amount);
			base.OnTick(game, entity, deltaT);
		}
	}
}
