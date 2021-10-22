using System;
using System.Collections.ObjectModel;
using DNA.Drawing;
using DNA.Drawing.Lights;
using Microsoft.Xna.Framework;

namespace DNA.Avatars 
{
	public class AvatarSceneLightingManager : AvatarLightingManager 
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public virtual bool UseLight(Avatar avatar, Light light) => true;
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected virtual Vector3 GetAvatarWorldPosition(Avatar avatar) => 
			avatar.WorldPosition + new Vector3(0.0f, 1f, 0.0f);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected virtual Scene GetScene(Avatar avatar) =>
			avatar.Scene;
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public override void SetAvatarLighting(Avatar avatar) 
		{
			Scene scene = this.GetScene(avatar);
			Vector3 avatarWorldPosition = this.GetAvatarWorldPosition(avatar);
			
			Vector3 lightDirection = Vector3.Zero;
			Vector3 ambientLightColor = Vector3.Zero;
			Vector3 lightColor = Vector3.Zero;
			
			float lightInfluence = 0.0f;
			float ambientLightInfluence = 0.0f;

			foreach (Light light in lights)
			{
				float influence = light.GetInfluence(avatarWorldPosition);
				
				if ((double)influence > 0.0 && this.UseLight(avatar, light))
				{
					lightInfluence += influence;
					
					if (light is AmbientLight)
					{
						ambientLightColor += light.LightColor.ToVector3() * influence;
						ambientLightInfluence += influence;
					}
					else
					{
						lightColor += light.LightColor.ToVector3() * influence;
						lightInfluence = influence;
						
						if (light is DirectionalLight)
						{
							DirectionalLight directionalLight = (DirectionalLight)light;
							lightDirection += directionalLight.LightDirection * influence;
						}
						else
						{
							Vector3 vector3 = avatarWorldPosition - light.WorldPosition;
							lightDirection += vector3 * influence;
						}
					}
				}
			}
			
			if ((double)ambientLightInfluence < 1.0)
			{
				ambientLightColor += 
					this.AmbientLightColor.ToVector3() * (1f - ambientLightInfluence);
			}

			if ((double)lightInfluence < 1.0)
			{
				lightColor += this.LightColor.ToVector3() * (1f - lightInfluence);
				lightDirection += this.LightDirection * (1f - lightInfluence);
			}
			
			this.LightDirection.Normalize();
			this.SetAvatarLighting(avatar, ambientLightColor, lightColor, lightDirection);
		}
	}
}
