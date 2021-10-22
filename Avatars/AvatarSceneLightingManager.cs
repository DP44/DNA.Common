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
			#if DECOMPILED
				Scene scene = this.GetScene(avatar);
				Vector3 avatarWorldPosition = this.GetAvatarWorldPosition(avatar);
				
				Vector3 vector = Vector3.Zero;
				Vector3 vector2 = Vector3.Zero;
				Vector3 vector3 = Vector3.Zero;

				float num = 0f;
				float num2 = 0f;
				ReadOnlyCollection<Light> lights = scene.Lights;
				int count = lights.Count;
			
				for (int i = 0; i < count; i++) 
				{
					Light light = lights[i];
					float influence = light.GetInfluence(avatarWorldPosition);
					
					if (influence > 0f && this.UseLight(avatar, light)) 
					{
						num += influence;
						
						if (light is AmbientLight) 
						{
							vector2 += light.LightColor.ToVector3() * influence;
							num2 += influence;
						} 
						else 
						{
							vector3 += light.LightColor.ToVector3() * influence;
							num = influence;

							if (light is DirectionalLight) 
							{
								DirectionalLight directionalLight = (DirectionalLight)light;
								vector += directionalLight.LightDirection * influence;
							} 
							else 
							{
								Vector3 value = avatarWorldPosition - light.WorldPosition;
								vector += value * influence;
							}
						}
					}
				}

				if (num2 < 1f) 
				{
					vector2 += base.AmbientLightColor.ToVector3() * (1f - num2);
				}

				if (num < 1f) 
				{
					vector3 += base.LightColor.ToVector3() * (1f - num);
					vector += base.LightDirection * (1f - num);
				}

				base.LightDirection.Normalize();
				this.SetAvatarLighting(avatar, vector2, vector3, vector);
			#else
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
								DirectionalLight directionalLight = (DirectionalLight) light;
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
			#endif
		}
	}
}
