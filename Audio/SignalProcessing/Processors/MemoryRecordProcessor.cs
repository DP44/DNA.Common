using System;
using System.IO;

namespace DNA.Audio.SignalProcessing.Processors
{
	public class MemoryRecordProcessor : SignalProcessor<RawPCMData>
	{
		private int RecordMax = 10000000;

		private bool recording;

		private MemoryStream _recordStream = new MemoryStream();

		public float RecordBufferFull
		{
			get
			{
				return (float)this._recordStream.Length / (float)this.RecordMax;
			}
		}

		public int BytesRecorded
		{
			get
			{
				return (int)this._recordStream.Position;
			}
		}

		public bool Recording
		{
			get
			{
				return this.recording;
			}
		}

		public void StartRecord()
		{
			if (this.recording)
			{
				throw new Exception("Already Recording");
			}
			this.recording = true;
			this._recordStream.Position = 0L;
			this._recordStream.SetLength(0L);
		}

		public void EndRecord()
		{
			if (!this.recording)
			{
				throw new Exception("Not Recording");
			}
			this.recording = false;
		}

		public byte[] GetData()
		{
			return this._recordStream.ToArray();
		}

		public MemoryRecordProcessor(int MaxSize)
		{
			this.RecordMax = MaxSize;
		}

		public override bool ProcessBlock(RawPCMData data)
		{
			if (this.recording && this.RecordBufferFull < 1f)
			{
				this._recordStream.Write(data.ChannelData, 0, data.ChannelData.Length);
			}
			return true;
		}
	}
}
