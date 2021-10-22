using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DNA.Text
{
	public static class TextTools
	{
		private static Regex SplitWordsRE = new Regex("[a-zA-Z]+", RegexOptions.Compiled);

		public static int IndexOf(this StringBuilder text, char c)
		{
			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] == c)
				{
					return i;
				}
			}
			
			return -1;
		}

		public static int CountSame(this string a, int starta, string b, int startb)
		{
			int count = 0;
			
			while (count + starta < a.Length && count + startb < b.Length && a[starta + count] == b[startb + count])
			{
				count++;
			}

			return count;
		}

		public static string RemovePatternWhiteSpace(this string source)
		{
			throw new NotImplementedException();
		}

		public static string ReplaceAny(this string source, char[] chars, string newValue)
		{
			foreach (char c in chars)
			{
				source = source.Replace(c.ToString(), newValue);
			}

			return source;
		}

		public static string ReplaceAny(this string source, string[] strings, string newValue)
		{
			foreach (string oldValue in strings)
			{
				source = source.Replace(oldValue, newValue);
			}

			return source;
		}

		public static string Capitalize(this string word)
		{
			StringBuilder stringBuilder = new StringBuilder(word.ToLower());
			stringBuilder[0] = char.ToUpper(stringBuilder[0]);
			return stringBuilder.ToString();
		}

		public static string[] SplitWords(this string text)
		{
			MatchCollection matchCollection = TextTools.SplitWordsRE.Matches(text);
			string[] splitString = new string[matchCollection.Count];
			int i = 0;
			
			foreach (object obj in matchCollection)
			{
				Match match = (Match)obj;
				splitString[i++] = match.Value;
			}

			return splitString;
		}

		public static string IntsToString(int[] ints)
		{
			byte[] array = TextTools.IntsToBytes(ints);
			string @string = Encoding.UTF8.GetString(array, 0, array.Length);
			return @string.Replace("\0", "");
		}

		public static byte[] IntsToBytes(int[] ints)
		{
			byte[] bytes = new byte[ints.Length * 4];
			int j = 0;
			
			for (int i = 0; i < ints.Length; i++)
			{
				bytes[j++] = (byte)ints[i];
				bytes[j++] = (byte)(ints[i] >> 8);
				bytes[j++] = (byte)(ints[i] >> 16);
				bytes[j++] = (byte)(ints[i] >> 24);
			}
			
			return bytes;
		}

		public static int[] BytesToInts(byte[] strBytes)
		{
			List<int> ints = new List<int>();
			int iter = 0;
			int i = 0;
			int currentInt = 0;
			
			while (i < strBytes.Length)
			{
				if (iter >= 32)
				{
					ints.Add(currentInt);
					currentInt = 0;
					iter = 0;
				}

				currentInt |= (int)strBytes[i] << iter;
				i++;
				iter += 8;
			}
			
			if (iter > 0)
			{
				ints.Add(currentInt);
			}

			return ints.ToArray();
		}

		public static int[] StringToInts(string str, int maxInts)
		{
			Encoding utf = Encoding.UTF8;
			char[] array = str.ToCharArray();
			new List<int>();
			int maxIntsQuad = maxInts * 4;
			int stringLength = 0;
			
			while (utf.GetByteCount(array, 0, stringLength) < maxIntsQuad && stringLength < array.Length)
			{
				stringLength++;
			}
			
			if (utf.GetByteCount(array, 0, stringLength) > maxIntsQuad)
			{
				stringLength--;
			}
			
			byte[] bytes = Encoding.UTF8.GetBytes(array, 0, stringLength);
			return TextTools.BytesToInts(bytes);
		}

		public static int[] StringToInts(string str)
		{
			new List<int>();
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			return TextTools.BytesToInts(bytes);
		}
	}
}
