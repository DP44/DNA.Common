using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public class PrimitiveBatch : IDisposable
	{
		private const int DefaultBufferSize = 500;

		private VertexPositionNormalColor[] vertices = new VertexPositionNormalColor[500];
		private int positionInBuffer;
		private BasicEffect basicEffect;
		private GraphicsDevice device;
		private PrimitiveType primitiveType;
		private int numVertsPerPrimitive;
		private bool hasBegun;
		private bool isDisposed;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public PrimitiveBatch(GraphicsDevice graphicsDevice)
		{
			if (graphicsDevice == null)
			{
				throw new ArgumentNullException("graphicsDevice");
			}

			this.device = graphicsDevice;
			this.basicEffect = new BasicEffect(graphicsDevice);
			this.basicEffect.VertexColorEnabled = true;
			this.basicEffect.EnableDefaultLighting();
		
			this.basicEffect.Projection = Matrix.CreateOrthographicOffCenter(0f, 
				(float)graphicsDevice.Viewport.Width, (float)graphicsDevice.Viewport.Height, 
				0f, 0f, 1f);
		}

		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing && !this.isDisposed)
			{
				if (this.basicEffect != null)
				{
					this.basicEffect.Dispose();
				}
				
				this.isDisposed = true;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void Begin(PrimitiveType primitiveType, Matrix world, 
						  Matrix view, Matrix projection)
		{
			this.basicEffect.World = world;
			this.basicEffect.View = view;
			this.basicEffect.Projection = projection;
			
			if (this.hasBegun)
			{
				throw new InvalidOperationException(
					"End must be called before Begin can be called again.");
			}
			
			if (primitiveType == PrimitiveType.LineStrip || 
				primitiveType == PrimitiveType.TriangleStrip)
			{
				throw new NotSupportedException(
					"The specified primitiveType is not supported by PrimitiveBatch.");
			}
			
			this.primitiveType = primitiveType;
			this.numVertsPerPrimitive = PrimitiveBatch.NumVertsPerPrimitive(primitiveType);
			this.basicEffect.CurrentTechnique.Passes[0].Apply();
			this.hasBegun = true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void AddVertex(Vector3 vertex, Vector3 normal, Color color)
		{
			if (!this.hasBegun)
			{
				throw new InvalidOperationException(
					"Begin must be called before AddVertex can be called.");
			}
			
			bool flag = this.positionInBuffer % this.numVertsPerPrimitive == 0;
			
			if (flag && 
				this.positionInBuffer + this.numVertsPerPrimitive >= this.vertices.Length)
			{
				this.Flush();
			}
			
			this.vertices[this.positionInBuffer].Position = vertex;
			this.vertices[this.positionInBuffer].Normal = normal;
			this.vertices[this.positionInBuffer].Color = color;
			this.vertices[this.positionInBuffer].TextureCoord = new Vector2(0f, 0f);
			
			this.positionInBuffer++;
		}

		/// <summary>
		/// 
		/// </summary>
		public void End()
		{
			if (!this.hasBegun)
			{
				throw new InvalidOperationException(
					"Begin must be called before End can be called.");
			}
			
			this.Flush();
			this.hasBegun = false;
		}

		/// <summary>
		/// 
		/// </summary>
		private void Flush()
		{
			if (!this.hasBegun)
			{
				throw new InvalidOperationException(
					"Begin must be called before Flush can be called.");
			}
			
			if (this.positionInBuffer == 0)
			{
				return;
			}
			
			int primitiveCount = this.positionInBuffer / this.numVertsPerPrimitive;
			
			this.device.DrawUserPrimitives<VertexPositionNormalColor>(
				this.primitiveType, this.vertices, 0, primitiveCount);
			
			this.positionInBuffer = 0;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		private static int NumVertsPerPrimitive(PrimitiveType primitive)
		{
			switch (primitive)
			{
				case PrimitiveType.TriangleList:
				{
					return 3;
				}

				case PrimitiveType.LineList:
				{
					return 2;
				}
			}

			throw new InvalidOperationException("primitive is not valid");
		}
	}
}
