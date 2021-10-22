using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI
{
	public class MenuItemElement
	{
		public object Tag;

		public string Text;

		public Color? OutlineColor;

		public int? OnlineWidth;

		public string Description = "";

		private Color? _textColor = null;

		private Color? _selectedColor = null;

		public bool Visible = true;

		private SpriteFont _font;

		public Color? TextColor
		{
			get
			{
				return this._textColor;
			}
			set
			{
				this._textColor = value;
			}
		}

		public Color? SelectedColor
		{
			get
			{
				return this._selectedColor;
			}
			set
			{
				this._selectedColor = value;
			}
		}

		public SpriteFont Font
		{
			get
			{
				return this._font;
			}
			set
			{
				this._font = value;
			}
		}

		public MenuItemElement(string text, object tag)
		{
			this.Text = text;
			this.Tag = tag;
		}

		public MenuItemElement(string text, string description, object tag)
		{
			this.Text = text;
			this.Tag = tag;
			this.Description = description;
		}

		public MenuItemElement(string text, Color textColor, Color selectedColor, object tag)
		{
			this.Text = text;
			this.Tag = tag;
			this._textColor = new Color?(textColor);
			this._selectedColor = new Color?(selectedColor);
		}

		public MenuItemElement(string text, string description, Color textColor, Color selectedColor, object tag)
		{
			this.Text = text;
			this.Tag = tag;
			this._textColor = new Color?(textColor);
			this._selectedColor = new Color?(selectedColor);
			this.Description = description;
		}

		public MenuItemElement(string text, Color textColor, Color selectedColor, SpriteFont font, object tag)
		{
			this.Text = text;
			this.Tag = tag;
			this._textColor = new Color?(textColor);
			this._selectedColor = new Color?(selectedColor);
			this.Font = font;
		}

		public MenuItemElement(string text, string description, Color textColor, Color selectedColor, SpriteFont font, object tag)
		{
			this.Text = text;
			this.Tag = tag;
			this._textColor = new Color?(textColor);
			this._selectedColor = new Color?(selectedColor);
			this.Font = font;
			this.Description = description;
		}
	}
}
