using System;
using System.Collections;
using System.Collections.Generic;

namespace DNA.Collections
{
	public class Tree<T> where T : Tree<T>
	{
		public class NodeCollection 
			: IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
		{
			private Tree<T> _owner;
			private List<T> List;
		
			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public NodeCollection(Tree<T> owner)
			{
				this._owner = owner;
				this.List = new List<T>();
			}
		
			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public NodeCollection(Tree<T> owner, int size)
			{
				this._owner = owner;
				this.List = new List<T>(size);
			}
		
			/// <summary>
			/// 
			/// </summary>
			/// <param name="index"></param>
			public T this[int index]
			{
				get => 
					this.List[index];
				
				set
				{
					T oldValue = this.List[index];
					this.List[index] = value;
					
					if ((object)oldValue != (object)value)
					{
						this.OnSetComplete(index, oldValue, value);
					}
				}
			}
		
			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public void Add(T t)
			{
				this.List.Add(t);
				this.OnInsertComplete(this.List.Count - 1, t);
			}
		
			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public void AddRange(IEnumerable<T> tlist)
			{
				foreach (T obj in tlist)
				{
					this.List.Add(obj);
					this.OnInsertComplete(this.List.Count - 1, obj);
				}
			}
		
			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public int IndexOf(T t) => 
				this.List.IndexOf(t);
		
			/// <summary>
			/// 
			/// </summary>
			/// <param name="t"></param>
			public bool Contains(T t) => 
				this.List.Contains(t);
		
			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public void Insert(int index, T t)
			{
				this.List.Insert(index, t);
				this.OnInsertComplete(index, t);
			}
		
			/// <summary>
			/// 
			/// </summary>
			private void OnClear()
			{
				for (int i = 0; i < this.List.Count; i++)
				{
					T t = this.List[i];
					t._parent = default(T);
					obj.OnParentChanged((T) this._owner, default(T));
				}
			}
		
			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			private void OnInsertComplete(int index, T value)
			{
				T obj = value;
				obj._parent = (T)this._owner;
				obj.OnParentChanged(default(T), (T)this._owner);
				this._owner.OnChildrenChanged();
			}
		
			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			private void OnRemoveComplete(int index, T value)
			{
				T obj = value;
				obj._parent = default(T);
				obj.OnParentChanged((T)this._owner, default(T));
				this._owner.OnChildrenChanged();
			}
		
			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			private void OnSetComplete(int index, T oldValue, T newValue)
			{
				T oldVal = oldValue;
				T newVal = newValue;
				oldVal._parent = default(T);
				newVal._parent = (T)this._owner;
				oldVal.OnParentChanged((T)this._owner, default(T));
				newVal.OnParentChanged(default(T), (T)this._owner);
				this._owner.OnChildrenChanged();
			}
		
			/// <summary>
			/// 
			/// </summary>
			public T[] ToArray()
			{
				T[] array = new T[this.List.Count];
				this.List.CopyTo(array, 0);
				return array;
			}
		
			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public void Sort(Comparison<T> comparison) => 
				this.List.Sort(comparison);
		
			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public void RemoveAt(int index)
			{
				T obj = this.List[index];
				this.List.RemoveAt(index);
				this.OnRemoveComplete(index, obj);
			}
		
			/// <summary>
			/// Clear the collection.
			/// </summary>
			public void Clear()
			{
				this.OnClear();
				this.List.Clear();
			}
		
			/// <summary>
			/// 
			/// </summary>
			/// <param name="array"></param>
			/// <param name="arrayIndex"></param>
			public void CopyTo(T[] array, int arrayIndex) => 
				this.List.CopyTo(array, arrayIndex);
		
			/// <summary>
			/// The amount of nodes in the collection.
			/// </summary>
			public int Count => 
				this.List.Count;
		
			/// <summary>
			/// If the collection is read only or not.
			/// </summary>
			public bool IsReadOnly => false; // hardcoded lol
		
			/// <summary>
			/// Removes an item from the collection.
			/// </summary>
			/// <param name="item">The item to remove</param>
			public bool Remove(T item)
			{
				int index = this.List.IndexOf(item);
				
				// Make sure the index is not a negative number.
				if (index < 0)
				{
					return false;
				}
				
				this.RemoveAt(index);
				return true;
			}
		
			/// <summary>
			/// 
			/// </summary>
			public IEnumerator<T> GetEnumerator() => 
				(IEnumerator<T>)this.List.GetEnumerator();
		
			/// <summary>
			/// 
			/// </summary>
			IEnumerator IEnumerable.GetEnumerator() => 
				((IEnumerable)this.List).GetEnumerator();
		}

		private WeakReference<T> _parentReference = 
			new WeakReference<T>((object)null);
		
		private Tree<T>.NodeCollection _children;
		
		/// <summary>
		/// The parent node.
		/// </summary>
		private T _parent
		{
			get => 
				this._parentReference.Target;
		
			set => 
				this._parentReference.Target = value;
		}
		
		/// <summary>
		/// The root node.
		/// </summary>
		public T Root
		{
			get
			{
				T root = (T)this;
				
				while ((object)root.Parent != null)
				{
					root = root.Parent;
				}
				
				return root;
			}
		}
		
		/// <summary>
		/// Checks if the current node is a decendant of the node given.
		/// </summary>
		/// <param name="node">The node to check.</param>
		public bool IsDecendantOf(T node)
		{
			for (Tree<T> tree = this; tree != null; tree = (Tree<T>)tree.Parent)
			{
				if (tree == (object)node)
				{
					return true;
				}
			}

			return false;
		}
		
		/// <summary>
		/// Called when a parent is changed.
		/// </summary>
		/// <param name="oldParent"></param>
		/// <param name="newParent"></param>
		protected virtual void OnParentChanged(T oldParent, T newParent) {}
				
		/// <summary>
		/// 
		/// </summary>
		protected virtual void OnChildrenChanged() {}
		
		/// <summary>
		/// 
		/// </summary>
		public Tree() => 
			this._children = new Tree<T>.NodeCollection(this);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public Tree(int size) => 
			this._children = new Tree<T>.NodeCollection(this, size);
		
		/// <summary>
		/// 
		/// </summary>
		public T Parent => 
			this._parent;
		
		/// <summary>
		/// 
		/// </summary>
		public Tree<T>.NodeCollection Children => 
			this._children;
	}
}
