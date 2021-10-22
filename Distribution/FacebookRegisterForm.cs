using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace DNA.Distribution
{
	public partial class FacebookRegisterForm : Form
	{
		private Uri _logoutURL;

		private OnlineServices _services;

		private string _fbID;

		private string _accessToken;

		private string _email;

		public string username
		{
			get
			{
				return this.usernameTextBox.Text.Trim();
			}
		}

		public string password
		{
			get
			{
				return this.PasswordTextBox.Text.Trim();
			}
		}

		public FacebookRegisterForm(string fbusername, string fbID, string accessToken, string email, string name, Uri logoutURL, OnlineServices services)
		{
			this.InitializeComponent();
			this.nameLabel.Text = name;
			this._logoutURL = logoutURL;
			this._fbID = fbID;
			this._accessToken = accessToken;
			this._email = email;
			this._services = services;
			this.pictureBox1.Image = Image.FromStream(new MemoryStream(new WebClient().DownloadData("http://graph.facebook.com/" + fbusername + "/picture")));
		}

		private void logoutLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			WebBrowser webBrowser = new WebBrowser();
			webBrowser.Navigate(this._logoutURL);
			base.DialogResult = DialogResult.Abort;
			base.Close();
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			string a;
			if (this._services.RegisterFacebook(this._fbID, this._accessToken, this._email, this.username, this.password, out a))
			{
				base.DialogResult = DialogResult.OK;
				base.Close();
				return;
			}
			if (a == "invalid user")
			{
				this.PasswordTextBox.Text = "";
				MessageBox.Show(this, CommonResources.Invalid_username_or_password_, CommonResources.Invalid_Login, MessageBoxButtons.OK);
				return;
			}
			this.PasswordTextBox.Text = "";
			MessageBox.Show(this, CommonResources.There_was_an_error_, CommonResources.Error, MessageBoxButtons.OK);
		}

		private void registerLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://www.digitaldnagames.com/Account/Register.aspx");
			base.DialogResult = DialogResult.Ignore;
			base.Close();
		}

		private void usernameTextBox_TextChanged(object sender, EventArgs e)
		{
		}
	}
}
