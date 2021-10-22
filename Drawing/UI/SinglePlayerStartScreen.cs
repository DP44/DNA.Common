using System;
using DNA.Audio;
using DNA.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DNA.Drawing.UI
{
	public class SinglePlayerStartScreen : Screen
	{
		public string ClickSound;

		public event EventHandler OnStartPressed;

		public event EventHandler OnBackPressed;

		public SinglePlayerStartScreen(bool drawBehind) : base(true, drawBehind)
		{
		}

		protected override bool OnInput(InputManager inputManager, GameTime gameTime)
		{
			if (inputManager.Mouse.LeftButtonPressed || inputManager.Keyboard.WasKeyPressed(Keys.Enter) || inputManager.Keyboard.WasKeyPressed(Keys.Space))
			{
				if (this.ClickSound != null)
				{
					SoundManager.Instance.PlayInstance(this.ClickSound);
				}
				Screen.SelectedPlayerIndex = new PlayerIndex?(PlayerIndex.One);
				if (this.OnStartPressed != null)
				{
					this.OnStartPressed(this, new EventArgs());
				}
			}
			if (inputManager.Keyboard.WasKeyPressed(Keys.Escape))
			{
				if (this.ClickSound != null)
				{
					SoundManager.Instance.PlayInstance(this.ClickSound);
				}
				base.PopMe();
				Screen.SelectedPlayerIndex = new PlayerIndex?(PlayerIndex.One);
				if (this.OnBackPressed != null)
				{
					this.OnBackPressed(this, new EventArgs());
				}
			}
			for (int i = 0; i < inputManager.Controllers.Length; i++)
			{
				if (inputManager.Controllers[i].PressedButtons.Start || inputManager.Controllers[i].PressedButtons.A)
				{
					if (this.ClickSound != null)
					{
						SoundManager.Instance.PlayInstance(this.ClickSound);
					}
					Screen.SelectedPlayerIndex = new PlayerIndex?((PlayerIndex)i);
					if (this.OnStartPressed != null)
					{
						this.OnStartPressed(this, new EventArgs());
					}
				}
				if (inputManager.Controllers[i].PressedButtons.Back)
				{
					if (this.ClickSound != null)
					{
						SoundManager.Instance.PlayInstance(this.ClickSound);
					}
					base.PopMe();
					Screen.SelectedPlayerIndex = new PlayerIndex?((PlayerIndex)i);
					if (this.OnBackPressed != null)
					{
						this.OnBackPressed(this, new EventArgs());
					}
				}
			}
			return base.OnInput(inputManager, gameTime);
		}
	}
}
