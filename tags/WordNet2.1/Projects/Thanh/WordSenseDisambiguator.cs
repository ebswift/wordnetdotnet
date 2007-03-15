
/* Disambiguate word sense (Adapted Lesk based approach)
 * Author : Dao Ngoc Thanh , thanh.dao@gmx.net 
 * (c) Dao Ngoc Thanh, 2005
 */

using System;
using Wnlib;

namespace WordsMatching
{
	/// <summary>
	/// Summary description for WSDisambiguator.
	/// </summary>
	/// 
	public class MyWordInfo
	{
		public Wnlib.PartsOfSpeech Pos;
		public string Word;
		public int Sense;
		public int Frequency;
		public int SynsetIndex;

		public MyWordInfo(string word, Wnlib.PartsOfSpeech pos)
		{
			this.Word=word;
			this.Pos=pos;
            this.Sense =0;
		}
	}

	public class WordSenseDisambiguator
	{

		public WordSenseDisambiguator()
		{
		}

		const int THRESHOLD=0;
		const int CONTEXT_SIZE=8;//Local disambiguation within the context size 


		Tokeniser tokenize=new Tokeniser() ;		
			
		private string[][][][] _relCube ;//[words][senses][relations]

		private MyWordInfo[] _contextWords;
		private int[] _bestSenses;
		
		private int _overallScore=0;
		

		private void MyInit()
		{
			_relCube=new string[_contextWords.Length][][][];
			_bestSenses=new int[_contextWords.Length];
			for(int i=0; i < _bestSenses.Length; i++)
				_bestSenses[i]=-1;

			tokenize.UseStemming=true;

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
            Opt[] relatedness = null;
			for (int i=0; i < _contextWords.Length; i++)
			{
				WnLexicon.WordInfo wordInfo=WnLexicon.Lexicon.FindWordInfo( _contextWords[i].Word , true );

				if( wordInfo.partOfSpeech != Wnlib.PartsOfSpeech.Unknown )
				{
					if (wordInfo.text != string.Empty)
						_contextWords[i].Word =wordInfo.text ;
					Wnlib.PartsOfSpeech[] posEnum=(Wnlib.PartsOfSpeech[])Enum.GetValues( typeof(Wnlib.PartsOfSpeech) );
					
					bool stop=false;
					int senseCount=0;
                    
					for( int j=1; j < posEnum.Length && !stop; j++ )
					{
						Wnlib.PartsOfSpeech pos=posEnum[j];												
						
						if (wordInfo.senseCounts[j] > 0 && _contextWords[i].Pos == pos)					
						{																							
							senseCount=wordInfo.senseCounts[j];
                            relatedness = Relatedness.GetRelatedness(pos);
                            stop = relatedness != null;
							break;
						}
					}

					if (stop) 						
					{
						string[][][] tmp=Relatedness.GetAllRelatednessData (_contextWords[i].Word , senseCount, relatedness);						
						_relCube[i]=tmp;
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

					_overallScore += bestScoreOf_i;
				}

			
			for (int i=0; i < wordCount; i++)
			{
				if (_bestSenses[i] == -1)
				{
					WnLexicon.WordInfo wordInfo=WnLexicon.Lexicon.FindWordInfo( _contextWords[i].Word , true );
					if( wordInfo.partOfSpeech != Wnlib.PartsOfSpeech.Unknown && _contextWords[i].Pos != Wnlib.PartsOfSpeech.Unknown )
					{
						_bestSenses[i]=0;
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
		
		public MyWordInfo[] Disambiguate(MyWordInfo[] words)
		{			
			_contextWords=words;
			MyInit();
			Scoring_Overlaps();			
			for (int i=0; i < _contextWords.Length ; i++)			
				_contextWords[i].Sense=_bestSenses[i];
			
			return _contextWords;
		}


	}
}
