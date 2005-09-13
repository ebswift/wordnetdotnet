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
			string s1="The main plication. pplication pplication pplication pplication pplication pplication pplication ";
			string s2="Add code to start application here pplication pplication pplication pplication pplication pplication pplication pplication pplication pplication pplication ";
			//Entertaiment Entail tail, fail, enter men end			
			s1="ZONGSHyryyyyyyyyyyyyyyyyyyyyyyyrtEN F 200ytrrrrrrrrrrrrrrrrrrr8 LZSJCJLytrrrrrrrrrrrrryrtyrt0645 50 0 00ytrrrrrrrrrrrrryyhjjjjjjjjjjjjjjjjj124 00144 000285 002 ZONGSHyryyyyyyyyyyyyyyyyyyyyyyyrtEN F 200ytrrrrrrrrrrrrrrrrrrr8 LZSJCJLytrrrrrrrrrrrrryrtyrt0645 50 0 00ytrrrrrrrrrrrrryyhjjjjjjjjjjjjjjjjj124 00144 000285 002ZONGSHyryyyyyyyyyyyyyyyyyyyyyyyrtEN F 200ytrrrrrrrrrrrrrrrrrrr8 LZSJCJLytrrrrrrrrrrrrryrtyrt0645 50 0 00ytrrrrrrrrrrrrryyhjjjjjjjjjjjjjjjjj124 00144 000285 002ZONGSHyryyyyyyyyyyyyyyyyyyyyyyyrtEN F 200ytrrrrrrrrrrrrrrrrrrr8 LZSJCJLytrrrrrrrrrrrrryrtyrt0645 50 0 00ytrrrrrrrrrrrrryyhjjjjjjjjjjjjjjjjj124 00144 000285 002";
			s2="ALFA ALFAROMERO145TDfdsfsdgfdgdfgggggggggggggggggggggggggggggggfsdfsddddddsdfdsfsdfsdfdsfsdfdsCAT ZAR93000002 40 1 01929  001730 005 0 00ytrrrrrrrrrrrrryyhjjjjjjjjjjjjjjjjj124 00144 000285 002005 0 00ytrrrrrrrrrrrrryyhjjjjjjjjjjjjjjjjj124 00144 000285 002005 0 00ytrrrrrrrrrrrrryyhjjjjjjjjjjjjjjjjj124 00144 000285 002005 0 00ytrrrrrrrrrrrrryyhjjjjjjjjjjjjjjjjj124 00144 000285 002005 0 00ytrrrrrrrrrrrrryyhjjjjjjjjjjjjjjjjj124 00144 000285 002005 0 00ytrrrrrrrrrrrrryyhjjjjjjjjjjjjjjjjj124 00144 000285 002005 0 00ytrrrrrrrrrrrrryyhjjjjjjjjjjjjjjjjj124 00144 000285 002";
			s1="2 JALAN PEMBERITA U1/49 \nTEMASYA INDUSTRIAL PARK \nGLENMARIE 40150 SHAH ALAM SEL";
			s2="D-3-2 ARCADIA APARTMENT \nUSJ 11/1 SUBANG JAYA \n47600 PETALING JAYA";
			//s1="ZONGSHEN F 2008 LZSJCJL0645 50 0 00124 00144 000285 002";
			//s2="ALFA ALFAROMERO145TDCAT ZAR93000002 40 1 01929 001730 005";
			//s1="aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc dddddddddddddddddddddddddddddddd";
			//s2="a b c d" ;

			s1="Entertaiment Entail tail, enter fail, use enter men end Detail	";
			s2="Retail detail entry point fai mention lure mention find" ;

			//s1="_incoming indicate the incomming left nodes of right nodes set";
			//s2="_outgoing indicate the outgoing right nodes of left nodes set";
			s1="boat";
			s2="truck";
			StemmerInterface stem=new PorterStemmer() ;
			Trace.WriteLine(stem.stemTerm("Transportation") );
			//MatchsMaker match=new MatchsMaker(s1, s2) ;
			
			//WNDistance.GetPathLength("car", "instrumentality") ;
			//WNRelatednessMatcher.GetPathLength("agent", "physical object") ;
			WNPathFinder f=new WNPathFinder() ;
			//f.GetPathLength("car",0, "fork", 0) ;
			Tokeniser tok=new Tokeniser() ;
			//string[] a=tok.Partition("pine cone") ;
			string[] a=tok.Partition("ministry of defence") ;
			string[] b=tok.Partition(" long  fruit of certain evergreen tree.");			
			OverlapRelatedness overlap=new OverlapRelatedness() ;
			int[] sense=overlap.Disambiguate(a) ;
			int i=1;
			//WNRelatednessMatcher.GetPathLength("living thing", "flora") ;
			//Trace.WriteLine(match.Score) ;			
		}

	}
}
