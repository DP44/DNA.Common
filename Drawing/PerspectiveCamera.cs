using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public class PerspectiveCamera : Camera
	{
		public Angle FieldOfView = Angle.FromDegrees(73f);
		public float NearPlane = 0.01f;
		public float FarPlane = 1000f;

		public override Matrix View => 
			this.WorldToLocal;

		public override Matrix GetProjection(GraphicsDevice device) =>
			Matrix.CreatePerspectiveFieldOfView(
				this.FieldOfView.Radians, device.Viewport.AspectRatio, 
				this.NearPlane, this.FarPlane);
	}
}
