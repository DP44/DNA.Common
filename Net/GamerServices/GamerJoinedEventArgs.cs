using System;

namespace DNA.Net.GamerServices
{
	public class GamerJoinedEventArgs : EventArgs
	{
		private NetworkGamer _gamer;

		public GamerJoinedEventArgs(NetworkGamer gamer) => 
			this._gamer = gamer;

		public NetworkGamer Gamer => 
			this._gamer;
	}
}
