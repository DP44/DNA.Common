using System;

namespace DNA 
{
	public interface IConsole 
	{
		/// <summary>
		/// Writes a message to the console.
		/// </summary>
		/// <param name="value">The message to write.</param>
		void Write(char value);

		/// <summary>
		/// Writes a message to the console.
		/// </summary>
		/// <param name="value">The message to write.</param>
		void Write(string value);

		/// <summary>
		/// Writes a message to the console and ends it with a newline.
		/// </summary>
		/// <param name="value">The message to write.</param>
		void WriteLine(string value);
		
		/// <summary>
		/// Outputs a newline to the console.
		/// </summary>
		void WriteLine();
	}
}
