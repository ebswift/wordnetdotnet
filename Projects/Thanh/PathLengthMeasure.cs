using System;
using System.Collections;
using System.Diagnostics;
using Wnlib;

namespace WordsMatching
{
	/// <summary>
	/// Summary description for PathLengthMeasure.
	/// </summary>
	public class PathLengthMeasure
	{
		public PathLengthMeasure()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		//const int DEPTH=6;
		const int NO_PATH=0xffff ;
		static readonly Opt IS_A_NOUN=Opt.at(11); //hypernymy and synonyms  12 FULL TREE
		static readonly Opt IS_A_VERB=Opt.at(35);//troponymy and synonyms

		private string[]  _word=new string[2] ;
		private int[]  _senseIndex=new int[2] ;
	
		private SynSet[] _sense=new SynSet[2] ;
		ArrayList[] queue=new ArrayList[2] ;// List of hypernymy for nound and troponymy for verb
		ArrayList[] depth=new ArrayList[2] ;// Depth of node		


		private static Hashtable trace=new Hashtable() ;

		public float Similarity1(string word1,int sense1, string word2, int sense2)
		{
			int length=GetPathLength(word1, sense1, word2, sense2);
			if (length != 0)
			{
				float tmp=1.0F/length;
				return (float)Math.Round(tmp , 2);
			}
			else
				return 0;		
		}

		public float Similarity2(string word1,int sense1,int total1, string word2, int sense2, int total2)
		{
			int length=GetPathLength(word1, sense1, word2, sense2);
			int w1=total1-sense1;
			int w2=total2-sense2;
			if (length != 0)
			{
				float tmp=(w1*w2)/length;
				return (float)Math.Round(tmp , 2);				
			}
			else
				return 0;
		}

		public int GetPathLength(SynSet s1, SynSet  s2)
		{
			_sense[0]=s1; _sense[1]=s2;
			_word[0]=_sense[0].words[_sense[0].whichword - 1].word ;				
			_word[1]=_sense[1].words[_sense[1].whichword - 1].word ;				
			_senseIndex[0]=_sense[0].words[_sense[0].whichword - 1].wnsns  ; // 0..
			_senseIndex[1]=_sense[1].words[_sense[1].whichword - 1].wnsns  ; // 0..
			int i=Search_IS_A_Connection (IS_A_NOUN);
			
			int	j=Search_IS_A_Connection (IS_A_VERB);
			
			return Math.Min(i, j);
		}

		public int GetPathLength(string word1,int sense1, string word2, int sense2)
		{
			_word[0]=word1;				
			_word[1]=word2;	
			_senseIndex[0]= sense1 + 1;
			_senseIndex[1]= sense2 + 1;
			int i=Search_IS_A_Connection (IS_A_NOUN);
			
			int	j=Search_IS_A_Connection (IS_A_VERB);
			
			return Math.Min(i, j);
		}
				                                                                                                                                                                            
		private void Flush()
		{
			for(int i=0; i<2 ; i++)
			{
				queue[i]=new ArrayList() ;				
				depth[i]=new ArrayList() ;				
				queue[i].Add(new MyWordSense(_word [i], _senseIndex[i], 0)) ;				
				depth[i].Add(0);
			}			
		}

		class MyWordSense
		{
			internal string word;
			internal int sense;
			internal int depth;

			internal MyWordSense(string word, int index, int depth)
			{
				this.word =word.ToLower();
				this.sense =index;
				this.depth =depth;
			}
		}

		private int Search_IS_A_Connection(Opt type)
		{
			Flush();
			int i=FindPath(0, type ); //build source tree
			if (i == NO_PATH || i == -1)			
				i=FindPath(1, type );	//build target tree and make connection
			return i;
		}


		private int HierarchicalWalk(SynSetList senList, int index, int dpt)
		{		
			int path=NO_PATH;
			foreach (SynSet sense in senList)
			{						
				path=Add_WordSenses(sense, index, dpt);
				if (path == -1 && sense.senses != null)				
					path=HierarchicalWalk(sense.senses, index, dpt + 1);
				else 
					break;
				
			}
			return path;
		}

		private int IsContain(MyWordSense sen, ArrayList list)
		{						
			for (int i=0; i < list.Count ; i++)			
			{				
				MyWordSense syn=(MyWordSense )list[i];
				if (syn.sense == sen.sense && syn.word.Equals(sen.word) )
					return i;
			}
			return -1;
		}


		private int Add_WordSenses(SynSet ss, int index, int dpt)
		{						
			foreach (Lexeme word in ss.words)
			{							
				MyWordSense mysen=new MyWordSense(word.word.Replace("_", " "), word.wnsns, dpt) ;
				int senInd=IsContain(mysen, queue[index]) ;
				if (senInd == -1)
				{
					queue[index].Add(mysen);
					depth[index].Add(dpt) ;
					
					if (_word[1 - index] == mysen.word  && dpt == 1) 
					{
						return 1;
					}

					int subsumer=IsContain(mysen, queue[1 -index]);
					if (subsumer != -1)
					{												
						int distance=dpt + ((MyWordSense )(queue[1- index][subsumer])).depth - 1;
						if (distance == 0) distance=1;
						subsumer=distance;					
						return subsumer;
					}				
				}
				else
				{
					MyWordSense oldsen=(MyWordSense )queue[index][senInd];
					if (oldsen.depth > dpt)
					{
						oldsen.depth=dpt;
					}
				}
			}			
			return NO_PATH;
		}

		public int FindPath(int index, Opt opt)
		{			
			Search se=new Search(_word[index], true, opt.pos , opt.sch, _senseIndex [index]);//
			int path=NO_PATH;
			if (se.senses != null)
				path=HierarchicalWalk (se.senses, index, 1);

			return path;

		}


	}
}
