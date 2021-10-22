using System;
using System.IO;
using DNA.Net.GamerServices;

namespace DNA.Net
{
	public class ChatMessage : Message
	{
		public string Message = "";

		private ChatMessage() {}

		public static void Send(LocalNetworkGamer from, string message)
		{
			ChatMessage sendInstance = DNA.Net.Message.GetSendInstance<ChatMessage>();
			sendInstance.Message = message;
			sendInstance.DoSend(from);
		}

		protected override SendDataOptions SendDataOptions => 
			SendDataOptions.Chat;

		protected override void RecieveData(BinaryReader reader) => 
			this.Message = reader.ReadString();

		protected override void SendData(BinaryWriter writer) => 
			writer.Write(this.Message);
	}
}
