using System;
using DNA.Drawing.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public class SkinnedModelEntity : ModelEntity
	{
		protected Matrix[] _skinTransforms;

		/// <summary>
		/// 
		/// </summary>
		private SkinedAnimationData SkinningData =>
			(SkinedAnimationData)base.Model.Tag;

		/// <summary>
		/// 
		/// </summary>
		protected override Skeleton GetSkeleton() =>
			this.SkinningData.Skeleton;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public SkinnedModelEntity(Model model) 
			: base(model)
		{
			this._skinTransforms = new Matrix[this.SkinningData.Skeleton.Count];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected override void OnUpdate(GameTime gameTime)
		{
			base.OnUpdate(gameTime);
			this.UpdateSkinTransforms();
		}

		/// <summary>
		/// 
		/// </summary>
		private void UpdateSkinTransforms()
		{
			for (int i = 0; i < this._skinTransforms.Length; i++)
			{
				this._skinTransforms[i] = 
					this.SkinningData.InverseBindPose[i] * this._worldBoneTransforms[i];
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected override bool SetEffectParams(ModelMesh mesh, Effect effect, GameTime gameTime, 
												Matrix world, Matrix view, Matrix projection)
		{
			if (effect is SkinnedEffect)
			{
				SkinnedEffect skinned = (SkinnedEffect)effect;
				skinned.SetBoneTransforms(this._skinTransforms);
				skinned.EnableDefaultLighting();
				skinned.SpecularColor = new Vector3(0.25f);
				skinned.SpecularPower = 16f;
			}
			else if (effect.Parameters["Bones"] != null)
			{
				effect.Parameters["Bones"].SetValue(this._skinTransforms);
			}
			
			return base.SetEffectParams(
				mesh, effect, gameTime, Matrix.Identity, view, projection);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public override void Draw(
			GraphicsDevice device, GameTime gameTime, Matrix view, Matrix projection)
		{
			for (int i = 0; i < base.Model.Meshes.Count; i++)
			{
				ModelMesh mesh = base.Model.Meshes[i];
				
				for (int j = 0; j < mesh.Effects.Count; j++)
				{
					Effect effect = mesh.Effects[j];
					
					this.SetEffectParams(
						mesh, effect, gameTime, base.LocalToWorld, view, projection);
				}
				
				mesh.Draw();
			}

			if (this.ShowSkeleton)
			{
				base.DrawWireframeBones(device, view, projection);
			}
		}
	}
}
