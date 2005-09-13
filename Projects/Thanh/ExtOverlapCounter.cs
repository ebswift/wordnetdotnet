using System;
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

		public enum BackTracking 
		{
			NEITHER,
			UP,
			LEFT,
			UP_AND_LEFT
		}
		
		public int GetScore(string[] a, string[] b)
		{			
			if (a == null || b == null) return 0;
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

		private int ConsecutiveMeasure(int k)
		{
			//f(k)=k*a - b;
			return k*k;
		}

		private int LCSMC(ref string[] list1,ref string[] list2) 
		{
			int m=list1.Length ;
			int n=list2.Length ;
			
			int[ , ] lcs=new int[m+1, n+1];
			BackTracking[ , ] backTracer=new BackTracking[m+1, n+1];
			int[ , ] w=new int[m+1, n+1];
			int i, j;
			
			for(i=0; i <= m; ++i) 
			{
				lcs[i,0] = 0;
				backTracer[i,0]=BackTracking.UP;
				
			}
			for(j= 0; j <= n; ++j) 
			{
				lcs[0,j]=0;
				backTracer[0,j]=BackTracking.LEFT;				
			}

			for(i =1; i <= m; ++i) 
			{
				for(j=1; j <= n; ++j) 
				{ 
					if( list1[i-1].Equals(list2[j-1]) ) 
					{
						int k = w[i-1, j-1];
						//lcs[i,j] = lcs[i-1,j-1] + 1;
						lcs[i,j]=lcs[i-1,j-1] + ConsecutiveMeasure(k+1) - ConsecutiveMeasure(k)  ;
						backTracer[i,j]=BackTracking.UP_AND_LEFT;
						w[i,j] = k+1;						
					}
					else 
					{
						lcs[i,j] = lcs[i-1,j-1];
						backTracer [i,j] = BackTracking.NEITHER;
					}

					if( lcs[i-1,j] >= lcs[i,j] ) 
					{	
						lcs[i,j] = lcs[i-1,j];
						backTracer[i,j] = BackTracking.UP;
						w[i,j] = 0;
					}

					if( lcs[i,j-1] >= lcs[i,j] ) 
					{
						lcs[i,j] = lcs[i,j-1];
						backTracer [i,j] = BackTracking.LEFT;
						w[i,j] = 0;
					}
				}
			}
			
			i=m; 
			j=n;
			
			int score=lcs[i,j];

			//trace the backtracking matrix.
			while( i > 0 || j > 0 ) 
			{
				if( backTracer[i,j] == BackTracking.UP_AND_LEFT ) 
				{
					i--;
					j--;
					++_num;
					Trace.WriteLine(list1[i]) ;
					list1[i]="M" + _num;
					++_num;
					list2[j]="M" + _num;					
				}
				else if( backTracer[i,j] == BackTracking.UP ) 				
					i--;
				else if( backTracer[i,j] == BackTracking.LEFT ) 
					j--;

			}
			
			
			return score;
		}

	}
}
