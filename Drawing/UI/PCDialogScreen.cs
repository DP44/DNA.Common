using System;
using System.Collections.Generic;
using System.Threading;
using DNA.Audio;
using DNA.Drawing.UI.Controls;
using DNA.Input;
using DNA.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DNA.Drawing.UI
{
	public class PCDialogScreen : UIControlScreen
	{
		public string Title;

		private string[] _options;

		protected Texture2D _bgImage;

		protected SpriteFont _font;

		public Vector2 TitlePadding = new Vector2(20f, 5f);

		public Vector2 DescriptionPadding = new Vector2(10f, 10f);

		public Vector2 OptionsPadding = new Vector2(10f, 10f);

		public Vector2 ButtonsPadding = new Vector2(10f, 10f);

		public Color TitleColor = Color.White;

		public Color DescriptionColor = Color.White;

		public Color OptionsColor = Color.White;

		public Color OptionsSelectedColor = Color.Red;

		public Color ButtonsColor = Color.White;

		public string ClickSound;

		public string OpenSound;

		private TextRegionElement _descriptionText;

		private List<string> optionLinesToPrint = new List<string>();

		private List<int> optionsStartLine = new List<int>();

		private bool _optionsLinesCalculated;

		protected int _optionSelected = -1;

		private int optionCurrentlySelected;

		private OneShotTimer flashTimer = new OneShotTimer(TimeSpan.FromSeconds(0.25));

		private bool selectedDirection;

		private bool JoystickMoved;

		public ThreadStart Callback;

		private Texture2D DummyTexture;

		private FrameButtonControl _button0 = new FrameButtonControl();

		private FrameButtonControl _button1 = new FrameButtonControl();

		private FrameButtonControl _button2 = new FrameButtonControl();

		private FrameButtonControl _button3 = new FrameButtonControl();

		public static Vector2 DefaultTitlePadding;

		public static Vector2 DefaultDescriptionPadding;

		public static Vector2 DefaultButtonsPadding;

		public static string DefaultClickSound;

		public static string DefaultOpenSound;

		protected float _endOfDescriptionLoc;

		public int OptionSelected
		{
			get
			{
				return this._optionSelected;
			}
		}

		public void UseDefaultValues()
		{
			this.TitlePadding = PCDialogScreen.DefaultTitlePadding;
			this.DescriptionPadding = PCDialogScreen.DefaultDescriptionPadding;
			this.ButtonsPadding = PCDialogScreen.DefaultButtonsPadding;
			this.ClickSound = PCDialogScreen.DefaultClickSound;
			this.OpenSound = PCDialogScreen.DefaultOpenSound;
		}

		public PCDialogScreen(string title, string description, string[] options, bool printCancel, Texture2D bgImage, SpriteFont font, bool drawBehind, ScalableFrame frame) : base(drawBehind)
		{
			this.Title = title;
			this._descriptionText = new TextRegionElement(description, font);
			this._descriptionText.OutlineWidth = 1;
			this._descriptionText.ScaleOnScreenResize = false;
			if (options != null)
			{
				this._options = new string[options.Length];
			}
			this._options = options;
			this._bgImage = bgImage;
			this._font = font;
			if (options == null)
			{
				this.optionCurrentlySelected = 0;
			}
			this._button0.LocalPosition = new Point(400, 155);
			this._button0.Size = new Size(150, 35);
			this._button0.Text = CommonResources.OK;
			this._button0.Font = this._font;
			this._button0.Frame = frame;
			this._button0.Pressed += this._button0_Pressed;
			this._button0.ButtonColor = new Color(0.6f, 0.6f, 0.6f, 0.6f);
			base.Controls.Add(this._button0);
			if (printCancel)
			{
				this._button1.LocalPosition = new Point(400, 155);
				this._button1.Size = new Size(150, 35);
				this._button1.Text = CommonResources.Cancel;
				this._button1.Font = this._font;
				this._button1.Frame = frame;
				this._button1.Pressed += this._button1_Pressed;
				this._button1.ButtonColor = new Color(0.6f, 0.6f, 0.6f, 0.6f);
				base.Controls.Add(this._button1);
			}
		}

		private void _button1_Pressed(object sender, EventArgs e)
		{
			this._optionSelected = -1;
			if (this.ClickSound != null)
			{
				SoundManager.Instance.PlayInstance(this.ClickSound);
			}
			base.PopMe();
			if (this.Callback != null)
			{
				this.Callback();
			}
		}

		private void _button0_Pressed(object sender, EventArgs e)
		{
			this._optionSelected = this.optionCurrentlySelected;
			if (this.ClickSound != null)
			{
				SoundManager.Instance.PlayInstance(this.ClickSound);
			}
			base.PopMe();
			if (this.Callback != null)
			{
				this.Callback();
			}
		}

		public override void OnPushed()
		{
			if (this.OpenSound != null)
			{
				SoundManager.Instance.PlayInstance(this.OpenSound);
			}
			float w = (float)this._bgImage.Width;
			this.GetOptionsLines(w);
			base.OnPushed();
		}

		private void GetOptionsLines(float w)
		{
			if (!this._optionsLinesCalculated && this._options != null)
			{
				this._optionsLinesCalculated = true;
				for (int i = 0; i < this._options.Length; i++)
				{
					string text = this._options[i];
					float num = 0f;
					int num2 = 0;
					int num3 = 0;
					if (text != null)
					{
						for (int j = 0; j < text.Length; j++)
						{
							if (text[j] == '\n')
							{
								if (this._font.MeasureString(text.Substring(num3, j - num3 + 1)).X > w - this.OptionsPadding.X * 2f)
								{
									this.optionLinesToPrint.Add(text.Substring(num3, num2 - num3));
									if (this.optionsStartLine.Count < i + 1)
									{
										this.optionsStartLine.Add(this.optionLinesToPrint.Count - 1);
									}
									this.optionLinesToPrint.Add(text.Substring(num2 + 1, j - num2));
								}
								else
								{
									this.optionLinesToPrint.Add(text.Substring(num3, j - num3));
									if (this.optionsStartLine.Count < i + 1)
									{
										this.optionsStartLine.Add(this.optionLinesToPrint.Count - 1);
									}
								}
								num3 = j + 1;
								num = 0f;
								num2 = j;
							}
							if (text[j] == ' ')
							{
								float x = this._font.MeasureString(text.Substring(num2, j - num2)).X;
								num += x;
								if (num > w - this.OptionsPadding.X * 2f)
								{
									this.optionLinesToPrint.Add(text.Substring(num3, num2 - num3 + 1));
									if (this.optionsStartLine.Count < i + 1)
									{
										this.optionsStartLine.Add(this.optionLinesToPrint.Count - 1);
									}
									num3 = num2 + 1;
									num = x;
									num2 = j + 1;
								}
								else
								{
									num2 = j;
								}
							}
							if (j == text.Length - 1)
							{
								if (this._font.MeasureString(text.Substring(num3, j - num3 + 1)).X > w - this.OptionsPadding.X * 2f)
								{
									this.optionLinesToPrint.Add(text.Substring(num3, num2 - num3 + 1));
									if (this.optionsStartLine.Count < i + 1)
									{
										this.optionsStartLine.Add(this.optionLinesToPrint.Count - 1);
									}
									this.optionLinesToPrint.Add(text.Substring(num2 + 1, j - num2));
								}
								else
								{
									this.optionLinesToPrint.Add(text.Substring(num3, j - num3 + 1));
									if (this.optionsStartLine.Count < i + 1)
									{
										this.optionsStartLine.Add(this.optionLinesToPrint.Count - 1);
									}
								}
							}
						}
					}
				}
			}
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			Rectangle screenRect = Screen.Adjuster.ScreenRect;
			float num = (float)this._bgImage.Width;
			float num2 = (float)this._bgImage.Height;
			Rectangle destinationRectangle = new Rectangle((int)((float)screenRect.Center.X - num / 2f), (int)((float)screenRect.Center.Y - num2 / 2f), (int)num, (int)num2);
			if (this.DummyTexture == null)
			{
				this.DummyTexture = new Texture2D(device, 1, 1);
				this.DummyTexture.SetData<Color>(new Color[]
				{
					Color.White
				});
			}
			spriteBatch.Begin();
			Rectangle destinationRectangle2 = new Rectangle(0, 0, device.Viewport.Width, device.Viewport.Height);
			spriteBatch.Draw(this.DummyTexture, destinationRectangle2, new Color(0f, 0f, 0f, 0.5f));
			spriteBatch.Draw(this._bgImage, destinationRectangle, Color.White);
			spriteBatch.DrawOutlinedText(this._font, this.Title, new Vector2((float)destinationRectangle.X + this.TitlePadding.X, (float)destinationRectangle.Y + this.TitlePadding.Y), this.TitleColor, Color.Black, 1);
			float num3 = (float)destinationRectangle.Y + this.DescriptionPadding.Y;
			float num4 = (float)this._font.LineSpacing;
			float x = (float)destinationRectangle.X + this.DescriptionPadding.X;
			float num5 = (float)destinationRectangle.Y + this.DescriptionPadding.Y + (float)this._font.LineSpacing;
			this._descriptionText.Location = new Vector2(x, num5);
			this._descriptionText.Size = new Vector2(num - this.DescriptionPadding.X * 2f, num2 - this.DescriptionPadding.Y - (float)this._font.LineSpacing);
			this._descriptionText.Draw(device, spriteBatch, gameTime, false);
			this._endOfDescriptionLoc = num5 + num4;
			num3 = (float)(destinationRectangle.Y + destinationRectangle.Height) - this.OptionsPadding.Y - this._font.MeasureString(this.Title).Y * (float)(this.optionLinesToPrint.Count + 2) - this.ButtonsPadding.Y;
			for (int i = 0; i < this.optionLinesToPrint.Count; i++)
			{
				if (i >= this.optionsStartLine[this.optionCurrentlySelected])
				{
					if (this.optionCurrentlySelected == this._options.Length - 1 || i < this.optionsStartLine[this.optionCurrentlySelected + 1])
					{
						if (i == this.optionsStartLine[this.optionCurrentlySelected])
						{
							this.flashTimer.Update(gameTime.ElapsedGameTime);
							if (this.flashTimer.Expired)
							{
								this.flashTimer.Reset();
								this.selectedDirection = !this.selectedDirection;
							}
						}
						Color textColor;
						if (this.selectedDirection)
						{
							textColor = Color.Lerp(this.OptionsColor, this.OptionsSelectedColor, this.flashTimer.PercentComplete);
						}
						else
						{
							textColor = Color.Lerp(this.OptionsSelectedColor, this.OptionsColor, this.flashTimer.PercentComplete);
						}
						num3 += this._font.MeasureString(this.Title).Y;
						spriteBatch.DrawOutlinedText(this._font, this.optionLinesToPrint[i], new Vector2((float)destinationRectangle.X + this.OptionsPadding.X, num3), textColor, Color.Black, 1);
					}
					else
					{
						num3 += this._font.MeasureString(this.Title).Y;
						spriteBatch.DrawOutlinedText(this._font, this.optionLinesToPrint[i], new Vector2((float)destinationRectangle.X + this.OptionsPadding.X, num3), this.OptionsColor, Color.Black, 1);
					}
				}
				else
				{
					num3 += this._font.MeasureString(this.Title).Y;
					spriteBatch.DrawOutlinedText(this._font, this.optionLinesToPrint[i], new Vector2((float)destinationRectangle.X + this.OptionsPadding.X, num3), this.OptionsColor, Color.Black, 1);
				}
			}
			num3 = (float)(destinationRectangle.Y + destinationRectangle.Height) - this.ButtonsPadding.Y - this._font.MeasureString(this.Title).Y + 5f;
			this._button0.LocalPosition = new Point((int)((float)destinationRectangle.X + this.ButtonsPadding.X), (int)num3);
			if (base.Controls.Contains(this._button1))
			{
				this._button1.LocalPosition = new Point((int)(this.ButtonsPadding.X + (float)this._button0.LocalBounds.Right), (int)num3);
				if (base.Controls.Contains(this._button2))
				{
					this._button2.LocalPosition = new Point((int)(this.ButtonsPadding.X + (float)this._button1.LocalBounds.Right), (int)num3);
					if (base.Controls.Contains(this._button3))
					{
						this._button3.LocalPosition = new Point((int)(this.ButtonsPadding.X + (float)this._button2.LocalBounds.Right), (int)num3);
					}
				}
			}
			spriteBatch.End();
			base.OnDraw(device, spriteBatch, gameTime);
		}

		protected override bool OnPlayerInput(InputManager input, GameController controller, KeyboardInput chatpad, GameTime gameTime)
		{
			if (controller.PressedButtons.A || controller.PressedButtons.Start || input.Keyboard.WasKeyPressed(Keys.Enter))
			{
				this._optionSelected = this.optionCurrentlySelected;
				if (this.ClickSound != null)
				{
					SoundManager.Instance.PlayInstance(this.ClickSound);
				}
				base.PopMe();
				if (this.Callback != null)
				{
					this.Callback();
				}
			}
			if (controller.PressedButtons.Back || controller.PressedButtons.B || input.Keyboard.WasKeyPressed(Keys.Escape))
			{
				this._optionSelected = -1;
				if (this.ClickSound != null)
				{
					SoundManager.Instance.PlayInstance(this.ClickSound);
				}
				base.PopMe();
				if (this.Callback != null)
				{
					this.Callback();
				}
			}
			if (controller.CurrentState.ThumbSticks.Left.Y > 0f || controller.CurrentState.IsButtonDown(Buttons.DPadUp) || input.Keyboard.IsKeyDown(Keys.Up))
			{
				if (this._options != null && !this.JoystickMoved)
				{
					this.JoystickMoved = true;
					if (this.optionCurrentlySelected > 0)
					{
						this.optionCurrentlySelected--;
						if (this.ClickSound != null)
						{
							SoundManager.Instance.PlayInstance(this.ClickSound);
						}
					}
				}
			}
			else if (controller.CurrentState.ThumbSticks.Left.Y < 0f || controller.CurrentState.IsButtonDown(Buttons.DPadDown) || input.Keyboard.IsKeyDown(Keys.Down))
			{
				if (this._options != null && !this.JoystickMoved)
				{
					this.JoystickMoved = true;
					if (this.optionCurrentlySelected < this._options.Length - 1)
					{
						this.optionCurrentlySelected++;
						if (this.ClickSound != null)
						{
							SoundManager.Instance.PlayInstance(this.ClickSound);
						}
					}
				}
			}
			else if (this.JoystickMoved)
			{
				this.JoystickMoved = false;
			}
			return base.OnPlayerInput(input, controller, chatpad, gameTime);
		}
	}
}
