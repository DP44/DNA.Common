using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace DNA.Distribution
{
	public partial class InvalidLoginForm : Form
	{
		public InvalidLoginForm()
		{
			this.InitializeComponent();
		}

		private void forgotpasswordlink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("http://www.digitaldnagames.com/ResetPassword.aspx");
			base.Close();
		}
	}
}
