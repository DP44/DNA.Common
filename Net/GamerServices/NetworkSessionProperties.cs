using System;
using System.Collections;
using System.Collections.Generic;

namespace DNA.Net.GamerServices
{
	public class NetworkSessionProperties
		: IList<int?>, ICollection<int?>, IEnumerable<int?>, IEnumerable
	{
		private int?[] _properties = new int?[8];

		private IList<int?> List => 
			(IList<int?>)this._properties;

		public int Count =>
			this.List.Count;

		public int? this[int index]
		{
			get =>
				this._properties[index];
			
			set =>
				this._properties[index] = value;
		}

		public IEnumerator<int?> GetEnumerator() => 
			this.List.GetEnumerator();

		public void Set(int?[] props) => 
			props.CopyTo((Array)this._properties, 0);

		public void CopyTo(NetworkSessionProperties props) => 
			props.CopyTo(this._properties, 0);

		public int IndexOf(int? item) => 
			this.List.IndexOf(item);

		public void Insert(int index, int? item) => 
			this.List.Insert(index, item);

		public void RemoveAt(int index) => 
			this.List.RemoveAt(index);

		public void Add(int? item) => 
			this.List.Add(item);

		public void Clear() => 
			this.List.Clear();

		public bool Contains(int? item) => 
			this.List.Contains(item);

		public void CopyTo(int?[] array, int arrayIndex) => 
			this.List.CopyTo(array, arrayIndex);

		public bool IsReadOnly => 
			this.List.IsReadOnly;

		public bool Remove(int? item) =>
			this.List.Remove(item);

		IEnumerator IEnumerable.GetEnumerator() => 
			(IEnumerator)this.List.GetEnumerator();

		void ICollection<int?>.Add(int? item) => 
			this.List.Add(item);

		void ICollection<int?>.Clear() => 
			this.List.Clear();

		bool ICollection<int?>.Contains(int? item) => 
			return this.List.Contains(item);

		void ICollection<int?>.CopyTo(int?[] array, int arrayIndex) => 
			this.List.CopyTo(array, arrayIndex);

		int ICollection<int?>.Count => 
			this.List.Count;

		bool ICollection<int?>.IsReadOnly => 
			this.List.IsReadOnly;

		bool ICollection<int?>.Remove(int? item) => 
			this.List.Remove(item);

		IEnumerator<int?> IEnumerable<int?>.GetEnumerator() => 
			this.List.GetEnumerator();
	}
}
