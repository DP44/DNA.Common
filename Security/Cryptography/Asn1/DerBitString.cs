using System;
using System.Text;
using DNA.Security.Cryptography.Utilities;

namespace DNA.Security.Cryptography.Asn1
{
	public class DerBitString : DerStringBase
	{
		private static readonly char[] table = new char[]
		{
			'0',
			'1',
			'2',
			'3',
			'4',
			'5',
			'6',
			'7',
			'8',
			'9',
			'A',
			'B',
			'C',
			'D',
			'E',
			'F'
		};

		private readonly byte[] data;

		private readonly int padBits;

		internal static int GetPadBits(int bitString)
		{
			int num = 0;
			for (int i = 3; i >= 0; i--)
			{
				if (i != 0)
				{
					if (bitString >> i * 8 != 0)
					{
						num = (bitString >> i * 8 & 255);
						break;
					}
				}
				else if (bitString != 0)
				{
					num = (bitString & 255);
					break;
				}
			}
			if (num == 0)
			{
				return 7;
			}
			int num2 = 1;
			while (((num <<= 1) & 255) != 0)
			{
				num2++;
			}
			return 8 - num2;
		}

		internal static byte[] GetBytes(int bitString)
		{
			int num = 4;
			int num2 = 3;
			while (num2 >= 1 && (bitString & 255 << num2 * 8) == 0)
			{
				num--;
				num2--;
			}
			byte[] array = new byte[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = (byte)(bitString >> i * 8 & 255);
			}
			return array;
		}

		public static DerBitString GetInstance(object obj)
		{
			if (obj == null || obj is DerBitString)
			{
				return (DerBitString)obj;
			}
			if (obj is Asn1OctetString)
			{
				byte[] octets = ((Asn1OctetString)obj).GetOctets();
				int num = (int)octets[0];
				byte[] destinationArray = new byte[octets.Length - 1];
				Array.Copy(octets, 1, destinationArray, 0, octets.Length - 1);
				return new DerBitString(destinationArray, num);
			}
			if (obj is Asn1TaggedObject)
			{
				return DerBitString.GetInstance(((Asn1TaggedObject)obj).GetObject());
			}
			throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name);
		}

		public static DerBitString GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return DerBitString.GetInstance(obj.GetObject());
		}

		internal DerBitString(byte data, int padBits)
		{
			this.data = new byte[]
			{
				data
			};
			this.padBits = padBits;
		}

		public DerBitString(byte[] data, int padBits)
		{
			this.data = data;
			this.padBits = padBits;
		}

		public DerBitString(byte[] data)
		{
			this.data = data;
		}

		public DerBitString(Asn1Encodable obj)
		{
			this.data = obj.GetDerEncoded();
		}

		public byte[] GetBytes()
		{
			return this.data;
		}

		public int PadBits
		{
			get
			{
				return this.padBits;
			}
		}

		public int IntValue
		{
			get
			{
				int num = 0;
				int num2 = 0;
				while (num2 != this.data.Length && num2 != 4)
				{
					num |= (int)(this.data[num2] & byte.MaxValue) << 8 * num2;
					num2++;
				}
				return num;
			}
		}

		internal override void Encode(DerOutputStream derOut)
		{
			byte[] array = new byte[this.GetBytes().Length + 1];
			array[0] = (byte)this.PadBits;
			Array.Copy(this.GetBytes(), 0, array, 1, array.Length - 1);
			derOut.WriteEncoded(3, array);
		}

		protected override int Asn1GetHashCode()
		{
			return this.padBits.GetHashCode() ^ Arrays.GetHashCode(this.data);
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerBitString derBitString = asn1Object as DerBitString;
			return derBitString != null && this.padBits == derBitString.padBits && Arrays.AreEqual(this.data, derBitString.data);
		}

		public override string GetString()
		{
			StringBuilder stringBuilder = new StringBuilder("#");
			byte[] derEncoded = base.GetDerEncoded();
			for (int num = 0; num != derEncoded.Length; num++)
			{
				uint num2 = (uint)derEncoded[num];
				stringBuilder.Append(DerBitString.table[(int)((UIntPtr)(num2 >> 4 & 15U))]);
				stringBuilder.Append(DerBitString.table[(int)(derEncoded[num] & 15)]);
			}
			return stringBuilder.ToString();
		}
	}
}
