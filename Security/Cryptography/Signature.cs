using System;
using System.Text;

namespace DNA.Security.Cryptography
{
	public abstract class Signature : IComparable<Signature>, IEquatable<Signature>
	{
		private byte[] _data;

		public byte[] Data
		{
			get
			{
				return this._data;
			}
		}

		protected Signature(byte[] data)
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

		public static bool operator ==(Signature a, Signature b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Signature a, Signature b)
		{
			return !a.Equals(b);
		}

		public override bool Equals(object obj)
		{
			return this.CompareTo((Signature)obj) == 0;
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

		public virtual bool Verify(ISignatureProvider signer, byte[] data)
		{
			return this.Verify(signer, data, 0L, (long)data.Length);
		}

		public virtual bool Verify(ISignatureProvider signer, byte[] data, long length)
		{
			return this.Verify(signer, data, 0L, length);
		}

		public abstract bool Verify(ISignatureProvider signer, byte[] data, long start, long length);

		public virtual bool VerifyFileSignature(ISignatureProvider signer, string path)
		{
			throw new NotImplementedException();
		}

		public int CompareTo(Signature other)
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

		public bool Equals(Signature other)
		{
			return this.CompareTo(other) == 0;
		}
	}
}
