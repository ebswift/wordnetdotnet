using System;
using System.Collections;
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
			// left-pad the integer with 0's into a string
			string key = hereiam.ToString("d8") + " " + lex.wnsns;

            System.IO.StreamReader indexFile = new System.IO.StreamReader(Wnlib.WNDB.path + @"\index.sense");

			// locate our word and key via a binary search
			string semline = Wnlib.WNDB.binSearchSemCor(key, lex.word, indexFile);

			string [] lexinfo = semline.Split(' ');
			semcor = Convert.ToInt16(lexinfo[lexinfo.GetUpperBound(0)]);
		}
	}
}
