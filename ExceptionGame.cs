using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA
{
	public class ExceptionGame : Game
	{
		private Exception _lastError;

		private string _link;

		private string _gameName;

		private string _version;

		private readonly Exception exception;

		private SpriteBatch batch;

		private SpriteFont font;

		private DateTime _startTime;

		private TimeSpan _runTime;

		public ExceptionGame(Exception e, string urlLink, string gameName, Version version, DateTime startTime)
		{
			this._lastError = e;
			this._link = urlLink;
			this._gameName = gameName;
			this._version = version.ToString();
			this._startTime = startTime;
			this._runTime = DateTime.UtcNow - this._startTime;
			GraphicsDeviceManager graphicsDeviceManager = new GraphicsDeviceManager(this);
			graphicsDeviceManager.PreferredBackBufferWidth = 1280;
			graphicsDeviceManager.PreferredBackBufferHeight = 720;
			this.exception = e;
			base.Content.RootDirectory = "Content";
		}

		protected override void LoadContent()
		{
			this.batch = new SpriteBatch(base.GraphicsDevice);
			this.font = base.Content.Load<SpriteFont>("Debug");
		}

		protected override void Draw(GameTime gameTime)
		{
			base.GraphicsDevice.Clear(Color.Black);
			this.batch.Begin();
			this._lastError = this._lastError.GetBaseException();
			base.GraphicsDevice.Clear(Color.Black);
			Rectangle titleSafeArea = base.GraphicsDevice.Viewport.TitleSafeArea;
			int num = titleSafeArea.Top;
			string name = this._lastError.GetType().Name;
			this.batch.DrawString(this.font, "Sorry, this game has encountered an error", new Vector2((float)titleSafeArea.X, (float)num), Color.White);
			num += this.font.LineSpacing;
			this.batch.DrawString(this.font, "Take a picture of this screen and email it to: " + this._link, new Vector2((float)titleSafeArea.X, (float)num), Color.White);
			num += this.font.LineSpacing;
			this.batch.DrawString(this.font, this._gameName + ", Version " + this._version + " ", new Vector2((float)titleSafeArea.X, (float)num), Color.White);
			num += this.font.LineSpacing;
			this.batch.DrawString(this.font, this._startTime.ToString("MM/dd/yy HH:mm") + " " + this._runTime.ToString(), new Vector2((float)titleSafeArea.X, (float)num), Color.White);
			num += this.font.LineSpacing * 2;
			this.batch.DrawString(this.font, this._lastError.Message, new Vector2((float)titleSafeArea.X, (float)num), Color.White);
			num += this.font.LineSpacing * 2;
			this.batch.DrawString(this.font, this._lastError.GetType().Name, new Vector2((float)titleSafeArea.X, (float)num), Color.White);
			num += this.font.LineSpacing * 2;
			string[] array = this._lastError.StackTrace.Split(new char[]
			{
				'\n'
			});
			foreach (string text in array)
			{
				string text2 = text.Trim();
				this.batch.DrawString(this.font, text2, new Vector2((float)titleSafeArea.X, (float)num), Color.White);
				num += this.font.LineSpacing;
			}
			this.batch.End();
			base.Draw(gameTime);
		}
	}
}
