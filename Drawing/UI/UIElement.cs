using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI
{
	public abstract class UIElement
	{
		private object _tag;
		private Color _color = Color.White;
		private Vector2 _location;
		private bool _visible = true;

		public object Tag
		{
			get => 
				this._tag;
			
			set => 
				this._tag = value;
		}

		public Color Color
		{
			get => 
				this._color;
			
			set => 
				this._color = value;
		}

		public RectangleF Bounds => 
			new RectangleF(this.Location, this.Size);

		public Vector2 Location
		{
			get => 
				this._location;
			
			set => 
				this._location = value;
		}

		public bool Visible
		{
			get => 
				this._visible;
			
			set
			{
				this._visible = value;
				this.OnVisibilityChanged(this._visible);
			}
		}

		public abstract Vector2 Size { get; set; }

		protected virtual void OnVisibilityChanged(bool visibility) {}

		protected abstract void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, 
									   GameTime gameTime, bool selected);

		public void Draw(GraphicsDevice device, SpriteBatch spriteBatch, 
						 GameTime gameTime, bool selected)
		{
			if (this._visible)
			{
				device.Viewport.TitleSafeArea;
				this.OnDraw(device, spriteBatch, gameTime, selected);
			}
		}

		protected virtual void OnUpdate(DNAGame game, GameTime gameTime) {}

		public void Update(DNAGame game, GameTime gameTime) => 
			this.OnUpdate(game, gameTime);
	}
}
