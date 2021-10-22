using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.UI
{
	public class CreditsScreenItem
	{
		public string Name;

		public ItemTypes ItemType = ItemTypes.Normal;

		public Color? TextColor = null;

		public CreditsScreenItem(string name, ItemTypes itemType)
		{
			this.Name = name;
			this.ItemType = itemType;
		}

		public CreditsScreenItem(string name)
		{
			this.Name = name;
		}
	}
}
