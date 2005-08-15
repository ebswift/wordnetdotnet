// WordNet.Net library tester - copyright (c) Richard Northedge, Troy simpson 2005
// For terms of use, contact me - Troy Simpson (troy at ebswift.com)
// and I will put you in touch with Richard.

// This code will test WordNet against itself - it will read every
// single word from a part of speech index and call its internal search
// methods to see if a result is returned.
// If a result is not returned then this indicates a bug that needs
// to be traced.

// I (Troy Simpson) heavily modified Richards code to allow a 
// binary only search, or a full overview search against each
// word in a given part of speech index.
// At the moment this code is really, really rough, but it works.
// Overview search test is really slow, so the best bet is to just
// set a breakpoint 
using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using WordNetClasses;
using Wnlib;

namespace BinSearchTest
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button1;
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
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(112, 88);
			this.button1.Name = "button1";
			this.button1.TabIndex = 0;
			this.button1.Text = "Test Now";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Controls.Add(this.button1);
			this.Name = "Form1";
			this.Text = "Form1";
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

		private void button1_Click(object sender, System.EventArgs e)
		{
			// change "noun" to the part of speech that you want:
			// noun adj adv verb
			// un-comment the type of search you want to do -
			// only the binary search or a full overview search

			//testbinsearch("noun"); // use to test the pure binary search method
			testoverview("noun"); // send the word through the entire pipeline
		}

		private void testoverview(string pos)
		{
			SearchSet bobj = null; // = new SearchSet();
            WordNetClasses.WN wnc = new WordNetClasses.WN(@"C:\Program Files\WordNet\2.1\dict\");
            bool b = false; // sets word found - true/false
			ArrayList list;
			System.IO.StreamReader indexFile = new System.IO.StreamReader(@"C:\Program Files\WordNet\2.1\dict\index." + pos);
			//StreamWriter foundNouns = new StreamWriter(@"C:\found " + pos + ".txt");
			StreamWriter lostNouns = new StreamWriter(@"C:\lost " + pos + ".txt");

			string currentLine = indexFile.ReadLine();
			string indexLine = string.Empty;

			while (currentLine != null)
			{
				string currentWord = currentLine.Substring(0, currentLine.IndexOf(' '));

				// this transformation doesn't reflect the library's
				// true transformations - it transforms internally
				// in index lookup
				//currentWord = currentWord.Replace("-"," ").Replace("_"," ");
				//currentWord = currentWord.Replace("_"," ");

				if (currentWord.Length > 0)
				{
	                list = new ArrayList();
		            wnc.OverviewFor(currentWord, pos, ref b, ref bobj, list);

					if (!b)
					{
						// set a breakpoint here to single-step and find
						// out why a word isn't picked up from the index
						wnc.OverviewFor(currentWord, pos, ref b, ref bobj, list);
						lostNouns.WriteLine(currentWord);
					}
					else
					{
						//foundNouns.WriteLine(currentWord);
					}
				}

				currentLine = indexFile.ReadLine();
			}

			//foundNouns.Close();
			lostNouns.Close();
			MessageBox.Show("done");
		}

		private void testbinsearch(string pos)
		{
			System.IO.StreamReader indexFile = new System.IO.StreamReader(@"C:\Program Files\WordNet\2.1\dict\" + pos);
//			StreamWriter foundNouns = new StreamWriter(@"C:\found nouns.txt");
			StreamWriter lostNouns = new StreamWriter(@"C:\lost " + pos + ".txt");

			string currentLine = indexFile.ReadLine();
			string indexLine = string.Empty;

			System.IO.StreamReader fp = new System.IO.StreamReader(@"C:\Program Files\WordNet\2.1\dict\index." + pos);

			while (currentLine != null)
			{
				string currentWord = currentLine.Substring(0, currentLine.IndexOf(' '));

				// this transformation doesn't reflect the library's
				// true transformations - it transforms internally
				// in index lookup
				//currentWord = currentWord.Replace("-"," ").Replace("_"," ");
				currentWord = currentWord.Replace("_"," ");

				if (currentWord.Length > 0)
				{
					//indexLine = GetIndexLine(currentWord, "noun");
					indexLine = Wnlib.WNDB.binSearch(currentWord, fp);

					if (indexLine == null)
					{
						lostNouns.WriteLine(currentWord);
					}
					else
					{
//						foundNouns.WriteLine(currentWord);
					}
				}

				currentLine = indexFile.ReadLine();
			}

//			foundNouns.Close();
			lostNouns.Close();
			MessageBox.Show("done");
		}
	}
}
