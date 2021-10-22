using System;
using System.Text;

namespace DNA.Security.Cryptography
{
	public abstract class Hash : IComparable<Hash>, IEquatable<Hash>
	{
		private byte[] _data;

		public byte[] Data
		{
			get
			{
				return this._data;
			}
		}

		protected Hash(byte[] data)
		{
			this._data = data;
		}

		public override int GetHashCode()
		{
			int num = 0;
			for (int i = 0; i < this._data.Length; i++)
			{
				num ^= (int)this._data[i];
			}
			return num;
		}

		public static bool operator ==(Hash a, Hash b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Hash a, Hash b)
		{
			return !a.Equals(b);
		}

		public override bool Equals(object obj)
		{
			return this.CompareTo((Hash)obj) == 0;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this._data.Length; i++)
			{
				string value = this._data[i].ToString("X2");
				stringBuilder.Append(value);
			}
			return stringBuilder.ToString();
		}

		public int CompareTo(Hash other)
		{
			if (base.GetType() != other.GetType())
			{
				return -1;
			}
			if (this._data.Length != other._data.Length)
			{
				return -1;
			}
			for (int i = 0; i < this._data.Length; i++)
			{
				int num = (int)(this._data[i] - other._data[i]);
				if (num != 0)
				{
					return num;
				}
			}
			return 0;
		}

		public bool Equals(Hash other)
		{
			return this.CompareTo(other) == 0;
		}
	}
}
