// pulled from gblosser fork https://github.com/gblosser/wordnetdotnet
// added to expand the range of sample code we have access to
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Tokenize;
using Wnlib;
using WnLexicon;

namespace WordNetConsoleTest
{
	class Program
	{
		static void Main(string[] args)
		{
			Wnlib.WNCommon.path = @"..\..\WordNet\";
			var aPartsOfSpeech = ((Wnlib.PartsOfSpeech[])Enum.GetValues(typeof(Wnlib.PartsOfSpeech))).ToList();

			var aTokenizer = new EnglishRuleBasedTokenizer();
			Console.WriteLine("Enter a sentence to look up {enter to quit}:");
			var aSentence = Console.ReadLine();
			while (!string.IsNullOrEmpty(aSentence))
			{
				var aTokens = aTokenizer.Tokenize(aSentence);
				foreach (var aToken in aTokens)
				{
					var aLexicon = Lexicon.FindWordInfo(aToken, true);
					if (!string.IsNullOrEmpty(aLexicon.text))
						Console.WriteLine($"Root word is {aLexicon.text}");
				}
				Console.WriteLine("enter to end");
				aSentence = Console.ReadLine();
			}

			/*
			Console.WriteLine("Enter a noun to look up {enter to quit}:");
			var aWord = Console.ReadLine();


			while (!string.IsNullOrEmpty(aWord))
			{
				for (int i = 0; i <= 55; i++)
				{
					var aOptions = Opt.at(i);

					foreach (var aPartOfSpeech in aPartsOfSpeech)
					{
						var aSearch = new Search(aWord, true, PartOfSpeech.of(aPartOfSpeech), aOptions.sch, 5);
						if (aSearch.senses == null)
							continue;

						var aSensesEnumerator = aSearch.senses.GetEnumerator();
						var aSensesList = new List<string>();
						while (aSensesEnumerator.MoveNext())
						{
							aSensesList.Add(((SynSet) aSensesEnumerator.Current).pos.name);
						}
						if(aSensesList.Any())
						{
							Console.WriteLine($"Opt {aOptions.label}"); 
							Console.WriteLine($"\tSenses found: {string.Join(",", aSensesList)}");
						}
					}
				}
				Console.WriteLine("Enter a word to look up {enter to quit}:");
				aWord = Console.ReadLine();
			}
			*/
		}
	}
}
