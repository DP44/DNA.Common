using System;

namespace DNA.Drawing.Curves
{
	public interface IFunction
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		float GetValue(float x);

		/// <summary>
		/// 
		/// </summary>
		RangeF Range { get; }
	}
}
