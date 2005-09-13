/*
Extending the Lesk gloss overlap measurement
Author: Thanh Ngoc Dao - Thanh.dao@gmx.net
Copyright (c) 2005 by Thanh Ngoc Dao.
*/

using System;
using Wnlib;
using WnLexicon;

namespace WordsMatching
{

	public interface IDisambiguator
	{
		int[] Disambiguate(string[] contextWords);
	}

	/// <summary>
	/// Summary description for Overlap.
	/// </summary>
	public class OverlapRelatedness:IDisambiguator 
	{		
		public OverlapRelatedness()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		const int THRESHOLD=0;

		static Opt[] NOUN_RELATIONS=new Opt[] { Opt.at(8) , //hyper
												 // Opt.at(14), //holo
												//  Opt.at(19), //mero
												  Opt.at(12) //hypo												
											  } ;
		static Opt[] VERB_RELATIONS=new Opt[] {
												  Opt.at(31),//hyper
												  Opt.at(36)//tropo // may be 38
											  } ;

		private string[][][][] _relatedCube ;//[words][senses][relations]

		private string[] _contextWords;
		private int[] _bestSenses;
		private Opt[] _priorRelations=null;
		

		private void MyInit()
		{
			_relatedCube=new string[_contextWords.Length][][][];
			_bestSenses=new int[_contextWords.Length];
			for(int i=0; i < _bestSenses.Length; i++)
				_bestSenses[i]=-1;
			Init_Relations();
		}

		private void Init_Relations()
		{
			for (int i=0; i < _contextWords.Length; i++)
			{
				WnLexicon.WordInfo wordinfo=WnLexicon.Lexicon.FindWordInfo( _contextWords[i], true );
				if( wordinfo.partOfSpeech != Wnlib.PartsOfSpeech.Unknown )
				{
					Wnlib.PartsOfSpeech[] enums = (Wnlib.PartsOfSpeech[])Enum.GetValues( typeof( Wnlib.PartsOfSpeech ) );
					
					bool stop=false;
					int senseCount=0;
					for( int j=0; j<enums.Length && !stop; j++ )
					{
						Wnlib.PartsOfSpeech pos=enums[j];												
						
						if (wordinfo.senseCounts[j] > 0)
							switch (pos)
							{
								case Wnlib.PartsOfSpeech.Noun:
								{									
									senseCount=wordinfo.senseCounts[j];
									_priorRelations=NOUN_RELATIONS ;									
									stop=true;
									break;
								}
								case Wnlib.PartsOfSpeech.Verb:
								{
									senseCount=wordinfo.senseCounts[j];
									_priorRelations=VERB_RELATIONS ;
									stop=true;
									break;
								}

							};						
					}

					if (stop) 						
					{
						string[][][] tmp=GetAllRelations(_contextWords[i], senseCount );						
						if (tmp == null)
						{
							int w=0;
						}
						_relatedCube[i]=tmp;
					}

				}
			}
		}

		private int GetOverlap(string[] a,string[] b)
		{
			//IOverlapCounter overlap=new SimpleOverlapCounter() ;
			IOverlapCounter overlap=new ExtOverlapCounter() ;
			return overlap.GetScore(a, b) ;
		}
			
		private int ScoringSensePair(string[][] sense1, string[][] sense2)
		{
			int score=0;
			int m=sense1.Length , n=sense2.Length ;
			for(int i=0; i < m; i++)
			{
				for(int j=0; j < n; j++)
				{
					score +=GetOverlap(sense1[i], sense2[j]) ;
				}
			}

			return score;
		}

		private void Init_ScoreCube(out int[][][][] _scores, int wordCount)
		{
			_scores=new int[_contextWords.Length][][][] ;
			for (int i=0; i < wordCount; i++)
				if (_relatedCube[i] != null)
			{				
				int iSenseCount=_relatedCube[i].Length ;
				_scores[i]=new int[iSenseCount][][] ;				

				for (int iSense=0;  iSense < iSenseCount; iSense++  )					
				{
					_scores[i][iSense]=new int[wordCount][] ;

					for (int j=0; j < wordCount; j++)
						if (_relatedCube[j] != null)
					{	
						int jSenseCount=_relatedCube[j].Length ;
						_scores[i][iSense][j]=new int[jSenseCount] ;										
					}
				}
			}			
		}

		private void Scoring_Overlaps()
		{
			int wordCount=_contextWords.Length ;
			int[][][][] scoreCube;
			Init_ScoreCube(out scoreCube, wordCount);
			
			for (int i=0; i < wordCount; i++)
				if (_relatedCube[i] != null)
			{				
				int iSenseCount=_relatedCube[i].Length ;				
				int bestScore=0;

				for (int iSense=0;  iSense < iSenseCount ; iSense++  )
				{					
					int senseScore=0;
					for (int j=0; j < wordCount; j++)
					if(i != j && _relatedCube[j] != null)
					{	
						int jSenseCount=_relatedCube[j].Length ;						
						
						int bestScoreWith_j=0;
						for (int jSense=0; jSense < jSenseCount ; jSense++  )
						{
							int score=0;
							if (scoreCube[i][iSense][j][jSense] != 0)
								score=scoreCube[i][iSense][j][jSense];
							else
								if (scoreCube[j][jSense][i][iSense] !=0)
									score=scoreCube[j][jSense][i][iSense];
							
							if (score == 0)
							{
								score=ScoringSensePair(_relatedCube[i][iSense], _relatedCube[j][jSense]);							
								if (score > 0 )
								{
									scoreCube[i][iSense][j][jSense]=score;
									scoreCube[j][jSense][i][iSense]=score;
								}
							}

							if (bestScoreWith_j < score)
								bestScoreWith_j=score;
						}				
		
						if (bestScoreWith_j > THRESHOLD)
							senseScore += bestScoreWith_j;
					}						

					if (senseScore > bestScore)
					{
						bestScore=senseScore ;
						_bestSenses[i]=iSense;
					}
				}
			}

			
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
		
		public int[] Disambiguate(string[] contextWords)
		{			
			_contextWords=contextWords;
			MyInit();
			Scoring_Overlaps();			

			return _bestSenses;
		}

		public string[] ConcateRel(Search se)
		{			

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
			if (con == string.Empty) return null;
			int pIndex=con.IndexOf(sense.defn);
			if (pIndex == -1) return null;
			int lcon=con.Length ;
			int ldef=sense.defn.Length ;
			
			string s=con.Substring(pIndex + ldef - 1, lcon- (pIndex + ldef - 1 )) ;					
			
			s=RemoveBadChars(s);
			Tokeniser tok=new Tokeniser() ;
			string[] toks=tok.Partition(s);
			return toks;
		}

		private string[] GetGloss(SynSet sense)
		{
			string gloss=sense.defn ;
			if (gloss.IndexOf(";") != -1)
				gloss=gloss.Substring(0, gloss.IndexOf(";")) ;
			
			Tokeniser tok=new Tokeniser() ;
			string[] glossToks=tok.Partition(gloss) ;

			return glossToks;
		}

		private string[][][] GetAllRelations(string word, int senseCount)
		{
			if (_priorRelations == null) return null;
			string[][][] matrix=new string[senseCount][][] ;
			for (int i=0; i < senseCount; i++)
			{				
				matrix[i]=GetRelations(word, i + 1);	
							
			}

			return matrix;
		}

		private string[][] GetRelations(string word, int senseIndex)
		{			
			string[][] relations=new string[_priorRelations.Length + 1][] ;						
		
			for(int i=0; i < _priorRelations.Length; i++ )
			{
				Opt rel=_priorRelations [i];				
				Search se=new Search(word, true, rel.pos, rel.sch, senseIndex);//				
				if (relations[0] == null )
					relations[0]=GetGloss (se.senses [0]);			

				relations[i + 1] = ConcateRel(se);				
				
			}
			
			return relations;
		}



	}
}
