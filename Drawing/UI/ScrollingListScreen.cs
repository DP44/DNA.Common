using System;
using System.Collections.Generic;
using DNA.Audio;
using DNA.Drawing.UI.Controls;
using DNA.Input;
using DNA.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DNA.Drawing.UI
{
	public class ScrollingListScreen : UIControlScreen
	{
		public List<ListItemControl> Items = new List<ListItemControl>();

		public string ClickSound;

		private int _topItemIndex;

		private int _selectedItemIndex;

		protected Size _itemSize;

		private int _itemsToDraw;

		protected Rectangle _drawArea;

		private ScrollBarControl _scrollBar;

		protected ButtonControl _selectButton;

		private ButtonControl _backButton;

		private UIControlGroup _listItems = new UIControlGroup();

		private OneShotTimer _doubleClickTimer = new OneShotTimer(TimeSpan.FromSeconds(0.4));

		private int mouseClicks;

		public ButtonControl SelectButton
		{
			set
			{
				if (this._selectButton != null)
				{
					base.Controls.Remove(this._selectButton);
				}
				this._selectButton = value;
				this._selectButton.Pressed += this._selectButton_Pressed;
				base.Controls.Add(this._selectButton);
			}
		}

		public ButtonControl BackButton
		{
			set
			{
				if (this._backButton != null)
				{
					base.Controls.Remove(this._backButton);
				}
				this._backButton = value;
				this._backButton.Pressed += this._backButton_Pressed;
				base.Controls.Add(this._backButton);
			}
		}

		private void _backButton_Pressed(object sender, EventArgs e)
		{
			if (this.ClickSound != null)
			{
				SoundManager.Instance.PlayInstance(this.ClickSound);
			}
			base.PopMe();
		}

		public event EventHandler<EventArgs> Clicked;

		protected virtual void OnClicked(ListItemControl selectedItem)
		{
		}

		protected override void OnUpdate(DNAGame game, GameTime gameTime)
		{
			int itemsToDraw = this._itemsToDraw;
			this._itemsToDraw = this._drawArea.Height / (this._itemSize.Height + 5);
			this._scrollBar.LocalPosition = new Point(this._drawArea.X, this._drawArea.Y);
			this._scrollBar.Size = new Size(20, this._itemsToDraw * (this._itemSize.Height + 5));
			this._scrollBar.SliderHeight = Math.Max(10, this._drawArea.Height - this._scrollBar.ArrowSize * 2 - 30 - (this.Items.Count - this._itemsToDraw) * 20);
			if (itemsToDraw != this._itemsToDraw)
			{
				this._updateControls();
			}
			if (this._selectButton != null)
			{
				if (this.Items.Count > 0)
				{
					this._selectButton.Enabled = this.SelectedItem.Active;
					this._selectButton.Visible = true;
				}
				else
				{
					this._selectButton.Visible = false;
				}
			}
			base.OnUpdate(game, gameTime);
		}

		public virtual void Click()
		{
			this.OnClicked(this.SelectedItem);
			if (this.Clicked != null)
			{
				this.Clicked(this, new EventArgs());
			}
			if (this.ClickSound != null)
			{
				SoundManager.Instance.PlayInstance(this.ClickSound);
			}
		}

		private void _selectButton_Pressed(object sender, EventArgs e)
		{
			this.Click();
		}

		public ListItemControl SelectedItem
		{
			get
			{
				if (this.Items.Count <= 0)
				{
					return null;
				}
				return this.Items[this._selectedItemIndex];
			}
		}

		public ScrollingListScreen(bool drawBehind, Size itemSize, Rectangle drawArea) : base(drawBehind)
		{
			this._itemSize = itemSize;
			this._drawArea = drawArea;
			this._itemsToDraw = drawArea.Height / (this._itemSize.Height + 5);
			this._scrollBar = new ScrollBarControl();
			this._scrollBar.LocalPosition = new Point(drawArea.X, drawArea.Y);
			this._scrollBar.Size = new Size(20, this._itemsToDraw * (this._itemSize.Height + 5));
			this._scrollBar.DownArrowPressed += this._scrollBar_DownArrowPressed;
			this._scrollBar.UpArrowPressed += this._scrollBar_UpArrowPressed;
			base.Controls.Add(this._listItems);
			base.Controls.Add(this._scrollBar);
		}

		private void _scrollBar_UpArrowPressed(object sender, EventArgs e)
		{
			this._moveUp(true);
		}

		private void _scrollBar_DownArrowPressed(object sender, EventArgs e)
		{
			this._moveDown(true);
		}

		public override void OnPushed()
		{
			this._selectedItemIndex = 0;
			this._topItemIndex = 0;
			this._updateControls();
			if (this._scrollBar.Visible)
			{
				int num = this.Items.Count - this._itemsToDraw;
				this._scrollBar.Value = (float)this._topItemIndex / (float)num;
			}
			else
			{
				this._scrollBar.Value = 0f;
			}
			base.OnPushed();
		}

		protected void UpdateAfterPopulate()
		{
			int topItemIndex = this._topItemIndex;
			int itemsToDraw = this._itemsToDraw;
			this._updateControls();
			if (this._itemsToDraw != itemsToDraw || this._topItemIndex != topItemIndex)
			{
				if (this._scrollBar.Visible)
				{
					int num = this.Items.Count - this._itemsToDraw;
					this._scrollBar.Value = (float)this._topItemIndex / (float)num;
					return;
				}
				this._scrollBar.Value = 0f;
			}
		}

		private void _updateControls()
		{
			this._listItems.Children.Clear();
			if (this.Items.Count > 0)
			{
				if (this.Items.Count <= this._itemsToDraw)
				{
					this._topItemIndex = 0;
					this._scrollBar.Visible = false;
				}
				else
				{
					this._scrollBar.Visible = true;
					this._scrollBar.SliderHeight = Math.Max(10, this._drawArea.Height - this._scrollBar.ArrowSize * 2 - 30 - (this.Items.Count - this._itemsToDraw) * 20);
					if (this._topItemIndex > this.Items.Count - this._itemsToDraw)
					{
						this._topItemIndex = this.Items.Count - this._itemsToDraw;
					}
				}
				if (this._selectedItemIndex >= this.Items.Count)
				{
					this._selectedItemIndex = this.Items.Count - 1;
				}
				Point localPosition = new Point(this._drawArea.X + 30, this._drawArea.Y);
				int num = Math.Min(this.Items.Count, this._topItemIndex + this._itemsToDraw);
				for (int i = this._topItemIndex; i < num; i++)
				{
					this.Items[i].LocalPosition = localPosition;
					this.Items[i].Selected = (this._selectedItemIndex == i);
					this._listItems.Children.Add(this.Items[i]);
					localPosition.Y += this._itemSize.Height + 5;
				}
				return;
			}
			this._scrollBar.Visible = false;
		}

		protected void _updateControlsOnSort()
		{
			this._listItems.Children.Clear();
			if (this.Items.Count > 0)
			{
				if (this.Items.Count <= this._itemsToDraw)
				{
					this._topItemIndex = 0;
					this._scrollBar.Visible = false;
				}
				else
				{
					this._scrollBar.Visible = true;
					this._scrollBar.SliderHeight = Math.Max(10, this._drawArea.Height - this._scrollBar.ArrowSize * 2 - 10 - (this.Items.Count - this._itemsToDraw) * 20);
					if (this._topItemIndex > this.Items.Count - this._itemsToDraw)
					{
						this._topItemIndex = this.Items.Count - this._itemsToDraw;
					}
				}
				Point localPosition = new Point(this._drawArea.X + 30, this._drawArea.Y);
				int num = Math.Min(this.Items.Count, this._topItemIndex + this._itemsToDraw);
				for (int i = 0; i < this.Items.Count; i++)
				{
					if (i >= this._topItemIndex && i < num)
					{
						this.Items[i].LocalPosition = localPosition;
						this._listItems.Children.Add(this.Items[i]);
						localPosition.Y += this._itemSize.Height + 5;
					}
					if (this.Items[i].Selected)
					{
						this._selectedItemIndex = i;
					}
				}
				if (this._selectedItemIndex >= this.Items.Count)
				{
					this._selectedItemIndex = this.Items.Count - 1;
				}
				this.Items[this._selectedItemIndex].Selected = true;
				return;
			}
			this._scrollBar.Visible = false;
		}

		private void _moveUp(bool setScrollbar)
		{
			if (this._topItemIndex > 0)
			{
				this._topItemIndex--;
				this._updateControls();
				if (setScrollbar)
				{
					int num = this.Items.Count - this._itemsToDraw;
					this._scrollBar.Value = (float)this._topItemIndex / (float)num;
				}
			}
		}

		private void _moveDown(bool setScrollBar)
		{
			if (this._topItemIndex < this.Items.Count - this._itemsToDraw)
			{
				this._topItemIndex++;
				this._updateControls();
				if (setScrollBar)
				{
					int num = this.Items.Count - this._itemsToDraw;
					this._scrollBar.Value = (float)this._topItemIndex / (float)num;
				}
			}
		}

		private void _selectUp()
		{
			if (this._selectedItemIndex > 0)
			{
				this.Items[this._selectedItemIndex].Selected = false;
				this._selectedItemIndex--;
				this.Items[this._selectedItemIndex].Selected = true;
				if (this._selectedItemIndex < this._topItemIndex)
				{
					this._moveUp(true);
				}
			}
		}

		private void _selectDown()
		{
			if (this._selectedItemIndex < this.Items.Count - 1)
			{
				this.Items[this._selectedItemIndex].Selected = false;
				this._selectedItemIndex++;
				this.Items[this._selectedItemIndex].Selected = true;
				if (this._selectedItemIndex >= this._topItemIndex + this._itemsToDraw)
				{
					this._moveDown(true);
				}
			}
		}

		protected override bool OnPlayerInput(InputManager inputManager, GameController controller, KeyboardInput chatPad, GameTime gameTime)
		{
			if (this.Items.Count > 0)
			{
				if (inputManager.Mouse.LeftButtonPressed)
				{
					if (this._doubleClickTimer.Expired)
					{
						this.mouseClicks = 0;
					}
					this._doubleClickTimer.Reset();
					this.mouseClicks++;
				}
				if (inputManager.Mouse.CurrentState.LeftButton == ButtonState.Pressed && !this._scrollBar.CaptureInput)
				{
					int num = Math.Min(this.Items.Count, this._topItemIndex + this._itemsToDraw);
					for (int i = this._topItemIndex; i < num; i++)
					{
						if (this.Items[i].CaptureInput)
						{
							if (this._selectedItemIndex == i)
							{
								if (this.mouseClicks >= 2)
								{
									this.Click();
								}
							}
							else
							{
								this.mouseClicks = 1;
							}
							this.Items[this._selectedItemIndex].Selected = false;
							this._selectedItemIndex = i;
							this.Items[i].Selected = true;
						}
						else
						{
							this.Items[i].Selected = false;
						}
					}
					this.Items[this._selectedItemIndex].Selected = true;
				}
				else if (inputManager.Keyboard.WasKeyPressed(Keys.Up))
				{
					this._selectUp();
				}
				else if (inputManager.Keyboard.WasKeyPressed(Keys.Down))
				{
					this._selectDown();
				}
				else if (inputManager.Mouse.DeltaWheel > 0)
				{
					this._moveUp(true);
				}
				else if (inputManager.Mouse.DeltaWheel < 0)
				{
					this._moveDown(true);
				}
			}
			if (inputManager.Keyboard.WasKeyPressed(Keys.Escape))
			{
				base.PopMe();
			}
			return base.OnPlayerInput(inputManager, controller, chatPad, gameTime);
		}

		public override void Update(DNAGame game, GameTime gameTime)
		{
			this._doubleClickTimer.Update(gameTime.ElapsedGameTime);
			if (this._scrollBar.Visible)
			{
				int num = this.Items.Count - this._itemsToDraw;
				int num2 = ((int)Math.Round((double)(this._scrollBar.Value * (float)num))).Clamp(0, num);
				while (num2 != this._topItemIndex)
				{
					if (num2 < this._topItemIndex)
					{
						this._moveUp(false);
					}
					else
					{
						this._moveDown(false);
					}
				}
			}
			base.Update(game, gameTime);
		}
	}
}
