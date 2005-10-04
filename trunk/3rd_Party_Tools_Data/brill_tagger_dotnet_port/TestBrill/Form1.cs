using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using BrillTagger;

namespace TestBrill
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnTag;
		private System.Windows.Forms.TextBox txtCorpus;
		private System.Windows.Forms.TextBox txtTagged;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnTag = new System.Windows.Forms.Button();
			this.txtCorpus = new System.Windows.Forms.TextBox();
			this.txtTagged = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// btnTag
			// 
			this.btnTag.Location = new System.Drawing.Point(216, 8);
			this.btnTag.Name = "btnTag";
			this.btnTag.TabIndex = 0;
			this.btnTag.Text = "Tag";
			this.btnTag.Click += new System.EventHandler(this.btnTag_Click);
			// 
			// txtCorpus
			// 
			this.txtCorpus.Location = new System.Drawing.Point(8, 8);
			this.txtCorpus.Name = "txtCorpus";
			this.txtCorpus.Size = new System.Drawing.Size(200, 20);
			this.txtCorpus.TabIndex = 1;
			this.txtCorpus.Text = "The quick brown fox jumped over the lazy dog ";
			// 
			// txtTagged
			// 
			this.txtTagged.Location = new System.Drawing.Point(8, 40);
			this.txtTagged.Multiline = true;
			this.txtTagged.Name = "txtTagged";
			this.txtTagged.ReadOnly = true;
			this.txtTagged.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtTagged.Size = new System.Drawing.Size(280, 104);
			this.txtTagged.TabIndex = 2;
			this.txtTagged.Text = "Enter text to tag above.  Ensure you end the corpus with a space.";
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 150);
			this.Controls.Add(this.txtTagged);
			this.Controls.Add(this.txtCorpus);
			this.Controls.Add(this.btnTag);
			this.Name = "Form1";
			this.Text = "Brill Tagger Test";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void btnTag_Click(object sender, System.EventArgs e)
		{
			txtTagged.Text = BrillTagger.BrillTagger.BrillTagged(txtCorpus.Text, true, true,true);
		}
	}
}
