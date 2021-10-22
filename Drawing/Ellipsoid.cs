using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing
{
	public struct Ellipsoid
	{
		public Vector3 Center;
		public Vector3 Radius;
		public Vector3 ReciprocalRadius;

		/// <summary>
		/// 
		/// </summary>
		public BoundingSphere GetBoundingSphere() =>
			new BoundingSphere(this.Center, Math.Max(
				this.Radius.X, Math.Max(this.Radius.Y, this.Radius.Z)));

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public Ellipsoid(Vector3 center, Vector3 scale, Quaternion orientation)
		{
			this.Center = center;
			this.Radius = scale;
			this.ReciprocalRadius = new Vector3(1f / scale.X, 1f / scale.Y, 1f / scale.Z);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public Vector3 TransformWorldSpacePointToUnitSphereSpace(Vector3 point) => 
			Vector3.Multiply(point - this.Center, this.ReciprocalRadius);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public Vector3 TransformUnitSphereSpacePointToWorldSpace(Vector3 point) =>
			Vector3.Multiply(point, this.Radius) + this.Center;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public Vector3 TransformWorldSpaceVectorToUnitSphereSpace(Vector3 vector)
		{
			Vector3 vec = Vector3.Multiply(vector, this.Radius);
			vec.Normalize();
			return vec;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public Vector3 TransformUnitSphereSpaceVectorToWorldSpace(Vector3 vector)
		{
			Vector3 vec = Vector3.Multiply(vector, this.ReciprocalRadius);
			vec.Normalize();
			return vec;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public Plane TransformWorldSpacePlaneToUnitSphereSpace(Plane plane)
		{
			Plane spherePlane = default(Plane);
			spherePlane.D = plane.D + Vector3.Dot(plane.Normal, this.Center);
			spherePlane.Normal = Vector3.Multiply(plane.Normal, this.Radius);
			float x = 1f / spherePlane.Normal.Length();
			spherePlane.Normal *= x;
			spherePlane.D *= x;
			return spherePlane;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public Triangle3D TransformWorldSpaceTriToUnitSphereSpace(Triangle3D tri, 
																  ref Triangle3D result)
		{
			result.A = this.TransformWorldSpacePointToUnitSphereSpace(tri.A);
			result.B = this.TransformWorldSpacePointToUnitSphereSpace(tri.B);
			result.C = this.TransformWorldSpacePointToUnitSphereSpace(tri.C);
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public float CalculateWorldSpacePenetration(Vector3 point) => 
			Vector3.Distance(
				this.TransformUnitSphereSpacePointToWorldSpace(Vector3.Normalize(point)), 
				this.TransformUnitSphereSpacePointToWorldSpace(point));
	}
}
