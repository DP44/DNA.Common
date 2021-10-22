using System;

namespace DNA.Profiling
{
	public class ProfilerSimpleStack<T> 
		where T : class, IProfilerLinkedListNode
	{
		private T _root = default(T);

		/// <summary>
		/// The stack's root.
		/// </summary>
		public T Root => this._root;

		/// <summary>
		/// Pushes a list to the stack.
		/// </summary>
		/// <param name="newList">The list to push.</param>
		public void PushList(T newList)
		{
			IProfilerLinkedListNode nodeList = 
				(IProfilerLinkedListNode)newList;
			
			while (nodeList.NextNode != null)
			{
				nodeList = nodeList.NextNode;
			}

			nodeList.NextNode = (IProfilerLinkedListNode)this._root;
			this._root = newList;
		}

		/// <summary>
		/// Pushes a node to the stack.
		/// </summary>
		/// <param name="newNode">The node to push.</param>
		public void Push(T newNode)
		{
			newNode.NextNode = (IProfilerLinkedListNode)this._root;
			this._root = newNode;
		}

		/// <summary>
		/// Pops the stack.
		/// </summary>
		public T Pop()
		{
			T root = this._root;
			
			if (this._root != null)
			{
				this._root = (this._root.NextNode as T);
			}

			return root;
		}

		/// <summary>
		/// Clears the stack.
		/// </summary>
		public void Clear() => 
			this._root = default(T);

		/// <summary>
		/// Keeps track on if the stack is empty or not.
		/// </summary>
		public bool Empty => 
			(object)this._root == null;
	}
}
