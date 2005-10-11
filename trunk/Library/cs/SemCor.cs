using System;
using System.IO;

namespace Wnlib
{
	/// <summary>
	/// Summary description for SemCor.
	/// </summary>
	public class SemCor
	{
		public int semcor = 0;

		public SemCor(Lexeme lex, int hereiam)
		{
			string key = hereiam.ToString("d8") + " " + lex.wnsns;

            System.IO.StreamReader indexFile = new System.IO.StreamReader(Wnlib.WNDB.path + @"\index.sense");

			string semline = Wnlib.WNDB.binSearchSemCor(key, lex.word, indexFile);

		}
	}
}
