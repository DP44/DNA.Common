using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace DNA.Distribution
{
	public partial class AcceptTOSForm : Form
	{
		public AcceptTOSForm()
		{
			this.InitializeComponent();
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("http://www.digitaldnagames.com/Terms.aspx");
		}
	}
}
