using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI
{
	public class SceneScreen : Screen
	{
		private List<Scene> _scenes = new List<Scene>();
		private List<View> _views = new List<View>();
		private Dictionary<Scene, int> scenes = new Dictionary<Scene, int>();

		public float TimeFactor = 1f;

		public List<Scene> Scenes => this._scenes;
		public List<View> Views => this._views;

		public SceneScreen(bool acceptInput, bool drawBehind) 
			: base(acceptInput, drawBehind) {}

		protected override void OnUpdate(DNAGame game, GameTime gameTime)
		{
			if (this.TimeFactor != 1f)
			{
				gameTime = new GameTime(
					TimeSpan.FromSeconds(
						gameTime.TotalGameTime.TotalSeconds * (double)this.TimeFactor), 
					TimeSpan.FromSeconds(
						gameTime.ElapsedGameTime.TotalSeconds * (double)this.TimeFactor));
			}

			for (int i = 0; i < this._scenes.Count; i++)
			{
				this._scenes[i].Update(game, gameTime);
			}
			
			base.OnUpdate(game, gameTime);
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, 
									   GameTime gameTime)
		{
			for (int i = 0; i < this._views.Count; i++)
			{
				if (this._views[i].Enabled)
				{
					this._views[i].Draw(device, spriteBatch, gameTime);
				}
			}
			
			for (int j = 0; j < this._scenes.Count; j++)
			{
				this._scenes[j].AfterFrame();
			}
			
			base.OnDraw(device, spriteBatch, gameTime);
		}
	}
}
