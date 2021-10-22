using System;
using Microsoft.Xna.Framework.Audio;

namespace DNA.Audio.SignalProcessing.Processors
{
	public class MicInputProcessor : BlockingSignalProcessor<RawPCMData>, IDisposable
	{
		private int _micBufferSize;

		private int _readPos;

		private int _writePos;

		private int _dataLength;

		private int _micSampleRate;

		private Microphone _microphone;

		private byte[] _micBuffer;

		private EventHandler<EventArgs> handler;

		private object readerLock = new object();

		private int _windowSize;

		private bool _disposed;

		public event EventHandler MicrophoneDisconnected;

		public override int? SampleRate
		{
			get
			{
				return new int?(this._micSampleRate);
			}
		}

		public override int? Channels
		{
			get
			{
				return new int?(1);
			}
		}

		public MicInputProcessor(Microphone mic)
		{
			this._microphone = mic;
			this._microphone.BufferDuration = TimeSpan.FromMilliseconds(100.0);
			this.handler = new EventHandler<EventArgs>(this._microphone_BufferReady);
			this._microphone.BufferReady += this.handler;
			this._micBufferSize = this._microphone.GetSampleSizeInBytes(this._microphone.BufferDuration);
			this._micBuffer = new byte[this._micBufferSize * 3];
			this._micSampleRate = this._microphone.SampleRate;
		}

		private void _microphone_BufferReady(object sender, EventArgs e)
		{
			if (this._writePos >= this._micBuffer.Length)
			{
				this._writePos = 0;
			}
			int num = this._micBufferSize;
			int num2 = this._micBuffer.Length - this._dataLength;
			if (num2 < num)
			{
				return;
			}
			int num3 = this._micBuffer.Length - this._writePos;
			if (num3 < this._micBufferSize)
			{
				try
				{
					this._microphone.GetData(this._micBuffer, this._writePos, num3);
				}
				catch (Exception)
				{
					this.DoMicDisconnect();
					return;
				}
				this._writePos += num3;
				num -= num3;
			}
			if (this._writePos >= this._micBuffer.Length)
			{
				this._writePos = 0;
			}
			try
			{
				this._microphone.GetData(this._micBuffer, this._writePos, num);
			}
			catch (NoMicrophoneConnectedException)
			{
				this.DoMicDisconnect();
				return;
			}
			this._writePos += num;
			lock (this.readerLock)
			{
				this._dataLength += this._micBufferSize;
			}
			base.SignalDataReady();
		}

		protected override bool IsReady
		{
			get
			{
				return this._dataLength >= this._windowSize;
			}
		}

		public override bool ProcessBlock(RawPCMData data)
		{
			this._windowSize = data.ChannelData.Length;
			if (!base.ProcessBlock(data))
			{
				return false;
			}
			int num = data.ChannelData.Length;
			for (int i = 0; i < num; i++)
			{
				if (this._readPos >= this._micBuffer.Length)
				{
					this._readPos = 0;
				}
				data.ChannelData[i] = this._micBuffer[this._readPos++];
			}
			lock (this.readerLock)
			{
				this._dataLength -= num;
			}
			return true;
		}

		public override void OnStart()
		{
			this._microphone.Start();
			base.OnStart();
		}

		public override void OnStop()
		{
			try
			{
				this._microphone.Stop();
			}
			catch
			{
			}
			base.OnStop();
		}

		private void DoMicDisconnect()
		{
			try
			{
				this._microphone.Start();
				return;
			}
			catch
			{
			}
			try
			{
				this._microphone.Stop();
			}
			catch
			{
			}
			if (this.MicrophoneDisconnected != null)
			{
				this.MicrophoneDisconnected(this, new EventArgs());
			}
		}

		public void Dispose()
		{
			if (this._disposed)
			{
				this._microphone.BufferReady -= this.handler;
				this._disposed = true;
				GC.SuppressFinalize(this);
			}
		}

		~MicInputProcessor()
		{
			this.Dispose();
		}
	}
}
