using System;
using System.Collections.Generic;

namespace DNA.Net.GamerServices
{
	public sealed class LocalNetworkGamer : NetworkGamer
	{
		private class PendingDataPacket
		{
			private static List<LocalNetworkGamer.PendingDataPacket> _freePackets = 
				new List<LocalNetworkGamer.PendingDataPacket>();

			public NetworkGamer Sender;

			public byte[] Data;

			public static LocalNetworkGamer.PendingDataPacket Alloc(NetworkGamer sender, 
																	byte[] data)
			{
				LocalNetworkGamer.PendingDataPacket pendingDataPacket = null;
				
				lock (LocalNetworkGamer.PendingDataPacket._freePackets)
				{
					if (LocalNetworkGamer.PendingDataPacket._freePackets.Count > 0)
					{
						int index = LocalNetworkGamer.PendingDataPacket._freePackets.Count - 1;
						
						pendingDataPacket = 
							LocalNetworkGamer.PendingDataPacket._freePackets[index];
						
						LocalNetworkGamer.PendingDataPacket._freePackets.RemoveAt(index);
					}
				}

				if (pendingDataPacket == null)
				{
					pendingDataPacket = new LocalNetworkGamer.PendingDataPacket();
				}
				
				pendingDataPacket.Sender = sender;
				pendingDataPacket.Data = data;
				return pendingDataPacket;
			}

			public void Release()
			{
				this.Sender = null;
				this.Data = null;
				
				lock (LocalNetworkGamer.PendingDataPacket._freePackets)
				{
					LocalNetworkGamer.PendingDataPacket._freePackets.Add(this);
				}
			}
		}

		private SignedInGamer _signedInGamer;

		private List<LocalNetworkGamer.PendingDataPacket> _pendingData = 
			new List<LocalNetworkGamer.PendingDataPacket>();

		internal LocalNetworkGamer(SignedInGamer signedInGamer, NetworkSession session, 
								   bool isLocal, bool isHost, byte globalID, ulong steamID) 
			: base(signedInGamer, session, isLocal, isHost, globalID, steamID)
		{
			this._signedInGamer = signedInGamer;
		}

		public bool IsDataAvailable
		{
			get
			{
				return this._pendingData.Count > 0;
			}
		}

		public SignedInGamer SignedInGamer
		{
			get
			{
				return this._signedInGamer;
			}
		}

		public int ReceiveData(byte[] data, out NetworkGamer sender)
		{
			int result = 0;
			sender = null;
			
			lock (this._pendingData)
			{
				if (this._pendingData.Count > 0)
				{
					if (data.Length < this._pendingData[0].Data.Length)
					{
						throw new ArgumentException("Data buffer is too small");
					}

					LocalNetworkGamer.PendingDataPacket pendingDataPacket = 
						this._pendingData[0];
					
					this._pendingData.RemoveAt(0);
					pendingDataPacket.Data.CopyTo(data, 0);
					sender = pendingDataPacket.Sender;
					result = pendingDataPacket.Data.Length;
					pendingDataPacket.Release();
				}
			}
			
			return result;
		}

		public void AppendNewDataPacket(byte[] data, NetworkGamer sender)
		{
			LocalNetworkGamer.PendingDataPacket item = 
				LocalNetworkGamer.PendingDataPacket.Alloc(sender, data);
			
			lock (this._pendingData)
			{
				this._pendingData.Add(item);
			}
		}

		public void AppendNewDataPacket(byte[] data, int offset, 
										int length, NetworkGamer sender)
		{
			byte[] array = new byte[length];
			Buffer.BlockCopy(data, offset, array, 0, length);
			
			LocalNetworkGamer.PendingDataPacket item = 
				LocalNetworkGamer.PendingDataPacket.Alloc(sender, array);
			
			lock (this._pendingData)
			{
				this._pendingData.Add(item);
			}
		}

		public void SendData(byte[] data, SendDataOptions options)
		{
			GamerCollection<LocalNetworkGamer> localGamers = base.Session.LocalGamers;
			
			for (int i = 0; i < localGamers.Count; i++)
			{
				if (localGamers[i] != null && !localGamers[i].HasLeftSession)
				{
					localGamers[i].AppendNewDataPacket(data, this);
				}
			}
			
			if (base.IsHost)
			{
				GamerCollection<NetworkGamer> remoteGamers = base.Session.RemoteGamers;
				
				for (int j = 0; j < remoteGamers.Count; j++)
				{
					if (remoteGamers[j] != null && !remoteGamers[j].HasLeftSession)
					{
						base.Session.SendRemoteData(data, options, remoteGamers[j]);
					}
				}
				
				return;
			}

			base.Session.BroadcastRemoteData(data, options);
		}

		public void SendData(byte[] data, SendDataOptions options, NetworkGamer recipient)
		{
			if (recipient is LocalNetworkGamer)
			{
				LocalNetworkGamer localNetworkGamer = recipient as LocalNetworkGamer;
				localNetworkGamer.AppendNewDataPacket(data, this);
				return;
			}

			base.Session.SendRemoteData(data, options, recipient);
		}

		public void SendData(byte[] data, int offset, int count, SendDataOptions options)
		{
			GamerCollection<LocalNetworkGamer> localGamers = base.Session.LocalGamers;
			
			for (int i = 0; i < localGamers.Count; i++)
			{
				if (localGamers[i] != null && !localGamers[i].HasLeftSession)
				{
					localGamers[i].AppendNewDataPacket(data, offset, count, this);
				}
			}
			
			if (base.IsHost)
			{
				GamerCollection<NetworkGamer> remoteGamers = base.Session.RemoteGamers;
				
				for (int j = 0; j < remoteGamers.Count; j++)
				{
					if (remoteGamers[j] != null && !remoteGamers[j].HasLeftSession)
					{
						base.Session.SendRemoteData(data, offset, count, 
													options, remoteGamers[j]);
					}
				}

				return;
			}

			base.Session.BroadcastRemoteData(data, offset, count, options);
		}

		public void SendData(byte[] data, int offset, int count, 
							 SendDataOptions options, NetworkGamer recipient)
		{
			if (recipient is LocalNetworkGamer)
			{
				LocalNetworkGamer localNetworkGamer = recipient as LocalNetworkGamer;
				localNetworkGamer.AppendNewDataPacket(data, offset, count, this);
				return;
			}

			base.Session.SendRemoteData(data, offset, count, options, recipient);
		}

		public int ReceiveData(PacketReader data, out NetworkGamer sender) =>
			throw new NotImplementedException();

		public int ReceiveData(byte[] data, int offset, out NetworkGamer sender) =>
			throw new NotImplementedException();

		public void SendData(PacketWriter data, SendDataOptions options) =>
			throw new NotImplementedException();

		public void SendData(PacketWriter data, SendDataOptions options, 
							 NetworkGamer recipient) =>
			throw new NotImplementedException();

		public void SendPartyInvites() =>
			throw new NotImplementedException();
	}
}
