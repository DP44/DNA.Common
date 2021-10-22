using System;
using DNA.Input;
using DNA.Net.GamerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI
{
	public class Screen
	{
		public static ScreenAdjuster Adjuster = new ScreenAdjuster();

		private bool _captureMouse;
		private bool _showMouseCursor = true;
		public Color? BackgroundColor = null;
		public Texture2D BackgroundImage;
		private static PlayerIndex? _selectedPlayerIndex = null;
		private bool _drawBehind;
		private bool _acceptInput = true;
		private bool _doUpdate = true;
		protected bool _mouseActive;

		private UpdateEventArgs _updateEventArgs = new UpdateEventArgs();
		private ControllerInputEventArgs _controllerEventArgs = new ControllerInputEventArgs();
		private DrawEventArgs args = new DrawEventArgs();

		public virtual bool CaptureMouse
		{
			get => 
				this._captureMouse;

			set => 
				this._captureMouse = value;
		}

		public virtual bool ShowMouseCursor
		{
			get => 
				this._showMouseCursor;

			set => 
				this._showMouseCursor = value;
		}

		static Screen()
		{
			SignedInGamer.SignedIn += Screen.SignedInGamer_SignedIn;
			SignedInGamer.SignedOut += Screen.SignedInGamer_SignedOut;
		}

		public static PlayerIndex? SelectedPlayerIndex
		{
			get => 
				Screen._selectedPlayerIndex;

			set => 
				Screen._selectedPlayerIndex = value;
		}

		public static SignedInGamer CurrentGamer => 
			!Screen._selectedPlayerIndex.HasValue 
				? (SignedInGamer)null 
				: Gamer.SignedInGamers[Screen._selectedPlayerIndex.Value];

		public virtual bool Exiting { get; set; }

		public bool DrawBehind =>
			this.BackgroundColor == null && this._drawBehind;

		public virtual bool AcceptInput
		{
			get => 
				this._acceptInput;

			set => 
				this._acceptInput = value;
		}

		public virtual bool DoUpdate
		{
			get => 
				this._doUpdate;

			set => 
				this._doUpdate = value;
		}

		public Screen(bool acceptInput, bool drawBehind)
		{
			this._acceptInput = acceptInput;
			this._drawBehind = drawBehind;
		}

		public event EventHandler<EventArgs> LostFocus;

		public virtual void OnLostFocus()
		{
			this._mouseActive = false;
			
			if (this.LostFocus == null)
			{
				return;
			}

			this.LostFocus((object)this, new EventArgs());
		}

		public event EventHandler<UpdateEventArgs> Updating;

		protected virtual void OnUpdate(DNAGame game, GameTime gameTime) {}

		public virtual void Update(DNAGame game, GameTime gameTime)
		{
			// Make sure we're allowed to update first.
			if (!this.DoUpdate)
			{
				return;
			}

			if (this.Updating != null)
			{
				this._updateEventArgs.GameTime = gameTime;
				this.Updating(this, this._updateEventArgs);
			}

			this.OnUpdate(game, gameTime);
		}

		public event EventHandler<CharEventArgs> ProcessingChar;
		public event EventHandler<InputEventArgs> ProcessingInput;

		protected virtual bool OnInput(InputManager inputManager, GameTime gameTime) => 
			!this.AcceptInput;

		protected virtual bool OnChar(GameTime gameTime, char c) => 
			!this.AcceptInput;

		public virtual bool ProcessChar(GameTime gameTime, char c)
		{
			if (!this.AcceptInput)
			{
				return true;
			}
			
			bool shouldProcess = this.OnChar(gameTime, c);
			
			if (this.ProcessingChar != null)
			{
				CharEventArgs charEventArgs = new CharEventArgs(c, gameTime, shouldProcess);
				this.ProcessingChar(this, charEventArgs);
				shouldProcess = charEventArgs.ContiuneProcessing;
			}
			
			return shouldProcess;
		}

		public virtual bool ProcessInput(InputManager inputManager, GameTime gameTime)
		{
			if (!this.AcceptInput)
			{
				return true;
			}
		
			bool shouldProcess = this.OnInput(inputManager, gameTime);
		
			if (this.ProcessingInput != null)
			{
				InputEventArgs inputEventArgs = 
					new InputEventArgs(inputManager, gameTime, shouldProcess);
		
				this.ProcessingInput(this, inputEventArgs);
				shouldProcess = inputEventArgs.ContiuneProcessing;
			}
		
			if (Screen._selectedPlayerIndex != null)
			{
				shouldProcess = (shouldProcess || this.ProcessInput(
					inputManager, inputManager.Controllers[(int)Screen._selectedPlayerIndex.Value], 
					inputManager.ChatPads[(int)Screen._selectedPlayerIndex.Value], gameTime));
			}
		
			return shouldProcess;
		}

		public event EventHandler<ControllerInputEventArgs> ProcessingPlayerInput;

		protected virtual bool OnPlayerInput(InputManager inputManager, GameController controller, 
											 KeyboardInput chatPad, GameTime gameTime)
		{
			if (controller.Activity)
			{
				this._mouseActive = false;
			}

			return !this.AcceptInput;
		}

		protected virtual bool ProcessInput(InputManager inputManager, GameController controller, 
											KeyboardInput chatPad, GameTime gameTime)
		{
			bool shouldProcess = 
				this.OnPlayerInput(inputManager, controller, chatPad, gameTime);
			
			if (this.ProcessingPlayerInput != null)
			{
				this._controllerEventArgs.Mouse = inputManager.Mouse;
				this._controllerEventArgs.Keyboard = inputManager.Keyboard;
				this._controllerEventArgs.Chatpad = chatPad;
				this._controllerEventArgs.Controller = controller;
				this._controllerEventArgs.GameTime = gameTime;
				this._controllerEventArgs.ContinueProcessing = shouldProcess;
				this.ProcessingPlayerInput(this, this._controllerEventArgs);
				
				shouldProcess = 
					(shouldProcess || this._controllerEventArgs.ContinueProcessing);
			}
			
			return shouldProcess;
		}

		public event EventHandler<DrawEventArgs> BeforeDraw;
		public event EventHandler<DrawEventArgs> AfterDraw;

		protected virtual void OnDraw(
			GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime) {}

		public virtual void Draw(
			GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			this.args.Device = device;
			this.args.GameTime = gameTime;
			
			if (this.BeforeDraw != null)
			{
				this.BeforeDraw(this, this.args);
			}
			
			if (this.BackgroundImage != null)
			{
				device.Clear(ClearOptions.DepthBuffer, Color.Red, 1f, 0);
				int width = device.Viewport.Width;
				int height = device.Viewport.Height;
				int num = width * this.BackgroundImage.Height / height;
				int num2 = num - width;
				int num3 = num2 / 2;
				
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, 
								  SamplerState.AnisotropicClamp, DepthStencilState.None, 
								  RasterizerState.CullNone);
				
				spriteBatch.Draw(this.BackgroundImage, 
					new Rectangle(-num3, 0, num, height), 
					new Rectangle?(
						new Rectangle(0, 0, this.BackgroundImage.Width, 
									  this.BackgroundImage.Height)), 
					Color.White);
				
				spriteBatch.End();
			}
			else if (this.BackgroundColor != null)
			{
				device.Clear(
					ClearOptions.Target | ClearOptions.DepthBuffer, 
					this.BackgroundColor.Value, 1f, 0);
			}
			else
			{
				device.Clear(ClearOptions.DepthBuffer, Color.CornflowerBlue, 1f, 0);
			}
			
			this.OnDraw(device, spriteBatch, gameTime);
			
			if (this.AfterDraw != null)
			{
				this.AfterDraw(this, this.args);
			}
		}

		public event EventHandler Pushed;
		public event EventHandler Poped;

		public virtual void OnPushed()
		{
			this.Exiting = false;
			
			if (this.Pushed == null)
			{
				return;
			}

			this.Pushed((object)this, new EventArgs());
		}

		public virtual void OnPoped()
		{
			if (this.Poped == null)
			{
				return;
			}

			this.Poped((object)this, new EventArgs());
		}

		public void PopMe() => 
			this.Exiting = true;

		public static event EventHandler<SignedOutEventArgs> PlayerSignedOut;
		public static event EventHandler<SignedInEventArgs> PlayerSignedIn;

		private static void SignedInGamer_SignedOut(object sender, SignedOutEventArgs e)
		{
			if (Screen._selectedPlayerIndex != null && 
				e.Gamer.PlayerIndex == Screen._selectedPlayerIndex.Value && 
				Screen.PlayerSignedOut != null)
			{
				Screen.PlayerSignedOut(sender, e);
			}
		}

		private static void SignedInGamer_SignedIn(object sender, SignedInEventArgs e)
		{
			if (Screen._selectedPlayerIndex != null && 
				e.Gamer.PlayerIndex == Screen._selectedPlayerIndex.Value && 
				Screen.PlayerSignedIn != null)
			{
				Screen.PlayerSignedIn(sender, e);
			}
		}
	}
}
