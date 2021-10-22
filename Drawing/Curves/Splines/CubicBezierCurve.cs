using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Curves.Splines
{
	public class CubicBezierCurve : Spline
	{
		public struct ControlPoint
		{
			private Vector3 _inHandle;
			private Vector3 _location;
			private Vector3 _outHandle;

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public ControlPoint(Vector3 inPoint, Vector3 location, Vector3 outPoint)
			{
				this._inHandle = inPoint;
				this._location = location;
				this._outHandle = outPoint;
			}

			/// <summary>
			/// 
			/// </summary>
			public Vector3 In
			{
				get
				{
					return this._inHandle;
				}
				set
				{
					this._inHandle = value;
				}
			}

			/// <summary>
			/// 
			/// </summary>
			public Vector3 Location
			{
				get
				{
					return this._location;
				}
				set
				{
					this._location = value;
				}
			}

			/// <summary>
			/// 
			/// </summary>
			public Vector3 Out
			{
				get
				{
					return this._outHandle;
				}
				set
				{
					this._outHandle = value;
				}
			}

			/// <summary>
			/// 
			/// </summary>
			public void ReflectInHandle()
			{
				Vector3 value = this.In - this.Location;
				this.Out = this.Location - value;
			}

			/// <summary>
			/// 
			/// </summary>
			public void ReflectOutHandle()
			{
				Vector3 value = this.Out - this.Location;
				this.In = this.Location - value;
			}

			/// <summary>
			/// 
			/// </summary>
			public override int GetHashCode()
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public bool Equals(CubicBezierCurve.ControlPoint other)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public override bool Equals(object obj)
			{
				return obj.GetType() == typeof(CubicBezierCurve.ControlPoint) && this.Equals((CubicBezierCurve.ControlPoint)obj);
			}

			public static bool operator == (CubicBezierCurve.ControlPoint a, CubicBezierCurve.ControlPoint b)
			{
				return a.Equals(b);
			}

			public static bool operator != (CubicBezierCurve.ControlPoint a, CubicBezierCurve.ControlPoint b)
			{
				return !a.Equals(b);
			}
		}

		private List<CubicBezierCurve.ControlPoint> _controlPoints = new List<CubicBezierCurve.ControlPoint>();

		/// <summary>
		/// 
		/// </summary>
		public List<CubicBezierCurve.ControlPoint> ControlPoints
		{
			get
			{
				return this._controlPoints;
			}
			set
			{
				this._controlPoints = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public override Vector3 ComputeValue(float t)
		{
			int controlPointIndex = Spline.GetControlPointIndex(this._controlPoints.Count, ref t);
			return CubicBezierCurve.ComputeValue(t, this.ControlPoints[controlPointIndex], this.ControlPoints[controlPointIndex + 1]);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		private static Vector3 ComputeValue(float t, CubicBezierCurve.ControlPoint cp1, CubicBezierCurve.ControlPoint cp2)
		{
			Vector3 location = cp1.Location;
			Vector3 @out = cp1.Out;
			Vector3 @in = cp2.In;
			Vector3 location2 = cp2.Location;
			
			float num = 1f - t;
			float num2 = num * num * num;
			float num3 = 3f * t * num * num;
			float num4 = 3f * t * t * num;
			float num5 = t * t * t;
			
			float x = num2 * location.X + num3 * @out.X + num4 * @in.X + num5 * location2.X;
			float y = num2 * location.Y + num3 * @out.Y + num4 * @in.Y + num5 * location2.Y;
			float z = num2 * location.Z + num3 * @out.Z + num4 * @in.Z + num5 * location2.Z;
			
			return new Vector3(x, y, z);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public override Vector3 ComputeVelocity(float t)
		{
			int controlPointIndex = Spline.GetControlPointIndex(this._controlPoints.Count, ref t);
			return CubicBezierCurve.ComputeVelocity(t, this.ControlPoints[controlPointIndex], this.ControlPoints[controlPointIndex + 1]);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static Vector3 ComputeVelocity(float t, CubicBezierCurve.ControlPoint cp1, CubicBezierCurve.ControlPoint cp2)
		{
			throw new NotImplementedException("The method or operation is not implemented.");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public override Vector3 ComputeAcceleration(float t)
		{
			throw new NotImplementedException("The method or operation is not implemented.");
		}
	}
}
