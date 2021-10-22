using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing
{
	public struct Capsule
	{
		public float Radius;
		public LineF3D Segment;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public Capsule(LineF3D segment, float radius)
		{
			this.Segment = segment;
			this.Radius = radius;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public bool Contains(Vector3 point) => 
			Vector3.DistanceSquared(this.Segment.ClosetPointTo(point), point) < 
				this.Radius * this.Radius;
	}
}
