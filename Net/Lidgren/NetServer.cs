using System;
using System.Collections.Generic;

namespace DNA.Net.Lidgren
{
	public class NetServer : NetPeer
	{
		public NetServer(NetPeerConfiguration config)
			: base(config)
		{
			config.AcceptIncomingConnections = true;
		}

		public void SendToAll(NetOutgoingMessage msg, NetDeliveryMethod method)
		{
			List<NetConnection> connections = base.Connections;
			
			if (connections.Count <= 0)
			{
				return;
			}

			base.SendMessage(msg, connections, method, 0);
		}

		public void SendToAll(NetOutgoingMessage msg, NetConnection except, 
							  NetDeliveryMethod method, int sequenceChannel)
		{
			List<NetConnection> connections = base.Connections;

			if (connections.Count <= 0)
			{
				return;
			}
			
			if (except == null)
			{
				base.SendMessage(msg, connections, method, sequenceChannel);
				return;
			}
			
			List<NetConnection> list = new List<NetConnection>(connections.Count - 1);
			
			foreach (NetConnection netConnection in connections)
			{
				if (netConnection != except)
				{
					list.Add(netConnection);
				}
			}
			
			if (list.Count > 0)
			{
				base.SendMessage(msg, list, method, sequenceChannel);
			}
		}

		public override string ToString() =>
			"[NetServer " + base.ConnectionsCount + " connections]";
	}
}
