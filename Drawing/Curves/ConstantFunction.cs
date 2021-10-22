using System;

namespace DNA.Drawing.Curves
{
	public class ConstantFunction : ISlopeFunction, IFunction
	{
		private float _value;

		/// <summary>
		/// 
		/// </summary>
		public float Value
		{
			get => 
				this._value;
			
			set => 
				this._value = value;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public float GetValue(float x) => 
			this._value;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public float GetSlope(float x) => 0.0f;

		/// <summary>
		/// 
		/// </summary>
		public RangeF Range => 
			new RangeF(this.Value, this.Value);

		/// <summary>
		/// 
		/// </summary>
		public RangeF SlopeRange => 
			new RangeF(0.0f, 0.0f);
	}
}
