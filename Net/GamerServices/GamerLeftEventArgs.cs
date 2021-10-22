using System;

namespace DNA.Net.GamerServices
{
	public class GamerLeftEventArgs : EventArgs
	{
		private NetworkGamer _networkGamer;

		public GamerLeftEventArgs(NetworkGamer gamer) => 
			this._networkGamer = gamer;

		public NetworkGamer Gamer => 
			this._networkGamer;
	}
}
