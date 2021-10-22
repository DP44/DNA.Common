using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace DNA.Distribution
{
	public partial class LauncherForm : Form
	{
		public event EventHandler<EventArgs> OptionsClicked;

		public event EventHandler<EventArgs> FacebookClicked;

		public event EventHandler<EventArgs> BeforeGameLaunched;

		public string Username
		{
			get
			{
				return this.launcherControl1.Username;
			}
		}

		public string Password
		{
			get
			{
				return this.launcherControl1.Password;
			}
		}

		public bool TrialMode
		{
			get
			{
				return this.launcherControl1.TrialMode;
			}
		}

		public bool HardwareInvalid
		{
			get
			{
				return this.launcherControl1.HardwareInvalid;
			}
		}

		public LauncherForm(OnlineServices services)
		{
			this.InitializeComponent();
			this.launcherControl1.Services = services;
			this.Text = services.GetProductTitle() + " - " + Assembly.GetEntryAssembly().GetName().Version.ToString();
			base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		}

		private void launcherControl1_GameLaunched(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.OK;
			base.Close();
		}

		private void launcherControl1_OptionsClicked(object sender, EventArgs e)
		{
			if (this.OptionsClicked != null)
			{
				this.OptionsClicked(this, new EventArgs());
			}
		}

		private void launcherControl1_FacebookLoginClicked(object sender, EventArgs e)
		{
			if (this.FacebookClicked != null)
			{
				this.FacebookClicked(this, new EventArgs());
			}
		}

		private void launcherControl1_BeforeGameLaunch(object sender, EventArgs e)
		{
			if (this.BeforeGameLaunched != null)
			{
				this.BeforeGameLaunched(this, new EventArgs());
			}
		}

		public void ValidateLicenseFacebook(string facebookID, string email, string accessToken, string facebookUsername, Uri logoutURL, string facebookName)
		{
			this.launcherControl1.ValidateLicenseFacebook(facebookID, accessToken, email, facebookUsername, logoutURL, facebookName);
		}

		public void RegisterFacebook(string facebookID, string accessToken, string email)
		{
			this.launcherControl1.RegisterFacebook(facebookID, accessToken, email);
		}
	}
}
