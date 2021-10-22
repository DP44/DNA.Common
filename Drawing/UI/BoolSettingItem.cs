using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI
{
	public class BoolSettingItem : SettingItemElement
	{
		private string[] TextValues = new string[2];

		public bool On;

		public BoolSettingItem(string text, bool isOn) : base(text)
		{
			this.TextValues[0] = CommonResources.On;
			this.TextValues[1] = CommonResources.Off;
			this.On = isOn;
		}

		public BoolSettingItem(string text, bool isOn, string onText, string offText) 
			: base(text)
		{
			this.TextValues[0] = onText;
			this.TextValues[1] = offText;
			this.On = isOn;
		}

		public override void OnClicked()
		{
			this.On = !this.On;
			base.OnClicked();
		}

		public override void Decreased() => 
			this.OnClicked();

		public override void Increased() => 
			this.OnClicked();

		public override bool ChangeValue(TimeSpan elapsedGameTime) => false;

		public override void OnDraw(DNAGame _game, GraphicsDevice device, SpriteBatch spriteBatch,
									SpriteFont font, Color textColor, Color outlineColor, 
									int outlineWidth, Vector2 loc)
		{
			Rectangle titleSafeArea = device.Viewport.TitleSafeArea;
			Vector2 vector = font.MeasureString(">");
			
			float num = 
				(float)(titleSafeArea.Right - titleSafeArea.Center.X - 120) - vector.X * 2f;
			
			float num2 = (float)(titleSafeArea.Center.X + 50);
			string text = this.On ? this.TextValues[0] : this.TextValues[1];
			float x = num2 + vector.X + 10f + num / 2f - font.MeasureString(text).X / 2f;
			
			spriteBatch.DrawOutlinedText(
				font, text, new Vector2(x, loc.Y), textColor, outlineColor, outlineWidth);
			
			base.OnDraw(
				_game, device, spriteBatch, font, textColor, outlineColor, outlineWidth, loc);
		}
	}
}
