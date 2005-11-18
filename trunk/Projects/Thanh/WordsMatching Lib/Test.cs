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
			
			AdaptedLesk dis=new AdaptedLesk() ;
			StopWordsHandler so=new StopWordsHandler() ;
			int[] diss=dis.Disambiguate(new string[2]{"taste","banana"} ) ;
			//POSTaggedPathLengthMeasure me=new POSTaggedPathLengthMeasure() ;
			PathLengthSimilarity sim=new PathLengthSimilarity() ;
			//float path=me.GetPathLength("eat",0, "taste",0) ;
			float path1=sim.GetPathLength("eat",0, "taste",0) ;

			//me.GetPathLength("car",0, "fork",0) ;
			//me.GetPathLength("car",0, "bike",1) ;
			//me.GetPathLength("ministry",2, "department",0) ;
			SemanticSimilarity semsim=new SemanticSimilarity() ;
			float score=semsim.GetScore("He eat banana", "He taste banana");
			
			//float score=semsim.GetScore("Pepsi is being drunk by Shilpa", "Niti is eating softy");
			
			
			int i=1;
			
			//Trace.WriteLine(match.Score) ;			
		}

	}
}
