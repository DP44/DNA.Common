using System;
using DNA.Text;

namespace DNA.Security.Cryptography.Asn1
{
	public class DerPrintableString : DerStringBase
	{
		private readonly string str;

		public static DerPrintableString GetInstance(object obj)
		{
			if (obj == null || obj is DerPrintableString)
			{
				return (DerPrintableString)obj;
			}
			if (obj is Asn1OctetString)
			{
				return new DerPrintableString(((Asn1OctetString)obj).GetOctets());
			}
			if (obj is Asn1TaggedObject)
			{
				return DerPrintableString.GetInstance(((Asn1TaggedObject)obj).GetObject());
			}
			throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name);
		}

		public static DerPrintableString GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return DerPrintableString.GetInstance(obj.GetObject());
		}

		public DerPrintableString(byte[] str) : this(ASCIIEncoder.GetString(str, 0, str.Length), false)
		{
		}

		public DerPrintableString(string str) : this(str, false)
		{
		}

		public DerPrintableString(string str, bool validate)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			if (validate && !DerPrintableString.IsPrintableString(str))
			{
				throw new ArgumentException("string contains illegal characters", "str");
			}
			this.str = str;
		}

		public override string GetString()
		{
			return this.str;
		}

		public byte[] GetOctets()
		{
			return ASCIIEncoder.GetBytes(this.str);
		}

		internal override void Encode(DerOutputStream derOut)
		{
			derOut.WriteEncoded(19, this.GetOctets());
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerPrintableString derPrintableString = asn1Object as DerPrintableString;
			return derPrintableString != null && this.str.Equals(derPrintableString.str);
		}

		public static bool IsPrintableString(string str)
		{
			int i = 0;
			while (i < str.Length)
			{
				char c = str[i];
				if (c <= '\u007f')
				{
					if (!char.IsLetterOrDigit(c))
					{
						char c2 = c;
						switch (c2)
						{
						case ' ':
						case '\'':
						case '(':
						case ')':
						case '+':
						case ',':
						case '-':
						case '.':
						case '/':
							goto IL_92;
						case '!':
						case '"':
						case '#':
						case '$':
						case '%':
						case '&':
						case '*':
							break;
						default:
							if (c2 == ':')
							{
								goto IL_92;
							}
							switch (c2)
							{
							case '=':
							case '?':
								goto IL_92;
							}
							break;
						}
						return false;
					}
					IL_92:
					i++;
					continue;
				}
				return false;
			}
			return true;
		}
	}
}
