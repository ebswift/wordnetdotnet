/*
 * This file is a part of the WordNet.Net open source project.
 * 
 * Author:	Jeff Martin
 * Date:	6/07/2005
 * 
 * Copyright (C) 2005 Malcolm Crowe, Troy Simpson, Jeff Martin
 * 
 * Project Home: http://www.ebswift.com
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 * 
*/

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace Lexicon_Sample
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class frmMain : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox txtWord;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtOut;
		private System.Windows.Forms.CheckBox chkMorphs;
		private System.Windows.Forms.Button cmdLookupPOS;
		private System.Windows.Forms.Button cmdLookupSYN;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmMain()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			// TDMS - 14 July 2005 - initialise the WordNet path
			Wnlib.WNCommon.path = "C:\\Program Files\\WordNet\\2.1\\dict\\";
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
			this.txtWord = new System.Windows.Forms.TextBox();
			this.cmdLookupPOS = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.txtOut = new System.Windows.Forms.TextBox();
			this.chkMorphs = new System.Windows.Forms.CheckBox();
			this.cmdLookupSYN = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// txtWord
			// 
			this.txtWord.Location = new System.Drawing.Point(24, 40);
			this.txtWord.Name = "txtWord";
			this.txtWord.Size = new System.Drawing.Size(192, 20);
			this.txtWord.TabIndex = 0;
			this.txtWord.Text = "";
			// 
			// cmdLookupPOS
			// 
			this.cmdLookupPOS.Location = new System.Drawing.Point(232, 40);
			this.cmdLookupPOS.Name = "cmdLookupPOS";
			this.cmdLookupPOS.Size = new System.Drawing.Size(152, 23);
			this.cmdLookupPOS.TabIndex = 1;
			this.cmdLookupPOS.Text = "Lookup Part Of Speech";
			this.cmdLookupPOS.Click += new System.EventHandler(this.cmdLookupPOS_Click);
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.Location = new System.Drawing.Point(16, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(368, 23);
			this.label1.TabIndex = 2;
			this.label1.Text = "WordNet.Net Lexicon Sample";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// txtOut
			// 
			this.txtOut.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.txtOut.Location = new System.Drawing.Point(24, 104);
			this.txtOut.Multiline = true;
			this.txtOut.Name = "txtOut";
			this.txtOut.ReadOnly = true;
			this.txtOut.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtOut.Size = new System.Drawing.Size(360, 104);
			this.txtOut.TabIndex = 3;
			this.txtOut.Text = "";
			// 
			// chkMorphs
			// 
			this.chkMorphs.Location = new System.Drawing.Point(24, 72);
			this.chkMorphs.Name = "chkMorphs";
			this.chkMorphs.Size = new System.Drawing.Size(144, 24);
			this.chkMorphs.TabIndex = 4;
			this.chkMorphs.Text = "Include Word Morphs";
			// 
			// cmdLookupSYN
			// 
			this.cmdLookupSYN.Location = new System.Drawing.Point(232, 72);
			this.cmdLookupSYN.Name = "cmdLookupSYN";
			this.cmdLookupSYN.Size = new System.Drawing.Size(152, 23);
			this.cmdLookupSYN.TabIndex = 5;
			this.cmdLookupSYN.Text = "Lookup Synonyms";
			this.cmdLookupSYN.Click += new System.EventHandler(this.cmdLookupSYN_Click);
			// 
			// frmMain
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(402, 230);
			this.Controls.Add(this.cmdLookupSYN);
			this.Controls.Add(this.chkMorphs);
			this.Controls.Add(this.txtOut);
			this.Controls.Add(this.txtWord);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cmdLookupPOS);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "frmMain";
			this.Text = "Lexicon Sample";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new frmMain());
		}

		private void cmdLookupPOS_Click(object sender, System.EventArgs e)
		{
			txtOut.Text = "";

			// perform a part-of-speech lookup using the Lexicon
			WnLexicon.WordInfo wordinfo = WnLexicon.Lexicon.FindWordInfo( txtWord.Text, chkMorphs.Checked );

			// NOTE: Including morphology matches only changes the output when no direct matches have been found.

			// no match?
			if( wordinfo.partOfSpeech == Wnlib.PartsOfSpeech.Unknown )
			//if( wordinfo.Strength == 0 )  ^ same as above ^
			{
				txtOut.AppendText( "No Match found!\r\n" );
				return;
			}

			// for each part of speech...
			Wnlib.PartsOfSpeech[] enums = (Wnlib.PartsOfSpeech[])Enum.GetValues( typeof( Wnlib.PartsOfSpeech ) );
			txtOut.AppendText( "\r\nSense Counts:\r\n" );
			for( int i=0; i<enums.Length; i++ )
			{
				Wnlib.PartsOfSpeech pos = enums[i];

				// skip "Unknown"
				if( pos == Wnlib.PartsOfSpeech.Unknown )
					continue;

				// output sense counts
				txtOut.AppendText( String.Format( "{0,12}: {1}\r\n", pos, wordinfo.senseCounts[i] ) );
			}

			txtOut.AppendText( String.Format( "\r\nProbable Part Of Speech: {0}\r\n", wordinfo.partOfSpeech ) );
		}

		private void cmdLookupSYN_Click(object sender, System.EventArgs e)
		{
			txtOut.Text = "";

			// first find the part of speech
			WnLexicon.WordInfo wordinfo = WnLexicon.Lexicon.FindWordInfo( txtWord.Text, chkMorphs.Checked );

			// see if the word was found
			if( wordinfo.partOfSpeech == Wnlib.PartsOfSpeech.Unknown )
			{
				txtOut.AppendText( "No Match found!\r\n" );
				return;
			}

			// perform a synonym lookup using the Lexicon
			string[] synonyms = WnLexicon.Lexicon.FindSynonyms( txtWord.Text, wordinfo.partOfSpeech, chkMorphs.Checked );

			// display results
			txtOut.AppendText( "Look up: " + txtWord.Text + "\r\n\r\n" );
			foreach( string s in synonyms )
				txtOut.AppendText( "\t" + s + "\r\n" );
		}
	}
}
