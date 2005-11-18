using System;
using Wnlib;

namespace WordsMatching
{
	/// <summary>
	/// Summary description for WSDisambiguator.
	/// </summary>
	/// 
	public class MyPos
	{
		public Wnlib.PartsOfSpeech Pos;
		public string Word;
		public int Sense;
		public int Frequency;
		public int SynsetIndex;

		public MyPos(string word, Wnlib.PartsOfSpeech pos)
		{
			this.Word=word;
			this.Pos=pos;
		}
	}

	public class POSTaggerWSD
	{

		public int GetOverallScore
		{
			get
			{
				return _overallScore;
			}
		}

		public POSTaggerWSD()
		{
		}

		const int THRESHOLD=0;
		const int CONTEXT_SIZE=8;//Local disambiguation within the context size 

		static Opt[] NOUN_RELATIONS=new Opt[] { Opt.at(8) , //hyper
												  Opt.at(14), //holo
												  Opt.at(19), //mero
												  Opt.at(12) //hypo												
											  } ;
		static Opt[] VERB_RELATIONS=new Opt[] {
												  Opt.at(31),//hyper
												  Opt.at(36)//tropo // may be 38
											  } ;
		static Opt[] ADJECTIVE_RELATIONS=new Opt[] {
													   Opt.at(0)												  
												   } ;

		static Opt[] ADVEB_RELATIONS=new Opt[] {
												   Opt.at(48)												  
											   } ;

		Tokeniser tokenize=new Tokeniser() ;		
			
		private string[][][][] _relCube ;//[words][senses][relations]

		private MyPos[] _contextWords;
		private int[] _bestSenses;
		private Opt[] _priorRelations=null;
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
					for( int j=0; j < posEnum.Length && !stop; j++ )
					{
						Wnlib.PartsOfSpeech pos=posEnum[j];												
						
						if (wordInfo.senseCounts[j] > 0 && _contextWords[i].Pos == pos)					
						{																							
							senseCount=wordInfo.senseCounts[j];

							switch (pos)
							{
								case Wnlib.PartsOfSpeech.Noun:
								{
									stop=true;
									_priorRelations = NOUN_RELATIONS;
									break;
								}
								case Wnlib.PartsOfSpeech.Verb:
								{
									stop=true;
									_priorRelations = VERB_RELATIONS;						
									break;
								}
								case Wnlib.PartsOfSpeech.Adj:
								{
									stop=true;
									_priorRelations = ADJECTIVE_RELATIONS;						
									break;
								}
								case Wnlib.PartsOfSpeech.Adv:
								{
									stop=true;
									_priorRelations = ADVEB_RELATIONS;						
									break;
								}				

							};						

							break;
						}
					}

					if (stop) 						
					{
						string[][][] tmp=GetAllRelations(_contextWords[i].Word , senseCount );						
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
		
		public MyPos[] Disambiguate(MyPos[] poses)
		{			
			_contextWords=poses;
			MyInit();
			Scoring_Overlaps();			
			for (int i=0; i < _contextWords.Length ; i++)			
				_contextWords[i].Sense=_bestSenses[i];
			
			return _contextWords;
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
						
			string[] toks=tokenize.Partition(rels);

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
			string[] toks=tokenize.Partition(gloss) ;

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
