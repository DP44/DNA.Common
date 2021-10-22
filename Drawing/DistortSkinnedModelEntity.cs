using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public class DistortSkinnedModelEntity : SkinnedModelEntity, IScreenDistortion
	{
		private Texture2D _backgroundImage;
		private float _distortionScale = 0.1f;
		private bool _blur;

		/// <summary>
		/// 
		/// </summary>
		public Texture2D ScreenBackground
		{
			get =>
				this._backgroundImage;

			set =>
				this._backgroundImage = value;
		}

		/// <summary>
		/// 
		/// </summary>
		public float DistortionScale
		{
			get =>
				this._distortionScale;

			set =>
				this._distortionScale = value;
		}

		/// <summary>
		/// 
		/// </summary>
		public bool Blur
		{
			get =>
				this._blur;

			set =>
				this._blur = value;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public DistortSkinnedModelEntity(Game game, Model model, Texture2D backgroundImage) 
			: base(model)
		{
			this._backgroundImage = backgroundImage;
			this.AlphaSort = true;
			
			foreach (ModelMesh modelMesh in model.Meshes)
			{
				foreach (ModelMeshPart modelMeshPart in modelMesh.MeshParts)
				{
					modelMeshPart.Effect = new DistortSkinnedEffect(game);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected override bool SetEffectParams(ModelMesh mesh, Effect effect, GameTime gameTime, 
												Matrix world, Matrix view, Matrix projection)
		{
			DistortSkinnedEffect distortSkinnedEffect = (DistortSkinnedEffect)effect;
			distortSkinnedEffect.Texture = this._backgroundImage;
			distortSkinnedEffect.SetBoneTransforms(this._skinTransforms);
			distortSkinnedEffect.DistortionScale = this._distortionScale;
			distortSkinnedEffect.Blur = this._blur;
			return base.SetEffectParams(mesh, effect, gameTime, world, view, projection);
		}
	}
}
