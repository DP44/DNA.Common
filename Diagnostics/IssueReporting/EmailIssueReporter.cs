using System;
using System.Net.Mail;
using System.Windows.Forms;
using DNA.Web;

namespace DNA.Diagnostics.IssueReporting
{
	public class EmailIssueReporter : IssueReporter
	{
		private string _gameName;

		private Version _version;

		private DateTime _startTime;

		public EmailIssueReporter(string name, Version version, DateTime gameStartTime)
		{
			this._gameName = name;
			this._version = version;
			this._startTime = gameStartTime;
		}

		public override void ReportCrash(Exception e)
		{
			DateTime.UtcNow - this._startTime;
			Cursor.Show();
			EmailBugForm emailBugForm = new EmailBugForm();
			emailBugForm.CrashInfo = this.GetErrorString(e);
			if (emailBugForm.ShowDialog() == DialogResult.OK)
			{
				MailTools.SendDefaultMailClientEmail(new MailMessage
				{
					To = 
					{
						"bugs@digitaldnagames.com"
					},
					Subject = this._gameName + " Crash",
					Body = emailBugForm.CrashInfo.Replace("\n", Environment.NewLine),
					IsBodyHtml = false
				});
			}
			base.ReportCrash(e);
		}

		private string GetErrorString(Exception e)
		{
			TimeSpan timeSpan = DateTime.UtcNow - this._startTime;
			string text = "Crash Info:";
			string name = e.GetType().Name;
			object obj = text;
			text = string.Concat(new object[]
			{
				obj,
				this._gameName,
				", Version ",
				this._version,
				"\n"
			});
			string text2 = text;
			text = string.Concat(new string[]
			{
				text2,
				this._startTime.ToString("MM/dd/yy HH:mm"),
				" ",
				timeSpan.ToString(),
				"\n\n"
			});
			text = text + e.Message + "\n\n";
			text = text + e.GetType().Name + "\n\n";
			string[] array = e.StackTrace.Split(new char[]
			{
				'\n'
			});
			foreach (string text3 in array)
			{
				string str = text3.Trim();
				text = text + str + "\n";
			}
			return text;
		}
	}
}
