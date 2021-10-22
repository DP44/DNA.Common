using System;
using System.Collections;
using System.Collections.Generic;

namespace DNA.Collections
{
	public class Set<T> : ICollection<T>, IEnumerable<T>, IEnumerable
	{
		private struct Link
		{
			public int HashCode;
			public int Next;
		}

		private class SetEqualityComparer : IEqualityComparer<Set<T>>
		{
			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public bool Equals(Set<T> lhs, Set<T> rhs)
			{
				if (lhs == rhs)
				{
					return true;
				}
				
				if (lhs == null || rhs == null || lhs.Count != rhs.Count)
				{
					return false;
				}
				
				foreach (T item in lhs)
				{
					if (!rhs.Contains(item))
					{
						return false;
					}
				}
				
				return true;
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public int GetHashCode(Set<T> hashset)
			{
				if (hashset == null)
				{
					return 0;
				}
				
				IEqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
				int hash = 0;
				
				// Construct the hash code.
				foreach (T obj in hashset)
				{
					hash ^= equalityComparer.GetHashCode(obj);
				}
				
				return hash;
			}
		}

		[Serializable]
		public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
		{
			private Set<T> hashset;
			private int next;
			private int stamp;
			private T current;
		
			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			internal Enumerator(Set<T> hashset) : this()
			{
				this.hashset = hashset;
				this.stamp = hashset.generation;
			}
		
			/// <summary>
			/// 
			/// </summary>
			public bool MoveNext()
			{
				this.CheckState();
				
				if (this.next < 0)
				{
					return false;
				}
				
				while (this.next < this.hashset.touched)
				{
					int nextValue = this.next++;
				
					if (this.hashset.GetLinkHashCode(nextValue) != 0)
					{
						this.current = this.hashset.slots[nextValue];
						return true;
					}
				}

				this.next = -1;

				return false;
			}
		
			/// <summary>
			/// 
			/// </summary>
			public T Current => this.current;
		
			/// <summary>
			/// 
			/// </summary>
			object IEnumerator.Current
			{
				get
				{
					this.CheckState();
					
					if (this.next <= 0)
					{
						throw new InvalidOperationException("Current is not valid");
					}
					
					return this.current;
				}
			}
		
			/// <summary>
			/// 
			/// </summary>
			void IEnumerator.Reset()
			{
				this.CheckState();
				this.next = 0;
			}
		
			/// <summary>
			/// 
			/// </summary>
			public void Dispose() =>
				this.hashset = (Set<T>)null;
		
			/// <summary>
			/// 
			/// </summary>
			private void CheckState()
			{
				if (this.hashset == null)
				{
					throw new ObjectDisposedException((string)null);
				}

				if (this.hashset.generation != this.stamp)
				{
					throw new InvalidOperationException(
						"Set have been modified while it was iterated over");
				}
			}
		}

		private static class PrimeHelper
		{
			private static readonly int[] primes_table = new int[]
			{
				11, 19, 37, 73, 109, 163, 251, 367, 557, 823, 1237, 1861, 2777, 4177,
				6247, 9371, 14057, 21089, 31627, 47431, 71143, 106721, 160073, 240101,
				360163, 540217, 810343, 1215497, 1823231, 2734867, 4102283, 6153409, 
				9230113, 13845163
			};
		
			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			private static bool TestPrime(int x)
			{
				if ((x & 1) != 0)
				{
					int xSqrt = (int)Math.Sqrt((double)x);
					
					for (int i = 3; i < xSqrt; i += 2)
					{
						if (x % i == 0)
						{
							return false;
						}
					}

					return true;
				}

				return x == 2;
			}
		
			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			private static int CalcPrime(int x)
			{
				// Iterate to the 8th Mersenne prime.
				for (int i = (x & -2) - 1; i < 2147483647; i += 2)
				{
					if (Set<T>.PrimeHelper.TestPrime(i))
					{
						return i;
					}
				}

				return x;
			}
		
			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public static int ToPrime(int x)
			{
				for (int i = 0; i < Set<T>.PrimeHelper.primes_table.Length; i++)
				{
					if (x <= Set<T>.PrimeHelper.primes_table[i])
					{
						return Set<T>.PrimeHelper.primes_table[i];
					}
				}

				return Set<T>.PrimeHelper.CalcPrime(x);
			}
		}

		private const int INITIAL_SIZE = 10;
		private const float DEFAULT_LOAD_FACTOR = 0.9f;
		private const int NO_SLOT = -1;
		private const int HASH_FLAG = -2147483648;

		private int[] table;
		private Set<T>.Link[] links;
		private T[] slots;
		private int touched;
		private int empty_slot;
		private int count;
		private int threshold;
		private IEqualityComparer<T> comparer;
		private int generation;

		private static readonly Set<T>.SetEqualityComparer setComparer = 
			new Set<T>.SetEqualityComparer();
		
		/// <summary>
		/// 
		/// </summary>
		public int Count => this.count;
		
		/// <summary>
		/// 
		/// </summary>
		public Set() =>
			this.Init(10, (IEqualityComparer<T>)null);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public Set(IEqualityComparer<T> comparer) => 
			this.Init(10, comparer);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public Set(IEnumerable<T> collection) : this(collection, null) {}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public Set(IEnumerable<T> collection, IEqualityComparer<T> comparer)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			
			int capacity = 0;
			
			if (collection is ICollection<T> objs)
			{
				capacity = objs.Count;
			}
			
			this.Init(capacity, comparer);
			
			foreach (T item in collection)
			{
				this.Add(item);
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		private void Init(int capacity, IEqualityComparer<T> comparer)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			
			this.comparer = (comparer ?? EqualityComparer<T>.Default);
			
			if (capacity == 0)
			{
				capacity = 10;
			}
			
			capacity = (int)((float)capacity / 0.9f) + 1;
			
			this.InitArrays(capacity);
			
			this.generation = 0;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		private void InitArrays(int size)
		{
			this.table = new int[size];
			this.links = new Set<T>.Link[size];
			this.empty_slot = -1;
			this.slots = new T[size];
			this.touched = 0;
			this.threshold = (int)((float)this.table.Length * 0.9f);
			
			if (this.threshold == 0 && this.table.Length > 0)
			{
				this.threshold = 1;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		private bool SlotsContainsAt(int index, int hash, T item)
		{
			Set<T>.Link link;
			
			for (int i = this.table[index] - 1; i != -1; i = link.Next)
			{
				link = this.links[i];
			
				if (link.HashCode == hash && ((hash == -2147483648 
					&& (item == null || this.slots[i] == null)) 
						? (item == null && null == this.slots[i]) 
						: this.comparer.Equals(item, this.slots[i])))
				{
					return true;
				}
			}

			return false;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void CopyTo(T[] array) => 
			this.CopyTo(array, 0, this.count);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void CopyTo(T[] array, int arrayIndex) => 
			this.CopyTo(array, arrayIndex, this.count);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void CopyTo(T[] array, int arrayIndex, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			
			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException("arrayIndex");
			}
			
			if (arrayIndex > array.Length)
			{
				throw new ArgumentException("index larger than largest valid index of array");
			}
			
			if (array.Length - arrayIndex < count)
			{
				throw new ArgumentException(
					"Destination array cannot hold the requested elements!");
			}
			
			int i = 0;
			int min = 0;
			
			while (i < this.touched && min < count)
			{
				if (this.GetLinkHashCode(i) != 0)
				{
					array[arrayIndex++] = this.slots[i];
				}

				i++;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		private void Resize()
		{
			int num = Set<T>.PrimeHelper.ToPrime(this.table.Length << 1 | 1);
			int[] array = new int[num];
			Set<T>.Link[] array2 = new Set<T>.Link[num];
			
			for (int i = 0; i < this.table.Length; i++)
			{
				for (int num2 = this.table[i] - 1; num2 != -1; num2 = this.links[num2].Next)
				{
					int num3 = array2[num2].HashCode = this.GetItemHashCode(this.slots[num2]);
					int num4 = (num3 & int.MaxValue) % num;
			
					array2[num2].Next = array[num4] - 1;
					array[num4] = num2 + 1;
				}
			}

			this.table = array;
			this.links = array2;
			
			T[] destinationArray = new T[num];
			
			Array.Copy(this.slots, 0, destinationArray, 0, this.touched);
			
			this.slots = destinationArray;
			this.threshold = (int)((float)num * 0.9f);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		private int GetLinkHashCode(int index) => 
			this.links[index].HashCode & int.MinValue;
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		private int GetItemHashCode(T item) => 
			(object)item == null 
				? int.MinValue 
				: this.comparer.GetHashCode(item) | int.MinValue;
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public bool Add(T item)
		{
			int itemHashCode = this.GetItemHashCode(item);
			int tableLen = (itemHashCode & int.MaxValue) % this.table.Length;
			
			if (this.SlotsContainsAt(tableLen, itemHashCode, item))
			{
				return false;
			}
			
			if (this.count++ > this.threshold)
			{
				this.Resize();
				tableLen = (itemHashCode & int.MaxValue) % this.table.Length;
			}
			
			int slot = this.empty_slot;
			
			if (slot == -1)
			{
				slot = this.touched++;
			}
			else
			{
				this.empty_slot = this.links[slot].Next;
			}
			
			this.links[slot].HashCode = itemHashCode;
			this.links[slot].Next = this.table[tableLen] - 1;
			this.table[tableLen] = slot + 1;
			this.slots[slot] = item;
			
			this.generation++;
			
			return true;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public IEqualityComparer<T> Comparer => 
			this.comparer;
		
		/// <summary>
		/// 
		/// </summary>
		public void Clear()
		{
			this.count = 0;
			
			Array.Clear((Array)this.table, 0, this.table.Length);
			Array.Clear((Array)this.slots, 0, this.slots.Length);
			Array.Clear((Array)this.links, 0, this.links.Length);
			
			this.empty_slot = -1;
			this.touched = 0;
			
			this.generation++;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public bool Contains(T item)
		{
			int itemHashCode = this.GetItemHashCode(item);
			
			return this.SlotsContainsAt(
				(itemHashCode & int.MaxValue) % this.table.Length, itemHashCode, item);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public bool Remove(T item)
		{
			int itemHashCode = this.GetItemHashCode(item);
			int num = (itemHashCode & int.MaxValue) % this.table.Length;
			int num2 = this.table[num] - 1;
			
			if (num2 == -1)
			{
				return false;
			}
			
			int num3 = -1;
			
			do
			{
				Set<T>.Link link = this.links[num2];
				
				if (link.HashCode == itemHashCode && ((itemHashCode == -2147483648 
					&& (item == null || this.slots[num2] == null)) 
						? (item == null && null == this.slots[num2]) 
						: this.comparer.Equals(this.slots[num2], item)))
				{
					break;
				}
				
				num3 = num2;
				num2 = link.Next;
			}
			while (num2 != -1);

			if (num2 == -1)
			{
				return false;
			}

			this.count--;

			if (num3 == -1)
			{
				this.table[num] = this.links[num2].Next + 1;
			}
			else
			{
				this.links[num3].Next = this.links[num2].Next;
			}

			this.links[num2].Next = this.empty_slot;
			this.empty_slot = num2;
			this.links[num2].HashCode = 0;
			this.slots[num2] = default(T);
			this.generation++;
			return true;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public int RemoveWhere(Predicate<T> match)
		{
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
			
			List<T> list = new List<T>();
			
			foreach (T t in this)
			{
				if (match(t))
				{
					list.Add(t);
				}
			}

			foreach (T item in list)
			{
				this.Remove(item);
			}
			
			return list.Count;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public void TrimExcess() => this.Resize();
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void IntersectWith(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			
			Set<T> other_set = this.ToSet(other);
			this.RemoveWhere((T item) => !other_set.Contains(item));
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void ExceptWith(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			
			foreach (T item in other)
			{
				this.Remove(item);
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public bool Overlaps(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			
			foreach (T item in other)
			{
				if (this.Contains(item))
				{
					return true;
				}
			}
			
			return false;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public bool SetEquals(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			
			Set<T> set = this.ToSet(other);
			
			if (this.count != set.Count)
			{
				return false;
			}
			
			foreach (T item in this)
			{
				if (!set.Contains(item))
				{
					return false;
				}
			}
			
			return true;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void SymmetricExceptWith(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			
			foreach (T item in this.ToSet(other))
			{
				if (!this.Add(item))
				{
					this.Remove(item);
				}
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		private Set<T> ToSet(IEnumerable<T> enumerable)
		{
			Set<T> set = enumerable as Set<T>;
			
			if (set == null || !this.Comparer.Equals((object)set.Comparer))
			{
				set = new Set<T>(enumerable, this.Comparer);
			}
			
			return set;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void UnionWith(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			
			foreach (T item in other)
			{
				this.Add(item);
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		private bool CheckIsSubsetOf(Set<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			
			foreach (T item in this)
			{
				if (!other.Contains(item))
				{
					return false;
				}
			}
			
			return true;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public bool IsSubsetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			
			if (this.count == 0)
			{
				return true;
			}
			
			Set<T> set = this.ToSet(other);
		
			return this.count <= set.Count && this.CheckIsSubsetOf(set);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public bool IsProperSubsetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			
			if (this.count == 0)
			{
				return true;
			}
			
			Set<T> set = this.ToSet(other);
			
			return this.count < set.Count && this.CheckIsSubsetOf(set);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		private bool CheckIsSupersetOf(Set<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			
			foreach (T item in other)
			{
				if (!this.Contains(item))
				{
					return false;
				}
			}
			
			return true;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public bool IsSupersetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			
			Set<T> set = this.ToSet(other);
			
			return this.count >= set.Count && this.CheckIsSupersetOf(set);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public bool IsProperSupersetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			
			Set<T> set = this.ToSet(other);
			
			return this.count > set.Count && this.CheckIsSupersetOf(set);
		}
		
		/// <summary>
		/// 
		/// </summary>
		public static IEqualityComparer<Set<T>> CreateSetComparer() => 
			(IEqualityComparer<Set<T>>)Set<T>.setComparer;
		
		/// <summary>
		/// 
		/// </summary>
		IEnumerator<T> IEnumerable<T>.GetEnumerator() => 
			(IEnumerator<T>)new Set<T>.Enumerator(this);
		
		/// <summary>
		/// 
		/// </summary>
		bool ICollection<T>.IsReadOnly => false;
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		void ICollection<T>.Add(T item) => 
			this.Add(item);
		
		/// <summary>
		/// 
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator() => 
			(IEnumerator)new Set<T>.Enumerator(this);
		
		/// <summary>
		/// 
		/// </summary>
		public Set<T>.Enumerator GetEnumerator() => 
			new Set<T>.Enumerator(this);
	}
}
