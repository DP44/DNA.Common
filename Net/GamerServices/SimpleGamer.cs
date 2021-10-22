using System;

namespace DNA.Net.GamerServices
{
	public class SimpleGamer : Gamer
	{
		public SimpleGamer()
		{
			this.PlayerID = PlayerID.Null;
			base.Gamertag = "";
		}

		public SimpleGamer(PlayerID pid, string name)
		{
			this.PlayerID = pid;
			base.Gamertag = name;
		}
	}
}
