/*
 * Heuristic Checking for abbreviation/acronym
 * Author: Dao Ngoc Thanh , thanh.dao@gmx.net
 * Copyright (c) 2006 Dao Ngoc Thanh
*/

using System;
using System.Text;
using System.Text.RegularExpressions;

namespace WordsMatching
{
    class AcronymChecker
    {

        public static int Min3(int a, int b, int c)
        {
            return Math.Min(Math.Min(a, b), c);
        }

        public static int ComputeEditDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] distance = new int[n + 1, m + 1];
            int cost = 0;

            if (n == 0) return m;
            if (m == 0) return n;

            for (int i = 0; i <= n; distance[i, 0] = i++) ;
            for (int j = 0; j <= m; distance[0, j] = j++) ;


            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    cost = (t.Substring(j - 1, 1) == s.Substring(i - 1, 1) ? 0 : 1); // all cost op of 1
                    distance[i, j] = Min3(distance[i - 1, j] + 1,
                        distance[i, j - 1] + 1,
                        distance[i - 1, j - 1] + cost);
                }
            }

            return distance[n, m];
        }
		
        
        static bool IsAcronym(string text)
        {        
        	Regex r = new Regex("([A-Z])([A-Z])*");
			MatchCollection mc=r.Matches(text);
			
			if (mc.Count >0 )
				return true;
			
			return false;
        	
        }
        
        public static float GetEditDistanceSimilarity(string string1, string string2)
        {
        	
            if ((Object)string1 == null || (Object)string2 == null || string2.Length == 0 || string1.Length == 0)
            {
                return 0.0F;
            }

            float dis = ComputeEditDistance(string1, string2);

            float maxLen = string1.Length;
            if (maxLen < (float)string2.Length)
                maxLen = string2.Length;

            float minLen = string1.Length;
            if (minLen > (float)string2.Length)
                minLen = string2.Length;

            if (dis + minLen == maxLen) //affixes : pre + suff + middle, acronym, abbreviation
                return 0.9F;

            if (maxLen == 0.0F)
                return 1.0F;
            else
            {
                return (float)Math.Round(1.0F - dis / maxLen, 2);
            }
        }		

    }
}
