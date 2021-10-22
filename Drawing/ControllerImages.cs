using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing 
{
	public static class ControllerImages 
	{
		public static Texture2D X;
		public static Texture2D Y;
		public static Texture2D A;
		public static Texture2D B;
		public static Texture2D Back;
		public static Texture2D Guide;
		public static Texture2D Start;
		public static Texture2D DPad;
		public static Texture2D LeftShoulder;
		public static Texture2D LeftThumstick;
		public static Texture2D LeftTrigger;
		public static Texture2D RightShoulder;
		public static Texture2D RightThumstick;
		public static Texture2D RightTrigger;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static void Load(ContentManager content) 
		{
			ControllerImages.X = 
				content.Load<Texture2D>("ControllerGlyphs\\ButtonX");
			
			ControllerImages.Y = 
				content.Load<Texture2D>("ControllerGlyphs\\ButtonY");
			
			ControllerImages.A = 
				content.Load<Texture2D>("ControllerGlyphs\\ButtonA");
			
			ControllerImages.B = 
				content.Load<Texture2D>("ControllerGlyphs\\ButtonB");
			
			ControllerImages.Back = 
				content.Load<Texture2D>("ControllerGlyphs\\ButtonBack");
			
			ControllerImages.Guide = 
				content.Load<Texture2D>("ControllerGlyphs\\ButtonGuide");
			
			ControllerImages.Start = 
				content.Load<Texture2D>("ControllerGlyphs\\ButtonStart");
			
			ControllerImages.DPad = 
				content.Load<Texture2D>("ControllerGlyphs\\DPad");
			
			ControllerImages.LeftShoulder = 
				content.Load<Texture2D>("ControllerGlyphs\\LeftShoulder");
			
			ControllerImages.LeftThumstick = 
				content.Load<Texture2D>("ControllerGlyphs\\LeftThumbstick");
			
			ControllerImages.LeftTrigger = 
				content.Load<Texture2D>("ControllerGlyphs\\LeftTrigger");
			
			ControllerImages.RightShoulder = 
				content.Load<Texture2D>("ControllerGlyphs\\RightShoulder");
			
			ControllerImages.RightThumstick = 
				content.Load<Texture2D>("ControllerGlyphs\\RightThumbstick");
			
			ControllerImages.RightTrigger = 
				content.Load<Texture2D>("ControllerGlyphs\\RightTrigger");
		}
	}
}
