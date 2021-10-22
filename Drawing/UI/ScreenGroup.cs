using System;
using System.Collections.Generic;
using System.Threading;
using DNA.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI
{
	public class ScreenGroup : Screen
	{
		private Stack<Screen> _screens = new Stack<Screen>();

		private Screen[] screensList = new Screen[0];

		public override bool CaptureMouse
		{
			get
			{
				lock (this)
				{
					for (int i = 0; i < this.screensList.Length; i++)
					{
						Screen screen = this.screensList[i];
						if (screen.AcceptInput)
						{
							return screen.CaptureMouse;
						}
						if (!screen.DrawBehind)
						{
							break;
						}
					}
				}
				return false;
			}
			set
			{
				throw new Exception("Cannot explictly set Caput Mouse Input on a Screen Group");
			}
		}

		public override bool ShowMouseCursor
		{
			get
			{
				lock (this)
				{
					for (int i = 0; i < this.screensList.Length; i++)
					{
						Screen screen = this.screensList[i];
						if (screen.AcceptInput)
						{
							return screen.ShowMouseCursor;
						}
						if (!screen.DrawBehind)
						{
							break;
						}
					}
				}
				return false;
			}
			set
			{
				throw new Exception("Cannot explictly set Caput Mouse Input on a Screen Group");
			}
		}

		public ScreenGroup(bool drawBehind) : base(false, drawBehind)
		{
		}

		public void ShowDialogScreen(DialogScreen screen, ThreadStart callback)
		{
			screen.Callback = callback;
			this.PushScreen(screen);
		}

		public void ShowPCDialogScreen(PCDialogScreen screen, ThreadStart callback)
		{
			screen.Callback = callback;
			this.PushScreen(screen);
		}

		public override bool AcceptInput
		{
			get
			{
				lock (this)
				{
					for (int i = 0; i < this.screensList.Length; i++)
					{
						Screen screen = this.screensList[i];
						if (screen.AcceptInput)
						{
							return true;
						}
						if (!screen.DrawBehind)
						{
							break;
						}
					}
				}
				return false;
			}
			set
			{
				throw new Exception("Cannot explictly set Accept Input on a Screen Group");
			}
		}

		public override bool Exiting
		{
			get
			{
				return this._screens.Count == 0 || base.Exiting;
			}
			set
			{
				base.Exiting = value;
			}
		}

		public void Clear()
		{
			while (this.PopScreen() != null)
			{
			}
		}

		public void PushScreen(Screen screen)
		{
			lock (this)
			{
				if (this._screens.Count > 0)
				{
					Screen screen2 = this._screens.Peek();
					screen2.OnLostFocus();
				}
				this._screens.Push(screen);
				screen.OnPushed();
				this.screensList = this._screens.ToArray();
			}
		}

		public Screen PopScreen()
		{
			Screen result;
			lock (this)
			{
				if (this._screens.Count == 0)
				{
					result = null;
				}
				else
				{
					Screen screen = this._screens.Pop();
					screen.OnPoped();
					screen.OnLostFocus();
					this.screensList = this._screens.ToArray();
					result = screen;
				}
			}
			return result;
		}

		public bool Contains(Screen screen)
		{
			return this._screens.Contains(screen);
		}

		public Screen CurrentScreen
		{
			get
			{
				if (this._screens.Count == 0)
				{
					return null;
				}
				return this._screens.Peek();
			}
		}

		public override bool ProcessChar(GameTime gameTime, char c)
		{
			bool result;
			lock (this)
			{
				bool flag2 = true;
				int num = 0;
				while (num < this.screensList.Length && flag2)
				{
					Screen screen = this.screensList[num];
					flag2 = (flag2 && screen.ProcessChar(gameTime, c));
					num++;
				}
				result = (flag2 && base.ProcessChar(gameTime, c));
			}
			return result;
		}

		public override bool ProcessInput(InputManager inputManager, GameTime gameTime)
		{
			bool result;
			lock (this)
			{
				bool flag2 = true;
				int num = 0;
				while (num < this.screensList.Length && flag2)
				{
					Screen screen = this.screensList[num];
					flag2 = (flag2 && screen.ProcessInput(inputManager, gameTime));
					num++;
				}
				result = (flag2 && base.ProcessInput(inputManager, gameTime));
			}
			return result;
		}

		public override void Update(DNAGame game, GameTime gameTime)
		{
			base.Update(game, gameTime);
			lock (this)
			{
				while (this._screens.Count != 0 && this._screens.Peek().Exiting)
				{
					Screen screen = this.PopScreen();
					screen.Exiting = false;
				}
				for (int i = 0; i < this.screensList.Length; i++)
				{
					Screen screen2 = this.screensList[i];
					screen2.Update(game, gameTime);
					if (!screen2.DrawBehind)
					{
						break;
					}
				}
			}
		}

		public override void Draw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			base.Draw(device, spriteBatch, gameTime);
			if (this._screens.Count == 0)
			{
				return;
			}
			int num = this.screensList.Length - 1;
			for (int i = 0; i < this.screensList.Length; i++)
			{
				Screen screen = this.screensList[i];
				if (!screen.DrawBehind)
				{
					num = i;
					break;
				}
			}
			for (int j = num; j >= 0; j--)
			{
				Screen screen2 = this.screensList[j];
				screen2.Draw(device, spriteBatch, gameTime);
			}
		}
	}
}
