using System;
using System.IO;
using DNA.Net.GamerServices;

namespace DNA.Net
{
	public class VoiceChatMessage : Message
	{
		public byte[] AudioBuffer = new byte[0];

		private VoiceChatMessage() {}

		public static void Send(LocalNetworkGamer from, byte[] _audioBuffer)
		{
			VoiceChatMessage sendInstance = 
				Message.GetSendInstance<VoiceChatMessage>();
			
			sendInstance.AudioBuffer = _audioBuffer;
			sendInstance.DoSend(from);
		}

		protected override SendDataOptions SendDataOptions => 
			SendDataOptions.Chat;

		protected override void RecieveData(BinaryReader reader)
		{
			int dataLength = reader.ReadInt32();
			
			if (this.AudioBuffer.Length != dataLength)
			{
				this.AudioBuffer = new byte[dataLength];
			}
			
			this.AudioBuffer = reader.ReadBytes(dataLength);
		}

		protected override void SendData(BinaryWriter writer)
		{
			writer.Write(this.AudioBuffer.Length);
			writer.Write(this.AudioBuffer);
		}
	}
}
