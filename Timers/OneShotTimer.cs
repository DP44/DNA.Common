using System;

namespace DNA.Timers
{
	public class OneShotTimer : Timer
	{
		public TimeSpan MaxTime;
		public bool AutoReset;

		public float PercentComplete => 
			Math.Min(1f, (float)(this.ElaspedTime.TotalSeconds / this.MaxTime.TotalSeconds));

		public bool Expired => 
			this.ElaspedTime >= this.MaxTime;

		public OneShotTimer(TimeSpan time) => 
			this.MaxTime = time;

		protected override void OnUpdate(TimeSpan time)
		{
			base.OnUpdate(time);

			if (this.AutoReset && this.Expired)
			{
				base.Reset();
			}
		}

		public OneShotTimer(TimeSpan time, bool autoReset)
		{
			this.AutoReset = autoReset;
			this.MaxTime = time;
		}
	}
}
