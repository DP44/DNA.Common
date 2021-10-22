using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing
{
	public struct BloomSettings
	{
		public static readonly BloomSettings Default = 
			new BloomSettings(0.25f, 4f, 1.25f, 1f, 1f, 1f);

		public static readonly BloomSettings Soft = 
			new BloomSettings(0f, 3f, 1f, 1f, 1f, 1f);

		public static readonly BloomSettings Desaturated = 
			new BloomSettings(0.5f, 8f, 2f, 1f, 0f, 1f);

		public static readonly BloomSettings Saturated = 
			new BloomSettings(0.25f, 4f, 2f, 1f, 2f, 0f);

		public static readonly BloomSettings Blurry = 
			new BloomSettings(0f, 2f, 1f, 0.1f, 1f, 1f);

		public static readonly BloomSettings Subtle = 
			new BloomSettings(0.5f, 2f, 1f, 1f, 1f, 1f);

		public float BloomThreshold;
		public float BlurAmount;
		public float BloomIntensity;
		public float BaseIntensity;
		public float BloomSaturation;
		public float BaseSaturation;

		public BloomSettings(float bloomThreshhold, float blurAmount, float bloomIntensity, 
							 float baseIntensity, float bloomSaturation, float baseSaturation)
		{
			this.BloomThreshold = bloomThreshhold;
			this.BlurAmount = blurAmount;
			this.BloomIntensity = bloomIntensity;
			this.BaseIntensity = baseIntensity;
			this.BloomSaturation = bloomSaturation;
			this.BaseSaturation = baseSaturation;
		}

		public void Lerp(BloomSettings settings, float blender)
		{
			this.BloomThreshold = 
				MathHelper.Lerp(this.BloomThreshold, settings.BloomThreshold, blender);
			
			this.BlurAmount = 
				MathHelper.Lerp(this.BlurAmount, settings.BlurAmount, blender);
			
			this.BloomIntensity = 
				MathHelper.Lerp(this.BloomIntensity, settings.BloomIntensity, blender);
			
			this.BaseIntensity = 
				MathHelper.Lerp(this.BaseIntensity, settings.BaseIntensity, blender);
			
			this.BloomSaturation = 
				MathHelper.Lerp(this.BloomSaturation, settings.BloomSaturation, blender);
			
			this.BaseSaturation = 
				MathHelper.Lerp(this.BaseSaturation, settings.BaseSaturation, blender);
		}

		public static BloomSettings Lerp(BloomSettings set1, BloomSettings set2, float blender) =>
			new BloomSettings(
				MathHelper.Lerp(set1.BloomThreshold, set2.BloomThreshold, blender), 
				MathHelper.Lerp(set1.BlurAmount, set2.BlurAmount, blender), 
				MathHelper.Lerp(set1.BloomIntensity, set2.BloomIntensity, blender), 
				MathHelper.Lerp(set1.BaseIntensity, set2.BaseIntensity, blender), 
				MathHelper.Lerp(set1.BloomSaturation, set2.BloomSaturation, blender), 
				MathHelper.Lerp(set1.BaseSaturation, set2.BaseSaturation, blender));
	}
}
