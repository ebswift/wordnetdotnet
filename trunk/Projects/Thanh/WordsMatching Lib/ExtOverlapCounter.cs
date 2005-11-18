/*
Overlap counter with new scoring mechanism (maximal consecutive with ZipF distribution law )
Author: Thanh Ngoc Dao - Thanh.dao@gmx.net
Copyright (c) 2005 by Thanh Ngoc Dao.
*/

using System.Diagnostics;

namespace WordsMatching
{
	/// <summary>
	/// Summary description for ExtOverlapCounter.
	/// </summary>
	public class ExtOverlapCounter:IOverlapCounter 
	{
		
		private int _num=0;
		public ExtOverlapCounter()
		{
		}

		public enum Back 
		{
			NEITHER,
			UP,
			LEFT,
			UP_AND_LEFT
		}
		
		public int GetScore(string[] a, string[] b)
		{			
			if (a == null || b == null || a.Length == 0 || b.Length == 0) return 0;
			string[] l1=(string[])a.Clone()  ;
			string[] l2=(string[])b.Clone()  ;			

			int count=0;
			_num=0;
			do
			{
				int score=LCSMC (ref l1,ref l2);
				count=count + score;
				if (score == 0) break;
			}while (true);
			
			return count;
		}

		private int ConsecutiveScore(int k)
		{
			//f(k)=k*a - b;
			return k*k;
		}

		private int LCSMC(ref string[] list1,ref string[] list2) 
		{
			int m=list1.Length ;
			int n=list2.Length ;
			
			int[ , ] lcs=new int[m+1, n+1];
			Back[ , ] backTracer=new Back[m+1, n+1];
			int[ , ] w=new int[m+1, n+1];
			int i, j;
			
			for(i=0; i <= m; ++i) 
			{
				lcs[i,0]=0;
				backTracer[i,0]=Back.UP;
				
			}
			for(j= 0; j <= n; ++j) 
			{
				lcs[0,j]=0;
				backTracer[0,j]=Back.LEFT;				
			}

			for(i =0; i < m; i++) 
			{
				for(j=0; j < n; j++) 
				{ 
					if( list1[i] == list2[j] ) 
					{
						int k=0;
						int prev=0;
						if (i > 0 && j > 0 )
						{
							k= w[i-1, j-1];
							prev=lcs[i-1,j-1];
						}
										
						lcs[i,j]=prev + ConsecutiveScore(k+1) - ConsecutiveScore(k)  ;
						backTracer[i,j]=Back.UP_AND_LEFT;
						w[i,j] = k+1;						
					};
					
				{
					if (i > 0 && (lcs[i - 1, j] > lcs[i, j] || (lcs[i,j] == 0 &&  lcs[i - 1, j] > 0)))
					{
						lcs[i, j]=lcs[i - 1, j];
						backTracer[i, j]=Back.UP;
						w[i, j]=0;
					};
					
					if (j > 0 && (lcs[i, j - 1] > lcs[i, j] || (lcs[i,j] == 0 &&  lcs[i , j-1] > 0)))
					{
						lcs[i, j]=lcs[i, j - 1];
						backTracer[i, j]=Back.LEFT;
						w[i, j]=0;
					}
				}
				}
			}
			
			i=m-1; 
			j=n-1;
			
			string subseq="";
			int score=lcs[i,j];

			//trace the backtracking matrix.
			while( i >= 0 && j >= 0 ) 
			{
				if (backTracer[i,j] == Back.NEITHER) break;
				if( backTracer[i,j] == Back.UP_AND_LEFT ) 
				{
					subseq = list1[i] + subseq;
					Trace.WriteLine(i + " " + list1[i] + " " + j) ;
					++_num;
					Trace.WriteLine(list1[i]) ;
					list1[i]="T" + _num;
					++_num;
					list2[j]="T" + _num;					

					i--;
					j--;
					
					
				}
	
				else if( backTracer[i,j] == Back.UP ) 
				{
					i--;
				}
	
				else if( backTracer[i,j] == Back.LEFT ) 
				{
					j--;
				}
			}
			

						
			
			return score;
		}

	}
}
