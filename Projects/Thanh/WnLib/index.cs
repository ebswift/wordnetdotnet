using System;

namespace Wnlib
{
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
