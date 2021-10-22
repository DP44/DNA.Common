using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public abstract class Camera : Entity
	{
		public abstract Matrix GetProjection(GraphicsDevice device);

		public abstract Matrix View { get; }

		public virtual void Draw(GraphicsDevice device, 
								 SpriteBatch spriteBatch, 
								 GameTime time, 
								 FilterCallback<Entity> entityFilter)
		{
			this.Scene?.Draw(device, time, this.View, 
				this.GetProjection(device), entityFilter);
		}
	}
}
