using System;
using System.Collections.Generic;
using DNA.Net.GamerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;

namespace DNA
{
	public class DialogManager
	{
		public delegate void MessageCallback(int? result);
		public delegate void StorageCallback(StorageDevice device);

		private abstract class DialogBox
		{
			protected DNAGame Game;

			public DialogBox(DNAGame game) => 
				this.Game = game;

			public abstract void Show();
		}

		private class MessageBox : DialogManager.DialogBox
		{
			private PlayerIndex Player;
			private string Title;
			private string Text;
			private IEnumerable<string> Buttons;
			private int FocusButton;
			private MessageBoxIcon Icon;
			private DialogManager.MessageCallback Callback;
			private object State;

			public MessageBox(DNAGame game, PlayerIndex player, string title, string text, 
							  IEnumerable<string> buttons, int focusButton, MessageBoxIcon icon, 
							  DialogManager.MessageCallback callback, object state) : base(game)
			{
				this.Player = player;
				this.Title = title;
				this.Text = text;
				this.Buttons = buttons;
				this.FocusButton = focusButton;
				this.Icon = icon;
				this.Callback = callback;
				this.State = state;
			}

			private void MessageCallback(IAsyncResult result)
			{
				int? result2 = Guide.EndShowMessageBox(result);
				
				if (this.Callback != null)
				{
					this.Callback(result2);
				}
			}

			public override void Show() => 
				Guide.BeginShowMessageBox(this.Player, this.Title, this.Text, 
										  this.Buttons, this.FocusButton, this.Icon, 
										  new AsyncCallback(this.MessageCallback), 
										  this.State);
		}

		private class SignIn : DialogManager.DialogBox
		{
			private bool OnlineOnly;

			public SignIn(DNAGame game, bool onlineOnly) : base(game)
			{
				this.OnlineOnly = onlineOnly;
			}

			public override void Show() => Guide.ShowSignIn(1, this.OnlineOnly);
		}

		private class Marketplace : DialogManager.DialogBox
		{
			public PlayerIndex Player;

			public Marketplace(DNAGame game, PlayerIndex player) : base(game)
			{
				this.Player = player;
			}

			public override void Show()
			{
				SignedInGamer gamer = Gamer.SignedInGamers[this.Player];
				
				if (gamer != null && gamer.Privileges.AllowPurchaseContent)
				{
					Guide.ShowMarketplace(this.Player);
				}
				else
				{
					this.Game.DialogManager.ShowSignIn(false);
				}
			}
		}

		private class Storage : DialogManager.DialogBox
		{
			public PlayerIndex Player;
			private DialogManager.StorageCallback Callback;
			private object State;

			public Storage(DNAGame game, PlayerIndex player,
						   DialogManager.StorageCallback callback, object state) 
				: base(game)
			{
				this.Player = player;
				this.Callback = callback;
				this.State = state;
			}

			private void EndStorage(IAsyncResult result)
			{
				if (this.Callback == null)
				{
					return;
				}

				this.Callback(StorageDevice.EndShowSelector(result));
			}

			public override void Show() => 
				StorageDevice.BeginShowSelector(
					this.Player, new AsyncCallback(this.EndStorage), this.State);
		}

		private Queue<DialogManager.DialogBox> _pendingMessages = 
			new Queue<DialogManager.DialogBox>();

		private DNAGame _game;


		public DialogManager(DNAGame game) => 
			this._game = game;


		public void ShowStorage(PlayerIndex player, DialogManager.StorageCallback callback, 
								object state) => 
			this._pendingMessages.Enqueue(
				(DialogManager.DialogBox)new DialogManager.Storage(
					this._game, player, callback, state));


		public void ShowSignIn(bool OnLineOnly) => 
			this._pendingMessages.Enqueue(
				(DialogManager.DialogBox)new DialogManager.SignIn(this._game, OnLineOnly));


		public void ShowMarketPlace(PlayerIndex player) => 
			this._pendingMessages.Enqueue(
				(DialogManager.DialogBox)new DialogManager.Marketplace(this._game, player));


		public void ShowMessageBox(PlayerIndex player, string title, string text, 
								   IEnumerable<string> buttons, int focusButton, 
								   MessageBoxIcon Icon, DialogManager.MessageCallback callback, 
								   object state)
		{
			this._pendingMessages.Enqueue(
				(DialogManager.DialogBox)new DialogManager.MessageBox(this._game, player, title, 
																	  text, buttons, focusButton, 
																	  Icon, callback, state));
		}

		public void Update(GameTime time)
		{
			try
			{
				if (Guide.IsVisible || this._pendingMessages.Count <= 0)
				{
					return;
				}

				this._pendingMessages.Dequeue().Show();
			}
			catch
			{
				// Swallow exceptions.
			}
		}
	}
}
