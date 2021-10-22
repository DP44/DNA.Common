using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Exporting
{
	public struct ExportVertex
	{
		public Vector3 Position;
		public Vector3 Normal;
		public Vector2 UV;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public ExportVertex(Vector3 pos, Vector3 norm, Vector2 uv)
		{
			this.Position = pos;
			this.Normal = norm;
			this.UV = uv;
		}
	}
}
