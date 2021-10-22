using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public class SpriteEffect : Effect
	{
		private EffectParameter matrixParam;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public SpriteEffect(Effect effect) : base(effect)
		{
			this.CacheEffectParameters();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected SpriteEffect(SpriteEffect cloneSource) 
			: base((Effect)cloneSource)
		{
			this.CacheEffectParameters();
		}

		/// <summary>
		/// 
		/// </summary>
		public override Effect Clone() =>
			new SpriteEffect(this);

		/// <summary>
		/// 
		/// </summary>
		private void CacheEffectParameters() =>
			this.matrixParam = base.Parameters["MatrixTransform"];

		/// <summary>
		/// 
		/// </summary>
		protected override void OnApply()
		{
			Viewport viewport = base.GraphicsDevice.Viewport;
			
			Matrix matrix = Matrix.CreateOrthographicOffCenter(
				0f, (float)viewport.Width, (float)viewport.Height, 0f, 0f, 1f);
			
			Matrix matrix2 = Matrix.CreateTranslation(-0.5f, -0.5f, 0f);
			this.matrixParam.SetValue(matrix2 * matrix);
		}
	}
}
