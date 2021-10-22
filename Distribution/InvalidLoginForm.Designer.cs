namespace DNA.Distribution
{
	public partial class InvalidLoginForm : global::System.Windows.Forms.Form
	{
		private global::System.ComponentModel.IContainer components;

		private global::System.Windows.Forms.Label label1;

		private global::System.Windows.Forms.Label label2;

		private global::System.Windows.Forms.Button noButton;

		private global::System.Windows.Forms.Button yesButton;

		private global::System.Windows.Forms.LinkLabel forgotpasswordlink;

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
			global::System.ComponentModel.ComponentResourceManager componentResourceManager = new global::System.ComponentModel.ComponentResourceManager(typeof(global::DNA.Distribution.InvalidLoginForm));
			this.label1 = new global::System.Windows.Forms.Label();
			this.label2 = new global::System.Windows.Forms.Label();
			this.noButton = new global::System.Windows.Forms.Button();
			this.yesButton = new global::System.Windows.Forms.Button();
			this.forgotpasswordlink = new global::System.Windows.Forms.LinkLabel();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			componentResourceManager.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			componentResourceManager.ApplyResources(this.noButton, "noButton");
			this.noButton.DialogResult = global::System.Windows.Forms.DialogResult.No;
			this.noButton.Name = "noButton";
			this.noButton.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.yesButton, "yesButton");
			this.yesButton.DialogResult = global::System.Windows.Forms.DialogResult.Yes;
			this.yesButton.Name = "yesButton";
			this.yesButton.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.forgotpasswordlink, "forgotpasswordlink");
			this.forgotpasswordlink.Name = "forgotpasswordlink";
			this.forgotpasswordlink.TabStop = true;
			this.forgotpasswordlink.LinkClicked += new global::System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.forgotpasswordlink_LinkClicked);
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.forgotpasswordlink);
			base.Controls.Add(this.yesButton);
			base.Controls.Add(this.noButton);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Name = "InvalidLoginForm";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
