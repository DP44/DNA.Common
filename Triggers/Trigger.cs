using System;

namespace DNA.Triggers
{
	public abstract class Trigger
	{
		private bool _oneShot;
		private bool _triggered;
		private bool _lastState;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="oneShot">If the trigger should only activate once.</param>
		public Trigger(bool oneShot) => 
			this._oneShot = oneShot;

		/// <summary>
		/// Used to check if the trigger has been activated.
		/// </summary>
		public bool Triggered => 
			this._triggered;

		/// <summary>
		/// Returns if the trigger condition is satisfied.
		/// </summary>
		protected abstract bool IsSastisfied();

		/// <summary>
		/// Called when the trigger is activated.
		/// </summary>
		public virtual void OnTriggered() {}

		/// <summary>
		/// Resets the trigger's state.
		/// </summary>
		public virtual void Reset() => 
			this._triggered = false;

		/// <summary>
		/// Updates the trigger.
		/// </summary>
		public void Update()
		{
			if (this._oneShot && this._triggered)
			{
				return;
			}
			
			this.OnUpdate();

			bool triggerSatisfied = this.IsSastisfied();
			
			// Check if the trigger should be activated.
			if (!this._lastState && triggerSatisfied)
			{
				this._triggered = true;

				// Call our function once triggered.
				this.OnTriggered();
			}

			// Store the last state of the trigger.
			this._lastState = triggerSatisfied;
		}

		/// <summary>
		/// Called when the trigger is updated.
		/// </summary>
		protected virtual void OnUpdate() {}
	}
}
