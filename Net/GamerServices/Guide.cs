using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DNA.Net.GamerServices
{
	public static class Guide
	{
		private static bool _isTrialMode = false;

		public static NotificationPosition NotificationPosition = 
			NotificationPosition.BottomCenter;

		public static bool IsScreenSaverEnabled
		{
			get => 
				throw new NotImplementedException();
		
			set => 
				throw new NotImplementedException();
		}

		public static bool IsTrialMode
		{
			get => 
				Guide._isTrialMode;
		
			set => 
				Guide._isTrialMode = value;
		}

		public static bool IsVisible => false;

		public static bool SimulateTrialMode
		{
			get => 
				false;

			set
			{
				// what
			}
		}

		public static IAsyncResult BeginShowKeyboardInput(PlayerIndex player, string title, 
														  string description, string defaultText, 
														  AsyncCallback callback, object state) =>
			throw new NotImplementedException();

		public static IAsyncResult BeginShowKeyboardInput(PlayerIndex player, string title, 
														  string description, string defaultText, 
														  AsyncCallback callback, object state, 
														  bool usePasswordMode) =>
			throw new NotImplementedException();

		public static IAsyncResult BeginShowMessageBox(string title, string text, 
													   IEnumerable<string> buttons, 
													   int focusButton, MessageBoxIcon icon, 
													   AsyncCallback callback, object state) =>
			throw new NotImplementedException();

		public static IAsyncResult BeginShowMessageBox(PlayerIndex player, string title, 
													   string text, IEnumerable<string> buttons, 
													   int focusButton, MessageBoxIcon icon, 
													   AsyncCallback callback, object state) =>
			throw new NotImplementedException();

		public static void DelayNotifications(TimeSpan delay) =>
			throw new NotImplementedException();

		public static string EndShowKeyboardInput(IAsyncResult result) =>
			throw new NotImplementedException();

		public static int? EndShowMessageBox(IAsyncResult result) =>
			throw new NotImplementedException();

		public static void ShowComposeMessage(PlayerIndex player, string text, IEnumerable<Gamer> recipients) =>
			throw new NotImplementedException();

		public static void ShowFriendRequest(PlayerIndex player, Gamer gamer) => 
			throw new NotImplementedException();

		public static void ShowFriends(PlayerIndex player) => 
			throw new NotImplementedException();

		public static void ShowGameInvite(string sessionId) => 
			throw new NotImplementedException();

		public static void ShowGameInvite(PlayerIndex player, IEnumerable<Gamer> recipients) => 
			throw new NotImplementedException();

		public static void ShowGamerCard(PlayerIndex player, Gamer gamer) => 
			throw new NotImplementedException();

		public static void ShowMarketplace(PlayerIndex player) => 
			throw new NotImplementedException();

		public static void ShowMessages(PlayerIndex player) => 
			throw new NotImplementedException();

		public static void ShowParty(PlayerIndex player) => 
			throw new NotImplementedException();

		public static void ShowPartySessions(PlayerIndex player) => 
			throw new NotImplementedException();

		public static void ShowPlayerReview(PlayerIndex player, Gamer gamer) => 
			throw new NotImplementedException();

		public static void ShowPlayers(PlayerIndex player) => 
			throw new NotImplementedException();

		public static void ShowSignIn(int paneCount, bool onlineOnly) => 
			throw new NotImplementedException();

	}
}
