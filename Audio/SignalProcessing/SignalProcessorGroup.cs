using System;
using System.Collections;
using System.Collections.Generic;

namespace DNA.Audio.SignalProcessing
{
	public abstract class SignalProcessorGroup<InputDataType, InternalDataType> : SignalProcessor<InputDataType>, IList<SignalProcessor<InternalDataType>>, ICollection<SignalProcessor<InternalDataType>>, IEnumerable<SignalProcessor<InternalDataType>>, IEnumerable
	{
		private List<SignalProcessor<InternalDataType>> Processors = new List<SignalProcessor<InternalDataType>>();

		public override void OnStart()
		{
			for (int i = 0; i < this.Processors.Count; i++)
			{
				this.Processors[i].OnStart();
			}
			
			base.OnStart();
		}

		public override void OnStop()
		{
			for (int i = 0; i < this.Processors.Count; i++)
			{
				this.Processors[i].OnStop();
			}
			
			base.OnStop();
		}

		public override int? SampleRate
		{
			get
			{
				for (int i = 0; i < this.Processors.Count; i++)
				{
					int? sampleRate = this.Processors[i].SampleRate;
					
					if (sampleRate != null)
					{
						return sampleRate;
					}
				}
				
				return null;
			}
		}

		public override int? Channels
		{
			get
			{
				for (int i = 0; i < this.Processors.Count; i++)
				{
					int? channels = this.Processors[i].Channels;
					
					if (channels != null)
					{
						return channels;
					}
				}
				
				return null;
			}
		}

		public int IndexOf(SignalProcessor<InternalDataType> item)
		{
			return this.Processors.IndexOf(item);
		}

		public void Insert(int index, SignalProcessor<InternalDataType> item)
		{
			this.Processors.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			this.Processors.RemoveAt(index);
		}

		public SignalProcessor<InternalDataType> this[int index]
		{
			get
			{
				return this.Processors[index];
			}
			set
			{
				this.Processors[index] = value;
			}
		}

		public void Add(SignalProcessor<InternalDataType> item)
		{
			this.Processors.Add(item);
		}

		public void Clear()
		{
			this.Processors.Clear();
		}

		public bool Contains(SignalProcessor<InternalDataType> item)
		{
			return this.Processors.Contains(item);
		}

		public void CopyTo(SignalProcessor<InternalDataType>[] array, int arrayIndex)
		{
			this.Processors.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get
			{
				return this.Processors.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool Remove(SignalProcessor<InternalDataType> item)
		{
			return this.Processors.Remove(item);
		}

		public IEnumerator<SignalProcessor<InternalDataType>> GetEnumerator()
		{
			return this.Processors.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.Processors.GetEnumerator();
		}
	}
}
