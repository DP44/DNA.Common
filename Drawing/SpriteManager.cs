using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public class SpriteManager
	{
		public class SpriteManagerReader : ContentTypeReader<SpriteManager>
		{
			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			protected override SpriteManager Read(
				ContentReader input, SpriteManager existingInstance)
			{
				SpriteManager spriteManager = new SpriteManager();
				Texture2D texture = input.ReadObject<Texture2D>();
				
				int length = input.ReadInt32();
				
				for (int i = 0; i < length; i++)
				{
					string key = input.ReadString();
					
					Rectangle sourceRectangle = new Rectangle(
						input.ReadInt32(), input.ReadInt32(), 
						input.ReadInt32(), input.ReadInt32());
					
					spriteManager._sprites[key] = new Sprite(texture, sourceRectangle);
				}
			
				return spriteManager;
			}
		}

		private Dictionary<string, Sprite> _sprites = new Dictionary<string, Sprite>();

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public Sprite this[string name] =>
			this._sprites[name];
	}
}
