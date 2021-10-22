using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Actions
{
	public class WaitAction : State
	{
		private TimeSpan _endTime;
		private TimeSpan _elapsedTime;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public WaitAction(TimeSpan time) => 
			this._endTime = time;

		/// <summary>
		/// 
		/// </summary>
		public override bool Complete => 
			this._elapsedTime > this._endTime;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected override void OnTick(DNAGame game, Entity actor, GameTime deltaT)
		{
			this._elapsedTime += deltaT.ElapsedGameTime;
			base.OnTick(game, actor, deltaT);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected override void OnStart(Entity actor) => 
			this._elapsedTime = TimeSpan.Zero;
	}
}
