namespace DNA.Distribution
{
	public partial class FacebookRegisterForm : global::System.Windows.Forms.Form
	{
		private global::System.ComponentModel.IContainer components;

		private global::System.Windows.Forms.PictureBox pictureBox1;

		private global::System.Windows.Forms.Label nameLabel;

		private global::System.Windows.Forms.LinkLabel logoutLink;

		private global::System.Windows.Forms.Button cancelButton;

		private global::System.Windows.Forms.Button okButton;

		private global::System.Windows.Forms.Label label1;

		private global::System.Windows.Forms.Label label2;

		private global::System.Windows.Forms.TextBox usernameTextBox;

		private global::System.Windows.Forms.TextBox PasswordTextBox;

		private global::System.Windows.Forms.Label label3;

		private global::System.Windows.Forms.Label label4;

		private global::System.Windows.Forms.LinkLabel registerLink;

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
			global::System.ComponentModel.ComponentResourceManager componentResourceManager = new global::System.ComponentModel.ComponentResourceManager(typeof(global::DNA.Distribution.FacebookRegisterForm));
			this.pictureBox1 = new global::System.Windows.Forms.PictureBox();
			this.nameLabel = new global::System.Windows.Forms.Label();
			this.logoutLink = new global::System.Windows.Forms.LinkLabel();
			this.cancelButton = new global::System.Windows.Forms.Button();
			this.okButton = new global::System.Windows.Forms.Button();
			this.label1 = new global::System.Windows.Forms.Label();
			this.label2 = new global::System.Windows.Forms.Label();
			this.usernameTextBox = new global::System.Windows.Forms.TextBox();
			this.PasswordTextBox = new global::System.Windows.Forms.TextBox();
			this.label3 = new global::System.Windows.Forms.Label();
			this.label4 = new global::System.Windows.Forms.Label();
			this.registerLink = new global::System.Windows.Forms.LinkLabel();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			componentResourceManager.ApplyResources(this.nameLabel, "nameLabel");
			this.nameLabel.Name = "nameLabel";
			componentResourceManager.ApplyResources(this.logoutLink, "logoutLink");
			this.logoutLink.Name = "logoutLink";
			this.logoutLink.TabStop = true;
			this.logoutLink.LinkClicked += new global::System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.logoutLink_LinkClicked);
			componentResourceManager.ApplyResources(this.cancelButton, "cancelButton");
			this.cancelButton.DialogResult = global::System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.okButton, "okButton");
			this.okButton.Name = "okButton";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new global::System.EventHandler(this.okButton_Click);
			componentResourceManager.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			componentResourceManager.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			componentResourceManager.ApplyResources(this.usernameTextBox, "usernameTextBox");
			this.usernameTextBox.Name = "usernameTextBox";
			this.usernameTextBox.TextChanged += new global::System.EventHandler(this.usernameTextBox_TextChanged);
			componentResourceManager.ApplyResources(this.PasswordTextBox, "PasswordTextBox");
			this.PasswordTextBox.Name = "PasswordTextBox";
			this.PasswordTextBox.UseSystemPasswordChar = true;
			componentResourceManager.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			componentResourceManager.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			componentResourceManager.ApplyResources(this.registerLink, "registerLink");
			this.registerLink.Name = "registerLink";
			this.registerLink.TabStop = true;
			this.registerLink.LinkClicked += new global::System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.registerLink_LinkClicked);
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.registerLink);
			base.Controls.Add(this.label4);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.PasswordTextBox);
			base.Controls.Add(this.usernameTextBox);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.okButton);
			base.Controls.Add(this.cancelButton);
			base.Controls.Add(this.logoutLink);
			base.Controls.Add(this.nameLabel);
			base.Controls.Add(this.pictureBox1);
			base.Name = "FacebookRegisterForm";
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
