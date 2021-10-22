using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using DNA.Security.Cryptography.Math;

namespace DNA.Security.Cryptography.Asn1
{
	public class DerObjectIdentifier : Asn1Object
	{
		private static readonly Regex OidRegex = new Regex("\\A[0-2](\\.[0-9]+)+\\z");

		private readonly string identifier;

		public static DerObjectIdentifier GetInstance(object obj)
		{
			if (obj == null || obj is DerObjectIdentifier)
			{
				return (DerObjectIdentifier)obj;
			}
			if (obj is Asn1OctetString)
			{
				return new DerObjectIdentifier(((Asn1OctetString)obj).GetOctets());
			}
			if (obj is Asn1TaggedObject)
			{
				return DerObjectIdentifier.GetInstance(((Asn1TaggedObject)obj).GetObject());
			}
			throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name, "obj");
		}

		public static DerObjectIdentifier GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return DerObjectIdentifier.GetInstance(obj.GetObject());
		}

		public DerObjectIdentifier(string identifier)
		{
			if (identifier == null)
			{
				throw new ArgumentNullException("identifier");
			}
			if (!DerObjectIdentifier.OidRegex.IsMatch(identifier))
			{
				throw new FormatException("string " + identifier + " not an OID");
			}
			this.identifier = identifier;
		}

		public string Id
		{
			get
			{
				return this.identifier;
			}
		}

		internal DerObjectIdentifier(byte[] bytes) : this(DerObjectIdentifier.MakeOidStringFromBytes(bytes))
		{
		}

		private void WriteField(Stream outputStream, long fieldValue)
		{
			if (fieldValue >= 128L)
			{
				if (fieldValue >= 16384L)
				{
					if (fieldValue >= 2097152L)
					{
						if (fieldValue >= 268435456L)
						{
							if (fieldValue >= 34359738368L)
							{
								if (fieldValue >= 4398046511104L)
								{
									if (fieldValue >= 562949953421312L)
									{
										if (fieldValue >= 72057594037927936L)
										{
											outputStream.WriteByte((byte)(fieldValue >> 56 | 128L));
										}
										outputStream.WriteByte((byte)(fieldValue >> 49 | 128L));
									}
									outputStream.WriteByte((byte)(fieldValue >> 42 | 128L));
								}
								outputStream.WriteByte((byte)(fieldValue >> 35 | 128L));
							}
							outputStream.WriteByte((byte)(fieldValue >> 28 | 128L));
						}
						outputStream.WriteByte((byte)(fieldValue >> 21 | 128L));
					}
					outputStream.WriteByte((byte)(fieldValue >> 14 | 128L));
				}
				outputStream.WriteByte((byte)(fieldValue >> 7 | 128L));
			}
			outputStream.WriteByte((byte)(fieldValue & 127L));
		}

		private void WriteField(Stream outputStream, BigInteger fieldValue)
		{
			int num = (fieldValue.BitLength + 6) / 7;
			if (num == 0)
			{
				outputStream.WriteByte(0);
				return;
			}
			BigInteger bigInteger = fieldValue;
			byte[] array = new byte[num];
			for (int i = num - 1; i >= 0; i--)
			{
				array[i] = (byte)((bigInteger.IntValue & 127) | 128);
				bigInteger = bigInteger.ShiftRight(7);
			}
			byte[] array2 = array;
			int num2 = num - 1;
			array2[num2] &= 127;
			outputStream.Write(array, 0, array.Length);
		}

		internal override void Encode(DerOutputStream derOut)
		{
			OidTokenizer oidTokenizer = new OidTokenizer(this.identifier);
			MemoryStream memoryStream = new MemoryStream();
			DerOutputStream derOutputStream = new DerOutputStream(memoryStream);
			string text = oidTokenizer.NextToken();
			int num = int.Parse(text);
			text = oidTokenizer.NextToken();
			int num2 = int.Parse(text);
			this.WriteField(memoryStream, (long)(num * 40 + num2));
			while (oidTokenizer.HasMoreTokens)
			{
				text = oidTokenizer.NextToken();
				if (text.Length < 18)
				{
					this.WriteField(memoryStream, long.Parse(text));
				}
				else
				{
					this.WriteField(memoryStream, new BigInteger(text));
				}
			}
			derOutputStream.Close();
			derOut.WriteEncoded(6, memoryStream.ToArray());
		}

		protected override int Asn1GetHashCode()
		{
			return this.identifier.GetHashCode();
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerObjectIdentifier derObjectIdentifier = asn1Object as DerObjectIdentifier;
			return derObjectIdentifier != null && this.identifier.Equals(derObjectIdentifier.identifier);
		}

		public override string ToString()
		{
			return this.identifier;
		}

		private static string MakeOidStringFromBytes(byte[] bytes)
		{
			StringBuilder stringBuilder = new StringBuilder();
			long num = 0L;
			BigInteger bigInteger = null;
			bool flag = true;
			for (int num2 = 0; num2 != bytes.Length; num2++)
			{
				int num3 = (int)bytes[num2];
				if (num < 36028797018963968L)
				{
					num = num * 128L + (long)(num3 & 127);
					if ((num3 & 128) == 0)
					{
						if (flag)
						{
							switch ((int)num / 40)
							{
							case 0:
								stringBuilder.Append('0');
								break;
							case 1:
								stringBuilder.Append('1');
								num -= 40L;
								break;
							default:
								stringBuilder.Append('2');
								num -= 80L;
								break;
							}
							flag = false;
						}
						stringBuilder.Append('.');
						stringBuilder.Append(num);
						num = 0L;
					}
				}
				else
				{
					if (bigInteger == null)
					{
						bigInteger = BigInteger.ValueOf(num);
					}
					bigInteger = bigInteger.ShiftLeft(7);
					bigInteger = bigInteger.Or(BigInteger.ValueOf((long)(num3 & 127)));
					if ((num3 & 128) == 0)
					{
						stringBuilder.Append('.');
						stringBuilder.Append(bigInteger);
						bigInteger = null;
						num = 0L;
					}
				}
			}
			return stringBuilder.ToString();
		}
	}
}
