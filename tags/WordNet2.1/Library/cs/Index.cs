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

namespace Wnlib
{
	/// <summary>
	/// Summary description for Index.
	/// </summary>
	public class Index
	{
		public PartOfSpeech pos =null;
		public string wd;
		public int sense_cnt = 0;		/* sense (collins) count */
		public PointerType[] ptruse = null; /* pointer data in index file */
		public int tagsense_cnt = 0;	/* number senses that are tagged */
		public int[] offs = null;		/* synset offsets */
		public SynSet[] syns = null;   /* cached */
		public Index next = null;
		
		public void Print()
		{
			Console.Write(pos.name+" "+sense_cnt+" ");
			for (int j=0;j<ptruse.Length;j++)
				Console.Write(ptruse[j].mnemonic+" ");
			Console.WriteLine();
			//		for (int k=0;k<offs.Length;k++) 
			//		{
			//			SynSet s = new SynSet(offs[k],pos);
			//			Console.WriteLine(s.defn);
			//		}
			if (next!=null)
				next.Print();
		}

		/* From search.c:
		 * Find word in index file and return parsed entry in data structure.
		   Input word must be exact match of string in database. */
		
		// From the WordNet Manual (http://wordnet.princeton.edu/man/wnsearch.3WN.html)
		// index_lookup() finds searchstr in the index file for pos and returns a pointer 
		// to the parsed entry in an Index data structure. searchstr must exactly match the 
		// form of the word (lower case only, hyphens and underscores in the same places) in 
		// the index file. NULL is returned if a match is not found.
		public static Index lookup(string word,PartOfSpeech pos)
		{
			int j;
			if (word=="")
				return null;
			// TDMS 14 Aug 2005 - changed to allow for numbers as well
			// because the database contains searches that can start with
			// numerals
			//if (!char.IsLetter(word[0]))
			if (!char.IsLetter(word[0]) && !char.IsNumber(word[0]))
				return null;
			string line = WNDB.binSearch(word,pos);
			if (line==null)
				return null;
			Index idx = new Index();
			StrTok st = new StrTok(line);
			idx.wd = st.next(); /* the word */
			idx.pos = PartOfSpeech.of(st.next()); /* the part of speech */
			idx.sense_cnt = int.Parse(st.next()); /* collins count */
			int ptruse_cnt = int.Parse(st.next()); /* number of pointers types */
			idx.ptruse = new PointerType[ptruse_cnt];
			for (j=0;j<ptruse_cnt;j++)
				idx.ptruse[j] = PointerType.of(st.next());
			int off_cnt = int.Parse(st.next());
			idx.offs = new int[off_cnt];
			idx.tagsense_cnt = int.Parse(st.next());
			for (j=0;j<off_cnt;j++)
				idx.offs[j] = int.Parse(st.next());
			return idx;
		}
		
		public bool HasHoloMero(string s,Search search)
		{
			return HasHoloMero(PointerType.of(s),search);
		}
		
		public bool HasHoloMero(PointerType p,Search search)
		{
			PointerType pbase;
			if (p.mnemonic=="HMERONYM")
				pbase = PointerType.of("HASMEMBERPTR");
			else
				pbase = PointerType.of("ISMEMBERPTR");
			for (int i=0;i<offs.Length;i++) 
			{
				SynSet s = new SynSet(offs[i],PartOfSpeech.of("noun"),"",search,0);
				if (s.has(pbase)|| s.has(pbase+1) || s.has(pbase+2))
					return true;
			}
			return false;
		}
	}
}
