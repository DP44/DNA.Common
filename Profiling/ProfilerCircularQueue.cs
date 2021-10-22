using System;

namespace DNA.Profiling
{
	public class ProfilerCircularQueue<T>
	{
		private T[] _buffer;
		private int _head;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="size">The size of the queue.</param>
		public ProfilerCircularQueue(int size)
		{
			this._buffer = new T[size];
			this._head = 0;
		}

		/// <summary>
		/// Resets the queue's head.
		/// </summary>
		public void Reset() => 
			this._head = 0;

		/// <summary>
		/// Adds a value to the queue.
		/// </summary>
		/// <param name="value">The value to add.</param>
		public void Add(T value)
		{
			this._buffer[this._head] = value;
			
			if (this._head++ != this._buffer.Length)
			{
				return;
			}

			this._head = 0;
		}

		/// <summary>
		/// The queue's buffer.
		/// </summary>
		public T[] Buffer => 
			this._buffer;

		/// <summary>
		/// The queue's head.
		/// </summary>
		public int Head => 
			this._head;
	}
}
