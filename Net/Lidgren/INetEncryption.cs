using System;

namespace DNA.Net.Lidgren
{
	public interface INetEncryption
	{
		bool Encrypt(NetOutgoingMessage msg);
		bool Decrypt(NetIncomingMessage msg);
	}
}
