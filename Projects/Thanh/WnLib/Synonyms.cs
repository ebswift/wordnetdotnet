using System;
using Wnlib;

namespace Wnlib
{
	/// <summary>
	/// Summary description for Synonyms.
	/// </summary>
	public class Synonym
	{
		public string Word;
		public int Sense;
		public Lexeme Synset;
		public Synonym(string w, int s)
		{this.Word=w; this.Sense=s;}
	}
}
