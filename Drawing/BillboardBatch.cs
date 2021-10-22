using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public class BillboardBatch
	{
		public BlendState BlendState = BlendState.AlphaBlend;

		private BillBoardMode _billboardMode = BillBoardMode.AxisAligned;

		private BillboardVertex[] billboardVerticies = new BillboardVertex[1024];

		private int cardCount;

		private DynamicVertexBuffer _vertexBuffer;

		private IndexBuffer _indexBuffer;

		private Effect _cardEffect;

		private Texture _texture;

		public static Effect _effect;

		private Game _game;

		private bool _started;

		public bool Started
		{
			get
			{
				return this._started;
			}
		}

		public void SetShader(Effect effect)
		{
			this._cardEffect = effect.Clone();
		}

		public BillboardBatch(Game game)
		{
			this._game = game;
			this.LoadParticleEffect();
		}

		public BillboardBatch(Game game, int startSize)
		{
			this.billboardVerticies = new BillboardVertex[startSize * 4];
			this._game = game;
			this.LoadParticleEffect();
		}

		private void LoadParticleEffect()
		{
			if (BillboardBatch._effect == null)
			{
				BillboardBatch._effect = this._game.Content.Load<Effect>("Billboard");
			}
			this._cardEffect = BillboardBatch._effect.Clone();
		}

		private void Initialize(GraphicsDevice device, int cards)
		{
			this._vertexBuffer = new DynamicVertexBuffer(device, BillboardVertex.VertexDeclaration, cards * 4, BufferUsage.WriteOnly);
			uint[] array = new uint[cards * 6];
			for (int i = 0; i < cards; i++)
			{
				array[i * 6] = (uint)(i * 4);
				array[i * 6 + 1] = (uint)(i * 4 + 1);
				array[i * 6 + 2] = (uint)(i * 4 + 2);
				array[i * 6 + 3] = (uint)(i * 4);
				array[i * 6 + 4] = (uint)(i * 4 + 2);
				array[i * 6 + 5] = (uint)(i * 4 + 3);
			}
			this._indexBuffer = new IndexBuffer(device, typeof(uint), array.Length, BufferUsage.WriteOnly);
			this._indexBuffer.SetData<uint>(array);
		}

		public void Begin(BillBoardMode mode)
		{
			if (this._started)
			{
				throw new Exception("Batch Already Started");
			}
			this._started = true;
			this._billboardMode = mode;
			this.cardCount = 0;
		}

		public void End(GraphicsDevice device, Matrix world, Matrix view, Matrix projection)
		{
			if (!this._started)
			{
				throw new Exception("End Before Start");
			}
			if (this._vertexBuffer == null)
			{
				this.Initialize(device, this.billboardVerticies.Length / 4);
			}
			this.Draw(device, world, view, projection);
			this._started = false;
		}

		private void Draw(GraphicsDevice device, Matrix world, Matrix view, Matrix projection)
		{
			if (this.cardCount == 0)
			{
				return;
			}
			EffectParameterCollection parameters = this._cardEffect.Parameters;
			device.BlendState = this.BlendState;
			device.DepthStencilState = DepthStencilState.DepthRead;
			device.DepthStencilState = DepthStencilState.Default;
			device.RasterizerState = RasterizerState.CullNone;
			parameters["View"].SetValue(view);
			parameters["Projection"].SetValue(projection);
			parameters["ViewportScale"].SetValue(new Vector2(1f / device.Viewport.AspectRatio, 1f));
			parameters["Texture"].SetValue(this._texture);
			switch (this._billboardMode)
			{
			case BillBoardMode.ScreenAligned:
				this._cardEffect.CurrentTechnique = this._cardEffect.Techniques["ScreenAlignedBillboards"];
				break;
			case BillBoardMode.AxisAligned:
				this._cardEffect.CurrentTechnique = this._cardEffect.Techniques["AxisAlignedBillboards"];
				break;
			default:
				throw new Exception("Unknown Mode");
			}
			this._vertexBuffer.SetData<BillboardVertex>(this.billboardVerticies, 0, this.cardCount * 4);
			device.SetVertexBuffer(this._vertexBuffer);
			device.Indices = this._indexBuffer;
			for (int i = 0; i < this._cardEffect.CurrentTechnique.Passes.Count; i++)
			{
				EffectPass effectPass = this._cardEffect.CurrentTechnique.Passes[i];
				effectPass.Apply();
				device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.cardCount * 4, 0, this.cardCount * 2);
			}
			device.SetVertexBuffer(null);
		}

		private void GrowArray()
		{
			BillboardVertex[] destinationArray = new BillboardVertex[this.billboardVerticies.Length * 2];
			Array.Copy(this.billboardVerticies, destinationArray, this.cardCount * 4);
			this.billboardVerticies = destinationArray;
			if (this._vertexBuffer != null)
			{
				this._vertexBuffer.Dispose();
				this._vertexBuffer = null;
			}
			if (this._indexBuffer != null)
			{
				this._indexBuffer.Dispose();
				this._indexBuffer = null;
			}
		}

		public void DrawCard(Texture texture, Vector3 position, Vector3 axis, Vector2 scale, Color color)
		{
			this._texture = texture;
			if ((this.cardCount + 1) * 4 > this.billboardVerticies.Length)
			{
				this.GrowArray();
			}
			int num = this.cardCount * 4;
			this.billboardVerticies[num] = new BillboardVertex(position, scale, axis, new Vector2(0f, 0f), color);
			this.billboardVerticies[num + 1] = new BillboardVertex(position, scale, axis, new Vector2(1f, 0f), color);
			this.billboardVerticies[num + 2] = new BillboardVertex(position, scale, axis, new Vector2(1f, 1f), color);
			this.billboardVerticies[num + 3] = new BillboardVertex(position, scale, axis, new Vector2(0f, 1f), color);
			this.cardCount++;
		}
	}
}
