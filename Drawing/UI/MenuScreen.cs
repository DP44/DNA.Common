using System;
using System.Collections.Generic;
using DNA.Audio;
using DNA.Input;
using DNA.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DNA.Drawing.UI
{
	public class MenuScreen : Screen
	{
		public enum HorizontalAlignmentTypes
		{
			Left,
			Right,
			Center
		}

		public enum VerticalAlignmentTypes
		{
			Top,
			Center,
			Bottom
		}

		private bool _flashDir;

		private OneShotTimer _flashTimer = new OneShotTimer(TimeSpan.FromSeconds(0.25));

		public string SelectSound;

		public string ClickSound;

		private List<MenuItemElement> _menuItems = new List<MenuItemElement>();

		private Rectangle[] _itemLocations = new Rectangle[0];

		private int _selectedIndex;

		public MenuScreen.HorizontalAlignmentTypes HorizontalAlignment = MenuScreen.HorizontalAlignmentTypes.Center;

		public MenuScreen.VerticalAlignmentTypes VerticalAlignment = MenuScreen.VerticalAlignmentTypes.Center;

		public Rectangle? DrawArea = null;

		public SpriteFont Font;

		public Color TextColor = Color.White;

		public Color SelectedColor = Color.Red;

		public Color OutlineColor = Color.Black;

		public int OutlineWidth = 2;

		public int? LineSpacing;

		private MenuItemElement _lastSelectedItem;

		public List<MenuItemElement> MenuItems
		{
			get
			{
				return this._menuItems;
			}
		}

		public TimeSpan FlashTime
		{
			get
			{
				return this._flashTimer.MaxTime;
			}
			set
			{
				this._flashTimer.MaxTime = value;
			}
		}

		public int SelectedIndex
		{
			get
			{
				return this._selectedIndex;
			}
			set
			{
				this._selectedIndex = value;
			}
		}

		public MenuItemElement SelectedMenuItem
		{
			get
			{
				return this.MenuItems[this.SelectedIndex];
			}
		}

		public MenuItemElement AddMenuItem(string text, object tag)
		{
			MenuItemElement menuItemElement = new MenuItemElement(text, tag);
			this.MenuItems.Add(menuItemElement);
			return menuItemElement;
		}

		public MenuItemElement AddMenuItem(string text, string description, object tag)
		{
			MenuItemElement menuItemElement = new MenuItemElement(text, description, tag);
			this.MenuItems.Add(menuItemElement);
			return menuItemElement;
		}

		public MenuScreen(SpriteFont font, bool drawBehind) : base(true, drawBehind)
		{
			this.Font = font;
		}

		public MenuScreen(SpriteFont font, Color textColor, Color selectedColor, bool drawBehind) : base(true, drawBehind)
		{
			this.TextColor = textColor;
			this.SelectedColor = selectedColor;
			this.Font = font;
		}

		protected override bool OnPlayerInput(InputManager input, GameController controller, KeyboardInput chatpad, GameTime gameTime)
		{
			int num = this.HitTest(input.Mouse.Position);
			if (!this._mouseActive)
			{
				num = -1;
			}
			if (input.Mouse.DeltaPosition != Vector2.Zero)
			{
				this._mouseActive = true;
			}
			if (num >= 0)
			{
				if (this._menuItems[num].Visible)
				{
					if (this._selectedIndex != num)
					{
						if (this.SelectSound != null)
						{
							SoundManager.Instance.PlayInstance(this.SelectSound);
						}
						this._selectedIndex = num;
					}
				}
				else
				{
					num = -1;
				}
			}
			float num2 = 0.25f;
			if (controller.PressedDPad.Down || controller.PressedButtons.RightShoulder || (controller.CurrentState.ThumbSticks.Left.Y < -num2 && controller.LastState.ThumbSticks.Left.Y > -num2) || (controller.CurrentState.ThumbSticks.Right.Y < -num2 && controller.LastState.ThumbSticks.Right.Y > -num2) || input.Keyboard.WasKeyPressed(Keys.Down))
			{
				if (this.SelectSound != null)
				{
					SoundManager.Instance.PlayInstance(this.SelectSound);
				}
				this.SelectNext();
			}
			if (controller.PressedDPad.Up || controller.PressedButtons.LeftShoulder || (controller.CurrentState.ThumbSticks.Left.Y > num2 && controller.LastState.ThumbSticks.Left.Y < num2) || (controller.CurrentState.ThumbSticks.Right.Y > num2 && controller.LastState.ThumbSticks.Right.Y < num2) || input.Keyboard.WasKeyPressed(Keys.Up))
			{
				if (this.SelectSound != null)
				{
					SoundManager.Instance.PlayInstance(this.SelectSound);
				}
				this.SelectPrevious();
			}
			if (controller.PressedButtons.B || controller.PressedButtons.Back || input.Keyboard.WasKeyPressed(Keys.Escape))
			{
				if (this.ClickSound != null)
				{
					SoundManager.Instance.PlayInstance(this.ClickSound);
				}
				base.PopMe();
			}
			if (controller.PressedButtons.A || controller.PressedButtons.Start || input.Keyboard.WasKeyPressed(Keys.Enter) || (input.Mouse.LeftButtonPressed && num >= 0))
			{
				if (this.ClickSound != null)
				{
					SoundManager.Instance.PlayInstance(this.ClickSound);
				}
				this.SelectMenuItem();
			}
			return base.OnPlayerInput(input, controller, chatpad, gameTime);
		}

		public event EventHandler<SelectedMenuItemArgs> MenuItemSelected;

		protected virtual void OnMenuItemSelected(MenuItemElement selectedControl)
		{
		}

		protected virtual void OnMenuItemFocus(MenuItemElement selectedControl)
		{
		}

		private void SelectMenuItem()
		{
			if (this._selectedIndex >= 0)
			{
				MenuItemElement menuItemElement = this._menuItems[this._selectedIndex];
				this.OnMenuItemSelected(menuItemElement);
				if (this.MenuItemSelected != null)
				{
					this.MenuItemSelected(this, new SelectedMenuItemArgs(menuItemElement));
				}
			}
		}

		private void SelectFirst()
		{
			if (this._menuItems.Count == 0)
			{
				this._selectedIndex = -1;
				return;
			}
			this._selectedIndex = 0;
		}

		private void SelectNext()
		{
			if (this._menuItems.Count == 0)
			{
				this._selectedIndex = -1;
				return;
			}
			int selectedIndex = this._selectedIndex;
			for (;;)
			{
				this._selectedIndex++;
				if (this._selectedIndex >= this._menuItems.Count)
				{
					this._selectedIndex = 0;
				}
				if (this._selectedIndex == selectedIndex && !this._menuItems[this._selectedIndex].Visible)
				{
					break;
				}
				if (this._menuItems[this._selectedIndex].Visible)
				{
					return;
				}
			}
			this._selectedIndex = -1;
		}

		private void SelectPrevious()
		{
			if (this._menuItems.Count == 0)
			{
				this._selectedIndex = -1;
				return;
			}
			int selectedIndex = this._selectedIndex;
			for (;;)
			{
				this._selectedIndex--;
				if (this._selectedIndex < 0)
				{
					this._selectedIndex = this._menuItems.Count - 1;
				}
				if (this._selectedIndex == selectedIndex && !this._menuItems[this._selectedIndex].Visible)
				{
					break;
				}
				if (this._menuItems[this._selectedIndex].Visible)
				{
					return;
				}
			}
			this._selectedIndex = -1;
		}

		private Vector2 MeasureItem(MenuItemElement item)
		{
			SpriteFont spriteFont = (item.Font == null) ? this.Font : item.Font;
			return spriteFont.MeasureString(item.Text) * Screen.Adjuster.ScaleFactor.Y;
		}

		protected override void OnUpdate(DNAGame game, GameTime gameTime)
		{
			if (this._lastSelectedItem != this.SelectedMenuItem)
			{
				this._lastSelectedItem = this.SelectedMenuItem;
				this.OnMenuItemFocus(this._lastSelectedItem);
			}
			base.OnUpdate(game, gameTime);
		}

		public int HitTest(Point p)
		{
			for (int i = 0; i < this._itemLocations.Length; i++)
			{
				if (this._itemLocations[i].Contains(p) && this._menuItems[i].Visible)
				{
					return i;
				}
			}
			return -1;
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			if (this._itemLocations.Length != this._menuItems.Count)
			{
				this._itemLocations = new Rectangle[this._menuItems.Count];
			}
			this._flashTimer.Update(gameTime.ElapsedGameTime);
			if (this._flashTimer.Expired)
			{
				this._flashTimer.Reset();
				this._flashDir = !this._flashDir;
			}
			while (this._selectedIndex >= this._menuItems.Count || !this.MenuItems[this._selectedIndex].Visible)
			{
				this._selectedIndex++;
				this._selectedIndex %= this._menuItems.Count;
			}
			spriteBatch.Begin();
			float num = 0f;
			float num2 = 0f;
			if (this.LineSpacing != null)
			{
				num2 = (float)this.LineSpacing.Value * Screen.Adjuster.ScaleFactor.Y;
			}
			for (int i = 0; i < this._menuItems.Count; i++)
			{
				if (this._menuItems[i].Visible)
				{
					num += this.MeasureItem(this._menuItems[i]).Y + num2;
				}
			}
			num -= num2;
			Rectangle rectangle = Screen.Adjuster.ScreenRect;
			if (this.DrawArea != null)
			{
				rectangle = this.DrawArea.Value;
			}
			float num3 = (float)rectangle.Height;
			float num4 = (float)rectangle.Width;
			float num5 = (num3 - num) / 2f + (float)rectangle.Y;
			if (this.VerticalAlignment == MenuScreen.VerticalAlignmentTypes.Top)
			{
				num5 = (float)rectangle.Top;
			}
			else if (this.VerticalAlignment == MenuScreen.VerticalAlignmentTypes.Bottom)
			{
				num5 = (float)rectangle.Bottom - num;
			}
			for (int j = 0; j < this._menuItems.Count; j++)
			{
				MenuItemElement menuItemElement = this._menuItems[j];
				if (menuItemElement.Visible)
				{
					SpriteFont font = (menuItemElement.Font == null) ? this.Font : menuItemElement.Font;
					Color color = (menuItemElement.TextColor != null) ? menuItemElement.TextColor.Value : this.TextColor;
					Color outlineColor = (menuItemElement.OutlineColor != null) ? menuItemElement.OutlineColor.Value : this.OutlineColor;
					int num6 = (menuItemElement.OnlineWidth != null) ? menuItemElement.OnlineWidth.Value : this.OutlineWidth;
					Vector2 vector = this.MeasureItem(menuItemElement);
					Vector2 location = new Vector2((float)rectangle.Left, num5);
					if (this.HorizontalAlignment == MenuScreen.HorizontalAlignmentTypes.Center)
					{
						location.X = (float)rectangle.X + (num4 - vector.X) / 2f;
					}
					else if (this.HorizontalAlignment == MenuScreen.HorizontalAlignmentTypes.Right)
					{
						location.X = (float)rectangle.Right - vector.X;
					}
					num5 += vector.Y + num2;
					Color textColor = color;
					if (j == this._selectedIndex)
					{
						Color value = (menuItemElement.SelectedColor != null) ? menuItemElement.SelectedColor.Value : this.SelectedColor;
						float amount = this._flashDir ? this._flashTimer.PercentComplete : (1f - this._flashTimer.PercentComplete);
						textColor = Color.Lerp(color, value, amount);
					}
					this._itemLocations[j] = new Rectangle((int)location.X, (int)location.Y, (int)vector.X, (int)vector.Y);
					spriteBatch.DrawOutlinedText(font, menuItemElement.Text, location, textColor, outlineColor, (int)Math.Ceiling((double)((float)num6 * Screen.Adjuster.ScaleFactor.Y)), Screen.Adjuster.ScaleFactor.Y, 0f, Vector2.Zero);
				}
			}
			spriteBatch.End();
			base.OnDraw(device, spriteBatch, gameTime);
		}
	}
}
