using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DNA.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI
{
	public class ConsoleElement : UIElement, IConsole
	{
		private class ConsoleTextWriter : TextWriter
		{
			private ConsoleElement _owner;

			private ConsoleElement Owner => 
				this._owner;

			public override Encoding Encoding => 
				Encoding.UTF8;

			public override void WriteLine(string value) => 
				this._owner.WriteLine(value);

			public override void Write(char value) => 
				this._owner.Write(value);

			public ConsoleTextWriter(ConsoleElement control)
			{
				this.NewLine = "\n";
				this._owner = control;
			}
		}

		private class Message
		{
			public string Text;
			public Color Color;

			private OneShotTimer lifeTimer = new OneShotTimer(TimeSpan.FromSeconds(10.0));
			private OneShotTimer fadeTimer = new OneShotTimer(TimeSpan.FromSeconds(3.0));

			public float Visibility => 1f;

			public Message() {}

			public Message(string text) => 
				this.Text = text;

			public Message(string text, Color color)
			{
				this.Text = text;
				this.Color = color;
			}

			public override string ToString() => 
				this.Text;

			public void Append(string str) => 
				this.Text += str;

			public void Update(GameTime gameTime)
			{
				this.lifeTimer.Update(gameTime.ElapsedGameTime);

				if (!this.lifeTimer.Expired)
				{
					return;
				}

				this.fadeTimer.Update(gameTime.ElapsedGameTime);
			}
		}

		private ConsoleElement.ConsoleTextWriter _textWriter;
		private SpriteFont _font;

		private ConsoleElement.Message _currentMessage = 
			new ConsoleElement.Message("");

		private Queue<ConsoleElement.Message> _messages = 
			new Queue<ConsoleElement.Message>();

		private Vector2 _size;

		private ConsoleElement.Message[] messages = new ConsoleElement.Message[0];

		private int LinesSupported => 100;

		public ConsoleElement(SpriteFont font)
		{
			this._font = font;
			this._textWriter = new ConsoleElement.ConsoleTextWriter(this);
		}

		public void GrabConsole()
		{
			GameConsole.SetControl(this);
			Console.SetOut(this._textWriter);
		}

		public void Write(char value)
		{
			if (value == '\n')
			{
				this.WriteLine();
			}
			else
			{
				this._currentMessage.Append(value.ToString());
			}
		}

		public void Write(string value)
		{
			string[] lines = value.Split('\n');
			
			for (int i = 0; i < lines.Length; i++)
			{
				this._currentMessage.Append(lines[i]);
				
				if (i < lines.Length - 1)
				{
					this.WriteLine();
				}
			}
		}

		public void WriteLine(string value)
		{
			this.Write(value);
			this.WriteLine();
		}

		public void WriteLine()
		{
			lock (this._messages)
			{
				this._messages.Enqueue(this._currentMessage);
				this._currentMessage = new ConsoleElement.Message("");
				
				while (this._messages.Count > this.LinesSupported)
				{
					this._messages.Dequeue();
				}
			}
		}

		public void Clear()
		{
			lock (this._messages)
			{
				this._messages.Clear();
				this._currentMessage = new ConsoleElement.Message("");
			}
		}

		public override Vector2 Size
		{
			get => 
				this._size;

			set => 
				this._size = value;
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, 
									   GameTime gameTime, bool selected)
		{
			lock (this._messages)
			{
				if (this.messages.Length != this._messages.Count)
				{
					this.messages = new ConsoleElement.Message[this._messages.Count];
				}
			
				this._messages.CopyTo(this.messages, 0);
			}
			
			int lineSpacing = this._font.LineSpacing;
			
			Vector2 location = new Vector2(
				base.Location.X, base.Location.Y + this.Size.Y - (float)lineSpacing);
			
			spriteBatch.Begin(SpriteSortMode.Deferred, 
				BlendState.AlphaBlend, SamplerState.AnisotropicClamp, 
				DepthStencilState.DepthRead, RasterizerState.CullNone);
			
			for (int i = this.messages.Length - 1; i >= 0; i--)
			{
				ConsoleElement.Message message = this.messages[i];
				message.Update(gameTime);
			
				if (message.Visibility > 0f)
				{
					spriteBatch.DrawOutlinedText(
						this._font, message.Text, location, 
						Color.Lerp(Color.Transparent, base.Color, message.Visibility), 
						Color.Lerp(Color.Transparent, Color.Black, message.Visibility), 1);
				}
			
				location.Y -= (float)lineSpacing;
			
				if (location.Y < base.Location.Y)
				{
					break;
				}
			}
			
			spriteBatch.End();
		}
	}
}
