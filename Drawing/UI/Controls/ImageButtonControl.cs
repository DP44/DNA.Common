using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI.Controls
{
	public class ImageButtonControl : ButtonControl
	{
		public Sprite Image;
		public Color ImageDefaultColor = Color.Black;

		public SpriteFont Font { get; set; }
		public string Text { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public override Size Size
		{
			get
			{
				return new Size(this.Image.Width, this.Image.Height);
			}
			set
			{

			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			Vector2 position = new Vector2((float)base.LocalPosition.X, (float)base.LocalPosition.Y);
			Vector2 position2 = new Vector2(position.X + (float)(this.Image.Width / 2) - this.Font.MeasureString(this.Text).X / 2f, position.Y + (float)(this.Image.Height / 2) - this.Font.MeasureString(this.Text).Y / 2f);
			
			if (base.CaptureInput)
			{
				spriteBatch.Draw(this.Image, position, Color.White);
				spriteBatch.DrawString(this.Font, this.Text, position2, Color.Black);
				return;
			}
			
			if (base.Hovering)
			{
				spriteBatch.Draw(this.Image, position, Color.Gray);
				spriteBatch.DrawString(this.Font, this.Text, position2, Color.White);
				return;
			}
			
			spriteBatch.Draw(this.Image, position, this.ImageDefaultColor);
			spriteBatch.DrawString(this.Font, this.Text, position2, Color.White);
		}
	}
}
