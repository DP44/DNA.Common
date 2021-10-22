using System;
using System.Collections.Generic;
using DNA.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI.Controls
{
	public class UIControlScreen : Screen
	{
		private UIControlGroup _controlGroup = new UIControlGroup();

		private GameController _controller = new GameController(PlayerIndex.One);

		private KeyboardInput _keyboard = new KeyboardInput();

		public IList<UIControl> Controls
		{
			get
			{
				return this._controlGroup.Children;
			}
		}

		public UIControlScreen(bool drawBehind) : base(true, drawBehind)
		{
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			spriteBatch.Begin();
			this._controlGroup.Draw(device, spriteBatch, gameTime);
			spriteBatch.End();
			base.OnDraw(device, spriteBatch, gameTime);
		}

		protected override void OnUpdate(DNAGame game, GameTime gameTime)
		{
			this._controlGroup.Update(game, gameTime);
			base.OnUpdate(game, gameTime);
		}

		public override bool ProcessChar(GameTime gameTime, char c)
		{
			this._controlGroup.ProcessChar(c);
			return base.ProcessChar(gameTime, c);
		}

		protected override bool OnInput(InputManager inputManager, GameTime gameTime)
		{
			if (Screen.SelectedPlayerIndex == null)
			{
				this._controlGroup.ProcessInput(inputManager, this._controller, this._keyboard, gameTime);
			}
			return base.OnInput(inputManager, gameTime);
		}

		protected override bool OnPlayerInput(InputManager inputManager, GameController controller, KeyboardInput chatPad, GameTime gameTime)
		{
			if (Screen.SelectedPlayerIndex != null)
			{
				this._controlGroup.ProcessInput(inputManager, controller, chatPad, gameTime);
			}
			return base.OnPlayerInput(inputManager, controller, chatPad, gameTime);
		}
	}
}
