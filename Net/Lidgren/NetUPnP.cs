using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;

namespace DNA.Net.Lidgren
{
	public class NetUPnP
	{
		private const int c_discoveryTimeOutMillis = 1000;

		private string m_serviceUrl;

		private string m_serviceName = "";

		private NetPeer m_peer;

		private ManualResetEvent m_discoveryComplete = new ManualResetEvent(false);

		internal float m_discoveryResponseDeadline;

		private UPnPStatus m_status;

		public UPnPStatus Status
		{
			get
			{
				return this.m_status;
			}
		}

		public NetUPnP(NetPeer peer)
		{
			this.m_peer = peer;
			this.m_discoveryResponseDeadline = float.MinValue;
		}

		internal void Discover(NetPeer peer)
		{
			string s = "M-SEARCH * HTTP/1.1\r\nHOST: 239.255.255.250:1900\r\nST:upnp:rootdevice\r\nMAN:\"ssdp:discover\"\r\nMX:3\r\n\r\n";
			this.m_status = UPnPStatus.Discovering;
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			peer.Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
			peer.RawSend(bytes, 0, bytes.Length, new IPEndPoint(IPAddress.Broadcast, 1900));
			peer.Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, false);
			this.m_discoveryResponseDeadline = (float)NetTime.Now + 6f;
			this.m_status = UPnPStatus.Discovering;
		}

		internal void ExtractServiceUrl(string resp)
		{
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(WebRequest.Create(resp).GetResponse().GetResponseStream());
				XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
				xmlNamespaceManager.AddNamespace("tns", "urn:schemas-upnp-org:device-1-0");
				XmlNode xmlNode = xmlDocument.SelectSingleNode("//tns:device/tns:deviceType/text()", xmlNamespaceManager);
				if (xmlNode.Value.Contains("InternetGatewayDevice"))
				{
					this.m_serviceName = "WANIPConnection";
					XmlNode xmlNode2 = xmlDocument.SelectSingleNode("//tns:service[tns:serviceType=\"urn:schemas-upnp-org:service:" + this.m_serviceName + ":1\"]/tns:controlURL/text()", xmlNamespaceManager);
					if (xmlNode2 == null)
					{
						this.m_serviceName = "WANPPPConnection";
						xmlNode2 = xmlDocument.SelectSingleNode("//tns:service[tns:serviceType=\"urn:schemas-upnp-org:service:" + this.m_serviceName + ":1\"]/tns:controlURL/text()", xmlNamespaceManager);
						if (xmlNode2 == null)
						{
							return;
						}
					}
					this.m_serviceUrl = NetUPnP.CombineUrls(resp, xmlNode2.Value);
					this.m_status = UPnPStatus.Available;
					this.m_discoveryComplete.Set();
				}
			}
			catch
			{
			}
		}

		private static string CombineUrls(string gatewayURL, string subURL)
		{
			if (subURL.Contains("http:") || subURL.Contains("."))
			{
				return subURL;
			}
			gatewayURL = gatewayURL.Replace("http://", "");
			int num = gatewayURL.IndexOf("/");
			if (num != -1)
			{
				gatewayURL = gatewayURL.Substring(0, num);
			}
			return "http://" + gatewayURL + subURL;
		}

		private bool CheckAvailability()
		{
			switch (this.m_status)
			{
			case UPnPStatus.Discovering:
				if (this.m_discoveryComplete.WaitOne(1000))
				{
					return true;
				}
				if (NetTime.Now > (double)this.m_discoveryResponseDeadline)
				{
					this.m_status = UPnPStatus.NotAvailable;
				}
				return false;
			case UPnPStatus.NotAvailable:
				return false;
			case UPnPStatus.Available:
				return true;
			default:
				return false;
			}
		}

		public bool ForwardPort(int port, string description)
		{
			if (!this.CheckAvailability())
			{
				return false;
			}
			IPAddress ipaddress;
			IPAddress myAddress = NetUtility.GetMyAddress(out ipaddress);
			if (myAddress == null)
			{
				return false;
			}
			try
			{
				this.SOAPRequest(this.m_serviceUrl, string.Concat(new string[]
				{
					"<u:AddPortMapping xmlns:u=\"urn:schemas-upnp-org:service:",
					this.m_serviceName,
					":1\"><NewRemoteHost></NewRemoteHost><NewExternalPort>",
					port.ToString(),
					"</NewExternalPort><NewProtocol>",
					ProtocolType.Udp.ToString().ToUpper(),
					"</NewProtocol><NewInternalPort>",
					port.ToString(),
					"</NewInternalPort><NewInternalClient>",
					myAddress.ToString(),
					"</NewInternalClient><NewEnabled>1</NewEnabled><NewPortMappingDescription>",
					description,
					"</NewPortMappingDescription><NewLeaseDuration>0</NewLeaseDuration></u:AddPortMapping>"
				}), "AddPortMapping");
				Thread.Sleep(50);
			}
			catch (Exception ex)
			{
				this.m_peer.LogWarning("UPnP port forward failed: " + ex.Message);
				return false;
			}
			return true;
		}

		public bool DeleteForwardingRule(int port)
		{
			if (!this.CheckAvailability())
			{
				return false;
			}
			bool result;
			try
			{
				this.SOAPRequest(this.m_serviceUrl, string.Concat(new object[]
				{
					"<u:DeletePortMapping xmlns:u=\"urn:schemas-upnp-org:service:",
					this.m_serviceName,
					":1\"><NewRemoteHost></NewRemoteHost><NewExternalPort>",
					port,
					"</NewExternalPort><NewProtocol>",
					ProtocolType.Udp.ToString().ToUpper(),
					"</NewProtocol></u:DeletePortMapping>"
				}), "DeletePortMapping");
				result = true;
			}
			catch (Exception ex)
			{
				this.m_peer.LogWarning("UPnP delete forwarding rule failed: " + ex.Message);
				result = false;
			}
			return result;
		}

		public IPAddress GetExternalIP()
		{
			if (!this.CheckAvailability())
			{
				return null;
			}
			IPAddress result;
			try
			{
				XmlDocument xmlDocument = this.SOAPRequest(this.m_serviceUrl, "<u:GetExternalIPAddress xmlns:u=\"urn:schemas-upnp-org:service:" + this.m_serviceName + ":1\"></u:GetExternalIPAddress>", "GetExternalIPAddress");
				XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
				xmlNamespaceManager.AddNamespace("tns", "urn:schemas-upnp-org:device-1-0");
				string value = xmlDocument.SelectSingleNode("//NewExternalIPAddress/text()", xmlNamespaceManager).Value;
				result = IPAddress.Parse(value);
			}
			catch (Exception ex)
			{
				this.m_peer.LogWarning("Failed to get external IP: " + ex.Message);
				result = null;
			}
			return result;
		}

		private XmlDocument SOAPRequest(string url, string soap, string function)
		{
			string s = "<?xml version=\"1.0\"?><s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\"><s:Body>" + soap + "</s:Body></s:Envelope>";
			WebRequest webRequest = WebRequest.Create(url);
			webRequest.Method = "POST";
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			webRequest.Headers.Add("SOAPACTION", string.Concat(new string[]
			{
				"\"urn:schemas-upnp-org:service:",
				this.m_serviceName,
				":1#",
				function,
				"\""
			}));
			webRequest.ContentType = "text/xml; charset=\"utf-8\"";
			webRequest.ContentLength = (long)bytes.Length;
			webRequest.GetRequestStream().Write(bytes, 0, bytes.Length);
			XmlDocument xmlDocument = new XmlDocument();
			WebResponse response = webRequest.GetResponse();
			Stream responseStream = response.GetResponseStream();
			xmlDocument.Load(responseStream);
			return xmlDocument;
		}
	}
}
