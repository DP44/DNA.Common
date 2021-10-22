using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DNA.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI.Controls
{
	public class ConsoleControl : UIControl, IConsole
	{
		private class ConsoleTextWriter : TextWriter
		{
			private ConsoleControl _owner;

			/// <summary>
			/// 
			/// </summary>
			private ConsoleControl Owner
			{
				get
				{
					return this._owner;
				}
			}

			/// <summary>
			/// 
			/// </summary>
			public override Encoding Encoding
			{
				get
				{
					return Encoding.UTF8;
				}
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public override void WriteLine(string value)
			{
				this._owner.WriteLine(value);
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public override void Write(char value)
			{
				this._owner.Write(value);
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public ConsoleTextWriter(ConsoleControl control)
			{
				this.NewLine = "\n";
				this._owner = control;
			}
		}

		private class Message
		{
			public string Text;
			public Color Color;

			/// <summary>
			/// 
			/// </summary>
			public Message() {}

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public Message(string text)
			{
				this.Text = text;
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public Message(string text, Color color)
			{
				this.Text = text;
				this.Color = color;
			}

			/// <summary>
			/// 
			/// </summary>
			public override string ToString()
			{
				return this.Text;
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public void Append(string str)
			{
				this.Text += str;
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public void Update(GameTime gameTime) {}
		}

		private ConsoleControl.ConsoleTextWriter _textWriter;
		private SpriteFont _font;
		private ConsoleControl.Message _currentMessage = new ConsoleControl.Message("");
		private Queue<ConsoleControl.Message> _messages = new Queue<ConsoleControl.Message>();
		private Size _size;
		private ConsoleControl.Message[] messages = new ConsoleControl.Message[0];

		/// <summary>
		/// 
		/// </summary>
		public override Size Size
		{
			get
			{
				return this._size;
			}
			set
			{
				this._size = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private int LinesSupported
		{
			get
			{
				return 100;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public ConsoleControl()
		{
			this._textWriter = new ConsoleControl.ConsoleTextWriter(this);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public ConsoleControl(SpriteFont font)
		{
			this._font = font;
			this._textWriter = new ConsoleControl.ConsoleTextWriter(this);
		}

		/// <summary>
		/// 
		/// </summary>
		public void GrabConsole()
		{
			GameConsole.SetControl(this);
			Console.SetOut(this._textWriter);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void Write(char value)
		{
			if (value == '\n')
			{
				this.WriteLine();
				return;
			}

			this._currentMessage.Append(value.ToString());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void Write(string value)
		{
			string[] array = value.Split(new char[] { '\n' });

			for (int i = 0; i < array.Length; i++)
			{
				this._currentMessage.Append(array[i]);
				
				if (i < array.Length - 1)
				{
					this.WriteLine();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void WriteLine(string value)
		{
			this.Write(value);
			this.WriteLine();
		}

		/// <summary>
		/// 
		/// </summary>
		public void WriteLine()
		{
			lock (this._messages)
			{
				this._messages.Enqueue(this._currentMessage);
				this._currentMessage = new ConsoleControl.Message("");
				while (this._messages.Count > this.LinesSupported)
				{
					this._messages.Dequeue();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void Clear()
		{
			lock (this._messages)
			{
				this._messages.Clear();
				this._currentMessage = new ConsoleControl.Message("");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			lock (this._messages)
			{
				if (this.messages.Length != this._messages.Count)
				{
					this.messages = new ConsoleControl.Message[this._messages.Count];
				}
			
				this._messages.CopyTo(this.messages, 0);
			}
			
			int lineSpacing = this._font.LineSpacing;
			Vector2 position = new Vector2((float)base.ScreenPosition.X, (float)base.ScreenBounds.Bottom);
			StringBuilder stringBuilder = new StringBuilder();
			
			for (int i = this.messages.Length - 1; i >= 0; i--)
			{
				ConsoleControl.Message message = this.messages[i];
				message.Update(gameTime);
				DrawingTools.SplitText(message.Text, stringBuilder, this._font, this.Size.Width);
				Vector2 vector = this._font.MeasureString(stringBuilder);
				
				if (position.Y - vector.Y < (float)base.ScreenBounds.Top)
				{
					if (vector.Y <= (float)this._font.LineSpacing)
					{
						return;
					}
					
					while (position.Y - vector.Y < (float)base.ScreenBounds.Top)
					{
						int num = stringBuilder.IndexOf('\n');
						
						if (num == -1)
						{
							throw new Exception("Don't know how this can happen");
						}
						
						stringBuilder.Remove(0, num + 1);
						vector = this._font.MeasureString(stringBuilder);
					}
				}
				
				position.Y -= vector.Y;
				spriteBatch.DrawString(this._font, stringBuilder, position, Color.Black);
				
				if (position.Y < (float)base.ScreenPosition.Y)
				{
					return;
				}
			}
		}
	}
}
