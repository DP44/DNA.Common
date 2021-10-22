using System;
using System.Collections.Generic;
using DNA.Collections;
using DNA.Drawing.Lights;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public class Entity : Tree<Entity>
	{
		public const int DefaultDrawPriority = 0;

		public Color? EntityColor;
		
		private Vector3 _localPosition = Vector3.Zero;
		private Quaternion _localRotation = Quaternion.Identity;
		private Vector3 _localScale = new Vector3(1f, 1f, 1f);
		private Matrix _localToWorld;
		private bool _ltwDirty = true;
		private State _currentAction;
		private Queue<State> _actionQueue = new Queue<State>();
		
		public int DrawPriority;
		
		public static SamplerState DefaultSamplerState = 
			SamplerState.AnisotropicWrap;
		
		public static BlendState DefaultBlendState = 
			BlendState.Opaque;

		public static RasterizerState DefaultRasterizerState = 
			RasterizerState.CullCounterClockwise;

		public static DepthStencilState DefaultDepthStencilState = 
			DepthStencilState.Default;
		
		private SamplerState _samplerState;
		private BlendState _blendState;
		private RasterizerState _rasterizerState;
		private DepthStencilState _depthStencilState;
		private Physics _physics;
		
		public bool AlphaSort;
		public bool Visible = true;
		public bool DoUpdate = true;
		public bool Collider;
		public bool Collidee;
		
		private string _name;
		
		private static Plane EmptyPlane = default(Plane);
		
		private bool _ltpDirty = true;
		private Matrix _cachedLocalToParent;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void ApplyEffect(Effect effect, bool applyToChildren)
		{
			this.OnApplyEffect(effect);
			
			// Apply the effect to child entities if told to do so.
			if (applyToChildren)
			{
				foreach (Entity entity in base.Children)
				{
					entity.ApplyEffect(effect, applyToChildren);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual BoundingSphere GetLocalBoundingSphere() => 
			new BoundingSphere(new Vector3(0.0f, 0.0f, 0.0f), 0.0f);

		/// <summary>
		/// 
		/// </summary>
		public virtual BoundingBox GetAABB() => 
			new BoundingBox(this.WorldPosition, this.WorldPosition);

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected virtual void OnApplyEffect(Effect sourceEffect) {}

		/// <summary>
		/// 
		/// </summary>
		public SamplerState SamplerState
		{
			get
			{
				if (this._samplerState != null)
				{
					return this._samplerState;
				}
				
				Entity parent = base.Parent;
				
				return parent == null 
					? Entity.DefaultSamplerState 
					: parent.SamplerState;
			}
			
			set => 
				this._samplerState = value;
		}

		/// <summary>
		/// 
		/// </summary>
		public BlendState BlendState
		{
			get
			{
				if (this._blendState != null)
				{
					return this._blendState;
				}
				
				Entity parent = base.Parent;
				
				return parent == null 
					? Entity.DefaultBlendState 
					: parent.BlendState;
			}
		
			set => 
				this._blendState = value;
		}

		/// <summary>
		/// 
		/// </summary>
		public RasterizerState RasterizerState
		{
			get
			{
				if (this._rasterizerState != null)
				{
					return this._rasterizerState;
				}
				
				Entity parent = base.Parent;
				
				return parent == null 
					? Entity.DefaultRasterizerState 
					: parent.RasterizerState;
			}
		
			set => 
				this._rasterizerState = value;
		}

		/// <summary>
		/// 
		/// </summary>
		public DepthStencilState DepthStencilState
		{
			get
			{
				if (this._depthStencilState != null)
				{
					return this._depthStencilState;
				}
				
				Entity parent = base.Parent;

				return parent == null 
					? Entity.DefaultDepthStencilState 
					: parent.DepthStencilState;
			}
			
			set => 
				this._depthStencilState = value;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void SetRenderState(GraphicsDevice device)
		{
			#if DECOMPILED
				Entity parent = base.Parent;
				
				if (parent == null)
				{
					if (this._samplerState != null)
					{
						device.SamplerStates[0] = this._samplerState;
					}
					else
					{
						device.SamplerStates[0] = Entity.DefaultSamplerState;
					}
					
					if (this._blendState != null)
					{
						device.BlendState = this._blendState;
					}
					else
					{
						device.BlendState = Entity.DefaultBlendState;
					}
					
					if (this._rasterizerState != null)
					{
						device.RasterizerState = this._rasterizerState;
					}
					else
					{
						device.RasterizerState = Entity.DefaultRasterizerState;
					}
					
					if (this._depthStencilState != null)
					{
						device.DepthStencilState = this._depthStencilState;
						return;
					}
					
					device.DepthStencilState = Entity.DefaultDepthStencilState;
					return;
				}
				else
				{
					if (this._samplerState != null)
					{
						device.SamplerStates[0] = this._samplerState;
					}
					else
					{
						device.SamplerStates[0] = parent.SamplerState;
					}
					
					if (this._blendState != null)
					{
						device.BlendState = this._blendState;
					}
					else
					{
						device.BlendState = parent.BlendState;
					}
					
					if (this._rasterizerState != null)
					{
						device.RasterizerState = this._rasterizerState;
					}
					else
					{
						device.RasterizerState = parent.RasterizerState;
					}
					
					if (this._depthStencilState != null)
					{
						device.DepthStencilState = this._depthStencilState;
						return;
					}
					
					device.DepthStencilState = parent.DepthStencilState;
					return;
				}
			#else
				Entity parent = this.Parent;
				
				if (parent == null)
				{
					device.SamplerStates[0] = 
						this._samplerState == null 
							? Entity.DefaultSamplerState 
							: this._samplerState;
					
					device.BlendState = 
						this._blendState == null 
							? Entity.DefaultBlendState 
							: this._blendState;
					
					device.RasterizerState = 
						this._rasterizerState == null 
							? Entity.DefaultRasterizerState 
							: this._rasterizerState;
					
					if (this._depthStencilState != null)
					{
						device.DepthStencilState = this._depthStencilState;
					}
					else
					{
						device.DepthStencilState = Entity.DefaultDepthStencilState;
					}
				}
				else
				{
					device.SamplerStates[0] = 
						this._samplerState == null 
							? parent.SamplerState 
							: this._samplerState;
					
					device.BlendState = 
						this._blendState == null 
							? parent.BlendState 
							: this._blendState;
					
					device.RasterizerState = 
						this._rasterizerState == null 
							? parent.RasterizerState 
							: this._rasterizerState;
					
					if (this._depthStencilState != null)
					{
						device.DepthStencilState = this._depthStencilState;
					}
					else
					{
						device.DepthStencilState = parent.DepthStencilState;
					}
				}
			#endif
		}

		/// <summary>
		/// 
		/// </summary>
		public Physics Physics
		{
			get => 
				this._physics;
			
			set
			{
				this._physics = value;
				
				if (this._physics != null && this._physics.Owner != this)
				{
					throw new Exception();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Queue<State> ActionQueue => 
			this._actionQueue;

		/// <summary>
		/// 
		/// </summary>
		public string Name
		{
			get => 
				this._name;
			
			set => 
				this._name = value;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected void GetDrawList(List<Light> lights, List<Entity> toSort, 
								   List<Entity> toDraw, FilterCallback<Entity> filter)
		{
			for (int i = 0; i < base.Children.Count; i++)
			{
				Entity child = base.Children[i];
				
				if (child.Visible && filter(child))
				{
					if (child is Light)
					{
						lights.Add((Light)child);
					}
					else if (this.AlphaSort)
					{
						if (this.AlphaSort)
						{
							toSort.Add(child);
						}
						else
						{
							toDraw.Add(child);
						}
						
						child.GetDrawList(lights, toSort, toDraw, filter);
					}
					else
					{
						toDraw.Add(child);
					}
					
					child.GetDrawList(lights, toSort, toDraw, filter);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public virtual void ResolveCollsions(List<Entity> collidees, GameTime dt)
		{
			for (int i = 0; i < collidees.Count; i++)
			{
				Entity ent = collidees[i];
				Plane collsionPlane;
				
				if (this.CollidesAgainst(ent) && 
					this.ResolveCollsion(ent, out collsionPlane, dt))
				{
					this.OnCollisionWith(ent, collsionPlane);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public virtual void OnCollisionWith(Entity e, Plane collsionPlane) {}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public virtual bool CollidesAgainst(Entity e) => 
			e != this && this.Collider && e.Collidee;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public virtual bool ResolveCollsion(Entity e, out Plane collsionPlane, GameTime dt)
		{
			collsionPlane = Entity.EmptyPlane;
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		public void RemoveFromParent()
		{
			// Make sure the parent exists first.
			if (base.Parent == null)
			{
				return;
			}

			base.Parent.Children.Remove(this);
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void OnMoved() {}

		/// <summary>
		/// 
		/// </summary>
		private void DirtyLTW()
		{
			this.OnMoved();
			this._ltwDirty = true;
			
			for (int i = 0; i < base.Children.Count; i++)
			{
				Entity entity = base.Children[i];
			
				if (!entity._ltwDirty)
				{
					entity.DirtyLTW();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Entity() : base(20) {}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected override void OnParentChanged(Entity oldParent, Entity newParent)
		{
			this.DirtyLTW();
			base.OnParentChanged(oldParent, newParent);
		}

		/// <summary>
		/// 
		/// </summary>
		public Matrix LocalToParent
		{
			get
			{
				if (this._ltpDirty)
				{
					this._cachedLocalToParent = 
						Matrix.CreateScale(this._localScale) * 
						Matrix.CreateFromQuaternion(this._localRotation);
					
					this._cachedLocalToParent.Translation = this._localPosition;
				}

				return this._cachedLocalToParent;
			}
			set
			{
				this._cachedLocalToParent = value;
				this._ltpDirty = false;
				
				value.Decompose(
					out this._localScale, out this._localRotation, out this._localPosition);
				
				this.DirtyLTW();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected bool LTWDirty => 
			this._ltwDirty;

		/// <summary>
		/// 
		/// </summary>
		public Matrix LocalToWorld
		{
			get
			{
				if (this._ltwDirty)
				{
					#if DECOMPILED
						if (base.Parent == null)
						{
							this._localToWorld = this.LocalToParent;
						}
						else
						{
							this._localToWorld = this.LocalToParent * base.Parent.LocalToWorld;
						}
					#else
						this._localToWorld = 
							this.Parent != null 
								? this.LocalToParent * this.Parent.LocalToWorld 
								: this.LocalToParent;
						
						this._ltwDirty = false;
					#endif
				}

				return this._localToWorld;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Matrix WorldToLocal => 
			Matrix.Invert(this.LocalToWorld);

		/// <summary>
		/// 
		/// </summary>
		public Vector3 WorldPosition => 
			this._ltwDirty ? this.LocalToWorld.Translation : this._localToWorld.Translation;

		/// <summary>
		/// 
		/// </summary>
		public Vector3 LocalPosition
		{
			get =>
				this._localPosition;

			set
			{
				this._localPosition = value;
				this._ltpDirty = true;
				this.DirtyLTW();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Vector3 LocalScale
		{
			get => this._localScale;

			set
			{
				this._localScale = value;
				this._ltpDirty = true;
				this.DirtyLTW();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Quaternion LocalRotation
		{
			get => 
				this._localRotation;

			set
			{
				this._localRotation = value;
				this._localRotation.Normalize();
				this._ltpDirty = true;
				this.DirtyLTW();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Scene Scene
		{
			/*
				get
				{
					Entity entity = this;
					while (true)
					{
						switch (entity)
						{
							case null:
								goto label_2;
							case Scene _:
							case null:
								goto label_3;
							default:
								entity = entity.Parent;
								continue;
						}
					}
				label_2:
					return (Scene) null;
				label_3:
					return (Scene) entity;
				}
			*/
			get
			{
				for (Entity entity = this; entity != null; entity = entity.Parent)
				{
					if (entity is Scene || entity == null)
					{
						return (Scene)entity;
					}
				}

				return null;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void AdoptChild(Entity child)
		{
			Vector3 worldPosition = this.WorldPosition;
			Matrix localToWorld = child.LocalToWorld;
			Matrix worldToLocal = this.WorldToLocal;
			Matrix matrix = localToWorld * worldToLocal;
			
			if (child.Parent != null)
			{
				child.RemoveFromParent();
			}
			
			base.Children.Add(child);
			Vector3 localScale;
			Quaternion localRotation;
			Vector3 localPosition;
			matrix.Decompose(out localScale, out localRotation, out localPosition);
			child.LocalPosition = localPosition;
			child.LocalRotation = localRotation;
			child.LocalScale = localScale;
			Vector3 worldPosition2 = this.WorldPosition;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected virtual void OnActionStarted(State action) {}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected virtual void OnActionComplete(State action) {}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void InjectAction(State action) =>
			this.InjectActions((IList<State>)new State[] { action });

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void InjectActions(IList<State> actions)
		{
			Queue<State> queue = new Queue<State>();
			
			for (int i = 0; i < actions.Count; i++)
			{
				queue.Enqueue(actions[i]);
			}
			
			if (this._currentAction != null)
			{
				queue.Enqueue(this._currentAction);
				this._currentAction = null;
			}
			
			foreach (State action in this._actionQueue)
			{
				queue.Enqueue(action);
			}
			
			this._actionQueue = queue;
		}

		/// <summary>
		/// 
		/// </summary>
		private void NextAction()
		{
			if (this._currentAction != null || this._actionQueue.Count <= 0)
			{
				return;
			}

			this._currentAction = this._actionQueue.Dequeue();
			this._currentAction.Start(this);
			this.OnActionStarted(this._currentAction);
		}

		/// <summary>
		/// 
		/// </summary>
		public void EndCurrentAction()
		{
			// Make sure there isn't an active action going on.
			if (this._currentAction == null)
			{
				return;
			}

			this._currentAction.End(this);
			this._currentAction = null;
			this.OnActionComplete(this._currentAction);
		}

		/// <summary>
		/// 
		/// </summary>
		public void ResetActions()
		{
			this.EndCurrentAction();
			this._actionQueue.Clear();
		}

		/// <summary>
		/// The entity's current action.
		/// </summary>
		public State CurrentAction => this._currentAction;

		/// <summary>
		/// Called every frame update.
		/// </summary>
		/// <param name=""></param>
		protected virtual void OnUpdate(GameTime gameTime) {}

		/// <summary>
		/// Called on physics update.
		/// </summary>
		/// <param name=""></param>
		public virtual void OnPhysics(GameTime gameTime) {}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public virtual void Update(DNAGame game, GameTime gameTime)
		{
			if (!this.DoUpdate)
			{
				return;
			}
			
			this.NextAction();
			
			if (this._currentAction != null)
			{
				this._currentAction.Tick(game, this, gameTime);
				
				if (this._currentAction != null && this._currentAction.Complete)
				{
					this.EndCurrentAction();
				}
			}
			
			this.NextAction();
			this.OnUpdate(gameTime);
			
			for (int i = 0; i < base.Children.Count; i++)
			{
				base.Children[i].Update(game, gameTime);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public virtual void Draw(
			GraphicsDevice device, GameTime gameTime, Matrix view, Matrix projection) {}

		/// <summary>
		/// Called after a frame is drawn.
		/// </summary>
		public void AfterFrame()
		{
			this.OnAfterFrame();
			
			for (int i = 0; i < base.Children.Count; i++)
			{
				base.Children[i].AfterFrame();
			}
		}

		/// <summary>
		/// Called after a frame is drawn.
		/// </summary>
		public virtual void OnAfterFrame() {}
	}
}
