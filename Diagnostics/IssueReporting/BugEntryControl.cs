using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DNA.Diagnostics.IssueReporting
{
	public class BugEntryControl : UserControl
	{
		private IContainer components;

		private GroupBox groupBox1;

		private TextBox titleBox;

		private TextBox descriptionBox;

		private GroupBox groupBox2;

		public string BugTitle
		{
			get
			{
				return this.titleBox.Text;
			}
			set
			{
				this.titleBox.Text = value;
			}
		}

		public string BugDescription
		{
			get
			{
				return this.descriptionBox.Text;
			}
			set
			{
				this.descriptionBox.Text = value;
			}
		}

		public BugEntryControl()
		{
			this.InitializeComponent();
		}

		private void titleBox_Validating(object sender, CancelEventArgs e)
		{
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
			this.groupBox1 = new GroupBox();
			this.titleBox = new TextBox();
			this.descriptionBox = new TextBox();
			this.groupBox2 = new GroupBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			base.SuspendLayout();
			this.groupBox1.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.groupBox1.Controls.Add(this.titleBox);
			this.groupBox1.Location = new Point(3, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new Size(328, 47);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Title";
			this.titleBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.titleBox.Location = new Point(6, 19);
			this.titleBox.Name = "titleBox";
			this.titleBox.Size = new Size(316, 20);
			this.titleBox.TabIndex = 0;
			this.titleBox.Validating += this.titleBox_Validating;
			this.descriptionBox.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.descriptionBox.Location = new Point(6, 19);
			this.descriptionBox.Multiline = true;
			this.descriptionBox.Name = "descriptionBox";
			this.descriptionBox.Size = new Size(316, 120);
			this.descriptionBox.TabIndex = 0;
			this.groupBox2.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.groupBox2.Controls.Add(this.descriptionBox);
			this.groupBox2.Location = new Point(3, 56);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new Size(328, 145);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Description";
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.groupBox2);
			base.Controls.Add(this.groupBox1);
			base.Name = "BugEntryControl";
			base.Size = new Size(331, 204);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
