using System;
using System.Text;

namespace DNA.Text
{
	public static class TextConverter
	{
		private static char Base32IndexToChar(int index)
		{
			char c;
			
			if (index < 10)
			{
				c = (char)(48 + index);
			}
			else
			{
				c = (char)(65 + (index - 10));
			}
			
			char c2 = c;
			
			switch (c2)
			{
				case '0':
				{
					c = 'W';
					break;
				}

				case '1':
				{
					c = 'X';
					break;
				}

				default:
				{
					if (c2 != 'I')
					{
						if (c2 == 'O')
						{
							c = 'Y';
						}
					}
					else
					{
						c = 'Z';
					}

					break;
				}
			}

			return c;
		}

		private static int Base32CharToIndex(char rchar)
		{
			switch (rchar)
			{
				case 'W':
				{
					rchar = '0';
					break;
				}

				case 'X':
				{
					rchar = '1';
					break;
				}

				case 'Y':
				{
					rchar = 'O';
					break;
				}

				case 'Z':
				{
					rchar = 'I';
					break;
				}
			}

			int index;

			if (rchar >= '0' && rchar <= '9')
			{
				index = (int)(rchar - '0');
			}
			else
			{
				if (rchar < 'A' || rchar > 'Z')
				{
					throw new FormatException("charactor is out of Base32 Range");
				}

				index = (int)(rchar - 'A' + '\n');
			}

			return index;
		}

		public static string ToBase32String(byte[] bytes)
		{
			StringBuilder base32String = new StringBuilder();
			int i = 0;
			int j = 0;
			int k = 0;
			
			while (j < bytes.Length)
			{
				if (i <= 8 && j < bytes.Length)
				{
					byte b = bytes[j];
					j++;
					k |= (int)b << i;
					i += 8;
				}
			
				char value = TextConverter.Base32IndexToChar(k & 31);
				base32String.Append(value);
				k >>= 5;
				i -= 5;
			}
			
			while (i > 0)
			{
				char value2 = TextConverter.Base32IndexToChar(k & 31);
				base32String.Append(value2);
				k >>= 5;
				i -= 5;
			}
			
			return base32String.ToString();
		}

		public static byte[] FromBase32String(string str)
		{
			str = str.ToUpper();
			int num = str.Length * 5 / 8;
			byte[] byteArray = new byte[num];
			int i = 0;
			int j = 0;
			int k = 0;
			int l = 0;
			
			while (j < str.Length)
			{
				while (i < 8)
				{
					int m = TextConverter.Base32CharToIndex(str[j++]);
					k |= m << i;
					i += 5;
				}

				byte b = (byte)(k & 255);
				
				k >>= 8;
				i -= 8;

				byteArray[l++] = b;
			}

			return byteArray;
		}
	}
}
