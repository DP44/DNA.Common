using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing
{
	public class UpdateEventArgs : EventArgs
	{
		public GameTime GameTime;

		/// <summary>
		/// Constructor.
		/// </summary>
		public UpdateEventArgs() {}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="gameTime">The GameTime object to use for updating events.</param>
		public UpdateEventArgs(GameTime gameTime) =>
			this.GameTime = gameTime;
	}
}
