using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Actions
{
	public class CallbackState : State
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public delegate bool StateCallBack(Entity e, CallbackState state, 
										   object data, GameTime time);

		private bool _finished;
		private object _data;

		public event CallbackState.StateCallBack Callback;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public CallbackState(CallbackState.StateCallBack callback, object data)
		{
			this.Callback += callback;
			this._data = data;
		}

		/// <summary>
		/// 
		/// </summary>
		public override bool Complete => 
			this._finished;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected override void OnTick(DNAGame game, Entity entity, GameTime deltaT)
		{
			this._finished = true;
			
			if (this.Callback != null)
			{
				this._finished = this.Callback(entity, this, this._data, deltaT);
			}
			
			base.OnTick(game, entity, deltaT);
		}
	}
}
