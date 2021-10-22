namespace DNA.Diagnostics.IssueReporting
{
	public partial class CustomerCrashReportForm : global::System.Windows.Forms.Form
	{
		private global::System.ComponentModel.IContainer components;

		private global::System.Windows.Forms.WebBrowser webBrowser1;

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
			global::System.ComponentModel.ComponentResourceManager componentResourceManager = new global::System.ComponentModel.ComponentResourceManager(typeof(global::DNA.Diagnostics.IssueReporting.CustomerCrashReportForm));
			this.webBrowser1 = new global::System.Windows.Forms.WebBrowser();
			base.SuspendLayout();
			this.webBrowser1.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Bottom | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.webBrowser1.Location = new global::System.Drawing.Point(12, 12);
			this.webBrowser1.MinimumSize = new global::System.Drawing.Size(20, 20);
			this.webBrowser1.Name = "webBrowser1";
			this.webBrowser1.Size = new global::System.Drawing.Size(760, 540);
			this.webBrowser1.TabIndex = 5;
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new global::System.Drawing.Size(784, 564);
			base.Controls.Add(this.webBrowser1);
			base.Icon = (global::System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			this.MinimumSize = new global::System.Drawing.Size(506, 266);
			base.Name = "CustomerCrashReportForm";
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Error Information";
			base.ResumeLayout(false);
		}
	}
}
