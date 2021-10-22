using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public class BltTargetView : View
	{
		private RenderTarget2D _offScreenBuffer;
		private int _downsample;
		private bool _mipMap;

		public RenderTarget2D OffScreenTarget => 
			this._offScreenBuffer;

		private void SetDestinationTargetInternal(RenderTarget2D destinationTarget)
		{
			int num;
			int num2;
			SurfaceFormat preferredFormat;
			DepthFormat depthStencilFormat;
			int multiSampleCount;
		
			if (destinationTarget == null)
			{
				PresentationParameters presentationParameters = 
					base.Game.GraphicsDevice.PresentationParameters;
		
				num = presentationParameters.BackBufferWidth;
				num2 = presentationParameters.BackBufferHeight;
				preferredFormat = presentationParameters.BackBufferFormat;
				depthStencilFormat = presentationParameters.DepthStencilFormat;
				multiSampleCount = presentationParameters.MultiSampleCount;
			}
			else
			{
				num = destinationTarget.Width;
				num2 = destinationTarget.Height;
				preferredFormat = destinationTarget.Format;
				depthStencilFormat = destinationTarget.DepthStencilFormat;
				multiSampleCount = destinationTarget.MultiSampleCount;
			}
	
			if (this._offScreenBuffer != null && !this._offScreenBuffer.IsDisposed)
			{
				this._offScreenBuffer.Dispose();
			}
		
			this._offScreenBuffer = new RenderTarget2D(base.Game.GraphicsDevice, 
				num / this._downsample, num2 / this._downsample, this._mipMap, preferredFormat, 
				depthStencilFormat, multiSampleCount, RenderTargetUsage.DiscardContents);
		}

		public override void SetDestinationTarget(RenderTarget2D destinationTarget)
		{
			base.SetDestinationTarget(destinationTarget);
			this.SetDestinationTargetInternal(destinationTarget);
		}

		public BltTargetView(Game game, RenderTarget2D destinationTarget, 
							 int downsample, bool mipmap) 
			: base(game, destinationTarget)
		{
			this._mipMap = mipmap;
			this._downsample = downsample;
			this.SetDestinationTargetInternal(destinationTarget);
		}

		protected override void OnDraw(GraphicsDevice device, 
									   SpriteBatch spriteBatch, 
									   GameTime gameTime)
		{
			Viewport viewport = device.Viewport;
			base.SetRenderTargetToDevice(device);
			
			this.DrawFullscreenQuad(spriteBatch, this.OffScreenTarget, 
				viewport.Width, viewport.Height, null);
		}

		protected void DrawFullscreenQuad(SpriteBatch spriteBatch, Texture2D texture, 
										  RenderTarget2D renderTarget, Effect effect)
		{
			spriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);
			
			this.DrawFullscreenQuad(spriteBatch, texture, 
				renderTarget.Width, renderTarget.Height, effect);
		}

		protected void DrawFullscreenQuad(SpriteBatch spriteBatch, Texture2D texture, 
										  int width, int height, Effect effect)
		{
			spriteBatch.Begin(SpriteSortMode.Deferred, 
				BlendState.Opaque, null, null, null, effect);
			
			spriteBatch.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
			spriteBatch.End();
		}
	}
}
