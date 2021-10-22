using System;
using System.Collections;
using System.Collections.Generic;
using DNA.Security.Cryptography.Utilities.Collections;

namespace DNA.Security.Cryptography.Asn1
{
	public abstract class Asn1Set : Asn1Object, IEnumerable
	{
		private class Asn1SetParserImpl : Asn1SetParser, IAsn1Convertible
		{
			private readonly Asn1Set outer;

			private readonly int max;

			private int index;

			public Asn1SetParserImpl(Asn1Set outer)
			{
				this.outer = outer;
				this.max = outer.Count;
			}

			public IAsn1Convertible ReadObject()
			{
				if (this.index == this.max)
				{
					return null;
				}
				Asn1Encodable asn1Encodable = this.outer[this.index++];
				if (asn1Encodable is Asn1Sequence)
				{
					return ((Asn1Sequence)asn1Encodable).Parser;
				}
				if (asn1Encodable is Asn1Set)
				{
					return ((Asn1Set)asn1Encodable).Parser;
				}
				return asn1Encodable;
			}

			public virtual Asn1Object ToAsn1Object()
			{
				return this.outer;
			}
		}

		private readonly List<Asn1Encodable> _set;

		public static Asn1Set GetInstance(object obj)
		{
			if (obj == null || obj is Asn1Set)
			{
				return (Asn1Set)obj;
			}
			throw new ArgumentException("Unknown object in GetInstance: " + obj.GetType().FullName, "obj");
		}

		public static Asn1Set GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			Asn1Object @object = obj.GetObject();
			if (explicitly)
			{
				if (!obj.IsExplicit())
				{
					throw new ArgumentException("object implicit - explicit expected.");
				}
				return (Asn1Set)@object;
			}
			else
			{
				if (obj.IsExplicit())
				{
					return new DerSet(@object);
				}
				if (@object is Asn1Set)
				{
					return (Asn1Set)@object;
				}
				if (@object is Asn1Sequence)
				{
					Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
					Asn1Sequence asn1Sequence = (Asn1Sequence)@object;
					foreach (object obj2 in asn1Sequence)
					{
						Asn1Encodable asn1Encodable = (Asn1Encodable)obj2;
						asn1EncodableVector.Add(new Asn1Encodable[]
						{
							asn1Encodable
						});
					}
					return new DerSet(asn1EncodableVector, false);
				}
				throw new ArgumentException("Unknown object in GetInstance: " + obj.GetType().FullName, "obj");
			}
		}

		protected internal Asn1Set(int capacity)
		{
			this._set = new List<Asn1Encodable>(capacity);
		}

		public virtual IEnumerator GetEnumerator()
		{
			return this._set.GetEnumerator();
		}

		[Obsolete("Use GetEnumerator() instead")]
		public IEnumerator GetObjects()
		{
			return this.GetEnumerator();
		}

		public virtual Asn1Encodable this[int index]
		{
			get
			{
				return this._set[index];
			}
		}

		[Obsolete("Use 'object[index]' syntax instead")]
		public Asn1Encodable GetObjectAt(int index)
		{
			return this[index];
		}

		[Obsolete("Use 'Count' property instead")]
		public int Size
		{
			get
			{
				return this.Count;
			}
		}

		public virtual int Count
		{
			get
			{
				return this._set.Count;
			}
		}

		public Asn1SetParser Parser
		{
			get
			{
				return new Asn1Set.Asn1SetParserImpl(this);
			}
		}

		protected override int Asn1GetHashCode()
		{
			int num = this.Count;
			foreach (object obj in this)
			{
				num *= 17;
				if (obj != null)
				{
					num ^= obj.GetHashCode();
				}
			}
			return num;
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			Asn1Set asn1Set = asn1Object as Asn1Set;
			if (asn1Set == null)
			{
				return false;
			}
			if (this.Count != asn1Set.Count)
			{
				return false;
			}
			IEnumerator enumerator = this.GetEnumerator();
			IEnumerator enumerator2 = asn1Set.GetEnumerator();
			while (enumerator.MoveNext() && enumerator2.MoveNext())
			{
				Asn1Object asn1Object2 = ((Asn1Encodable)enumerator.Current).ToAsn1Object();
				if (!asn1Object2.Equals(enumerator2.Current))
				{
					return false;
				}
			}
			return true;
		}

		private bool LessThanOrEqual(byte[] a, byte[] b)
		{
			int num = Math.Min(a.Length, b.Length);
			for (int i = 0; i < num; i++)
			{
				byte b2 = a[i];
				byte b3 = b[i];
				if (b2 != b3)
				{
					return b3 > b2;
				}
			}
			return a.Length <= b.Length;
		}

		protected internal void Sort()
		{
			if (this._set.Count > 1)
			{
				bool flag = true;
				int num = this._set.Count - 1;
				while (flag)
				{
					int num2 = 0;
					int num3 = 0;
					byte[] a = this._set[0].GetEncoded();
					flag = false;
					while (num2 != num)
					{
						byte[] encoded = this._set[num2 + 1].GetEncoded();
						if (this.LessThanOrEqual(a, encoded))
						{
							a = encoded;
						}
						else
						{
							Asn1Encodable value = this._set[num2];
							this._set[num2] = this._set[num2 + 1];
							this._set[num2 + 1] = value;
							flag = true;
							num3 = num2;
						}
						num2++;
					}
					num = num3;
				}
			}
		}

		protected internal void AddObject(Asn1Encodable obj)
		{
			this._set.Add(obj);
		}

		public override string ToString()
		{
			return CollectionUtilities.ToString(this._set);
		}
	}
}
