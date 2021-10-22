using System;
using System.Collections.Generic;
using DNA.Audio;
using DNA.Drawing.UI.Controls;
using DNA.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DNA.Drawing.UI
{
	public class SettingScreen : UIControlScreen
	{
		public string SelectSound;

		public string ClickSound;

		private List<SettingItemElement> _menuItems = new List<SettingItemElement>();

		private int _selectedIndex;

		public float? MenuStart = null;

		private Rectangle[] _itemLocations = new Rectangle[0];

		public int? LineSpacing;

		public SpriteFont Font;

		public Color TextColor;

		public Color SelectedColor;

		public Color OutlineColor = Color.Black;

		public int OnlineWidth = 2;

		private DNAGame _game;

		public Rectangle? DrawArea = null;

		public List<SettingItemElement> MenuItems
		{
			get
			{
				return this._menuItems;
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

		public SettingScreen(DNAGame game, SpriteFont font, bool drawBehind) : base(drawBehind)
		{
			this.Font = font;
			this._game = game;
			this.TextColor = Color.White;
			this.SelectedColor = Color.Red;
		}

		public int HitTest(Point p)
		{
			for (int i = 0; i < this._itemLocations.Length; i++)
			{
				if (this._itemLocations[i].Contains(p))
				{
					return i;
				}
			}
			return -1;
		}

		public SettingScreen(DNAGame game, SpriteFont font, Color textColor, Color selectedColor, bool drawBehind) : base(drawBehind)
		{
			this.TextColor = textColor;
			this.SelectedColor = selectedColor;
			this.Font = font;
			this._game = game;
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
			bool flag = true;
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
					BarSettingItem barSettingItem = this._menuItems[num] as BarSettingItem;
					if (barSettingItem != null)
					{
						flag = !barSettingItem.SetBarValue(input.Mouse);
					}
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
			if (controller.PressedButtons.A || input.Keyboard.WasKeyPressed(Keys.Enter) || (input.Mouse.LeftButtonPressed && this.HitTest(input.Mouse.Position) >= 0))
			{
				if (this.ClickSound != null)
				{
					SoundManager.Instance.PlayInstance(this.ClickSound);
				}
				if (flag)
				{
					this.ClickedMenuItem();
				}
			}
			if (controller.PressedDPad.Left || (controller.CurrentState.ThumbSticks.Left.X < -num2 && controller.LastState.ThumbSticks.Left.X > -num2) || (controller.CurrentState.ThumbSticks.Right.X < -num2 && controller.LastState.ThumbSticks.Right.X > -num2) || input.Keyboard.WasKeyPressed(Keys.Left) || (input.Mouse.DeltaWheel < 0 && this.HitTest(input.Mouse.Position) >= 0))
			{
				this.DecreasedMenuItem();
				this._menuItems[this._selectedIndex].ResetTimer();
			}
			else if ((controller.CurrentState.DPad.Left == ButtonState.Pressed || controller.CurrentState.ThumbSticks.Left.X < -num2 || controller.CurrentState.ThumbSticks.Right.X < -num2 || input.Keyboard.IsKeyDown(Keys.Left)) && this._menuItems[this._selectedIndex].ChangeValue(gameTime.ElapsedGameTime))
			{
				this.DecreasedMenuItem();
			}
			if (controller.PressedDPad.Right || (controller.CurrentState.ThumbSticks.Left.X > num2 && controller.LastState.ThumbSticks.Left.X < num2) || (controller.CurrentState.ThumbSticks.Right.X > num2 && controller.LastState.ThumbSticks.Right.X < num2) || input.Keyboard.WasKeyPressed(Keys.Right) || (input.Mouse.DeltaWheel > 0 && this.HitTest(input.Mouse.Position) >= 0))
			{
				this.IncreasedMenuItem();
				this._menuItems[this._selectedIndex].ResetTimer();
			}
			else if ((controller.CurrentState.DPad.Right == ButtonState.Pressed || controller.CurrentState.ThumbSticks.Left.X > num2 || controller.CurrentState.ThumbSticks.Right.X > num2 || input.Keyboard.IsKeyDown(Keys.Right)) && this._menuItems[this._selectedIndex].ChangeValue(gameTime.ElapsedGameTime))
			{
				this.IncreasedMenuItem();
			}
			return base.OnPlayerInput(input, controller, chatpad, gameTime);
		}

		private void ClickedMenuItem()
		{
			if (this._selectedIndex >= 0)
			{
				SettingItemElement settingItemElement = this._menuItems[this._selectedIndex];
				settingItemElement.OnClicked();
			}
		}

		private void IncreasedMenuItem()
		{
			if (this._selectedIndex >= 0)
			{
				SettingItemElement settingItemElement = this._menuItems[this._selectedIndex];
				settingItemElement.Increased();
			}
		}

		private void DecreasedMenuItem()
		{
			if (this._selectedIndex >= 0)
			{
				SettingItemElement settingItemElement = this._menuItems[this._selectedIndex];
				settingItemElement.Decreased();
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

		private Vector2 MeasureItem(SettingItemElement item)
		{
			SpriteFont spriteFont = (item.Font == null) ? this.Font : item.Font;
			return spriteFont.MeasureString(item.Text);
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			if (this._itemLocations.Length != this._menuItems.Count)
			{
				this._itemLocations = new Rectangle[this._menuItems.Count];
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
				num2 = (float)this.LineSpacing.Value;
			}
			for (int i = 0; i < this._menuItems.Count; i++)
			{
				if (this._menuItems[i].Visible)
				{
					num += this.MeasureItem(this._menuItems[i]).Y + num2;
				}
			}
			if (this.DrawArea == null)
			{
				this.DrawArea = new Rectangle?(device.Viewport.TitleSafeArea);
			}
			num -= num2;
			float num3 = (float)this.DrawArea.Value.Height;
			int width = this.DrawArea.Value.Width;
			float num4 = (num3 - num) / 2f + (float)this.DrawArea.Value.Y;
			if (this.MenuStart != null)
			{
				num4 = this.MenuStart.Value;
			}
			if (num4 < (float)this.DrawArea.Value.Top)
			{
				num4 = (float)this.DrawArea.Value.Top;
			}
			for (int j = 0; j < this._menuItems.Count; j++)
			{
				SettingItemElement settingItemElement = this._menuItems[j];
				if (settingItemElement.Visible)
				{
					SpriteFont font = (settingItemElement.Font == null) ? this.Font : settingItemElement.Font;
					Color textColor = (settingItemElement.TextColor != null) ? settingItemElement.TextColor.Value : this.TextColor;
					Color outlineColor = (settingItemElement.OutlineColor != null) ? settingItemElement.OutlineColor.Value : this.OutlineColor;
					int outlineWidth = (settingItemElement.OnlineWidth != null) ? settingItemElement.OnlineWidth.Value : this.OnlineWidth;
					Vector2 vector = this.MeasureItem(settingItemElement);
					if (j == this._selectedIndex)
					{
						textColor = ((settingItemElement.SelectedColor != null) ? settingItemElement.SelectedColor.Value : this.SelectedColor);
					}
					this._itemLocations[j] = new Rectangle(this.DrawArea.Value.Left, (int)num4, this.DrawArea.Value.Width, (int)vector.Y);
					settingItemElement.OnDraw(this._game, device, spriteBatch, font, textColor, outlineColor, outlineWidth, new Vector2((float)this.DrawArea.Value.Left, num4));
					num4 += vector.Y + num2;
				}
			}
			spriteBatch.End();
			base.OnDraw(device, spriteBatch, gameTime);
		}
	}
}
