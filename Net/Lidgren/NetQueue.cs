using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DNA.Net.Lidgren
{
	[DebuggerDisplay("Count={Count} Capacity={Capacity}")]
	public sealed class NetQueue<T>
	{
		private T[] m_items;

		private readonly object m_lock;

		private int m_size;

		private int m_head;

		public int Count =>
			this.m_size;

		public int Capacity =>
			this.m_items.Length;

		public NetQueue(int initialCapacity)
		{
			this.m_lock = new object();
			this.m_items = new T[initialCapacity];
		}

		public void Enqueue(T item)
		{
			lock (this.m_lock)
			{
				if (this.m_size == this.m_items.Length)
				{
					this.SetCapacity(this.m_items.Length + 8);
				}

				int num = (this.m_head + this.m_size) % this.m_items.Length;
				this.m_items[num] = item;
				this.m_size++;
			}
		}

		public void Enqueue(IEnumerable<T> items)
		{
			lock (this.m_lock)
			{
				foreach (T t in items)
				{
					if (this.m_size == this.m_items.Length)
					{
						this.SetCapacity(this.m_items.Length + 8);
					}

					int num = (this.m_head + this.m_size) % this.m_items.Length;
					this.m_items[num] = t;
					this.m_size++;
				}
			}
		}

		public void EnqueueFirst(T item)
		{
			lock (this.m_lock)
			{
				if (this.m_size >= this.m_items.Length)
				{
					this.SetCapacity(this.m_items.Length + 8);
				}

				this.m_head--;

				if (this.m_head < 0)
				{
					this.m_head = this.m_items.Length - 1;
				}

				this.m_items[this.m_head] = item;
				this.m_size++;
			}
		}

		private void SetCapacity(int newCapacity)
		{
			if (this.m_size == 0 && this.m_size == 0)
			{
				this.m_items = new T[newCapacity];
				this.m_head = 0;
				return;
			}

			T[] array = new T[newCapacity];
		
			if (this.m_head + this.m_size - 1 < this.m_items.Length)
			{
				Array.Copy(this.m_items, this.m_head, array, 0, this.m_size);
			}
			else
			{
				Array.Copy(this.m_items, this.m_head, array, 0, 
						   this.m_items.Length - this.m_head);
				
				Array.Copy(this.m_items, 0, array, this.m_items.Length - this.m_head, 
						   this.m_size - (this.m_items.Length - this.m_head));
			}
			
			this.m_items = array;
			this.m_head = 0;
		}

		public bool TryDequeue(out T item)
		{
			if (this.m_size == 0)
			{
				item = default(T);
				return false;
			}
			
			lock (this.m_lock)
			{
				if (this.m_size == 0)
				{
					item = default(T);
					return false;
				}
				else
				{
					item = this.m_items[this.m_head];
					this.m_items[this.m_head] = default(T);
					this.m_head = (this.m_head + 1) % this.m_items.Length;
					this.m_size--;
					return true;
				}
			}
		}

		public int TryDrain(IList<T> addTo)
		{
			if (this.m_size == 0)
			{
				return 0;
			}

			lock (this.m_lock)
			{
				int size = this.m_size;
				
				while (this.m_size > 0)
				{
					T item = this.m_items[this.m_head];
					addTo.Add(item);
					this.m_items[this.m_head] = default(T);
					this.m_head = (this.m_head + 1) % this.m_items.Length;
					this.m_size--;
				}
				
				return size;
			}
		}

		public T TryPeek(int offset)
		{
			if (this.m_size == 0)
			{
				return default(T);
			}

			lock (this.m_lock)
			{
				if (this.m_size == 0)
				{
					return default(T);
				}
				else
				{
					return this.m_items[(this.m_head + offset) % this.m_items.Length];
				}
			}
		}

		public bool Contains(T item)
		{
			lock (this.m_lock)
			{
				int num = this.m_head;
				
				for (int i = 0; i < this.m_size; i++)
				{
					if (this.m_items[num] == null)
					{
						if (item == null)
						{
							return true;
						}
					}
					else if (this.m_items[num].Equals(item))
					{
						return true;
					}
				
					num = (num + 1) % this.m_items.Length;
				}
			}
			
			return false;
		}

		public T[] ToArray()
		{
			T[] result;
			
			lock (this.m_lock)
			{
				T[] array = new T[this.m_size];
				int num = this.m_head;
			
				for (int i = 0; i < this.m_size; i++)
				{
					array[i] = this.m_items[num++];
			
					if (num >= this.m_items.Length)
					{
						num = 0;
					}
				}
			
				result = array;
			}
			
			return result;
		}

		public void Clear()
		{
			lock (this.m_lock)
			{
				for (int i = 0; i < this.m_items.Length; i++)
				{
					this.m_items[i] = default(T);
				}
				
				this.m_head = 0;
				this.m_size = 0;
			}
		}
	}
}
