using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace DNA.Audio.SignalProcessing.Processors
{
	public class OutputProcessor : BlockingSignalProcessor<RawPCMData>, IDisposable
	{
		private DynamicSoundEffectInstance _playbackBuffer;

		public bool WaitForOutput;

		private Queue<byte[]> waitingBuffers = new Queue<byte[]>();

		private Stack<byte[]> bufferRecycle = new Stack<byte[]>();

		private byte[] currentBuffer;

		private bool _disposed;

		private int BlocksReady
		{
			get
			{
				return this.waitingBuffers.Count;
			}
		}

		protected override bool IsReady
		{
			get
			{
				return this.BlocksReady < 3 || !this.WaitForOutput;
			}
		}

		private void Initalize(int sampleRate)
		{
			this._playbackBuffer = new DynamicSoundEffectInstance(sampleRate, AudioChannels.Mono);
			this._playbackBuffer.BufferNeeded += this._playbackBuffer_BufferNeeded;
			this._playbackBuffer.Play();
		}

		private void PushBuffer(RawPCMData data)
		{
			lock (this)
			{
				byte[] array;
				if (this.bufferRecycle.Count == 0)
				{
					array = new byte[data.ChannelData.Length];
				}
				else
				{
					array = this.bufferRecycle.Pop();
				}
				Buffer.BlockCopy(data.ChannelData, 0, array, 0, array.Length);
				this.waitingBuffers.Enqueue(array);
			}
		}

		private void PopBuffer()
		{
			lock (this)
			{
				if (this.currentBuffer != null)
				{
					this.bufferRecycle.Push(this.currentBuffer);
				}
				this.currentBuffer = null;
				if (this.waitingBuffers.Count > 0)
				{
					this.currentBuffer = this.waitingBuffers.Dequeue();
					this._playbackBuffer.SubmitBuffer(this.currentBuffer);
				}
			}
		}

		public override void OnStart()
		{
			if (this._playbackBuffer != null && this._playbackBuffer.State == SoundState.Paused)
			{
				this._playbackBuffer.Resume();
			}
			base.OnStart();
		}

		public override bool ProcessBlock(RawPCMData data)
		{
			this.PushBuffer(data);
			if (!base.ProcessBlock(data))
			{
				return false;
			}
			if (this._playbackBuffer == null)
			{
				this.Initalize(data.SampleRate);
			}
			if (this.currentBuffer == null)
			{
				this.PopBuffer();
			}
			return true;
		}

		private void _playbackBuffer_BufferNeeded(object sender, EventArgs e)
		{
			for (int i = 2 - this._playbackBuffer.PendingBufferCount; i > 0; i--)
			{
				this.PopBuffer();
			}
			base.SignalDataReady();
		}

		public override void OnStop()
		{
			this._playbackBuffer.Pause();
			base.SignalDataReady();
			base.OnStop();
		}

		public void Dispose()
		{
			if (this._disposed)
			{
				this._playbackBuffer.Stop();
				this._playbackBuffer.Dispose();
				this._disposed = true;
				GC.SuppressFinalize(this);
			}
		}

		~OutputProcessor()
		{
			this.Dispose();
		}
	}
}
