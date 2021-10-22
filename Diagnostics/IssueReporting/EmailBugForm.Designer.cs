namespace DNA.Diagnostics.IssueReporting
{
	public partial class EmailBugForm : global::System.Windows.Forms.Form
	{
		private global::System.ComponentModel.IContainer components;

		private global::System.Windows.Forms.Label label1;

		private global::System.Windows.Forms.TextBox crashDumpInfoBox;

		private global::System.Windows.Forms.Button emailButton;

		private global::System.Windows.Forms.Button closeButton;

		private global::System.Windows.Forms.Button copyToClipboardButton;

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
			this.label1 = new global::System.Windows.Forms.Label();
			this.crashDumpInfoBox = new global::System.Windows.Forms.TextBox();
			this.emailButton = new global::System.Windows.Forms.Button();
			this.closeButton = new global::System.Windows.Forms.Button();
			this.copyToClipboardButton = new global::System.Windows.Forms.Button();
			base.SuspendLayout();
			this.label1.AutoSize = true;
			this.label1.Location = new global::System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new global::System.Drawing.Size(358, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Help us by emailing us this info and we will do our best to fix this problem....";
			this.crashDumpInfoBox.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Bottom | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.crashDumpInfoBox.Location = new global::System.Drawing.Point(15, 25);
			this.crashDumpInfoBox.Multiline = true;
			this.crashDumpInfoBox.Name = "crashDumpInfoBox";
			this.crashDumpInfoBox.ReadOnly = true;
			this.crashDumpInfoBox.ScrollBars = global::System.Windows.Forms.ScrollBars.Vertical;
			this.crashDumpInfoBox.Size = new global::System.Drawing.Size(551, 301);
			this.crashDumpInfoBox.TabIndex = 1;
			this.emailButton.Anchor = (global::System.Windows.Forms.AnchorStyles.Bottom | global::System.Windows.Forms.AnchorStyles.Left);
			this.emailButton.DialogResult = global::System.Windows.Forms.DialogResult.OK;
			this.emailButton.Location = new global::System.Drawing.Point(12, 332);
			this.emailButton.Name = "emailButton";
			this.emailButton.Size = new global::System.Drawing.Size(134, 23);
			this.emailButton.TabIndex = 2;
			this.emailButton.Text = "Email us this info";
			this.emailButton.UseVisualStyleBackColor = true;
			this.closeButton.Anchor = (global::System.Windows.Forms.AnchorStyles.Bottom | global::System.Windows.Forms.AnchorStyles.Right);
			this.closeButton.DialogResult = global::System.Windows.Forms.DialogResult.Cancel;
			this.closeButton.Location = new global::System.Drawing.Point(491, 332);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new global::System.Drawing.Size(75, 23);
			this.closeButton.TabIndex = 3;
			this.closeButton.Text = "Close";
			this.closeButton.UseVisualStyleBackColor = true;
			this.copyToClipboardButton.Anchor = global::System.Windows.Forms.AnchorStyles.Bottom;
			this.copyToClipboardButton.Location = new global::System.Drawing.Point(239, 332);
			this.copyToClipboardButton.Name = "copyToClipboardButton";
			this.copyToClipboardButton.Size = new global::System.Drawing.Size(131, 23);
			this.copyToClipboardButton.TabIndex = 4;
			this.copyToClipboardButton.Text = "Copy To Clipboard";
			this.copyToClipboardButton.UseVisualStyleBackColor = true;
			this.copyToClipboardButton.Click += new global::System.EventHandler(this.copyToClipboardButton_Click);
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new global::System.Drawing.Size(578, 367);
			base.Controls.Add(this.copyToClipboardButton);
			base.Controls.Add(this.closeButton);
			base.Controls.Add(this.emailButton);
			base.Controls.Add(this.crashDumpInfoBox);
			base.Controls.Add(this.label1);
			base.Name = "EmailBugForm";
			this.Text = "Sorry the game has crashed....";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
