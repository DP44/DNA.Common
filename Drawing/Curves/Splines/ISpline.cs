using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Curves.Splines
{
	public interface ISpline
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		Vector3 ComputeValue(float t);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		Vector3 ComputeVelocity(float t);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		Vector3 ComputeAcceleration(float t);
	}
}
