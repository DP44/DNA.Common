using System;

namespace DNA.Audio.SignalProcessing
{
	public abstract class InPlaceProcessorGroup<InputDataType, InternalDataType> : SignalProcessorGroup<InputDataType, InternalDataType>
	{
		private InternalDataType _internalDataBuffer = default(InternalDataType);

		protected abstract InternalDataType GetInternalBuffer(InputDataType sourceData);
		protected abstract void ConvertFrom(InputDataType inputData, InternalDataType internalData);
		protected abstract void ConvertTo(InternalDataType internalData, InputDataType outputData);

		public override bool ProcessBlock(InputDataType data)
		{
			if (this._internalDataBuffer == null)
			{
				this._internalDataBuffer = this.GetInternalBuffer(data);
			}
			
			this.ConvertFrom(data, this._internalDataBuffer);
			
			for (int i = 0; i < base.Count; i++)
			{
				SignalProcessor<InternalDataType> processor = base[i];
				
				if (processor.Active && !processor.ProcessBlock(this._internalDataBuffer))
				{
					return false;
				}
			}

			this.ConvertTo(this._internalDataBuffer, data);
			
			return true;
		}
	}
}
