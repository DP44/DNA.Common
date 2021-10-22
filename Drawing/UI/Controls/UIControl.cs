using System;
using System.Collections.Generic;
using DNA.Collections;
using DNA.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI.Controls
{
	public abstract class UIControl
	{
		protected class ControlList<T> : NotifyList<T> where T : UIControl
		{
			private UIControl _owner;

			public ControlList(UIControl owner)
			{
				this._owner = owner;
			}

			protected override void OnInsertComplete(int index, T value)
			{
				if (value._owner != null)
				{
					throw new Exception("Control can only have one parent");
				}
				value._owner = this._owner;
				base.OnInsertComplete(index, value);
			}

			protected override void OnRemoveComplete(int index, T value)
			{
				value._owner = null;
				base.OnRemoveComplete(index, value);
			}

			protected override void OnSetComplete(int index, T oldValue, T newValue)
			{
				oldValue._owner = null;
				if (newValue._owner != null)
				{
					throw new Exception("Control can only have one parent");
				}
				newValue._owner = this._owner;
				base.OnSetComplete(index, oldValue, newValue);
			}

			protected override void OnClear()
			{
				foreach (T t in this)
				{
					UIControl uicontrol = t;
					if (this._owner.FocusControl == uicontrol)
					{
						this._owner.FocusControl = null;
					}
					if (this._owner.CaptureControl == uicontrol)
					{
						this._owner.CaptureControl = null;
					}
					uicontrol._owner = null;
				}
				base.OnClear();
			}
		}

		protected static Texture2D DummyTexture;

		internal UIControl _owner;

		private bool _isTabStop;

		public virtual UIControl CaptureControl
		{
			get
			{
				if (this._owner == null)
				{
					return null;
				}
				return this._owner.CaptureControl;
			}
			set
			{
				this._owner.CaptureControl = value;
			}
		}

		public virtual UIControl FocusControl
		{
			get
			{
				return this._owner.FocusControl;
			}
			set
			{
				this._owner.FocusControl = value;
			}
		}

		public bool CaptureInput
		{
			get
			{
				return this.CaptureControl == this;
			}
			set
			{
				if (value)
				{
					this.CaptureControl = this;
					return;
				}
				if (this.CaptureControl == this)
				{
					this.CaptureControl = null;
				}
			}
		}

		public bool IsTabStop
		{
			get
			{
				return this._isTabStop;
			}
			protected set
			{
				this._isTabStop = value;
			}
		}

		public bool HasFocus
		{
			get
			{
				return this.FocusControl == this;
			}
			set
			{
				if (value)
				{
					this.FocusControl = this;
					return;
				}
				if (this.FocusControl == this)
				{
					this.FocusControl = null;
				}
			}
		}

		public object Tag { get; set; }

		public bool Visible { get; set; }

		public bool Enabled { get; set; }

		public abstract Size Size { get; set; }

		public Point LocalPosition { get; set; }

		public Point ScreenPosition
		{
			get
			{
				return this.LocalToScreen(this.LocalPosition);
			}
		}

		public virtual Rectangle LocalBounds
		{
			get
			{
				return new Rectangle(this.LocalPosition.X, this.LocalPosition.Y, this.Size.Width, this.Size.Height);
			}
			set
			{
				this.LocalPosition = new Point(value.Left, value.Top);
				this.Size = new Size(value.Width, value.Height);
			}
		}

		public Rectangle ScreenBounds
		{
			get
			{
				return new Rectangle(this.ScreenPosition.X, this.ScreenPosition.Y, this.Size.Width, this.Size.Height);
			}
		}

		public Point ScreenToLocal(Point point)
		{
			if (this._owner != null)
			{
				return new Point(point.X - this._owner.ScreenPosition.X, point.Y - this._owner.ScreenPosition.Y);
			}
			return point;
		}

		public Point LocalToScreen(Point point)
		{
			if (this._owner != null)
			{
				return new Point(this._owner.ScreenPosition.X + point.X, this._owner.ScreenPosition.Y + point.Y);
			}
			return point;
		}

		public UIControl()
		{
			this.Enabled = true;
			this.Visible = true;
		}

		protected abstract void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime);

		public void Draw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			if (UIControl.DummyTexture == null)
			{
				UIControl.DummyTexture = new Texture2D(device, 1, 1);
				UIControl.DummyTexture.SetData<Color>(new Color[]
				{
					Color.White
				});
			}
			if (this.Visible)
			{
				this.OnDraw(device, spriteBatch, gameTime);
			}
		}

		public virtual void CollectControls(List<UIControl> controlList)
		{
			controlList.Add(this);
		}

		protected virtual void OnUpdate(DNAGame game, GameTime gameTime)
		{
		}

		public void Update(DNAGame game, GameTime gameTime)
		{
			this.OnUpdate(game, gameTime);
		}

		public virtual bool HitTest(Point screenPoint)
		{
			return this.ScreenBounds.Contains(screenPoint);
		}

		public void ProcessChar(char c)
		{
			this.OnChar(c);
		}

		protected virtual void OnChar(char c)
		{
		}

		protected virtual void PreProcessInput(InputManager inputManager, GameController controller, KeyboardInput chatPad, GameTime gameTime)
		{
		}

		public void ProcessInput(InputManager inputManager, GameController controller, KeyboardInput chatPad, GameTime gameTime)
		{
			if (this.Enabled)
			{
				this.PreProcessInput(inputManager, controller, chatPad, gameTime);
				if (this.CaptureInput || this.CaptureControl == null)
				{
					if (this.HitTest(inputManager.Mouse.Position) && inputManager.Mouse.LeftButtonPressed)
					{
						this.HasFocus = true;
					}
					this.OnInput(inputManager, controller, chatPad, gameTime);
					return;
				}
			}
			else
			{
				if (this.CaptureInput)
				{
					this.CaptureInput = false;
				}
				if (this.HasFocus)
				{
					this.HasFocus = false;
				}
			}
		}

		protected virtual void OnInput(InputManager inputManager, GameController controller, KeyboardInput chatPad, GameTime gameTime)
		{
		}
	}
}
