using System;
using DNA.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI.Controls
{
	public class MenuBarControl : UIControl
	{
		public enum Alignment
		{
			Left,
			Right,
			Center
		}

		public bool DragMenu;

		public MenuBarControl.Alignment TextAlignment;

		private Size _size = new Size(100, 100);

		public ScalableFrame Frame { get; set; }

		public SpriteFont Font { get; set; }

		public string Text { get; set; }

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

		public Color ButtonColor { get; set; }

		public Color TextColor { get; set; }

		public MenuBarControl()
		{
			this.ButtonColor = Color.White;
			this.TextColor = Color.Black;
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			Rectangle screenBounds = base.ScreenBounds;
			Vector2 value = new Vector2((float)screenBounds.Center.X, (float)screenBounds.Center.Y);
			Vector2 value2 = this.Font.MeasureString(this.Text);
			value2.Y = (float)this.Font.LineSpacing;
			Vector2 position = Vector2.Zero;
			switch (this.TextAlignment)
			{
			case MenuBarControl.Alignment.Left:
				position.X = (float)(screenBounds.Left + 5);
				position.Y = value.Y - (float)(this.Font.LineSpacing / 2);
				break;
			case MenuBarControl.Alignment.Right:
				position.X = (float)screenBounds.Right - value2.X - 5f;
				position.Y = value.Y - (float)(this.Font.LineSpacing / 2);
				break;
			case MenuBarControl.Alignment.Center:
				position = value - value2 / 2f;
				break;
			}
			this.Frame.Draw(spriteBatch, screenBounds, this.ButtonColor);
			if (this.Font != null && this.Text != null)
			{
				spriteBatch.DrawString(this.Font, this.Text, position, this.TextColor);
			}
		}

		protected override void OnInput(InputManager inputManager, GameController controller, KeyboardInput chatPad, GameTime gameTime)
		{
			base.CaptureInput = false;
			bool flag = this.HitTest(inputManager.Mouse.Position);
			if (flag && inputManager.Mouse.LeftButtonPressed)
			{
				this.DragMenu = true;
			}
			if (inputManager.Mouse.LeftButtonReleased)
			{
				this.DragMenu = false;
			}
			base.OnInput(inputManager, controller, chatPad, gameTime);
		}
	}
}
