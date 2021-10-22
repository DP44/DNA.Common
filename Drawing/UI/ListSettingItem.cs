using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI
{
	public class ListSettingItem : SettingItemElement
	{
		private int _index;

		private List<object> _items;

		public int Index
		{
			get
			{
				return this._index;
			}
			set
			{
				this._index = value;
			}
		}

		public object CurrentItem
		{
			get
			{
				return this._items[this._index];
			}
		}

		public ListSettingItem(string text, List<object> items, int defaultIndex) : base(text)
		{
			this._index = defaultIndex;
			this._items = items;
		}

		public override void OnClicked()
		{
			this._index++;
			if (this._index >= this._items.Count)
			{
				this._index = 0;
			}
			base.OnClicked();
		}

		public override void Decreased()
		{
			this._index--;
			if (this._index < 0)
			{
				this._index = 0;
			}
		}

		public override void Increased()
		{
			this._index++;
			if (this._index >= this._items.Count)
			{
				this._index = this._items.Count - 1;
			}
		}

		public override void OnDraw(DNAGame _game, GraphicsDevice device, SpriteBatch spriteBatch, SpriteFont font, Color textColor, Color outlineColor, int outlineWidth, Vector2 loc)
		{
			Rectangle titleSafeArea = device.Viewport.TitleSafeArea;
			Vector2 vector = font.MeasureString(">");
			float num = (float)(titleSafeArea.Right - titleSafeArea.Center.X - 50) - vector.X * 2f;
			float num2 = (float)(titleSafeArea.Center.X + 15);
			string text = this.CurrentItem.ToString();
			float x = num2 + vector.X + 10f + num / 2f - font.MeasureString(text).X / 2f;
			spriteBatch.DrawOutlinedText(font, "<", new Vector2(num2, loc.Y), textColor, outlineColor, outlineWidth);
			spriteBatch.DrawOutlinedText(font, ">", new Vector2((float)(titleSafeArea.Right - 15) - vector.X, loc.Y), textColor, outlineColor, outlineWidth);
			spriteBatch.DrawOutlinedText(font, text, new Vector2(x, loc.Y), textColor, outlineColor, outlineWidth);
			base.OnDraw(_game, device, spriteBatch, font, textColor, outlineColor, outlineWidth, loc);
		}
	}
}
