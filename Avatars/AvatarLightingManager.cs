using System;
using Microsoft.Xna.Framework;

namespace DNA.Avatars 
{
	public abstract class AvatarLightingManager 
	{
		private Vector3 _lightDirection = new Vector3(-0.5f, -0.6123f, -0.6123f);
		private Color _lightColor = new Color(0.4f, 0.4f, 0.4f);
		private Color _ambientLightColor = new Color(0.55f, 0.55f, 0.55f);
		
		/// <summary>
		/// 
		/// </summary>
		public Vector3 LightDirection
		{
			get => 
				this._lightDirection;
			
			set => 
				this._lightDirection = value;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public Color LightColor
		{
			get => 
				this._lightColor;
			
			set => 
				this._lightColor = value;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public Color AmbientLightColor
		{
			get => 
				this._ambientLightColor;
			
			set => 
				this._ambientLightColor = value;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected virtual void SetAvatarLighting(Avatar avatar, Vector3 ambientLightColor, 
												 Vector3 LightColor, Vector3 LightDirection) 
		{
			avatar.AvatarRenderer.LightDirection = LightDirection;
			avatar.AvatarRenderer.LightColor = LightColor;
			avatar.AvatarRenderer.AmbientLightColor = ambientLightColor;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public virtual void SetAvatarLighting(Avatar avatar) => 
			this.SetAvatarLighting(avatar, this.AmbientLightColor.ToVector3(), 
								   this.LightColor.ToVector3(), this.LightDirection);
	}
}
