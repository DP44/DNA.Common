using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Lights 
{
	public class DirectionalLight : Light 
	{
		/// <summary>
		/// 
		/// </summary>
		public Vector3 LightDirection => 
			base.LocalToWorld.Forward;
	}
}
