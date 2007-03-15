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
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox cboLexicon;
		private System.Windows.Forms.ComboBox cboLexicalRuleFile;
		private System.Windows.Forms.ComboBox cboContextualRuleFile;
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
			this.label1 = new System.Windows.Forms.Label();
			this.cboLexicon = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.cboLexicalRuleFile = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.cboContextualRuleFile = new System.Windows.Forms.ComboBox();
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
			this.txtTagged.Location = new System.Drawing.Point(8, 136);
			this.txtTagged.Multiline = true;
			this.txtTagged.Name = "txtTagged";
			this.txtTagged.ReadOnly = true;
			this.txtTagged.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtTagged.Size = new System.Drawing.Size(280, 104);
			this.txtTagged.TabIndex = 2;
			this.txtTagged.Text = "Enter text to tag above.  Ensure you end the corpus with a space.";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 40);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 23);
			this.label1.TabIndex = 3;
			this.label1.Text = "Lexicon:";
			// 
			// cboLexicon
			// 
			this.cboLexicon.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboLexicon.Items.AddRange(new object[] {
															"LEXICON.BROWN.AND.WSJ",
															"LEXICON.BROWN",
															"LEXICON.WSJ"});
			this.cboLexicon.Location = new System.Drawing.Point(112, 40);
			this.cboLexicon.Name = "cboLexicon";
			this.cboLexicon.Size = new System.Drawing.Size(184, 21);
			this.cboLexicon.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 72);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(104, 23);
			this.label2.TabIndex = 5;
			this.label2.Text = "Lexical Rule File:";
			// 
			// cboLexicalRuleFile
			// 
			this.cboLexicalRuleFile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboLexicalRuleFile.Items.AddRange(new object[] {
																	"LEXICALRULEFILE.BROWN",
																	"LEXICALRULEFILE.WSJ"});
			this.cboLexicalRuleFile.Location = new System.Drawing.Point(112, 72);
			this.cboLexicalRuleFile.Name = "cboLexicalRuleFile";
			this.cboLexicalRuleFile.Size = new System.Drawing.Size(184, 21);
			this.cboLexicalRuleFile.TabIndex = 6;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 104);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(112, 23);
			this.label3.TabIndex = 7;
			this.label3.Text = "Contextual Rule File:";
			// 
			// cboContextualRuleFile
			// 
			this.cboContextualRuleFile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboContextualRuleFile.Items.AddRange(new object[] {
																	   "CONTEXTUALRULEFILE.BROWN",
																	   "CONTEXTUALRULEFILE.WSJ",
																	   "CONTEXTUALRULEFILE.WSJ.NOLEX"});
			this.cboContextualRuleFile.Location = new System.Drawing.Point(112, 104);
			this.cboContextualRuleFile.Name = "cboContextualRuleFile";
			this.cboContextualRuleFile.Size = new System.Drawing.Size(184, 21);
			this.cboContextualRuleFile.TabIndex = 8;
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(298, 248);
			this.Controls.Add(this.cboContextualRuleFile);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.cboLexicalRuleFile);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.cboLexicon);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtTagged);
			this.Controls.Add(this.txtCorpus);
			this.Controls.Add(this.btnTag);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
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
			txtTagged.Text = "";
			if(cboLexicalRuleFile.SelectedIndex > -1)
				txtTagged.Text = BrillTagger.BrillTagger.BrillTagged(txtCorpus.Text, cboLexicon.SelectedIndex > -1, cboContextualRuleFile.SelectedIndex > -1, true, cboLexicon.SelectedItem.ToString(), cboLexicalRuleFile.SelectedItem.ToString(), cboContextualRuleFile.SelectedItem.ToString());
			else
				MessageBox.Show("You must select a Lexical Rule File.");
		}
	}
}
