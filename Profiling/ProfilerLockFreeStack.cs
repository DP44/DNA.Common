using System;
using System.Threading;

namespace DNA.Profiling
{
	public class ProfilerLockFreeStack<T> where T : class, IProfilerLinkedListNode
	{
		private T _root = default(T);

		/// <summary>
		/// The stack's root.
		/// </summary>
		public T Root => this._root;

		/// <summary>
		/// Pushes a node to the stack.
		/// </summary>
		/// <param name="newNode">The node to push.</param>
		public void Push(T newNode)
		{
			T t = default(T);
			
			do
			{
				t = this._root;
				newNode.NextNode = t;
			}
			while (t != Interlocked.CompareExchange<T>(ref this._root, newNode, t));
		}

		/// <summary>
		/// Pushes a list to the stack.
		/// </summary>
		/// <param name="newList">The list to push.</param>
		public void PushList(T newList)
		{
			IProfilerLinkedListNode profilerLinkedListNode = newList;
			
			while (profilerLinkedListNode.NextNode != null)
			{
				profilerLinkedListNode = profilerLinkedListNode.NextNode;
			}
			
			T t = default(T);
			
			do
			{
				t = this._root;
				profilerLinkedListNode.NextNode = t;
			}
			while (t != Interlocked.CompareExchange<T>(ref this._root, newList, t));
		}

		/// <summary>
		/// Pops the stack.
		/// </summary>
		public T Pop()
		{
			T root;
			
			do
			{
				root = this._root;
			}
			while (root != null && root != Interlocked.CompareExchange<T>(
				ref this._root, root.NextNode as T, root));
			
			return root;
		}

		/// <summary>
		/// Clears the stack.
		/// </summary>
		public T Clear()
		{
			T t = default(T);
		
			do
			{
				t = this._root;
			}
			while (t != Interlocked.CompareExchange<T>(ref this._root, default(T), t));
		
			return t;
		}

		/// <summary>
		/// Keeps track on if the stack is empty or not.
		/// </summary>
		public bool Empty => 
			(object)this._root == null;
	}
}
