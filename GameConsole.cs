using System;

namespace DNA 
{
	public static class GameConsole 
	{
		private static IConsole _control;

		/// <summary>
		/// Sets the control interface for the console.
		/// </summary>
		/// <param name="control">The IConsole interface to set.</param>
		public static void SetControl(IConsole control) => 
			GameConsole._control = control;

		/// <summary>
		/// Writes a message to the console.
		/// </summary>
		/// <param name="value">The message to write.</param>
		public static void Write(char value) 
		{
			if (GameConsole._control == null)
			{
				return;
			}

			GameConsole._control.Write(value);
		}

		/// <summary>
		/// Writes a message to the console.
		/// </summary>
		/// <param name="value">The message to write.</param>
		public static void Write(string value) 
		{
			if (GameConsole._control == null)
			{
				return;
			}

			GameConsole._control.Write(value);
		}

		/// <summary>
		/// Writes a message to the console and ends it with a newline.
		/// </summary>
		/// <param name="value">The message to write.</param>
		public static void WriteLine(string value) 
		{
			if (GameConsole._control == null)
			{
				return;
			}

			GameConsole._control.WriteLine(value);
		}

		/// <summary>
		/// Outputs a newline to the console.
		/// </summary>
		public static void WriteLine() => 
			GameConsole._control.WriteLine();
	}
}
