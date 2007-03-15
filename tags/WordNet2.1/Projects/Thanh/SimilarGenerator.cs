/* Generate similar sentences of a given setence
 * Author : Dao Ngoc Thanh , thanh.dao@gmx.net 
 * (c) Dao Ngoc Thanh.
 * Methods Used: Simulated Annealing & BackTracking generator
 * 
 * Acknowledgements: To J. Martin for the reuse of his original function "FindSynonyms" .
 * 
 */

using System;
using System.Collections;
using Wnlib;
using WordsMatching;

namespace SimilarSentence
{
	/// <summary>
	/// This class inputs a sentence and produces a list of approximately similar sentences
    /// 
	/// </summary>
	public class SimilarGenerator
	{
		private string _originalSentence;
		const int CONTEXT_SIZE=6;//Local disambiguation within the context size 
		private string[][][][] _dictInfo ;//[words][relations][tokens]
		
		Tokeniser tokenize=new Tokeniser() ;

		ArrayList list=new ArrayList() ;
		MyWordInfo[] _myPos;
		int _numItems=0;
		int _numWord;
		private string bestSentence;
		int bestScore=0;
		private MyWordInfo[][] _alterWord;
		private int[][] dx;
		private int[] _selected;
		const int _max=50;

		private MyWordInfo[] _contextWords;		
		private Opt[] _relatedness=null;
		private int _overallScore=0;					
		private int[][][][] _scores;//[i][alter_i][j][alter_j]

		public SimilarGenerator(MyWordInfo[] pos, string originalSentence)
		{
			_myPos=pos;
			_originalSentence=originalSentence;
			MyInit();			
			Generate();
			list=k_best_sentence;
			
			//
			// TODO: Add constructor logic here
			//
		}
		

		public ArrayList GetResult
		{
			get
			{	
				list.Insert(0, bestSentence) ;
				return list;
			}
		}
		
		public MyWordInfo[] FindSynonyms(ref MyWordInfo pos, bool includeMorphs)
		{
			pos.Word = pos.Word.ToLower();
			Wnlib.Index index = Wnlib.Index.lookup( pos.Word, PartOfSpeech.of( pos.Pos  ) );
		
			if( index == null )
			{
				if( !includeMorphs )
					return null;

				Wnlib.MorphStr morphs = new Wnlib.MorphStr( pos.Word, Wnlib.PartOfSpeech.of( pos.Pos  ) );
				string morph = "";
				while( ( morph = morphs.next() ) != null )
				{
					index = Wnlib.Index.lookup( morph, Wnlib.PartOfSpeech.of( pos.Pos  ) );
					pos.Word=morph;
					if( index != null )
						break;
				}
			}

			
			if( index == null )
				return null;
			
			return LookupCandidates( index, pos );
		}

		public static int GetSynsetIndex(string word, PartsOfSpeech pos)
		{
			word=word.ToLower() ;
			//word=RemoveBadChars (word);
			Wnlib.Index index=Wnlib.Index.lookup( word, PartOfSpeech.of(pos) );
			
			if( index == null )
			{
				Wnlib.MorphStr morphs=new Wnlib.MorphStr(word, Wnlib.PartOfSpeech.of( pos  ) );
				string morph = "";
				while( ( morph = morphs.next() ) != null )
				{
					index = Wnlib.Index.lookup(morph, Wnlib.PartOfSpeech.of(pos) );
					if( index != null )
						break;
				}
			}			

			if (index == null) return -1;
			else 
				return 0;
		}

		class CompareLexeme : IComparer
		{
			public int Compare(object x, object y) //descent sorting
			{
				return ((MyWordInfo)y).Frequency - ((MyWordInfo)x).Frequency;
			}
		}

		private MyWordInfo[] LookupCandidates(Wnlib.Index index, MyWordInfo pos )
		{						
			if (pos.Sense < 0) pos.Sense=1;						
			SynSet synset=new Wnlib.SynSet( index.offs[pos.Sense - 1 ], index.pos , index.wd, null , pos.Sense - 1);					
						
			ArrayList lexemes=new ArrayList() ;
			ArrayList synIndex=new ArrayList() ;

			foreach (Lexeme obj in synset.words)
			{
				lexemes.Add(obj) ;
				synIndex.Add(index.offs[pos.Sense - 1 ]);
			}
			
			if (index.offs.Length > 1)
			{
				if (lexemes.Count <= 1)
				{
					for(int i=0; i < index.offs.Length; i++ )
					{				
						synset=new Wnlib.SynSet( index.offs[i], index.pos, index.wd, null, i );

						foreach (Lexeme obj in synset.words)
						{
							synIndex.Add(index.offs[i]);
							lexemes.Add(obj) ;
						}
					}
				}
				else
				{
					synset=new Wnlib.SynSet( index.offs[0], index.pos, index.wd, null, 0 );
					int count=0; //get top most frequency word senses
					foreach (Lexeme obj in synset.words)
					{
						lexemes.Add(obj) ;
						synIndex.Add(index.offs[0]);
						++count;
						if (count > 4) break;
					}

				}
			}
			
			ArrayList sortedSet=new ArrayList() ;
			Hashtable trace=new Hashtable() ;
			int hasSem=0;
			for (int i = 0; i < lexemes.Count; i++)
			{				
				Lexeme word=(Lexeme)lexemes[i];				
				word.word=word.word.ToLower() ;

				int senIndex=(int)synIndex[i];
				if (senIndex != -1  && word.wnsns > 0)
				{
					word.semcor=new Wnlib.SemCor(word, senIndex);
					lexemes[i]=word;					
					++hasSem;
				}

				if (!trace.ContainsKey(word.word) )					
				{					
					if ((word.semcor != null &&  word.semcor.semcor  > 0 ) || (hasSem < 4))
					{
						trace[word.word]=1;
						sortedSet.Add(word) ;
					}
				}
				//catch
				{}
			}
			
			Lexeme[] words=(Lexeme[])sortedSet.ToArray( typeof(Lexeme) );						

			ArrayList candidates=new ArrayList();

			for( int i=0; i < words.Length; i++ )
			{
				string word=words[i].word.Replace("_", " " );				
				if( word[0] <= 'Z' ) continue;

				MyWordInfo newpos=new MyWordInfo(word, pos.Pos) ;
				newpos.Sense=words[i].wnsns;
				if (words[i].semcor != null)
					newpos.Frequency=words[i].semcor.semcor;
				else
					newpos.Frequency=0;

				candidates.Add( newpos);								
			}

			if (!trace.ContainsKey (index.wd))
				candidates.Add(pos) ;

			if (candidates.Count > 1)
			{
				CompareLexeme comparer=new CompareLexeme();
				candidates.Sort(comparer);
			}
			

			return (MyWordInfo[])candidates.ToArray( typeof(MyWordInfo) );
		}

		
		private int GetNeighbour(MyWordInfo[] current, out MyWordInfo[] trial)
		{			
			trial=(MyWordInfo[])current.Clone() ;

			int wordIndex=random.Next(current.Length);
			if (_alterWord[wordIndex] != null )
			{
				int candIndex=random.Next(_alterWord[wordIndex].Length) ;
				
				_selected[wordIndex]=candIndex;
				_contextWords[wordIndex]=_alterWord[wordIndex][candIndex];
				
				if (!Read_WordSenseInfo (wordIndex))  return -1;

				trial[wordIndex]=_alterWord[wordIndex][candIndex];
				int overall=0;
				for (int i=0; i < trial.Length; i++)
				{
					overall += ComputeScore (i);
				}

				return overall;
			}

			return -1;
		}
		

		private int InitialSentence(out MyWordInfo[] current)
		{
			current=new MyWordInfo[_myPos.Length] ;
			int overall=0;
			for (int i=0; i < current.Length; i++)
				if (_alterWord[i] != null && _alterWord[i].Length > 0)
			{
				for (int j = 0; j < _alterWord[i].Length; j++)
				{
					_selected[i]=j;
					_contextWords[i]=_alterWord[i][j];	
				
					if (Read_WordSenseInfo (i))
					{
						int score=ComputeScore(i);				
						overall +=score;

						break;
					}

				}
			}

			return overall;
		}

		private bool Metropolis(float delta, float temperature)
		{
			double p=random.NextDouble() ;
			return (p <= Math.Exp( delta / temperature));
			//return (delta >= 0) && (p.NextDouble() <= Math.Exp(- delta / temperature));

		}

		private float GetCost(int num)
		{
			return (float)1.0f/(1 + num);
		}

		private Random random;
		private void IterativeGenerate(int trialNum, float descentFactor)
		{
			float temperature=1;
			float ANNEAL_SCHEDULE=0.9F;
			int MAX_TRIAL=100;
			int MAX_SUCCEED=10; // 1/10 of N_TRIAL

			MyWordInfo[] current;
			int current_cost=InitialSentence(out current);
			Add_Sentence(Convert.ToInt32(current_cost) );

			random=new Random() ;
			for (int i=0; i < 100; i++)
			{								
				int nsuccess=0;
				for (int j=0; j < MAX_TRIAL; j++)
				{
					MyWordInfo[] trial;
					int new_cost=GetNeighbour (current,out trial);

					if ( new_cost != -1)
					{						
						///float delta=current_cost - new_cost;
						float delta=new_cost - current_cost;
						//float delta=GetCost(trial_cost) - GetCost(current_cost);

						if (delta > 0 || Metropolis (delta, temperature)) //accept trial higher than current or with a probability
						{
							current_cost=new_cost;
							current=trial;
							Add_Sentence(new_cost) ;
							++nsuccess;
							
						}						
					}
				}
				
				temperature *= ANNEAL_SCHEDULE;
			}
		}

		private void Generate()
		{				
			_alterWord=new MyWordInfo[_myPos.Length][] ;
			
			_numWord=_myPos.Length;
			dx=new int[_numWord][] ;
			_selected=new int[_numWord] ;

			for(int i=0; i <_myPos.Length; i++)				
			{		
				_selected[i]=-1;
				MyWordInfo pos=_myPos[i];				
				if (pos.Pos != PartsOfSpeech.Unknown && pos.Sense != -1)
				{
					_alterWord[i]=FindSynonyms(ref pos , true );
					
					if (_alterWord[i] != null)
					{
						foreach(MyWordInfo poss in _alterWord[i])						
							poss.Pos=pos.Pos;
						
						dx[i]=new int[_alterWord[i].Length] ;
						_dictInfo[i]=new string[_alterWord[i].Length][][] ;

					}				
				}
			}
			
			_scores=new int[_myPos.Length][][][] ;
			for(int i=0; i <_myPos.Length; i++)
				if (_alterWord[i] != null)
			{				
				_scores[i]=new int[_alterWord[i].Length][][] ;
				for(int a_i=0; a_i < _alterWord[i].Length; a_i++)					
				{
					_scores[i][a_i]=new int[_myPos.Length][] ;
					
					for(int j=0; j <_myPos.Length; j++)
						if (_alterWord[j] != null)	
					{
						_scores[i][a_i][j]=new int[_alterWord[j].Length] ;
					}
				}
			}

			_numItems=0 ;
			list.Clear() ;
			bestScore=0;
			bestSentence=string.Empty;
			IterativeGenerate(100, 0.9F);
			//TryAll (0);
			
			list.Insert(0, bestSentence) ;

		}
		
		private ArrayList k_best_sentence=new ArrayList() ;
		private ArrayList k_best_score=new ArrayList() ;

		private void Add_Sentence(int score)
		{
			++_numItems ;
			MyWordInfo[] pos=new MyWordInfo[_myPos.Length] ;
			string newsen="";
			string[] words=new string[_numWord] ;
			for (int i=0; i <_numWord ; i++)
			{									
				string word=string.Empty ;

				if (_selected[i] == -1 )
				{					
					word=_myPos[i].Word;
				}
				else
				{						
					word=_alterWord[i][_selected[i]].Word;										
				}
				words[i]=word;
				pos[i]=new MyWordInfo(word, _myPos[i].Pos) ;
			}

			newsen=string.Format(_originalSentence, words) ;
			
			if (score > bestScore)
			{
				bestSentence=newsen + " " + score;
				bestScore=score;
			}

			if (k_best_sentence.Count < 500)
			{
				newsen=newsen + " " + score;
				if (!k_best_sentence.Contains(newsen) )
				{
					k_best_sentence.Add(newsen);
					k_best_score.Add(score);
				}
			}
			else
			{
				int min=10000000;
				int rem=-1;
				for (int j=0; j < k_best_sentence.Count ; j++)
				{
					if ((int)k_best_score[j] < min)
					{
						min=(int)k_best_score[j];
						rem=j;
					}
				}
				newsen=newsen + " " + score;
				if (!k_best_sentence.Contains(newsen) && rem != -1)				
				{
					k_best_sentence.RemoveAt(rem) ;
					k_best_score.RemoveAt(rem) ;

					k_best_sentence.Add(newsen);
					k_best_score.Add(score);					
				}

			}							
			
		}

		private void BackTracking(int index)
		{
			//if (_numItems > _max) return;
			
			if (_numItems > 1000) return;
			if (index == _numWord)
			{

				Add_Sentence(_overallScore);

				return;
			}

			if (_alterWord[index] != null && _alterWord[index].Length > 0)
			{
				for (int j = 0; j < _alterWord[index].Length; j++)
					if (dx[index][j] == 0)
					{
						dx[index][j]=1;
						_selected[index]=j;
						_contextWords[index]=_alterWord[index][j];
						int delta=0;
						if (Read_WordSenseInfo (index)) delta=ComputeScore(index);
						_overallScore +=delta;

						BackTracking(index + 1);

						_selected[index]=-1;
						_overallScore -=delta;
						_contextWords[index]=null;						
						dx[index][j]=0;
					}
			}
			else
			{
				_selected[index] = -1;
				BackTracking(index + 1);
			}
		}

		private int ComputeScore(int currentIndex)
		{
			int total=0;
			{
				if (_dictInfo[currentIndex] == null) return 0;
				int med=CONTEXT_SIZE/2;
				
				for (int i=currentIndex - med; i < currentIndex; i++)
					if ( i >= 0 && _dictInfo[i] != null && _selected[i] >= 0)
					{
						if (_scores[i][_selected[i]][currentIndex][_selected[currentIndex]] > 0)
							total += _scores[i][_selected[i]][currentIndex][_selected[currentIndex]];
						else
						{
							int score=ScoringSensePair (_dictInfo[i][_selected[i]], _dictInfo[currentIndex][_selected[currentIndex]]);
							total += score;
							if (score > 0 )
							{
								_scores[i][_selected[i]][currentIndex][_selected[currentIndex]]=score;
								_scores[currentIndex][_selected[currentIndex]][i][_selected[i]]=score;
								
								for (int a_i=0; a_i < _alterWord[i].Length; a_i++ )
									if (_alterWord[i][a_i].SynsetIndex == _alterWord[i][_selected[i]].SynsetIndex)
								{
									for (int a_j=0; a_j < _alterWord[currentIndex].Length; a_j++ )
										if (_alterWord[currentIndex][a_j].SynsetIndex == _alterWord[currentIndex][_selected[currentIndex]].SynsetIndex)
									{
										_scores[i][a_i][currentIndex][a_j]=score;
										_scores[currentIndex][a_j][i][a_i]=score;
									}
								}
							}
						}
					}

			}

			return total;
		}		

		private void MyInit()
		{
			tokenize.UseStemming=true;
			_contextWords=new MyWordInfo[_myPos.Length] ;
			_dictInfo=new string[_contextWords.Length][][][];			

			for (int i=0; i < _myPos.Length; i++)				
			{			
				++_myPos[i].Sense ;					
			}								
		}

		private bool Read_WordSenseInfo(int wordIndex)
		{
			if (_dictInfo[wordIndex][_selected[wordIndex]] != null) return true;

            _relatedness = Relatedness.GetRelatedness(_contextWords[wordIndex].Pos);

			//if (stop) 						
			{
                string[][] tmp = Relatedness.GetRelatednessGlosses(_contextWords[wordIndex].Word, _contextWords[wordIndex].Sense, _relatedness);						
				_dictInfo[wordIndex][_selected[wordIndex]]=tmp;

				for (int i=0; i < _dictInfo[wordIndex].Length ; i++)				
					if (_alterWord[wordIndex][i].SynsetIndex == _contextWords[wordIndex].SynsetIndex)
				{
					_dictInfo[wordIndex][i]=tmp;
				}
			}

			if (_dictInfo[wordIndex][_selected[wordIndex]] != null && _dictInfo[wordIndex][_selected[wordIndex]].Length > 0 ) 
				return true;
			else 
				return false;

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
			
			try
			{
				int m=sense1.Length , n=sense2.Length ;
				for(int i=0; i < m; i++)
				{
					for(int j=0; j < n; j++)
					{
						score +=GetOverlap(sense1[i], sense2[j]) ;
					}
				}
			}catch (Exception e)
			{
				int uu=0;
			}

			return score;
		}


		
		private string RemoveBadChars(string s)
		{
			string[] badChars=new string[]{"=>", "==","=","->",">","+",";",",","_","-","."} ;
			foreach(string ch in badChars)			
				s=s.Replace(ch, " ") ;

			return s;
		}
		


	}
}
