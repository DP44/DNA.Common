using System;
using DNA.Net.GamerServices;

namespace DNA.Net.MatchMaking
{
	public class QuerySessionInfo
	{
		public NetworkSessionProperties _props = 
			new NetworkSessionProperties();

		public int? MaxPlayers { get; set; }
		public int? MinOpenSlots { get; set; }
		public int? MaxOpenSlots { get; set; }

		public NetworkSessionProperties SessionProperties =>
			this._props;
	}
}
