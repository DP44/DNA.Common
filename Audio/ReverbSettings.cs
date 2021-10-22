using System;

namespace DNA.Audio
{
	public struct ReverbSettings
	{
		public float ReflectionsGain;
		public float ReverbGain;
		public float DecayTime;
		public float ReflectionsDelay;
		public float ReverbDelay;
		public float RearDelay;
		public float RoomSize;
		public float Density;
		public float LowEQGain;
		public float LowEQCutoff;
		public float HighEQGain;
		public float HighEQCutoff;
		public float PositionLeft;
		public float PositionRight;
		public float PositionLeftMatrix;
		public float PositionRightMatrix;
		public float EarlyDiffusion;
		public float LateDiffusion;
		public float RoomFilterMain;
		public float RoomFilterFrequency;
		public float RoomFilterHighFrequency;
		public float WetDryMix;

		public static readonly ReverbSettings Default;
		public static readonly ReverbSettings Generic;
		public static readonly ReverbSettings Mountains;
		public static readonly ReverbSettings Cave;
		public static readonly ReverbSettings Underwater;
		
		/// <summary>
		/// 
		/// </summary>
		static ReverbSettings()
		{
			ReverbSettings.Mountains.ReflectionsGain = -27.8f;
			ReverbSettings.Mountains.ReverbGain = -20.14f;
			ReverbSettings.Mountains.DecayTime = 1.49f;
			ReverbSettings.Mountains.ReflectionsDelay = 299f;
			ReverbSettings.Mountains.ReverbDelay = 84f;
			ReverbSettings.Mountains.RearDelay = 5f;
			ReverbSettings.Mountains.RoomSize = 100f;
			ReverbSettings.Mountains.Density = 100f;
			ReverbSettings.Mountains.LowEQGain = 8f;
			ReverbSettings.Mountains.LowEQCutoff = 4f;
			ReverbSettings.Mountains.HighEQGain = 6f;
			ReverbSettings.Mountains.HighEQCutoff = 6f;
			ReverbSettings.Mountains.PositionLeft = 6f;
			ReverbSettings.Mountains.PositionRight = 6f;
			ReverbSettings.Mountains.PositionLeftMatrix = 27f;
			ReverbSettings.Mountains.PositionRightMatrix = 27f;
			ReverbSettings.Mountains.EarlyDiffusion = 4f;
			ReverbSettings.Mountains.LateDiffusion = 4f;
			ReverbSettings.Mountains.RoomFilterMain = -10f;
			ReverbSettings.Mountains.RoomFilterFrequency = 5000f;
			ReverbSettings.Mountains.RoomFilterHighFrequency = -25f;
			ReverbSettings.Mountains.WetDryMix = 100f;
			ReverbSettings.Cave.ReflectionsGain = -6.02f;
			ReverbSettings.Cave.ReverbGain = -3.02f;
			ReverbSettings.Cave.DecayTime = 3.78f;
			ReverbSettings.Cave.ReflectionsDelay = 15f;
			ReverbSettings.Cave.ReverbDelay = 22f;
			ReverbSettings.Cave.RearDelay = 5f;
			ReverbSettings.Cave.RoomSize = 100f;
			ReverbSettings.Cave.Density = 100f;
			ReverbSettings.Cave.LowEQGain = 8f;
			ReverbSettings.Cave.LowEQCutoff = 4f;
			ReverbSettings.Cave.HighEQGain = 8f;
			ReverbSettings.Cave.HighEQCutoff = 6f;
			ReverbSettings.Cave.PositionLeft = 6f;
			ReverbSettings.Cave.PositionRight = 6f;
			ReverbSettings.Cave.PositionLeftMatrix = 27f;
			ReverbSettings.Cave.PositionRightMatrix = 27f;
			ReverbSettings.Cave.EarlyDiffusion = 15f;
			ReverbSettings.Cave.LateDiffusion = 15f;
			ReverbSettings.Cave.RoomFilterMain = -10f;
			ReverbSettings.Cave.RoomFilterFrequency = 5000f;
			ReverbSettings.Cave.RoomFilterHighFrequency = 0f;
			ReverbSettings.Cave.WetDryMix = 100f;
			ReverbSettings.Underwater.ReflectionsGain = -4.49f;
			ReverbSettings.Underwater.ReverbGain = 17f;
			ReverbSettings.Underwater.DecayTime = 1.49f;
			ReverbSettings.Underwater.ReflectionsDelay = 7f;
			ReverbSettings.Underwater.ReverbDelay = 11f;
			ReverbSettings.Underwater.RearDelay = 5f;
			ReverbSettings.Underwater.RoomSize = 100f;
			ReverbSettings.Underwater.Density = 100f;
			ReverbSettings.Underwater.LowEQGain = 8f;
			ReverbSettings.Underwater.LowEQCutoff = 4f;
			ReverbSettings.Underwater.HighEQGain = 5f;
			ReverbSettings.Underwater.HighEQCutoff = 6f;
			ReverbSettings.Underwater.PositionLeft = 6f;
			ReverbSettings.Underwater.PositionRight = 6f;
			ReverbSettings.Underwater.PositionLeftMatrix = 27f;
			ReverbSettings.Underwater.PositionRightMatrix = 27f;
			ReverbSettings.Underwater.EarlyDiffusion = 15f;
			ReverbSettings.Underwater.LateDiffusion = 15f;
			ReverbSettings.Underwater.RoomFilterMain = -10f;
			ReverbSettings.Underwater.RoomFilterFrequency = 5000f;
			ReverbSettings.Underwater.RoomFilterHighFrequency = -40f;
			ReverbSettings.Underwater.WetDryMix = 100f;
			ReverbSettings.Default.ReflectionsGain = -100f;
			ReverbSettings.Default.ReverbGain = -100f;
			ReverbSettings.Default.DecayTime = 1f;
			ReverbSettings.Default.ReflectionsDelay = 20f;
			ReverbSettings.Default.ReverbDelay = 40f;
			ReverbSettings.Default.RearDelay = 5f;
			ReverbSettings.Default.RoomSize = 100f;
			ReverbSettings.Default.Density = 100f;
			ReverbSettings.Default.LowEQGain = 8f;
			ReverbSettings.Default.LowEQCutoff = 4f;
			ReverbSettings.Default.HighEQGain = 7f;
			ReverbSettings.Default.HighEQCutoff = 6f;
			ReverbSettings.Default.PositionLeft = 6f;
			ReverbSettings.Default.PositionRight = 6f;
			ReverbSettings.Default.PositionLeftMatrix = 27f;
			ReverbSettings.Default.PositionRightMatrix = 27f;
			ReverbSettings.Default.EarlyDiffusion = 15f;
			ReverbSettings.Default.LateDiffusion = 15f;
			ReverbSettings.Default.RoomFilterMain = -100f;
			ReverbSettings.Default.RoomFilterFrequency = 5000f;
			ReverbSettings.Default.RoomFilterHighFrequency = 0f;
			ReverbSettings.Default.WetDryMix = 100f;
			ReverbSettings.Generic.ReflectionsGain = -26.02f;
			ReverbSettings.Generic.ReverbGain = 2f;
			ReverbSettings.Generic.DecayTime = 1.49f;
			ReverbSettings.Generic.ReflectionsDelay = 7f;
			ReverbSettings.Generic.ReverbDelay = 11f;
			ReverbSettings.Generic.RearDelay = 5f;
			ReverbSettings.Generic.RoomSize = 100f;
			ReverbSettings.Generic.Density = 100f;
			ReverbSettings.Generic.LowEQGain = 8f;
			ReverbSettings.Generic.LowEQCutoff = 4f;
			ReverbSettings.Generic.HighEQGain = 8f;
			ReverbSettings.Generic.HighEQCutoff = 6f;
			ReverbSettings.Generic.PositionLeft = 6f;
			ReverbSettings.Generic.PositionRight = 6f;
			ReverbSettings.Generic.PositionLeftMatrix = 27f;
			ReverbSettings.Generic.PositionRightMatrix = 27f;
			ReverbSettings.Generic.EarlyDiffusion = 15f;
			ReverbSettings.Generic.LateDiffusion = 15f;
			ReverbSettings.Generic.RoomFilterMain = -10f;
			ReverbSettings.Generic.RoomFilterFrequency = 5000f;
			ReverbSettings.Generic.RoomFilterHighFrequency = -1f;
			ReverbSettings.Generic.WetDryMix = 100f;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void Blend(ReverbSettings settings, float blender)
		{
			float num = 1f - blender;
			
			this.ReflectionsGain = this.ReflectionsGain * num + settings.ReflectionsGain * blender;
			this.ReverbGain = this.ReverbGain * num + settings.ReverbGain * blender;
			this.DecayTime = this.DecayTime * num + settings.DecayTime * blender;
			this.ReflectionsDelay = this.ReflectionsDelay * num + settings.ReflectionsDelay * blender;
			this.ReverbDelay = this.ReverbDelay * num + settings.ReverbDelay * blender;
			this.RearDelay = this.RearDelay * num + settings.RearDelay * blender;
			this.RoomSize = this.RoomSize * num + settings.RoomSize * blender;
			this.Density = this.Density * num + settings.Density * blender;
			this.LowEQGain = this.LowEQGain * num + settings.LowEQGain * blender;
			this.LowEQCutoff = this.LowEQCutoff * num + settings.LowEQCutoff * blender;
			this.HighEQGain = this.HighEQGain * num + settings.HighEQGain * blender;
			this.HighEQCutoff = this.HighEQCutoff * num + settings.HighEQCutoff * blender;
			this.PositionLeft = this.PositionLeft * num + settings.PositionLeft * blender;
			this.PositionRight = this.PositionRight * num + settings.PositionRight * blender;
			this.PositionLeftMatrix = this.PositionLeftMatrix * num + settings.PositionLeftMatrix * blender;
			this.PositionRightMatrix = this.PositionRightMatrix * num + settings.PositionRightMatrix * blender;
			this.EarlyDiffusion = this.EarlyDiffusion * num + settings.EarlyDiffusion * blender;
			this.LateDiffusion = this.LateDiffusion * num + settings.LateDiffusion * blender;
			this.RoomFilterMain = this.RoomFilterMain * num + settings.RoomFilterMain * blender;
			this.RoomFilterFrequency = this.RoomFilterFrequency * num + settings.RoomFilterFrequency * blender;
			this.RoomFilterHighFrequency = this.RoomFilterHighFrequency * num + settings.RoomFilterHighFrequency * blender;
			this.WetDryMix = this.WetDryMix * num + settings.WetDryMix * blender;
		}
	}
}
