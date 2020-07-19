/*
 * Copyright (C) 2005 Malcolm Crowe, Troy Simpson 
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
 * */

using System.Linq;
using System;
using System.Reflection;
using System.Collections;
using Microsoft.VisualBasic;
using Wnlib;
using System.Text.RegularExpressions;
using System.IO;

namespace WordNet
{

	public class Overview
	{
		public Overview()
		{
		}

		private void usePassage(string passage, ref string tmpstr)
		{
			StrTok st = new StrTok(passage.Replace(Constants.vbLf + Constants.vbLf, Constants.vbLf + " " + Constants.vbLf), '\n');
			tmpstr += passage;
		}

		//As SizeF ' of Search
		public void useList(ArrayList w, string help, ref string tmpstr)
		{
			int j = 0;
			int i = 0;
			int x = 0;
			int senses = 0;
			int ttexts = 0;
			string wrdtype = "";
			string srchword = "";
			string tmpword = null;

			if (help != null & !string.IsNullOrEmpty(help))
			{
				//usePassage(help, tmpstr)
				//            tmpstr += help
			}

			// loop for noun, verb, adj, adv
			for (j = 0; j <= w.Count - 1; j++)
			{
				Search se = (Search)w[j];
				Wnlib.PointerType ptp = (Wnlib.PointerType)se.sch.ptp;

				if (string.IsNullOrEmpty(srchword) & !string.IsNullOrEmpty(se.word))
				{
					srchword = se.word;
				}

				if (se.senses.Count == 0 & string.IsNullOrEmpty(se.buf))
				{
					continue;
				}

				// this info will be used in the search summary
				if (senses == 0)
				{
					senses = se.senses.Count;
					ttexts = se.taggedSenses;
					wrdtype = se.pos.name;
				}

				switch (se.sch.label)
				{
					case "Frequency":
						ArrayList freqcats = new ArrayList();

						freqcats.Add("extremely rare");
						freqcats.Add("very rare");
						freqcats.Add("rare");
						freqcats.Add("uncommon");
						freqcats.Add("common");
						freqcats.Add("familiar");
						freqcats.Add("very familiar");
						freqcats.Add("extremely familiar");

						string thisfreq = null;
						int cnt = 0;
						int familiar = 0;
						string wrd = null;

						cnt = (int)se.countSenses[0];
						if (cnt == 0)
						{
							familiar = 0;
						}
						else if (cnt == 1)
						{
							familiar = 1;
						}
						else if (cnt == 2)
						{
							familiar = 2;
						}
						else if (cnt >= 3 & cnt <= 4)
						{
							familiar = 3;
						}
						else if (cnt >= 5 & cnt <= 8)
						{
							familiar = 4;
						}
						else if (cnt >= 9 & cnt <= 16)
						{
							familiar = 5;
						}
						else if (cnt >= 17 & cnt <= 32)
						{
							familiar = 6;
						}
						else if (cnt > 32)
						{
							familiar = 7;
						}

						switch (se.pos.name)
						{
							case "noun":
								wrd = "a noun";

								break;
							case "adj":
								wrd = "an adjective";

								break;
							case "verb":
								wrd = "a verb";

								break;
							case "adverb":
								wrd = "an adverb";

								break;
							default:
								// should never get to here
								wrd = "a " + se.pos.name;
								break;
						}

						tmpstr = se.word + " used as " + wrd + " is " + freqcats[familiar] + " (polysemy count=" + cnt + ").";
						return;


						break;
					case "Overview":
						string tmpname = se.pos.name;

						if (tmpname == "adj")
						{
							tmpname = "adjective";
							// looks better when the full word is presented
						}
						tmpstr += "<p class='Type'>" + Strings.UCase(se.sch.label + " of " + tmpname + " " + se.word) + "</p>";

						Wnlib.SynSet syns = null;
						string wrds = null;

						for (x = 0; x <= se.senses.Count - 1; x++)
						{
							tmpstr += "<font color='red'><b>" + x + 1 + ") </b></font> ";
							//                        tmpstr += "<font color='blue'><b>"

							syns = (Wnlib.SynSet)se.senses[x];
							wrds = "";

							// loop for each synonym in the definition
							for (i = 0; i <= syns.words.Length - 1; i++)
							{
								if (i > 0)
								{
									wrds += ",";
								}

								tmpword = syns.words[i].word;

								wrds += tmpword;

								//If syns.words(i).uniq <> 0 Then
								//    tmpstr += "</b><font color='red'><sup>" & syns.words(i).uniq
								//    tmpstr += "</sup></font><b>"
								//End If
							}

							//wrds = formatWordList(wrds)

							//tmpstr += wrds

							string tmpstr2 = null;

							//tmpstr2 = syns.defn;
							//tmpstr2 = linkDefinition(tmpstr2)
							tmpstr += formatWrdDefn(wrds, syns);
							//tmpstr += "</b><b>:</b></font> " & tmpstr2 & "<br />"
						}


						break;
					//Dim se As Search = CType(w(j), Search)
					//sw = se.word
					//usePassage(se.buf, tmpstr)
					//tmpstr.Text += se.buf;

					default:
						Wnlib.SynSet ss = new Wnlib.SynSet();
						ss.defn = se.buf;
						ss.hereiam = -1;
						tmpstr = decodeOther(ss);
						break;
						//tmpstr += se.buf
						//tmpstr = Replace(tmpstr, "_", " ")
						//tmpstr = Replace(tmpstr, vbLf, "<br />")

				}
			}

			// show the message at the bottom showing senses and tagged texts
			//        If senses = 0 Then
			//        tmpstr = "No results found for " & srchword & "."
			//        Else
			tmpstr += "<font color='green'>";
			tmpstr += "<br />" + Constants.vbCrLf + wrdtype + " " + srchword + " has ";
			if (senses > 0)
			{
				tmpstr += senses + " sense(s)";
			}
			else
			{
				tmpstr += "no senses ";
			}

			if (ttexts > 0)
			{
				tmpstr += " (first " + ttexts + " from tagged texts)";
			}
			tmpstr += "</font>";

			// make contents of quotes italicized
			italicizeQuotes(ref tmpstr);
			//        End If
		}

		// sets contents of quotes to italized and maroon colour
		private void italicizeQuotes(ref string tmpstr)
		{
			int i = 1;
			bool iflag = true;

			while (i > 0)
			{
				i = Strings.InStr(i, tmpstr, "\"");

				if (i > 0)
				{
					if (iflag)
					{
						tmpstr = Strings.Left(tmpstr, i - 1) + Strings.Replace(tmpstr, "\"", Strings.Chr(34) + "<span class='Quote'>", i, 1);
						i += Strings.Len(Strings.Chr(34) + "<span class='Quote'>") + 1;
					}
					else
					{
						tmpstr = Strings.Left(tmpstr, i - 1) + Strings.Replace(tmpstr, "\"", "</span>" + Strings.Chr(34), i, 1);
						i += Strings.Len("</span>" + Strings.Chr(34)) + 1;
					}
					iflag = !iflag;
				}
			}
		}

		// format lists
		private string decodeOther(SynSet ss)
		{
			string buf = ss.defn;
			string tmpstr = null;
			string[] tmparr = null;
			string retstr = null;
			string wrdlst = null;
			string defn = null;
			string fullAppName = Assembly.GetExecutingAssembly().GetName().CodeBase;
			//This strips off the exe name
			string FullAppPath = Path.GetDirectoryName(fullAppName);

			FullAppPath = Strings.Mid(FullAppPath, Strings.Len("file:\\\\"));

			buf = Strings.Replace(buf, "_", " ");

			// split the summary buffer into lines
			tmparr = Strings.Split(buf, Constants.vbLf);

			int i = 0;
			int x = 0;
			bool senseflag = false;
			// determine if a sense has been located
			int prevLevel = 0;
			// previous indent level, so the system knows how many ul's to close and how to handle new indents
			ArrayList levelArray = new ArrayList();

			for (i = 2; i <= tmparr.GetUpperBound(0); i++)
			{
				tmpstr = tmparr[i];
				// get a line for processing

				// this is a section separator
				if (Strings.InStr(tmpstr, "----") > 0)
				{
					continue;
				}

				// Sense, followed by a number (eg. Sense 1) denotes a definition line
				if (Strings.Left(tmpstr, 5) == "Sense" & !(Strings.InStr(tmpstr, "--") > 0))
				{
					if (Strings.Asc(Strings.Mid(tmpstr, 7, 1)) >= 49 & Strings.Asc(Strings.Mid(tmpstr, 7, 1)) <= 57)
					{
						senseflag = true;

						// collapse all open levels
						if (prevLevel > 0)
						{
							int inc = 0;

							for (inc = levelArray.Count; inc >= 0; inc += -1)
							{
								try
								{
									levelArray.RemoveAt(levelArray.Count - 1);
									retstr += "</ul>";
								}
								catch
								{
								}
							}

							prevLevel = 0;
						}

						continue;
					}
				}

				// this is a 'pertains to' line
				//            If Left(LTrim(tmpstr), 11) = "Pertains to" And InStr(tmpstr, "Sense") Then
				// this is 'pertains to' or antonym
				if (Strings.InStr(tmpstr, "(Sense") > 0)
				{
					retstr += "<br />" + Constants.vbCrLf + Strings.Left(tmpstr, Strings.InStr(tmpstr, ")"));
					tmparr[i] = Strings.Mid(tmpstr, Strings.Len(Strings.Left(tmpstr, Strings.InStr(tmpstr, ")"))) + 1);
					i -= 1;
					continue;
				}

				// this is a definition line
				if (senseflag)
				{
					retstr += "<font color='red'><b>" + x + 1 + ") </b></font> ";
					tmpstr = Strings.RTrim(tmpstr);
					wrdlst = Strings.Left(tmpstr, Strings.InStr(tmpstr, "--") - 2);
					defn = Strings.Mid(tmpstr, Strings.Len(wrdlst) + 6);
					wrdlst = Strings.Replace(wrdlst, ", ", ",");

					Wnlib.SynSet tmpss = new Wnlib.SynSet();
					tmpss.defn = defn;
					tmpss.hereiam = -1;

					retstr += formatWrdDefn(wrdlst, tmpss);
					x += 1;
					senseflag = false;
					// this is an indented definition
				}
				else if (!senseflag & !string.IsNullOrEmpty(Strings.LTrim(Strings.RTrim(tmpstr))))
				{
					if (Strings.InStr(tmpstr, "CATEGORY TERM") > 0)
					{
						// collapse higher levels
						if (prevLevel > 0)
						{
							int inc = 0;

							for (inc = 1; inc <= prevLevel; inc++)
							{
								try
								{
									levelArray.RemoveAt(levelArray.Count - 1);
									retstr += "</ul>";
								}
								catch
								{
								}
							}

							prevLevel = 0;
						}

						wrdlst = Strings.Mid(tmpstr, Strings.InStr(tmpstr, ")") + 2);
						wrdlst = formatWordList(wrdlst);
						defn = Strings.Left(tmpstr, Strings.InStr(tmpstr, ")"));

						retstr += "<li>" + Constants.vbTab + defn + " " + wrdlst + "</li>";
						continue;
					}
					else if (Strings.InStr(tmpstr, "(noun)") > 0 || Strings.InStr(tmpstr, "(verb)") > 0 || Strings.InStr(tmpstr, "(adj)") > 0 || Strings.InStr(tmpstr, ("adverb")) > 0)
					{
						// this check must be made first, otherwise confusion can occur from colons in the sentence (for the next check)
						int lvl = 0;

						lvl = indentLevel(tmpstr);

						if (lvl < prevLevel)
						{
							int levdif = 0;
							int inc = 0;

							levdif = prevLevel - lvl;
							// get the number of levels we've jumped back

							for (inc = 1; inc <= levdif; inc++)
							{
								// remove the defined level from the tracking array
								try
								{
									levelArray.RemoveAt(levelArray.Count - 1);
									retstr += "</ul>";
								}
								catch
								{
								}
							}
						}
						else if (lvl > prevLevel)
						{
							levelArray.Add(lvl);
							retstr += "<ul>";
						}
						else
						{
							if (levelArray.IndexOf(lvl) == -1)
							{
								// starting level was level 2, so this confuses the system
								retstr += "<ul>";
								levelArray.Add(lvl);
							}
						}

						prevLevel = lvl;

						wrdlst = Strings.Mid(tmpstr, Strings.InStr(tmpstr, ")") + 2);
						defn = "";
						//defn = Mid(wrdlst, InStr(wrdlst, "--") + 3)
						//wrdlst = Left(wrdlst, InStr(wrdlst, "--") - 1)
						Wnlib.SynSet tmpss = new Wnlib.SynSet();
						tmpss.defn = "";
						tmpss.hereiam = -1;
						wrdlst = formatWrdDefn(wrdlst, tmpss);

						retstr += "<li>" + Constants.vbTab + Strings.Left(tmpstr, Strings.InStr(tmpstr, ")")) + " " + wrdlst + "</li>";
						continue;
					}
					else if (Strings.InStr(tmpstr, ">") > 0)
					{
						// this check must be made first, otherwise confusion can occur from colons in the sentence (for the next check)
						int lvl = 0;

						lvl = indentLevel(tmpstr);

						if (lvl < prevLevel)
						{
							int levdif = 0;
							int inc = 0;

							levdif = prevLevel - lvl;
							// get the number of levels we've jumped back

							for (inc = 1; inc <= levdif; inc++)
							{
								// remove the defined level from the tracking array
								try
								{
									levelArray.RemoveAt(levelArray.Count - 1);
									retstr += "</ul>";
								}
								catch
								{
								}
							}
						}
						else if (lvl > prevLevel)
						{
							levelArray.Add(lvl);
							retstr += "<ul>";
						}
						else
						{
							if (levelArray.IndexOf(lvl) == -1)
							{
								// starting level was level 2, so this confuses the system
								retstr += "<ul>";
								levelArray.Add(lvl);
							}
						}

						prevLevel = lvl;

						wrdlst = Strings.Mid(tmpstr, Strings.InStr(tmpstr, ">") + 2);
						if (wrdlst.IndexOf("--") > -1)
						{
							defn = Strings.Mid(wrdlst, Strings.InStr(wrdlst, "--") + 3);
							wrdlst = Strings.Left(wrdlst, Strings.InStr(wrdlst, "--") - 1);
						}
						else
						{
							int z = 1; // this line exists purely as a breakpoint placeholder
						}

						Wnlib.SynSet tmpss = new Wnlib.SynSet();
						tmpss.defn = defn;
						tmpss.hereiam = -1;
						wrdlst = formatWrdDefn(wrdlst, tmpss);
						retstr += "<li>" + Constants.vbTab + Strings.Left(tmpstr, Strings.InStr(tmpstr, ">")) + " " + wrdlst + "</li>";
						continue;
					}
					else if (Strings.InStr(tmpstr, ":") > 0)
					{
						int lvl = 0;

						lvl = indentLevel(tmpstr);

						if (lvl < prevLevel)
						{
							int levdif = 0;
							int inc = 0;

							levdif = prevLevel - lvl;
							// get the number of levels we've jumped back

							for (inc = 1; inc <= levdif; inc++)
							{
								// remove the defined level from the tracking array
								try
								{
									levelArray.RemoveAt(levelArray.Count - 1);
									retstr += "</ul>";
								}
								catch
								{
								}
							}
						}
						else if (lvl > prevLevel)
						{
							levelArray.Add(lvl);
							retstr += "<ul>";
						}
						else
						{
							if (levelArray.IndexOf(lvl) == -1)
							{
								// starting level was level 2, so this confuses the system
								retstr += "<ul>";
								levelArray.Add(lvl);
							}
						}

						prevLevel = lvl;

						wrdlst = Strings.Mid(tmpstr, Strings.InStr(tmpstr, ":") + 2);
						defn = Strings.Mid(wrdlst, Strings.InStr(wrdlst, "--") + 3);
						if (Strings.InStr(wrdlst, "--") == 0)
							continue;
						wrdlst = Strings.Left(wrdlst, Strings.InStr(wrdlst, "--") - 1);
						Wnlib.SynSet tmpss = new Wnlib.SynSet();
						tmpss.defn = defn;
						tmpss.hereiam = -1;
						wrdlst = formatWrdDefn(wrdlst, tmpss);

						retstr += "<li>" + Constants.vbTab + Strings.Left(tmpstr, Strings.InStr(tmpstr, ":") + 1) + " " + wrdlst + "</li>";
						continue;
					}
				}
			}

			//If thirdindentflag Then
			//    retstr += "</ul>" ' close the third indent
			//    thirdindentflag = False
			//End If
			//If secondindentflag Then
			//    retstr += "</ul>" ' close the second indent
			//    secondindentflag = False
			//End If
			//If firstindentflag Then
			//    retstr += "</ul>" ' close the first indent
			//    firstindentflag = False
			//End If
			// collapse all open levels
			if (prevLevel > 0)
			{
				int inc = 0;

				for (inc = levelArray.Count; inc >= 0; inc += -1)
				{
					//levelArray.RemoveAt(levelArray.IndexOf(inc))
					try
					{
						levelArray.RemoveAt(levelArray.Count - 1);
						retstr += "</ul>";
					}
					catch
					{
					}
				}

				prevLevel = 0;
			}

			return retstr;
		}

		// gets the indentation level of a line
		private int indentLevel(string indstr)
		{
			string tmpstr = "     ";
			string spc = "     ";
			int i = 1;

			// loop, adding another 5 spaces in each loop.
			// levels are delimited by spaces.
			while ((1 == 1))
			{
				if (!(Strings.Left(indstr, Strings.Len(tmpstr)) == tmpstr))
				{
					// alpha/numeric characters have entered the check, so leave the loop
					break; // TODO: might not be correct. Was : Exit While
				}

				tmpstr += spc;
				// add another 5 spaces
				i += 1;
			}

			return i - 1;
		}

		// turns all words in a definition into hyperlinks
		private string linkDefinition(string tmpstr)
		{
			int wrdstart = -1;
			int wrdend = -1;
			string wrd = null;
			string tmpstr2 = null;
			int i = 1;
			int chr = 0;

			tmpstr = Strings.LTrim(tmpstr);

			if (string.IsNullOrEmpty(tmpstr))
			{
				return tmpstr;
			}

			while (true)
			{
				chr = Strings.Asc(Strings.Mid(tmpstr, i, 1));
				// include uppercase, lowercase, numbers and hyphen (respectively)
				if (((chr >= 65 & chr <= 90) | (chr >= 97 & chr <= 122) | (chr >= 48 & chr <= 57) | chr == 45) & wrdstart == -1)
				{
					wrdstart = i;
					wrdend = -1;
				}
				else
				{
					if ((!((chr >= 65 & chr <= 90) | (chr >= 97 & chr <= 122) | (chr >= 48 & chr <= 57) | chr == 45)) & wrdstart > 0)
					{
						// complete word has been found, so replace that instance with a hyperlinked version of self
						wrdend = i - 1;
						wrd = Strings.Mid(tmpstr, wrdstart, (wrdend - wrdstart) + 1);
						tmpstr2 = Strings.Left(tmpstr, wrdstart - 1);
						tmpstr = tmpstr2 + Strings.Replace(tmpstr, wrd, "<a href='" + wrd + "'>" + wrd + "</a>", i - Strings.Len(wrd), 1);
						wrdstart = -1;
						i += Strings.Len("<a href=''></a>") + Strings.Len(wrd);
					}
				}

				i += 1;

				if (i > Strings.Len(tmpstr))
				{
					break; // TODO: might not be correct. Was : Exit While
				}
			}

			// end of string has been found but last word has not been processed
			// - so process it
			if (wrdstart > 0 & wrdend == -1)
			{
				wrdend = i - 1;
				wrd = Strings.Mid(tmpstr, wrdstart, (wrdend - wrdstart) + 1);
				tmpstr2 = Strings.Left(tmpstr, wrdstart - 1);
				tmpstr = tmpstr2 + Strings.Replace(tmpstr, wrd, "<a href='" + wrd + "'>" + wrd + "</a>", i - Strings.Len(wrd), 1);
				wrdstart = -1;
				i += Strings.Len("<a href=''></a>") + Strings.Len(wrd);
			}

			return tmpstr;
		}

		// breaks the word list down and placed it into hyperlinks, and appends the definition
		private string formatWrdDefn(string wrdlst, SynSet defn)
		{
			string retstr = null;

			retstr = formatWordList(wrdlst);

			string tmpstr = linkDefinition(defn.defn);
			// turn definition words into hyperlinks
			if (!string.IsNullOrEmpty(tmpstr))
			{
				//if (WNOpt.opt("-o").flag && defn.hereiam != -1)
				//{
				//	tmpstr = "<font color='green'>Synset offset:" + defn.hereiam.ToString() + "<font> " + tmpstr;
				//}
				retstr += ":";
			}

			retstr += " <span class='Defn'>" + tmpstr + "</span><br />" + Constants.vbCrLf;

			return retstr;
		}

		// format the word list
		private string formatWordList(string wrdlst)
		{
			string[] wrdarr = null;
			string tmpwrd = null;
			string tmpspr = null;
			string retstr = null;
			int x = 0;

			// (a), (ip) and (p) are a product of deadjify.
			// they are removed from the buffer, but not from
			// the individual words
			wrdlst = Strings.Replace(wrdlst, "(a)", "");
			wrdlst = Strings.Replace(wrdlst, "(ip)", "");
			wrdlst = Strings.Replace(wrdlst, "(p)", "");

			wrdarr = Strings.Split(wrdlst, ",");

			if (!string.IsNullOrEmpty(wrdlst))
			{
				retstr += "<span class='Word'>";
			}

			for (x = 0; x <= wrdarr.GetUpperBound(0); x++)
			{
				if (x > 0)
				{
					retstr += ", ";
				}
				// get only the text
				tmpwrd = MainWord(wrdarr[x]);
				//Regex.Replace(wrdarr(x), "[[^0-9]", "")

				// get only the superscript number
				tmpspr = SuperScriptNumber(wrdarr[x]);
				// Regex.Replace(wrdarr(x), "[[^a-zA-Z]", "")
				retstr += "<a href='" + tmpwrd + "'>" + tmpwrd + "</a>";

				// display superscript number if it exists
				//If tmpspr <> "" Then
				// the superscript number is useless
				// retstr += "<span class='Sup'>" & tmpspr & "</span>"
				//End If
			}

			if (!string.IsNullOrEmpty(wrdlst))
			{
				retstr += "</span>";
			}

			return retstr;
		}

		private string SuperScriptNumber(string wrd)
		{
			int i = 0;

			for (i = Strings.Len(wrd); i >= 1; i += -1)
			{
				if ((Strings.Asc(Strings.Mid(wrd, i, 1)) >= 65 & Strings.Asc(Strings.Mid(wrd, i, 1)) <= 122) | Strings.Asc(Strings.Mid(wrd, i, 1)) == 40 | Strings.Asc(Strings.Mid(wrd, i, 1)) == 41)
				{
					// if this is a normal character string
					break; // TODO: might not be correct. Was : Exit For
				}
			}

			return Strings.Mid(wrd, i + 1);
		}

		private string MainWord(string wrd)
		{
			int i = 0;

			for (i = Strings.Len(wrd); i >= 1; i += -1)
			{
				if ((Strings.Asc(Strings.Mid(wrd, i, 1)) >= 65 & Strings.Asc(Strings.Mid(wrd, i, 1)) <= 122) | Strings.Asc(Strings.Mid(wrd, i, 1)) == 40 | Strings.Asc(Strings.Mid(wrd, i, 1)) == 41)
				{
					// if this is a normal character string
					break; // TODO: might not be correct. Was : Exit For
				}
			}

			return Strings.Left(wrd, i);
		}
	}
}
