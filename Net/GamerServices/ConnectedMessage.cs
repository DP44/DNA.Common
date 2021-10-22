using System;
using System.Collections.Generic;

namespace DNA.Net.GamerServices
{
	public class ConnectedMessage
	{
		public byte PlayerGID;
		public Gamer[] Peers;
		public byte[] ids;

		public void SetPeerList(List<NetworkGamer> peerList)
		{
			if (peerList.Count == 0)
			{
				this.Peers = null;
				return;
			}
			
			this.Peers = peerList.ToArray();
			this.ids = new byte[peerList.Count];
			
			for (int i = 0; i < peerList.Count; i++)
			{
				this.ids[i] = peerList[i].Id;
			}
		}
	}
}
