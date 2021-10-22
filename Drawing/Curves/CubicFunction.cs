using System;

namespace DNA.Drawing.Curves
{
	public class CubicFunction : ISlopeFunction, IFunction
	{
		private float _startValue;
		private float _startSlope;
		private float _a;
		private float _b;

		/// <summary>
		/// 
		/// </summary>
		public float StartValue
		{
			get =>
				this._startValue;

			set =>
				this.SetCurve(value, this._startSlope, 
							  this.EndValue, this.EndSlope);
		}

		/// <summary>
		/// 
		/// </summary>
		public float StartSlope
		{
			get =>
				this._startSlope;
			
			set =>
				this.SetCurve(this.StartValue, value, 
							  this.EndValue, this.EndSlope);
		}

		public float EndValue
		{
			get =>
				this.GetValue(1f);

			set =>
				this.SetCurve(this.StartValue, this.StartSlope, 
							  value, this.EndSlope);
		}

		/// <summary>
		/// 
		/// </summary>
		public float EndSlope
		{
			get =>
				this.GetSlope(1f);

			set =>
				this.SetCurve(this.StartValue, this.StartSlope, 
							  this.EndValue, value);
		}

		/// <summary>
		/// 
		/// </summary>
		public CubicFunction() {}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public CubicFunction(float startValue, float startSlope, 
							 float endValue, float endSlope)
		{
			this.SetCurve(startValue, startSlope, endValue, endSlope);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void SetCurve(float startValue, float startSlope, 
							 float endValue, float endSlope)
		{
			this._a = endSlope + startSlope + 2f * (startValue - endValue);
			this._b = (endSlope - startSlope - 3f * this._a) * 0.5f;
			this._startSlope = startSlope;
			this._startValue = startValue;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public float GetSlope(float x) =>
			(3f * this._a * x + 2f * this._b) * x + this._startSlope;

		/// <summary>
		/// 
		/// </summary>
		public RangeF SlopeRange
		{
			get
			{
				if (this.StartSlope < this.EndSlope)
				{
					return new RangeF(this.StartSlope, this.EndSlope);
				}

				return new RangeF(this.EndSlope, this.StartSlope);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public float GetValue(float x) =>
			((this._a * x + this._b) * x + this._startSlope) * x + this._startValue;

		/// <summary>
		/// 
		/// </summary>
		public RangeF Range
		{
			get
			{
				float num2;
				float num = num2 = this.StartValue;
				float num3 = this.EndValue;
				
				if (num3 > num2)
				{
					num2 = num3;
				}
				else if (num3 < num)
				{
					num = num3;
				}
				
				if (this._a == 0f)
				{
					if (this._b != 0f)
					{
						num3 = -0.5f * this._startSlope / this._b;
						
						if (num3 > 0f && num3 < 1f)
						{
							num3 = this.GetValue(num3);
							
							if (num3 < num)
							{
								num = num3;
							}
							else if (num3 > num2)
							{
								num2 = num3;
							}
						}
					}
				}
				else
				{
					float num4 = 3f * this._a;
					float num5 = 2f * this._b;
					float num6 = num5 * num5 - 4f * num4 * this._startSlope;
					
					if (num6 >= 0f)
					{
						num4 = 0.5f / num4;
						num5 = -num5 * num4;
						num6 = (float)(Math.Sqrt((double)num6) * (double)num4);
						num3 = num5 - num6;
						
						if (num3 > 0f && num3 < 1f)
						{
							num3 = this.GetValue(num3);
							
							if (num3 < num)
							{
								num = num3;
							}
							else if (num3 > num2)
							{
								num2 = num3;
							}
						}
						
						num3 = num5 + num6;
						
						if (num3 > 0f && num3 < 1f)
						{
							num3 = this.GetValue(num3);
							
							if (num3 < num)
							{
								num = num3;
							}
							else if (num3 > num2)
							{
								num2 = num3;
							}
						}
					}
				}

				return new RangeF(num, num2);
			}
		}
	}
}
