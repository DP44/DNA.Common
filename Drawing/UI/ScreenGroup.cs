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
			
			set =>
				throw new Exception("Cannot explictly set Caput Mouse Input on a Screen Group");
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

			set =>
				throw new Exception("Cannot explictly set Caput Mouse Input on a Screen Group");
		}

		public ScreenGroup(bool drawBehind) 
			: base(false, drawBehind) {}

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

			set => 
				throw new Exception("Cannot explictly set Accept Input on a Screen Group");
		}

		public override bool Exiting
		{
			get => 
				this._screens.Count == 0 || base.Exiting;
		
			set => 
				base.Exiting = value;
		}

		public void Clear()
		{
			while (this.PopScreen() != null);
		}

		public void PushScreen(Screen screen)
		{
			lock (this)
			{
				if (this._screens.Count > 0)
				{
					this._screens.Peek().OnLostFocus();
				}

				this._screens.Push(screen);
				screen.OnPushed();
				this.screensList = this._screens.ToArray();
			}
		}

		public Screen PopScreen()
		{
			lock (this)
			{
				if (this._screens.Count == 0)
				{
					return null;
				}

				Screen screen = this._screens.Pop();
				screen.OnPoped();
				screen.OnLostFocus();
				this.screensList = this._screens.ToArray();
				return screen;
			}
		}

		public bool Contains(Screen screen) => 
			this._screens.Contains(screen);

		public Screen CurrentScreen => 
			this._screens.Count == 0 ? (Screen)null : this._screens.Peek();

		public override bool ProcessChar(GameTime gameTime, char c)
		{
			lock (this)
			{
				bool flag = true;
				
				for (int i = 0; i < this.screensList.Length && flag; i++)
				{
					Screen screen = this.screensList[i];
					flag = flag && screen.ProcessChar(gameTime, c);
				}

				return flag && base.ProcessChar(gameTime, c);
			}
		}

		public override bool ProcessInput(InputManager inputManager, GameTime gameTime)
		{
			lock (this)
			{
				bool flag = true;
				
				for (int i = 0; i < this.screensList.Length && flag; i++)
				{
					Screen screen = this.screensList[i];
					flag = flag && screen.ProcessInput(inputManager, gameTime);
				}

				return flag && base.ProcessInput(inputManager, gameTime);
			}
		}

		public override void Update(DNAGame game, GameTime gameTime)
		{
			base.Update(game, gameTime);
		
			lock (this)
			{
				while (this._screens.Count != 0 && this._screens.Peek().Exiting)
				{
					this.PopScreen().Exiting = false;
				}

				for (int i = 0; i < this.screensList.Length; i++)
				{
					Screen screen = this.screensList[i];

					screen.Update(game, gameTime);
					if (!screen.DrawBehind)
					{
						break;
					}
				}
			}
		}

		public override void Draw(GraphicsDevice device, 
								  SpriteBatch spriteBatch, 
								  GameTime gameTime)
		{
			base.Draw(device, spriteBatch, gameTime);

			if (this._screens.Count == 0)
			{
				return;
			}
			
			int num = this.screensList.Length - 1;
			
			for (int i = 0; i < this.screensList.Length; i++)
			{
				if (!this.screensList[i].DrawBehind)
				{
					num = i;
					break;
				}
			}
			
			for (int j = num; j >= 0; j--)
			{
				this.screensList[j].Draw(device, spriteBatch, gameTime);
			}
		}
	}
}
