using System;
using System.Collections.Generic;
using DNA.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI.Controls
{
	public class UIControlGroup : UIControl
	{
		private UIControl _focusControl;

		private UIControl _captureControl;

		private UIControl.ControlList<UIControl> _children;

		public override UIControl FocusControl
		{
			get
			{
				if (this._owner == null)
				{
					return this._focusControl;
				}
				return this._owner.FocusControl;
			}
			set
			{
				if (this._owner == null)
				{
					this._focusControl = value;
					return;
				}
				this._focusControl = null;
				this._owner.FocusControl = value;
			}
		}

		public override UIControl CaptureControl
		{
			get
			{
				if (this._owner == null)
				{
					return this._captureControl;
				}
				return this._owner.CaptureControl;
			}
			set
			{
				if (this._owner == null)
				{
					this._captureControl = value;
					return;
				}
				this._captureControl = null;
				this._owner.CaptureControl = value;
			}
		}

		public override Rectangle LocalBounds
		{
			get
			{
				if (this._children.Count == 0)
				{
					return Rectangle.Empty;
				}
				Rectangle rectangle = this._children[0].LocalBounds;
				for (int i = 1; i < this._children.Count; i++)
				{
					rectangle = Rectangle.Union(rectangle, this._children[i].LocalBounds);
				}
				return rectangle;
			}
		}

		public UIControlGroup()
		{
			this._children = new UIControl.ControlList<UIControl>(this);
		}

		public IList<UIControl> Children
		{
			get
			{
				return this._children;
			}
		}

		public override Size Size
		{
			get
			{
				Rectangle localBounds = this.LocalBounds;
				return new Size(localBounds.Width, localBounds.Height);
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override bool HitTest(Point screenPoint)
		{
			return false;
		}

		public override void CollectControls(List<UIControl> controlList)
		{
			base.CollectControls(controlList);
			for (int i = 0; i < this._children.Count; i++)
			{
				this._children[i].CollectControls(controlList);
			}
		}

		protected override void OnChar(char c)
		{
			if (c == '\t')
			{
				List<UIControl> list = new List<UIControl>();
				this.CollectControls(list);
				UIControl uicontrol = this.FocusControl;
				foreach (UIControl uicontrol2 in list)
				{
					if (uicontrol2 == uicontrol)
					{
						uicontrol = null;
					}
					else if (uicontrol2.IsTabStop && uicontrol == null)
					{
						uicontrol = uicontrol2;
						break;
					}
				}
				if (uicontrol == null)
				{
					foreach (UIControl uicontrol3 in list)
					{
						if (uicontrol3.IsTabStop)
						{
							uicontrol = uicontrol3;
							break;
						}
					}
				}
				this.FocusControl = uicontrol;
			}
			else if (this.FocusControl != null)
			{
				this.FocusControl.ProcessChar(c);
			}
			base.OnChar(c);
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			for (int i = 0; i < this._children.Count; i++)
			{
				this._children[i].Draw(device, spriteBatch, gameTime);
			}
		}

		protected override void OnUpdate(DNAGame game, GameTime gameTime)
		{
			base.OnUpdate(game, gameTime);
			for (int i = 0; i < this._children.Count; i++)
			{
				this._children[i].Update(game, gameTime);
			}
		}

		protected override void PreProcessInput(InputManager inputManager, GameController controller, KeyboardInput chatPad, GameTime gameTime)
		{
			for (int i = 0; i < this._children.Count; i++)
			{
				this._children[i].ProcessInput(inputManager, controller, chatPad, gameTime);
			}
			base.PreProcessInput(inputManager, controller, chatPad, gameTime);
		}

		protected override void OnInput(InputManager inputManager, GameController controller, KeyboardInput chatPad, GameTime gameTime)
		{
			base.OnInput(inputManager, controller, chatPad, gameTime);
			for (int i = 0; i < this._children.Count; i++)
			{
				if (inputManager.Mouse.LeftButtonPressed && this._children[i].HitTest(inputManager.Mouse.Position))
				{
					this._children[i].HasFocus = true;
				}
				this._children[i].ProcessInput(inputManager, controller, chatPad, gameTime);
			}
		}
	}
}
