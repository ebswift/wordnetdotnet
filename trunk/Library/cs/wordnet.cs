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
using System.Globalization;
using System.Diagnostics;

namespace Wnlib
{
// C# interface to WordNet data
	// WordNet is from Princton University
	// this interface by Malcolm Crowe
	// update to latest WordNet version by Troy Simpson
	[Serializable]
	public class Search
	{
		public string word;
		public SearchType sch;
		public PartOfSpeech pos;
		public int whichsense;  // which sense to search for
		public Hashtable parts = null; // string->Search filled in by the two-parameter form of do_search
		public Hashtable morphs = null; // string->Search: filled in by do_search(true)
		//public ArrayList senses = null; // of SynSet: filled in by findtheinfo 
		public SynSetList senses = null; // of SynSet: filled in by findtheinfo 
		public ArrayList countSenses = null; // of int: filled in by findtheinfo for FREQ
		public ArrayList strings = null; // of string: filled in by findtheinfo for WNGREP
		public Hashtable lexemes = new Hashtable(); // results of search: Lexeme -> true
		public string buf = ""; // results of search as text string
		internal bool prflag = true;
		internal bool prlexid = false;
		public int taggedSenses;
		public const int ALLSENSES = 0;

		public Search(string w, bool doMorphs, string p, string s, int sn)
			:
			this(w, doMorphs, PartOfSpeech.of(p), new SearchType(s), sn) { }

		public Search(string w, bool doMorphs, PartOfSpeech p, SearchType s, int sn)
			: this(w, p, s, sn)
		{
			if (p != null)
				do_search(doMorphs);
		}

		internal Search(string w, PartOfSpeech p, SearchType s, int sn)
		{
			word = w;
			pos = p;
			sch = s;
			whichsense = sn;
		}

		int lastholomero = 0;

		internal void mark()
		{
			lastholomero = buf.Length;
		}

		internal void trunc()
		{
			buf = buf.Substring(0, lastholomero);
		}

		/// <summary>
		/// Performs a search based on the parameters setup
		/// in the Search constructor.
		/// </summary>
		/// <param name="m">Specify if morphs should be searched</param>
		/// <param name="p">The Part Of Speech to perform the search on</param>
		public void do_search(bool m, string p)
		{
			if (parts == null)
				parts = new Hashtable();
			Search s = new Search(word, PartOfSpeech.of(p), sch, whichsense);
			s.do_search(m);
			parts[p] = s;
			buf += s.buf;
		}

		/// <summary>
		/// Performs a search based on the parameters setup
		/// in the Search constructor.
		/// </summary>
		/// <param name="doMorphs">Specifies whether to perform a search on morphs.  
		/// This parameter is retrieved in the first
		/// call from do_search(bool m, string p).  If morph searching is specified
		/// and a morph is found, then on a recursive call to this method
		/// morphing will be turned off to prevent unnecessary morph searching.</param>
		internal void do_search(bool doMorphs)
		{
			findtheinfo();
			if (buf.Length > 0)
				buf = "\n" + sch.label + " of " + pos.name + " " + word + "\n" + buf;
			if (!doMorphs)
				return;
			morphs = new Hashtable();
			MorphStr st = new MorphStr(word, pos);
			string morphword;
			
			// if there are morphs then perform iterative searches
			// on each morph, filling the morph tree in the search
			// object.
			while ((morphword = st.next()) != null)
			{
				Search s = new Search(morphword, pos, sch, whichsense);
				s.do_search(false);
				// Fill the morphlist - eg. if verb relations of 'drunk' are requested, none are directly 
				// found, but the morph 'drink' will have results.  The morph hashtable will be populated 
				// into the search results and should be iterated instead of the returned synset if the 
				// morphs are non-empty
				morphs[morphword] = s;
				buf += s.buf;
			}
		}

		// From the WordNet Manual (http://wordnet.princeton.edu/man/wnsearch.3WN.html)
		// findtheinfo() is the primary search algorithm for use with database interface 
		// applications. Search results are automatically formatted, and a pointer to the 
		// text buffer is returned. All searches listed in WNHOME/include/wnconsts.h can be 
		// done by findtheinfo().
		void findtheinfo()
		{
			SynSet cursyn = null;
			Indexes ixs = new Indexes(word, pos);
			Index idx = null;
			int depth = sch.rec ? 1 : 0;
			senses = new SynSetList();
			switch (sch.ptp.mnemonic)
			{
				case "OVERVIEW":
					WNOverview();
					break;
				case "FREQ":
					if (countSenses == null)
						countSenses = new ArrayList();
					while ((idx = ixs.next()) != null)
					{
						countSenses.Add(idx.offs.Length);
						buf += "Sense " + countSenses.Count + ": " +
							idx.offs.Length;
					}
					break;
				case "WNGREP":
					strings = WNDB.wngrep(word, pos);
					for (int wi = 0; wi < strings.Count; wi++)
						buf += (string)strings[wi] + "\n";
					break;
				case "VERBGROUP":
					goto case "RELATIVES";
				case "RELATIVES":
					while ((idx = ixs.next()) != null)
						relatives(idx);
					break;
				default:
					/* look at all spellings of word */
					while ((idx = ixs.next()) != null)
					{
						/* Print extra sense msgs if looking at all senses */
						if (whichsense == ALLSENSES)
							buf += "\n";

						/* Go through all of the searchword's senses in the
						   database and perform the search requested. */
						for (int sense = 0; sense < idx.offs.Length; sense++)
							if (whichsense == ALLSENSES || whichsense == sense + 1)
							{
								prflag = false;
								/* Determine if this synset has already been done
								   with a different spelling. If so, skip it. */
								for (int j = 0; j < senses.Count; j++)
								{
									SynSet ss = (SynSet)senses[j];
									if (ss.hereiam == idx.offs[sense])
										goto skipit;
								}
								cursyn = new SynSet(idx, sense, this);

								//TODO: moved senses.add(cursyn) from here to each case and handled it differently according to search - this handling needs to be verified to ensure the filter is not to limiting
								switch (sch.ptp.mnemonic)
								{
									case "ANTPTR":
										if (pos.name == "adj")
											cursyn.traceAdjAnt();
										else
											cursyn.tracePtrs(sch.ptp, pos, depth);
                                        
                                        if (cursyn.isDirty)
                                        { // TDMS 25 Oct 2005 - restrict to relevant values
                                            cursyn.frames.Clear(); // TDMS 03 Jul 2006 - frames get added in wordnet.cs after filtering
                                            senses.Add(cursyn);
                                        }
                                        /*
                                                                                if (cursyn.senses != null )
                                                                                    if (cursyn.senses.isDirty)
                                                                                    { // TDMS 25 Oct 2005 - restrict to relevant values
                                                                                        cursyn.frames.Clear(); // TDMS 03 Jul 2006 - frames get added in wordnet.cs after filtering
                                                                                        senses.Add(cursyn);
                                                                                    }
                                        */
										// perform the senses restrictions based upon pos
										/*
																					switch(pos.name) {
																						case "verb":
																							if (cursyn.senses != null) // TDMS 25 Oct 2005 - restrict to relevant values
																								senses.Add(cursyn);
																							break;
													
																						default:
																							if (cursyn.senses != null && cursyn.sense != 0) // TDMS 25 Oct 2005 - restrict to relevant values
																								senses.Add(cursyn);
																							break;
																					}
										*/											
										break;
									case "COORDS":
										//eg. search for 'car', select Noun -> 'Coordinate Terms'
										cursyn.traceCoords(PointerType.of("HYPOPTR"), pos, depth);

                                        if (cursyn.isDirty) // TDMS 25 Oct 2005 - restrict to relevant values
                                            senses.Add(cursyn);
                                        /*
                                            if (cursyn.senses != null )
                                                if (cursyn.senses.isDirty) // TDMS 25 Oct 2005 - restrict to relevant values
                                                    senses.Add(cursyn);
    */
										break;
									case "FRAMES":
										//eg. search for 'right', select Verb -> 'Sample Sentences'
										cursyn.strFrame(true);
// TDMS 03 JUL 2006 fixed relevancy check										if (cursyn.sense != 0) // TDMS 25 Oct 2005 - restrict to relevant values
                                        if (cursyn.isDirty)
                                            senses.Add(cursyn);

                                    /*
                                        if (cursyn.frames.Count != 0) // TDMS 03 Jul 2006 - only add frame if there are any retrieved
                                            senses.Add(cursyn);
*/
										break;
									case "MERONYM":
										//eg. search for 'car', select Noun -> 'Meronym'
										senses.isDirty = false;
										cursyn.tracePtrs(PointerType.of("HASMEMBERPTR"), pos, depth);
										cursyn.tracePtrs(PointerType.of("HASSTUFFPTR"), pos, depth);
										cursyn.tracePtrs(PointerType.of("HASPARTPTR"), pos, depth);

                                        if (cursyn.isDirty) // TDMS 25 Oct 2005 - restrict to relevant values
                                            senses.Add(cursyn);

                                    /*
                                        if (cursyn.senses != null )
											if (cursyn.senses.isDirty) // TDMS 25 Oct 2005 - restrict to relevant values
												senses.Add(cursyn);
 */
										break;
									case "HOLONYM":
										//eg. search for 'car', select Noun -> 'Holonyms'
										cursyn.tracePtrs(PointerType.of("ISMEMBERPTR"), pos, depth);
										cursyn.tracePtrs(PointerType.of("ISSTUFFPTR"), pos, depth);
										cursyn.tracePtrs(PointerType.of("ISPARTPTR"), pos, depth);
										//											if (cursyn.senses != null && cursyn.sense != 0) // TDMS 25 Oct 2005 - restrict to relevant values
										//												senses.Add(cursyn);
                                        if (cursyn.isDirty) // TDMS 25 Oct 2005 - restrict to relevant values
                                            senses.Add(cursyn);
                                        /*
                                                                                if (cursyn.senses != null )
                                                                                    if (cursyn.senses.isDirty) // TDMS 25 Oct 2005 - restrict to relevant values
                                                                                        senses.Add(cursyn);
                                         */
										break;
									case "HMERONYM":
										//eg. search for 'car', select Noun -> 'Meronyms Tree'
										cursyn.partsAll(sch.ptp);
                                        if (cursyn.isDirty) // TDMS 25 Oct 2005 - restrict to relevant values
                                            senses.Add(cursyn);
//                                            senses.Add(SearchTrack.ssParent);
                                        /*
										if (cursyn.senses != null )
											if (cursyn.senses.isDirty) // TDMS 25 Oct 2005 - restrict to relevant values
												senses.Add(cursyn);
                                         */
										//											if (cursyn.sense != 0) // TDMS 25 Oct 2005 - restrict to relevant values
										//												senses.Add(cursyn);
										break;
									case "HHOLONYM":
										cursyn.partsAll(sch.ptp);
                                        if (cursyn.isDirty) // TDMS 25 Oct 2005 - restrict to relevant values
                                            senses.Add(cursyn);
                                        /*
                                        if (cursyn.senses != null) // && cursyn.sense != 0) // TDMS 25 Oct 2005 - restrict to relevant values
											senses.Add(cursyn);
                                         */
										break;
									case "SEEALSOPTR":
										cursyn.seealso();
                                        if (cursyn.isDirty) // TDMS 25 Oct 2005 - restrict to relevant values
                                            senses.Add(cursyn);
                                        /*
										if (cursyn.sense != 0) // TDMS 25 Oct 2005 - restrict to relevant values
											senses.Add(cursyn);
                                         */
										break;
									case "SIMPTR":
										goto case "HYPERPTR";
									case "SYNS":
										goto case "HYPERPTR";
									case "HYPERPTR":
										//eg. search for 'car', select Noun -> 'Synonyms/Hypernyms, ordered by estimated frequency'
										wordsFrom(cursyn);
										cursyn.strsns(sense + 1);
										prflag = true;
										cursyn.tracePtrs(sch.ptp, pos, depth);
										if (pos.name == "adj")
										{
											cursyn.tracePtrs(PointerType.of("PERTPTR"), pos, depth);
											cursyn.tracePtrs(PointerType.of("PPLPTR"), pos, depth);
										}
										else if (pos.name == "adv")
											cursyn.tracePtrs(PointerType.of("PERTPTR"), pos, depth);
										if (pos.name == "verb")
											cursyn.strFrame(false);

                                        if (cursyn.isDirty) // TDMS 25 Oct 2005 - restrict to relevant values
                                            senses.Add(cursyn);
                                        //												senses.Add(cursyn);
										break;
									case "NOMINALIZATIONS": // 26/8/05 - changed "DERIVATION" to "NOMINALIZATIONS" - this needs to be verified
										// derivation - TDMS
										cursyn.tracenomins(sch.ptp);
                                        if (cursyn.isDirty) // TDMS 25 Oct 2005 - restrict to relevant values
                                            senses.Add(cursyn);
                                        /*
                                                                                if (cursyn.sense != 0) // TDMS 25 Oct 2005 - restrict to relevant values
                                                                                    senses.Add(cursyn);
                                         */
										break;
									case "CLASSIFICATION":
										goto case "CLASS";
									case "CLASS":
										//eg. search for 'car', select Noun -> 'Domain Terms'
										cursyn.traceclassif(sch.ptp, new SearchType(false, sch.ptp));
                                        if (cursyn.isDirty) // TDMS 25 Oct 2005 - restrict to relevant values
                                            senses.Add(cursyn);
                                        /*
                                        if (cursyn.senses != null )
											if (cursyn.senses.isDirty) // TDMS 25 Oct 2005 - restrict to relevant values
												senses.Add(cursyn);
                                         */
										break;
											
									case "HYPOPTR":
										//eg. search for 'car', select Noun -> 'Hyponyms'
										cursyn.tracePtrs(sch.ptp, pos, depth);
                                        if (cursyn.isDirty) // TDMS 25 Oct 2005 - restrict to relevant values
                                            senses.Add(cursyn);
                                        /*
                                                                                if (cursyn.senses != null )
                                                                                    if (cursyn.senses.isDirty)
                                                                                    { // TDMS 25 Oct 2005 - restrict to relevant values
                                                                                        cursyn.frames.Clear(); // TDMS 03 Jul 2006 - frames get added in wordnet.cs after filtering
                                                                                        senses.Add(cursyn);
                                                                                    }
                                         */
										break;
											
									default:
										cursyn.tracePtrs(sch.ptp, pos, depth);
                                        if (cursyn.isDirty) // TDMS 25 Oct 2005 - restrict to relevant values
                                            senses.Add(cursyn);
                                        /*
                                                                                if (cursyn.senses != null )
                                                                                    if (cursyn.senses.isDirty) // TDMS 25 Oct 2005 - restrict to relevant values
                                                                                        senses.Add(cursyn);
                                         */
										break;
								}
							skipit: ;
							}
					}
					break;
			}
		}

		public void wordsFrom(SynSet s)
		{
			for (int j = 0; j < s.words.Length; j++)
			{
				Lexeme lx = s.words[j];
				//				lx.wnsns = s.sense+1;
				lexemes[lx] = true;
			}
		}

		// TDMS - relatives - synonyms of verb - grouped by similarity of meaning
		void relatives(Index idx)
		{
			RelList rellist = null;
			switch (pos.name)
			{
				case "verb":
					rellist = findVerbGroups(idx, rellist);
					doRelList(idx, rellist);
					break;
			}
		}

		RelList findVerbGroups(Index idx, RelList rellist)
		{
			int i, j, k;
			/* Read all senses */
			for (i = 0; i < idx.offs.Length; i++)
			{
				SynSet synset = new SynSet(idx.offs[i], pos, idx.wd, this, i);
				/* Look for VERBGROUP ptr(s) for this sense.  If found,
				   create group for senses, or add to existing group. */
				for (j = 0; j < synset.ptrs.Length; j++)
				{
					Pointer p = synset.ptrs[j];
					if (p.ptp.mnemonic == "VERBGROUP")
						/* Need to find sense number for ptr offset */
						for (k = 0; k < idx.offs.Length; k++)
							if (p.off == idx.offs[k])
							{
								rellist = addRelatives(idx, i, k, rellist);
								break;
							}
				}
			}
			return rellist;
		}

		RelList addRelatives(Index idx, int rel1, int rel2, RelList rellist)
		{
			/* If either of the new relatives are already in a relative group,
			   then add the other to the existing group (transitivity).
				   Otherwise create a new group and add these 2 senses to it. */
			RelList rel, last = null;
			for (rel = rellist; rel != null; rel = rel.next)
			{
				if (rel.senses[rel1] || rel.senses[rel2])
				{
					rel.senses[rel1] = rel.senses[rel2] = true;
					/* If part of another relative group, merge the groups */
					for (RelList r = rellist; r != null; r = r.next)
						if (r != rel && r.senses[rel1] || r.senses[rel2])
							rel.senses = rel.senses.Or(r.senses);
					return rellist;
				}
				last = rel;
			}
			rel = new RelList();
			rel.senses[rel1] = rel.senses[rel2] = true;
			if (rellist == null)
				return rel;
			last.next = rel;
			return rellist;
		}

		void doRelList(Index idx, RelList rellist)
		{
			int i;
			bool flag;
			SynSet synptr;
			BitSet outsenses = new BitSet(300);
			prflag = true;
			for (RelList rel = rellist; rel != null; rel = rel.next)
			{
				flag = false;
				for (i = 0; i < idx.offs.Length; i++)
					if (rel.senses[i] && !outsenses[i])
					{
						flag = true;
						synptr = new SynSet(idx.offs[i], pos, "", this, i);
						synptr.strsns(i + 1);
						synptr.tracePtrs(PointerType.of("HYPERPTR"), pos, 0);
                        synptr.frames.Clear(); // TDMS 03 Jul 2006 - frames get added in wordnet.cs after filtering
                        // TDMS 11 Oct 2005 - build hierarchical results
						senses.Add(synptr);
						outsenses[i] = true;
					}
				if (flag)
					buf += "--------------\n";
			}
			for (i = 0; i < idx.offs.Length; i++)
				if (!outsenses[i])
				{
					synptr = new SynSet(idx.offs[i], pos, "", this, i);
					synptr.strsns(i + 1);
					synptr.tracePtrs(PointerType.of("HYPERPTR"), pos, 0);
                    synptr.frames.Clear(); // TDMS 03 Jul 2006 - frames get added in wordnet.cs after filtering
                    // TDMS 11 Oct 2005 - build hierarchical results
					senses.Add(synptr);
					buf += "---------------\n";
				}
		}

		void WNOverview()
		{
			Index idx;
			//senses = new ArrayList();
			senses = new SynSetList();
			Indexes ixs = new Indexes(word, pos);
			while ((idx = ixs.next()) != null)
			{
				buf += "\n";
				/* Print synset for each sense.  If requested, precede
				   synset with synset offset and/or lexical file information.*/
				for (int sens = 0; sens < idx.offs.Length; sens++)
				{
					for (int j = 0; j < senses.Count; j++)
					{
						SynSet ss = (SynSet)senses[j];
						if (ss.hereiam == idx.offs[sens])
							goto skipit;

					}
					SynSet cursyn = new SynSet(idx, sens, this);

					bool svdflag = WNOpt.opt("-g").flag;
					WNOpt.opt("-g").flag = true;
					bool svaflag = WNOpt.opt("-a").flag;
					WNOpt.opt("-a").flag = WNOpt.opt("-A").flag;
					bool svoflag = WNOpt.opt("-o").flag;
					WNOpt.opt("-o").flag = WNOpt.opt("-O").flag;

					cursyn.str("" + (sens + 1) + ". ", "\n", 1, 0, 0, 0);

					WNOpt.opt("-g").flag = svdflag;
					WNOpt.opt("-a").flag = svaflag;
					WNOpt.opt("-o").flag = svoflag;
					wordsFrom(cursyn);
                    cursyn.frames.Clear(); // TDMS 03 Jul 2006 - frames get added in wordnet.cs after filtering
					senses.Add(cursyn);
				skipit: ;
				}
				/* Print sense summary message */
				if (senses.Count > 0)
				{
					taggedSenses = 0;

					if (senses.Count == 1)
						buf += "\nThe " + pos.name + " " + idx.wd + " has 1 sense";
					else
						buf += "\nThe " + pos.name + " " + idx.wd + " has " + senses.Count + " senses";
					if (idx.tagsense_cnt > 0)
					{
						taggedSenses = idx.tagsense_cnt;
						buf += " (first " + idx.tagsense_cnt + " from tagged texts)\n";
					}
					else
						buf += " (no senses from tagged texts)\n";
				}
			}
		}
	}

	internal class RelList
	{
		public BitSet senses = new BitSet(300);
		public RelList next = null;
		public RelList() { }
		public RelList(RelList n) { next = n; }
	}

	public class SynSetFrame
	{
		public Frame fr;
		public int to;
		internal SynSetFrame(Frame f, int t) { fr = f; to = t; }
	}

	public enum AdjSynSetType { DontKnow, DirectAnt, IndirectAnt, Pertainym }

	[Serializable]
	public class AdjMarker
	{
		static Hashtable marks = new Hashtable();
		public string mnem;
		public string mark;

        AdjMarker() {
            // empty constructor for serialization
        }

		public static AdjMarker of(string s)
		{
			return (AdjMarker)marks[s];
		}

		AdjMarker(string n, string k)
		{
			mnem = n; mark = k;
			marks[n] = this;
		}

		static AdjMarker()
		{
			new AdjMarker("UNKNOWN_MARKER", "");
			new AdjMarker("ATTRIBUTIVE", "(prenominal)");
			new AdjMarker("IMMED_POSTNOMINAL", "(postnominal)");
			new AdjMarker("PREDICATIVE", "(predicate)");
		}
	}
}
