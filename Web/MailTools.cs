using System;
using System.Diagnostics;
using System.Net.Mail;

namespace DNA.Web
{
	public static class MailTools
	{
		public static void SendDefaultMailClientEmail(MailMessage message)
		{
			string mailProcess = string.Format(
				"mailto:{0}?subject={1}&body={2}", message.To, message.Subject, message.Body);
			
			if (message.CC.Count > 0)
			{
				mailProcess += "&CC=";
			}

			for (int i = 0; i < message.CC.Count; i++)
			{
				MailAddress mail = message.CC[i];
				mailProcess += mail.Address;
				
				if (i < message.CC.Count - 1)
				{
					mailProcess += ";";
				}
			}
			
			if (message.Bcc.Count > 0)
			{
				mailProcess += "&BCC=";
			}
			
			for (int j = 0; j < message.Bcc.Count; j++)
			{
				MailAddress mail = message.Bcc[j];
				mailProcess += mail.Address;
			
				if (j < message.Bcc.Count - 1)
				{
					mailProcess += ";";
				}
			}
			
			Process.Start(mailProcess);
		}
	}
}
