using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DNA.Diagnostics.IssueReporting
{
	public partial class CustomerCrashReportForm : Form
	{
		public CustomerCrashReportForm(CrashID cid)
		{
			this.InitializeComponent();
			string text = "http:\\\\services.digitaldnagames.com\\Support.aspx";
			if (cid != null)
			{
				text = text + "?CrashID=" + cid.ToString();
			}
			this.webBrowser1.Navigate(text);
		}
	}
}
