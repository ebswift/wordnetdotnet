using System;

namespace Wnlib
{
	/// <summary>
	/// Summary description for Lexeme.
	/// </summary>
	public class Lexeme 
	{
		public string word;  // word in synset
		public int uniq;     // unique id in lexicographer file
		public int wnsns;    // sense number in wordnet: filled in during search
		internal Lexeme()
		{ 
		}
	}

}
