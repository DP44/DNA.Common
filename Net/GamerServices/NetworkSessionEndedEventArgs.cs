using System;

namespace DNA.Net.GamerServices
{
	public class NetworkSessionEndedEventArgs : EventArgs
	{
		private NetworkSessionEndReason _endReason;

		public NetworkSessionEndedEventArgs(NetworkSessionEndReason endReason) => 
			this._endReason = endReason;

		public NetworkSessionEndReason EndReason => 
			this._endReason;
	}
}
