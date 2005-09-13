using System;

namespace WordsMatching
{
	/// <summary>
	/// This class implements the Longest common sub-string problem
	/// </summary>
	public class ExtOverlapCounter
	{
		public ExtOverlapCounter()
		{
		}

		private const int NEITHER=0;
		private const int UP=1;
		private const int LEFT=2;
		private const int UP_AND_LEFT=3;

		public static string GetLCS(string[] a, string[] b) 
		{
			int n=a.Length ;
			int m=b.Length ;

			int[ , ] s=new int[n+1, m+1];
			int[ , ] r=new int[n+1, m+1];
			int i, j;

			// It is important to use <=, not <.  The next two for-loops are initialization
			for(i = 0; i <= n; ++i) 
			{
				s[i,0] = 0;
				r[i,0] = UP;
			}
			for(j = 0; j <= m; ++j) 
			{
				s[0,j] = 0;
				r[0,j] = LEFT;
			}

			// This is the main dynamic programming loop that computes the score and
			// backtracking arrays.

			for(i = 1; i <= n; ++i) 
			{
				for(j = 1; j <= m; ++j) 
				{ 
	
					if( a[i-1].Equals(b[j-1]) ) 
					{
						s[i,j] = s[i-1,j-1] + 1;
						r[i,j] = UP_AND_LEFT;
					}

					else 
					{
						s[i,j] = s[i-1,j-1] + 0;
						r[i,j] = NEITHER;
					}

					if( s[i-1,j] >= s[i,j] ) 
					{	
						s[i,j] = s[i-1,j];
						r[i,j] = UP;
					}

					if( s[i,j-1] >= s[i,j] ) 
					{
						s[i,j] = s[i,j-1];
						r[i,j] = LEFT;
					}
				}
			}

			// The length of the longest substring is S[n,m]
			i = n; 
			j = m;
			int pos = s[i,j] - 1;
			string concat="";
			string[] lcs = new string[ pos+1 ];

			// Trace the backtracking matrix.
			while( i > 0 || j > 0 ) 
			{
				if( r[i,j] == UP_AND_LEFT ) 
				{
					i--;
					j--;
					lcs[pos--] = a[i];						
				}
	
				else if( r[i,j] == UP ) 
				{
					i--;
				}
	
				else if( r[i,j] == LEFT ) 
				{
					j--;
				}
			}

			foreach (string ss in lcs) concat += " " + ss;
			
			return concat ;
		}

	}
}
