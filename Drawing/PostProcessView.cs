using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public class PostProcessView : BltTargetView
	{
		private RenderTarget2D _renderTarget1;

		private RenderTarget2D _renderTarget2;

		private RenderTarget2D _lastFrame;

		private Effect bloomExtractEffect;

		private Effect bloomCombineEffect;

		private Effect gaussianBlurEffect;

		public BloomSettings BloomSettings = BloomSettings.Default;

		private float[] sampleWeights = new float[0];

		private Vector2[] sampleOffsets = new Vector2[0];

		public RenderTarget2D Target1
		{
			get
			{
				return this._renderTarget1;
			}
		}

		public RenderTarget2D Target2
		{
			get
			{
				return this._renderTarget2;
			}
		}

		public Texture2D LastFrame
		{
			get
			{
				return this._lastFrame;
			}
		}

		private void SetDestinationTargetInternal(RenderTarget2D destinationTarget)
		{
			GraphicsDevice graphicsDevice = base.Game.GraphicsDevice;
			int num;
			int num2;
			SurfaceFormat preferredFormat;
			DepthFormat depthStencilFormat;
			if (destinationTarget == null)
			{
				PresentationParameters presentationParameters = graphicsDevice.PresentationParameters;
				num = presentationParameters.BackBufferWidth;
				num2 = presentationParameters.BackBufferHeight;
				preferredFormat = presentationParameters.BackBufferFormat;
				depthStencilFormat = presentationParameters.DepthStencilFormat;
				int multiSampleCount = presentationParameters.MultiSampleCount;
			}
			else
			{
				num = destinationTarget.Width;
				num2 = destinationTarget.Height;
				preferredFormat = destinationTarget.Format;
				depthStencilFormat = destinationTarget.DepthStencilFormat;
				int multiSampleCount2 = destinationTarget.MultiSampleCount;
			}
			if (this._renderTarget1 != null && !this._renderTarget1.IsDisposed)
			{
				this._renderTarget1.Dispose();
			}
			if (this._renderTarget2 != null && !this._renderTarget2.IsDisposed)
			{
				this._renderTarget1.Dispose();
			}
			if (this._lastFrame != null && !this._lastFrame.IsDisposed)
			{
				this._lastFrame.Dispose();
			}
			num /= 2;
			num2 /= 2;
			this._renderTarget1 = new RenderTarget2D(graphicsDevice, num, num2, false, preferredFormat, depthStencilFormat, 1, RenderTargetUsage.DiscardContents);
			this._renderTarget2 = new RenderTarget2D(graphicsDevice, num, num2, false, preferredFormat, DepthFormat.None, 1, RenderTargetUsage.DiscardContents);
			if (graphicsDevice.GraphicsProfile == GraphicsProfile.HiDef)
			{
				this._lastFrame = new RenderTarget2D(graphicsDevice, num, num2, true, preferredFormat, DepthFormat.None, 1, RenderTargetUsage.DiscardContents);
				return;
			}
			int width = 1 << (int)(Math.Log((double)num) / Math.Log(2.0));
			int height = 1 << (int)(Math.Log((double)num2) / Math.Log(2.0));
			this._lastFrame = new RenderTarget2D(graphicsDevice, width, height, false, preferredFormat, DepthFormat.None, 1, RenderTargetUsage.DiscardContents);
		}

		public override void SetDestinationTarget(RenderTarget2D destinationTarget)
		{
			base.SetDestinationTarget(destinationTarget);
			this.SetDestinationTargetInternal(destinationTarget);
		}

		public PostProcessView(Game game, RenderTarget2D destinationTarget) : base(game, destinationTarget, 1, false)
		{
			this.bloomExtractEffect = base.Game.Content.Load<Effect>("PostProcess\\BloomExtract");
			this.bloomCombineEffect = base.Game.Content.Load<Effect>("PostProcess\\BloomCombine");
			this.gaussianBlurEffect = base.Game.Content.Load<Effect>("PostProcess\\GaussianBlur");
			this.SetDestinationTargetInternal(destinationTarget);
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			device.SamplerStates[1] = SamplerState.LinearClamp;
			this.bloomExtractEffect.Parameters["BloomThreshold"].SetValue(this.BloomSettings.BloomThreshold);
			base.DrawFullscreenQuad(spriteBatch, base.OffScreenTarget, this._renderTarget1, this.bloomExtractEffect);
			this.SetBlurEffectParameters(1f / (float)this._renderTarget1.Width, 0f);
			base.DrawFullscreenQuad(spriteBatch, this._renderTarget1, this._renderTarget2, this.gaussianBlurEffect);
			this.SetBlurEffectParameters(0f, 1f / (float)this._renderTarget1.Height);
			base.DrawFullscreenQuad(spriteBatch, this._renderTarget2, this._renderTarget1, this.gaussianBlurEffect);
			EffectParameterCollection parameters = this.bloomCombineEffect.Parameters;
			parameters["BloomIntensity"].SetValue(this.BloomSettings.BloomIntensity);
			parameters["BaseIntensity"].SetValue(this.BloomSettings.BaseIntensity);
			parameters["BloomSaturation"].SetValue(this.BloomSettings.BloomSaturation);
			parameters["BaseSaturation"].SetValue(this.BloomSettings.BaseSaturation);
			device.Textures[1] = base.OffScreenTarget;
			base.DrawFullscreenQuad(spriteBatch, this._renderTarget1, this._renderTarget2, this.bloomCombineEffect);
			device.SetRenderTarget(this._lastFrame);
			Viewport viewport = device.Viewport;
			base.DrawFullscreenQuad(spriteBatch, this._renderTarget1, viewport.Width, viewport.Height, this.bloomCombineEffect);
			base.SetRenderTargetToDevice(device);
			viewport = device.Viewport;
			base.DrawFullscreenQuad(spriteBatch, this._renderTarget1, viewport.Width, viewport.Height, this.bloomCombineEffect);
		}

		private void SetBlurEffectParameters(float dx, float dy)
		{
			EffectParameter effectParameter = this.gaussianBlurEffect.Parameters["SampleWeights"];
			EffectParameter effectParameter2 = this.gaussianBlurEffect.Parameters["SampleOffsets"];
			int count = effectParameter.Elements.Count;
			if (this.sampleWeights.Length != count)
			{
				this.sampleWeights = new float[count];
			}
			if (this.sampleOffsets.Length != count)
			{
				this.sampleOffsets = new Vector2[count];
			}
			this.sampleWeights[0] = this.ComputeGaussian(0f);
			this.sampleOffsets[0] = new Vector2(0f);
			float num = this.sampleWeights[0];
			for (int i = 0; i < count / 2; i++)
			{
				float num2 = this.ComputeGaussian((float)(i + 1));
				this.sampleWeights[i * 2 + 1] = num2;
				this.sampleWeights[i * 2 + 2] = num2;
				num += num2 * 2f;
				float scaleFactor = (float)(i * 2) + 1.5f;
				Vector2 vector = new Vector2(dx, dy) * scaleFactor;
				this.sampleOffsets[i * 2 + 1] = vector;
				this.sampleOffsets[i * 2 + 2] = -vector;
			}
			for (int j = 0; j < this.sampleWeights.Length; j++)
			{
				this.sampleWeights[j] /= num;
			}
			effectParameter.SetValue(this.sampleWeights);
			effectParameter2.SetValue(this.sampleOffsets);
		}

		private float ComputeGaussian(float n)
		{
			float blurAmount = this.BloomSettings.BlurAmount;
			return (float)(1.0 / Math.Sqrt(6.2831853071795862 * (double)blurAmount) * Math.Exp((double)(-(double)(n * n) / (2f * blurAmount * blurAmount))));
		}
	}
}
