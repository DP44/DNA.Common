using System;
using System.Text;

namespace DNA.Net.Lidgren
{
	public class NetXorEncryption : INetEncryption
	{
		private byte[] m_key;

		public NetXorEncryption(byte[] key) =>
			this.m_key = key;

		public NetXorEncryption(string key) =>
			this.m_key = Encoding.UTF8.GetBytes(key);

		public bool Encrypt(NetOutgoingMessage msg)
		{
			int lengthBytes = msg.LengthBytes;
			
			for (int i = 0; i < lengthBytes; i++)
			{
				int x = i % this.m_key.Length;
				msg.m_data[i] = (msg.m_data[i] ^ this.m_key[x]);
			}

			return true;
		}

		public bool Decrypt(NetIncomingMessage msg)
		{
			int lengthBytes = msg.LengthBytes;
			
			for (int i = 0; i < lengthBytes; i++)
			{
				int x = i % this.m_key.Length;
				msg.m_data[i] = (msg.m_data[i] ^ this.m_key[x]);
			}
			
			return true;
		}
	}
}
