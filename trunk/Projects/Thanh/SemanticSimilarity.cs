using System;

namespace WordsMatching
{
	/// <summary>
	/// Summary description for SemanticSimilarity.
	/// </summary>
	public class SemanticSimilarity
	{
		private int[] _senses1, _senses2;
		float[ , ] _matrix;
		string[] _source, _target;

		public SemanticSimilarity()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		private void MyInit()
		{
			ExtendedLesk dis=new ExtendedLesk() ;
			_senses1=dis.Disambiguate(_source) ;
			_senses2=dis.Disambiguate(_target) ;
			int m=_senses1.Length, n=_senses2.Length ;
			
			_matrix =new float[m, n] ;

			for (int i=0; i < m-1; i++)
			{
				int s1=_senses1 [i];
				string word1=_source[i];
				for (int j=0; j < n; j++)
				{
					int s2=_senses2 [i];					
					string word2=_target[j];
					WNPathFinder is_a=new WNPathFinder() ;
					int weight=is_a.GetPathLength(word1, s1, word2, s2)  ;
					_matrix[i, j]=weight;					
				}
			}
		}

		float GetScore(string string1, string string2)		
		{
			Tokeniser tok=new Tokeniser() ;
			_source=tok.Partition(string1) ;
			_target=tok.Partition(string1) ;
			MyInit();

			return 0;	
		}
	}
}
