using System;

namespace DNA.Drawing.Curves
{
	public class LinearFunction : ISlopeFunction, IFunction
	{
		private float _intercept;
		private float _slope;

		/// <summary>
		/// 
		/// </summary>
		public float Start
		{
			get =>
				return this._intercept;

			set =>
				this._intercept = value;
		}

		/// <summary>
		/// 
		/// </summary>
		public float End
		{
			get =>
				this.GetValue(1f);

			set =>
				this.SetFunction(this._intercept, value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		private void SetFunction(float start, float end)
		{
			this._intercept = start;
			this._slope = end - start;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public float GetValue(float x) =>
			this._slope * x + this._intercept;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public float GetSlope(float x) =>
			this._slope;

		/// <summary>
		/// 
		/// </summary>
		public RangeF Range =>
				new RangeF(this.Start, this.End);

		/// <summary>
		/// 
		/// </summary>
		public RangeF SlopeRange =>
			new RangeF(this._slope, this._slope);
	}
}
