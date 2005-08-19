using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using WnLexicon;
using Wnlib;
using System.Globalization;

namespace WordsMatching
{
	/// <summary>
	/// Summary description for WNDistance.
	/// </summary>
	public class WNRelatednessMatcher
	{
		const int DEPTH=6;
		static string[]  word=new string[2] ;	
		private static Thread[] _thread=new Thread[2] ;
		private static ThreadStart[] _threadStart=new ThreadStart[2] ;
		static ArrayList[] queue=new ArrayList[2] ;// List of hypernymy
		static ArrayList[] depth=new ArrayList[2] ;// Depth of node


		public WNRelatednessMatcher()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		private static Hashtable trace=new Hashtable() ;

		private string GetFirstMorph(string word, Wnlib.PartsOfSpeech pos)
		{
			word = word.ToLower();
			Wnlib.Index index = Wnlib.Index.lookup( word, Wnlib.PartOfSpeech.of( pos ) );

			// none found?
			if( index == null )
			{
				// check morphs
				Wnlib.MorphStr morphs = new Wnlib.MorphStr( word, Wnlib.PartOfSpeech.of( pos ) );
				string morph = "";
				while( ( morph = morphs.next() ) != null )
				{
					index = Wnlib.Index.lookup( morph, Wnlib.PartOfSpeech.of( pos ) );
					if( index != null )					
						return morph;//just get first morph
					
				}
				return string.Empty ;
			}
			else 						
				return word;
		}
		public static int GetPathLength(string word1, string word2)
		{
			word[0]=word1;				
			word[1]=word2;	
//			ArrayList opts=new ArrayList() ;
//			opts.Add(Opt.at(8)) ;
//			opts.Add(Opt.at(35)) ;
//			foreach (Opt item in opts)
//			{
//			}

			for(int i=0; i<2 ; i++)
			{
				queue[i]=new ArrayList() ;
				depth[i]=new ArrayList() ;
			}
			Opt opt=Opt.at(8);//search for hypernymy 12 == full, 8==brief;
//			_threadStart[0]=new ThreadStart(Search0);
//			_thread[0]=new Thread(_threadStart[0]);								
//			_thread[0].Start();									
			
			Search0();
			Search1();

			Search1();
			Search0();

			//			_threadStart[1]=new ThreadStart(Search1);
//			_thread[1]=new Thread(_threadStart[1]);								
//			_thread[1].Start();									

			return 0;
		}
				


		//search subsumer, a shared parent of two synsets.
		public static int FindSubsumer(Lexeme lexe, ArrayList hypernyms)
		{
			for(int i=0; i < hypernyms.Count; i++)
			{
				Lexeme l=(Lexeme)hypernyms[i];
				if (lexe.word == l.word && lexe.wnsns == l.wnsns)
					return i;
			}

			return -1;
		}
		
		public static void Search0()
		{
			Spread(0, Opt.at (8));
		}
		
		public static void Search1()
		{
			Spread(1, Opt.at (8));
		}

		 
		public static int Spread(int index, Opt opt)
		{
			int head=-1, tail=-1;
			queue[index]=new ArrayList() ;
			depth[index]=new ArrayList() ;

			Search se=new Search(word[index], true, opt.pos , opt.sch, int.Parse("0"));//
			foreach (object obj in se.lexemes)
			{
				DictionaryEntry dic = (DictionaryEntry) obj;

				Lexeme lex = (Lexeme) dic.Key;
				bool ok = true;
				foreach (SynSet syn in se.senses)
				{
					if (!ok) break;
					else
						foreach (Lexeme l in syn.words)
							if (lex.word == l.word)
							{
								if (word[1 - index] == l.word)
								{
									Trace.WriteLine("They are synonymy");
									return 1;
								}
								ok = false;
								break;
							}
				}

				foreach (Lexeme l in queue[index])
					if (lex.word == l.word) ok = false;

				if (ok && !trace.ContainsKey(lex))
				{
					++tail;
					lex.word = lex.word.Replace("_", " ");
					queue[index].Add(lex);
					depth[index].Add(1);
					trace.Add(lex, 1) ;					
				}
			}


			while (head < tail )
			{
				++head;
				Lexeme lexHead=(Lexeme)queue[index][head];
				int lexHeadDis=(int)depth[index][head]  ;
				if (lexHead.word == word[1 - index])
				{					
					Trace.WriteLine(lexHead.word + " is hypernym of " + word[index]) ;					
					return lexHeadDis;
				}
				else
				{
					int subsumer=FindSubsumer(lexHead, queue [1-index]);				
					if (subsumer >= 0)
					{
						int distance=lexHeadDis + (int)depth[1-index][subsumer] ;
						Trace.WriteLine("Hierachy shared common parent found : " + distance + " shared parent :" + lexHead.word) ;
						return distance;
					}
				}	
				se=new Search(lexHead.word, true, opt.pos , opt.sch, lexHead.wnsns); //lexHead.wnsns

				IDictionaryEnumerator enumerator=se.lexemes.GetEnumerator();

				while (enumerator.MoveNext())
				{
					Lexeme lex=(Lexeme) enumerator.Key;
					lex.word=lex.word.Replace("_", " ");

					if ((bool) enumerator.Value)
					{						
						bool ok=true;
						foreach (Lexeme l in queue[index])
							if (lex.word == l.word && lex.wnsns == l.wnsns) ok = false;

						if (ok)
						{
							foreach (SynSet syn in se.senses  )
							{			
								if (!ok) break;
								else
									foreach (Lexeme l in syn.words)							
										if (lex.word == l.word )  //do not consider sense number here
										{ok=false; break;}
						
							}

							if (!ok)
							{
								++tail;
								trace.Add(lex, lexHeadDis ) ;								
								depth[index].Add(lexHeadDis);								
								queue[index].Add(lex);

							}
							else if (lex.word != lexHead.word)
							{
								if (!trace.ContainsKey(lex))
								{
									++tail;									
									trace.Add(lex, lexHeadDis + 1) ;
									depth[index].Add(lexHeadDis + 1);									
									queue[index].Add(lex);
								}
							}
						}

					}

				}

			}
			return -1; //not found 
		}

	}
}
