using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing
{
	[Serializable]
	public struct ColorF
	{
		private float _red;
		private float _green;
		private float _blue;
		private float _alpha;

		/// <summary>
		/// 
		/// </summary>
		public float Red => this._red;

		/// <summary>
		/// 
		/// </summary>
		public float Green => this._green;

		/// <summary>
		/// 
		/// </summary>
		public float Blue => this._blue;

		/// <summary>
		/// 
		/// </summary>
		public float Alpha => this._alpha;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static ColorF FromRGB(float r, float g, float b) => 
			new ColorF(1f, r, g, b);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static ColorF FromARGB(float a, float r, float g, float b) => 
			new ColorF(a, r, g, b);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static ColorF FromColor(Color col) => 
			new ColorF(col);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public Vector3 ToVector3() => 
			new Vector3(this._red, this._green, this._blue);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public Vector4 ToVector4() => 
			new Vector4(this._red, this._green, this._blue, this._alpha);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static implicit operator Color(ColorF col) => 
			col.GetColor();

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static implicit operator ColorF(Color col) => 
			ColorF.FromColor(col);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public ColorF Brighten(float factor) => 
			ColorF.FromARGB(this.Alpha, this.Red * factor, 
							this.Green * factor, 
							this.Blue * factor);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public ColorF Saturate(float factor)
		{
			Angle h;
			float num;
			float brt;
			this.GetHSV(out h, out num, out brt);
			num *= factor;
			return ColorF.FromAHSV(this.Alpha, h, num, brt);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public ColorF AdjustBrightness(float factor)
		{
			float r = this.Red * factor;
			float g = this.Green * factor;
			float b = this.Blue * factor;
			return ColorF.FromARGB(this.Alpha, r, g, b);
		}

		private static float ColVal(float n1, float n2, float hue)
		{
			if (hue < 60f)
			{
				return n1 + (n2 - n1) * hue / 60f;
			}
			
			if (hue < 180f)
			{
				return n2;
			}
			
			if (hue < 240f)
			{
				return n1 + (n2 - n1) * (240f - hue) / 60f;
			}

			return n1;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static ColorF FromHSL(Angle hue, float sat, float brt) => 
			ColorF.FromAHSL(1f, hue, sat, brt);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static ColorF FromAHSL(float alpha, Angle hue, float sat, float brt)
		{
			hue.Normalize();
			float num = (brt <= 0.5f) ? (brt * (1f + sat)) : (brt + sat - brt * sat);
			float n = 2f * brt - num;
			
			float r;
			float g;
			float b;
			
			if (sat == 0f)
			{
				b = brt;
				g = brt;
				r = brt;
			}
			else
			{
				float num2 = hue.Degrees;
				r = ColorF.ColVal(n, num, num2 + 120f);
				g = ColorF.ColVal(n, num, num2);
				b = ColorF.ColVal(n, num, num2 - 120f);
			}

			return ColorF.FromARGB(alpha, r, g, b);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static ColorF FromHSV(Angle hue, float sat, float brt) => 
			ColorF.FromAHSV(1f, hue, sat, brt);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static ColorF FromAHSV(float alpha, Angle h, float sat, float brt)
		{
			float b;
			float g;
			float r;
			
			if (sat == 0f)
			{
				b = brt;
				g = brt;
				r = brt;
			}
			else
			{
				h.Normalize();
				
				float num = h.Degrees;
				num /= 60f;
				
				int num2 = (int)Math.Floor((double)num);
				
				float num3 = num - (float)num2;
				
				float num4 = brt * (1f - sat);
				float num5 = brt * (1f - sat * num3);
				float num6 = brt * (1f - sat * (1f - num3));
				
				switch (num2)
				{
					case 0:
					{
						r = brt;
						g = num6;
						b = num4;
						break;
					}

					case 1:
					{
						r = num5;
						g = brt;
						b = num4;
						break;
					}

					case 2:
					{
						r = num4;
						g = brt;
						b = num6;
						break;
					}

					case 3:
					{
						r = num4;
						g = num5;
						b = brt;
						break;
					}

					case 4:
					{
						r = num6;
						g = num4;
						b = brt;
						break;
					}

					case 5:
					{
						r = brt;
						g = num4;
						b = num5;
						break;
					}

					default:
					{
						throw new Exception("Hue Out of Range");
					}
				}
			}

			return ColorF.FromARGB(alpha, r, g, b);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static ColorF FromVector3(Vector3 vector) =>
			new ColorF(vector);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static ColorF FromVector4(Vector4 vector) =>
			new ColorF(vector);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static ColorF FromCMY(float c, float m, float y) =>
			ColorF.FromACMY(1f, c, m, y);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static ColorF FromACMY(float a, float c, float m, float y) => 
			ColorF.FromARGB(a, 1f - c, 1f - m, 1f - y);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static ColorF FromCMYK(float c, float m, float y, float k) => 
			ColorF.FromACMYK(1f, c, m, y, k);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static ColorF FromACMYK(float a, float c, float m, float y, float k)
		{
			c = c * (1f - k) + k;
			m = m * (1f - k) + k;
			y = y * (1f - k) + k;
			return ColorF.FromACMY(a, c, m, y);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void GetHSL(out Angle h, out float s, out float l)
		{
			float num = MathTools.Max(this.Red, this.Green, this.Blue);
			float num2 = MathTools.Min(this.Red, this.Green, this.Blue);
			
			l = (num + num2) / 2f;
			
			float num3 = num - num2;
			
			if (num3 == 0f)
			{
				s = 0f;
				h = Angle.FromDegrees(0f);
				return;
			}
			
			if (l < 0.5f)
			{
				s = num3 / (num + num2);
			}
			else
			{
				s = num3 / (2f - (num + num2));
			}
			
			float num4;
			
			if (this.Red == num)
			{
				num4 = (this.Green - this.Blue) / num3;
			}
			else if (this.Green == num)
			{
				num4 = 2f + (this.Blue - this.Red) / num3;
			}
			else if (this.Blue == num)
			{
				num4 = 4f + (this.Red - this.Green) / num3;
			}
			else
			{
				num4 = 0f;
			}
			
			num4 *= 60f;
			
			if (num4 < 0f)
			{
				num4 += 360f;
			}

			h = Angle.FromDegrees(num4);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void GetHSV(out Angle h, out float s, out float v)
		{
			float red = this.Red;
			float green = this.Green;
			float blue = this.Blue;
			float colMax = MathTools.Max(red, green, blue);
			float colMin = MathTools.Min(red, green, blue);
			
			v = colMax;
			
			float colMaxMin = colMax - colMin;
			
			if (colMax == 0f || colMaxMin == 0f)
			{
				h = Angle.FromDegrees(0f);
				s = 0f;
				return;
			}
			
			s = colMaxMin / colMax;
			float hueDeg;
			
			if (red == colMax)
			{
				hueDeg = (green - blue) / colMaxMin;
			}
			else if (green == colMax)
			{
				hueDeg = 2f + (blue - red) / colMaxMin;
			}
			else
			{
				hueDeg = 4f + (red - green) / colMaxMin;
			}
			
			hueDeg *= 60f;
			
			if (hueDeg < 0f)
			{
				hueDeg += 360f;
			}
			
			h = Angle.FromDegrees(hueDeg);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void GetCMY(out float c, out float m, out float y)
		{
			c = 1f - this.Red;
			m = 1f - this.Green;
			y = 1f - this.Blue;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void GetCMYK(out float c, out float m, out float y, out float k)
		{
			c = 1f - this.Red;
			m = 1f - this.Green;
			y = 1f - this.Blue;
			k = 1f;
			
			if (c < k)
			{
				k = c;
			}
			
			if (m < k)
			{
				k = m;
			}
			
			if (y < k)
			{
				k = y;
			}

			c = (c - k) / (1f - k);
			m = (m - k) / (1f - k);
			y = (y - k) / (1f - k);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public Color GetColor() => 
			new Color(this.Red, this.Green, this.Blue, this.Alpha);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static ColorF Lerp(ColorF a, ColorF b, float factor) =>
			new ColorF(a.Alpha * factor + b.Alpha * (1f - factor), 
					   a.Red * factor + b.Red * (1f - factor), 
					   a.Green * factor + b.Green * (1f - factor), 
					   a.Blue * factor + b.Blue * (1f - factor));

		private ColorF(Color col)
		{
			this._red = (float)col.R / 255f;
			this._green = (float)col.G / 255f;
			this._blue = (float)col.B / 255f;
			this._alpha = (float)col.A / 255f;
		}

		private ColorF(float a, float r, float g, float b)
		{
			this._red = r;
			this._green = g;
			this._blue = b;
			this._alpha = a;
		}

		private ColorF(Vector3 vector)
		{
			this._red = vector.X;
			this._green = vector.Y;
			this._blue = vector.Z;
			this._alpha = 1f;
		}

		private ColorF(Vector4 vector)
		{
			this._red = vector.X;
			this._green = vector.Y;
			this._blue = vector.Z;
			this._alpha = vector.W;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public override string ToString() => 
			this.Red.ToString() + "," + this.Green.ToString() + "," + 
			this.Blue.ToString() + "," + this.Alpha.ToString();

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static ColorF Parse(string strval)
		{
			string[] array = strval.Split(',');
			
			float r = float.Parse(array[0]);
			float g = float.Parse(array[1]);
			float b = float.Parse(array[2]);
			float a = float.Parse(array[3]);
			
			return ColorF.FromARGB(a, r, g, b);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public override bool Equals(object obj) => 
			obj.GetType() == typeof(ColorF) && this == (ColorF)obj;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public override int GetHashCode() => 
			throw new NotImplementedException();

		public static bool operator == (ColorF a, ColorF b) => 
			throw new NotImplementedException();

		public static bool operator != (ColorF a, ColorF b) => 
			throw new NotImplementedException();
	}
}
