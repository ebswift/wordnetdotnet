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
using System.Collections;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;

namespace Wnlib
{
	/// <summary>
	/// Summary description for SynSet.
	/// </summary>
    [Serializable]
	public class SynSet
	{
		/* directly correlates to classinit in util.cs */
		// This should remain a duplicated list to the one
		// in util.cs for performance reasons.  Using a hashtable
		// in loops will slow searches down.
		private const int ANTPTR =          1;	/* ! */
		private const int HYPERPTR =        2;	/* @ */
		private const int HYPOPTR =         3;	/* ~ */
		private const int ENTAILPTR =       4;	/* * */
		private const int SIMPTR =          5;	/* & */
		private const int ISMEMBERPTR = 	 6; /* #m */ // TDMS 11 JUL 2006 - Added new ident matcher
		private const int ISSTUFFPTR = 	 7; /* #s */ // TDMS 11 JUL 2006 - Added new ident matcher
		private const int ISPARTPTR = 		 8; /* #p */ // TDMS 11 JUL 2006 - Added new ident matcher
		private const int HASMEMBERPTR = 	 9; /* %m */ // TDMS 11 JUL 2006 - Added new ident matcher
		private const int HASSTUFFPTR = 	10; /* %s */ // TDMS 11 JUL 2006 - Added new ident matcher
		private const int HASPARTPTR = 	11; /* %p */ // TDMS 11 JUL 2006 - Added new ident matcher
		private const int MERONYM = 		12; /* % */ // TDMS 11 JUL 2006 - Added new ident matcher
		private const int HOLONYM = 		13; /* # */ // TDMS 11 JUL 2006 - Added new ident matcher
		private const int CAUSETO = 		14; /* > */ // TDMS 11 JUL 2006 - Added new ident matcher
		private const int PPLPTR = 		15; /* < */ // TDMS 11 JUL 2006 - Added new ident matcher
		private const int SEEALSOPTR = 	16; /* ^ */ // TDMS 11 JUL 2006 - Added new ident matcher
		private const int PERTPTR = 		17; /* \\ */ // TDMS 11 JUL 2006 - Added new ident matcher
		private const int ATTRIBUTE = 		18; /* = */ // TDMS 11 JUL 2006 - Added new ident matcher
		private const int VERBGROUP = 		19; /* $ */ // TDMS 11 JUL 2006 - Added new ident matcher
		private const int NOMINALIZATIONS = 20; /* + */ // TDMS 11 JUL 2006 - Added new ident matcher
		private const int CLASSIFICATION = 21; /* ; */ // TDMS 11 JUL 2006 - Added new ident matcher
		private const int CLASS =          22;	/* - */
		private const int SYNS = 			23; /* */ // TDMS 11 JUL 2006 - Added new ident matcher
		private const int FREQ = 			24; /* */ // TDMS 11 JUL 2006 - Added new ident matcher
		// TODO: FRAMES symbol is the same as NOMINALIZATIONS - check
		private const int FRAMES = 		25; /* + */ // TDMS 11 JUL 2006 - Added new ident matcher
		private const int COORDS = 		26; /* */ // TDMS 11 JUL 2006 - Added new ident matcher
		private const int RELATIVES = 		27; /* */ // TDMS 11 JUL 2006 - Added new ident matcher
		private const int HMERONYM = 		28; /* */ // TDMS 11 JUL 2006 - Added new ident matcher
		private const int HHOLONYM = 		29; /* */ // TDMS 11 JUL 2006 - Added new ident matcher
		private const int WNGREP = 		30; /* */ // TDMS 11 JUL 2006 - Added new ident matcher

		/* Additional searches, but not pointers.  */
		private const int LASTTYPE =	CLASS;
		private const int OVERVIEW	= (LASTTYPE + 9);
		private const int MAXSEARCH =       OVERVIEW;
		private const int CLASSIF_START =    (MAXSEARCH + 1);
		private const int CLASSIF_REGIONAL = (CLASSIF_START + 2);    // ;r
		private const int CLASSIF_END =     CLASSIF_REGIONAL;
		private const int CLASS_START =      (CLASSIF_END + 1);
		private const int CLASS_REGIONAL =   (CLASS_START + 2);      // -r
		private const int CLASS_END =       CLASS_REGIONAL;
		private const int INSTANCE =         (CLASS_END + 1);        // @i
		private const int INSTANCES =        (CLASS_END + 2);        // ~i

		public bool isDirty = false;

		public int hereiam;
		public int fnum;
		public PartOfSpeech pos;	/* part of speech */
		public Lexeme[] words;		/* words in synset */
		public int whichword=0;     /* 1.. of the words array */
		public AdjSynSetType sstype;
		public int sense; // "global" variable: will match search.sense-1 if this is nonzero
		//public ArrayList senses = null; // of SynSet (creates our hierarchy) - TDMS 6 Oct 2005
		public SynSetList senses = null; // of SynSet (creates our hierarchy) - TDMS 6 Oct 2005
		Search search;
		public Pointer[] ptrs;		/* number of pointers */
		public Pointer thisptr; // the current pointertype - TDMS 17 Nov 2005
		public ArrayList frames = new ArrayList(); /* of SynSetFrame */
		public string defn;		/* synset gloss (definition) */
		public AdjMarker adj_marker;

        public SynSet() { } // for serialization

		public SynSet (int off,PartOfSpeech p, string wd,Search sch,int sens)
		{
			pos = p;
			hereiam = off;
			search = sch;
			sense = sens;
			StreamReader f = WNDB.data(p);
			f.DiscardBufferedData();
			//f.BaseStream.Seek(off,SeekOrigin.Begin);
			f.BaseStream.Position = off;
			string rec = f.ReadLine();
			//string str;
			if (!rec.StartsWith(off.ToString("D8")))
			{
				Console.WriteLine("Error reading "+p.name+" file! "+off+": "+rec);
				WNDB.reopen(p);
				f = WNDB.data(p);
				f.DiscardBufferedData();
				//				f.BaseStream.Seek(off,SeekOrigin.Begin);
				f.BaseStream.Position = off;
				rec = f.ReadLine();
				//if (!rec.StartsWith(off.ToString("D8")))
				//	str = "";
				//else
				//	Console.WriteLine("Recovered...");
			}
			Parse(rec,pos,wd);
		}
		
		public SynSet (int off,PartOfSpeech p,string wd,SynSet fr) : this(off,p,wd,fr.search,fr.sense) {}
		public SynSet (Index idx,int sens,Search sch) : this(idx.offs[sens],idx.pos,idx.wd,sch,sens){ }
		public SynSet (int off,PartOfSpeech p,SynSet fr) : this(off,p,"",fr) {}

		void Parse(string s,PartOfSpeech fpos,string word)
		{
			int j;
			StrTok st = new StrTok(s);
			int off = int.Parse(st.next());
			fnum = int.Parse(st.next());
			string f = st.next();
			PartOfSpeech pos = PartOfSpeech.of(f);
			if (pos.clss=="SATELLITE")
				sstype = AdjSynSetType.IndirectAnt;
			int wcnt = int.Parse(st.next(),NumberStyles.HexNumber);
			words = new Lexeme[wcnt];
			for (j=0;j<wcnt;j++) 
			{
				words[j] = new Lexeme();
				words[j].word = st.next();
				words[j].uniq = int.Parse(st.next(),NumberStyles.HexNumber);

				// Thanh Dao 7 Nov 2005 - Added missing word sense values
				int ss = getsearchsense(j+1);
				words[j].wnsns = ss;

				if (words[j].word.ToLower()==word)
					whichword = j+1;
			}
			int pcnt = int.Parse(st.next());
			ptrs = new Pointer[pcnt];
			for (j=0;j<pcnt;j++)
			{
				string p = st.next();
				ptrs[j] = new Pointer(p);
				if (fpos.name=="adj" && sstype==AdjSynSetType.DontKnow) 
				{
					if (ptrs[j].ptp.ident == ANTPTR) // TDMS 11 JUL 2006 - change comparison to int //.mnemonic=="ANTPTR")
						sstype = AdjSynSetType.DirectAnt;
					else if (ptrs[j].ptp.ident==PERTPTR) // TDMS 11 JUL 2006 - change comparison to int //mnemonic=="PERTPTR")
						sstype = AdjSynSetType.Pertainym;
				}
				ptrs[j].off = int.Parse(st.next());
				ptrs[j].pos = PartOfSpeech.of(st.next());
				int sx = int.Parse(st.next(),NumberStyles.HexNumber);
				ptrs[j].sce = sx>>8;
				ptrs[j].dst = sx&0xff;
			}
			f = st.next();
			if (f!="|") 
			{
				int fcnt = int.Parse(f);
				for (j=0;j<fcnt;j++) 
				{
					f = st.next(); // +
					Frame fr = Frame.frame(int.Parse(st.next()));
					frames.Add(new SynSetFrame(fr,int.Parse(st.next(),NumberStyles.HexNumber)));
				}
				f = st.next();
			}
			defn = s.Substring(s.IndexOf('|')+1);
		}
		
		internal bool has(PointerType p)
		{
			for (int i=0;i<ptrs.Length;i++)
				if (ptrs[i].ptp==p)
					return true;
			return false;
		}
		
		internal void tracePtrs(PointerType ptp,PartOfSpeech p,int depth)
		{
			tracePtrs(new SearchType(false,ptp),p,depth);
		}

        /// <summary>
        /// Traces pointer hierarchy.
        /// </summary>
        /// <param name="stp"></param>
        /// <param name="fpos"></param>
        /// <param name="depth"></param>
		internal void tracePtrs(SearchType stp,PartOfSpeech fpos, int depth)
		{
			int i;
			SynSet cursyn;
			PointerType ptp = stp.ptp;
			string prefix;
			int realptr; // WN2.1
			
			for (i=0;i<ptrs.Length;i++)
			{
				Pointer pt = ptrs[i];
				// following if statement is WN2.1 - TDMS
				if ((ptp.ident == HYPERPTR && (pt.ptp.ident == HYPERPTR ||
					pt.ptp.ident == INSTANCE)) ||
					(ptp.ident == HYPOPTR && (pt.ptp.ident == HYPOPTR ||
					pt.ptp.ident == INSTANCES)) ||
					((pt.ptp == ptp) &&
					((pt.sce == 0) ||
					(pt.sce == whichword)))) 
					
				{
					realptr = pt.ptp.ident; /* WN2.1 deal with INSTANCE */
					if (!search.prflag) // print sense number and synset
						strsns(sense+1);
					search.prflag = true;
					spaces("TRACEP",depth+(stp.rec?2:0));
					//					switch (ptp.mnemonic) 
// TDMS 11 JUL 2006 - changed switch to ident	switch (pt.ptp.mnemonic) // TDMS - WN2.1 MOD
					switch (pt.ptp.ident) // TDMS 11 JUL 2006 - changed switch to ident
					{
						case PERTPTR:
							if (fpos.name=="adv") // TDMS "adverb")
								prefix = "Derived from "+pt.pos.name+" ";
							else
								prefix = "Pertains to "+pt.pos.name+" ";
							break;
						case ANTPTR: // TDMS 26/8/05
							if(fpos.name=="adj") //TODO: which adjective will fall into the below?
								prefix = "Antonym of ";
							else
								prefix = "";
							break;
						case PPLPTR:
							prefix = "Participle of verb";
							break;
						case INSTANCE:
							prefix = "INSTANCE OF=> ";
							break;
						case INSTANCES:
							prefix = "HAS INSTANCE=> ";
							break;
						case HASMEMBERPTR:
							prefix = "   HAS MEMBER: "; break;
						case HASSTUFFPTR:
							prefix = "   HAS SUBSTANCE: "; break;
						case HASPARTPTR:
							prefix = "   HAS PART:  "; break;
						case ISMEMBERPTR:
							prefix = "   MEMBER OF:  "; break;
						case ISSTUFFPTR: // TDMS 26/8/05
							prefix = "   SUBSTANCE OF: ";
							break;
						case ISPARTPTR: // TDMS 26/8/05
							prefix = "   PART OF: ";
							break;
						default:
							prefix = "=> "; break;
					}

					/* Read synset pointed to */
					cursyn = new SynSet(pt.off,pt.pos,this);
					search.wordsFrom(cursyn);

					// TDMS 6 Oct 2005 - build hierarchical results
					
					if(this.senses == null)
						this.senses = new SynSetList();
					cursyn.thisptr = pt;  // TDMS 17 Nov 2005 - add this pointer type
					this.senses.Add(cursyn);

					/* For Pertainyms and Participles pointing to a specific
					   sense, indicate the sense then retrieve the synset
					   pointed to and other info as determined by type.
					   Otherwise, just print the synset pointed to. */
					if ((ptp.ident==PERTPTR||ptp.ident==PPLPTR)&&
						pt.dst!=0) 
					{
						string tbuf = " (Sense "+cursyn.getsearchsense(pt.dst)+")";
						cursyn.str(prefix,tbuf,0,pt.dst,0,1);
						if (ptp.ident==PPLPTR) // adj pointing to verb
						{
							cursyn.str("     =>","\n",1,0,1,1);
							cursyn.tracePtrs(PointerType.of("HYPERPTR"),cursyn.pos,0);
						} 
						else if (fpos.name=="adverb") // adverb pointing to adjective
						{
							cursyn.str("     =>","\n",0,0,(pos.clss=="SATELLITE")?0:1,1);
							// cursyn.traceptrs(HYPERPTR,pos,0);
						} 
						else  // adjective pointing to noun
						{
							cursyn.str("     =>","\n",1,0,1,1);
							cursyn.tracePtrs(PointerType.of("HYPERPTR"),pos,0);
						}
					} 
					else
						cursyn.str(prefix,"\n",1,0,1,1);
					/* For HOLONYMS and MERONYMS, keep track of last one
					   printed in buffer so results can be truncated later. */
                    if (ptp.ident >= PointerType.of("ISMEMBERPTR").ident &&
                        ptp.ident <= PointerType.of("HASPARTPTR").ident)
                    {
                        search.mark();
                    }
					if (depth>0) 
					{
						depth = cursyn.depthcheck(depth);
						cursyn.tracePtrs(ptp,cursyn.pos,depth+1);
					}
				}
			}
		}

        /// <summary>
        /// Trace coordinate terms.
        /// </summary>
        /// <param name="ptp"></param>
        /// <param name="fpos"></param>
        /// <param name="depth"></param>
        internal void traceCoords(PointerType ptp,PartOfSpeech fpos,int depth)
		{
			for (int i=0;i<ptrs.Length;i++)
			{
				Pointer pt = ptrs[i];
				/* WN2.0
								if (pt.ptp.mnemonic=="HYPERPTR" &&
									(pt.sce==0 ||
									 pt.sce==whichword))
				*/
				// WN2.1 if statement change - TDMS
				if((pt.ptp.ident==HYPERPTR || pt.ptp.ident==INSTANCE) &&
					((pt.sce==0) ||
					(pt.sce==whichword)))
				{
					if(!search.prflag) 
					{
						strsns(sense + 1);
						search.prflag = true;
					}
					spaces("TRACEC",depth);
					SynSet cursyn = new SynSet(pt.off,pt.pos,this);
					search.wordsFrom(cursyn);
					cursyn.str("-> ","\n",1,0,0,1);
					cursyn.tracePtrs(ptp,cursyn.pos,depth);
					// TDMS 6 Oct 2005 - build hierarchical results
					if(this.senses == null)
						this.senses = new SynSetList();
					cursyn.thisptr = pt;  // TDMS 17 Nov 2005 - add this pointer type
					this.senses.Add(cursyn);

					if (depth>0) 
					{
						depth = depthcheck(depth);
						cursyn.traceCoords(ptp,cursyn.pos,depth+1);
						// TDMS 6 Oct 2005 - build hierarchical results
						// TODO: verify this
						if(this.senses == null)
							this.senses = new SynSetList();
						cursyn.thisptr = pt;  // TDMS 17 Nov 2005 - add this pointer type
						this.senses.Add(cursyn);
					}
				}
			}
		}
		
        /// <summary>
        /// Trace classification.
        /// </summary>
        /// <param name="ptp"></param>
        /// <param name="stp"></param>
		internal void traceclassif(PointerType ptp, SearchType stp) //,PartOfSpeech fpos)
		{
			int j;
			int idx = 0;
			//int svwnsnsflag;
			string head = "";
			int LASTTYPE = PointerType.of("CLASS").ident;
			int OVERVIEW = (LASTTYPE + 9);
			int MAXSEARCH = OVERVIEW;
			int CLASSIF_START = (MAXSEARCH + 1);
			int CLASSIF_CATEGORY = (CLASSIF_START);        /* ;c */
			int CLASSIF_USAGE = (CLASSIF_START + 1);    /* ;u */
			int CLASSIF_REGIONAL = (CLASSIF_START + 2);    /* ;r */
			int CLASSIF_END = CLASSIF_REGIONAL;
			int CLASS_START = (CLASSIF_END + 1);
			int CLASS_CATEGORY = (CLASS_START);          /* -c */
			int CLASS_USAGE = (CLASS_START + 1);      /* -u */
			int CLASS_REGIONAL = (CLASS_START + 2);      /* -r */
			int CLASS_END = CLASS_REGIONAL;
			//long prlist[1024];
			ArrayList prlist = new ArrayList();

			for (int i=0;i<ptrs.Length;i++)
			{
				Pointer pt = ptrs[i];
				if (((pt.ptp.ident >= CLASSIF_START) &&
					(pt.ptp.ident <= CLASSIF_END) && stp.ptp.ident == PointerType.of("CLASSIFICATION").ident) ||
	    
					((pt.ptp.ident >= CLASS_START) &&
					(pt.ptp.ident <= CLASS_END) && stp.ptp.ident == PointerType.of("CLASS").ident) ) 
				{
					if(!search.prflag) 
					{
						strsns(sense + 1);
						search.prflag = true;
					}
				
					SynSet cursyn = new SynSet(pt.off,pt.pos,this);
					// TDMS 6 Oct 2005 - build hierarchical results
					// TODO: verify this
					if(this.senses == null)
						this.senses = new SynSetList();
					cursyn.thisptr = pt;  // TDMS 17 Nov 2005 - add this pointer type
					this.senses.Add(cursyn);

					for (j = 0; j < idx; j++) 
					{
						if (pt.off == Convert.ToInt16(prlist[j])) 
						{
							break;
						}
					}

					if (j == idx) 
					{
						prlist.Add(pt.off);
						spaces("TRACEP",0);

						if (pt.ptp.ident == CLASSIF_CATEGORY)
							head = "TOPIC->("; // WN2.1 - TDMS
							//							head = "CATEGORY->(";
						else if (pt.ptp.ident == CLASSIF_USAGE)
							head = "USAGE->(";
						else if (pt.ptp.ident == CLASSIF_REGIONAL)
							head = "REGION->(";
						else if (pt.ptp.ident == CLASS_CATEGORY)
							head = "TOPIC_TERM->("; // WN2.1 - TDMS
							//							head = "CATEGORY_TERM->(";
						else if (pt.ptp.ident == CLASS_USAGE)
							head = "USAGE_TERM->(";
						else if (pt.ptp.ident == CLASS_REGIONAL)
							head = "REGION_TERM->(";

						head += pt.pos.name;
						head += ") ";

						//svwnsnsflag = wnsnsflag;
						//wnsnsflag = 1;

						//						printsynset(head, cursyn, "\n", DEFOFF, ALLWORDS,
						//							SKIP_ANTS, SKIP_MARKER);
						cursyn.str(head,"\n", 0,0,0,0);

						//wnsnsflag = svwnsnsflag;
					}
				}
			}
		}
		
        /// <summary>
        /// Trace nominalizations.
        /// </summary>
        /// <param name="ptp"></param>
		internal void tracenomins(PointerType ptp) //,PartOfSpeech fpos)
		{
/*
            int j;
			int idx = 0;
			ArrayList prlist = new ArrayList();
*/
			for (int i=0;i<ptrs.Length;i++)
			{
				Pointer pt = ptrs[i];
				// TDMS 26/8/05 changed DERIVATION to NOMINALIZATIONS - verify this
				if (pt.ptp.ident==NOMINALIZATIONS && (pt.sce==0 ||pt.sce==whichword)) 
				{
					if(!search.prflag) 
					{
						strsns(sense + 1);
						search.prflag = true;
					}
					spaces("TRACEP",0);
					SynSet cursyn = new SynSet(pt.off,pt.pos,this);
					search.wordsFrom(cursyn);
					cursyn.str("RELATED TO-> ","\n",0,0,0,0);
					// TDMS 6 Oct 2005 - build hierarchical results
					// TODO: verify this
					if(this.senses == null)
						this.senses = new SynSetList();
					cursyn.thisptr = pt;  // TDMS 17 Nov 2005 - add this pointer type
					this.senses.Add(cursyn);

                    cursyn.tracePtrs(ptp,cursyn.pos,0);
/*
                    for (j = 0; j < idx; j++) 
					{
						//#ifdef FOOP
						//						if (synptr->ptroff[i] == prlist[j]) 
						//						{
						//							break;
						//						}
						//#endif
					}

					if (j == idx) 
					{
						prlist.Add(pt.off);
						spaces("TRACEP",2);
						cursyn.str("=> ","\n",1,0,0,1);
					}
*/
				}
			}
		}

        /// <summary>
        /// Trace meronyms.
        /// </summary>
        /// <param name="pbase"></param>
        /// <param name="fpos"></param>
        /// <param name="depth"></param>
        void traceInherit(PointerType pbase,PartOfSpeech fpos,int depth)
		{
            for (int i = 0; i < ptrs.Length; i++) 
			{
				Pointer pt = ptrs[i];
				if (pt.ptp.ident==HYPERPTR && (pt.sce==0||pt.sce==whichword)) 
				{
					spaces("TRACEI",depth);
					SynSet cursyn = new SynSet(pt.off,pt.pos,this);
					search.wordsFrom(cursyn);
					cursyn.str("=> ","\n",1,0,0,1);
					// TDMS 6 Oct 2005 - build hierarchical results
					// TODO: verify this
					if(this.senses == null)
						this.senses = new SynSetList();
					cursyn.thisptr = pt;  // TDMS 17 Nov 2005 - add this pointer type
                    // TODO: This is adding senses incorrectly
					this.senses.Add(cursyn);

					cursyn.tracePtrs(pbase,PartOfSpeech.of("noun"),depth);
					cursyn.tracePtrs(pbase+1,PartOfSpeech.of("noun"),depth);
					cursyn.tracePtrs(pbase+2,PartOfSpeech.of("noun"),depth);
					if (depth>0) 
					{
						depth=depthcheck(depth);
						cursyn.traceInherit(pbase,cursyn.pos,depth+1);
					}
				}
			}
			search.trunc();
        }

        /// <summary>
        /// Trace adjective antonyms.
        /// </summary>
        internal void traceAdjAnt()
        {
            SynSet newsynptr;
            int i, j;
            AdjSynSetType anttype = AdjSynSetType.DirectAnt;
            SynSet simptr, antptr;
            string similar = "        => ";
            /* This search is only applicable for ADJ synsets which have
               either direct or indirect antonyms (not valid for pertainyms). */
            if (sstype == AdjSynSetType.DirectAnt || sstype == AdjSynSetType.IndirectAnt)
            {
                strsns(sense + 1);
                search.buf += "\n";
                /* if indirect, get cluster head */
                if (sstype == AdjSynSetType.IndirectAnt)
                {
                    anttype = AdjSynSetType.IndirectAnt;
                    i = 0;
                    while (ptrs[i].ptp.ident != SIMPTR)
                        i++;
                    newsynptr = new SynSet(ptrs[i].off, PartOfSpeech.of("adj"), this);
                }
                else
                    newsynptr = this;
                /* find antonyms - if direct, make sure that the antonym
                   ptr we're looking at is from this word */
                for (i = 0; i < newsynptr.ptrs.Length; i++)
                {
                    if (newsynptr.ptrs[i].ptp.ident == ANTPTR && // TDMS 11 JUL 2006 // mnemonic=="ANTPTR" &&
                        ((anttype == AdjSynSetType.DirectAnt &&
                        newsynptr.ptrs[i].sce == newsynptr.whichword) ||
                        anttype == AdjSynSetType.IndirectAnt))
                    {
                        /* read the antonym's synset and print it.  if a
                           direct antonym, print it's satellites. */
                        antptr = new SynSet(newsynptr.ptrs[i].off, PartOfSpeech.of("adj"), this);
                        search.wordsFrom(antptr);
                        // TDMS 6 Oct 2005 - build hierarchical results
                        if (this.senses == null)
                            this.senses = new SynSetList();
                        //TODO: check the ptrs reference
                        antptr.thisptr = newsynptr.ptrs[i];  // TDMS 17 Nov 2005 - add this pointer type
                        this.senses.Add(antptr);
                        if (anttype == AdjSynSetType.DirectAnt)
                        {
                            antptr.str("", "\n", 1, 0, 1, 1);
                            for (j = 0; j < antptr.ptrs.Length; j++)
                                if (antptr.ptrs[j].ptp.ident == SIMPTR) // TDMS 11 JUL 2006 - changed to INT //.mnemonic=="SIMPTR")
                                {
                                    simptr = new SynSet(antptr.ptrs[j].off, PartOfSpeech.of("adj"), this);
                                    search.wordsFrom(simptr);
                                    simptr.str(similar, "\n", 1, 0, 0, 1);
                                    // TDMS 6 Oct 2005 - build hierarchical results
                                    if (antptr.senses == null)
                                        antptr.senses = new SynSetList();
                                    antptr.senses.Add(simptr);
                                }
                        }
                        else
                            antptr.strAnt("\n", anttype, 1);
                    }
                }
            }
        }

        void spaces(string trace, int n)
		{
			for (int j=0;j<n;j++)
				search.buf += "     ";
			switch(trace) 
			{
				case "TRACEP": // traceptrs
					if (n>0)
						search.buf += "   ";
					else
						search.buf += "       ";
					break;
				case "TRACEC": // tracecoords
					if (n==0)
						search.buf += "    ";
					break;
				case "TRACEI": // traceinherit
					if (n==0)
						search.buf += "\n    ";
					break;
			}
		}

		public void Print(string head,string tail,int definition,int wdnum, int antflag, int markerflag)
		{
			string keep = search.buf;
			search.buf = "";
			str(head,tail,definition,wdnum,antflag,markerflag);
			Console.Write(search.buf);
			search.buf = keep;
		}
		
		internal void str(string head,string tail,int definition,int wdnum, int antflag, int markerflag)
		{
			int i, wdcnt;
			search.buf += head;
			/* Precede synset with additional information as indicated
			   by flags */
			if (WNOpt.opt("-o").flag)
				search.buf+="("+hereiam+") ";
			if (WNOpt.opt("-a").flag) 
			{
				search.buf+="<"+WNDB.lexfiles[fnum]+"> ";
				search.prlexid=true;
			} 
			else
				search.prlexid = false;
			if (wdnum>0)
				catword(wdnum-1,markerflag,antflag);
			else
				for (i=0,wdcnt=words.Length;i<wdcnt;i++) 
				{
					catword(i,markerflag,antflag);
					if (i<wdcnt-1)
						search.buf +=", ";
				}
            if (definition != 0 && WNOpt.opt("-g").flag && defn != null)
            {
                search.buf += " -- " + defn;

                isDirty = true; // TDMS 19 July 2006 - attempt to tie the logic which 
                // populates buf to the logic that defines whether the 
                // synset is populated with relevant information
            }
			
			search.buf += tail;
        }
		
		void strAnt(string tail,AdjSynSetType attype,int definition)
		{
			int i,wdcnt;
			bool first = true;
			if (WNOpt.opt("-o").flag)
				search.buf += "("+hereiam+") ";
			if (WNOpt.opt("-a").flag) 
			{
				search.buf += "<"+WNDB.lexfiles[fnum]+"> ";
				search.prlexid = true;
			}
			else
				search.prlexid = false;
			/* print antonyms from cluster head (of indirect ant) */
			search.buf += "INDIRECT (VIA ";
			for (i=0,wdcnt=words.Length;i<wdcnt;i++) 
			{
				if (first) 
				{
					strAnt(PartOfSpeech.of("adj"),i+1,"",", ");
					first = false;
				} 
				else
					strAnt(PartOfSpeech.of("adj"),i+1,", ",", ");
			}
			search.buf += ") -> ";
			/* now print synonyms from cluster head (of indirect ant) */
			for (i=0,wdcnt=words.Length;i<wdcnt;i++) 
			{
				catword(i,0,0);
				if (i<wdcnt-1)
					search.buf += ", ";
			}
            if (WNOpt.opt("-g").flag && defn != null && definition != 0)
            {
                search.buf += " -- " + defn;

                isDirty = true; // TDMS 19 July 2006 - attempt to tie the logic which 
                // populates buf to the logic that defines whether the 
                // synset is populated with relevant information
            }
			search.buf += tail;
		}
		
		void catword(int wdnum,int adjmarker,int antflag)
		{
			search.buf += deadjify(words[wdnum].word);
			/* Print additional lexicographer information and WordNet sense
			   number as indicated by flags */
            if (words[wdnum].uniq != 0)
            {
                search.buf += "" + words[wdnum].uniq;
                isDirty = true; // TDMS 19 July 2006 - attempt to tie the logic which 
                // populates buf to the logic that defines whether the 
                // synset is populated with relevant information
            }
			int s = getsearchsense(wdnum+1);
			words[wdnum].wnsns = s;
            if (WNOpt.opt("-s").flag)
            {
                search.buf += "#" + s;
                isDirty = true; // TDMS 19 July 2006 - attempt to tie the logic which 
                // populates buf to the logic that defines whether the 
                // synset is populated with relevant information
            }
			/* For adjectives, append adjective marker if present, and
			   print antonym if flag is passed */
			if (pos.name=="adj") 
			{
                if (adjmarker > 0)
                {
                    search.buf += "" + adj_marker.mark;
                    isDirty = true; // TDMS 19 July 2006 - attempt to tie the logic which 
                    // populates buf to the logic that defines whether the 
                    // synset is populated with relevant information
                }
                if (antflag > 0)
                    strAnt(PartOfSpeech.of("adj"), wdnum + 1, "(vs. ", ")");
			}
		}
		
		internal void strAnt(PartOfSpeech pos,int wdnum,string head,string tail) 
		{
			int i,j,wdoff;

			/* Go through all the pointers looking for anotnyms from the word
			   indicated by wdnum.  When found, print all the antonym's
			   antonym pointers which point back to wdnum. */
			for (i=0;i<ptrs.Length;i++) 
			{
				Pointer pt = ptrs[i];
				if (pt.ptp.ident==ANTPTR && pt.sce==wdnum) 
				{
					SynSet psyn = new SynSet(pt.off,pos,this);
					for (j=0;j<psyn.ptrs.Length;j++) 
					{
						Pointer ppt = psyn.ptrs[j];
						if (ppt.ptp.ident==ANTPTR &&
							ppt.dst==wdnum &&
							ppt.off==hereiam) 
						{
							wdoff = ppt.sce>0?ppt.sce-1:0;
							search.buf += head;
							/* Construct buffer containing formatted antonym,
							   then add it onto end of return buffer */
							search.buf += deadjify(psyn.words[wdoff].word);
							/* Print additional lexicographer information and
							   WordNet sense number as indicated by flags */
                            isDirty = true; // TDMS 19 July 2006 - attempt to tie the logic which 
                            // populates buf to the logic that defines whether the 
                            // synset is populated with relevant information

                            if (search.prlexid && psyn.words[wdoff].uniq != 0)
								search.buf += psyn.words[wdoff].uniq;
							int s = getsearchsense(wdoff+1);
							psyn.words[wdoff].wnsns = s;
                            if (WNOpt.opt("-s").flag)
                            {
                                search.buf += "#" + s;
                                isDirty = true; // TDMS 19 July 2006 - attempt to tie the logic which 
                                // populates buf to the logic that defines whether the 
                                // synset is populated with relevant information
                            }
							search.buf += tail;
						}
					}
				}
			}
		}
		
		string deadjify(string word)
		{
			string tmpword = word + " ";
			adj_marker = AdjMarker.of("UNKNOWN_MARKER");
			for (int j=0;j<word.Length;j++)
				if (word[j]=='(')
				{
					if (tmpword.Substring(j,3)=="(a)")
						adj_marker = AdjMarker.of("ATTRIBUTIVE");
					else if (tmpword.Substring(j,4)=="(ip)")
						adj_marker = AdjMarker.of("IMMED_POSTNOMINAL");
					else if (tmpword.Substring(j,3)=="(p)")
						adj_marker = AdjMarker.of("PREDICATIVE");
					return word.Substring(0,j);
				}
			return word;
		}
		
		internal void strsns(int sense)
		{
			strsense(sense);
			str("","\n",1,0,1,1);
		}
		
		void strsense(int sense)
		{
			/* Append lexicographer filename after Sense # if flag is set. */
			if (WNOpt.opt("-a").flag)
				search.buf += "\nSense "+sense+" in file \""+WNDB.lexfiles[fnum]+"\"\n";
			else
				search.buf += "\nSense "+sense+"\n";

            isDirty = true; // TDMS 19 July 2006 - attempt to tie the logic which 
            // populates buf to the logic that defines whether the 
            // synset is populated with relevant information
        }
		
		internal int depthcheck(int depth)
		{
			if (depth>=20)
				depth = -1;
			return depth;
		}
		
		internal void partsAll(PointerType ptp)
		{
			int hasptr = 0;
			PointerType ptrbase = PointerType.of((ptp.ident==HMERONYM)?"HASMEMBERPTR":"ISMEMBERPTR");
			/* First, print out the MEMBER, STUFF, PART info for this synset */
			for (int i=0;i<3;i++) 
				if (has(ptrbase+i)) 
				{
					tracePtrs(ptrbase+i,PartOfSpeech.of("noun"),i);
					hasptr++;
				}
			/* Print out MEMBER, STUFF, PART info for hypernyms on
	   HMERONYM search only */
			if (hasptr>0 && ptp.ident==HMERONYM) 
			{
				search.mark();

                traceInherit(ptrbase, PartOfSpeech.of("noun"), 1);
            }
		}
		
		internal void strFrame(bool prsynset)
		{
			int i;
			if(prsynset)
				strsns(sense+1);
			if (!findExample()) 
				for (i=0;i<frames.Count;i++) 
				{
					SynSetFrame sf = (SynSetFrame)frames[i];
					if (sf.to==whichword || sf.to==0) 
					{
						if (sf.to==whichword)
							search.buf += "          => ";
						else
							search.buf += "          *> ";
						search.buf += sf.fr.str + "\n";

                        isDirty = true; // TDMS 19 July 2006 - attempt to tie the logic which 
                        // populates buf to the logic that defines whether the 
                        // synset is populated with relevant information
                    }

				}
		}
		
		/* find the example sentence references in the example sentence index file */
		bool findExample()
		{
			bool retval = false;
            
			StreamReader fp = new StreamReader(WNDB.path+"SENTIDX.VRB");
            Byte[] b = System.Text.Encoding.Unicode.GetBytes(fp.ReadToEnd());

            MemoryStream ms = new MemoryStream(b);
            //StreamReader fp = new StreamReader(mms);

            int wdnum = whichword -1;
			Lexeme lx = words[wdnum];
			string tbuf = lx.word+"%"+pos.ident+":"+fnum+":"+lx.uniq+"::";
			string str = WNDB.binSearch(tbuf,ms); //fp);
			if (str!=null) 
			{
				str = str.Substring(lx.word.Length+11);
				StrTok st = new StrTok(str,' ',',','\n');
				string offset;
				while ((offset = st.next())!=null) 
				{
					getExample(offset,lx.word);
					retval = true;
				}
			}
			fp.Close();
			return retval;
		}
		
		void getExample(string off,string wd)
		{
			StreamReader fp = new StreamReader(WNDB.path+"SENTS.VRB");
            Byte[] b = System.Text.Encoding.Unicode.GetBytes(fp.ReadToEnd());
            MemoryStream ms = new MemoryStream(b);
			string line = WNDB.binSearch(off,ms); //fp);
			line = line.Substring(line.IndexOf(' ')+1);
			search.buf += "         EX: "+line.Replace("%s",wd);
			fp.Close();

            isDirty = true; // TDMS 19 July 2006 - attempt to tie the logic which 
            // populates buf to the logic that defines whether the 
            // synset is populated with relevant information
        }

		public int getsearchsense(int which)
		{
			int i;
			string wdbuf = words[which-1].word.Replace(' ','_').ToLower();
			Index idx = Index.lookup(wdbuf,pos);
			if (idx!=null)
				for (i=0;i<idx.offs.Length;i++)
					if (idx.offs[i]==hereiam) 
						return i+1;
			return 0;
		}

		internal void seealso()
		{
			/* Find all SEEALSO pointers from the searchword and print the
			   word or synset pointed to. */
			string prefix = "      Also See-> ";
			for (int i=0;i<ptrs.Length;i++)
			{
				Pointer p = ptrs[i];
				if (p.ptp.ident==SEEALSOPTR &&
					(p.sce==0 || (p.sce==whichword))) 
				{
					SynSet cursyn = new SynSet(p.off,p.pos,"",this);
					bool svwnsnsflag = WNOpt.opt("-s").flag;
					WNOpt.opt("-s").flag = true;
					cursyn.str(prefix,"",0,
						(p.dst==0)?0:p.dst,0,0);
					prefix = "; ";
				}
			}
		}
	}
}
