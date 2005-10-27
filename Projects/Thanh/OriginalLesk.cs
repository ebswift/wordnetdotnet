/*
adapted version Lesk WSD
Author: Thanh Ngoc Dao - Thanh.dao@gmx.net
Copyright (c) 2005 by Thanh Ngoc Dao.
*/

using System;
using Wnlib;

namespace WordsMatching
{



	/// <summary>
	/// Summary description for Overlap.
	/// </summary>
	public class OriginalLesk: IDisambiguator 
	{		
		public OriginalLesk()
		{
		}
		const int THRESHOLD=0;
		const int CONTEXT_SIZE=5;//Local disambiguation within the context size 

		static Opt[] NOUN_RELATIONS=new Opt[] { Opt.at(8) , //hyper
//												  Opt.at(14), //holo
//												  Opt.at(19), //mero
												  Opt.at(12) //hypo												
											  } ;
		static Opt[] VERB_RELATIONS=new Opt[] {
												  Opt.at(31),//hyper
												  Opt.at(36)//tropo // may be 38
											  } ;

		private string[][][][] _relCube ;//[words][senses][relations]

		private string[] _contextWords;
		private int[] _bestSenses;
		private Opt[] _priorRelations=null;
		

		private void MyInit()
		{
			_relCube=new string[_contextWords.Length][][][];
			_bestSenses=new int[_contextWords.Length];
			for(int i=0; i < _bestSenses.Length; i++)
				_bestSenses[i]=-1;
			Init_Relations();
		}
		
		private bool InContext(int pos_target, int pos_j, int size)
		{
			int med=size/2;
			if (size == 0 ) med=_contextWords.Length - 1;
			
			if (pos_j <= pos_target + med && pos_j >= pos_target - med)
				return true;
			else
				return false;
		}

		private void Init_Relations()
		{
			for (int i=0; i < _contextWords.Length; i++)
			{
				WnLexicon.WordInfo wordInfo=WnLexicon.Lexicon.FindWordInfo( _contextWords[i], true );
				
				if( wordInfo.partOfSpeech != Wnlib.PartsOfSpeech.Unknown )
				{
					if (wordInfo.text != string.Empty)
						_contextWords[i]=wordInfo.text ;
					Wnlib.PartsOfSpeech[] posEnum=(Wnlib.PartsOfSpeech[])Enum.GetValues( typeof(Wnlib.PartsOfSpeech) );
					
					bool stop=false;
					int senseCount=0;
					for( int j=0; j < posEnum.Length && !stop; j++ )
					{
						Wnlib.PartsOfSpeech pos=posEnum[j];												
						
						if (wordInfo.senseCounts[j] > 0)
							switch (pos)
							{
								case Wnlib.PartsOfSpeech.Noun:
								{									
									senseCount=wordInfo.senseCounts[j];
									_priorRelations=NOUN_RELATIONS ;									
									stop=true;
									break;
								}
								case Wnlib.PartsOfSpeech.Verb:
								{
									senseCount=wordInfo.senseCounts[j];
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
						_relCube[i]=tmp;
					}

				}
			}
		}

		private int GetOverlap(string[] a,string[] b)
		{
			IOverlapCounter overlap=new SimpleOverlapCounter() ;
			//IOverlapCounter overlap=new ExtOverlapCounter() ;
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
				if (_relCube[i] != null)
				{				
					int iSenseCount=_relCube[i].Length ;
					_scores[i]=new int[iSenseCount][][] ;				

					for (int iSense=0;  iSense < iSenseCount; iSense++  )					
					{
						_scores[i][iSense]=new int[wordCount][] ;

						for (int j=0; j < wordCount; j++)
							if (_relCube[j] != null)
							{	
								int jSenseCount=_relCube[j].Length ;
								_scores[i][iSense][j]=new int[jSenseCount] ;										
							}
					}
				}			
		}

		private void Scoring_Overlaps()//Greedy
		{
			int wordCount=_contextWords.Length ;
			int[][][][] scoreCube;
			Init_ScoreCube(out scoreCube, wordCount);
			
			for (int i=0; i < wordCount; i++)
				if (_relCube[i] != null)
				{				
					int iSenseCount=_relCube[i].Length ;				
					int bestScoreOf_i=0;

					for (int iSense=0;  iSense < iSenseCount ; iSense++  )
					{					
						int senseTotalScore=0;
						for (int j=0; j < wordCount; j++)
							if(i != j && InContext(i, j, CONTEXT_SIZE) && _relCube[j] != null)
							{	
								int jSenseCount=_relCube[j].Length ;						
						
								int bestScoreOf_j=0;
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
										score=ScoringSensePair(_relCube[i][iSense], _relCube[j][jSense]);							
										if (score > 0 )
										{
											scoreCube[i][iSense][j][jSense]=score;
											scoreCube[j][jSense][i][iSense]=score;
										}
									}

									if (bestScoreOf_j < score)
										bestScoreOf_j=score;
								}				
		
								if (bestScoreOf_j > THRESHOLD)
									senseTotalScore += bestScoreOf_j;
							}						

						if (senseTotalScore > bestScoreOf_i)
						{
							bestScoreOf_i=senseTotalScore ;
							_bestSenses[i]=iSense;
						}
					}
				}

			
		}
		
		private string RemoveBadChars(string s)
		{
			string[] badChars=new string[]{"=>", "==","=","->",">","+",";",",","_","-","."} ;
			foreach(string ch in badChars)			
				s=s.Replace(ch, " ") ;

			return s;
		}
		
		public int[] Disambiguate(string[] contextWords)
		{			
			_contextWords=contextWords;
			MyInit();
			Scoring_Overlaps();			

			return _bestSenses;
		}

		public string[] GetRelativeGlosses(Search se)
		{			
			string rels="";
			if (se.senses[0].senses != null)
				foreach (SynSet ss in se.senses[0].senses)
				{								
					foreach (Lexeme ww in ss.words)											
						rels += " " + ww.word;
				
					rels += ss.defn ;			
				}
			
			Tokeniser tok=new Tokeniser() ;
			string[] toks=tok.Partition(rels);

			return toks;
		}

		private string[] GetDefinition(SynSet sense)
		{
			if (sense == null) return null;
			string gloss=sense.defn ;
			//			if (gloss.IndexOf(";") != -1)
			//				gloss=gloss.Substring(0, gloss.IndexOf(";")) ;
			foreach (Lexeme word in sense.words)
			{
				gloss += " " + word.word;
			}
			Tokeniser tok=new Tokeniser() ;
			string[] toks=tok.Partition(gloss) ;

			return toks;
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
				if( se.senses != null && se.senses.Count > 0)
				{						
					if (word == "pine" && i==3)
					{
						int yy=0;
					}
			
					if (relations[0] == null  )
						relations[0]=GetDefinition (se.senses [0]);			
					if (se.senses[0].senses != null)					
						relations[i + 1]=GetRelativeGlosses(se) ;									

				}				
				else relations[i+1]= null;
			}
			
			return relations;
		}



	}
}
