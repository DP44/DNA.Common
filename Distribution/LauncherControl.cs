using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DNA.Security;
using DNA.Security.Cryptography;
using DNA.Text;
using Microsoft.Win32;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Distribution
{
	public class LauncherControl : UserControl
	{
		public OnlineServices Services;

		private bool _trialMode = true;

		public bool HardwareInvalid;

		private readonly string _keyLoc = "Software\\DigitalDNAGames\\";

		private Uri landerURL;

		private bool docComplete;

		private IContainer components;

		private WebBrowser webBrowser1;

		private TextBox userNameBox;

		private TextBox passworrdBox;

		private CheckBox rememberPassBox;

		private Label label1;

		private Label label2;

		private Button optionsButton;

		private Button loginButton;

		private LinkLabel NeedAccountLabel;

		private Button facebookLoginButton;

		public event EventHandler<EventArgs> GameLaunched;

		public event EventHandler<EventArgs> OptionsClicked;

		public event EventHandler<EventArgs> FacebookLoginClicked;

		public event EventHandler<EventArgs> BeforeGameLaunch;

		public string Username
		{
			get
			{
				return this.userNameBox.Text;
			}
		}

		public string Password
		{
			get
			{
				return this.passworrdBox.Text;
			}
		}

		public LauncherControl()
		{
			this.InitializeComponent();
		}

		public bool TrialMode
		{
			get
			{
				return this._trialMode;
			}
		}

		private bool CheckHardware()
		{
			if (GraphicsAdapter.DefaultAdapter.IsProfileSupported(GraphicsProfile.HiDef) || GraphicsAdapter.DefaultAdapter.IsProfileSupported(GraphicsProfile.Reach))
			{
				return true;
			}
			MessageBox.Show(this, CommonResources.We_re_sorry_but_your_video_hardware_is_not_currently_supported__We_are_currently_working_on_supporting_more_hardware__and_we_will_have_a_solution_for_you_soon_, CommonResources.Unsupported_Hardware, MessageBoxButtons.OK);
			return false;
		}

		private void LoadSettings()
		{
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(this._keyLoc + this.Services.ProductID.ToString(), false);
			if (registryKey != null)
			{
				bool flag = bool.Parse((string)registryKey.GetValue("SavePassword", "false"));
				this.rememberPassBox.Checked = flag;
				string text = (string)registryKey.GetValue("Username", "");
				this.userNameBox.Text = text;
				if (flag)
				{
					try
					{
						string text2 = (string)registryKey.GetValue("Password", "");
						if (!string.IsNullOrEmpty(text2))
						{
							MD5HashProvider md5HashProvider = new MD5HashProvider();
							byte[] data = md5HashProvider.Compute(Encoding.UTF8.GetBytes(text + "DDNA12345" + this.Services.ProductID.ToString())).Data;
							string text3 = SecurityTools.DecryptString(data, TextConverter.FromBase32String(text2));
							this.passworrdBox.Text = text3;
						}
					}
					catch
					{
						this.passworrdBox.Text = "";
					}
				}
			}
		}

		private void SaveSettings()
		{
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(this._keyLoc + this.Services.ProductID.ToString(), true);
			if (registryKey == null)
			{
				registryKey = Registry.CurrentUser.CreateSubKey(this._keyLoc + this.Services.ProductID.ToString());
			}
			registryKey.SetValue("SavePassword", this.rememberPassBox.Checked);
			registryKey.SetValue("Username", this.userNameBox.Text);
			if (this.rememberPassBox.Checked)
			{
				MD5HashProvider md5HashProvider = new MD5HashProvider();
				byte[] data = md5HashProvider.Compute(Encoding.UTF8.GetBytes(this.userNameBox.Text + "DDNA12345" + this.Services.ProductID.ToString())).Data;
				byte[] bytes = SecurityTools.EncryptString(data, this.passworrdBox.Text);
				string value = TextConverter.ToBase32String(bytes);
				registryKey.SetValue("Password", value);
			}
			else
			{
				try
				{
					registryKey.DeleteValue("Password");
				}
				catch
				{
				}
			}
			registryKey.Close();
		}

		protected override void OnLoad(EventArgs e)
		{
			if (this.Services != null)
			{
				this.landerURL = new Uri(this.Services.GetLauncherPage());
				this.webBrowser1.Navigate(this.landerURL);
				this.webBrowser1.DocumentCompleted += this.webBrowser1_DocumentCompleted;
				this.webBrowser1.Navigating += this.webBrowser1_Navigating;
				this.LoadSettings();
			}
			base.OnLoad(e);
		}

		private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			if (this.landerURL == e.Url)
			{
				this.docComplete = true;
			}
		}

		private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
		{
			if (this.docComplete)
			{
				e.Cancel = true;
				Process.Start(e.Url.ToString());
			}
		}

		private void optionsButton_Click(object sender, EventArgs e)
		{
			if (this.OptionsClicked != null)
			{
				this.OptionsClicked(this, new EventArgs());
			}
		}

		public void RegisterFacebook(string facebookID, string accessToken, string email)
		{
			string text;
			this.Services.RegisterFacebook(facebookID, accessToken, email, this.Username, this.Password, out text);
		}

		public void ValidateLicenseFacebook(string facebookID, string accessToken, string email, string facebookUsername, Uri logoutURL, string facebookName)
		{
			Cursor.Current = Cursors.WaitCursor;
			string text;
			string a;
			if (this.Services.ValidateLicenseFacebook(facebookID, accessToken, out text, out a))
			{
				this._trialMode = false;
				if (this.CheckHardware())
				{
					if (this.BeforeGameLaunch != null)
					{
						this.BeforeGameLaunch(this, new EventArgs());
					}
					if (this.GameLaunched != null)
					{
						this.GameLaunched(this, new EventArgs());
					}
				}
				else
				{
					this.HardwareInvalid = true;
					base.ParentForm.Close();
				}
			}
			else if (a == "terms not accepted")
			{
				AcceptTOSForm acceptTOSForm = new AcceptTOSForm();
				DialogResult dialogResult = acceptTOSForm.ShowDialog();
				if (dialogResult == DialogResult.Yes)
				{
					this.Services.AcceptTermsFacebook(facebookID);
					this.ValidateLicenseFacebook(facebookID, accessToken, email, facebookUsername, logoutURL, facebookName);
				}
			}
			else if (a == "invalid license")
			{
				if (this.CheckHardware())
				{
					this._trialMode = true;
					if (this.BeforeGameLaunch != null)
					{
						this.BeforeGameLaunch(this, new EventArgs());
					}
					if (this.GameLaunched != null)
					{
						this.GameLaunched(this, new EventArgs());
					}
				}
				else
				{
					this.HardwareInvalid = true;
					base.ParentForm.Close();
				}
			}
			else if (a == "not registered")
			{
				FacebookRegisterForm facebookRegisterForm = new FacebookRegisterForm(facebookUsername, facebookID, accessToken, email, facebookName, logoutURL, this.Services);
				DialogResult dialogResult2 = facebookRegisterForm.ShowDialog(this);
				if (dialogResult2 == DialogResult.OK)
				{
					this.ValidateLicenseFacebook(facebookID, accessToken, email, facebookUsername, logoutURL, facebookName);
				}
			}
			else if (a == "invalid access token")
			{
				this.passworrdBox.Text = "";
				MessageBox.Show(CommonResources.LauncherControl_ValidateLicenseFacebook_There_was_a_problem_authenticating_your_facebook_account_, CommonResources.Error, MessageBoxButtons.OK);
			}
			else
			{
				this.passworrdBox.Text = "";
				MessageBox.Show(CommonResources.There_was_an_error_, CommonResources.Error, MessageBoxButtons.OK);
			}
			Cursor.Current = Cursors.Default;
		}

		private void loginButton_Click(object sender, EventArgs e)
		{
			Cursor.Current = Cursors.WaitCursor;
			this.userNameBox.Text = this.userNameBox.Text.Trim();
			string a;
			if (this.Services.ValidateLicense(this.userNameBox.Text, this.passworrdBox.Text, out a))
			{
				this.SaveSettings();
				if (this.CheckHardware())
				{
					this._trialMode = false;
					if (this.BeforeGameLaunch != null)
					{
						this.BeforeGameLaunch(this, new EventArgs());
					}
					if (this.GameLaunched != null)
					{
						this.GameLaunched(this, new EventArgs());
					}
				}
				else
				{
					this.HardwareInvalid = true;
					base.ParentForm.Close();
				}
			}
			else if (a == "terms not accepted")
			{
				AcceptTOSForm acceptTOSForm = new AcceptTOSForm();
				DialogResult dialogResult = acceptTOSForm.ShowDialog();
				if (dialogResult == DialogResult.Yes)
				{
					this.Services.AcceptTerms(this.userNameBox.Text, this.passworrdBox.Text);
					this.loginButton_Click(sender, e);
				}
			}
			else if (a == "invalid license")
			{
				this.SaveSettings();
				if (this.CheckHardware())
				{
					this._trialMode = true;
					if (this.BeforeGameLaunch != null)
					{
						this.BeforeGameLaunch(this, new EventArgs());
					}
					if (this.GameLaunched != null)
					{
						this.GameLaunched(this, new EventArgs());
					}
				}
				else
				{
					this.HardwareInvalid = true;
					base.ParentForm.Close();
				}
			}
			else if (a == "invalid user")
			{
				this.passworrdBox.Text = "";
				InvalidLoginForm invalidLoginForm = new InvalidLoginForm();
				DialogResult dialogResult2 = invalidLoginForm.ShowDialog(this);
				if (dialogResult2 == DialogResult.Yes)
				{
					Process.Start("https://www.digitaldnagames.com/Account/Register.aspx");
				}
			}
			else if (a == "not verified")
			{
				this.passworrdBox.Text = "";
				Process.Start("https://www.digitaldnagames.com/Account/VerifyEmail.aspx");
			}
			else
			{
				this.passworrdBox.Text = "";
				MessageBox.Show(CommonResources.There_was_an_error_, CommonResources.Error, MessageBoxButtons.OK);
			}
			Cursor.Current = Cursors.Default;
		}

		private void NeedAccountLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://www.digitaldnagames.com/Account/Register.aspx");
		}

		private void facebookLoginButton_Click(object sender, EventArgs e)
		{
			if (this.FacebookLoginClicked != null)
			{
				this.FacebookLoginClicked(this, new EventArgs());
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(LauncherControl));
			this.webBrowser1 = new WebBrowser();
			this.userNameBox = new TextBox();
			this.passworrdBox = new TextBox();
			this.rememberPassBox = new CheckBox();
			this.label1 = new Label();
			this.label2 = new Label();
			this.optionsButton = new Button();
			this.loginButton = new Button();
			this.NeedAccountLabel = new LinkLabel();
			this.facebookLoginButton = new Button();
			base.SuspendLayout();
			this.webBrowser1.AllowWebBrowserDrop = false;
			componentResourceManager.ApplyResources(this.webBrowser1, "webBrowser1");
			this.webBrowser1.IsWebBrowserContextMenuEnabled = false;
			this.webBrowser1.MinimumSize = new Size(20, 20);
			this.webBrowser1.Name = "webBrowser1";
			this.webBrowser1.Url = new Uri("", UriKind.Relative);
			componentResourceManager.ApplyResources(this.userNameBox, "userNameBox");
			this.userNameBox.Name = "userNameBox";
			componentResourceManager.ApplyResources(this.passworrdBox, "passworrdBox");
			this.passworrdBox.Name = "passworrdBox";
			componentResourceManager.ApplyResources(this.rememberPassBox, "rememberPassBox");
			this.rememberPassBox.Name = "rememberPassBox";
			this.rememberPassBox.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			componentResourceManager.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			componentResourceManager.ApplyResources(this.optionsButton, "optionsButton");
			this.optionsButton.Name = "optionsButton";
			this.optionsButton.UseVisualStyleBackColor = true;
			this.optionsButton.Click += this.optionsButton_Click;
			componentResourceManager.ApplyResources(this.loginButton, "loginButton");
			this.loginButton.Name = "loginButton";
			this.loginButton.UseVisualStyleBackColor = true;
			this.loginButton.Click += this.loginButton_Click;
			componentResourceManager.ApplyResources(this.NeedAccountLabel, "NeedAccountLabel");
			this.NeedAccountLabel.Name = "NeedAccountLabel";
			this.NeedAccountLabel.TabStop = true;
			this.NeedAccountLabel.LinkClicked += this.NeedAccountLabel_LinkClicked;
			componentResourceManager.ApplyResources(this.facebookLoginButton, "facebookLoginButton");
			this.facebookLoginButton.BackColor = Color.CornflowerBlue;
			this.facebookLoginButton.ForeColor = Color.White;
			this.facebookLoginButton.Image = CommonDialogResources.loginwithfacebook;
			this.facebookLoginButton.Name = "facebookLoginButton";
			this.facebookLoginButton.UseVisualStyleBackColor = false;
			this.facebookLoginButton.Click += this.facebookLoginButton_Click;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.facebookLoginButton);
			base.Controls.Add(this.NeedAccountLabel);
			base.Controls.Add(this.loginButton);
			base.Controls.Add(this.optionsButton);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.rememberPassBox);
			base.Controls.Add(this.passworrdBox);
			base.Controls.Add(this.userNameBox);
			base.Controls.Add(this.webBrowser1);
			base.Name = "LauncherControl";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
