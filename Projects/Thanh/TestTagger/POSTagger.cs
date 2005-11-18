using System;
using System.Collections;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TestTagger
{
	/// <summary>
	/// Summary description for POSTagger.
	/// </summary>
	/// 
	public delegate void FinishTaggingEventHandler(Object sender, TaggingEventArgs e);

	public class TaggingEventArgs: EventArgs
	{
		string _logger=string.Empty;
		ArrayList _pos=new ArrayList() ;
			
		private string RemoveBadChars(string s)
		{
			string[] badChars=new string[]{"`", "?","=",">",">","+",";",",","_","-","."} ;
			foreach(string ch in badChars)			
				s=s.Replace(ch, " ") ;

			return s;
		}

		public TaggingEventArgs(string logger)
		{
			_logger=logger;
			Regex r=new Regex("([ ])");
			String [] tokens=r.Split(_logger); 							

			foreach (string token in tokens)
				if (token.Trim() != string.Empty )
			{
					r=new Regex("([/])");
					string[] poses=r.Split(token) ;
					if (poses != null && poses.Length > 1)
						_pos.Add(new string[2] {RemoveBadChars(poses[0]), poses[2]}) ;
			}
		}

		public ArrayList GetPOS
		{
			get
			{
				return _pos;
			}
		}
	}

	public class POSTagger
	{
		public POSTagger()
		{
			//
			// TODO: Add constructor logic here
			//
		}

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

			toRun = "./tagger";

			//this.Cursor = Cursors.AppStarting;			
			this._output=string.Empty ;
			processCaller = new ProcessCaller(_invoker);
			processCaller.FileName = @"..\..\..\..\..\3rd_Party_Tools_Data\brill_tagger\Data\" + toRun;
			processCaller.WorkingDirectory = @"..\..\..\..\..\3rd_Party_Tools_Data\brill_tagger\Data\";
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
