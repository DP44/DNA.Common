using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing
{
	public static class PlaneTools
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static Plane Create(Vector3 point, Vector3 normal)
		{
			normal.Normalize();

			#if DECOMPILED
/*
	IL_0007: ldarga.s	normal
	IL_0009: ldfld		float32 [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Vector3::X
	IL_000e: ldarga.s	point
	IL_0010: ldfld		float32 [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Vector3::X
	IL_0015: mul
	IL_0016: ldarga.s	normal
	IL_0018: ldfld		float32 [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Vector3::Y
	IL_001d: ldarga.s	point
	IL_001f: ldfld		float32 [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Vector3::Y
	IL_0024: mul
	IL_0025: add
	IL_0026: ldarga.s	normal
	IL_0028: ldfld		float32 [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Vector3::Z
	IL_002d: ldarga.s	point
	IL_002f: ldfld		float32 [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Vector3::Z
	IL_0034: mul
	IL_0035: add
	IL_0036: neg
	IL_0037: stloc.0	// d
*/
				float d = (float)-((double)normal.X * (double)point.X +
								   (double)normal.Y * (double)point.Y +
								   (double)normal.Z * (double)point.Z);
			#else
				float d = -(normal.X * point.X + normal.Y * point.Y + normal.Z * point.Z);
			#endif

			return new Plane(normal, d);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static PlaneIntersectionType SideOf(this Plane plane, Vector3 point)
		{
			#if DECOMPILED
				float calc = (float)((double)point.X * (double)plane.Normal.X + 
									 (double)point.Y * (double)plane.Normal.Y +
									 (double)point.Z * (double)plane.Normal.Z) + plane.D;
				
				if ((double)calc == 0.0)
				{
					return PlaneIntersectionType.Intersecting;
				}

				/*
				    IL_0051: ldloc.0	// calc
					IL_0052: ldc.r4		0.0
					IL_0057: ble.un.s	IL_005b
					IL_0059: ldc.i4.0
					IL_005a: ret
					IL_005b: ldc.i4.1
					IL_005c: ret
				*/
				return (double)calc > 0.0 
					? PlaneIntersectionType.Front
					: PlaneIntersectionType.Back;
			#else
				float calc = point.X * plane.Normal.X + 
							 point.Y * plane.Normal.Y + 
							 point.Z * plane.Normal.Z + plane.D;
				
				if (calc == 0f)
				{
					return PlaneIntersectionType.Intersecting;
				}
				
				if (calc > 0f)
				{
					return PlaneIntersectionType.Front;
				}
				
				return PlaneIntersectionType.Back;
			#endif
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static bool Contains(this Plane plane, Vector3 point) =>
			plane.SideOf(point) == PlaneIntersectionType.Intersecting;
	}
}
