using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Curves.Splines
{
	public class HermiteSpline : Spline
	{
		public struct ControlPoint
		{
			private Vector3 _location;
			private Vector3 _inHandle;
			private Vector3 _outHandle;

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public ControlPoint(Vector3 inVect, Vector3 location, Vector3 outVect)
			{
				this._inHandle = inVect;
				this._location = location;
				this._outHandle = outVect;
			}

			/// <summary>
			/// 
			/// </summary>
			public Vector3 In
			{
				get =>
					this._inHandle;

				set =>
					this._inHandle = value;
			}

			/// <summary>
			/// 
			/// </summary>
			public Vector3 Location
			{
				get =>
					this._location;

				set =>
					this._location = value;
			}

			/// <summary>
			/// 
			/// </summary>
			public Vector3 Out
			{
				get =>
					this._outHandle;

				set =>
					this._outHandle = value;
			}

			/// <summary>
			/// 
			/// </summary>
			public void ReflectInHandle() =>
				this.Out = -this.In;

			/// <summary>
			/// 
			/// </summary>
			public void ReflectOutHandle() =>
				this.In = -this.Out;

			/// <summary>
			/// 
			/// </summary>
			public override int GetHashCode() =>
				throw new NotImplementedException();

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public bool Equals(HermiteSpline.ControlPoint other) =>
				throw new NotImplementedException();

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public override bool Equals(object obj) =>
				obj.GetType() == typeof(HermiteSpline.ControlPoint) && 
				this.Equals((HermiteSpline.ControlPoint)obj);

			public static bool operator == (HermiteSpline.ControlPoint a, 
											HermiteSpline.ControlPoint b) =>
				a.Equals(b);

			public static bool operator != (HermiteSpline.ControlPoint a, 
											HermiteSpline.ControlPoint b) =>
				!a.Equals(b);
		}

		private List<HermiteSpline.ControlPoint> _controlPoints = 
			new List<HermiteSpline.ControlPoint>();

		/// <summary>
		/// 
		/// </summary>
		public List<HermiteSpline.ControlPoint> ControlPoints
		{
			get =>
				this._controlPoints;

			set =>
				this._controlPoints = value;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public override Vector3 ComputeValue(float t)
		{
			int controlPointIndex = 
				Spline.GetControlPointIndex(this._controlPoints.Count, ref t);
			
			return HermiteSpline.ComputeValue(t, 
				this.ControlPoints[controlPointIndex], 
				this.ControlPoints[controlPointIndex + 1]);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		private static Vector3 ComputeValue(float t, HermiteSpline.ControlPoint cp1, 
											HermiteSpline.ControlPoint cp2)
		{
			Vector3 location = cp1.Location;
			Vector3 @out = cp1.Out;
			
			Vector3 location2 = cp2.Location;
			Vector3 @in = cp2.In;
			
			float num = t * t;
			float num2 = num * t;
			float num3 = 2f * num2 - 3f * num + 1f;
			float num4 = -2f * num2 + 3f * num;
			float num5 = num2 - 2f * num + t;
			float num6 = num2 - num;
			
			float x = num3 * location.X + num4 * location2.X + num5 * @out.X + num6 * @in.X;
			float y = num3 * location.Y + num4 * location2.Y + num5 * @out.Y + num6 * @in.Y;
			float z = num3 * location.Z + num4 * location2.Z + num5 * @out.Z + num6 * @in.Z;
			
			return new Vector3(x, y, z);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public override Vector3 ComputeVelocity(float t) =>
			throw new NotImplementedException();

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public override Vector3 ComputeAcceleration(float t) =>
			throw new NotImplementedException();

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public override bool Equals(object obj)
		{
			HermiteSpline hermiteSpline = obj as HermiteSpline;
			return !(hermiteSpline == null) && this == hermiteSpline;
		}

		/// <summary>
		/// 
		/// </summary>
		public override int GetHashCode() =>
			throw new NotImplementedException();

		public static bool operator == (HermiteSpline a, HermiteSpline b) =>
			throw new NotImplementedException();

		public static bool operator != (HermiteSpline a, HermiteSpline b) =>
			throw new NotImplementedException();
	}
}
