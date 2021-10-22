using System;
using DNA.Drawing.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public class CameraView : View
	{
		private FilterCallback<Entity> _filterBase;
		private FilterCallback<Entity> _filter;

		private Camera _camera;

		public static bool FilterDistortions(Entity e) => 
			(!(e is ParticleEmitter) || 
				!((ParticleEmitter)e).IsDistortionEffect) && !(e is IScreenDistortion);

		public Camera Camera
		{
			get => 
				this._camera;
			
			set
			{
				this._camera = value;
				
				if (this._camera != null && this._camera.Scene == null)
				{
					throw new Exception("Camera Must Be in Scene");
				}
			}
		}

		public CameraView(Game game, RenderTarget2D target, Camera camera) 
			: base(game, target)
		{
			this._filterBase = new FilterCallback<Entity>(this.FilterEntities);
			
			if (camera != null && camera.Scene == null)
			{
				throw new Exception("Camera Must Be in Scene");
			}
			
			this._camera = camera;
		}

		public CameraView(Game game, RenderTarget2D target, Camera camera, 
						  FilterCallback<Entity> filter) 
			: base(game, target)
		{
			this._filterBase = new FilterCallback<Entity>(this.FilterEntities);
			this._filter = filter;
				
			if (camera != null && camera.Scene == null)
			{
				throw new Exception("Camera Must Be in Scene");
			}
			
			this._camera = camera;
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, 
									   GameTime gameTime)
		{
			base.OnDraw(device, spriteBatch, gameTime);
			
			if (this.Camera != null)
			{
				this.Camera.Draw(device, spriteBatch, gameTime, this._filterBase);
			}
		}

		private bool FilterEntities(Entity e)
		{
			bool shouldFilter = true;
			
			if (this._filter != null)
			{
				shouldFilter = this._filter(e);
			}
			
			return shouldFilter && this.FilterEntity(e);
		}

		protected virtual bool FilterEntity(Entity e) => true;
	}
}
