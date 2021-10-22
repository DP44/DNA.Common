using System;
using System.Collections.Generic;
using System.Text;
using DNA.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI.Controls
{
	public class TabControl : UIControl
	{
		public class TabPage : UIControlGroup
		{
			private Size _size;

			private bool _selectedTab;

			public override Size Size
			{
				get
				{
					return new Size(this._size.Width, this._size.Height);
				}
				set
				{
					this._size = value;
				}
			}

			public string Text { get; set; }

			public bool SelectedTab
			{
				get
				{
					return this._selectedTab;
				}
			}

			public TabPage()
			{
				this.Text = "Tab Page";
			}

			public TabPage(string text)
			{
				this.Text = text;
			}

			public virtual void OnSelected()
			{
				this._selectedTab = true;
			}

			public virtual void OnLostFocus()
			{
				this._selectedTab = false;
			}
		}

		public float Scale = 1f;

		private UIControl.ControlList<TabControl.TabPage> _tabs;

		private int _selectedTabIndex;

		private int _prevTabIndex;

		private Size _size;

		private int _hoverIndex = -1;

		private StringBuilder _drawBuilder = new StringBuilder();

		public SpriteFont Font { get; set; }

		public IList<TabControl.TabPage> Tabs
		{
			get
			{
				return this._tabs;
			}
		}

		public Color TextColor { get; set; }

		public Color TextSelectedColor { get; set; }

		public Color TextHoverColor { get; set; }

		public Color TextPressedColor { get; set; }

		public Color BarColor { get; set; }

		public Color BarSelectedColor { get; set; }

		public Color BarHoverColor { get; set; }

		public Color BarPressedColor { get; set; }

		public TabControl()
		{
			this._tabs = new UIControl.ControlList<TabControl.TabPage>(this);
			this._tabs.Modified += this._tabs_Modified;
			this.TextColor = Color.Black;
			this.TextSelectedColor = Color.Black;
			this.TextHoverColor = Color.Black;
			this.TextPressedColor = Color.Black;
			this.BarColor = Color.Black;
			this.BarSelectedColor = Color.Black;
			this.BarHoverColor = Color.Gray;
			this.BarPressedColor = Color.Black;
		}

		private void _tabs_Modified(object sender, EventArgs e)
		{
			this._selectedTabIndex = 0;
		}

		public int SelectedTabIndex
		{
			get
			{
				return this._selectedTabIndex;
			}
			set
			{
				this._selectedTabIndex = value;
			}
		}

		public TabControl.TabPage SelectedTab
		{
			get
			{
				if (this._tabs.Count == 0)
				{
					return null;
				}
				return this._tabs[this._selectedTabIndex];
			}
			set
			{
				this._selectedTabIndex = this._tabs.IndexOf(value);
			}
		}

		public override Size Size
		{
			get
			{
				return new Size((int)((float)this._size.Width * this.Scale), (int)((float)this._size.Height * this.Scale));
			}
			set
			{
				this._size = value;
			}
		}

		public override void CollectControls(List<UIControl> controlList)
		{
			base.CollectControls(controlList);
			foreach (TabControl.TabPage tabPage in this.Tabs)
			{
				foreach (UIControl uicontrol in tabPage.Children)
				{
					uicontrol.CollectControls(controlList);
				}
			}
		}

		public override bool HitTest(Point screenPoint)
		{
			Rectangle rectangle = new Rectangle(base.ScreenBounds.Left, base.ScreenBounds.Top, base.ScreenBounds.Width, (int)((float)this.Font.LineSpacing * this.Scale));
			return rectangle.Contains(screenPoint);
		}

		protected override void PreProcessInput(InputManager inputManager, GameController controller, KeyboardInput chatPad, GameTime gameTime)
		{
			if (this.SelectedTab != null)
			{
				this.SelectedTab.ProcessInput(inputManager, controller, chatPad, gameTime);
			}
			base.PreProcessInput(inputManager, controller, chatPad, gameTime);
		}

		protected override void OnInput(InputManager inputManager, GameController controller, KeyboardInput chatPad, GameTime gameTime)
		{
			if (inputManager.Mouse.LeftButtonReleased)
			{
				base.CaptureInput = false;
			}
			Rectangle screenBounds = base.ScreenBounds;
			if (this.HitTest(inputManager.Mouse.Position))
			{
				this._hoverIndex = -1;
				int num = screenBounds.Left;
				lock (this._drawBuilder)
				{
					for (int i = 0; i < this._tabs.Count; i++)
					{
						this._drawBuilder.Clear();
						this._drawBuilder.Append(" ");
						this._drawBuilder.Append(this._tabs[i].Text);
						this._drawBuilder.Append(" ");
						Vector2 vector = this.Font.MeasureString(this._drawBuilder) * this.Scale;
						Rectangle rectangle = new Rectangle(num, screenBounds.Top, (int)vector.X, (int)((float)this.Font.LineSpacing * this.Scale));
						if (rectangle.Contains(inputManager.Mouse.Position))
						{
							this._hoverIndex = i;
							break;
						}
						num += (int)vector.X;
					}
				}
				if (this._hoverIndex >= 0 && inputManager.Mouse.LeftButtonDown)
				{
					base.CaptureInput = true;
					this._selectedTabIndex = this._hoverIndex;
				}
			}
			else
			{
				this._hoverIndex = -1;
			}
			base.OnInput(inputManager, controller, chatPad, gameTime);
		}

		protected override void OnUpdate(DNAGame game, GameTime gameTime)
		{
			if (this._selectedTabIndex != this._prevTabIndex)
			{
				this._prevTabIndex = this._selectedTabIndex;
				this._tabs[this._prevTabIndex].OnLostFocus();
				this._tabs[this._selectedTabIndex].OnSelected();
			}
			for (int i = 0; i < this._tabs.Count; i++)
			{
				this._tabs[i].Update(game, gameTime);
			}
			base.OnUpdate(game, gameTime);
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			int num = (int)((float)this.Font.LineSpacing * this.Scale);
			Rectangle screenBounds = base.ScreenBounds;
			int num2 = (int)(4f * this.Scale);
			spriteBatch.Draw(UIControl.DummyTexture, new Rectangle(screenBounds.Left, screenBounds.Top + num + num2, screenBounds.Width, num2), this.BarColor);
			int num3 = screenBounds.Left;
			lock (this._drawBuilder)
			{
				for (int i = 0; i < this._tabs.Count; i++)
				{
					this._drawBuilder.Clear();
					this._drawBuilder.Append(" ");
					this._drawBuilder.Append(this._tabs[i].Text);
					this._drawBuilder.Append(" ");
					Vector2 vector = this.Font.MeasureString(this._drawBuilder) * this.Scale;
					Rectangle destinationRectangle = new Rectangle(num3, screenBounds.Top, (int)vector.X, num + num2);
					if (i == this._selectedTabIndex)
					{
						if (base.CaptureInput)
						{
							spriteBatch.Draw(UIControl.DummyTexture, destinationRectangle, this.BarPressedColor);
							spriteBatch.DrawString(this.Font, this._drawBuilder, new Vector2((float)num3, (float)screenBounds.Top), this.TextPressedColor, 0f, Vector2.Zero, this.Scale, SpriteEffects.None, 0f);
						}
						else
						{
							spriteBatch.DrawString(this.Font, this._drawBuilder, new Vector2((float)num3, (float)screenBounds.Top), this.BarSelectedColor, 0f, Vector2.Zero, this.Scale, SpriteEffects.None, 0f);
							spriteBatch.Draw(UIControl.DummyTexture, new Rectangle(num3, screenBounds.Top + num, (int)vector.X, num2), this.TextSelectedColor);
						}
						this._tabs[i].Draw(device, spriteBatch, gameTime);
					}
					else if (this._hoverIndex == i)
					{
						spriteBatch.Draw(UIControl.DummyTexture, destinationRectangle, this.BarHoverColor);
						spriteBatch.DrawString(this.Font, this._drawBuilder, new Vector2((float)num3, (float)screenBounds.Top), this.TextHoverColor, 0f, Vector2.Zero, this.Scale, SpriteEffects.None, 0f);
					}
					else
					{
						spriteBatch.DrawString(this.Font, this._drawBuilder, new Vector2((float)num3, (float)screenBounds.Top), this.TextColor, 0f, Vector2.Zero, this.Scale, SpriteEffects.None, 0f);
					}
					num3 += (int)vector.X;
				}
			}
		}
	}
}
