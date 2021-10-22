using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.Particles
{
	public class ParticleEmitter : Entity
	{
		internal class ParticleEmitterCore
		{
			private bool _emitting = true;

			private int MaxParticles;

			private bool _firstUpdate = true;

			private Vector3 _previousPosition;

			private float _timeLeftOver;

			private TimeSpan _timeToEmit;

			private Random _rand = new Random();

			private Effect particleEffect;

			private EffectParameter effectWorldParameter;

			private EffectParameter effectViewParameter;

			private EffectParameter effectProjectionParameter;

			private EffectParameter effectViewProjectionParameter;

			private EffectParameter effectViewportScaleParameter;

			private EffectParameter effectTimeParameter;

			private EffectParameter effectTotalTimeParameter;

			private EffectParameter effectMinColorParameter;

			private EffectParameter effectMaxColorParameter;

			private EffectParameter effectTileSizeParameter;

			private DynamicVertexBuffer vertexBuffer;

			private IndexBuffer indexBuffer;

			private ParticleVertex[] particles;

			private int firstActiveParticle;

			private int firstNewParticle;

			private int firstFreeParticle;

			private int firstRetiredParticle;

			private float currentTime;

			private int drawCounter;

			private static Random random = new Random();

			public static Effect _effect;

			public ParticleEmitter ParticleEmitter;

			private static Texture2D _heatShimmerTexture;

			private VertexBuffer _instanceVertexBuffer;

			private static VertexDeclaration instanceVertexDeclaration = new VertexDeclaration(new VertexElement[]
			{
				new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
				new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1),
				new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2),
				new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3)
			});

			public bool Emitting
			{
				get
				{
					return this._emitting;
				}
				set
				{
					if (!this._emitting && value)
					{
						this._timeToEmit = this._particleEffect.EmmissionTime;
						this._firstUpdate = true;
					}
					this._emitting = value;
				}
			}

			public bool HasActiveParticles
			{
				get
				{
					return this.firstActiveParticle != this.firstFreeParticle;
				}
			}

			private ParticleEffect _particleEffect
			{
				get
				{
					return this.ParticleEmitter._particleEffect;
				}
			}

			public bool Loaded
			{
				get
				{
					return this.particleEffect != null;
				}
			}

			public DNAGame Game
			{
				get
				{
					return this.ParticleEmitter._game;
				}
			}

			internal ParticleEmitterCore(ParticleEmitter emitter)
			{
				this.ParticleEmitter = emitter;
			}

			private void LoadParticleEffect(GraphicsDevice device)
			{
				if (ParticleEmitter.ParticleEmitterCore._effect == null)
				{
					ParticleEmitter.ParticleEmitterCore._effect = this.Game.Content.Load<Effect>("ParticleEffect");
				}
				this.particleEffect = ParticleEmitter.ParticleEmitterCore._effect.Clone();
			}

			private void SetParams()
			{
				if (!this.Loaded)
				{
					this.Initialize();
				}
				EffectParameterCollection parameters = this.particleEffect.Parameters;
				this.effectWorldParameter = parameters["World"];
				this.effectViewParameter = parameters["View"];
				this.effectProjectionParameter = parameters["Projection"];
				this.effectViewProjectionParameter = parameters["ViewProjection"];
				this.effectViewportScaleParameter = parameters["ViewportScale"];
				this.effectTimeParameter = parameters["CurrentTime"];
				this.effectTotalTimeParameter = parameters["TotalTime"];
				this.effectMinColorParameter = parameters["MinColor"];
				this.effectMaxColorParameter = parameters["MaxColor"];
				this.effectTileSizeParameter = parameters["TileSize"];
				parameters["Duration"].SetValue((float)this._particleEffect.ParticleLifeTime.TotalSeconds);
				parameters["DurationRandomness"].SetValue(this._particleEffect.LifetimeVariation);
				parameters["Gravity"].SetValue(this._particleEffect.Gravity);
				parameters["EndVelocity"].SetValue(this._particleEffect.VelocityEnd);
				parameters["FadeOut"].SetValue(this._particleEffect.FadeOut);
				parameters["StartRotation"].SetValue(this._particleEffect.RandomizeRotations ? 1 : 0);
				parameters["DistortionScale"].SetValue(this._particleEffect.DistortionScale);
				parameters["DistortionAmplitude"].SetValue(this._particleEffect.DistortionAmplitude);
				this.effectMinColorParameter.SetValue(this._particleEffect.ColorMin.ToVector4());
				this.effectMaxColorParameter.SetValue(this._particleEffect.ColorMax.ToVector4());
				parameters["RotateSpeed"].SetValue(new Vector2(this._particleEffect.RotateSpeedMin, this._particleEffect.RotateSpeedMax));
				parameters["StartSize"].SetValue(new Vector2(this._particleEffect.StartSizeMin, this._particleEffect.StartSizeMax));
				parameters["EndSize"].SetValue(new Vector2(this._particleEffect.EndSizeMin, this._particleEffect.EndSizeMax));
				Texture2D texture = this._particleEffect.Texture;
				parameters["Texture"].SetValue(texture);
				switch (this._particleEffect.Technique)
				{
				case ParticleTechnique.Normal:
					this.particleEffect.CurrentTechnique = this.particleEffect.Techniques["Particles"];
					return;
				case ParticleTechnique.HeatShimmer:
					this.particleEffect.CurrentTechnique = this.particleEffect.Techniques["HeatHazeParticles"];
					if (ParticleEmitter.ParticleEmitterCore._heatShimmerTexture == null)
					{
						ParticleEmitter.ParticleEmitterCore._heatShimmerTexture = this.Game.Content.Load<Texture2D>("HeatNormal");
					}
					if (this.ParticleEmitter.ScreenBackground == null)
					{
						throw new Exception("Screen Background image must be set to use Heat haze effect");
					}
					parameters["ScreenMap"].SetValue(this.ParticleEmitter.ScreenBackground);
					parameters["DisplacementMap"].SetValue(ParticleEmitter.ParticleEmitterCore._heatShimmerTexture);
					return;
				case ParticleTechnique.Overlay:
					this.particleEffect.CurrentTechnique = this.particleEffect.Techniques["GlowParticles"];
					return;
				default:
					return;
				}
			}

			public void Reset()
			{
				this.firstRetiredParticle = (this.firstFreeParticle = (this.firstNewParticle = (this.firstActiveParticle = 0)));
				this._firstUpdate = true;
				this._timeLeftOver = 0f;
				this.currentTime = 0f;
				this.drawCounter = 0;
				this.SetParams();
			}

			private int CalcMaxParticles()
			{
				return (int)Math.Ceiling((double)this._particleEffect.ParticlesPerSecond * this._particleEffect.ParticleLifeTime.TotalSeconds) + 2;
			}

			public void Initialize()
			{
				switch (this._particleEffect.BlendMode)
				{
				case ParticleBlendMode.Inherit:
					this.ParticleEmitter.BlendState = null;
					break;
				case ParticleBlendMode.Additive:
					this.ParticleEmitter.BlendState = BlendState.Additive;
					break;
				case ParticleBlendMode.NonPreMult:
					this.ParticleEmitter.BlendState = BlendState.NonPremultiplied;
					break;
				}
				this._timeToEmit = this._particleEffect.EmmissionTime;
				this.MaxParticles = this.CalcMaxParticles();
				this.particles = new ParticleVertex[this.MaxParticles * 4];
				for (int i = 0; i < this.MaxParticles; i++)
				{
					this.particles[i * 4].Corner = new Vector2(-1f, -1f);
					this.particles[i * 4 + 1].Corner = new Vector2(1f, -1f);
					this.particles[i * 4 + 2].Corner = new Vector2(1f, 1f);
					this.particles[i * 4 + 3].Corner = new Vector2(-1f, 1f);
				}
				this.LoadParticleEffect(this.Game.GraphicsDevice);
				this.Reset();
				this.vertexBuffer = new DynamicVertexBuffer(this.Game.GraphicsDevice, ParticleVertex.VertexDeclaration, this.MaxParticles * 4, BufferUsage.WriteOnly);
				short[] array = new short[this.MaxParticles * 6];
				for (int j = 0; j < this.MaxParticles; j++)
				{
					array[j * 6] = (short)(j * 4);
					array[j * 6 + 1] = (short)(j * 4 + 1);
					array[j * 6 + 2] = (short)(j * 4 + 2);
					array[j * 6 + 3] = (short)(j * 4);
					array[j * 6 + 4] = (short)(j * 4 + 2);
					array[j * 6 + 5] = (short)(j * 4 + 3);
				}
				this.indexBuffer = new IndexBuffer(this.Game.GraphicsDevice, typeof(short), array.Length, BufferUsage.WriteOnly);
				this.indexBuffer.SetData<short>(array);
			}

			public void SetInitalPosition(Vector3 position)
			{
				this._previousPosition = position;
				this._firstUpdate = false;
				if (!this.Loaded)
				{
					this.Initialize();
				}
			}

			public void OnUpdate(GameTime gameTime, Vector3 emitterSize)
			{
				if (this.MaxParticles != this.CalcMaxParticles())
				{
					this.Initialize();
				}
				Vector3 worldPosition = this.ParticleEmitter.WorldPosition;
				float num = 1f / this._particleEffect.ParticlesPerSecond;
				if (this._firstUpdate)
				{
					this._timeLeftOver = num;
					this.SetInitalPosition(worldPosition);
				}
				bool flag = this._particleEffect.EmmissionTime > TimeSpan.Zero;
				this.currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
				this.RetireActiveParticles();
				this.FreeRetiredParticles();
				if (!this.HasActiveParticles)
				{
					this.currentTime = 0f;
				}
				if (this.firstRetiredParticle == this.firstActiveParticle)
				{
					this.drawCounter = 0;
				}
				float num2 = (float)gameTime.ElapsedGameTime.TotalSeconds;
				if (flag)
				{
					this._timeToEmit -= gameTime.ElapsedGameTime;
					if (this._timeToEmit <= TimeSpan.Zero)
					{
						num2 += (float)this._timeToEmit.TotalSeconds;
						this._timeToEmit = TimeSpan.Zero;
						this._emitting = false;
					}
				}
				if (num2 > 0f && this.Emitting)
				{
					Vector3 velocity = (worldPosition - this._previousPosition) / num2;
					float num3 = this._timeLeftOver + num2;
					float num4 = -this._timeLeftOver;
					if (emitterSize.LengthSquared() == 0f)
					{
						while (num3 > num)
						{
							num4 += num;
							num3 -= num;
							float amount = num4 / num2;
							Vector3 position = Vector3.Lerp(this._previousPosition, worldPosition, amount);
							this.AddParticle(position, velocity);
						}
					}
					else
					{
						while (num3 > num)
						{
							num4 += num;
							num3 -= num;
							Vector3 position2 = new Vector3((float)((double)worldPosition.X + (this._rand.NextDouble() * 2.0 - 1.0) * (double)emitterSize.X), (float)((double)worldPosition.Y + (this._rand.NextDouble() * 2.0 - 1.0) * (double)emitterSize.Y), (float)((double)worldPosition.Z + (this._rand.NextDouble() * 2.0 - 1.0) * (double)emitterSize.Z));
							this.AddParticle(position2, velocity);
						}
					}
					this._timeLeftOver = num3;
				}
				this._previousPosition = worldPosition;
			}

			public void OnAfterFrame()
			{
				this.drawCounter++;
			}

			private void RetireActiveParticles()
			{
				float num = (float)this._particleEffect.ParticleLifeTime.TotalSeconds;
				while (this.firstActiveParticle != this.firstNewParticle)
				{
					float num2 = this.currentTime - this.particles[this.firstActiveParticle * 4].Time;
					if (num2 < num)
					{
						return;
					}
					this.particles[this.firstActiveParticle * 4].Time = (float)this.drawCounter;
					this.firstActiveParticle++;
					if (this.firstActiveParticle >= this.MaxParticles)
					{
						this.firstActiveParticle = 0;
					}
				}
			}

			private void FreeRetiredParticles()
			{
				while (this.firstRetiredParticle != this.firstActiveParticle)
				{
					int num = this.drawCounter - (int)this.particles[this.firstRetiredParticle * 4].Time;
					if (num < 3)
					{
						return;
					}
					this.firstRetiredParticle++;
					if (this.firstRetiredParticle >= this.MaxParticles)
					{
						this.firstRetiredParticle = 0;
					}
				}
			}

			public void Draw(GraphicsDevice device, GameTime gameTime, Matrix view, Matrix projection, Vector4 lightColor)
			{
				if (this._firstUpdate)
				{
					return;
				}
				this.particleEffect.Parameters["Light"].SetValue(lightColor);
				this.Draw(device, gameTime, view, projection);
			}

			public void Draw(GraphicsDevice device, GameTime gameTime, Matrix view, Matrix projection)
			{
				if (this._firstUpdate)
				{
					return;
				}
				EffectParameterCollection parameters = this.particleEffect.Parameters;
				if (this.ParticleEmitter.EntityColor != null)
				{
					Color color = DrawingTools.ModulateColors(this.ParticleEmitter.EntityColor.Value, this._particleEffect.ColorMin);
					Color color2 = DrawingTools.ModulateColors(this.ParticleEmitter.EntityColor.Value, this._particleEffect.ColorMax);
					this.effectMinColorParameter.SetValue(color.ToVector4());
					this.effectMaxColorParameter.SetValue(color2.ToVector4());
				}
				if (this.ParticleEmitter.IsDistortionEffect && this.ParticleEmitter.ScreenBackground != null)
				{
					this.particleEffect.Parameters["ScreenMap"].SetValue(this.ParticleEmitter.ScreenBackground);
				}
				this.effectTotalTimeParameter.SetValue((float)gameTime.TotalGameTime.TotalSeconds);
				if (this._particleEffect.LocalSpace)
				{
					this.effectWorldParameter.SetValue(this.ParticleEmitter.LocalToWorld);
				}
				else
				{
					this.effectWorldParameter.SetValue(Matrix.Identity);
				}
				this.effectViewParameter.SetValue(view);
				this.effectProjectionParameter.SetValue(projection);
				this.effectViewProjectionParameter.SetValue(view * projection);
				this.effectTileSizeParameter.SetValue(this._particleEffect.TileSize);
				if (this.vertexBuffer.IsContentLost)
				{
					this.vertexBuffer.SetData<ParticleVertex>(this.particles);
				}
				if (this.firstNewParticle != this.firstFreeParticle)
				{
					this.AddNewParticlesToVertexBuffer();
				}
				if (this.firstActiveParticle != this.firstFreeParticle)
				{
					this.effectViewportScaleParameter.SetValue(new Vector2(0.5f / device.Viewport.AspectRatio, -0.5f));
					this.effectTimeParameter.SetValue(this.currentTime);
					if (this.ParticleEmitter.Instances != null)
					{
						EffectTechnique currentTechnique = this.particleEffect.Techniques["ParticlesInstanced"];
						this.particleEffect.CurrentTechnique = currentTechnique;
						int num = this.ParticleEmitter.Instances.Length;
						if (num > 0)
						{
							if (this.ParticleEmitter._instanceListDirty)
							{
								if (this._instanceVertexBuffer == null || this.ParticleEmitter.Instances.Length > this._instanceVertexBuffer.VertexCount)
								{
									if (this._instanceVertexBuffer != null)
									{
										this._instanceVertexBuffer.Dispose();
									}
									this._instanceVertexBuffer = new VertexBuffer(device, ParticleEmitter.ParticleEmitterCore.instanceVertexDeclaration, num, BufferUsage.WriteOnly);
								}
								this._instanceVertexBuffer.SetData<Matrix>(this.ParticleEmitter.Instances, 0, this.ParticleEmitter.Instances.Length);
								this.ParticleEmitter._instanceListDirty = false;
							}
							device.SetVertexBuffers(new VertexBufferBinding[]
							{
								new VertexBufferBinding(this.vertexBuffer, 0, 0),
								new VertexBufferBinding(this._instanceVertexBuffer, 0, 1)
							});
							device.Indices = this.indexBuffer;
							for (int i = 0; i < this.particleEffect.CurrentTechnique.Passes.Count; i++)
							{
								EffectPass effectPass = this.particleEffect.CurrentTechnique.Passes[i];
								effectPass.Apply();
								if (this.firstActiveParticle < this.firstFreeParticle)
								{
									device.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, this.firstActiveParticle * 4, (this.firstFreeParticle - this.firstActiveParticle) * 4, this.firstActiveParticle * 6, (this.firstFreeParticle - this.firstActiveParticle) * 2, num);
								}
								else
								{
									device.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, this.MaxParticles * 4, 0, this.MaxParticles * 2, num);
								}
							}
							return;
						}
					}
					else
					{
						device.SetVertexBuffer(this.vertexBuffer);
						device.Indices = this.indexBuffer;
						for (int j = 0; j < this.particleEffect.CurrentTechnique.Passes.Count; j++)
						{
							EffectPass effectPass2 = this.particleEffect.CurrentTechnique.Passes[j];
							effectPass2.Apply();
							if (this.firstActiveParticle < this.firstFreeParticle)
							{
								device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, this.firstActiveParticle * 4, (this.firstFreeParticle - this.firstActiveParticle) * 4, this.firstActiveParticle * 6, (this.firstFreeParticle - this.firstActiveParticle) * 2);
							}
							else
							{
								device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, this.firstActiveParticle * 4, (this.MaxParticles - this.firstActiveParticle) * 4, this.firstActiveParticle * 6, (this.MaxParticles - this.firstActiveParticle) * 2);
								if (this.firstFreeParticle > 0)
								{
									device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.firstFreeParticle * 4, 0, this.firstFreeParticle * 2);
								}
							}
						}
					}
				}
			}

			private void AddNewParticlesToVertexBuffer()
			{
				int num = 36;
				if (this.firstNewParticle < this.firstFreeParticle)
				{
					this.vertexBuffer.SetData<ParticleVertex>(this.firstNewParticle * num * 4, this.particles, this.firstNewParticle * 4, (this.firstFreeParticle - this.firstNewParticle) * 4, num, SetDataOptions.NoOverwrite);
				}
				else
				{
					this.vertexBuffer.SetData<ParticleVertex>(this.firstNewParticle * num * 4, this.particles, this.firstNewParticle * 4, (this.MaxParticles - this.firstNewParticle) * 4, num, SetDataOptions.NoOverwrite);
					if (this.firstFreeParticle > 0)
					{
						this.vertexBuffer.SetData<ParticleVertex>(0, this.particles, 0, this.firstFreeParticle * 4, num, SetDataOptions.NoOverwrite);
					}
				}
				this.firstNewParticle = this.firstFreeParticle;
			}

			public void AddParticle(Vector3 position, Vector3 velocity)
			{
				if (this._particleEffect.LocalSpace)
				{
					position = Vector3.Zero;
				}
				int num = this.firstFreeParticle + 1;
				if (num >= this.MaxParticles)
				{
					num = 0;
				}
				if (num == this.firstRetiredParticle)
				{
					return;
				}
				velocity *= this._particleEffect.EmitterVelocitySensitivity;
				float num2 = MathHelper.Lerp(this._particleEffect.HorizontalVelocityMin, this._particleEffect.HorizontalVelocityMax, (float)ParticleEmitter.ParticleEmitterCore.random.NextDouble());
				double num3 = ParticleEmitter.ParticleEmitterCore.random.NextDouble() * 6.2831854820251465;
				velocity.X += num2 * (float)Math.Cos(num3);
				velocity.Y += num2 * (float)Math.Sin(num3);
				velocity.Z += MathHelper.Lerp(this._particleEffect.VerticalVelocityMin, this._particleEffect.VerticalVelocityMax, (float)ParticleEmitter.ParticleEmitterCore.random.NextDouble());
				velocity = Vector3.TransformNormal(velocity, this.ParticleEmitter.LocalToWorld);
				Color color = new Color((int)((byte)ParticleEmitter.ParticleEmitterCore.random.Next(255)), (int)((byte)ParticleEmitter.ParticleEmitterCore.random.Next(255)), (int)((byte)ParticleEmitter.ParticleEmitterCore.random.Next(255)), (int)((byte)ParticleEmitter.ParticleEmitterCore.random.Next(255)));
				int num4 = MathTools.RandomInt(this._particleEffect.FirstTileToInclude, this._particleEffect.LastTileToInclude + 1);
				int x = num4 % this._particleEffect.NumTilesWide;
				int y = num4 / this._particleEffect.NumTilesWide;
				for (int i = 0; i < 4; i++)
				{
					this.particles[this.firstFreeParticle * 4 + i].Position = position;
					this.particles[this.firstFreeParticle * 4 + i].SetTileXY(x, y);
					this.particles[this.firstFreeParticle * 4 + i].Velocity = velocity;
					this.particles[this.firstFreeParticle * 4 + i].Random = color;
					this.particles[this.firstFreeParticle * 4 + i].Time = this.currentTime;
				}
				this.firstFreeParticle = num;
			}
		}

		public Texture2D ScreenBackground;

		private bool _instanceListDirty = true;

		private Matrix[] _instances;

		private ParticleEmitter.ParticleEmitterCore _core;

		private ParticleEffect _particleEffect;

		private DNAGame _game;

		public Vector4 _lightColor = Vector4.One;

		public Vector3 EmitterSize = Vector3.Zero;

		private bool _emitting;

		private static Queue<ParticleEmitter> _graveYard = new Queue<ParticleEmitter>();

		public Matrix[] Instances
		{
			get
			{
				return this._instances;
			}
			set
			{
				this._instances = value;
				this._instanceListDirty = true;
			}
		}

		public Vector3 LightColor
		{
			get
			{
				return new Vector3(this._lightColor.X, this._lightColor.Y, this._lightColor.Z);
			}
			set
			{
				this._lightColor = new Vector4(value, 1f);
			}
		}

		public bool HasActiveParticles
		{
			get
			{
				return this._core != null && this._core.HasActiveParticles;
			}
		}

		public bool IsDistortionEffect
		{
			get
			{
				return this._particleEffect.Technique == ParticleTechnique.HeatShimmer;
			}
		}

		public bool Emitting
		{
			get
			{
				return this._emitting;
			}
			set
			{
				this._emitting = value;
				if (this._core != null)
				{
					this._core.Emitting = value;
				}
			}
		}

		public void Reset()
		{
			if (this._core == null)
			{
				this._core = this._particleEffect.CreateParticleCore(this);
				this._instanceListDirty = true;
				return;
			}
			this._core.Reset();
			this._instanceListDirty = true;
		}

		public void SetInitalPosition(Vector3 position)
		{
			if (this._core == null)
			{
				this._core = this._particleEffect.CreateParticleCore(this);
			}
			this._core.SetInitalPosition(position);
		}

		internal void ReleaseCore()
		{
			this._instanceListDirty = true;
			this._core = null;
		}

		internal void SetCore(ParticleEmitter.ParticleEmitterCore core)
		{
			this._core = core;
			this._core.ParticleEmitter = this;
			this._core.Reset();
			this._instanceListDirty = true;
		}

		internal static ParticleEmitter Create(DNAGame game, ParticleEffect particleEffect, Texture2D screenMap)
		{
			if (!particleEffect.DieAfterEmmision)
			{
				return new ParticleEmitter(game, particleEffect, screenMap);
			}
			if (ParticleEmitter._graveYard.Count == 0)
			{
				return new ParticleEmitter(game, particleEffect, screenMap);
			}
			ParticleEmitter particleEmitter = ParticleEmitter._graveYard.Dequeue();
			particleEmitter.Setup(game, particleEffect, screenMap);
			return particleEmitter;
		}

		private void Setup(DNAGame game, ParticleEffect particleEffect, Texture2D screenMap)
		{
			this.ScreenBackground = screenMap;
			this._game = game;
			this._particleEffect = particleEffect;
			this.AlphaSort = true;
			this.EntityColor = null;
			base.LocalRotation = Quaternion.Identity;
			base.LocalPosition = Vector3.Zero;
			base.LocalScale = Vector3.One;
			this._lightColor = Vector4.One;
			this.Visible = true;
			switch (this._particleEffect.BlendMode)
			{
			case ParticleBlendMode.Additive:
				base.BlendState = BlendState.Additive;
				break;
			case ParticleBlendMode.NonPreMult:
				base.BlendState = BlendState.NonPremultiplied;
				break;
			}
			base.DepthStencilState = DepthStencilState.DepthRead;
			base.SamplerState = SamplerState.LinearWrap;
			if (this._core != null)
			{
				this.ReleaseCore();
			}
			this._core = this._particleEffect.CreateParticleCore(this);
		}

		internal ParticleEmitter(DNAGame game, ParticleEffect particleEffect)
		{
			this.Setup(game, particleEffect, null);
		}

		internal ParticleEmitter(DNAGame game, ParticleEffect particleEffect, Texture2D screenMap)
		{
			this.Setup(game, particleEffect, screenMap);
		}

		protected override void OnUpdate(GameTime gameTime)
		{
			if (this._emitting && this._core == null)
			{
				this._core = this._particleEffect.CreateParticleCore(this);
				this._core.Emitting = true;
			}
			if (this._core != null)
			{
				this._core.OnUpdate(gameTime, this.EmitterSize);
				this._emitting = this._core.Emitting;
			}
			if (this._particleEffect.DieAfterEmmision && (this._core == null || (!this._core.Emitting && !this._core.HasActiveParticles)))
			{
				this.ReleaseCore();
				base.RemoveFromParent();
				ParticleEmitter._graveYard.Enqueue(this);
			}
			base.OnUpdate(gameTime);
		}

		public override void Draw(GraphicsDevice device, GameTime gameTime, Matrix view, Matrix projection)
		{
			if (this._core != null)
			{
				this._core.Draw(device, gameTime, view, projection, this._lightColor);
			}
			base.Draw(device, gameTime, view, projection);
		}

		public override void OnAfterFrame()
		{
			if (this._core != null)
			{
				this._core.OnAfterFrame();
			}
			base.OnAfterFrame();
		}

		public void AdvanceEffect(TimeSpan time)
		{
			int num = (int)(time.TotalSeconds * 60.0);
			TimeSpan timeSpan = TimeSpan.Zero;
			TimeSpan timeSpan2 = TimeSpan.FromSeconds(0.016666666666666666);
			for (int i = 0; i < num; i++)
			{
				timeSpan += timeSpan2;
				this.Update(this._game, new GameTime(timeSpan, timeSpan2));
			}
		}
	}
}
