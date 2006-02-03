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
			// TDMS 21 Sept 2005 - added dictionary path
			Wnlib.WNCommon.path = "C:\\Program Files\\WordNet\\2.1\\dict\\";

			Test t=new Test() ;			
		}

        void Test_1()
        {
            SemanticSimilarity semsim = new SemanticSimilarity();
            float score = 0;
            score = semsim.GetScore(
            "flora",
            "person");
            System.Console.WriteLine("Score: " + (score == 0.83F));

            score = semsim.GetScore(
             "boy",
             "teacher");
            System.Console.WriteLine("Score: " + (score == 0.57F));

            //is cursing the boy?
            score = semsim.GetScore(
            "boy",
            "animal");
            System.Console.WriteLine("Score: " + (score == 0.71F));
            
        }
        public Test() //NUnit missing! 
		{
            Test_1();
            //SemanticSimilarity semsim = new SemanticSimilarity();
            //float score = semsim.GetScore("Defense Ministry", "Department of defence"); //0.75
            //score = semsim.GetScore("Tom is a doctor", "Tom is a teacher");
            //score = semsim.GetScore("car", "auto");
			
			//semsim.GetScore("Pepsi is being drunk by Shilpa", "Niti is eating softy");
			
			//score = semsim.GetScore("Pepsi is being drunk by Shilpa", "Shilpa is drinking pepsi");
			
			int i=1;
			
			//Trace.WriteLine(match.Score) ;			
		}

	}
}
