using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing
{
	public static class ColorTools
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static Color Blend(Color c1, Color c2, float factor)
		{
			int a = 
				(int)Math.Round((double)((float)c1.A * (1f - factor) + (float)c2.A * factor));
			
			int r = 
				(int)Math.Round((double)((float)c1.R * (1f - factor) + (float)c2.R * factor));
			
			int g = 
				(int)Math.Round((double)((float)c1.G * (1f - factor) + (float)c2.G * factor));
			
			int b = 
				(int)Math.Round((double)((float)c1.B * (1f - factor) + (float)c2.B * factor));
			
			return new Color(r, g, b, a);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static Color FromAHSB(Angle h, float s, float b) => 
			ColorTools.FromAHSB(1f, h, s, b);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static Color FromAHSL(Angle h, float s, float b) => 
			ColorTools.FromAHSL(1f, h, s, b);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static Color FromAHSV(Angle h, float s, float b) => 
			ColorTools.FromAHSV(1f, h, s, b);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static Color FromAHSB(float alpha, Angle hue, float sat, float brt) => 
			ColorTools.FromAHSL(alpha, hue, sat, brt);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static Color FromAHSL(float alpha, Angle hue, float sat, float brt) => 
			ColorF.FromAHSL(alpha, hue, sat, brt).GetColor();

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static Color FromCMY(float c, float m, float y) => 
			ColorTools.FromACMY(1f, c, m, y);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static Color FromACMY(float alpha, float c, float m, float y) => 
			ColorF.FromACMY(alpha, c, m, y).GetColor();

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static Color FromCMYK(float alpha, float c, float m, float y, float k) => 
			ColorTools.FromACMYK(1f, c, m, y, k);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static Color FromACMYK(float alpha, float c, float m, float y, float k) => 
			ColorF.FromACMYK(alpha, c, m, y, k).GetColor();

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static void ToCMYK(Color color, out float c, out float m, 
								  out float y, out float k) =>
			ColorTools.ToACMYK(color, out float _, out c, out m, out y, out k);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static void ToCMY(Color color, out float c, out float m, out float y) => 
			ColorF.FromColor(color).GetCMY(out c, out m, out y);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static void ToACMY(Color color, out float alpha, out float c, 
								  out float m, out float y)
		{
			alpha = (float)color.A / 255f;
			ColorF.FromColor(color).GetCMY(out c, out m, out y);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static void ToACMYK(Color color, out float alpha, out float c, 
								   out float m, out float y, out float k)
		{
			alpha = (float)color.A / 255f;
			ColorF.FromColor(color).GetCMYK(out c, out m, out y, out k);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static void ToAHSL(Color color, out float alpha, out Angle h, 
								  out float s, out float l)
		{
			ColorF colorF = ColorF.FromColor(color);
			alpha = (float)color.A / 255f;
			colorF.GetHSL(out h, out s, out l);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static void ToAHSV(Color color, out float alpha, out Angle h, 
								  out float s, out float v)
		{
			alpha = (float)color.A / 255f;
			ColorF.FromColor(color).GetHSV(out h, out s, out v);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static Color FromAHSV(float alpha, Angle hue, float sat, float brt) =>
			ColorF.FromAHSV(alpha, hue, sat, brt).GetColor();

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static Color GetRandomColor(Random rnd) =>
			new Color(rnd.Next(255), rnd.Next(255), rnd.Next(255));

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static Color GetRandomColor(Random rnd, float saturation, float brightness) => 
			ColorTools.FromAHSV(Angle.FromRevolutions((float)rnd.NextDouble()), 
								saturation, brightness);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static Color Brighten(Color c, float factor) => 
			(Color)((ColorF)c).Brighten(factor);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static Color Saturate(Color c, float factor) => 
			(Color)((ColorF)c).Saturate(factor);
	}
}
