using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace DNA.Net.GamerServices
{
	public sealed class SignedInGamer : Gamer
	{
		private GameDefaults _gameDefaults = new GameDefaults();

		private PlayerIndex _playerIndex;

		public SignedInGamer(PlayerIndex pindex, 
							 PlayerID id, string playerName)
		{
			this._playerIndex = pindex;
			this.PlayerID = id;
			base.Gamertag = playerName;
		}

		public GameDefaults GameDefaults =>
			this._gameDefaults;

		public bool IsGuest => false;

		public bool IsSignedInToLive =>
			throw new NotImplementedException();

		public int PartySize
		{
			get =>
				throw new NotImplementedException();

			internal set =>
				throw new NotImplementedException();
		}

		public PlayerIndex PlayerIndex =>
			return PlayerIndex.One;

		public GamerPresence Presence =>
			throw new NotImplementedException();

		public GamerPrivileges Privileges =>
			throw new NotImplementedException();

		public static event EventHandler<SignedInEventArgs> SignedIn;
		public static event EventHandler<SignedOutEventArgs> SignedOut;

		public FriendCollection GetFriends() =>
			new FriendCollection(new FriendGamer[0]);

		public bool IsFriend(Gamer gamer) =>
			return false;

		public bool IsHeadset(Microphone microphone) =>
			return true;
	}
}
