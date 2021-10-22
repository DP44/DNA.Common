using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public class DrawEventArgs : EventArgs
	{
		public GraphicsDevice Device;
		public GameTime GameTime;

		/// <summary>
		/// Constructor.
		/// </summary>
		public DrawEventArgs() {}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name=""></param>
		public DrawEventArgs(GraphicsDevice device, GameTime gameTime)
		{
			this.Device = device;
			this.GameTime = gameTime;
		}
	}
}
