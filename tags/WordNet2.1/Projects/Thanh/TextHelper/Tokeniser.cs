/*
Tokenization
Author: Thanh Ngoc Dao - Thanh.dao@gmx.net
Copyright (c) 2005 by Thanh Ngoc Dao.
*/

using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace WordsMatching
{
	/// <summary>
	/// Summary description for Tokeniser.
	/// Partition string off into subwords
	/// </summary>
	public class Tokeniser
	{

        private bool _useStemming = false;
        public bool UseStemming
        {
            set { value=_useStemming; }
        }

		private void Normalize_Casing(ref string input)
		{
			//if it is formed by Pascal/Carmel casing
			for(int i=0; i < input.Length; i++)
			{
				if (Char.IsSeparator(input[i]))
					input=input.Replace(input[i].ToString() , " ") ;
			}
			int idx=1;
			while (idx < input.Length - 2)
			{
				++idx;								
				if (
					(Char.IsUpper(input[idx])   
					&& Char.IsLower(input[idx + 1]))
					&& 
					(!Char.IsWhiteSpace(input[idx - 1]) && !Char.IsSeparator(input[idx - 1]) )
					)
				{
					input=input.Insert(idx, " ") ; 
					++idx;
				}
			}
		}

		public string[] Partition(string input)
		{
            Regex r = new Regex("([ \\t{}():;._,\\-! \"?\n])");

            Normalize_Casing(ref input);
            input = input.ToLower();

            String[] tokens = r.Split(input);

            ArrayList filter = new ArrayList();

			for (int i=0; i < tokens.Length ; i++)
			{
				MatchCollection mc=r.Matches(tokens[i]);
				if (mc.Count <= 0 && tokens[i].Trim().Length > 0 					 
					&& !StopWordsHandler.IsStopWord(tokens[i]) )								
					filter.Add(tokens[i]) ;
				
				
			}

			return (string[])filter.ToArray( typeof( string ) );
			
		}

		
		public Tokeniser()
		{
            StopWordsHandler stop=new StopWordsHandler() ;
		}
	}
}
