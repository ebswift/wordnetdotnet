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
			int num=0;
			
			for (int i=0; i < l1.Length; i++)
			{												
				for (int j=0; j < l2.Length; j++)
				if (l1[i] == l2[j])
				{
					++num;
					l1[i]="T_" + num;
					++num;
					l2[j]="T_" + num;
					++count;
					break;
				}									
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
