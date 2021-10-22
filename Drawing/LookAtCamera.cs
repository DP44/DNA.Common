using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing
{
	public class LookAtCamera : PerspectiveCamera
	{
		public Entity LookAtEntity;
		public Angle Roll = Angle.Zero;

		/// <summary>
		/// 
		/// </summary>
		public override Matrix View
		{
			get
			{
				if (this.LookAtEntity != null)
				{
					Matrix worldMatrix = Matrix.CreateFromAxisAngle(
						Vector3.Forward, this.Roll.Radians);

					Matrix localToWorldMatrix = worldMatrix * base.LocalToWorld;

					Vector3 cameraUpVector = Vector3.TransformNormal(
						Vector3.Up, localToWorldMatrix);

					Vector3 worldPosition = base.WorldPosition;
					Vector3 entityPosition = this.LookAtEntity.WorldPosition;

					return Matrix.CreateLookAt(worldPosition, 
						entityPosition, cameraUpVector);
				}
				
				return base.View;
			}
		}
	}
}
