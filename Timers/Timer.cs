using System;

namespace DNA.Timers
{
	public class Timer
	{
		private TimeSpan _elapsedTime;

		/// <summary>
		/// How long the timer has been running for.
		/// </summary>
		public TimeSpan ElaspedTime => 
			this._elapsedTime;

		/// <summary>
		/// Called when the timer is updated. (IE. Every frame the timer is active.)
		/// </summary>
		protected virtual void OnUpdate(TimeSpan time) {}

		/// <summary>
		/// Updates the timer.
		/// </summary>
		public void Update(TimeSpan time)
		{
			this._elapsedTime += time;
			this.OnUpdate(time);
		}

		/// <summary>
		/// Resets the timer.
		/// </summary>
		public void Reset() => 
			this._elapsedTime = TimeSpan.Zero;
	}
}
