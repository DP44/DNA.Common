using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DNA.IO
{
	[Serializable]
	public class HTFElement
	{
		private const char EscapeChar = '~';

		private string _id = "";

		private string _value = "";

		private List<HTFElement> _children = new List<HTFElement>();

		public override string ToString()
		{
			if (this._id != "")
			{
				return this._id + "=" + this._value;
			}
			return this._value;
		}

		public string ID
		{
			get
			{
				return this._id;
			}
			set
			{
				this._id = value;
			}
		}

		public string Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this._value = value;
			}
		}

		public virtual List<HTFElement> Children
		{
			get
			{
				return this._children;
			}
		}

		public bool IsID(string id)
		{
			return string.Compare(id, this._id, StringComparison.CurrentCultureIgnoreCase) == 0;
		}

		public HTFElement()
		{
		}

		public HTFElement(Stream stream)
		{
			char c;
			for (;;)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				c = (char)num;
				if (!char.IsWhiteSpace(c))
				{
					goto Block_2;
				}
			}
			throw new EndOfStreamException("No Elements Found");
			Block_2:
			if (c != '<')
			{
				throw new IOException("This is not a valid File");
			}
			this.ParseChildren(stream);
		}

		public HTFElement(string value)
		{
			this._value = value;
		}

		public HTFElement(string id, string value)
		{
			this._id = id;
			this._value = value;
		}

		private static char TranslateEscapeChar(char c)
		{
			if (c <= 'b')
			{
				if (c == '0')
				{
					return '\0';
				}
				switch (c)
				{
				case 'a':
					return '\a';
				case 'b':
					return '\b';
				}
			}
			else
			{
				if (c == 'f')
				{
					return '\f';
				}
				if (c == 'n')
				{
					return '\n';
				}
				switch (c)
				{
				case 'r':
					return '\r';
				case 't':
					return '\t';
				case 'v':
					return '\v';
				}
			}
			return c;
		}

		protected void ParseChildren(Stream stream)
		{
			int num = 0;
			int num2 = 0;
			bool flag = true;
			while (flag)
			{
				HTFElement htfelement = new HTFElement();
				flag = htfelement.Parse(stream, out num2);
				this._children.Add(htfelement);
				num += num2;
			}
		}

		private bool AddChildNode(Stream stream)
		{
			try
			{
				this.ParseChildren(stream);
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				return false;
			}
			return true;
		}

		private bool Parse(Stream stream, out int length)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			bool result = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			while (!flag)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException("Unexpected End of File");
				}
				char c = (char)num;
				if (flag3)
				{
					c = HTFElement.TranslateEscapeChar(c);
					stringBuilder.Append(c);
					flag3 = false;
				}
				else if (flag5 || !char.IsWhiteSpace(c))
				{
					if (flag4)
					{
						if (c == '"')
						{
							flag4 = false;
						}
						else if (c == '~')
						{
							flag3 = true;
						}
						else
						{
							stringBuilder.Append(c);
						}
					}
					else if (c == ',')
					{
						flag = true;
						result = true;
					}
					else if (c == '>')
					{
						flag = true;
						result = false;
					}
					else if (c == '<')
					{
						this.ParseChildren(stream);
					}
					else if (c == '~')
					{
						flag3 = true;
					}
					else if (c == '=' && !flag2)
					{
						this._id = stringBuilder.ToString().Trim();
						stringBuilder = new StringBuilder();
					}
					else if (c == '"')
					{
						flag4 = true;
					}
					else
					{
						stringBuilder.Append(c);
					}
					flag5 = true;
				}
			}
			this._value = stringBuilder.ToString().Trim();
			length = this._value.Length;
			return result;
		}

		private static char GetEscapeChar(char c)
		{
			if (c != '\0')
			{
				switch (c)
				{
				case '\a':
					return 'a';
				case '\b':
					return 'b';
				case '\n':
					return 'n';
				case '\f':
					return 'f';
				case '\r':
					return 'r';
				}
				return c;
			}
			return '0';
		}

		private static bool NeedsEscape(char c)
		{
			return c == '~' || c == '\a' || c == '\b' || c == '\f' || c == '\n' || c == '\r' || c == '\0' || c == '<' || c == '>' || c == ',' || c == '=' || c == '"';
		}

		private static string BuildEscapedString(string s)
		{
			bool flag = true;
			bool flag2 = false;
			foreach (char c in s)
			{
				if (HTFElement.NeedsEscape(c))
				{
					flag2 = true;
				}
				if (c == '"' || c == '\a' || c == '\b' || c == '\f' || c == '\n' || c == '\r' || c == '\0')
				{
					flag = false;
					break;
				}
			}
			if (flag && flag2)
			{
				return "\"" + s + "\"";
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char c2 in s)
			{
				if (HTFElement.NeedsEscape(c2))
				{
					stringBuilder.Append('~');
					stringBuilder.Append(HTFElement.GetEscapeChar(c2));
				}
				else
				{
					stringBuilder.Append(c2);
				}
			}
			return stringBuilder.ToString();
		}

		public void Save(StreamWriter writer)
		{
			this.Save(writer, 0);
		}

		private void Save(StreamWriter writer, int level)
		{
			if (this._id != "" && this._id != null)
			{
				writer.Write(HTFElement.BuildEscapedString(this._id));
				writer.Write('=');
			}
			writer.Write(HTFElement.BuildEscapedString(this._value));
			if (this.Children.Count > 0)
			{
				writer.Write('<');
				writer.WriteLine();
				for (int i = 0; i < this.Children.Count; i++)
				{
					for (int j = 0; j < level; j++)
					{
						writer.Write('\t');
					}
					this.Children[i].Save(writer, level + 1);
					if (i < this.Children.Count - 1)
					{
						writer.Write(',');
					}
					writer.WriteLine();
				}
				for (int k = 0; k < level; k++)
				{
					writer.Write('\t');
				}
				writer.Write('>');
			}
		}
	}
}
