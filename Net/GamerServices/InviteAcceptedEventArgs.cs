using System;

namespace DNA.Net.GamerServices
{
	public class InviteAcceptedEventArgs : EventArgs
	{
		public ulong LobbyId;
		public ulong InviterId;

		private SignedInGamer _gamer;

		public InviteAcceptedEventArgs(SignedInGamer gamer, bool isCurrentSession) => 
			this._gamer = gamer;

		public SignedInGamer Gamer => 
			this._gamer;

		public bool IsCurrentSession => 
			throw new NotImplementedException();
	}
}
