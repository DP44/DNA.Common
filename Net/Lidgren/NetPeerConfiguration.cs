using System;
using System.Globalization;
using System.Net;

namespace DNA.Net.Lidgren
{
	public sealed class NetPeerConfiguration
	{
		private const string c_isLockedMessage = "You may not modify the NetPeerConfiguration after it has been used to initialize a NetPeer";

		private bool m_isLocked;

		private readonly string m_appIdentifier;

		private string m_networkThreadName;

		private IPAddress m_localAddress;

		private IPAddress m_broadcastAddress;

		internal bool m_acceptIncomingConnections;

		internal int m_maximumConnections;

		internal int m_defaultOutgoingMessageCapacity;

		internal float m_pingInterval;

		internal bool m_useMessageRecycling;

		internal float m_connectionTimeout;

		internal bool m_enableUPnP;

		internal bool m_autoFlushSendQueue;

		internal NetIncomingMessageType m_disabledTypes;

		internal int m_port;

		internal int m_receiveBufferSize;

		internal int m_sendBufferSize;

		internal float m_resendHandshakeInterval;

		internal int m_maximumHandshakeAttempts;

		internal float m_loss;

		internal float m_duplicates;

		internal float m_minimumOneWayLatency;

		internal float m_randomOneWayLatency;

		internal int m_maximumTransmissionUnit;

		internal bool m_autoExpandMTU;

		internal float m_expandMTUFrequency;

		internal int m_expandMTUFailAttempts;

		public NetPeerConfiguration(string appIdentifier)
		{
			if (string.IsNullOrEmpty(appIdentifier))
			{
				throw new NetException("App identifier must be at least one character long");
			}
			this.m_appIdentifier = appIdentifier.ToString(CultureInfo.InvariantCulture);
			this.m_disabledTypes = (NetIncomingMessageType)4230;
			this.m_networkThreadName = "Lidgren network thread";
			this.m_localAddress = IPAddress.Any;
			this.m_broadcastAddress = IPAddress.Broadcast;
			IPAddress broadcastAddress = NetUtility.GetBroadcastAddress();
			if (broadcastAddress != null)
			{
				this.m_broadcastAddress = broadcastAddress;
			}
			this.m_port = 0;
			this.m_receiveBufferSize = 131071;
			this.m_sendBufferSize = 131071;
			this.m_acceptIncomingConnections = false;
			this.m_maximumConnections = 32;
			this.m_defaultOutgoingMessageCapacity = 16;
			this.m_pingInterval = 4f;
			this.m_connectionTimeout = 25f;
			this.m_useMessageRecycling = true;
			this.m_resendHandshakeInterval = 3f;
			this.m_maximumHandshakeAttempts = 5;
			this.m_autoFlushSendQueue = true;
			this.m_maximumTransmissionUnit = 1408;
			this.m_autoExpandMTU = false;
			this.m_expandMTUFrequency = 2f;
			this.m_expandMTUFailAttempts = 5;
			this.m_loss = 0f;
			this.m_minimumOneWayLatency = 0f;
			this.m_randomOneWayLatency = 0f;
			this.m_duplicates = 0f;
			this.m_isLocked = false;
		}

		internal void Lock()
		{
			this.m_isLocked = true;
		}

		public string AppIdentifier
		{
			get
			{
				return this.m_appIdentifier;
			}
		}

		public void EnableMessageType(NetIncomingMessageType type)
		{
			this.m_disabledTypes &= ~type;
		}

		public void DisableMessageType(NetIncomingMessageType type)
		{
			this.m_disabledTypes |= type;
		}

		public void SetMessageTypeEnabled(NetIncomingMessageType type, bool enabled)
		{
			if (enabled)
			{
				this.m_disabledTypes &= ~type;
				return;
			}
			this.m_disabledTypes |= type;
		}

		public bool IsMessageTypeEnabled(NetIncomingMessageType type)
		{
			return (this.m_disabledTypes & type) != type;
		}

		public string NetworkThreadName
		{
			get
			{
				return this.m_networkThreadName;
			}
			set
			{
				if (this.m_isLocked)
				{
					throw new NetException("NetworkThreadName may not be set after the NetPeer which uses the configuration has been started");
				}
				this.m_networkThreadName = value;
			}
		}

		public int MaximumConnections
		{
			get
			{
				return this.m_maximumConnections;
			}
			set
			{
				if (this.m_isLocked)
				{
					throw new NetException("You may not modify the NetPeerConfiguration after it has been used to initialize a NetPeer");
				}
				this.m_maximumConnections = value;
			}
		}

		public int MaximumTransmissionUnit
		{
			get
			{
				return this.m_maximumTransmissionUnit;
			}
			set
			{
				if (this.m_isLocked)
				{
					throw new NetException("You may not modify the NetPeerConfiguration after it has been used to initialize a NetPeer");
				}
				if (value < 1 || value >= 8192)
				{
					throw new NetException("MaximumTransmissionUnit must be between 1 and " + 8191 + " bytes");
				}
				this.m_maximumTransmissionUnit = value;
			}
		}

		public int DefaultOutgoingMessageCapacity
		{
			get
			{
				return this.m_defaultOutgoingMessageCapacity;
			}
			set
			{
				this.m_defaultOutgoingMessageCapacity = value;
			}
		}

		public float PingInterval
		{
			get
			{
				return this.m_pingInterval;
			}
			set
			{
				this.m_pingInterval = value;
			}
		}

		public bool UseMessageRecycling
		{
			get
			{
				return this.m_useMessageRecycling;
			}
			set
			{
				if (this.m_isLocked)
				{
					throw new NetException("You may not modify the NetPeerConfiguration after it has been used to initialize a NetPeer");
				}
				this.m_useMessageRecycling = value;
			}
		}

		public float ConnectionTimeout
		{
			get
			{
				return this.m_connectionTimeout;
			}
			set
			{
				if (value < this.m_pingInterval)
				{
					throw new NetException("Connection timeout cannot be lower than ping interval!");
				}
				this.m_connectionTimeout = value;
			}
		}

		public bool EnableUPnP
		{
			get
			{
				return this.m_enableUPnP;
			}
			set
			{
				if (this.m_isLocked)
				{
					throw new NetException("You may not modify the NetPeerConfiguration after it has been used to initialize a NetPeer");
				}
				this.m_enableUPnP = value;
			}
		}

		public bool AutoFlushSendQueue
		{
			get
			{
				return this.m_autoFlushSendQueue;
			}
			set
			{
				this.m_autoFlushSendQueue = value;
			}
		}

		public IPAddress LocalAddress
		{
			get
			{
				return this.m_localAddress;
			}
			set
			{
				if (this.m_isLocked)
				{
					throw new NetException("You may not modify the NetPeerConfiguration after it has been used to initialize a NetPeer");
				}
				this.m_localAddress = value;
			}
		}

		public IPAddress BroadcastAddress
		{
			get
			{
				return this.m_broadcastAddress;
			}
			set
			{
				if (this.m_isLocked)
				{
					throw new NetException("You may not modify the NetPeerConfiguration after it has been used to initialize a NetPeer");
				}
				this.m_broadcastAddress = value;
			}
		}

		public int Port
		{
			get
			{
				return this.m_port;
			}
			set
			{
				if (this.m_isLocked)
				{
					throw new NetException("You may not modify the NetPeerConfiguration after it has been used to initialize a NetPeer");
				}
				this.m_port = value;
			}
		}

		public int ReceiveBufferSize
		{
			get
			{
				return this.m_receiveBufferSize;
			}
			set
			{
				if (this.m_isLocked)
				{
					throw new NetException("You may not modify the NetPeerConfiguration after it has been used to initialize a NetPeer");
				}
				this.m_receiveBufferSize = value;
			}
		}

		public int SendBufferSize
		{
			get
			{
				return this.m_sendBufferSize;
			}
			set
			{
				if (this.m_isLocked)
				{
					throw new NetException("You may not modify the NetPeerConfiguration after it has been used to initialize a NetPeer");
				}
				this.m_sendBufferSize = value;
			}
		}

		public bool AcceptIncomingConnections
		{
			get
			{
				return this.m_acceptIncomingConnections;
			}
			set
			{
				this.m_acceptIncomingConnections = value;
			}
		}

		public float ResendHandshakeInterval
		{
			get
			{
				return this.m_resendHandshakeInterval;
			}
			set
			{
				this.m_resendHandshakeInterval = value;
			}
		}

		public int MaximumHandshakeAttempts
		{
			get
			{
				return this.m_maximumHandshakeAttempts;
			}
			set
			{
				if (value < 1)
				{
					throw new NetException("MaximumHandshakeAttempts must be at least 1");
				}
				this.m_maximumHandshakeAttempts = value;
			}
		}

		public bool AutoExpandMTU
		{
			get
			{
				return this.m_autoExpandMTU;
			}
			set
			{
				if (this.m_isLocked)
				{
					throw new NetException("You may not modify the NetPeerConfiguration after it has been used to initialize a NetPeer");
				}
				this.m_autoExpandMTU = value;
			}
		}

		public float ExpandMTUFrequency
		{
			get
			{
				return this.m_expandMTUFrequency;
			}
			set
			{
				this.m_expandMTUFrequency = value;
			}
		}

		public int ExpandMTUFailAttempts
		{
			get
			{
				return this.m_expandMTUFailAttempts;
			}
			set
			{
				this.m_expandMTUFailAttempts = value;
			}
		}

		public NetPeerConfiguration Clone()
		{
			NetPeerConfiguration netPeerConfiguration = base.MemberwiseClone() as NetPeerConfiguration;
			netPeerConfiguration.m_isLocked = false;
			return netPeerConfiguration;
		}
	}
}
