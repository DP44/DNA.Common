namespace DNA.Distribution
{
	public partial class AcceptTOSForm : global::System.Windows.Forms.Form
	{
		private global::System.ComponentModel.IContainer components;

		private global::System.Windows.Forms.LinkLabel linkLabel1;

		private global::System.Windows.Forms.Button button1;

		private global::System.Windows.Forms.Button button2;

		private global::System.Windows.Forms.Label label1;

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
			global::System.ComponentModel.ComponentResourceManager componentResourceManager = new global::System.ComponentModel.ComponentResourceManager(typeof(global::DNA.Distribution.AcceptTOSForm));
			this.linkLabel1 = new global::System.Windows.Forms.LinkLabel();
			this.button1 = new global::System.Windows.Forms.Button();
			this.button2 = new global::System.Windows.Forms.Button();
			this.label1 = new global::System.Windows.Forms.Label();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.linkLabel1, "linkLabel1");
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.TabStop = true;
			this.linkLabel1.LinkClicked += new global::System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			componentResourceManager.ApplyResources(this.button1, "button1");
			this.button1.DialogResult = global::System.Windows.Forms.DialogResult.Yes;
			this.button1.Name = "button1";
			this.button1.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.button2, "button2");
			this.button2.DialogResult = global::System.Windows.Forms.DialogResult.Cancel;
			this.button2.Name = "button2";
			this.button2.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.linkLabel1);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.button2);
			base.Controls.Add(this.button1);
			base.Name = "AcceptTOSForm";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
