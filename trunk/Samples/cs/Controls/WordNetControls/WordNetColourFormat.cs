using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

namespace WordNetControls
{
	///*
	// * This file is a part of the WordNet.Net open source project.
	// * 
	// * Copyright (C) 2005 Malcolm Crowe, Troy Simpson 
	// * 
	// * Project Home: http://www.ebswift.com
	// *
	// * This library is free software; you can redistribute it and/or
	// * modify it under the terms of the GNU Lesser General Public
	// * License as published by the Free Software Foundation; either
	// * version 2.1 of the License, or (at your option) any later version.
	// *
	// * This library is distributed in the hope that it will be useful,
	// * but WITHOUT ANY WARRANTY; without even the implied warranty of
	// * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
	// * Lesser General Public License for more details.
	// *
	// * You should have received a copy of the GNU Lesser General Public
	// * License along with this library; if not, write to the Free Software
	// * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
	// * 
	// * */

	//
	// Created by SharpDevelop.
	// User: Troy
	// Date: 4/07/2006
	// Time: 7:21 AM
	// 
	// To change this template use Tools | Options | Coding | Edit Standard Headers.
	//
	public partial class WordNetColourFormat
	{
		public event CanvasNavigatingEventHandler CanvasNavigating;
		public delegate void CanvasNavigatingEventHandler(object sender, string url);
		//ByVal e As System.Windows.Forms.WebBrowserNavigatingEventArgs)

// define the CSS for the display elements
		private string m_cssbody = "font-family:'Verdana';font-size:10pt;";
		private string m_csslexeme = "color: #0000FF; font-weight:bold; text-decoration: none";
		private string m_csspos = "font-size: 12pt; color: #FFFFFF; font-weight: bold; background-color: #808080";
		private string m_cssdefn = "color: #000000; text-decoration: none";
		private string m_cssquote = "color: #800000; text-decoration: none; font-style:italic";
		private string m_csshover = "background-color: yellow";

		public WordNetColourFormat()
		{
			// The Me.InitializeComponent call is required for Windows Forms designer support.
			this.InitializeComponent();

			//
			// TODO : Add constructor code after InitializeComponents
			//

			// setup our custom event for the application to handle
			Canvas.Navigating += NavigateCanvas;
		}

		/// <summary>
		/// Event handler for a user clicking on a word hyperlink inside the webbrowser.
		/// This raises an event that can be handled by the application.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void NavigateCanvas(object sender, System.Windows.Forms.WebBrowserNavigatingEventArgs e)
		{
			string tmpstr = null;

			tmpstr = e.Url.ToString();
			tmpstr = Strings.Replace(tmpstr, "about:blank", "");
			tmpstr = Strings.Replace(tmpstr, "about:", "");

			tmpstr = Strings.Replace(tmpstr, "%20", " ");

			if (!string.IsNullOrEmpty(tmpstr)) {
				//e.Url = tmpstr
				e.Cancel = true;

				if (CanvasNavigating != null) {
					CanvasNavigating(sender, tmpstr);
				}
			}
		}

		/// <summary>
		/// Iterates each part of speech which intern calls the iteration to the search 
		/// object to create the hierarchical output.
		/// </summary>
		/// <param name="list"></param>
		/// <param name="tmppos"></param>
		/// <returns></returns>
		public string buildContents(ArrayList list)
		{
			string outstr = "";
			string headstyle = null;
			// Wnlib.Search ss = default(Wnlib.Search);

			// define the CSS
			// TODO: move this to properties (property sheet)
			headstyle = "<style>" + Constants.vbCrLf + "<!--" + Constants.vbCrLf + "*\t{ " + CSSBody + " }" + Constants.vbCrLf + ".Word a { " + CSSLexeme + " }" + Constants.vbCrLf + ".pos        { " + CSSPOS + " }" + Constants.vbCrLf + ".Defn a { " + CSSDefinition + " }" + Constants.vbCrLf + ".Quote a { " + CSSQuote + " }" + Constants.vbCrLf + "a \t\t{ text-decoration: none }" + Constants.vbCrLf + "a:hover { " + CSSHover + " }" + Constants.vbCrLf + "-->" + Constants.vbCrLf + "</style>" + Constants.vbCrLf + Constants.vbCrLf;

			outstr = "<html><body>" + headstyle;

			//For i = 0 To list.Count - 1
			foreach ( Wnlib.Search ss in list) {
				if (ss.senses.Count > 0) {
					outstr += FormatPOS(ss.pos.name);
					outstr += contentIteration(ss);
				}
			}

			return outstr + "</body></html>";
		}

		/// <summary>
		/// Takes a given Part Of Speech and iterates the Search object to build the 
		/// hierarchical output.
		/// </summary>
		/// <param name="sch"></param>
		/// <param name="tmppos"></param>
		/// <returns></returns>
		public string contentIteration(Wnlib.Search sch)
		{
			//Wnlib.SynSet ss = default(Wnlib.SynSet);
			//Wnlib.Lexeme lx = default(Wnlib.Lexeme);
			string outstr = "";

			outstr += "<ul>";

			// iterate the returned senses
			foreach ( Wnlib.SynSet ss in sch.senses) {
				outstr += "<li>";

				// format the WORDs
				foreach ( Wnlib.Lexeme lx in ss.words) {
					outstr += FormatLexeme(lx.word) + ", ";
				}

				// remove last comma
				outstr = Strings.Mid(outstr, 1, outstr.Length - 2);

				outstr += ": ";

				// show the definition
				outstr += FormatDefn(ss.defn);

				// children
				if ((ss.senses != null)) {
					outstr += FormatWN(ss.senses);
				}

				outstr += "</li>";
			}

			outstr += "</ul>";

			return outstr;
		}

		/// <summary>
		/// Takes a definition (a phrase) and breaks each word apart, creating 
		/// a hyperlink that can be clicked in order to drill down.
		/// </summary>
		/// <param name="tmpstr"></param>
		/// <returns></returns>
		private string linkDefinition(string tmpstr)
		{
			int wrdstart = -1;
			int wrdend = -1;
			string wrd = null;
			string tmpstr2 = null;
			int i = 1;
			int chr = 0;

			tmpstr = Strings.LTrim(tmpstr);

			if (string.IsNullOrEmpty(tmpstr)) {
				return tmpstr;
			}

			while (1 == 1) {
				chr = Strings.Asc(Strings.Mid(tmpstr, i, 1));
				// include uppercase, lowercase, numbers and hyphen (respectively)
				if (((chr >= 65 & chr <= 90) | (chr >= 97 & chr <= 122) | (chr >= 48 & chr <= 57) | chr == 45) & wrdstart == -1) {
					wrdstart = i;
					wrdend = -1;
				} else {
					if ((!((chr >= 65 & chr <= 90) | (chr >= 97 & chr <= 122) | (chr >= 48 & chr <= 57) | chr == 45)) & wrdstart > -1) {
						// complete word has been found, so replace that instance with a hyperlinked version of self
						wrdend = i - 1;
						wrd = Strings.Mid(tmpstr, wrdstart, (wrdend - wrdstart) + 1);
						tmpstr2 = Strings.Mid(tmpstr, 1, wrdstart - 1);
						tmpstr = tmpstr2 + Strings.Replace(tmpstr, wrd, "<a href='" + wrd + "'>" + wrd + "</a>", i - Strings.Len(wrd), 1);
						wrdstart = -1;
						i += Strings.Len("<a href=''></a>") + Strings.Len(wrd);
					}
				}

				i += 1;

				if (i > Strings.Len(tmpstr)) {
					break;
				}
			}

			// end of string has been found but last word has not been processed
			// - so process it
			if (wrdstart > -1 & wrdend == -1) {
				wrdend = i - 1;
				wrd = Strings.Mid(tmpstr, wrdstart, (wrdend - wrdstart) + 1);
				tmpstr2 = Strings.Mid(tmpstr, 1, wrdstart - 1);
				tmpstr = tmpstr2 + Strings.Replace(tmpstr, wrd, "<a href='" + wrd + "'>" + wrd + "</a>", i - Strings.Len(wrd), 1);
				wrdstart = -1;
				i += Strings.Len("<a href=''></a>") + Strings.Len(wrd);
			}

			return tmpstr;
		}

		/// <summary>
		/// Takes a sample sentence (defined by being enclosed in quotes) and
		/// defines it for CSS formatting.
		/// </summary>
		/// <param name="tmpstr">The complete definition including sample sentences wrapped in quotes</param>
		/// <returns></returns>
		private string formatSampleSentence(string tmpstr)
		{
			int i = 1;
			bool iflag = true;

			while (i > 0) {
				// i = Strings.InStr(i, tmpstr, Strings.Chr(34));
				i = tmpstr.IndexOf((char) 34, i);
				if (i > 0) {
					if (iflag) {
						//tmpstr = Strings.Mid(tmpstr, 1, i - 1) + Strings.Replace(tmpstr, Strings.Chr(34), Strings.Chr(34) + "<span class='Quote'>", i, 1);
						tmpstr = tmpstr.Substring(1, i - 1) + Strings.Replace(tmpstr, "\"", "\"<span class='Quote'>", i, 1);
						i += Strings.Len(Strings.Chr(34) + "<span class='Quote'>") + 1;
					} else {
						tmpstr = Strings.Mid(tmpstr, 1, i - 1) + Strings.Replace(tmpstr, "\"", "\"</span>" + Strings.Chr(34), i, 1);
						i += Strings.Len("</span>" + Strings.Chr(34)) + 1;
					}
					iflag = !iflag;
				}
			}

			return tmpstr;
		}

		/// <summary>
		/// Takes the Part Of Speech heading and wraps it in a span for further 
		/// CSS formatting.
		/// </summary>
		/// <param name="pos">Part Of Speech description (noun, verb, adj, adv)</param>
		/// <returns></returns>
		public string FormatPOS(string pos)
		{
			string retstr = "";

			switch (pos) {
				case "noun":
					retstr = "Noun";

					break;
				case "verb":
					retstr = "Verb";

					break;
				case "adj":
					retstr = "Adjective";

					break;
				case "adv":
					retstr = "Adverb";
					break;
			}

			// prepare for CSS formatting
			retstr = "<div class='pos'>" + retstr + "</div>";

			return retstr;
		}

		/// <summary>
		/// Iterates and formats all of the children of the given top-level Search 
		/// object for a given Part Of Speech.
		/// </summary>
		/// <param name="ssarray">The SynSetList for a given Part Of Speech</param>
		/// <returns></returns>
		private string FormatWN(Wnlib.SynSetList ssarray)
		{
			string retstr = "";
			//Wnlib.SynSet ss = default(Wnlib.SynSet);
			//Wnlib.Lexeme lx = default(Wnlib.Lexeme);

			foreach ( Wnlib.SynSet ss in ssarray) {
				retstr += "<li>";

				foreach (Wnlib.Lexeme lx in ss.words) {
					retstr += FormatLexeme(lx.word) + ", ";
				}

				// remove last comma
				retstr = Strings.Mid(retstr, 1, retstr.Length - 2);

				retstr += ": ";

				// show the definition
				retstr += FormatDefn(ss.defn);

				// recursive call to self for all children
				if ((ss.senses != null)) {
					retstr += "<ul>";
					retstr += FormatWN(ss.senses);
					retstr += "</ul>";
				}

				retstr += "</li>";
			}

			return retstr;
		}

		/// <summary>
		/// Formats each lexeme by turning it into a hyperlink.
		/// </summary>
		/// <param name="lx">A lexeme (word)</param>
		/// <returns></returns>
		private string FormatLexeme(string lx)
		{
			string retstr = "";

			lx = Strings.Replace(lx, "_", " ");

			retstr = "<a href='" + lx + "' class='Word'>" + lx + "</a>";

			return retstr;
		}

		/// <summary>
		/// Formats the definition (defn) part of the Search object.
		/// </summary>
		/// <param name="defn">The definition part of the Search object</param>
		/// <returns></returns>
		private string FormatDefn(string defn)
		{
			string retstr = "";

			defn = Strings.Replace(defn, "_", " ");

			retstr = "<span class='Defn'>" + formatSampleSentence(linkDefinition(defn)) + "</span>";

			return retstr;
		}

		public string CSSLexeme {
			get { return m_csslexeme; }
			set { m_csslexeme = value; }
		}

		public string CSSPOS {
			get { return m_csspos; }
			set { m_csspos = value; }
		}

		public string CSSDefinition {
			get { return m_cssdefn; }
			set { m_cssdefn = value; }
		}

		public string CSSQuote {
			get { return m_cssquote; }
			set { m_cssquote = value; }
		}

		public string CSSBody {
			get { return m_cssbody; }
			set { m_cssbody = value; }
		}

		public string CSSHover {
			get { return m_csshover; }
			set { m_csshover = value; }
		}
	}
}
