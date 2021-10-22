using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DNA.Drawing.Lights;
using DNA.Profiling;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public class Scene : Entity
	{
		private class DistanceComparer : IComparer<Entity>
		{
			private Scene _owner;

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public DistanceComparer(Scene owner) => 
				this._owner = owner;

			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			public int Compare(Entity e1, Entity e2)
			{
				Matrix matrix = e1.LocalToWorld * this._owner._view;
				e2.LocalToWorld * this._owner._view;
				Vector3 vector = Vector3.Transform(Vector3.Zero, matrix);
				Vector3 vector2 = Vector3.Transform(Vector3.Zero, matrix);
				
				if (vector2.Z > vector.Z)
				{
					return 1;
				}
				
				if (vector2.Z < vector.Z)
				{
					return -1;
				}
				
				return 0;
			}
		}

		private List<Entity> Colliders = new List<Entity>();
		private List<Entity> Collidees = new List<Entity>();
		private List<Light> _lights = new List<Light>();
		private ReadOnlyCollection<Light> _lightsReadOnly;
		private Matrix _view;
		private List<Entity> toDraw = new List<Entity>();
		private List<Entity> toSort = new List<Entity>();
		private IComparer<Entity> comparer;

		/// <summary>
		/// 
		/// </summary>
		public Scene() => 
			this._lightsReadOnly = this._lights.AsReadOnly();

		/// <summary>
		/// 
		/// </summary>
		public ReadOnlyCollection<Light> Lights => 
			this._lightsReadOnly;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		protected override void OnUpdate(GameTime gameTime)
		{
			double totalSeconds = gameTime.ElapsedGameTime.TotalSeconds;
			
			for (int i = 0; i < base.Children.Count; i++)
			{
				Entity entity = base.Children[i];
				
				if (entity.Physics != null)
				{
					entity.Physics.Accelerate(gameTime.ElapsedGameTime);
				}
			}
			
			this.Colliders.Clear();
			this.Collidees.Clear();
			
			for (int j = 0; j < base.Children.Count; j++)
			{
				Entity entity2 = base.Children[j];
				
				if (entity2.Collider)
				{
					this.Colliders.Add(entity2);
				}
				
				if (entity2.Collidee)
				{
					this.Collidees.Add(entity2);
				}
			}
			
			using (Profiler.TimeSection("Collision", ProfilerThreadEnum.MAIN))
			{
				for (int k = 0; k < this.Colliders.Count; k++)
				{
					Entity entity3 = this.Colliders[k];
					entity3.ResolveCollsions(this.Collidees, gameTime);
				}
			}
			
			for (int l = 0; l < base.Children.Count; l++)
			{
				Entity entity4 = base.Children[l];
				
				if (entity4.Physics != null)
				{
					entity4.Physics.Move(gameTime.ElapsedGameTime);
				}
			}
			
			base.OnUpdate(gameTime);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		private void DrawList(GraphicsDevice device, List<Entity> drawList, 
							  GameTime gameTime, Matrix projection)
		{
			int num = int.MaxValue;
			int count = drawList.Count;
			
			for (int i = 0; i < count; i++)
			{
				num = Math.Min(num, drawList[i].DrawPriority);
			}
			
			bool flag = false;
			
			while (!flag)
			{
				flag = true;
				int num2 = num;
				num = int.MaxValue;
				
				for (int j = 0; j < count; j++)
				{
					Entity entity = drawList[j];
					
					if (num2 == entity.DrawPriority)
					{
						entity.SetRenderState(device);
						entity.Draw(device, gameTime, this._view, projection);
					}
					else if (entity.DrawPriority > num2 && entity.DrawPriority < num)
					{
						num = entity.DrawPriority;
						flag = false;
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void Draw(GraphicsDevice device, GameTime gameTime, Matrix view, 
						 Matrix projection, FilterCallback<Entity> filter)
		{
			if (this.comparer == null)
			{
				this.comparer = new Scene.DistanceComparer(this);
			}
			
			this._view = view;
			this._lights.Clear();
			this.toSort.Clear();
			this.toDraw.Clear();
			base.GetDrawList(this._lights, this.toSort, this.toDraw, filter);
			this.DrawList(device, this.toDraw, gameTime, projection);
			this.toSort.Sort(this.comparer);
			this.DrawList(device, this.toSort, gameTime, projection);
			base.Draw(device, gameTime, view, projection);
		}
	}
}
