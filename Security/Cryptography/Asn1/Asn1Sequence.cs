using System;
using System.Collections;
using System.Collections.Generic;
using DNA.Security.Cryptography.Utilities.Collections;

namespace DNA.Security.Cryptography.Asn1
{
	public abstract class Asn1Sequence : Asn1Object, IEnumerable
	{
		private class Asn1SequenceParserImpl : Asn1SequenceParser, IAsn1Convertible
		{
			private readonly Asn1Sequence outer;

			private readonly int max;

			private int index;

			public Asn1SequenceParserImpl(Asn1Sequence outer)
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

			public Asn1Object ToAsn1Object()
			{
				return this.outer;
			}
		}

		private readonly List<Asn1Encodable> seq;

		public static Asn1Sequence GetInstance(object obj)
		{
			if (obj == null || obj is Asn1Sequence)
			{
				return (Asn1Sequence)obj;
			}
			throw new ArgumentException("Unknown object in GetInstance: " + obj.GetType().FullName, "obj");
		}

		public static Asn1Sequence GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			Asn1Object @object = obj.GetObject();
			if (explicitly)
			{
				if (!obj.IsExplicit())
				{
					throw new ArgumentException("object implicit - explicit expected.");
				}
				return (Asn1Sequence)@object;
			}
			else if (obj.IsExplicit())
			{
				if (obj is BerTaggedObject)
				{
					return new BerSequence(@object);
				}
				return new DerSequence(@object);
			}
			else
			{
				if (@object is Asn1Sequence)
				{
					return (Asn1Sequence)@object;
				}
				throw new ArgumentException("Unknown object in GetInstance: " + obj.GetType().FullName, "obj");
			}
		}

		protected internal Asn1Sequence(int capacity)
		{
			this.seq = new List<Asn1Encodable>(capacity);
		}

		public virtual IEnumerator GetEnumerator()
		{
			return this.seq.GetEnumerator();
		}

		[Obsolete("Use GetEnumerator() instead")]
		public IEnumerator GetObjects()
		{
			return this.GetEnumerator();
		}

		public virtual Asn1SequenceParser Parser
		{
			get
			{
				return new Asn1Sequence.Asn1SequenceParserImpl(this);
			}
		}

		public virtual Asn1Encodable this[int index]
		{
			get
			{
				return this.seq[index];
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
				return this.seq.Count;
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
			Asn1Sequence asn1Sequence = asn1Object as Asn1Sequence;
			if (asn1Sequence == null)
			{
				return false;
			}
			if (this.Count != asn1Sequence.Count)
			{
				return false;
			}
			IEnumerator enumerator = this.GetEnumerator();
			IEnumerator enumerator2 = asn1Sequence.GetEnumerator();
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

		protected internal void AddObject(Asn1Encodable obj)
		{
			this.seq.Add(obj);
		}

		public override string ToString()
		{
			return CollectionUtilities.ToString(this.seq);
		}
	}
}
