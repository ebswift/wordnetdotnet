/*
 Semantic between two phrasals/sentences
 Author: Thanh Ngoc Dao - Thanh.dao@gmx.net
 Copyright (c) 2005 by Thanh Ngoc Dao.
*/

using System;

namespace WordsMatching
{
	/// <summary>
	/// 
	/// </summary>
	public class SemanticSimilarity
	{
		private int[] _senses1, _senses2;
		float[ , ] _cost;
		string[] _source, _target;
		private int m, n;

		public SemanticSimilarity()
		{
			StopWordsHandler stopword=new StopWordsHandler() ;
		}
		

		private void MyInit()
		{
			
			AdaptedLesk dis=new AdaptedLesk() ;
			_senses1=dis.Disambiguate(_source) ;
			_senses2=dis.Disambiguate(_target) ;
			
			m=_senses1.Length; n=_senses2.Length ;
			_cost =new float[m, n] ;

			for (int i=0; i < m; i++)
			{
				int s1=_senses1 [i];
				if (s1 == -1 ) s1=0;

				string word1=_source[i];
				for (int j=0; j < n; j++)
				{
					int s2=_senses2 [j];					
					if (s2 == -1 ) s2=0;

					string word2=_target[j];
					PathLengthSimilarity is_a=new PathLengthSimilarity() ;
					float weight=is_a.Similarity1(word1, s1, word2, s2)  ;
					if (weight <= 0)
					{
						//EditDistance ed=new EditDistance() ;
						//weight=ed.GetSimilarity(word1, word2) ;
					}

					_cost[i, j]=weight;					
				}
			}
		}

		public float GetScore(string string1, string string2)		
		{			
			Tokeniser tok=new Tokeniser() ;
			_source=tok.Partition(string1) ;
			_target=tok.Partition(string2) ;
			if (_source.Length == 0 || _target.Length == 0 )
				return 0F;
			
			MyInit();
			
			BipartiteMatcher match=new BipartiteMatcher(_source , _target, _cost) ;
			float[] matches=match.GetMatches() ;

			float score=SimilarityCombiner.MatchingAverage(matches, m, n) ;
			return score;	
		}
	}
}
