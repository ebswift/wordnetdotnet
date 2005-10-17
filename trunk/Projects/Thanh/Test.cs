using System;
using System.Diagnostics;
using System.Text.RegularExpressions ;

namespace WordsMatching
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Test
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			//
			// TODO: Add code to start application here
			//

			// TDMS 21 Sept 2005 - added dictionary path
			Wnlib.WNCommon.path = "C:\\Program Files\\WordNet\\2.1\\dict\\";

//Survey sheet in article Sentence similarity
//1. Shilpa is drinking pepsi.
//(a) Pepsi is being drunk by Shilpa
//(b) Niti is eating softy.
//(c) Shilpa loves drinking pepsi.
//2. IIT is located in Hauz Khas.
//(a) Hauz Khas has IIT in it.
//(b) Hauz Khas is located in IIT.
//(c) IIT in Hauz Khas has nice location.
//3. Ram purchased two books.
//(a) Two books were purchased by me.
//(b) I ate two sweets.
//(c) I purchased two books for Ram.
//4. I gave Mohan a book.
//(a) Mohan got a book.
//(b) I gave Ram a pen.
//(c) I gave Mohan a pen and a book.
//5. Deepa is running well.
//(a) Deepa runs well.
//(b) Rashi is walking.
//(c) Well run Deepa.
			Test t=new Test() ;			
		}

		public Test()
		{
			
			
			SemanticSimilarity semsim=new SemanticSimilarity() ;
			float score=semsim.GetScore("Defense Ministry", "Department of defence");
			
			//float score=semsim.GetScore("Pepsi is being drunk by Shilpa", "Niti is eating softy");
			
			
			int i=1;
			
			//Trace.WriteLine(match.Score) ;			
		}

	}
}
