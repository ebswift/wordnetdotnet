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
using System.Reflection;
using System.Windows.Forms;

namespace Wnlib
{
	internal class WNDBpart
	{
		static Hashtable fps = new Hashtable();
		public StreamReader index;
		public StreamReader data;
		public WNDBpart(PartOfSpeech p)
		{
			try 
			{
				index = new StreamReader(WNDB.IndexFile(p));
				data = new StreamReader(WNDB.DataFile(p));
				fps[p] = this;
			} 
			catch 
			{
				// TODO: handle bad dict path
				// ignore errors - as the user is locating the dictionary location
				// wordnet classes are trying to instantiate based on an incorrect dict path
			}
		}
		public static WNDBpart of(PartOfSpeech p)
		{
			return (WNDBpart)fps[p];
		}
	}
	public class WNOpt
	{
		static Hashtable opts = new Hashtable();
		public static WNOpt opt(string a) 
		{ 
			return (WNOpt)(opts[a]); 
		}
		public string arg;
		public string help;
		public bool flag;
		WNOpt(string a, string h, bool i) 
		{
			arg = a;
			help = h;
			flag = i;
			opts.Add(a,this);
		}
		static WNOpt() 
		{
			new WNOpt("-l","license and copyright",false);
			new WNOpt("-h","help text on each search",false);
			new WNOpt("-g","gloss",true);
			new WNOpt("-a","lexicographer file information",false);
			new WNOpt("-o","synset offset",false);
			new WNOpt("-s","sense numbers",false);
			new WNOpt("-A","lexicog file info for overview",false);
			new WNOpt("-O","synset offset for overview",false);
			new WNOpt("-S","sense numbers for overview",false);
		}
	}
	public class Opt
	{
		public string arg;
		public SearchType sch;
		public PartOfSpeech pos;
		public int helpx;
		public string label;
		public int id;
		public static int Count { get { return opts.Count; }}
		public static Opt at(int ix) { return (Opt)opts[ix]; }
		static ArrayList opts = new ArrayList();
		Opt(string a,string m,string p,int h,string b) 
		{
			arg = a;
			if (m[0]=='-') 
				sch = new SearchType(true,m.Substring(1));
			else
				sch = new SearchType(false,m);
			pos = PartOfSpeech.of(p.ToLower());
			helpx = h;
			label = b;
			id = opts.Count;
			opts.Add(this);
		}

		static Opt() 
		{
			new Opt( "-synsa", "SIMPTR",	"ADJ", 0, "Similarity" );//0
			new Opt( "-antsa", "ANTPTR",	"ADJ", 1, "Antonyms" );//1
			new Opt( "-perta", "PERTPTR", "ADJ", 0, "Pertainyms" );//2
			new Opt( "-attra", "ATTRIBUTE", "ADJ", 2, "Attributes" );//3
			new Opt( "-domna", "CLASSIFICATION", "ADJ", 3, "Domain" );//4
			new Opt( "-domta", "CLASS", "ADJ", 4, "Domain Terms" );//5
			new Opt( "-famla", "FREQ", "ADJ", 5, "Familiarity" );//6
			new Opt( "-grepa", "WNGREP", "ADJ", 6, "Grep" );//7

			new Opt( "-synsn", "HYPERPTR", "NOUN", 0, "Synonyms/Hypernyms (Ordered by Estimated Frequency): brief" );//8
			// WN1.6			new Opt( "-simsn", "RELATIVES", "NOUN", 1, "Synonyms (Grouped by Similarity of Meaning)" );
			new Opt( "-antsn", "ANTPTR",	"NOUN", 2, "Antonyms" );//9
			new Opt( "-coorn", "COORDS", "NOUN", 3, "Coordinate Terms (sisters)" );//10
			new Opt( "-hypen", "-HYPERPTR", "NOUN", 4, "Synonyms/Hypernyms (Ordered by Estimated Frequency): full" );//11
			new Opt( "-hypon", "HYPOPTR", "NOUN", 5, "Hyponyms" );//12
			new Opt( "-treen", "-HYPOPTR", "NOUN", 6, "Hyponyms Tree" );//13
			new Opt( "-holon", "HOLONYM", "NOUN", 7, "Holonyms" );//14
			new Opt( "-sprtn", "ISPARTPTR", "NOUN", 7, "Part Holonyms" );//15
			new Opt( "-smemn", "ISMEMBERPTR", "NOUN", 7, "Member Holonyms" );//16
			new Opt( "-ssubn", "ISSTUFFPTR", "NOUN", 7, "Substance Holonyms" );//17
			new Opt( "-hholn",	"-HHOLONYM", "NOUN", 8, "Holonyms Tree" );//18
			new Opt( "-meron", "MERONYM", "NOUN", 9, "Meronyms" );//19
			new Opt( "-subsn", "HASSTUFFPTR", "NOUN", 9, "Substance Meronyms" );//20
			new Opt( "-partn", "HASPARTPTR", "NOUN", 9, "Part Meronyms" );//21
			new Opt( "-membn", "HASMEMBERPTR", "NOUN", 9, "Member Meronyms" );//22
			new Opt( "-hmern", "-HMERONYM", "NOUN", 10, "Meronyms Tree" );//23

			// TDMS 26/8/05 - this has been modified inline with WordNet 2.1, however it is not verified
			// the derin and nomn codes need to be verified as being identified correctly
			new Opt( "-nomnn", "NOMINALIZATIONS", "NOUN", 11, "Derived Forms" ); //24// modified TDMS 26/8/05 - derivation->nominalization
			new Opt( "-derin", "NOMINALIZATIONS", "NOUN", 11, "Derived Forms" ); //25// modified TDMS 26/8/05 - derivation->nominalization
			//new Opt( "-nomnn", "DERIVATION", "NOUN", 11, "Derived Forms" ); // TDMS 26/8/05 - replaced by above
			//new Opt( "-derin", "DERIVATION", "NOUN", 11, "Derived Forms" ); // TDMS 26/8/05 - replaced by above

			new Opt( "-domnn", "CLASSIFICATION", "NOUN", 13, "Domain" );//26
			new Opt( "-domtn", "CLASS", "NOUN", 14, "Domain Terms" );//27
			new Opt( "-attrn", "ATTRIBUTE", "NOUN", 12, "Attributes" );//28
			new Opt( "-famln", "FREQ", "NOUN", 15, "Familiarity" );//29
			new Opt( "-grepn", "WNGREP", "NOUN", 16, "Grep" );//30

			new Opt( "-synsv", "HYPERPTR", "VERB", 0, "Synonyms/Hypernyms (Ordered by Estimated Frequency): brief" );//31
			new Opt( "-simsv", "RELATIVES", "VERB", 1, "Synonyms (Grouped by Similarity of Meaning)" );//32
			new Opt( "-antsv", "ANTPTR", "VERB", 2, "Antonyms" );//33
			new Opt( "-coorv", "COORDS", "VERB", 3, "Coordinate Terms (sisters)" );//34
			new Opt( "-hypev", "-HYPERPTR", "VERB", 4, "Synonyms/Hypernyms (Ordered by Estimated Frequency): full" );//35
			new Opt( "-hypov", "HYPOPTR", "VERB", 5, "Troponyms (hyponyms)" );//36
			new Opt( "-treev", "-HYPOPTR", "VERB", 5, "Troponyms (hyponyms)" );//37
			new Opt( "-tropv", "-HYPOPTR", "VERB", 5, "Troponyms (hyponyms)" );//38
			new Opt( "-entav", "ENTAILPTR", "VERB", 6, "Entailment" );//39
			new Opt( "-causv", "CAUSETO", "VERB", 7, "\'Cause To\'" );

			// TDMS 26/8/05 - this has been modified inline with WordNet 2.1, however it is not verified
			// the nomnv and deriv codes need to be verified as being identified correctly
			new Opt( "-nomnv", "NOMINALIZATIONS", "VERB", 8, "Derived Forms" ); // TDMS 26/8/05 - changed derivation to nominalizations
			new Opt( "-deriv", "NOMINALIZATIONS", "VERB", 8, "Derived Forms" ); // TDMS 26/8/05 - changed derivation to nominalizations
			//			new Opt( "-nomnv", "DERIVATION", "VERB", 8, "Derived Forms" );  // TDMS 26/8/05 - replaced by above
			//			new Opt( "-deriv", "DERIVATION", "VERB", 8, "Derived Forms" );  // TDMS 26/8/05 - replaced by above

			new Opt( "-domnv", "CLASSIFICATION", "VERB", 10, "Domain" );
			new Opt( "-domtv", "CLASS", "VERB", 11, "Domain Terms" );
			new Opt( "-framv", "FRAMES", "VERB", 9, "Sample Sentences" );
			new Opt( "-famlv", "FREQ", "VERB", 12, "Familiarity" );
			new Opt( "-grepv", "WNGREP", "VERB", 13, "Grep" );

			new Opt( "-synsr", "SYNS", "ADV", 0, "Synonyms" );
			new Opt( "-antsr", "ANTPTR", "ADV", 1, "Antonyms" );
			new Opt( "-pertr", "PERTPTR", "ADV", 0, "Pertainyms" );
			new Opt( "-domnr", "CLASSIFICATION", "ADV", 2, "Domain" );
			new Opt( "-domtr", "CLASS", "ADV", 3, "Domain Terms" );
			new Opt( "-famlr", "FREQ", "ADV", 4, "Familiarity" );
			new Opt( "-grepr", "WNGREP", "ADV", 5, "Grep" );

			new Opt( "-over", "OVERVIEW", "ALL_POS", -1, "Overview" );
		}
	}
	// TDMS - path helper for the library - must be set before any call to WNDB static members
	public class WNCommon
	{
		// TODO: (TDMS) make this a property so that the database path can be dynamically checked when set.
		public static string path;
	}
	public class WNDB
	{
		public static string path; // set from WNCommon FIRST

		static WNDB()
		{
			path=WNCommon.path;
			IDictionaryEnumerator d = PartOfSpeech.parts.GetEnumerator();
			while (d.MoveNext()) 
				new WNDBpart((PartOfSpeech)(d.Value));
		}
		public static StreamReader index(PartOfSpeech p) 
		{
			return WNDBpart.of(p).index;
		}
		public static StreamReader data(PartOfSpeech p) 
		{
			return WNDBpart.of(p).data;
		}
		public static void reopen(PartOfSpeech p)
		{
			WNDBpart w = WNDBpart.of(p);
			w.index.Close();
			w.data.Close();
			w.index = new StreamReader(IndexFile(p));
			w.data = new StreamReader(DataFile(p));
		}
		public static string[] lexfiles = 
		{
			"adj.all",			/* 0 */
			"adj.pert",			/* 1 */
			"adv.all",			/* 2 */
			"noun.Tops",		/* 3 */
			"noun.act",			/* 4 */
			"noun.animal",		/* 5 */
			"noun.artifact",		/* 6 */
			"noun.attribute",		/* 7 */
			"noun.body",		/* 8 */
			"noun.cognition",		/* 9 */
			"noun.communication",	/* 10 */
			"noun.event",		/* 11 */
			"noun.feeling",		/* 12 */
			"noun.food",		/* 13 */
			"noun.group",		/* 14 */
			"noun.location",		/* 15 */
			"noun.motive",		/* 16 */
			"noun.object",		/* 17 */
			"noun.person",		/* 18 */
			"noun.phenomenon",		/* 19 */
			"noun.plant",		/* 20 */
			"noun.possession",		/* 21 */
			"noun.process",		/* 22 */
			"noun.quantity",		/* 23 */
			"noun.relation",		/* 24 */
			"noun.shape",		/* 25 */
			"noun.state",		/* 26 */
			"noun.substance",		/* 27 */
			"noun.time",		/* 28 */
			"verb.body",		/* 29 */
			"verb.change",		/* 30 */
			"verb.cognition",		/* 31 */
			"verb.communication",	/* 32 */
			"verb.competition",		/* 33 */
			"verb.consumption",		/* 34 */
			"verb.contact",		/* 35 */
			"verb.creation",		/* 36 */
			"verb.emotion",		/* 37 */
			"verb.motion",		/* 38 */
			"verb.perception",		/* 39 */
			"verb.possession",		/* 40 */
			"verb.social",		/* 41 */
			"verb.stative",		/* 42 */
			"verb.weather",		/* 43 */
			"adj.ppl",			/* 44 */
		};
		internal static string ExcFile(PartOfSpeech n)
		{
			return path+n.name+".exc";
		}
		internal static string IndexFile(PartOfSpeech n)
		{
			return path+"index."+n.name; // WN2.1 - TDMS
		}
		internal static string DataFile(PartOfSpeech n)
		{
			return path+"data."+n.name; // WN2.1 - TDMS
		}
		public static string binSearch(string searchKey,StreamReader fp)
		{
			int c,n;
			long top,bot,mid,diff;
			string line,key;
			diff = 666; // ???
			line = "";
			bot = fp.BaseStream.Seek(0,SeekOrigin.End);
			top = 0;
			mid = (bot-top)/2;
			do 
			{
				fp.DiscardBufferedData();
				//				fp.BaseStream.Seek(mid-1,SeekOrigin.Begin);
				fp.BaseStream.Position = mid-1;

				if (mid!=1)
					while ((c=fp.Read())!='\n' && c!=-1)
						;
				//				fp.DiscardBufferedData();
				line = fp.ReadLine();
				if (line==null)
					return null;
				n = line.IndexOf(' ');
				key = line.Substring(0,n);
				// TDMS - 14/Aug/2005 - no need for key
				// transformation - that is done in index lookup
				//key=key.Replace("-"," ").Replace("_"," ");
				//TDMS - 13/Aug/2005 fixed comparison
				//an example of the previous problem was a search for 'mammal'.
				//'mamma's boy' was rated as greater than mammal, however it is
				//stored logically in the WordNet database as less than.
				//CompareOrdinal correctly returns less than in that scenario
				//rather than greater than.
				//if (key.CompareTo(searchKey)<0) 
				if (String.CompareOrdinal(key, searchKey)<0) 
				{
					// key is alphabetically less than the search key
					top = mid;
					diff = (bot - top)/2;
					mid = top + diff;
				}
				//TDMS - 13/Aug/2005 fixed comparison
				//if (key.CompareTo(searchKey)>0)
				if (String.CompareOrdinal(key, searchKey)>0) 
				{
					// key is alphabetically greater than the search key
					bot = mid;
					diff = (bot - top)/2;
					mid = top + diff;
				}
			} while (key!=searchKey && diff!=0);
			if (key==searchKey)
				return line;
			return null;
		}
		public static string binSearch(string word,PartOfSpeech pos)
		{
			return binSearch(word,WNDB.index(pos));
		}
		public static SearchSet is_defined(string word,string p)
		{
			return is_defined(word,PartOfSpeech.of(p));
		}
		public static SearchSet is_defined(string searchstr,PartOfSpeech fpos)
		{
			Indexes ixs = new Indexes(searchstr,fpos);
			Index index;
			int i;
			int CLASS =          22;	/* - */
			int LASTTYPE =	CLASS;
			
			Search s = new Search(searchstr,fpos,new SearchType(false,"FREQ"),0);
			SearchSet retval = new SearchSet();
			while ((index=ixs.next())!=null) 
			{
				retval=retval+"SIMPTR"+"FREQ"+"SYNS"+"WNGREP"+"OVERVIEW"; // added WNGREP - TDMS
				for (i=0;i<index.ptruse.Length;i++) 
				{
					PointerType pt = index.ptruse[i];
					//					retval=retval+pt;

					// WN2.1 - TDMS
					if (pt.ident <= LASTTYPE) 
					{
						retval = retval + pt;
					} 
					else if (pt.mnemonic == "INSTANCE") 
					{
						retval = retval + "HYPERPTR";
					} 
					else if (pt.mnemonic == "INSTANCES") 
					{
						retval = retval + "HYPOPTR";
					}

					// WN2.1 - TDMS
					if (pt.mnemonic == "SIMPTR") 
					{
						retval = retval + "ANTPTR";
					} 

					if (fpos.name=="noun")
					{
						/* set generic HOLONYM and/or MERONYM bit if necessary */
						if (pt>="ISMEMBERPTR" && pt<="ISPARTPTR")
							retval=retval+"HOLONYM";
						else if (pt>="HASMEMBERPTR" && pt<="HASPARTPTR")
							retval=retval+"MERONYM";
					} 
					// WN2.1 - TDMS					else if (fpos.name=="adj" && pt.mnemonic=="SIMPTR")
					//						retval=retval+"ANTPTR";
				}
				if (fpos.name=="noun") 
				{
					retval=retval+"RELATIVES";
					if (index.HasHoloMero("HMERONYM",s))
						retval=retval+"HMERONYM";
					if (index.HasHoloMero("HHOLONYM",s))
						retval=retval+"HHOLONYM";
					if (retval["HYPERPTR"])
						retval = retval+"COORDS";
				} 
				else if (fpos.name=="verb")
					retval=retval+"RELATIVES"+"FRAMES"; // added frames - TDMS
			}
			return retval;
		}

		internal static ArrayList wngrep(string wordPassed,PartOfSpeech pos)
		{
			ArrayList r = new ArrayList();
			StreamReader inputFile = index(pos);
			inputFile.BaseStream.Seek(0L,SeekOrigin.Begin);
			inputFile.DiscardBufferedData();
			string word = wordPassed.Replace(" ","_");
			string line;

			while ((line=inputFile.ReadLine())!=null)
			{
				int lineLen = line.IndexOf(' ');
				line = line.Substring(0,lineLen);
				try 
				{
					if (line.IndexOf(word)>=0)
						r.Add(line.Replace("_"," "));
				} 
				catch 
				{
				}
			}
			return r;
		}
	}

	internal class Indexes
	{
		// TDMS - 14 Aug 2005 - added a new index count
		// so that we could patch more possibilities into
		// the strings array below
		static int stringscount = 7; 
		Index[] offsets = new Index[stringscount]; // of Index
		int offset=0;
		PartOfSpeech fpos;
		public Indexes(string str,PartOfSpeech pos)
		{
			string[] strings = new string[stringscount];
			str = str.ToLower();
			strings[0] = str;
			strings[1] = str.Replace('_','-');
			strings[2] = str.Replace('-','_');
			strings[3] = str.Replace("-","").Replace("_","");
			strings[4] = str.Replace(".","");
			// TDMS - 14 Aug 2005 - added two more possibilities
			// to allow for spaces to be transformed
			// an example is a search for "11 plus", without this
			// transformation no result would be found
			strings[5] = str.Replace(' ','-');
			strings[6] = str.Replace(' ','_');
			offsets[0] = Index.lookup(str,pos);
			// TDMS - 14 Aug 2005 - changed 5 to 7 to allow for two
			// new possibilities
			for (int i=1;i<stringscount;i++)
				if (str!=strings[i])
					offsets[i] = Index.lookup(strings[i],pos);
			fpos = pos;
		}
		public Index next()
		{
			for (int i=offset; i<stringscount; i++)
				if (offsets[i]!=null) 
				{
					offset = i+1;
					return (Index)offsets[i];
				}
			return null;
		}
	}

	
	public class StrTok
	{
		string[] strs;
		int pos;
		public StrTok(string s,params char [] seps) 
		{
			strs = s.Split(seps);
			pos = 0;
		}
		public string next()
		{
			for (;pos<strs.Length;pos++)
				if ((strs[pos]!=""))
					return strs[pos++];
			return null;
		}
	}

	public class PointerType
	{
		static Hashtable ptypes = new Hashtable();
		public static PointerType of(string s) // lookup by symbol or mnemonic
		{
			if (uniq==0)
				classinit();
			return (PointerType) ptypes[s];
		}
		public static PointerType of(int id) // lookup by ident
		{
			if (uniq==0)
				classinit();
			return (PointerType) ptypes[id];
		}
		public static int Count { get { return uniq; }}
		string sym;
		public string symbol { get { return sym; }}
		string mnem;
		public string mnemonic { get { return mnem; }}
		int id;
		public int ident { get { return id; }}
		string labl;
		public string label { get { return labl; }}
		static int uniq=0;
		PointerType(string s,string m,string h) 
		{
			sym = s; 
			mnem = m;
			labl = h;
			id = ++uniq;
			ptypes[m] = this;
			ptypes[s] = this;
			ptypes[id] = this;
		}
		public static PointerType operator+(PointerType a,int b)
		{
			return (PointerType)ptypes[a.id+b];
		}
		static void classinit() 
		{
			new PointerType("!","ANTPTR","Antonyms"); // 1
			new PointerType("@","HYPERPTR","Synonyms/Hypernyms"); // 2
			new PointerType("~","HYPOPTR","Hyponyms");  // 3
			new PointerType("*","ENTAILPTR","Entailment");  // 4
			new PointerType("&","SIMPTR","Similarity"); // 5
			new PointerType("#m","ISMEMBERPTR","Member Holonyms"); // 6
			new PointerType("#s","ISSTUFFPTR","Substance Holonyms");  // 7
			new PointerType("#p","ISPARTPTR","Part Holonyms");  // 8
			new PointerType("%m","HASMEMBERPTR","Member Meronyms"); // 9
			new PointerType("%s","HASSTUFFPTR","Substance Meronyms"); // 10
			new PointerType("%p","HASPARTPTR","Part Meronyms"); // 11
			new PointerType("%","MERONYM","Meronyms"); // 12
			new PointerType("#","HOLONYM","Holonyms"); // 13
			new PointerType(">","CAUSETO","'Cause To'"); // 14
			new PointerType("<","PPLPTR",""); // 15
			new PointerType("^","SEEALSOPTR","See also"); // 16
			new PointerType("\\","PERTPTR","Pertainyms"); // 17
			new PointerType("=","ATTRIBUTE","Attributes"); // 18
			new PointerType("$","VERBGROUP",""); // 19
			new PointerType("+","NOMINALIZATIONS",""); // 20 // TDMS 26/8/05 - re-added nominalizations which was commented
			//			new PointerType("+","DERIVATION",""); // 20 // TDMS 26/8/05 - removed deviation
			new PointerType(";","CLASSIFICATION",""); // 21
			new PointerType("-","CLASS",""); // 22
			/* Additional searches, but not pointers.  */
			new PointerType("","SYNS","Synonyms");
			new PointerType("","FREQ","Frequency");
			new PointerType("+","FRAMES","Sample Sentences");
			new PointerType("","COORDS","Coordinate Terms");
			new PointerType("","RELATIVES","Relatives");
			new PointerType("","HMERONYM","Meronyms");
			new PointerType("","HHOLONYM","Holonyms");
			new PointerType("","WNGREP","Grep");
			new PointerType("","OVERVIEW","Overview");
			new PointerType(";c","CLASSIF_CATEGORY","Classification Category");
			new PointerType(";u","CLASSIF_USAGE","Classification Usage");
			new PointerType(";r","CLASSIF_REGIONAL","Classification Regional");
			new PointerType("-c","CLASS_CATEGORY","Class Category");
			new PointerType("-u","CLASS_USAGE","Class Usage");
			new PointerType("-r","CLASS_REGIONAL","Class Regional");

			// WN2.1 - TDMS
			new PointerType("@i","INSTANCE","Instance");
			new PointerType("~i","INSTANCES","Instances");
		}
		public static bool operator>=(PointerType a,string s) 
		{
			return a.ident>=PointerType.of(s).ident;
		}
		public static bool operator<=(PointerType a,string s) 
		{
			return a.ident<=PointerType.of(s).ident;
		}
	}

	public class SearchSet
	{
		BitSet b;
		internal SearchSet() { b = new BitSet(30); }
		internal SearchSet(SearchSet s) { b = new BitSet(s.b); }
		public static SearchSet operator+ (SearchSet a,string s)
		{
			SearchSet r = new SearchSet(a);
			r.b[PointerType.of(s).ident]=true;
			return r;
		}
		public static SearchSet operator+ (SearchSet a,PointerType p)
		{
			SearchSet r = new SearchSet(a);
			r.b[p.ident]=true;
			return r;
		}
		public static SearchSet operator+ (SearchSet a,SearchSet b)
		{
			SearchSet r = new SearchSet(a);
			r.b = a.b.Or(b.b);
			return r;
		}
		public bool this[int i] { get { return b[i]; }}
		public bool this[string s] { get { return b[PointerType.of(s).ident]; }}
		public bool NonEmpty { get {	return b.GetHashCode()!=0; }}
	}

	public class SearchType : IComparable
	{
		public bool rec;
		public PointerType ptp;
		//public static SortedList searchtypes = new SortedList(); // SearchType -> SearchType
		public SearchType(bool r,string t) : this(r,PointerType.of(t)) {}
		public SearchType(bool r,PointerType p)
		{
			ptp = p;
			rec = r;
		}
		public SearchType(string m) : this((m[0]=='-'),(m[0]==' ')?m.Substring(1):m) {}
		public string label { get { return ptp.label; }}
		public int CompareTo(object a)
		{
			SearchType s = (SearchType)a;
			if (ptp.ident<s.ptp.ident)
				return -1;
			if (ptp.ident>s.ptp.ident)
				return 1;
			if ((!rec) && s.rec)
				return -1;
			if (rec && !s.rec)
				return 1;
			return 1;
		}
		public override bool Equals(object a)
		{
			return CompareTo(a)==0;
		}
		public override int GetHashCode()
		{
			return rec.GetHashCode()+ptp.GetHashCode();
		}
	}

	public class Pointer
	{
		public PointerType ptp;
		public PartOfSpeech pos;
		public int sce;   // which word in source synset
		public int dst;   // which word in target synset
		public int off; // target offset
		public SynSet target; // cached version of off
		internal Pointer(string s) { ptp=PointerType.of(s); }
	}

	[FlagsAttribute]
	public enum PartsOfSpeech { Unknown=0, Noun=1, Verb=2, Adj=4, Adv=8 } ;
	public class PartOfSpeech 
	{
		public static Hashtable parts = new Hashtable();
		string sy;
		public string symbol { get { return sy; }}
		string nm;
		public string name {get{return nm;}}
		string cl;
		public string clss {get{return cl;}}
		int id;
		public int ident {get{return id; }}
		PartsOfSpeech flg;
		public PartsOfSpeech flag { get{ return flg; }}
		static int uniq = 0;
		internal Hashtable help = new Hashtable(); // string searchtype->string help: see WnHelp
		PartOfSpeech(string s,string n, string c,PartsOfSpeech f) 
		{
			sy = s;
			nm = n;
			cl = c;
			flg = f;
			id = uniq++;
			parts[s] = this;
			if (c=="")
				parts[nm] = this;
		}
		PartOfSpeech(string s,string n,PartsOfSpeech f) : this(s,n,"",f) {}
		public static PartOfSpeech of(string s)
		{
			if (uniq==0)
				classinit();
			return (PartOfSpeech)parts[s];
		}
		public static PartOfSpeech of(PartsOfSpeech f)
		{
			if (f==PartsOfSpeech.Noun)
				return PartOfSpeech.of("noun");
			if (f==PartsOfSpeech.Verb)
				return PartOfSpeech.of("verb");
			if (f==PartsOfSpeech.Adj)
				return PartOfSpeech.of("adj");
			if (f==PartsOfSpeech.Adv)
				return PartOfSpeech.of("adv");
			return null;			// unknown or not unique
		}
		static void classinit()
		{
			new PartOfSpeech("n","noun",PartsOfSpeech.Noun); // 0
			new PartOfSpeech("v","verb",PartsOfSpeech.Verb); // 1
			new PartOfSpeech("a","adj",PartsOfSpeech.Adj); // 2
			new PartOfSpeech("r","adv",PartsOfSpeech.Adv); // 3
			new PartOfSpeech("s","adj","SATELLITE",PartsOfSpeech.Adj);
		}
	}

	public class Frame 
	{
		static ArrayList frames = new ArrayList();
		public static Frame frame(int i) 
		{
			if (frames.Count==0)
				classinit();
			return (Frame)frames[i];
		}
		string st;
		public string str { get{return st;}}
		Frame(string f) 
		{
			st = f;
			frames.Add(this);
		}
		static void classinit()
		{
			new Frame("");
			new Frame("Something ----s");
			new Frame("Somebody ----s");
			new Frame("It is ----ing");
			new Frame("Something is ----ing PP");
			new Frame("Something ----s something Adjective/Noun");
			new Frame("Something ----s Adjective/Noun");
			new Frame("Somebody ----s Adjective");
			new Frame("Somebody ----s something");
			new Frame("Somebody ----s somebody");
			new Frame("Something ----s somebody");
			new Frame("Something ----s something");
			new Frame("Something ----s to somebody");
			new Frame("Somebody ----s on something");
			new Frame("Somebody ----s somebody something");
			new Frame("Somebody ----s something to somebody");
			new Frame("Somebody ----s something from somebody");
			new Frame("Somebody ----s somebody with something");
			new Frame("Somebody ----s somebody of something");
			new Frame("Somebody ----s something on somebody");
			new Frame("Somebody ----s somebody PP");
			new Frame("Somebody ----s something PP");
			new Frame("Somebody ----s PP");
			new Frame("Somebody's (body part) ----s");
			new Frame("Somebody ----s somebody to INFINITIVE");
			new Frame("Somebody ----s somebody INFINITIVE");
			new Frame("Somebody ----s that CLAUSE");
			new Frame("Somebody ----s to somebody");
			new Frame("Somebody ----s to INFINITIVE");
			new Frame("Somebody ----s whether INFINITIVE");
			new Frame("Somebody ----s somebody into V-ing something");
			new Frame("Somebody ----s something with something");
			new Frame("Somebody ----s INFINITIVE");
			new Frame("Somebody ----s VERB-ing");
			new Frame("It ----s that CLAUSE");
			new Frame("Something ----s INFINITIVE");
		}
	}
}
