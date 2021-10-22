using System;

namespace DNA.Drawing.UI
{
	public class SelectedMenuItemArgs : EventArgs
	{
		public MenuItemElement MenuItem;

		public SelectedMenuItemArgs(MenuItemElement control)
		{
			this.MenuItem = control;
		}
	}
}
