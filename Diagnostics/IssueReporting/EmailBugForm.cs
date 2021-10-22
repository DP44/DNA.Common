using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DNA.Diagnostics.IssueReporting
{
	public partial class EmailBugForm : Form
	{
		private string _crashInfo;

		public string CrashInfo
		{
			get
			{
				return this._crashInfo;
			}
			set
			{
				this._crashInfo = value;
				this.crashDumpInfoBox.Text = this._crashInfo.Replace("\n", "\r\n");
			}
		}

		public EmailBugForm()
		{
			this.InitializeComponent();
		}

		private void copyToClipboardButton_Click(object sender, EventArgs e)
		{
			Clipboard.SetText(this.CrashInfo);
		}
	}
}
