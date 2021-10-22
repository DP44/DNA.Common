using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Lights
{
	public abstract class Light : Entity
	{
		public float InnerRadius = 1f;

		private float _outerRadius = 2f;

		private float _outerRadiusSquared = 4f;

		public FallOffType FallOff;

		public Color LightColor;

		private Vector3 _lastLocation;

		public float OuterRadius
		{
			get
			{
				return this._outerRadius;
			}
			set
			{
				this._outerRadius = value;
				this._outerRadiusSquared = this._outerRadius * this._outerRadius;
			}
		}

		public override void Update(DNAGame game, GameTime gameTime)
		{
			this._lastLocation = base.WorldPosition;
			base.Update(game, gameTime);
		}

		public virtual float GetInfluence(Vector3 worldLocation)
		{
			if (!this.Visible)
			{
				return 0f;
			}
			switch (this.FallOff)
			{
			case FallOffType.None:
				return 1f;
			case FallOffType.Linear:
			{
				float num = Vector3.DistanceSquared(worldLocation, this._lastLocation);
				if (num > this._outerRadiusSquared)
				{
					return 0f;
				}
				float num2 = (float)Math.Sqrt((double)num);
				if (num2 < this.InnerRadius)
				{
					return 1f;
				}
				float num3 = this.OuterRadius - this.InnerRadius;
				float num4 = num2 - this.InnerRadius;
				float val = 1f - num4 / num3;
				return Math.Max(val, 0f);
			}
			case FallOffType.Squared:
			{
				float num5 = Vector3.DistanceSquared(worldLocation, this._lastLocation);
				if (num5 > this._outerRadiusSquared)
				{
					return 0f;
				}
				float num6 = (float)Math.Sqrt((double)num5);
				if (num6 < this.InnerRadius)
				{
					return 1f;
				}
				float num7 = this.OuterRadius - this.InnerRadius;
				float num8 = num6 - this.InnerRadius;
				float num9 = 1f - num8 / num7;
				num9 *= num9;
				return Math.Max(num9, 0f);
			}
			default:
				return 1f;
			}
		}
	}
}
