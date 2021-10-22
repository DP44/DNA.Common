using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public class OrthoCamera : Camera
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public override Matrix GetProjection(GraphicsDevice device) => 
			throw new NotImplementedException();

		/// <summary>
		/// 
		/// </summary>
		public override Matrix View => 
			throw new NotImplementedException();
	}
}
