using System;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI.Controls
{
	public class TextRegionControl : TextControl
	{
		private Size _size;

		public override Size Size
		{
			get
			{
				return this._size;
			}
			set
			{
				this._size = value;
			}
		}

		public TextRegionControl(SpriteFont font) : base(font)
		{
		}

		public TextRegionControl(string text, SpriteFont font) : base(text, font)
		{
		}

		protected override void ProcessText(string text, StringBuilder builder)
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			float num = (float)this._size.Width;
			foreach (char c in text)
			{
				stringBuilder.Append(c);
				if (char.IsSeparator(c))
				{
					if (this.Font.MeasureString(stringBuilder).X > num)
					{
						builder.Append(stringBuilder2);
						builder.Append('\n');
						stringBuilder.Remove(0, stringBuilder2.Length);
					}
					stringBuilder2.Length = 0;
					stringBuilder2.Append(stringBuilder);
				}
			}
			if (this.Font.MeasureString(stringBuilder).X > num)
			{
				builder.Append(stringBuilder2);
				builder.Append('\n');
				stringBuilder.Remove(0, stringBuilder2.Length);
			}
			builder.Append(stringBuilder);
		}
	}
}
