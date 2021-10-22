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
			get
			{
				return this._value;
			}
			set
			{
				this._value = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public float GetValue(float x)
		{
			return this._value;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public float GetSlope(float x)
		{
			return 0f;
		}

		/// <summary>
		/// 
		/// </summary>
		public RangeF Range
		{
			get
			{
				return new RangeF(this.Value, this.Value);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public RangeF SlopeRange
		{
			get
			{
				return new RangeF(0f, 0f);
			}
		}
	}
}
