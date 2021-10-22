using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.Effects
{
	public class HeatHazeEffect : DNAEffect
	{
		private float _waveMagnitude = 0.2f;
		private EffectParameter displaceTextureParam;
		private EffectParameter screenTextureParam;
		private EffectParameter waveMagnitudeParam;
		private Texture2D heatMap;

		/// <summary>
		/// 
		/// </summary>
		public float WaveMagnitude
		{
			get =>
				this._waveMagnitude;

			set =>
				this._waveMagnitude = value;
		}

		/// <summary>
		/// 
		/// </summary>
		public Texture2D DisplacementTexture
		{
			get =>
				this.displaceTextureParam.GetValueTexture2D();

			set =>
				this.displaceTextureParam.SetValue(value);
		}

		/// <summary>
		/// 
		/// </summary>
		public Texture2D ScreenTexture
		{
			get =>
				this.screenTextureParam.GetValueTexture2D();

			set =>
				this.screenTextureParam.SetValue(value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public HeatHazeEffect(Game game) 
			: base(game.Content.Load<Effect>("HeatHaze"))
		{
			this.heatMap = game.Content.Load<Texture2D>("HeatNormal");
			this.CacheEffectParameters(null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected HeatHazeEffect(HeatHazeEffect cloneSource) : base(cloneSource)
		{
			this.CacheEffectParameters(cloneSource);
			this._waveMagnitude = cloneSource._waveMagnitude;
			this.heatMap = cloneSource.heatMap;
		}

		/// <summary>
		/// 
		/// </summary>
		public override Effect Clone() =>
			(Effect)new HeatHazeEffect(this);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		private void CacheEffectParameters(HeatHazeEffect cloneSource)
		{
			this.displaceTextureParam = base.Parameters["DisplacementMap"];
			this.screenTextureParam = base.Parameters["ScreenMap"];
			this.waveMagnitudeParam = base.Parameters["WaveMagnitude"];
			this.displaceTextureParam.SetValue(this.heatMap);
		}

		/// <summary>
		/// 
		/// </summary>
		protected override void OnApply()
		{
			this.waveMagnitudeParam.SetValue(this._waveMagnitude);
			base.OnApply();
		}
	}
}
