using System;
using Wnlib;

namespace WordsMatching
{
	/// <summary>
	/// Summary description for LeskDisambiguator.
	/// </summary>
	public class LeskDisambiguator
	{
		static Opt[] NOUN_RELATIONS=new Opt[] { Opt.at(8) , //hyper
												  Opt.at(14), //holo
												  Opt.at(19), //mero
												  Opt.at(12) //hypo												
											  } ;
		static Opt[] VERB_RELATIONS=new Opt[] {
												  Opt.at(31),//hyper
												  Opt.at(36)//tropo // may be 38
											  } ;

		private SynSetList _senses;
		
		public LeskDisambiguator()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		private void MyInit()
		{
			
		}
		
		private string RemoveBadChars(string s)
		{
			s=s.Replace("=>", " ");
			s=s.Replace("=", " ");
			s=s.Replace("--", " ");
			s=s.Replace(">", " ");
			s=s.Replace("+", " ");
			s=s.Replace(";", " ");
			s=s.Replace(",", " ");
			return s;
		}

		public SynSetList Disambiguate(string[] contextWords)
		{
			SynSetList myList=new SynSetList() ;
			
			return myList ;
		}

		public string ConcateRel(string word, int senseIndex, Opt rel)
		{			
			Search se=new Search(word, true, rel.pos, rel.sch, senseIndex);//

			int a=se.buf.IndexOf("\n");
			if (a>=0) 
			{
				if (a==0) 
				{
					se.buf = se.buf.Substring(a+1);
					a = se.buf.IndexOf("\n");
				}
				se.buf = se.buf.Substring(a+1);
			}

			SynSet sense=se.senses [0];

			string con=se.buf.ToString();
			int pIndex=con.IndexOf(sense.defn);
			int lcon=con.Length ;
			int ldef=sense.defn.Length ;

			string s=con.Substring(pIndex + ldef - 1, lcon- (pIndex + ldef - 1 )) ;					

			return RemoveBadChars(s);
		}

		private string GetGloss(SynSet sense)
		{
			string gloss=sense.defn ;
			if (gloss.IndexOf(";") != -1)
				gloss=gloss.Substring(0, gloss.IndexOf(";")) ;
	
			return gloss;
		}

		private int GetOverlap(string[] a, string[] b)
		{
			int score=0;
			return score;
		}

		private int GetNounRelations(SynSet sense1, SynSet sense2)
		{
			
			string[] relations1=new string[NOUN_RELATIONS.Length + 1] ;
			string[] relations2=new string[NOUN_RELATIONS.Length + 1] ;
			
			relations1[0]=GetGloss (sense1);
			relations2[0]=GetGloss (sense2);

			int senseIndex1=sense1.words[sense1.whichword - 1].wnsns; 
			int senseIndex2=sense2.words[sense2.whichword - 1].wnsns; 
			string word1=sense1.words[sense1.whichword - 1].word ;
			string word2=sense2.words[sense2.whichword - 1].word ;
			
			for(int i=0; i < NOUN_RELATIONS.Length; i++ )
			{
				Opt rel=NOUN_RELATIONS[i];				
				relations1[i + 1] = ConcateRel(word1, senseIndex1, rel);				
				relations2[i + 1] = ConcateRel(word2, senseIndex2, rel);
			}
			
			return 0;
		}


	}
}
