using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing
{
	public class ScreenAdjuster
	{
		private Size _authoredSize;
		private Size _screenSize;

		private Vector2 _scale;

		private Matrix _authoredToLetterBox = Matrix.Identity;
		private Matrix _authoredToClipped = Matrix.Identity;
		private Matrix _authoredToStretched = Matrix.Identity;

		/// <summary>
		/// 
		/// </summary>
		public Vector2 ScaleFactor => 
			this._scale;

		/// <summary>
		/// 
		/// </summary>
		public Size ScreenSize
		{
			get => 
				this._screenSize;
			
			set
			{
				this._screenSize = value;
				this.Recalulate();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Size AuthoredSize
		{
			get => 
				this._authoredSize;
			
			set
			{
				this._authoredSize = value;
				this.Recalulate();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Rectangle ScreenRect => 
			new Rectangle(0, 0, this._screenSize.Width, this._screenSize.Height);

		/// <summary>
		/// 
		/// </summary>
		private void Recalulate()
		{
			float authoredSize = 
				(float)this._authoredSize.Height / (float)this._authoredSize.Width;
			float screenSize = 
				(float)this._screenSize.Height / (float)this._screenSize.Width;
			
			this._scale = new Vector2(
				(float)this._screenSize.Width / (float)this._authoredSize.Width, 
				(float)this._screenSize.Height / (float)this._authoredSize.Height);
			
			this._authoredToStretched = Matrix.CreateScale(
				(float)this._screenSize.Width / (float)this._authoredSize.Width, 
				(float)this._screenSize.Height / (float)this._authoredSize.Height, 1f);
			
			if (authoredSize < screenSize)
			{
				this._authoredToClipped = 
					Matrix.CreateScale(
						(float)this._screenSize.Height / (float)this._authoredSize.Height, 
						(float)this._screenSize.Height / (float)this._authoredSize.Height, 1f) * 
						Matrix.CreateTranslation(
							new Vector3(
								((float)this._screenSize.Width - 
									(float)(this._authoredSize.Width * this._screenSize.Height) / 
									(float)this._authoredSize.Height) / 2f, 0f, 0f));
				
				this._authoredToLetterBox = 
					Matrix.CreateScale(
						(float)this._screenSize.Width / (float)this._authoredSize.Width, 
						(float)this._screenSize.Width / (float)this._authoredSize.Width, 1f) * 
						Matrix.CreateTranslation(
							new Vector3(0f, ((float)this._screenSize.Height - 
								(float)(this._authoredSize.Height * this._screenSize.Width) / 
								(float)this._authoredSize.Width) / 2f, 0f));

				return;
			}
			
			this._authoredToClipped = Matrix.CreateScale(
				(float)this._screenSize.Width / (float)this._authoredSize.Width, 
				(float)this._screenSize.Width / (float)this._authoredSize.Width, 1f) * 
					Matrix.CreateTranslation(
						new Vector3(0f, ((float)this._screenSize.Height - 
							(float)(this._authoredSize.Height * this._screenSize.Width) / 
						(float)this._authoredSize.Width) / 2f, 0f));
			
			this._authoredToLetterBox = Matrix.CreateScale(
				(float)this._screenSize.Height / (float)this._authoredSize.Height, 
				(float)this._screenSize.Height / (float)this._authoredSize.Height, 1f) * 
					Matrix.CreateTranslation(
						new Vector3(((float)this._screenSize.Width - 
							(float)(this._authoredSize.Width * this._screenSize.Height) / 
							(float)this._authoredSize.Height) / 2f, 0f, 0f));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		private static Rectangle Transform(Rectangle original, Matrix matrix)
		{
			Vector3 vector = Vector3.Transform(
				new Vector3((float)original.Left, (float)original.Top, 0f), matrix);
			
			Vector3 vector2 = Vector3.TransformNormal(
				new Vector3((float)original.Width, (float)original.Height, 0f), matrix);
			
			return new Rectangle((int)vector.X, (int)vector.Y, (int)vector2.X, (int)vector2.Y);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		private static Vector2 Transform(Vector2 original, Matrix matrix)
		{
			Vector3 newVec = 
				Vector3.Transform(new Vector3(original.X, original.Y, 0f), matrix);
			
			return new Vector2(newVec.X, newVec.Y);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public Vector2 Scale(Vector2 original) =>
			original * this._scale;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public Rectangle TransformClipped(Rectangle original) =>
			ScreenAdjuster.Transform(original, this._authoredToClipped);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public Rectangle TransformStretched(Rectangle original) =>
			ScreenAdjuster.Transform(original, this._authoredToStretched);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public Rectangle TransformLetterBox(Rectangle original) =>
			ScreenAdjuster.Transform(original, this._authoredToLetterBox);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public Vector2 TransformClipped(Vector2 original) =>
			ScreenAdjuster.Transform(original, this._authoredToClipped);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public Vector2 TransformStretched(Vector2 original) =>
			ScreenAdjuster.Transform(original, this._authoredToStretched);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public Vector2 TransformLetterBox(Vector2 original) =>
			ScreenAdjuster.Transform(original, this._authoredToLetterBox);
	}
}
