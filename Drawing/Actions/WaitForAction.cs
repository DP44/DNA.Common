using System;

namespace DNA.Drawing.Actions
{
	public class WaitForAction : State
	{
		private State _otherAction;

		/// <summary>
		/// 
		/// </summary>
		public override bool Complete => 
			this._otherAction.Complete;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public WaitForAction(State otherAction) => 
			this._otherAction = otherAction;
	}
}
