using System;

namespace DNA.Audio.SignalProcessing.Processors
{
	public class SourceDataProcessor : SignalProcessor<RawPCMData>
	{
		private RawPCMData _sourceData;

		private int _readPos;

		private bool _loop = true;

		public bool Loop
		{
			get
			{
				return this._loop;
			}
			set
			{
				this._loop = value;
			}
		}

		public override int? SampleRate
		{
			get
			{
				return new int?(this._sourceData.SampleRate);
			}
		}

		public override int? Channels
		{
			get
			{
				return new int?(this._sourceData.Channels);
			}
		}

		public int Position
		{
			get
			{
				return this._readPos;
			}
			set
			{
				this._readPos = value;
			}
		}

		public SourceDataProcessor(RawPCMData sourceData)
		{
			this._sourceData = sourceData;
		}

		public override bool ProcessBlock(RawPCMData data)
		{
			if (this._readPos + data.ChannelData.Length < this._sourceData.ChannelData.Length)
			{
				Buffer.BlockCopy(this._sourceData.ChannelData, this._readPos, data.ChannelData, 0, data.ChannelData.Length);
				this._readPos += data.ChannelData.Length;
			}
			else
			{
				for (int i = 0; i < data.ChannelData.Length; i++)
				{
					if (this._readPos >= this._sourceData.ChannelData.Length)
					{
						if (!this.Loop)
						{
							return false;
						}
						
						this._readPos = 0;
					}
					
					data.ChannelData[i] = this._sourceData.ChannelData[this._readPos];
					this._readPos++;
				}
			}
			
			return true;
		}

		public override void OnStart()
		{
			base.OnStart();
		}
	}
}
