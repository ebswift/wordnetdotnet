/*
 * This file is a part of the WordNet.Net open source project.
 * 
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

using System;
using System.IO;
using System.Collections;

namespace Wnlib
{
	/// <summary>
	/// WordNet search code morphology functions
	/// </summary>
	public class MorphStr
	{
		static string[] sufx = {
								   /* Noun suffixes */
								   "s", "ses", "xes", "zes", "ches", "shes", "men", "ies",
								   /* Verb suffixes */
								   "s", "ies", "es", "es", "ed", "ed", "ing", "ing",
								   /* Adjective suffixes */
								   "er", "est", "er", "est"
							   };
		static string[] addr = { 
								   /* Noun endings */
								   "", "s", "x", "z", "ch", "sh", "man", "y",
								   /* Verb endings */
								   "", "y", "e", "", "e", "", "e", "",
								   /* Adjective endings */
								   "", "", "e", "e"
							   };
		static int[] offsets = { 0, 8, 8, 16 };  
//		static int[] offsets = { 0, 8, 16 };   // 7 Nov 2006 - contributed bug fix by Saurabh Abichandani
//		static int[] offsets = { 0, 0, 8, 16 }; // TODO: investigate this - wordnet "morph.c" defines this as being correct but we are using the above
		static int[] cnts = { 8, 8, 8, 4 }; // 0 changed to 8 - Troy
//		static int[] cnts = { 8, 8, 4 };    // 7 Nov 2006 - contributed bug fix by Saurabh Abichandani
		
		static string[] prepositions = 
			{
				"to", "at", "of", "on", "off", "in", "out", "up", "down", "from", 
				"with", "into", "for", "about", "between" 
			};
		string searchstr,str;
		int svcnt,svprep;
		PartOfSpeech pos;
		Exceptions e;
		bool firsttime;
		int cnt;
		public MorphStr(string s,string p) : this(s,PartOfSpeech.of(p)) {}
		public MorphStr(string s,PartOfSpeech p)
		{
			string origstr = s;
			pos = p;
			if (pos.clss=="SATELLITE")
				pos = PartOfSpeech.of("adj");
			/* Assume string hasnt had spaces substitued with _ */
			str = origstr.Replace(' ','_').ToLower();
			searchstr = "";
			cnt = str.Split('_').Length;
			svprep = 0;
			firsttime = true;
		}
		public string next()
		{
			string word,tmp;
			int prep,cnt,st_idx=0,end_idx= 0,end_idx1,end_idx2;
			string append="";

			/* first time through for this string */
			if (firsttime) 
			{
				firsttime = false;
				cnt = str.Split('_').Length;
				svprep = 0;

				/* first try exception list */
				e = new Exceptions(str,pos);
				if ((tmp=e.next())!=null && tmp!=str) 
				{
					svcnt=1; /* force next time to pass NULL */
					return tmp;
				}
				/* then try simply morph on original string */
				if (pos.name!="verb" && ((tmp=morphword(str))!=null) && str!=tmp)
						return tmp;
				if (pos.name=="verb" && cnt>1 && (prep=hasprep(str,cnt))!=0)
				{
					svprep = prep;
					return morphprep(str);
				} 
				else 
				{
					svcnt = cnt = str.Split('_').Length;
					while (--cnt>0) 
					{
						end_idx1 = str.Substring(st_idx).IndexOf('_')+st_idx;
						end_idx2 = str.Substring(st_idx).IndexOf('-')+st_idx;
						if (end_idx1>=st_idx && end_idx2>=st_idx) 
						{
							if (end_idx1<end_idx2) 
							{
								end_idx = end_idx1;
								append = "_";
							} 
							else
							{
								end_idx = end_idx2;
								append = "-";
							}
						} 
						else if (end_idx1>=st_idx) 
						{
							end_idx = end_idx1;
							append = "_";
						} 
						else 
						{
							end_idx = end_idx2;
							append = "-";
						}
						if (end_idx<0)
							return null;
						word = str.Substring(st_idx,end_idx-st_idx);
						if ((tmp=morphword(word))!=null)
							searchstr+=tmp;
						else
							searchstr+=word;
						searchstr+=append;
						st_idx = end_idx+1;
					}
					word =str.Substring(st_idx);
					if ((tmp=morphword(word))!=null)
						searchstr+=tmp;
					else
						searchstr+=word;
					if (searchstr!=str && WNDB.is_defined(searchstr,pos).NonEmpty)
						return searchstr;
					else 
						return null;
				}
			} 
			else  // not firsttime
			{
				if (svprep>0) 
				{
					svprep = 0;
					return null;
				} 
				else if (svcnt==1)
					return e.next();
				else 
				{
					svcnt = 1;
					e = new Exceptions(str,pos);
					if ((tmp=e.next())!=null && tmp!=str)
						return tmp;
					return null;
				}
			}
		}
		string morphword(string word) 
		{
			string end = "";
			string tmpbuf = "";
			if (word==null)
				return null;
			Exceptions e = new Exceptions(word,pos);
			string tmp = e.next();
			if (tmp!=null)
				return tmp;
			if (pos.name=="adverb")
				return null;
			if (pos.name=="noun") 
			{
				if (word.EndsWith("ful")) 
				{
					tmpbuf = word.Substring(0,word.Length-3);
					end = "ful";
				} 
				else if (word.EndsWith("ss")||word.Length<=2)
					return null;
			}
			if (tmpbuf.Length==0)
				tmpbuf = word;
			int offset = offsets[pos.ident];
			int cnt = cnts[pos.ident];
			for (int i=0;i<cnt;i++)
				if (tmpbuf.EndsWith(sufx[i+offset]))
				{
					// TDMS 11 Oct 2005 - bug fix - "word" substituted with "tmpbuf" as per
					// wordnet code morph.c
					//string retval = word.Substring(0,word.Length-sufx[i+offset].Length)+addr[i+offset];
					string retval = tmpbuf.Substring(0,tmpbuf.Length-sufx[i+offset].Length)+addr[i+offset];
					if (WNDB.is_defined(retval,pos).NonEmpty) 
						return retval+end;
				}
			return null;
		}

        /* Find a preposition in the verb string and return its
           corresponding word number. */
        int hasprep(string s, int wdcnt)
		{
			int i, wdnum;
			int pos = 0;
			for (wdnum=2;wdnum<=wdcnt;wdnum++) 
			{
				pos = s.IndexOf('_');
				for (pos++,i=0;i<prepositions.Length;i++) 
				{
					int len = prepositions[i].Length;
					if (len<=s.Length-pos && s.Substring(pos,len)==prepositions[i]
						&& (len==s.Length-pos || s[pos+len]=='_'))
						return wdnum;
				}
//				return 0; TDMS 8 Feb 2006 - this always returned zero if the first iteration did not return a result
//                                          Removed the return to allow the outer loop to run correctly
			}
			return 0;
		}
		string morphprep(string s)
		{
			string excWord,lastwd=null;
			int i,offset,cnt,rest,last;
			string word,end,retval;

			/* Assume that the verb is the first word in the phrase.  Strip it
			   off, check for validity, then try various morphs with the
			   rest of the phrase tacked on, trying to find a match. */

			rest = s.IndexOf('_');
			last = s.LastIndexOf('_');
			end = "";
			if (rest!=last) 
			{ // more than 2 words
				lastwd = morphword(s.Substring(last+1));
				if (lastwd!=null) 
					end = s.Substring(rest,last-rest+1)+lastwd;
			}
			word = s.Substring(0,rest);
			for (i=0;i<word.Length;i++)
				if (!char.IsLetterOrDigit(word[i]))
					return null;
			offset = offsets[PartOfSpeech.of("verb").ident];
			cnt = cnts[PartOfSpeech.of("verb").ident];
			/* First try to find the verb in the exception list */
			Exceptions e = new Exceptions(word,PartOfSpeech.of("verb"));
			while ((excWord=e.next())!=null && excWord!=word) 
			{
				retval = excWord+s.Substring(rest);
				if (WNDB.is_defined(retval,PartOfSpeech.of("verb")).NonEmpty)
					return retval;
				else if (lastwd!=null) 
				{
					retval = excWord+end;
					if (WNDB.is_defined(retval,PartOfSpeech.of("verb")).NonEmpty)
						return retval;
				}
			}
			for (i=0;i<cnt;i++) 
				if ((excWord=wordbase(word,i+offset))!=null && excWord!=word) // ending is different
				{
					retval = excWord+s.Substring(rest);
					if (WNDB.is_defined(retval,PartOfSpeech.of("verb")).NonEmpty)
						return retval;
					else if (lastwd!=null) 
					{
						retval = excWord+end;
						if (WNDB.is_defined(retval,PartOfSpeech.of("verb")).NonEmpty)
							return retval;
					}
				}
			retval = word+s.Substring(rest);
			if (s!=retval)
				return retval;
			if (lastwd!=null) 
			{
				retval=word+end;
				if (s!=retval)
					return retval;
			}
			return null;
		}
		string wordbase(string word,int ender)
		{
			if (word.EndsWith(sufx[ender]))
				return word.Substring(0,word.Length-sufx[ender].Length)+addr[ender];
			return word;
		}
	}
	public class Exceptions 
	{
		// exception list files
		static StreamReader[] excfps =null; 
		static Exceptions()
		{
			excfps = new StreamReader[PartOfSpeech.parts.Count];
			IDictionaryEnumerator d = PartOfSpeech.parts.GetEnumerator();
			while (d.MoveNext()) 
			{
				PartOfSpeech p = (PartOfSpeech)(d.Value);
				excfps[p.ident] = new StreamReader(WNDB.ExcFile(p));
			}
		}
		string line = null;
		int beglp=0, endlp= -1;
        private static string laststr = "";
        private static MemoryStream exc;

		public Exceptions(string word,string p) : this (word,PartOfSpeech.of(p)) { }
		public Exceptions(string word,PartOfSpeech pos)
		{
            if(laststr != ((System.IO.FileStream)(excfps[pos.ident].BaseStream)).Name) 
            {
                laststr = ((System.IO.FileStream)(excfps[pos.ident].BaseStream)).Name;
                StreamReader fs = excfps[pos.ident];
                Byte[] b = System.Text.Encoding.Unicode.GetBytes(fs.ReadToEnd());
                excfps[pos.ident].BaseStream.Seek(0, SeekOrigin.Begin);

                exc = new MemoryStream(b);
            }
			line = WNDB.binSearch(word,exc); //excfps[pos.ident]);
			if (line!=null)
				endlp = line.IndexOf(' ');
		}
		public string next()
		{
			if (endlp>=0 && endlp+1<line.Length) 
			{
				beglp = endlp+1;
				while (beglp<line.Length && line[beglp]==' ')
					beglp++;
				endlp = beglp;
				while (endlp<line.Length && line[endlp]!=' ' && line[endlp]!='\n')
					endlp++;
				if (endlp!=beglp)
					return line.Substring(beglp,endlp-beglp);
			}
			endlp = -1;
			return null;
		}
	}
}
