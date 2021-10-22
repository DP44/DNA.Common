using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public class View
	{
		public bool Enabled = true;
		private RenderTarget2D _target;
		private Game _game;
		public Color? BackgroundColor = null;
		public Texture2D BackgroundImage;
		private DrawEventArgs args = new DrawEventArgs();

		/// <summary>
		/// 
		/// </summary>
		public RenderTarget2D Target => 
			this._target;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public virtual void SetDestinationTarget(RenderTarget2D target) =>
			this._target = target;

		/// <summary>
		/// 
		/// </summary>
		public Game Game => this._game;

		/// <summary>
		/// Constructor.
		/// </summary>
		public View() {}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name=""></param>
		public View(Game game, RenderTarget2D target)
		{
			this._game = game;
			this._target = target;
		}

		public event EventHandler<DrawEventArgs> BeforeDraw;
		public event EventHandler<DrawEventArgs> AfterDraw;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected void SetRenderTargetToDevice(GraphicsDevice device) =>
			device.SetRenderTarget(this._target);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected virtual void OnDraw(
			GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime) {}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void Draw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			this.args.Device = device;
			this.args.GameTime = gameTime;
			device.SetRenderTarget(this._target);
			
			if (this.BackgroundImage != null)
			{
				if (this._target == null || 
					this._target.DepthStencilFormat != DepthFormat.None)
				{
					device.Clear(ClearOptions.DepthBuffer, Color.Red, 1f, 0);
				}

				int width = device.Viewport.Width;
				int height = device.Viewport.Height;
				int num = this.BackgroundImage.Width * height / this.BackgroundImage.Height;
				int num2 = num - width;
				int num3 = num2 / 2;
				
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, 
								  SamplerState.AnisotropicClamp, DepthStencilState.None, 
								  RasterizerState.CullNone);
				
				spriteBatch.Draw(this.BackgroundImage, 
					new Rectangle(-num3, 0, num, height), 
					new Rectangle?(
						new Rectangle(0, 0, 
							this.BackgroundImage.Width, this.BackgroundImage.Height)), 
					Color.White);
				
				spriteBatch.End();
			}
			else if (this.BackgroundColor != null)
			{
				if (this._target == null || 
					this._target.DepthStencilFormat != DepthFormat.None)
				{
					device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, 
						this.BackgroundColor.Value, 1f, 0);
				}
				else
				{
					device.Clear(ClearOptions.Target, this.BackgroundColor.Value, 1f, 0);
				}
			}
			else if (this._target == null || 
					 this._target.DepthStencilFormat != DepthFormat.None)
			{
				device.Clear(ClearOptions.DepthBuffer, Color.CornflowerBlue, 1f, 0);
			}
			
			if (this.BeforeDraw != null)
			{
				this.BeforeDraw(this, this.args);
			}
			
			this.OnDraw(device, spriteBatch, gameTime);
			
			if (this.AfterDraw != null)
			{
				this.AfterDraw(this, this.args);
			}
		}
	}
}
