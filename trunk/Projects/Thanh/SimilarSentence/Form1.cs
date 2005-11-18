using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using TestTagger;
using WnLexicon;
using Wnlib;
using WordsMatching;

namespace SimilarSentence
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : Form
	{
		private Button btnStart;
		private TextBox txtSentence;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;
		private System.Windows.Forms.TextBox txtPOSTagged;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.TextBox txtWSD;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader Sentence;
		private System.Windows.Forms.ColumnHeader ScoreOfMutualInfo;
		private System.Windows.Forms.ColumnHeader SimilarityScore;
		private System.Windows.Forms.TextBox txtSentence2;
		string[] sentences;
		string[] wsdText;
		string[] posText;
		int index=-1;
		private System.Windows.Forms.TextBox txtTaggedPOS2;
		private System.Windows.Forms.TextBox txtWSD2;
		private POSTagger POS=new POSTagger() ;		

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			Wnlib.WNCommon.path = "C:\\Program Files\\WordNet\\2.1\\dict\\";
			POS.FinishTagging += 
				new FinishTaggingEventHandler (FinishTagging);
			sentences=new string[2] ;
			wsdText=new string[2] ;
			posText=new string[2] ;
			StopWordsHandler stop=new StopWordsHandler() ;
			
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
			System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("test");
			this.txtSentence = new System.Windows.Forms.TextBox();
			this.btnStart = new System.Windows.Forms.Button();
			this.txtPOSTagged = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.txtWSD = new System.Windows.Forms.TextBox();
			this.listView1 = new System.Windows.Forms.ListView();
			this.Sentence = new System.Windows.Forms.ColumnHeader();
			this.ScoreOfMutualInfo = new System.Windows.Forms.ColumnHeader();
			this.SimilarityScore = new System.Windows.Forms.ColumnHeader();
			this.txtSentence2 = new System.Windows.Forms.TextBox();
			this.txtTaggedPOS2 = new System.Windows.Forms.TextBox();
			this.txtWSD2 = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// txtSentence
			// 
			this.txtSentence.Location = new System.Drawing.Point(8, 32);
			this.txtSentence.Name = "txtSentence";
			this.txtSentence.Size = new System.Drawing.Size(480, 20);
			this.txtSentence.TabIndex = 0;
			this.txtSentence.Text = "He eat a banana";
			// 
			// btnStart
			// 
			this.btnStart.Location = new System.Drawing.Point(528, 32);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(56, 23);
			this.btnStart.TabIndex = 2;
			this.btnStart.Text = "Make!";
			this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
			// 
			// txtPOSTagged
			// 
			this.txtPOSTagged.BackColor = System.Drawing.Color.LightSalmon;
			this.txtPOSTagged.Location = new System.Drawing.Point(8, 56);
			this.txtPOSTagged.Name = "txtPOSTagged";
			this.txtPOSTagged.ReadOnly = true;
			this.txtPOSTagged.Size = new System.Drawing.Size(480, 20);
			this.txtPOSTagged.TabIndex = 3;
			this.txtPOSTagged.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(120, 17);
			this.label1.TabIndex = 4;
			this.label1.Text = "The original sentence";
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(528, 64);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(56, 20);
			this.textBox1.TabIndex = 5;
			this.textBox1.Text = "textBox1";
			// 
			// txtWSD
			// 
			this.txtWSD.Location = new System.Drawing.Point(8, 80);
			this.txtWSD.Name = "txtWSD";
			this.txtWSD.Size = new System.Drawing.Size(480, 20);
			this.txtWSD.TabIndex = 6;
			this.txtWSD.Text = "textBox2";
			// 
			// listView1
			// 
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this.Sentence,
																						this.ScoreOfMutualInfo,
																						this.SimilarityScore});
			this.listView1.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
																					  listViewItem1});
			this.listView1.Location = new System.Drawing.Point(8, 224);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(584, 248);
			this.listView1.TabIndex = 7;
			this.listView1.View = System.Windows.Forms.View.Details;
			// 
			// Sentence
			// 
			this.Sentence.Width = 1000;
			// 
			// txtSentence2
			// 
			this.txtSentence2.Location = new System.Drawing.Point(8, 112);
			this.txtSentence2.Name = "txtSentence2";
			this.txtSentence2.Size = new System.Drawing.Size(480, 20);
			this.txtSentence2.TabIndex = 8;
			this.txtSentence2.Text = "He taste a banana";
			// 
			// txtTaggedPOS2
			// 
			this.txtTaggedPOS2.BackColor = System.Drawing.Color.LightSalmon;
			this.txtTaggedPOS2.Location = new System.Drawing.Point(8, 136);
			this.txtTaggedPOS2.Name = "txtTaggedPOS2";
			this.txtTaggedPOS2.ReadOnly = true;
			this.txtTaggedPOS2.Size = new System.Drawing.Size(480, 20);
			this.txtTaggedPOS2.TabIndex = 9;
			this.txtTaggedPOS2.Text = "";
			// 
			// txtWSD2
			// 
			this.txtWSD2.Location = new System.Drawing.Point(8, 160);
			this.txtWSD2.Name = "txtWSD2";
			this.txtWSD2.Size = new System.Drawing.Size(480, 20);
			this.txtWSD2.TabIndex = 10;
			this.txtWSD2.Text = "textBox2";
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(664, 478);
			this.Controls.Add(this.txtWSD2);
			this.Controls.Add(this.txtTaggedPOS2);
			this.Controls.Add(this.txtSentence2);
			this.Controls.Add(this.listView1);
			this.Controls.Add(this.txtWSD);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.txtPOSTagged);
			this.Controls.Add(this.txtSentence);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnStart);
			this.Name = "Form1";
			this.Text = "SimilarSentence:";
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

		private ArrayList[] _pos=new ArrayList[2] ;
		private MyPos[][] _myPos=new MyPos[2][] ;

		private void FinishTagging(Object sender, TaggingEventArgs e)
		{

			ArrayList taggedPos=e.GetPOS ;					
			if (taggedPos.Count > 0)
			{
				++index;
				string taggedText=string.Empty;								
				originSentence=string.Empty;
				ArrayList tmp=new ArrayList() ;
				int[] dx=new int[taggedPos.Count] ;
				for (int i=0; i < taggedPos.Count; i++)					
					if (dx[i] == 0)
				{
					string[] pos=(string[]) taggedPos[i];
					
					PartsOfSpeech partsOfSpeech=PartsOfSpeech.Unknown ;
					
					if (pos[1].StartsWith("N") )
						partsOfSpeech =PartsOfSpeech.Noun;
					if (pos[1].StartsWith("J") )
						partsOfSpeech =PartsOfSpeech.Adj;
					if (pos[1].StartsWith("RB") )
						partsOfSpeech =PartsOfSpeech.Adv;
					if (pos[1].StartsWith("V") )
						partsOfSpeech =PartsOfSpeech.Verb;
							
					//Phrasal verb 
					if (i < taggedPos.Count - 1 && pos[1].StartsWith("V"))
					{						
						string[] posnext=(string[]) taggedPos[i + 1];
						if (posnext[1].StartsWith("IN") )
						{
							
							string phrasal=pos[0] + "_" + posnext[0];
							if ( SimilarGenerator.GetSynsetIndex(phrasal, partsOfSpeech) != -1)
							{
								pos[0]=phrasal;	
								dx[i + 1]=1;
							}
														
						}
					}

					taggedText=taggedText + pos[0] + "/" + pos[1] + " ";	
					if (!StopWordsHandler.IsStopWord(pos[0].ToLower() ))				
					{						
						originSentence=originSentence + " {" + (tmp.Count) + "}";					
						tmp.Add(new MyPos(pos[0].Trim() , partsOfSpeech))  ;					
										
					}
					else
					{
						originSentence=originSentence + " " + pos[0] ;
					}									
				}

				_myPos[index]=(MyPos[])tmp.ToArray( typeof(MyPos) );
				
				//this.txtPOSTagged.Text=taggedText;				
				posText[index]=taggedText;
				Do_Disambiguating ();

				taggedText=String.Empty ;
				for (int i=0; i < _myPos[index].Length; i++)
				{															
					taggedText=taggedText + _myPos[index][i].Word + "/" + _myPos[index][i].Sense + " ";					
				
				}
				wsdText[index]=taggedText;
				//txtWSD.Text=taggedText;

				SimilarGenerator sim=new SimilarGenerator(_myPos[index], originSentence) ;
				ArrayList result=sim.GetResult ;
				foreach (string sentence in result)
				{
					listView1.Items.Add(sentence) ;
				}				

				if (index == 1)
				{
					txtPOSTagged.Text=posText [0];
					txtTaggedPOS2.Text=posText [1];
					txtWSD.Text =wsdText [0];
					txtWSD2.Text =wsdText [1];

				}
			}
			
			this.Cursor=Cursors.Default ;
		}

			
		private void Do_POSTagging()
		{
			this.Cursor=Cursors.AppStarting;
			//_pos=new ArrayList() ;
			sentences[0]=txtSentence.Text;
			sentences[1]=txtSentence2.Text;			
			POS.DoTagging(sentences[0], this) ;		
			//do{}while (index <= 0) ;

			//POS.DoTagging(sentences[1], this) ;		
		}
		

		private string originSentence;
		private void Do_Disambiguating()
		{			
			POSTaggerWSD wsd=new POSTaggerWSD();		
			_myPos[index]=wsd.Disambiguate(_myPos[index]);			
		}
		
	
		private void btnStart_Click(object sender, EventArgs e)
		{
			Do_POSTagging ();			
		}
	}
}
