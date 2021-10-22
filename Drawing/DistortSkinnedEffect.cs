using System;
using DNA.Drawing.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public class DistortSkinnedEffect : DNAEffect
	{
		public const int MaxBones = 72;
		private const float blurAmount = 2f;

		private float _distortionScale = 0.1f;

		private EffectParameter textureParam;
		private EffectParameter bonesParam;
		private EffectParameter shaderIndexParam;
		private EffectParameter distortionScaleParam;

		private float alpha = 1f;

		public bool Blur = true;

		private EffectTechnique distortTechnique;
		private EffectTechnique distortBlurTechnique;

		private int weightsPerVertex = 4;

		public float DistortionScale
		{
			get =>
				this._distortionScale;
			
			set =>
				this._distortionScale = value;
		}

		public float Alpha
		{
			get =>
				this.alpha;

			set =>
				this.alpha = value;
		}

		public Texture2D Texture
		{
			get =>
				this.textureParam.GetValueTexture2D();
			
			set =>
				this.textureParam.SetValue(value);
		}

		public int WeightsPerVertex
		{
			get =>
				this.weightsPerVertex;

			set
			{
				if (value != 1 && value != 2 && value != 4)
				{
					throw new ArgumentOutOfRangeException("value");
				}

				this.weightsPerVertex = value;
			}
		}

		private void SetBlurEffectParameters(float dx, float dy)
		{
			EffectParameter sampleWeights = base.Parameters["SampleWeights"];
			EffectParameter sampleOffsets = base.Parameters["SampleOffsets"];
			
			int count = sampleWeights.Elements.Count;
			
			float[] newWeights = new float[count];
			Vector2[] newOffsets = new Vector2[count];
			
			newWeights[0] = DistortSkinnedEffect.ComputeGaussian(0f);
			newOffsets[0] = new Vector2(0f);
			
			float firstWeight = newWeights[0];
			
			for (int i = 0; i < count / 2; i++)
			{
				float gaussian = DistortSkinnedEffect.ComputeGaussian((float)(i + 1));
				newWeights[i * 2 + 1] = gaussian;
				newWeights[i * 2 + 2] = gaussian;
				firstWeight += gaussian * 2f;
				
				float scaleFactor = (float)(i * 2) + 1.5f;
				Vector2 offset = new Vector2(dx, dy) * scaleFactor;
				newOffsets[i * 2 + 1] = offset;
				newOffsets[i * 2 + 2] = -offset;
			}
			
			for (int j = 0; j < newWeights.Length; j++)
			{
				newWeights[j] /= firstWeight;
			}

			sampleWeights.SetValue(newWeights);
			sampleOffsets.SetValue(newOffsets);
		}

		private static float ComputeGaussian(float n) =>
			(float)(1.0 / Math.Sqrt(12.566370614359173) * 
				Math.Exp((double)(-(double)(n * n) / 8f)));

		public void SetBoneTransforms(Matrix[] boneTransforms)
		{
			if (boneTransforms == null || boneTransforms.Length == 0)
			{
				throw new ArgumentNullException("boneTransforms");
			}
			
			if (boneTransforms.Length > 72)
			{
				throw new ArgumentException();
			}

			this.bonesParam.SetValue(boneTransforms);
		}

		public Matrix[] GetBoneTransforms(int count)
		{
			if (count <= 0 || count > 72)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			
			Matrix[] boneTransforms = this.bonesParam.GetValueMatrixArray(count);
			
			for (int i = 0; i < boneTransforms.Length; i++)
			{
				boneTransforms[i].M44 = 1f;
			}

			return boneTransforms;
		}

		public DistortSkinnedEffect(Game game) 
			: base(game.Content.Load<Effect>("DistortSkinnedEffect"))
		{
			this.CacheEffectParameters(null);
			Matrix[] boneTransforms = new Matrix[72];
		
			for (int i = 0; i < 72; i++)
			{
				boneTransforms[i] = Matrix.Identity;
			}
		
			this.SetBoneTransforms(boneTransforms);
		}

		protected DistortSkinnedEffect(DistortSkinnedEffect cloneSource) 
			: base(cloneSource)
		{
			this.CacheEffectParameters(cloneSource);
			this.Blur = cloneSource.Blur;
			this.alpha = cloneSource.alpha;
			this._distortionScale = cloneSource._distortionScale;
			this.weightsPerVertex = cloneSource.weightsPerVertex;
		}

		public override Effect Clone() =>
			new DistortSkinnedEffect(this);

		private void CacheEffectParameters(DistortSkinnedEffect cloneSource)
		{
			this.textureParam = base.Parameters["Texture"];
			this.distortionScaleParam = base.Parameters["DistortionScale"];
			this.bonesParam = base.Parameters["Bones"];
			this.shaderIndexParam = base.Parameters["ShaderIndex"];
			this.distortTechnique = base.Techniques["Distort"];
			this.distortBlurTechnique = base.Techniques["DistortBlur"];
			
			PresentationParameters presentationParameters = 
				base.GraphicsDevice.PresentationParameters;
			
			int backBufferWidth = presentationParameters.BackBufferWidth;
			int backBufferHeight = presentationParameters.BackBufferHeight;
			
			SurfaceFormat backBufferFormat = presentationParameters.BackBufferFormat;
			DepthFormat depthStencilFormat = presentationParameters.DepthStencilFormat;
			
			this.SetBlurEffectParameters(
				1f / (float)backBufferWidth, 1f / (float)backBufferHeight);
		}

		protected override void OnApply()
		{
			int newVal = 0;
			
			if (this.weightsPerVertex == 2)
			{
				newVal++;
			}
			else if (this.weightsPerVertex == 4)
			{
				newVal += 2;
			}
			
			this.shaderIndexParam.SetValue(newVal);
			this.distortionScaleParam.SetValue(this._distortionScale);
			
			base.CurrentTechnique = 
				(this.Blur ? this.distortBlurTechnique : this.distortTechnique);
			
			base.OnApply();
		}
	}
}
