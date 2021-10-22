using System;
using System.Collections;
using System.Collections.Generic;

namespace DNA.Collections
{
	public class NotifyList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
	{
		private List<T> _list = new List<T>();

		public event EventHandler Cleared;
		public event EventHandler Inserted;
		public event EventHandler Removed;
		public event EventHandler Set;
		public event EventHandler Modified;
		
		/// <summary>
		/// 
		/// </summary>
		protected virtual void OnModified() {}
		
		/// <summary>
		/// 
		/// </summary>
		protected virtual void OnClear() {}
		
		/// <summary>
		/// 
		/// </summary>
		protected virtual void OnClearComplete() {}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected virtual void OnInsert(int index, T value) {}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected virtual void OnInsertComplete(int index, T value) {}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected virtual void OnRemove(int index, T value) {}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected virtual void OnRemoveComplete(int index, T value) {}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected virtual void OnSet(int index, T oldValue, T newValue) {}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected virtual void OnSetComplete(int index, T oldValue, T newValue) {}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected virtual void OnValidate(T value) {}
		
		/// <summary>
		/// Finds the index of the item given in the list.
		/// </summary>
		/// <param name="item">The item to check.</param>
		public int IndexOf(T item) => 
			this._list.IndexOf(item);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void Insert(int index, T item)
		{
			this.OnValidate(item);
			this.OnInsert(index, item);
			
			this._list.Insert(index, item);
			
			this.OnInsertComplete(index, item);
			
			if (this.Inserted != null)
			{
				this.Inserted((object)this, (EventArgs)null);
			}
			
			if (this.Modified != null)
			{
				this.Modified((object)this, (EventArgs)null);
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void RemoveAt(int index)
		{
			T value = this._list[index];
			this.OnRemove(index, value);
			this._list.RemoveAt(index);
			this.OnRemoveComplete(index, value);
			
			if (this.Removed != null)
			{
				this.Removed((object)this, (EventArgs)null);
			}
			
			if (this.Modified != null)
			{
				this.Modified((object)this, (EventArgs)null);
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public T this[int index]
		{
			get => 
				this._list[index];

			set
			{
				this.OnValidate(value);
				T oldValue = this._list[index];
				this.OnSet(index, this._list[index], value);
				this._list[index] = value;
				this.OnSetComplete(index, oldValue, value);
				
				if (this.Set != null)
				{
					this.Set((object)this, (EventArgs)null);
				}
				
				if (this.Modified != null)
				{
					this.Modified((object)this, (EventArgs)null);
				}
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void Add(T item)
		{
			this.OnValidate(item);
			this.OnInsert(this._list.Count, item);
			this._list.Add(item);
			this.OnInsertComplete(this._list.Count - 1, item);
			
			if (this.Inserted != null)
			{
				this.Inserted((object)this, (EventArgs)null);
			}
			
			if (this.Modified != null)
			{
				this.Modified((object)this, (EventArgs)null);
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public void Clear()
		{
			this.OnClear();
			this._list.Clear();
			this.OnClearComplete();
			
			if (this.Cleared != null)
			{
				this.Cleared((object)this, (EventArgs)null);
			}
			
			if (this.Modified != null)
			{
				this.Modified((object)this, (EventArgs)null);
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public bool Contains(T item) => 
			this._list.Contains(item);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void CopyTo(T[] array, int arrayIndex) => 
			this._list.CopyTo(array, arrayIndex);
		
		/// <summary>
		/// Amount of items in the list.
		/// </summary>
		public int Count => this._list.Count;
		
		/// <summary>
		/// Is the list read only?
		/// </summary>
		public bool IsReadOnly => false; // hardcoded lol
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public bool Remove(T item)
		{
			// Get the given item's index.
			int itemIndex = this._list.IndexOf(item);
			
			// Accessing -1 of an array is not advised.
			if (itemIndex < 0)
			{
				return false;
			}
			
			this.OnRemove(itemIndex, item);
			
			this._list.RemoveAt(itemIndex);

			// Let us know that the item has been removed.
			this.OnRemoveComplete(itemIndex, item);
			
			if (this.Removed != null)
			{
				this.Removed((object)this, (EventArgs)null);
			}
			
			if (this.Modified != null)
			{
				this.Modified((object)this, (EventArgs)null);
			}
			
			return true;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public IEnumerator<T> GetEnumerator() => 
			(IEnumerator<T>)this._list.GetEnumerator();
		
		/// <summary>
		/// 
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator() => 
			(IEnumerator)this._list.GetEnumerator();
	}
}
