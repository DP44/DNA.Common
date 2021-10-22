using System;
using System.Threading;

namespace DNA.Audio.SignalProcessing.Processors
{
	public abstract class BlockingSignalProcessor<T> : SignalProcessor<T>
	{
		private AutoResetEvent _processBlockEvent = new AutoResetEvent(false);
		private bool _exiting;

		protected abstract bool IsReady { get; }

		protected void SignalDataReady()
		{
			this._processBlockEvent.Set();
		}

		public override bool ProcessBlock(T data)
		{
			while (!this.IsReady)
			{
				if (!this._processBlockEvent.WaitOne())
				{
					return false;
				}
				
				if (this._exiting)
				{
					return false;
				}
			}
			
			return true;
		}

		public override void OnStart()
		{
			this._exiting = false;
			base.OnStart();
		}

		public override void OnStop()
		{
			this._exiting = true;
			this.SignalDataReady();
			base.OnStop();
		}
	}
}
