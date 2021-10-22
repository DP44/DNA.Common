using System;
using DNA.Drawing.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public class SkySphere : Entity
	{
		private DNAEffect _effect;
		private VertexBuffer _vertexBuffer;
		private IndexBuffer _indexBuffer;
		private Texture _texture;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		private void CreateBall(GraphicsDevice device, float radius, int segments, Vector3 center, bool insideOut)
		{
			int num = segments / 2;
			VertexPositionNormalTexture[] array = new VertexPositionNormalTexture[segments * (num - 1) + 2];
			int num2 = array.Length - 1;
			array[0].Position = new Vector3(0f, radius, 0f);
			array[0].Normal = new Vector3(0f, 1f, 0f);
			array[0].TextureCoordinate = new Vector2(0f, 0f);
			array[num2].Position = new Vector3(0f, -radius, 0f);
			array[num2].Normal = new Vector3(0f, -1f, 0f);
			array[num2].TextureCoordinate = new Vector2(0f, 1f);
			int num3 = 1;
			
			for (int i = 1; i < num; i++)
			{
				for (int j = 0; j < segments; j++)
				{
					Angle angle = Angle.Lerp(Angle.FromDegrees(0f), Angle.FromDegrees(180f), (float)i / (float)num);
					Angle angle2 = Angle.Lerp(Angle.FromDegrees(0f), Angle.FromDegrees(360f), (float)j / (float)segments);
					array[num3].Normal = new Vector3((float)(angle2.Sin * angle.Sin), (float)angle.Cos, (float)(angle2.Cos * angle.Sin));
					array[num3].Position = center + array[num3].Normal * radius;
					array[num3].TextureCoordinate = new Vector2((float)j / (float)segments, -((float)angle.Cos - 1f) / 2f);
					num3++;
				}
			}
			
			short[] array2 = new short[segments * 6 * num];
			int num4 = 0;
			
			for (int k = 0; k < segments; k++)
			{
				int num5 = 1;
				int num6 = (k + 1) % segments;
				array2[num4++] = 0;
				array2[num4++] = (short)(num6 + num5);
				array2[num4++] = (short)(k + num5);
				num5 = num2 - segments;
				array2[num4++] = (short)num2;
				array2[num4++] = (short)(num5 + k);
				array2[num4++] = (short)(num5 + num6);
			}
			
			for (int l = 0; l < num - 2; l++)
			{
				int num7 = l * segments + 1;
			
				for (int m = 0; m < segments; m++)
				{
					int num8 = (m + 1) % segments;
					array2[num4++] = (short)(num7 + m);
					array2[num4++] = (short)(num7 + num8 + segments);
					array2[num4++] = (short)(num7 + m + segments);
					array2[num4++] = (short)(num7 + m);
					array2[num4++] = (short)(num7 + num8);
					array2[num4++] = (short)(num7 + num8 + segments);
				}
			}
			
			if (insideOut)
			{
				for (int n = 0; n < array.Length; n++)
				{
					array[n].Normal = -array[n].Normal;
				}
			
				for (int num9 = 0; num9 < array2.Length; num9 += 3)
				{
					short num10 = array2[num9 + 1];
					array2[num9 + 1] = array2[num9 + 2];
					array2[num9 + 2] = num10;
				}
			}
			
			this._vertexBuffer = new VertexBuffer(device, typeof(VertexPositionNormalTexture), array.Length, BufferUsage.WriteOnly);
			this._vertexBuffer.SetData<VertexPositionNormalTexture>(array);
			this._indexBuffer = new IndexBuffer(device, typeof(short), array2.Length, BufferUsage.WriteOnly);
			this._indexBuffer.SetData<short>(array2);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public SkySphere(Game game, float radius, Vector3 center, int segments, Texture texture)
		{
			this.CreateBall(game.GraphicsDevice, radius, segments, center, true);
			this._effect = new DNAEffect(game.Content.Load<Effect>("SkySphere"));
			this._texture = texture;
			base.RasterizerState = RasterizerState.CullNone;
			base.DepthStencilState = DepthStencilState.None;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public SkySphere(GraphicsDevice device, float radius, Vector3 center, int segments, Effect effect, Texture texture)
		{
			this.CreateBall(device, radius, segments, center, true);
			this._effect = new DNAEffect(effect);
			this._texture = texture;
			base.RasterizerState = RasterizerState.CullNone;
			base.DepthStencilState = DepthStencilState.None;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected virtual bool SetEffectParams(DNAEffect effect, GameTime gameTime, Matrix world, Matrix view, Matrix projection)
		{
			effect.World = world;
			effect.View = view;
			effect.Projection = projection;
			
			if (effect.Parameters["Texture"] != null)
			{
				effect.Parameters["Texture"].SetValue(this._texture);
			}
			
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public override void Draw(GraphicsDevice device, GameTime gameTime, Matrix view, Matrix projection)
		{
			view.Translation = Vector3.Zero;
			this.SetEffectParams(this._effect, gameTime, base.LocalToWorld, view, projection);
			device.SetVertexBuffer(this._vertexBuffer);
			device.Indices = this._indexBuffer;
			
			for (int i = 0; i < this._effect.CurrentTechnique.Passes.Count; i++)
			{
				EffectPass effectPass = this._effect.CurrentTechnique.Passes[i];
				effectPass.Apply();
				device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this._vertexBuffer.VertexCount, 0, this._indexBuffer.IndexCount / 3);
			}
			
			base.Draw(device, gameTime, view, projection);
		}
	}
}
