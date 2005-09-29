// TestTagger
// Author: Troy Simpson
// 29 September 2005
// demonstrates capturing stdout and stderr from a console application
// using the Brill Tagger

// alternative command line:
// lexicon.brown test.txt BIGRAMS lexicalrulefile.brown contextualrulefile.brown

//***************************
// tagged output is only output out of stdout.
// stderr is used as a status indicator in the brill tagger.
// Therefore, by capturing stdout you get a clean return value
// with no need for parsing.
//
// Always append a space or some other character if using a string
// instead of a filename in the commandline parameters; this allows
// for the taggers assumption of an extra character.
//***************************

// this code is based on Mike Mayer's code project article:
// http://www.codeproject.com/csharp/LaunchProcess.asp

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Data;
using System.IO;

namespace TestTagger
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.ComponentModel.Container components = null;

		internal System.Windows.Forms.TextBox txtOutput;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtParams;
		private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.Button btnCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
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
			this.txtOutput = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.txtParams = new System.Windows.Forms.TextBox();
			this.btnStart = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// txtOutput
			// 
			this.txtOutput.BackColor = System.Drawing.Color.Gainsboro;
			this.txtOutput.ForeColor = System.Drawing.Color.Black;
			this.txtOutput.Location = new System.Drawing.Point(0, 56);
			this.txtOutput.Multiline = true;
			this.txtOutput.Name = "txtOutput";
			this.txtOutput.ReadOnly = true;
			this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtOutput.Size = new System.Drawing.Size(504, 64);
			this.txtOutput.TabIndex = 19;
			this.txtOutput.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(0, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(152, 23);
			this.label1.TabIndex = 20;
			this.label1.Text = "Commandline Parameters:";
			// 
			// txtParams
			// 
			this.txtParams.Location = new System.Drawing.Point(128, 8);
			this.txtParams.Name = "txtParams";
			this.txtParams.Size = new System.Drawing.Size(376, 20);
			this.txtParams.TabIndex = 21;
			this.txtParams.Text = "lexicon.brown \"this is some test text \" BIGRAMS lexicalrulefile.brown contextualr" +
				"ulefile.brown";
			// 
			// btnStart
			// 
			this.btnStart.Location = new System.Drawing.Point(0, 32);
			this.btnStart.Name = "btnStart";
			this.btnStart.TabIndex = 22;
			this.btnStart.Text = "Start";
			this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Enabled = false;
			this.btnCancel.Location = new System.Drawing.Point(80, 32);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 23;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(512, 118);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnStart);
			this.Controls.Add(this.txtParams);
			this.Controls.Add(this.txtOutput);
			this.Controls.Add(this.label1);
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

		private ProcessCaller processCaller;

		private void btnStart_Click(object sender, System.EventArgs e)
		{
			btnCancel.Enabled = true;
            begin_Tag();
		}

		private void begin_Tag()
		{
			string toRun;

			toRun = "./tagger";

			this.Cursor = Cursors.AppStarting;
			this.btnStart.Enabled = false;  

			processCaller = new ProcessCaller(this);
			processCaller.FileName = @"..\..\..\Data\" + toRun;
			processCaller.WorkingDirectory = @"..\..\..\Data\";
			processCaller.Arguments = txtParams.Text;
			processCaller.StdErrReceived += new DataReceivedHandler(writeStreamInfo);
			processCaller.StdOutReceived += new DataReceivedHandler(writeStreamInfo);
			processCaller.Completed += new EventHandler(processCompletedOrCanceled);
			processCaller.Cancelled += new EventHandler(processCompletedOrCanceled);
            
			this.txtOutput.Text = "Started function.  Please stand by.." + Environment.NewLine;

			// the following function starts a process and returns immediately,
			// thus allowing the form to stay responsive.
			processCaller.Start();  
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			if (processCaller != null)
			{
				processCaller.Cancel();
			}
		}

		/// <summary>
		/// Handles the events of StdErrReceived and StdOutReceived.
		/// </summary>
		/// <remarks>
		/// If stderr were handled in a separate function, it could possibly
		/// be displayed in red in the richText box, but that is beyond 
		/// the scope of this demo.
		/// </remarks>
		private void writeStreamInfo(object sender, DataReceivedEventArgs e)
		{
			this.txtOutput.AppendText(e.Text + Environment.NewLine);
		}

		/// <summary>
		/// Handles the events of processCompleted & processCanceled
		/// </summary>
		private void processCompletedOrCanceled(object sender, EventArgs e)
		{
			this.Cursor = Cursors.Default;
			this.btnStart.Enabled = true;
			this.btnCancel.Enabled = false;
		}

	}
}
