using System;
using System.Text;
using DNA.Audio;
using DNA.Drawing.UI.Controls;
using DNA.Input;
using DNA.Text;
using DNA.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DNA.Drawing.UI
{
	public class AchievementScreen<T> : UIControlScreen where T : PlayerStats
	{
		public const float deadZone = 0.25f;

		private AchievementManager<T> _achievementManager;

		private int MaxAchievementsToDisplay = 6;

		private int TopDisplayIndex;

		private SpriteFont _largeFont;

		private SpriteFont _smallFont;

		private Color _mainTextColor = new Color(225, 229, 220);

		private Color _otherTextColor = new Color(115, 131, 136);

		private Color _backColor = new Color(26, 27, 26);

		private Color _progressBackColor = new Color(38, 38, 38);

		private Color _progressColor = new Color(68, 68, 67);

		private Color _progressOutlineColor = new Color(60, 57, 52);

		private Texture2D _dummyTexture;

		private StringBuilder sbuilder = new StringBuilder();

		private OneShotTimer holdTimer = new OneShotTimer(TimeSpan.FromSeconds(0.5));

		private OneShotTimer scrollTimer = new OneShotTimer(TimeSpan.FromSeconds(0.10000000149011612));

		private bool lastselectup;

		private bool lastselectdown;

		public string ClickSound;

		private bool _mouseMovement;

		public AchievementScreen(AchievementManager<T> achievementManager, SpriteFont largeFont, SpriteFont smallFont, Texture2D dummyTexture) : base(false)
		{
			this._achievementManager = achievementManager;
			this._largeFont = largeFont;
			this._smallFont = smallFont;
			this._dummyTexture = dummyTexture;
		}

		public override void Draw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			Rectangle rectangle = new Rectangle(Screen.Adjuster.ScreenRect.Center.X - 412, Screen.Adjuster.ScreenRect.Center.Y - 288, 824, 576);
			Vector2 location = new Vector2((float)(rectangle.X + 10), (float)(rectangle.Y + 15));
			spriteBatch.Begin();
			spriteBatch.Draw(this._dummyTexture, new Rectangle(rectangle.X, rectangle.Top, rectangle.Width, rectangle.Height), Color.Black);
			spriteBatch.Draw(this._dummyTexture, new Rectangle(rectangle.X + 5, rectangle.Top + 5, rectangle.Width - 10, rectangle.Height - 10), this._backColor);
			spriteBatch.DrawOutlinedText(this._largeFont, CommonResources.Awards, location, this._mainTextColor, this._progressOutlineColor, 2);
			int num = 0;
			for (int i = 0; i < this._achievementManager.Count; i++)
			{
				if (this._achievementManager[i].Acheived)
				{
					num++;
				}
			}
			this.sbuilder.Length = 0;
			this.sbuilder.Concat(num);
			this.sbuilder.Append("/");
			this.sbuilder.Concat(this._achievementManager.Count);
			spriteBatch.DrawOutlinedText(this._largeFont, this.sbuilder, new Vector2((float)(rectangle.X + rectangle.Width - 110) - this._largeFont.MeasureString(this.sbuilder).X, location.Y), this._mainTextColor, this._progressOutlineColor, 2);
			location.X += 65f;
			location.Y += 75f;
			float num2 = this._smallFont.MeasureString("OK").Y - 5f;
			for (int j = 0; j < this.MaxAchievementsToDisplay; j++)
			{
				spriteBatch.Draw(this._dummyTexture, new Rectangle((int)location.X, (int)location.Y, 700, 70), this._progressOutlineColor);
				spriteBatch.Draw(this._dummyTexture, new Rectangle((int)location.X + 2, (int)location.Y + 2, 696, 66), this._progressBackColor);
				spriteBatch.Draw(this._dummyTexture, new Rectangle((int)location.X + 2, (int)location.Y + 2, (int)(696f * this._achievementManager[j + this.TopDisplayIndex].ProgressTowardsUnlock), 66), this._progressColor);
				spriteBatch.DrawOutlinedText(this._smallFont, this._achievementManager[j + this.TopDisplayIndex].Name, new Vector2(location.X + 10f, location.Y + 35f - num2), this._mainTextColor, Color.Black, 1);
				spriteBatch.DrawOutlinedText(this._smallFont, this._achievementManager[j + this.TopDisplayIndex].HowToUnlock, new Vector2(location.X + 10f, location.Y + 35f), this._otherTextColor, Color.Black, 1);
				if (this._achievementManager[j + this.TopDisplayIndex].Reward == null)
				{
					spriteBatch.DrawOutlinedText(this._smallFont, this._achievementManager[j + this.TopDisplayIndex].ProgressTowardsUnlockMessage, new Vector2(location.X + 690f - this._smallFont.MeasureString(this._achievementManager[j + this.TopDisplayIndex].ProgressTowardsUnlockMessage).X, location.Y + 35f - num2 / 2f), this._mainTextColor, Color.Black, 1);
				}
				else
				{
					spriteBatch.DrawOutlinedText(this._smallFont, this._achievementManager[j + this.TopDisplayIndex].ProgressTowardsUnlockMessage, new Vector2(location.X + 690f - this._smallFont.MeasureString(this._achievementManager[j + this.TopDisplayIndex].ProgressTowardsUnlockMessage).X, location.Y + 35f - num2), this._mainTextColor, Color.Black, 1);
					spriteBatch.DrawOutlinedText(this._smallFont, this._achievementManager[j + this.TopDisplayIndex].Reward, new Vector2(location.X + 690f - this._smallFont.MeasureString(this._achievementManager[j + this.TopDisplayIndex].Reward).X, location.Y + 35f), this._otherTextColor, Color.Black, 1);
				}
				location.Y += 80f;
			}
			spriteBatch.End();
			base.Draw(device, spriteBatch, gameTime);
		}

		private void PlayClickSound()
		{
			if (this.ClickSound != null)
			{
				SoundManager.Instance.PlayInstance(this.ClickSound);
			}
		}

		protected override bool OnPlayerInput(InputManager input, GameController controller, KeyboardInput chatpad, GameTime gameTime)
		{
			bool flag = false;
			bool flag2 = false;
			if (input.Mouse.DeltaPosition.X != 0f || input.Mouse.DeltaPosition.Y != 0f)
			{
				this._mouseMovement = true;
			}
			else if (input.Mouse.DeltaWheel != 0 || input.Keyboard.CurrentState.IsKeyDown(Keys.Down) || input.Keyboard.IsKeyDown(Keys.Up))
			{
				this._mouseMovement = false;
			}
			if (controller.CurrentState.ThumbSticks.Left.Y < -0.25f || controller.CurrentState.DPad.Down == ButtonState.Pressed || controller.CurrentState.Triggers.Right > 0.25f || input.Keyboard.IsKeyDown(Keys.Down) || (input.Mouse.Position.Y > 480 && this._mouseMovement))
			{
				flag2 = true;
			}
			bool flag3 = input.Mouse.Position.Y < 240 && this._mouseMovement;
			if (controller.CurrentState.ThumbSticks.Left.Y > 0.25f || controller.CurrentState.DPad.Up == ButtonState.Pressed || controller.CurrentState.Triggers.Left > 0.25f || input.Keyboard.IsKeyDown(Keys.Up) || flag3)
			{
				flag = true;
			}
			if (input.Mouse.DeltaWheel < 0)
			{
				if (this.TopDisplayIndex < this._achievementManager.Count - this.MaxAchievementsToDisplay)
				{
					this.PlayClickSound();
				}
				this.TopDisplayIndex++;
				flag2 = false;
			}
			else if (input.Mouse.DeltaWheel > 0)
			{
				if (this.TopDisplayIndex > 0)
				{
					this.PlayClickSound();
				}
				this.TopDisplayIndex--;
				flag = false;
			}
			if ((controller.CurrentState.ThumbSticks.Left.Y < -0.25f && controller.LastState.ThumbSticks.Left.Y > -0.25f) || (controller.PressedDPad.Down || (controller.CurrentState.Triggers.Right > 0.25f && controller.LastState.Triggers.Right < 0.25f)) || input.Keyboard.WasKeyPressed(Keys.Down) || (input.Mouse.Position.Y > 480 && input.Mouse.LastPosition.Y <= 480))
			{
				if (this.TopDisplayIndex < this._achievementManager.Count - this.MaxAchievementsToDisplay)
				{
					this.PlayClickSound();
				}
				this.TopDisplayIndex++;
				flag2 = false;
			}
			if ((controller.CurrentState.ThumbSticks.Left.Y > 0.25f && controller.LastState.ThumbSticks.Left.Y < 0.25f) || (controller.PressedDPad.Up || (controller.CurrentState.Triggers.Left > 0.25f && controller.LastState.Triggers.Left < 0.25f)) || input.Keyboard.WasKeyPressed(Keys.Up) || (input.Mouse.Position.Y < 240 && input.Mouse.Position.Y >= 240))
			{
				if (this.TopDisplayIndex > 0)
				{
					this.PlayClickSound();
				}
				this.TopDisplayIndex--;
				flag = false;
			}
			if (flag2)
			{
				this.holdTimer.Update(gameTime.ElapsedGameTime);
				if (this.holdTimer.Expired)
				{
					this.scrollTimer.Update(gameTime.ElapsedGameTime);
					if (this.scrollTimer.Expired)
					{
						if (this.TopDisplayIndex < this._achievementManager.Count - this.MaxAchievementsToDisplay)
						{
							this.PlayClickSound();
						}
						this.scrollTimer.Reset();
						this.TopDisplayIndex++;
					}
				}
			}
			else if (flag)
			{
				this.holdTimer.Update(gameTime.ElapsedGameTime);
				if (this.holdTimer.Expired)
				{
					this.scrollTimer.Update(gameTime.ElapsedGameTime);
					if (this.scrollTimer.Expired)
					{
						if (this.TopDisplayIndex > 0)
						{
							this.PlayClickSound();
						}
						this.scrollTimer.Reset();
						this.TopDisplayIndex--;
					}
				}
			}
			if (this.TopDisplayIndex < 0)
			{
				this.TopDisplayIndex = 0;
			}
			if (this.TopDisplayIndex > this._achievementManager.Count - this.MaxAchievementsToDisplay)
			{
				this.TopDisplayIndex = this._achievementManager.Count - this.MaxAchievementsToDisplay;
			}
			if (controller.PressedButtons.A || controller.PressedButtons.B || controller.PressedButtons.Back || input.Keyboard.WasKeyPressed(Keys.Escape))
			{
				this.PlayClickSound();
				base.PopMe();
			}
			base.OnPlayerInput(input, controller, chatpad, gameTime);
			if ((!flag2 && this.lastselectdown) || (!flag && this.lastselectup))
			{
				this.holdTimer.Reset();
			}
			this.lastselectdown = flag2;
			this.lastselectup = flag;
			return base.OnPlayerInput(input, controller, chatpad, gameTime);
		}
	}
}
