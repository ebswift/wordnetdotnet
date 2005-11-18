using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace TestTagger
{
	/// <summary>
	/// Summary description for Tagger.
	/// </summary>
	public class Tagger : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Tagger()
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
				if(components != null)
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
			// 
			// Tagger
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Name = "Tagger";
			this.Text = "Tagger";
			//this.Load += new System.EventHandler(this.Tagger_Load);

		}
		#endregion

		private ProcessCaller processCaller;
		private ISynchronizeInvoke _invoker;
		private string _param=string.Empty ;
		public string _output=string.Empty;

		public event FinishTaggingEventHandler FinishTagging;

		protected virtual void OnFinishTagging(TaggingEventArgs e)
		{
			FinishTagging(this, e);
		}


		public void DoTagging(string sentence, ISynchronizeInvoke invoker)
		{			
			this._invoker=invoker;
			_param=string.Format("lexicon.brown \"{0} \" BIGRAMS lexicalrule" +
				"file.brown contextualrulefile.brown", sentence) ;

			begin_Tag();
		}

		private void begin_Tag()
		{
			string toRun;

			toRun = "tagger";

			//this.Cursor = Cursors.AppStarting;			
			this._output=string.Empty ;
			processCaller = new ProcessCaller(_invoker);
			processCaller.FileName = @"..\..\..\..\..\3rd_Party_Tools_Data\brill_tagger\TestTagger\bin\Debug\" + toRun;
			processCaller.WorkingDirectory = @"..\..\..\..\..\3rd_Party_Tools_Data\brill_tagger\TestTagger\bin\Debug\";
			processCaller.Arguments = _param;
			processCaller.StdErrReceived += new DataReceivedHandler(writeStreamInfo);
			processCaller.StdOutReceived += new DataReceivedHandler(writeOut);
			processCaller.Completed += new EventHandler(processCompleted);
			processCaller.Cancelled += new EventHandler(processCanceled);
            			
			// the following function starts a process and returns immediately,
			// thus allowing the form to stay responsive.
			processCaller.Start();  

		}

		private void processCanceled(object sendr, EventArgs e)
		{
			//MessageBox.Show("err") ;
		}

		private void processCompleted(object sendr, EventArgs e)
		{
			
			//MessageBox.Show(_output + "  " + _param) ;
		}


		private void writeOut(object sender, DataReceivedEventArgs e)
		{
			if (_output == string.Empty)
			{				
				_output=e.Text ;							
				OnFinishTagging(new TaggingEventArgs(_output) );		
			}			
		}


		private void writeStreamInfo(object sender, DataReceivedEventArgs e)
		{


			//canceling 
			//			if (processCaller != null)
			//			{
			//				processCaller.Cancel();				
			//			}
			
		}

		//		private void writeStreamInfo(object sender, DataReceivedEventArgs e)
		//		{
		//			this.txtOutput.AppendText(e.Text + Environment.NewLine);
		//		}

	}
}
