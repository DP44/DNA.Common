using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI
{
	public class TextRegionElement : TextElement
	{
		private Vector2 _size;

		public int NumberOfLines = 1;

		public override Vector2 Size
		{
			get
			{
				return this._size;
			}
			set
			{
				if (this._size != value)
				{
					base.DirtyText();
				}
				this._size = value;
			}
		}

		public TextRegionElement(SpriteFont font) : base(font)
		{
		}

		public TextRegionElement(string text, SpriteFont font) : base(text, font)
		{
		}

		protected override void ProcessText(string text, StringBuilder builder)
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			float x = this._size.X;
			this.NumberOfLines = 1;
			float num = this.ScaleOnScreenResize ? Screen.Adjuster.ScaleFactor.Y : 1f;
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				char next = '\0';
				if (i != text.Length - 1)
				{
					next = text[i + 1];
				}
				stringBuilder.Append(c);
				if (this._isSeparator(c, next))
				{
					if (this.Font.MeasureString(stringBuilder).X * num > x)
					{
						builder.Append(stringBuilder2);
						builder.Append('\n');
						this.NumberOfLines++;
						stringBuilder.Remove(0, stringBuilder2.Length);
					}
					stringBuilder2.Length = 0;
					stringBuilder2.Append(stringBuilder);
				}
			}
			if (this.Font.MeasureString(stringBuilder).X * num > x)
			{
				builder.Append(stringBuilder2);
				builder.Append('\n');
				this.NumberOfLines++;
				stringBuilder.Remove(0, stringBuilder2.Length);
			}
			builder.Append(stringBuilder);
		}

		private bool _isSeparator(char c, char next)
		{
			if (char.IsSeparator(c))
			{
				return true;
			}
			if ((c >= '぀' && c <= 'ゕ') || (c >= '゠' && c <= 'ヿ') || (c >= '一' && c <= '龿'))
			{
				if (c <= '\\')
				{
					if (c != '$' && c != '(')
					{
						switch (c)
						{
						case '[':
						case '\\':
							break;
						default:
							goto IL_10F;
						}
					}
				}
				else if (c <= '¢')
				{
					if (c != '{' && c != '¢')
					{
						goto IL_10F;
					}
				}
				else
				{
					switch (c)
					{
					case '腥':
					case '腧':
					case '腩':
					case '腫':
					case '腭':
					case '腯':
					case '腱':
					case '腳':
					case '腵':
					case '腷':
					case '腹':
						break;
					case '腦':
					case '腨':
					case '腪':
					case '腬':
					case '腮':
					case '腰':
					case '腲':
					case '腴':
					case '腶':
					case '腸':
						goto IL_10F;
					default:
						switch (c)
						{
						case '膏':
						case '膐':
						case '膒':
							break;
						case '膑':
							goto IL_10F;
						default:
							goto IL_10F;
						}
						break;
					}
				}
				return false;
				IL_10F:
				if (next <= '腺')
				{
					if (next <= '?')
					{
						if (next <= '%')
						{
							if (next != '!' && next != '%')
							{
								return true;
							}
						}
						else if (next != ')')
						{
							switch (next)
							{
							case ',':
							case '.':
								break;
							case '-':
								return true;
							default:
								if (next != '?')
								{
									return true;
								}
								break;
							}
						}
					}
					else if (next <= '°')
					{
						if (next != ']' && next != '}')
						{
							switch (next)
							{
							case '¡':
							case '£':
							case '¤':
							case '¥':
							case '§':
							case '¨':
							case '©':
							case 'ª':
							case '«':
							case '¬':
							case '­':
							case '®':
							case '¯':
							case '°':
								break;
							case '¢':
							case '¦':
								return true;
							default:
								return true;
							}
						}
					}
					else
					{
						switch (next)
						{
						case 'Þ':
						case 'ß':
							break;
						default:
							switch (next)
							{
							case '腁':
							case '腂':
							case '腃':
							case '腄':
							case '腅':
							case '腆':
							case '腇':
							case '腈':
							case '腉':
							case '腊':
							case '腋':
							case '腒':
							case '腓':
							case '腔':
							case '腕':
							case '腘':
							case '腛':
								break;
							case '腌':
							case '腍':
							case '腎':
							case '腏':
							case '腐':
							case '腑':
							case '腖':
							case '腗':
							case '腙':
							case '腚':
								return true;
							default:
								switch (next)
								{
								case '腦':
								case '腨':
								case '腪':
								case '腬':
								case '腮':
								case '腰':
								case '腲':
								case '腴':
								case '腶':
								case '腸':
								case '腺':
									break;
								case '腧':
								case '腩':
								case '腫':
								case '腭':
								case '腯':
								case '腱':
								case '腳':
								case '腵':
								case '腷':
								case '腹':
									return true;
								default:
									return true;
								}
								break;
							}
							break;
						}
					}
				}
				else if (next <= '若')
				{
					if (next <= '臱')
					{
						switch (next)
						{
						case '膋':
						case '膌':
						case '膍':
						case '膎':
						case '膑':
						case '膓':
							break;
						case '膏':
						case '膐':
						case '膒':
							return true;
						default:
							if (next != '臱')
							{
								return true;
							}
							break;
						}
					}
					else
					{
						switch (next)
						{
						case '芟':
						case '芡':
						case '芣':
						case '芥':
						case '芧':
							break;
						case '芠':
						case '芢':
						case '芤':
						case '芦':
							return true;
						default:
							if (next != '苁')
							{
								switch (next)
								{
								case '苡':
								case '苣':
								case '若':
									break;
								case '苢':
								case '苤':
									return true;
								default:
									return true;
								}
							}
							break;
						}
					}
				}
				else if (next <= '荢')
				{
					if (next != '苬')
					{
						switch (next)
						{
						case '荀':
						case '荂':
						case '荄':
						case '荆':
						case '荈':
							break;
						case '荁':
						case '荃':
						case '荅':
						case '荇':
							return true;
						default:
							if (next != '荢')
							{
								return true;
							}
							break;
						}
					}
				}
				else
				{
					switch (next)
					{
					case '莃':
					case '莅':
					case '莇':
						break;
					case '莄':
					case '莆':
						return true;
					default:
						if (next != '莎')
						{
							switch (next)
							{
							case '莕':
							case '莖':
								break;
							default:
								return true;
							}
						}
						break;
					}
				}
				return false;
			}
			return false;
		}
	}
}
