using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Lights
{
	public class SpotLight : DirectionalLight
	{
		public Angle InnerSpotAngle = Angle.FromDegrees(10f);
		public Angle OuterSpotAngle = Angle.FromDegrees(30f);
		
		public FallOffType ConeFalloff = FallOffType.Linear;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public override float GetInfluence(Vector3 worldLocation)
		{
			float influence = base.GetInfluence(worldLocation);
			
			if (influence > 0f)
			{
				switch (this.FallOff)
				{
					case FallOffType.Linear:
					{
						Vector3 calculatedPosition = worldLocation - base.WorldPosition;
						
						Angle calculatedAngle = 
							calculatedPosition.AngleBetween(base.LightDirection);
						
						if (calculatedAngle > this.InnerSpotAngle)
						{
							float angleAmount = 1f - calculatedAngle / this.OuterSpotAngle;
							angleAmount = Math.Max(angleAmount, 0f);
							influence *= angleAmount;
						}
						
						break;
					}

					case FallOffType.Squared:
					{
						Vector3 calculatedPosition = worldLocation - base.WorldPosition;
						
						Angle calculatedAngle = 
							calculatedPosition.AngleBetween(base.LightDirection);
						
						if (calculatedAngle > this.InnerSpotAngle)
						{
							float angleAmount = 1f - calculatedAngle / this.OuterSpotAngle;
							angleAmount *= angleAmount;
							angleAmount = Math.Max(angleAmount, 0f);
							influence *= angleAmount;
						}

						break;
					}
				}
			}

			return influence;
		}
	}
}
