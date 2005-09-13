/*
 Simple Ovelap counter - Bag of words method
 Author: Thanh Ngoc Dao - Thanh.dao@gmx.net
 Copyright (c) 2005 by Thanh Ngoc Dao.
*/

using System;

namespace WordsMatching
{
	public interface IOverlapCounter
	{
		int GetScore(string[] a, string[] b);
	}
	/// <summary>
	/// Summary description for OverlapCounter.
	/// </summary>
	/// 
	public class SimpleOverlapCounter: IOverlapCounter 
	{
		public SimpleOverlapCounter()
		{
		}

		private static int SimpleCount(string[] l1, string [] l2)
		{
			int count=0;
			//Tokeniser tok=new Tokeniser() ;
			//string[] l1=tok.Partition(a) ;
			//string[] l2=tok.Partition(b) ;
			int[] dx=new int[l1.Length + 1] ;

			for (int i=0; i < l1.Length; i++)
			{
				string word1=l1[i];
				for (int j=i+1; j < l1.Length - 1; j++)
				{ if (l1[i] == l1[j]) dx[j]=-1;}
				
				if (dx[i] == 0)
					foreach (string word2 in l2)
						if (word1 == word2)
						{
							++dx[i];
							break;
						}
						
				if (dx[i] > 0) ++count;
			}
						
			return count;
		}

		public int GetScore(string[] a, string[] b)
		{
			if (a == null || b == null) return 0;
			int score=SimpleCount (a, b);
			return score;
		}
	}
}
