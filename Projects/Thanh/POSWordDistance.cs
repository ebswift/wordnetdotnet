using System;
using System.Collections;
using System.Diagnostics;
using Wnlib;
using System.Text.RegularExpressions;

namespace WordsMatching
{
	/// <summary>
	/// Summary description for PathLengthMeasure.
	/// </summary>
	public class POSWordDistance
	{
		public POSWordDistance()
		{
		}

		//const int DEPTH=6;
		const int NO_PATH=0xffff ;
		static readonly Opt IS_A_NOUN=Opt.at(11); //hypernymy and synonyms  12 FULL TREE
		static readonly Opt IS_A_VERB=Opt.at(35);//troponymy and synonyms 35

		private string[]  _word=new string[2] ;
		private int[]  _senseIndex=new int[2] ;
			
		ArrayList[] queue=new ArrayList[2] ;// List of hypernymy for nound and troponymy for verb
		ArrayList[] depth=new ArrayList[2] ;// Depth of node		
		//private MyPos[] _pos=new MyPos[2] ;


		private static Hashtable trace=new Hashtable() ;

		public float Similarity(MyWordInfo pos1, MyWordInfo pos2)
		{
			if (pos1.Pos != pos2.Pos || pos1.Pos == PartsOfSpeech.Unknown ) return 0;
            if (pos1.Word == pos2.Word) return 1;

			int length=GetPathLength(pos1, pos2);
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
			int length=0;
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

		public int GetPathLength(MyWordInfo pos1, MyWordInfo pos2)
		{
			_word[0]=pos1.Word;				
			_word[1]=pos2.Word;	
			_senseIndex[0]= pos1.Sense + 1;
			_senseIndex[1]= pos2.Sense + 1;
			int length=NO_PATH;
			switch (pos1.Pos )
			{
				case Wnlib.PartsOfSpeech.Noun:
				{																		
					length=Search_IS_A_Connection (IS_A_NOUN);
					break;
				}
				case Wnlib.PartsOfSpeech.Verb:
				{
					length=Search_IS_A_Connection (IS_A_VERB);
					break;
				}

			};						

								
			return length;
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
                if (path != NO_PATH) break;

				path=Add_WordSenses(sense, index, dpt);
                if (sense.senses != null && path == NO_PATH)
                    path = HierarchicalWalk(sense.senses, index, dpt + 1);
				
			}

			return path;
		}

		int IsContain(MyWordSense sen, ArrayList list)
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

                    if (_word[1 - index] == mysen.word && dpt == 1)                     
					{
						return dpt;
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
            Search se = new Search(_word[index], true, opt.pos, opt.sch, 0);//

            if (se.senses != null && se.senses.Count == 0 && se.morphs.Count > 0)
            {   
                IDictionaryEnumerator  getEnum=se.morphs.GetEnumerator ();

                while (getEnum.MoveNext())
                {
                    string rootWord = (string)getEnum.Key;

                    if ((Wnlib.Search)getEnum.Value != null)
                    {
                        se = (Wnlib.Search)getEnum.Value;
                        if (se.senses != null && se.senses.Count > 0) break;
                    }
                 }
            }

			int path=NO_PATH;

            //Regex r=new Regex("([\n])");
            //String [] tokens=r.Split(se.buf); 									
            
			if (se.senses != null)
				path=HierarchicalWalk (se.senses, index, 1);

			return path;

		}


	}
}
