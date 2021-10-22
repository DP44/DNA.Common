using System;

namespace DNA.Drawing.Curves
{
	public interface ISlopeFunction : IFunction
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		float GetSlope(float x);

		/// <summary>
		/// 
		/// </summary>
		RangeF SlopeRange { get; }
	}
}
