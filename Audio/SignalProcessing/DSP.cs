using System;
using System.Diagnostics;
using System.Threading;

namespace DNA.Audio.SignalProcessing
{
	public class DSP
	{
		public RawProcessorGroup Processors = new RawProcessorGroup();
		private Thread ProcessingThread;
		private RawPCMData _processBuffer;
		private int _windowSize = 1024;
		public float CPULoad;
		private bool _running;
		private Stopwatch stopWatch = new Stopwatch();

		public int SampleRate
		{
			get
			{
				return this.Processors.SampleRate.Value;
			}
		}

		private int WindowSize
		{
			get
			{
				return this._windowSize;
			}
		}

		public bool Running
		{
			get
			{
				return this._running;
			}
		}

		public DSP()
		{
			this._processBuffer = null;
		}

		public void Start()
		{
			if (this._running || this.ProcessingThread != null)
			{
				throw new Exception("DSP Already Running");
			}
			
			if (this._processBuffer == null || this._processBuffer.Channels != this.Processors.Channels || this._processBuffer.Samples != this.WindowSize || this._processBuffer.SampleRate != this.SampleRate)
			{
				this._processBuffer = new RawPCMData(this.Processors.Channels.Value, this.WindowSize, this.SampleRate, 16);
			}
			
			this._running = true;
			this.ProcessingThread = new Thread(new ThreadStart(this.ProcessThread));
			this.Processors.OnStart();
			this.ProcessingThread.Name = "DSP Processing Thread";
			this.ProcessingThread.Start();
		}

		public void Stop()
		{
			if (!this.Running)
			{
				throw new Exception("Dsp not runnning");
			}
			
			this.Processors.OnStop();
			
			lock (this)
			{
				if (this.ProcessingThread != null)
				{
					if (this.ProcessingThread.IsAlive)
					{
						this.ProcessingThread.Join();
					}
					
					this.ProcessingThread = null;
				}
			}
		}

		public void ProcessThread()
		{
			try
			{
				for (;;)
				{
					this.stopWatch.Reset();
					this.stopWatch.Start();
					
					if (!this.Processors.ProcessBlock(this._processBuffer))
					{
						break;
					}
					
					this.stopWatch.Stop();
					TimeSpan elapsed = this.stopWatch.Elapsed;
					TimeSpan timeSpan = TimeSpan.FromSeconds((double)((float)this.WindowSize / (float)this.SampleRate));
					this.CPULoad = (float)(elapsed.TotalMilliseconds / timeSpan.TotalMilliseconds);
				}
			}
			catch (Exception)
			{
				// Swallow exceptions.
			}
			finally
			{
				this._running = false;
			}
		}
	}
}
