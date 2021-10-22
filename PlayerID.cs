using System;
using System.IO;

namespace DNA 
{
	public class PlayerID : IComparable<PlayerID>, IEquatable<PlayerID> 
	{
		private byte[] _playerHash;

		public static readonly PlayerID Null = new PlayerID((byte[])null);

		public PlayerID(byte[] hash) => 
			this._playerHash = hash;

		public void Read(BinaryReader reader) 
		{
			int length = reader.ReadInt32();
			this._playerHash = new byte[length];

			for (int i = 0; i < length; i++)
			{
				this._playerHash[i] = reader.ReadByte();
			}
		}

		public void Write(BinaryWriter writer) 
		{
			writer.Write(this._playerHash.Length);

			for (int i = 0; i < this._playerHash.Length; i++) 
			{
				writer.Write(this._playerHash[i]);
			}
		}

		public byte[] Data => this._playerHash;

		public override int GetHashCode() 
		{
			int hashCode = 0;
			
			for (int i = 0; i < this._playerHash.Length; i++) 
			{
				hashCode ^= (int)this._playerHash[i];
			}

			return hashCode;
		}

		public static bool operator == (PlayerID a, PlayerID b) => a.Equals(b);
		public static bool operator != (PlayerID a, PlayerID b) => !a.Equals(b);

		public override bool Equals(object obj) => 
			this.CompareTo((PlayerID)obj) == 0;

		public override string ToString() => 
			this._playerHash.ToString();

		public int CompareTo(PlayerID other) 
		{

			if (base.GetType() != other.GetType() 
				|| this._playerHash.Length != other._playerHash.Length) 
			{
				return -1;
			}

			for (int i = 0; i < this._playerHash.Length; i++) 
			{
				int hashCode = (int)(this._playerHash[i] - other._playerHash[i]);
				
				if (hashCode != 0) 
				{
					return hashCode;
				}
			}

			return 0;
		}

		public bool Equals(PlayerID other) => 
			this.CompareTo(other) == 0;
	}
}
