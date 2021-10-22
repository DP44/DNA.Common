using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Collision
{
	public abstract class CollisionMap
	{
		public struct ContactPoint
		{
			public Vector3 PenetrationDirection;
			public Triangle3D Triangle;
			public float PenetrationDepth;

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public ContactPoint(Vector3 dir, Triangle3D tri)
			{
				this.PenetrationDirection = dir;
				this.Triangle = tri;
				this.PenetrationDepth = 0f;
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public ContactPoint(Vector3 dir, Triangle3D tri, float p)
			{
				this.PenetrationDirection = dir;
				this.Triangle = tri;
				this.PenetrationDepth = p;
			}
		}

		public struct RayQueryResult
		{
			public Triangle3D Triangle;
			public float T;

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public RayQueryResult(float t, Triangle3D tri)
			{
				this.T = t;
				this.Triangle = tri;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public float? CollidesWith(LineF3D line) => 
			this.CollidesWith(line, out Triangle3D _);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public float? CollidesWith(Ray ray) => 
			this.CollidesWith(ray, out Triangle3D _);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public float? CollidesWith(Ray ray, out Triangle3D triangle)
		{
			CollisionMap.RayQueryResult? result = 
				this.CollidesWith(ray, 0f, float.MaxValue);
			
			if (result == null)
			{
				triangle = default(Triangle3D);
				return null;
			}
			
			triangle = result.Value.Triangle;
			
			return new float?(result.Value.T);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public float? CollidesWith(LineF3D line, out Triangle3D triangle)
		{
			Ray ray = new Ray(line.Start, line.End - line.Start);
			CollisionMap.RayQueryResult? result = this.CollidesWith(ray, 0f, 1f);
			
			if (result == null)
			{
				triangle = default(Triangle3D);
				return null;
			}
			
			triangle = result.Value.Triangle;
			return new float?(result.Value.T);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected abstract CollisionMap.RayQueryResult? CollidesWith(
			Ray ray, float min, float max);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public abstract void GetTriangles(List<Triangle3D> tris);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public abstract void FindCollisions(Ellipsoid ellipsoid, 
											List<CollisionMap.ContactPoint> contacts);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public abstract void FindCollisions(BoundingSphere sphere, 
											List<CollisionMap.ContactPoint> contacts);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public abstract void Load(BinaryReader reader);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public abstract void Save(BinaryWriter reader);
	}
}
