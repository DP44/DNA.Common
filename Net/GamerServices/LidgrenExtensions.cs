using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using DNA.Net.Lidgren;

namespace DNA.Net.GamerServices
{
	public static class LidgrenExtensions
	{
		public static void Write(this NetBuffer msg, NetworkSessionProperties props)
		{
			msg.Write(props.Count);
			
			for (int i = 0; i < props.Count; i++)
			{
				int? num = props[i];
				
				if (num == null)
				{
					msg.Write(false);
				}
				else
				{
					msg.Write(true);
					msg.Write(num.Value);
				}
			}
		}

		public static NetworkSessionProperties ReadSessionProps(this NetBuffer msg)
		{
			NetworkSessionProperties networkSessionProperties = 
				new NetworkSessionProperties();
			
			int len = msg.ReadInt32();

			for (int i = 0; i < len; i++)
			{
				if (msg.ReadBoolean())
				{
					networkSessionProperties[i] = new int?(msg.ReadInt32());
				}
				else
				{
					networkSessionProperties[i] = null;
				}
			}

			return networkSessionProperties;
		}

		public static void Write(this NetBuffer msg, HostDiscoveryRequestMessage value, 
								 string validGameName, int validNetworkVersion)
		{
			value.Write(msg, validGameName, validNetworkVersion);
			msg.Write(value.RequestID);
			msg.Write(value.PlayerID);
		}

		public static HostDiscoveryRequestMessage ReadDiscoveryRequestMessage(
			this NetBuffer msg, string validGameName, int validNetworkVersion)
		{
			HostDiscoveryRequestMessage hostDiscoveryRequestMessage = 
				new HostDiscoveryRequestMessage();
			
			if (hostDiscoveryRequestMessage.ReadAndValidate(
					msg, validGameName, validNetworkVersion))
			{
				hostDiscoveryRequestMessage.RequestID = msg.ReadInt32();
				hostDiscoveryRequestMessage.PlayerID = msg.ReadPlayerID();
			}

			return hostDiscoveryRequestMessage;
		}

		public static void Write(this NetBuffer msg, HostDiscoveryResponseMessage hdrm, 
								 string validGameName, int validNetworkVersion)
		{
			hdrm.Write(msg, validGameName, validNetworkVersion);
			msg.Write((int)hdrm.Result);

			if (hdrm.Result == NetworkSession.ResultCode.Succeeded)
			{
				msg.Write((byte)hdrm.CurrentPlayers);
				msg.Write((byte)hdrm.MaxPlayers);
				msg.Write(hdrm.Message);
				msg.Write(hdrm.HostUsername);
				msg.Write(hdrm.RequestID);
				msg.Write(hdrm.SessionID);
				msg.Write(hdrm.PasswordProtected ? 1 : 0);
				msg.Write(hdrm.SessionProperties);
			}
		}

		public static HostDiscoveryResponseMessage ReadDiscoveryResponseMessage(
			this NetBuffer msg, string validGameName, int validNetworkVersion)
		{
			HostDiscoveryResponseMessage hostDiscoveryResponseMessage = 
				new HostDiscoveryResponseMessage();
			
			if (hostDiscoveryResponseMessage.ReadAndValidate(
					msg, validGameName, validNetworkVersion))
			{
				try
				{
					hostDiscoveryResponseMessage.Result = 
						(NetworkSession.ResultCode)msg.ReadInt32();

					if (hostDiscoveryResponseMessage.Result == 
							NetworkSession.ResultCode.Succeeded)
					{
						hostDiscoveryResponseMessage.CurrentPlayers = (int)msg.ReadByte();
						hostDiscoveryResponseMessage.MaxPlayers = (int)msg.ReadByte();
						hostDiscoveryResponseMessage.Message = msg.ReadString();
						hostDiscoveryResponseMessage.HostUsername = msg.ReadString();
						hostDiscoveryResponseMessage.RequestID = msg.ReadInt32();
						hostDiscoveryResponseMessage.SessionID = msg.ReadInt32();
						hostDiscoveryResponseMessage.PasswordProtected = msg.ReadInt32() == 1;
						hostDiscoveryResponseMessage.SessionProperties = msg.ReadSessionProps();
					}
				}
				catch
				{
					hostDiscoveryResponseMessage.Result = 
						NetworkSession.ResultCode.VersionIsInvalid;
				}
			}

			return hostDiscoveryResponseMessage;
		}

		public static void Write(this NetBuffer msg, RequestConnectToHostMessage cam, 
								 string validGameName, int validNetworkVersion)
		{
			cam.Write(msg, validGameName, validNetworkVersion);
			msg.Write(cam.SessionID);
		
			if (string.IsNullOrEmpty(cam.Password))
			{
				msg.Write(0);
			}
			else
			{
				msg.Write(1);
				msg.Write(cam.Password);
			}
		
			msg.Write(cam.Gamer);
			msg.Write(cam.SessionProperties);
		}

		public static RequestConnectToHostMessage ReadRequestConnectToHostMessage(
			this NetBuffer msg, string validGameName, int validVersion)
		{
			RequestConnectToHostMessage requestConnectToHostMessage = 
				new RequestConnectToHostMessage();
			
			if (requestConnectToHostMessage.ReadAndValidate(msg, validGameName, validVersion))
			{
				requestConnectToHostMessage.SessionID = msg.ReadInt32();
				
				int passwordProtected = msg.ReadInt32();
				
				requestConnectToHostMessage.Password = 
					passwordProtected == 1 ? msg.ReadString() : null;
			
				requestConnectToHostMessage.Gamer = msg.ReadGamer();
				requestConnectToHostMessage.SessionProperties = msg.ReadSessionProps();
			}

			return requestConnectToHostMessage;
		}

		public static void Write(this NetBuffer msg, DropPeerMessage data) =>
			msg.Write(data.PlayerGID);

		public static DropPeerMessage ReadDropPeerMessage(this NetBuffer msg) =>
			new DropPeerMessage { PlayerGID = msg.ReadByte() };

		public static void Write(this NetBuffer msg, ConnectedMessage cm)
		{
			msg.Write(cm.PlayerGID);
			msg.WriteArray(cm.Peers);
			msg.WriteArray(cm.ids);
		}

		public static ConnectedMessage ReadConnectedMessage(this NetBuffer msg) =>
			new ConnectedMessage { PlayerGID = msg.ReadByte(), 
								   Peers = msg.ReadGamerArray(), 
								   ids = msg.ReadByteArray() };

		public static void Write(this NetBuffer msg, Gamer gamer)
		{
			msg.Write(gamer.PlayerID);
			msg.Write(gamer.Gamertag);
		}

		public static Gamer ReadGamer(this NetBuffer msg)
		{
			PlayerID pid = msg.ReadPlayerID();
			string name = msg.ReadString();
			return new SimpleGamer(pid, name);
		}

		public static void WriteArray(this NetBuffer msg, Gamer[] data)
		{
			if (data == null)
			{
				msg.Write(-1);
			}
			else
			{
				msg.Write(data.Length);

				for (int i = 0; i < data.Length; i++)
				{
					msg.Write(data[i]);
				}
			}
		}

		public static Gamer[] ReadGamerArray(this NetBuffer msg)
		{
			Gamer[] gamerArray = null;
			int arrayLen = msg.ReadInt32();

			if (arrayLen > -1)
			{
				gamerArray = new SimpleGamer[arrayLen];
			
				for (int i = 0; i < arrayLen; i++)
				{
					gamerArray[i] = msg.ReadGamer();
				}
			}
			
			return gamerArray;
		}

		public static void Write(this NetBuffer msg, PlayerID id) =>
			msg.WriteArray(id.Data);

		public static PlayerID ReadPlayerID(this NetBuffer msg)
		{
			try
			{
				// byte[] hash = msg.ReadByteArray();
				return new PlayerID(msg.ReadByteArray());
			}
			catch
			{
				return null;
			}
		}

		public static void WriteArray(this NetBuffer msg, IPEndPoint[] data)
		{
			if (data == null)
			{
				msg.Write(-1);
			}
			else
			{
				msg.Write(data.Length);
				
				for (int i = 0; i < data.Length; i++)
				{
					msg.Write(data[i]);
				}
			}
		}

		public static IPEndPoint[] ReadIPEndPointArray(this NetBuffer msg)
		{
			int len = msg.ReadInt32();

			if (len == -1)
			{
				return null;
			}
			
			IPEndPoint[] array = new IPEndPoint[len];
			
			for (int i = 0; i < len; i++)
			{
				array[i] = msg.ReadIPEndPoint();
			}
			
			return array;
		}

		public static void CopyByteArrayFrom(this NetBuffer msg, NetBuffer src)
		{
			int byteCount = src.ReadInt32();
			msg.Write(byteCount);
			
			if (byteCount > 0)
			{
				msg.CopyBytesFrom(src, byteCount);
			}
		}

		public static void WriteArray(this NetBuffer msg, byte[] data)
		{
			if (data == null)
			{
				msg.Write(-1);
			}
			else
			{
				msg.Write(data.Length);

				if (data.Length > 0)
				{
					msg.Write(data);
				}
			}
		}

		public static void WriteArray(this NetBuffer msg, byte[] data, 
									  int offset, int length)
		{
			if (data == null)
			{
				msg.Write(-1);
			}
			else
			{
				msg.Write(length);

				if (length > 0)
				{
					msg.Write(data, offset, length);
				}
			}
		}

		public static byte[] ReadByteArray(this NetBuffer msg)
		{
			int numberOfBytes = msg.ReadInt32();
		
			switch (numberOfBytes)
			{
				case -1:
				{
					return null;
				}
				
				case 0:
				{
					return new byte[0];
				}
				
				default:
				{
					return msg.ReadBytes(numberOfBytes);
				}
			}
		}

		public static string GetPublicIP()
		{
			if (!NetworkInterface.GetIsNetworkAvailable())
			{
				return CommonResources.Not_connected_to_internet;
			}
			
			string publicIP = "";
			
			WebRequest webRequest = WebRequest.Create("http://checkip.dyndns.org");
			webRequest.Timeout = 4000;
			
			for (int i = 0; i < 5; i++)
			{
				try
				{
					using (WebResponse response = webRequest.GetResponse())
					{
						using (StreamReader streamReader = 
							new StreamReader(response.GetResponseStream()))
						{
							publicIP = streamReader.ReadToEnd();
						}
					}

					int num = publicIP.IndexOf("Address: ") + 9;
					int num2 = publicIP.LastIndexOf("</body>");
					publicIP = publicIP.Substring(num, num2 - num);
					i = 5;
				}
				catch (WebException ex)
				{
					publicIP = string.Concat(
						CommonResources.Error_getting_address_dyndns_returned, ": ", 
						ex.Status.ToString(), " ", ex.Response);
				}
			}

			return publicIP;
		}

		public static string GetLanIPAddress()
		{
			if (!NetworkInterface.GetIsNetworkAvailable())
			{
				return CommonResources.No_network_is_available;
			}
			
			IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
			
			IPAddress ipaddr = host.AddressList.FirstOrDefault(
				(IPAddress ip) => ip.AddressFamily == AddressFamily.InterNetwork);
			
			if (ipaddr != null)
			{
				return ipaddr.ToString();
			}
			
			return CommonResources.Couldn_t_get_local_address;
		}
	}
}
