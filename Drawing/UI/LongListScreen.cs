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
	public class LongListScreen : Screen
	{
		public abstract class MenuItem
		{
			public object Tag;

			public bool Selected;

			public MenuItem(object tag)
			{
				this.Tag = tag;
			}

			public abstract void Draw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime, Vector2 pos);

			public abstract Vector2 Measure();
		}

		public class TextItem : LongListScreen.MenuItem
		{
			public string Text;

			public SpriteFont Font;

			public Color Color = Color.White;

			public Color SelectedColor = Color.Red;

			private OneShotTimer flashTimer = new OneShotTimer(TimeSpan.FromSeconds(0.25));

			private bool selectedDirection;

			public TextItem(string text, SpriteFont font, object tag) : base(tag)
			{
				this.Text = text;
				this.Font = font;
			}

			public override void Draw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime, Vector2 pos)
			{
				Color textColor = this.Color;
				if (this.Selected)
				{
					this.flashTimer.Update(gameTime.ElapsedGameTime);
					if (this.flashTimer.Expired)
					{
						this.flashTimer.Reset();
						this.selectedDirection = !this.selectedDirection;
					}
					if (this.selectedDirection)
					{
						textColor = Color.Lerp(this.Color, this.SelectedColor, this.flashTimer.PercentComplete);
					}
					else
					{
						textColor = Color.Lerp(this.SelectedColor, this.Color, this.flashTimer.PercentComplete);
					}
				}
				spriteBatch.DrawOutlinedText(this.Font, this.Text, pos, textColor, Color.Black, 1);
			}

			public override Vector2 Measure()
			{
				return this.Font.MeasureString(this.Text);
			}
		}

		public const float deadZone = 0.25f;

		private List<LongListScreen.MenuItem> _menuItems = new List<LongListScreen.MenuItem>();

		protected int _middleIndex;

		protected int _selectedIndex;

		private OneShotTimer holdTimer = new OneShotTimer(TimeSpan.FromSeconds(0.25));

		private OneShotTimer scrollTimer = new OneShotTimer(TimeSpan.FromSeconds(0.10000000149011612));

		private OneShotTimer accelerateDelayTimer = new OneShotTimer(TimeSpan.FromSeconds(3.0));

		private OneShotTimer accelerateTimer = new OneShotTimer(TimeSpan.FromSeconds(5.0));

		private TimeSpan scrollRate = TimeSpan.FromSeconds(0.05000000074505806);

		private bool lastselectup;

		private bool lastselectdown;

		public string SelectSound;

		public string ClickSound;

		private Rectangle[] _itemLocations = new Rectangle[0];

		public Rectangle destRect;

		private Viewport viewPort = default(Viewport);

		private bool hitTestTrue;

		public List<LongListScreen.MenuItem> MenuItems
		{
			get
			{
				return this._menuItems;
			}
		}

		public LongListScreen.MenuItem SelectedItem
		{
			get
			{
				if (this._selectedIndex < 0 || this._selectedIndex >= this._menuItems.Count)
				{
					return null;
				}
				return this._menuItems[this._selectedIndex];
			}
		}

		public LongListScreen(bool drawBehind) : base(true, drawBehind)
		{
		}

		public event EventHandler<SelectedEventArgs> Clicked;

		protected virtual void OnClicked(LongListScreen.MenuItem selectedItem)
		{
		}

		public void Click()
		{
			this.OnClicked(this.SelectedItem);
			if (this.Clicked != null)
			{
				this.Clicked(this, new SelectedEventArgs(this.SelectedItem.Tag));
			}
			if (this.ClickSound != null)
			{
				SoundManager.Instance.PlayInstance(this.ClickSound);
			}
		}

		public event EventHandler<SelectedEventArgs> BackClicked;

		protected virtual void OnBack()
		{
		}

		public void Back()
		{
			this.OnBack();
			if (this.BackClicked != null)
			{
				this.BackClicked(this, null);
			}
			if (this.ClickSound != null)
			{
				SoundManager.Instance.PlayInstance(this.ClickSound);
			}
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

		protected override bool OnPlayerInput(InputManager input, GameController controller, KeyboardInput chatpad, GameTime gameTime)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			float num = (float)(this.viewPort.Bounds.Height / 3);
			float num2 = (float)this.viewPort.Bounds.Bottom - num;
			float num3 = num;
			float num4 = (float)this.viewPort.Bounds.Bottom - num * 0.2f;
			float num5 = num * 0.2f;
			if (controller.CurrentState.ThumbSticks.Left.Y < -0.25f || controller.CurrentState.DPad.Down == ButtonState.Pressed || controller.CurrentState.Triggers.Right > 0.25f || input.Keyboard.IsKeyDown(Keys.Down) || (!flag && (float)input.Mouse.Position.Y > num2 && !this.hitTestTrue && input.Mouse.Position.X < this.viewPort.Bounds.Center.X) || (!flag && input.Mouse.Position.Y > this.destRect.Bottom && input.Mouse.Position.X < this.viewPort.Bounds.Center.X))
			{
				flag3 = true;
				if (!this.lastselectdown)
				{
					this.accelerateDelayTimer.Reset();
					this.accelerateTimer.Reset();
				}
			}
			if (controller.CurrentState.ThumbSticks.Left.Y > 0.25f || controller.CurrentState.DPad.Up == ButtonState.Pressed || controller.CurrentState.Triggers.Left > 0.25f || input.Keyboard.IsKeyDown(Keys.Up) || (!flag && (float)input.Mouse.Position.Y < num3 && !this.hitTestTrue))
			{
				flag2 = true;
				if (!this.lastselectup)
				{
					this.accelerateDelayTimer.Reset();
					this.accelerateTimer.Reset();
				}
			}
			if (Math.Abs(controller.CurrentState.ThumbSticks.Left.Y) > 0.8f || (!flag && (float)input.Mouse.Position.Y < num5) || (!flag && (float)input.Mouse.Position.Y > num4))
			{
				this.accelerateDelayTimer.Update(gameTime.ElapsedGameTime);
			}
			if (this.accelerateDelayTimer.Expired)
			{
				this.accelerateTimer.Update(gameTime.ElapsedGameTime);
			}
			float num6 = this.accelerateTimer.PercentComplete * (float)this.scrollRate.TotalSeconds / 2f;
			float val = Math.Abs(controller.CurrentState.ThumbSticks.Left.Y);
			float val2;
			if (!flag && (float)input.Mouse.Position.Y < num3 && !this.hitTestTrue)
			{
				val2 = (num3 - (float)input.Mouse.Position.Y) / num;
			}
			else if (!flag && (float)input.Mouse.Position.Y > num2 && !this.hitTestTrue)
			{
				val2 = ((float)input.Mouse.Position.Y - num2) / num;
			}
			else
			{
				val2 = 0f;
			}
			float num7 = Math.Max(val, val2);
			if (num7 > 0f)
			{
				this.scrollTimer.MaxTime = TimeSpan.FromSeconds(this.scrollRate.TotalSeconds / (double)num7 - (double)num6);
			}
			else
			{
				this.scrollTimer.MaxTime = TimeSpan.FromSeconds(0.1);
			}
			if ((controller.CurrentState.ThumbSticks.Left.Y < -0.25f && controller.LastState.ThumbSticks.Left.Y > -0.25f) || (controller.PressedDPad.Down || (controller.CurrentState.Triggers.Right > 0.25f && controller.LastState.Triggers.Right < 0.25f)) || (input.Keyboard.WasKeyPressed(Keys.Down) || (!flag && (float)input.Mouse.Position.Y > num2 && (float)input.Mouse.LastPosition.Y <= num2 && !this.hitTestTrue)) || input.Mouse.DeltaWheel < 0)
			{
				if (this._middleIndex < this._menuItems.Count - 1 && this.SelectSound != null && input.Mouse.DeltaWheel < 0)
				{
					SoundManager.Instance.PlayInstance(this.SelectSound);
				}
				this._middleIndex++;
			}
			if ((controller.CurrentState.ThumbSticks.Left.Y > 0.25f && controller.LastState.ThumbSticks.Left.Y < 0.25f) || (controller.PressedDPad.Up || (controller.CurrentState.Triggers.Left > 0.25f && controller.LastState.Triggers.Left < 0.25f)) || (input.Keyboard.WasKeyPressed(Keys.Up) || (!flag && (float)input.Mouse.Position.Y < num3 && (float)input.Mouse.LastPosition.Y >= num3 && !this.hitTestTrue)) || input.Mouse.DeltaWheel > 0)
			{
				if (this._middleIndex > 0 && this.SelectSound != null && input.Mouse.DeltaWheel > 0)
				{
					SoundManager.Instance.PlayInstance(this.SelectSound);
				}
				this._middleIndex--;
			}
			if (flag3)
			{
				this.holdTimer.Update(gameTime.ElapsedGameTime);
				if (this.holdTimer.Expired)
				{
					this.scrollTimer.Update(gameTime.ElapsedGameTime);
					if (this.scrollTimer.Expired)
					{
						this.scrollTimer.Reset();
						if (this.accelerateTimer.Expired)
						{
							this._middleIndex += 10;
						}
						else
						{
							this._middleIndex++;
						}
						if (this._middleIndex < this._menuItems.Count && this.SelectSound != null)
						{
							SoundManager.Instance.PlayInstance(this.SelectSound);
						}
					}
				}
			}
			else if (flag2)
			{
				this.holdTimer.Update(gameTime.ElapsedGameTime);
				if (this.holdTimer.Expired)
				{
					this.scrollTimer.Update(gameTime.ElapsedGameTime);
					if (this.scrollTimer.Expired)
					{
						this.scrollTimer.Reset();
						if (this.accelerateTimer.Expired)
						{
							this._middleIndex -= 10;
						}
						else
						{
							this._middleIndex--;
						}
						if (this._middleIndex >= 0 && this.SelectSound != null)
						{
							SoundManager.Instance.PlayInstance(this.SelectSound);
						}
					}
				}
			}
			if (this._middleIndex < 0)
			{
				this._middleIndex = 0;
			}
			if (this._middleIndex >= this._menuItems.Count)
			{
				this._middleIndex = this._menuItems.Count - 1;
			}
			if (controller.PressedButtons.A || controller.PressedButtons.Start || input.Keyboard.WasKeyPressed(Keys.Enter))
			{
				if (this.MenuItems.Count > 0)
				{
					this.Click();
				}
				else
				{
					this.Back();
				}
			}
			else if (input.Mouse.LeftButtonPressed)
			{
				if (this.MenuItems.Count > 0)
				{
					if (this.HitTest(input.Mouse.Position) >= 0)
					{
						this.Click();
					}
				}
				else
				{
					this.Back();
				}
			}
			if (controller.PressedButtons.B || controller.PressedButtons.Back || input.Keyboard.WasKeyPressed(Keys.Escape))
			{
				this.Back();
			}
			base.OnPlayerInput(input, controller, chatpad, gameTime);
			if ((!flag3 && this.lastselectdown) || (!flag2 && this.lastselectup))
			{
				this.holdTimer.Reset();
			}
			this.lastselectdown = flag3;
			this.lastselectup = flag2;
			int num8 = this.HitTest(input.Mouse.Position);
			if (num8 >= 0)
			{
				this.hitTestTrue = true;
				if (this._selectedIndex != num8)
				{
					if (this.SelectSound != null && !flag3 && !flag2)
					{
						SoundManager.Instance.PlayInstance(this.SelectSound);
					}
					this._selectedIndex = num8;
				}
			}
			else
			{
				this.hitTestTrue = false;
				if (this._selectedIndex != this._middleIndex)
				{
					if (this.SelectSound != null)
					{
						SoundManager.Instance.PlayInstance(this.SelectSound);
					}
					this._selectedIndex = this._middleIndex;
				}
			}
			return base.OnPlayerInput(input, controller, chatpad, gameTime);
		}

		private void _drawPreviousItems(int middleIndex, float startYPos, GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			int i = middleIndex - 1;
			if (i < 0 || i >= this._menuItems.Count)
			{
				return;
			}
			float num = startYPos;
			while (i >= 0)
			{
				LongListScreen.MenuItem menuItem = this._menuItems[i];
				if (i == this._selectedIndex)
				{
					menuItem.Selected = true;
				}
				else
				{
					menuItem.Selected = false;
				}
				Vector2 vector = menuItem.Measure();
				num -= vector.Y;
				if (num + vector.Y < (float)this.destRect.Top)
				{
					IL_CF:
					while (i >= 0)
					{
						this._setItemLocation(i, Rectangle.Empty);
						i--;
					}
					return;
				}
				this._setItemLocation(i, new Rectangle(this.destRect.Left, (int)num, (int)vector.X, (int)vector.Y));
				menuItem.Draw(device, spriteBatch, gameTime, new Vector2((float)this.destRect.Left, num));
				i--;
			}
			goto IL_CF;
		}

		private void _drawPostItems(int middleIndex, float startYPos, GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			int i = middleIndex + 1;
			if (i < 0 || i >= this._menuItems.Count)
			{
				return;
			}
			float num = startYPos;
			while (i < this._menuItems.Count)
			{
				LongListScreen.MenuItem menuItem = this._menuItems[i];
				if (i == this._selectedIndex)
				{
					menuItem.Selected = true;
				}
				else
				{
					menuItem.Selected = false;
				}
				Vector2 vector = menuItem.Measure();
				if (num > (float)this.viewPort.Bounds.Bottom)
				{
					IL_DA:
					while (i < this._menuItems.Count)
					{
						this._setItemLocation(i, Rectangle.Empty);
						i++;
					}
					return;
				}
				this._setItemLocation(i, new Rectangle(this.destRect.Left, (int)num, (int)vector.X, (int)vector.Y));
				menuItem.Draw(device, spriteBatch, gameTime, new Vector2((float)this.destRect.Left, num));
				i++;
				num += vector.Y;
			}
			goto IL_DA;
		}

		private void _drawSelectedItem(int middleIndex, float startYPos, GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			if (middleIndex < 0 || middleIndex >= this._menuItems.Count)
			{
				return;
			}
			LongListScreen.MenuItem menuItem = this._menuItems[middleIndex];
			Vector2 vector = menuItem.Measure();
			menuItem.Draw(device, spriteBatch, gameTime, new Vector2((float)this.destRect.Left, startYPos));
			this._setItemLocation(middleIndex, new Rectangle(this.destRect.Left, (int)startYPos, (int)vector.X, (int)vector.Y));
		}

		private void _setItemLocation(int index, Rectangle location)
		{
			try
			{
				this._itemLocations[index] = location;
			}
			catch
			{
			}
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			if (this._itemLocations.Length != this._menuItems.Count)
			{
				this._itemLocations = new Rectangle[this._menuItems.Count];
			}
			this.viewPort = device.Viewport;
			int middleIndex = this._middleIndex;
			if (middleIndex < 0 || middleIndex >= this._menuItems.Count)
			{
				return;
			}
			LongListScreen.MenuItem menuItem = this._menuItems[middleIndex];
			if (middleIndex == this._selectedIndex)
			{
				menuItem.Selected = true;
			}
			else
			{
				menuItem.Selected = false;
			}
			Vector2 vector = menuItem.Measure();
			float num = (float)this.destRect.Center.Y - vector.Y / 2f;
			spriteBatch.Begin();
			this._drawSelectedItem(middleIndex, num, device, spriteBatch, gameTime);
			this._drawPreviousItems(middleIndex, num, device, spriteBatch, gameTime);
			this._drawPostItems(middleIndex, num + vector.Y, device, spriteBatch, gameTime);
			spriteBatch.End();
			base.OnDraw(device, spriteBatch, gameTime);
		}
	}
}
