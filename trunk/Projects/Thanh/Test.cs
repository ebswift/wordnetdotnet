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

			Test t=new Test() ;			
		}

		public Test()
		{
			
			
			SemanticSimilarity semsim=new SemanticSimilarity() ;
			//float score=semsim.GetScore("Defense Ministry", "Department of defence");
			
			float score=0;//semsim.GetScore("Pepsi is being drunk by Shilpa", "Niti is eating softy");
			
			score = semsim.GetScore("Pepsi is being drunk by Shilpa", "Shilpa is drinking pepsi");
			
			int i=1;
			
			//Trace.WriteLine(match.Score) ;			
		}

	}
}
