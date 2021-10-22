using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.Particles
{
	internal struct ParticleVertex
	{
		public const int SizeInBytes = 36;
		
		private int _cornerAndTileIndex;

		public Vector3 Position;
		public Vector3 Velocity;
		public Color Random;
		public float Time;

		public static readonly VertexDeclaration VertexDeclaration = 
			new VertexDeclaration(new VertexElement[]
		{
			new VertexElement(0, VertexElementFormat.Byte4, VertexElementUsage.Position, 0),
			new VertexElement(4, VertexElementFormat.Vector3, VertexElementUsage.Position, 1),
			new VertexElement(16, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
			new VertexElement(28, VertexElementFormat.Color, VertexElementUsage.Color, 0),
			
			new VertexElement(32, VertexElementFormat.Single, 
				VertexElementUsage.TextureCoordinate, 0)
		});

		/// <summary>
		/// 
		/// </summary>
		public Vector2 Corner
		{
			get =>
				new Vector2((float)((this._cornerAndTileIndex & 255) - 128), 
					(float)((this._cornerAndTileIndex >> 8 & 255) - 128));

			set
			{
				this._cornerAndTileIndex &= -65536;
				
				this._cornerAndTileIndex |= (int)((uint)Math.Floor(
					(double)(value.Y + 128f)) << 8 & 65280U);
				
				this._cornerAndTileIndex |= (int)((uint)Math.Floor(
					(double)(value.X + 128f)) & 255U);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void SetTileXY(int x, int y)
		{
			this._cornerAndTileIndex &= 65535;
			this._cornerAndTileIndex |= (x << 16 & 16711680);
			this._cornerAndTileIndex |= (y << 24 & -16777216);
		}
	}
}
