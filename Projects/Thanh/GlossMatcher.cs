/*
Lexical Similarity
Author: Thanh Ngoc Dao - Thanh.dao@gmx.net
Copyright (C) 2005 Malcolm Crowe, Troy Simpson, Jeff Martin, Thanh Dao

*/

using System;
using System.Collections;
using WnLexicon;
using Wnlib;


namespace WordsMatching
{
	/// <summary>
	/// Summary description for SemanticSimilarity.
	/// </summary>
	public class GlossMatcher : ISimilarity
	{
		const int DEPTH=2;
		public GlossMatcher()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		
		public float GetSimilarity(string string1, string string2)
		{
			//return CalcSimilarity(string1, string2, DEPTH)/10;
			return GetGlossSimilarity (string1, string2, DEPTH);
		}

		public static string[] GetSynonyms(string word)
		{
			WordInfo wordinfo=Lexicon.FindWordInfo(word, true ) ;
			if( wordinfo.partOfSpeech == Wnlib.PartsOfSpeech.Unknown )			
			{			
				return null;
			}
			Wnlib.PartsOfSpeech[] enums = (Wnlib.PartsOfSpeech[])Enum.GetValues( typeof( Wnlib.PartsOfSpeech ) );
			ArrayList synsList=new ArrayList() ;			
			for( int i=0; i<enums.Length; i++ )
			{
				Wnlib.PartsOfSpeech pos = enums[i];
				if( pos == Wnlib.PartsOfSpeech.Unknown )continue;

//				txtOut.AppendText( String.Format( "{0,12}: {1}\r\n", pos, wordinfo.senseCounts[i] ) );
				if (wordinfo.senseCounts[i] > 0)
				{
					string[] synonyms=Lexicon.FindSynonyms(word, pos , false ) ;
					if (synonyms != null)
					foreach (string asyn in synonyms)
					{
						string w=asyn.ToLower(); 
						bool add=true;
						foreach (string s in synsList)
							if (s.Equals(w)) add=false;
						if (add) synsList.Add(w) ;
					}
				}
			}

			string[] result=new string[synsList.Count] ;
			for(int i=0; i < synsList.Count ; i++) result[i]=(string) synsList[i];

			return result;
		}

		private ArrayList GetDefinitions(string word)
		{
			Opt opt=Opt.at(8);//search for hypernymy
			Search se=new Search(word, true, opt.pos , opt.sch, int.Parse("0"));//
			ArrayList senses=new ArrayList() ;
			foreach (Wnlib.SynSet syn in se.senses)
			{
				senses.Add(syn.defn) ;
			}
			return senses;
		}

		private bool AreEqual(string w1, string w2)
		{
			//MorphStr morph=new MorphStr(w1, "noun") ;
			//w1=morph.morphword(w1) ;
			//w2=morph.morphword(w2) ;
			StemmerInterface stem=new PorterStemmer() ;
			
			//w1= ;
			//2=) ;
			if (stem.stemTerm(w1) == stem.stemTerm(w2)) return true;
			string[] wordList1=GetSynonyms (w1);
			string[] wordList2=GetSynonyms (w2);

			for (int i=0; i < wordList1.Length ; i++ )
			{
				if (w2.Equals(wordList1[i]) ) return true;
				for (int j=0; j < wordList2.Length ; j++ )
				{
					if (w1.Equals(wordList2[j]) ) return true;

				}
			}

			return false;
		}

		private float GetGlossSimilarity(string word1, string word2,int depth)
		{
			//if (word1.Equals(word2) ) return 1.0F;
			if (AreEqual(word1, word2) ) return 1.0F;
			if (depth <= 0) 
			{
				return 0.0f;				
			};
		
			float score=0;
			ArrayList defnList1=GetDefinitions(word1);
			ArrayList defnList2=GetDefinitions(word2);
			if (defnList1 == null || defnList2 == null ||
				defnList1.Count == 0 || defnList2.Count  == 0)
			{
				return 0;	
			};
			Tokeniser tok=new Tokeniser() ;
			float maxScore=float.MinValue ;
			foreach (string s1 in defnList1)
				foreach (string s2 in defnList2)
				{
					string def1=s1;
					if (s1.IndexOf(";") != -1)
						def1=s1.Substring(0, s1.IndexOf(";")) ;
					string def2=s2;
					if (s2.IndexOf(";") != -1)
						def2=s2.Substring(0, s2.IndexOf(";")) ;

					string[] wordList1=tok.Partition(def1) ;
					string[] wordList2=tok.Partition(def2) ;
					float[ , ] cost=new float[wordList1.Length, wordList2.Length ] ;

					//Init 
					for (int i=0; i < wordList1.Length ; i++ )
					{						
						for (int j=0; j < wordList2.Length ; j++ )
						{							
							if (cost[i, j] == 0)
								cost[i, j]=(float) Math.Round(GetGlossSimilarity(wordList1[i], wordList2[j], depth - 1), 2);
						}
					}
					if (cost != null)
					{
						BipartiteMatcher bipartite=new BipartiteMatcher(wordList1, wordList2, cost) ;
						
						score=bipartite.Score ;
						if (maxScore < score) maxScore=score;
					}
				}
			

			return maxScore;				  
		}


		private float CalcSimilarity(string word1, string word2,int depth)
		{
			if (word1.Equals(word2) ) return 10;
			if (depth <= 0) 
			{
				Leven leven=new Leven() ;
				return leven.GetSimilarity(word1, word2) ;
				
			};
		
			float score=0;
			string[] wordList1=GetSynonyms (word1);
			string[] wordList2=GetSynonyms (word2);
			if (wordList1 == null || wordList2 == null ||
				wordList1.Length == 0 || wordList2.Length == 0)
			{
				Leven leven=new Leven() ;

				return leven.GetSimilarity(word1, word2) ;
				
			}
			else
			{
				
				float[ , ] cost=new float[wordList1.Length, wordList2.Length ] ;
				//Init 
				for (int i=0; i < wordList1.Length ; i++ )
				{
					if (word2.Equals(wordList1[i]) ) return 10;
					for (int j=0; j < wordList2.Length ; j++ )
					{
						if (word1.Equals(wordList2[j]) ) return 10;

						if (cost[i,j] == 0)
						cost[i, j]=(float) Math.Round(CalcSimilarity(wordList1[i], wordList2[j], depth - 1),1);
					}
				}
				if (cost != null)
				{
					BipartiteMatcher bipartite=new BipartiteMatcher(wordList1, wordList2, cost) ;
				
					score=bipartite.Score ;
				}
			}
			
			if (score == 0 )
			{
				Leven leven=new Leven() ;
				return leven.GetSimilarity(word1, word2) ;				
			}

			return score;				  
		}
	}
}
