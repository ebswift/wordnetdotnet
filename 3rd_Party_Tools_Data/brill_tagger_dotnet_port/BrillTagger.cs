// Direct port to c# by Troy Simpson from Steven Abbott's VB.Net version.

using System;
using System.IO;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Collections.Specialized;
using BrillTagger;
using ebswift;

// This implementation of Eric Brill's (GNU licensed Copyright  1993 MIT) tagger
// owes much to various other implementations.
// In retrospect, Brill's C code is readable to a VB guy like me
// but I had to refer to other more modern (including Java) implementations
// (often found on Source Forge) before things made sense
// Interestingly, this implementation seems to be rather clear and compact
// partly because .Net is good, partly because it's always easier to code when you have 
// other people's good and bad code before you.
// You will find plenty of links and discussions on the Brill tagger via Google.
// This version uses the combined Wall Street Journal and Brown corpora supplied by Brill.
// This has classified >90,000 individual words from >1million words of text.
// Hugo Liu of MIT kindly let me use is much-expanded lexicon built on top of Brill's
// then I was able to add another 90,000 words from the Moby project, giving >300,000 words
// The tagging of the Moby words won't be quite as good as the others so feel free to remove them
// You can find them where Liu's alphabetised set finishes and a fresh alphabetised set begins
// The alphabetisation is not necessary for the tagger - it was for my convenience for checking out words
// As I'm English I've duplicated the 'color' and 'center' words for 'colour' and 'centre'.
// It makes no attempt to implement Brill's learning scheme because more modern corpora
// are closely protected so I have no access to them.
// Most good taggers (including Brill's) score ~96% accuracy on 'standard tests'
// but in the real world, with much more varied texts that don't match the starting corpora
// expect closer to 90% for this version.
// I considered adding Brill's N-Best scheme (hedging one's bets by adding extra possible tags) but
// it didn't seem to be of real help.
// Brill assumes Penn Treebank Formatting of the incoming text. My Format is close enough.
// It also assumes one sentence at a time, which is what I provide.

namespace BrillTagger
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class BrillTagger
	{
		private static Hashtable Lexicon = new Hashtable();
    
		//private static string [] TheNBest;
		//private static string [] TheWords;
		//private static string [] TheTags;
		//private static string [] TheRules;
		//private static string [] TheContext;

		private static StringCollection TheNBest = new StringCollection();
		private static StringCollection TheWords = new StringCollection();
		private static StringCollection TheTags = new StringCollection();
		private static StringCollection TheRules = new StringCollection();
		private static StringCollection TheContext = new StringCollection();
    
		private static int LastWord;

		public BrillTagger()
		{
			//
			// TODO: Add constructor logic here
			//
		}
    
		public static string BrillTagged(string TheSentence, bool DoLexical, bool DoContextual, bool DoClean, string LexiconFile, string LexicalRuleFile, string ContextualRuleFile) 
		{
			// , ByVal TheFlagBox As TextBox) As String
			int i;
			StringBuilder Tagged = new StringBuilder();
			Lexicon.Clear();
			TheNBest.Clear();
			TheWords.Clear();
			TheTags.Clear();
			TheRules.Clear();
			TheContext.Clear();

			TheSentence = Formatter.FormatText(TheSentence, DoClean);
			// Because we've done a FormatText it is easy to create individual words via Split
			// Lexical tagger requires the first word of the sentence to be S-T-A-R-T
			TheWords.AddRange(("S-T-A-R-T " + TheSentence).Split(' '));
			LastWord = (TheWords.Count - 1);
			//object TheTags;
			if ((Lexicon.Count < 1)) 
			{
				// TheFlagBox.Text = "Reading in the Lexicon and other files..."
				// TheFlagBox.Refresh()
				GetLexiconEtc(LexiconFile, LexicalRuleFile, ContextualRuleFile);
			}
			// TheFlagBox.Text = "Tagging..."
			// TheFlagBox.Refresh()
			DoBasicTagging();
			if (DoLexical) 
			{
				DoLexicalTagging();
			}
			if (DoContextual) 
			{
				// Contextual tagger starts with STAART
				TheWords[0] = "STAART";
				DoContextualTagging();
			}
			for (i = 1; (i <= LastWord); i++) 
			{
				if ((TheTags[i] != "")) 
				{
					Tagged.Append((TheWords[i] + ("/" + (TheTags[i] + " "))));
				}
				else 
				{
					Tagged.Append((TheWords[i] + " "));
				}
			}
			return Tagged.ToString();
		}
    
		public static void GetLexiconEtc(string LexiconFile, string LexicalRuleFile, string ContextualRuleFile) 
		{
			string lx;
			string [] lv;
			string [] tlist;
			int i = 0;
			int j;
			//StreamReader SR = new StreamReader((Application.StartupPath + "\\Lexiconlong.txt"));
			StreamReader SR = new StreamReader((Application.StartupPath + "\\" + LexiconFile));
			// I usually read in long files with SR.ReadToEnd then do a Split on VBNewLine
			// But in this case it is MUCH slower than doing it via ReadLine
			// And reading and hashing this way is MUCH faster than saving the serialized hash table
			// especially for a very long lexicon. Serializing a big hash table is VERY, VERY slow
			string s = SR.ReadLine();
			// TDMS 20 Oct 2005 - added check for empty string
			while (!(s == null) && !((string)s == "")) 
			{
				j = s.IndexOf(" ");
				tlist = s.Substring((j + 1)).Split(' ');
				Lexicon.Add(s.Substring(0, j), tlist);
				s = SR.ReadLine();
			}
			SR.Close();
			//SR = new StreamReader((Application.StartupPath + "\\LexicalRuleFile.txt"));
			SR = new StreamReader((Application.StartupPath + "\\" + LexicalRuleFile));
			lx = SR.ReadToEnd();
			SR.Close();
			lv = lx.Split('\n');

			for (i = 0; i <= (lv.Length - 1); i++) 
			{
				// TheRules[i] = lv[i];
				// TDMS 20 Oct 2005 - added check for empty string
				if((string)lv[i] != "")
					TheRules.Add((string)lv[i].Replace("\r", ""));
			}
			//SR = new StreamReader((Application.StartupPath + "\\ContextualRuleFile.txt"));
			SR = new StreamReader((Application.StartupPath + "\\" + ContextualRuleFile));
			lx = SR.ReadToEnd();
			SR.Close();
			lv = lx.Split('\n');
			//object TheContext;
			for (i = 0; (i <= (lv.Length - 1)); i++) 
			{
				// TDMS 20 Oct 2005 - added check for empty string
				if((string)lv[i] != "")
					//TDMS 17 Nov 2005 - fixed the rules matching by removing \r from each entry in the array - the last entry contained \r which breaks contextual comparisons
					TheContext.Add(lv[i].Replace("\r", ""));
			}
		}
    
		private static void DoBasicTagging() 
		{
			int i;
			string[] s;
			// If the word is in the lexicon, tag it with its first (most likely) tag
			// if not tag it as NN or NNP if it has a capital letter.
			// An unofficial rule for my convenience:
			// Ignore everything that doesn't start with a letter of the alphabet
			// except for something which starts with a number then make it CD
			// it will get changed to JJ if it contains a 'd' (e.g. 2nd) or a 't' (e.g. 31st)
			for (i = 0; (i <= LastWord); i++) 
			{
				if (ebString.vbLike(TheWords[i].Substring(0, 1), "[a-zA-Z\']")) 
				{
					if (Lexicon.ContainsKey(TheWords[i])) 
					{
						s = ((string[])(Lexicon[TheWords[i]]));
						TheTags.Add(s[0]);
					}
					else if (ebString.vbLike(TheWords[i].Substring(0, 1), "[a-z]")) 
					{
						TheTags.Add("NN");
					}
					else if ((TheWords[i] == "\'")) 
					{
						TheTags.Add("");
					}
					else 
					{
						TheTags.Add("NNP");
					}
				}
				else if (ebString.vbLike(TheWords[i].Substring(0, 1), "[0-9]")) 
				{
					// TDMS 18 Nov 2005 - changed unknown words to noun, which duplicates
					// functionality of the original Brill Tagger.  Numbers were being
					// incorrectly tagged as /CD instead of /JJ.
					TheTags.Add("NN");
//					TheTags.Add("CD");
				}
				else 
				{
					TheTags.Add("");
				}
			}
		}
    
		private static void DoLexicalTagging() 
		{
			// Go through each of the rules
			// Each of these will go through every word in the sentence to see if the rule applies
			// This is a lot of work, but it has to be done
			// The code is tedious and repetitive. But just think how horrendous it looked in C!
			// The code assumes that the rules are all perfectly formed, so don't hand edit the rule file!
			int i;
			int j;
			string[] SubRule;
			string SR0;
			string SR1;
			string SR2;
			string SR3 = null;
			string SR4 = null;
			string[] tlist;
			for (i = 0; (i 
				<= (TheRules.Count - 1)); i++) 
			{
				SubRule = TheRules[i].Split(' ');
				// We have to refer to the individual items in the rule
				// The code is much clearer if we name them now
				SR0 = SubRule[0];
				SR1 = SubRule[1];
				SR2 = SubRule[2];
				if ((SubRule.Length >= 4)) 
				{
					SR3 = SubRule[3];
				} else {
					SR3 = null;
				}
					if ((SubRule.Length == 5)) 
				{
					SR4 = SubRule[4];
				} else {
					SR4 = null;
				}
				for (j = 0; (j <= LastWord); j++) 
				{
					// I may be wrong on this but it makes sense to me to ONLY check if the word
					// is NOT in the lexicon. You can easily disable this check if you think I'm wrong
					// If the word is unknown then it's probably best to try the substitution
					tlist = ((string[])(Lexicon[TheWords[j]]));
//					if ((tlist == null)) 
//					{
						//  Change this to If True then... if you want everything checked
						if ((SR2.Substring(0, 1) != "f")) 
						{
							// Two types of rules take their choice from SR1 or SR2
							switch (SR1) 
							{
								case "haspref":
									if (TheWords[j].StartsWith(SR0)) 
									{
										TheTags[j] = SR3;
									}
									break;
								case "deletepref":
									if ((TheWords[j].StartsWith(SR0) && Lexicon.ContainsKey((SR0 + TheWords[j].Substring(SR0.Length))))) 
									{
										TheTags[j] = SR3;
									}
									break;
								case "addpref":
									if (Lexicon.ContainsKey((SR0 + TheWords[j]))) 
									{
										TheTags[j] = SR3;
									}
									break;
								case "hassuf":
									if (TheWords[j].EndsWith(SR0)) 
									{
										TheTags[j] = SR3;
									}
									break;
								case "deletesuf":
									if ((TheWords[j].EndsWith(SR0) && Lexicon.ContainsKey(TheWords[j].Substring(0, (TheWords[j].Length - SR0.Length))))) 
									{
										TheTags[j] = SR3;
									}
									break;
								case "addsuf":
									if (Lexicon.ContainsKey((TheWords[j] + SR0))) 
									{
										TheTags[j] = SR3;
									}
									// Not implemented as these depend on bigrams and I have not implemented them and the standard Brill sources come with no useful bigrams
									// Case "goodright"
									// Case "goodleft"
									break;
								case "char":
									if ((TheWords[j].IndexOf(SR0) != -1)) 
									{
										TheTags[j] = SR2;
									}
									break;
							}
						}
						else 
						{
							switch (SR2) 
							{
								case "fhaspref":
									if (((TheTags[j] == SR0) 
										&& TheWords[j].StartsWith(SR1))) 
									{
										TheTags[j] = SR4;
									}
									break;
								case "fdeletepref":
									if (((TheTags[j] == SR0) 
										&& (TheWords[j].StartsWith(SR1) && Lexicon.ContainsKey((SR1 + TheWords[j].Substring(SR1.Length)))))) 
									{
										TheTags[j] = SR4;
									}
									break;
								case "faddpref":
									if (((TheTags[j] == SR0) 
										&& Lexicon.ContainsKey((SR1 + TheWords[j])))) 
									{
										TheTags[j] = SR4;
									}
									break;
								case "fhassuf":
									if (((TheTags[j] == SR0) 
										&& TheWords[j].EndsWith(SR1))) 
									{
										TheTags[j] = SR4;
									}
									break;
								case "fdeletesuf":
									if (((TheTags[j] == SR0) 
										&& (TheWords[j].EndsWith(SR1) && Lexicon.ContainsKey(TheWords[j].Substring(0, (TheWords[j].Length - SR1.Length)))))) 
									{
										TheTags[j] = SR4;
									}
									break;
								case "faddsuf":
									if (((TheTags[j] == SR0) 
										&& Lexicon.ContainsKey((TheWords[j] + SR1)))) 
									{
										TheTags[j] = SR4;
									}
									// Not implemented as these depend on bigrams and I have not implemented them and the standard Brill sources come with no useful bigrams
									// Case "fgoodright"
									// Case "fgoodleft"
									break;
								case "fchar":
									if ((TheTags[j] == SR0)) 
									{
										if ((TheWords[j].IndexOf(SR1) != -1)) 
										{
											TheTags[j] = SR3;
										}
									}
									break;
							}
						}
					//}
				}
			}
		}
    
		private static void DoContextualTagging() 
		{
			int i;
			int j;
			int k;
			string[] SubRule;
			string SR0;
			string SR1;
			string SR2;
			string SR3 = null;
			string SR4 = null;
			string[] tlist;
			bool OKtoCheck;
			for (i = 0; (i 
				<= (TheContext.Count - 1)); i++) 
			{
				SubRule = TheContext[i].Split(' ');
				// We have to refer to the individual items in the rule
				// The code is much clearer if we name them now

				SR0 = SubRule[0];
				SR1 = SubRule[1];
				SR2 = SubRule[2];
				if ((SubRule.Length >= 4)) 
				{
					SR3 = SubRule[3];
				} 
				else 
				{
					SR3 = null;
				}
				if ((SubRule.Length == 5)) 
				{
					SR4 = SubRule[4];
				} else {
					SR4 = null;
				}
				for (j = 0; (j <= LastWord); j++) 
				{
					// The norm is to only check for a substitution if the new tag
					// already exists in the list of possible tags
					// If the word is unknown then it's probably best to try the substitution
					OKtoCheck = false;
					tlist = ((string[])(Lexicon[TheWords[j]]));
					if ((tlist == null)) 
					{
						OKtoCheck = true;
					}
					else 
					{
						for (k = 0; (k 
							<= (tlist.Length - 1)); k++) 
						{
							if ((tlist[k] == SR1)) 
							{
								OKtoCheck = true;
								break;
							}
						}
					}
					if (OKtoCheck) 
					{
						//  Change this to If True then... if you want everything checked
						switch (SR2) 
						{
							case "PREVTAG":
								if ((j > 0)) 
								{
									if ((TheTags[j] == SR0) && (TheTags[(j - 1)] == SR3)) 
									{
										TheTags[j] = SR1;
									}
								}
								break;
							case "PREV1OR2TAG":
								if ((j == 1)) 
								{
									if (((TheTags[j] == SR0) 
										&& (TheTags[(j - 1)] == SR3))) 
									{
										TheTags[j] = SR1;
									}
								}
								else if ((j > 1)) 
								{
									if (((TheTags[j] == SR0) 
										&& ((TheTags[(j - 2)] == SR3) 
										|| (TheTags[(j - 1)] == SR3)))) 
									{
										TheTags[j] = SR1;
									}
								}
								break;
							case "PREV1OR2OR3TAG":
								if ((j == 1)) 
								{
									if (((TheTags[j] == SR0) 
										&& (TheTags[(j - 1)] == SR3))) 
									{
										TheTags[j] = SR1;
									}
								}
								else if (((j == 2) 
									&& ((TheTags[j] == SR0) 
									&& ((TheTags[(j - 2)] == SR3) 
									|| (TheTags[(j - 1)] == SR3))))) 
								{
									TheTags[j] = SR1;
								}
								else if ((j > 2)) 
								{
									if (((TheTags[j] == SR0) 
										&& ((TheTags[(j - 3)] == SR3) 
										|| ((TheTags[(j - 2)] == SR3) 
										|| (TheTags[(j - 1)] == SR3))))) 
									{
										TheTags[j] = SR1;
									}
								}
								break;
							case "PREV2TAG":
								if ((j > 1)) 
								{
									if (((TheTags[j] == SR0) 
										&& (TheTags[(j - 2)] == SR3))) 
									{
										TheTags[j] = SR1;
									}
								}
								break;
							case "NEXTTAG":
								if ((j < LastWord)) 
								{
									if (((TheTags[j] == SR0) 
										&& (TheTags[(j + 1)] == SR3))) 
									{
										TheTags[j] = SR1;
									}
								}
								break;
							case "NEXT1OR2TAG":
								if ((j 
									== (LastWord - 1))) 
								{
									if (((TheTags[j] == SR0) 
										&& (TheTags[(j + 1)] == SR3))) 
									{
										TheTags[j] = SR1;
									}
								}
								else if ((j 
									< (LastWord - 2))) 
								{
									if (((TheTags[j] == SR0) 
										&& ((TheTags[(j + 2)] == SR3) 
										|| (TheTags[(j + 1)] == SR3)))) 
									{
										TheTags[j] = SR1;
									}
								}
								break;
							case "NEXT1OR2OR3TAG":
								if ((j == (LastWord - 1))) 
								{
									if (((TheTags[j] == SR0) 
										&& (TheTags[(j + 1)] == SR3))) 
									{
										TheTags[j] = SR1;
									}
								}
								else if (((j 
									== (LastWord - 2)) 
									&& ((TheTags[j] == SR0) 
									&& ((TheTags[(j + 2)] == SR3) 
									|| (TheTags[(j + 1)] == SR3))))) 
								{
									TheTags[j] = SR1;
								}
								else if ((j 
									< (LastWord - 2))) 
								{
									if (((TheTags[j] == SR0) 
										&& ((TheTags[(j + 3)] == SR3) 
										|| ((TheTags[(j + 2)] == SR3) 
										|| (TheTags[(j + 1)] == SR3))))) 
									{
										TheTags[j] = SR1;
									}
								}
								break;
							case "NEXT2TAG":
								if ((j 
									< (LastWord - 1))) 
								{
									if (((TheTags[j] == SR0) 
										&& (TheTags[(j + 2)] == SR3))) 
									{
										TheTags[j] = SR1;
									}
								}
								break;
							case "PREVBIGRAM":
								if ((j > 1)) 
								{
									if (((TheTags[j] == SR0) 
										&& ((TheTags[(j - 2)] == SR3) 
										&& (TheTags[(j - 1)] == SR4)))) 
									{
										TheTags[j] = SR1;
									}
								}
								break;
							case "NEXTBIGRAM":
								if ((j 
									< (LastWord - 1))) 
								{
									if (((TheTags[j] == SR0) 
										&& ((TheTags[(j + 1)] == SR3) 
										&& (TheTags[(j + 2)] == SR4)))) 
									{
										TheTags[j] = SR1;
									}
								}
								break;
							case "SURROUNDTAG":
								if (((j > 0) 
									&& (j < LastWord))) 
								{
									if (((TheTags[j] == SR0) 
										&& ((TheTags[(j - 1)] == SR3) 
										&& (TheTags[(j + 1)] == SR4)))) 
									{
										TheTags[j] = SR1;
									}
								}
								break;
							case "CURWD":
								if (((TheWords[j] == SR3) 
									&& (TheTags[j] == SR0))) 
								{
									TheTags[j] = SR1;
								}
								break;
							case "PREVWD":
								if ((j > 0)) 
								{
									if (((TheWords[(j - 1)] == SR3) 
										&& (TheTags[j] == SR0))) 
									{
										TheTags[j] = SR1;
									}
								}
								break;
							case "PREV1OR2WD":
								if ((j == 1)) 
								{
									if (((TheWords[(j - 1)] == SR3) 
										&& (TheTags[j] == SR0))) 
									{
										TheTags[j] = SR1;
									}
								}
								else if ((j > 1)) 
								{
									if ((((TheWords[(j - 1)] == SR3) 
										|| (TheWords[(j - 2)] == SR3)) 
										&& (TheTags[j] == SR0))) 
									{
										TheTags[j] = SR1;
									}
								}
								break;
							case "PREV2WD":
								if ((j > 1)) 
								{
									if (((TheWords[(j - 2)] == SR3) 
										&& (TheTags[j] == SR0))) 
									{
										TheTags[j] = SR1;
									}
								}
								break;
							case "NEXTWD":
								if ((j < LastWord)) 
								{
									if (((TheWords[(j + 1)] == SR3) 
										&& (TheTags[j] == SR0))) 
									{
										TheTags[j] = SR1;
									}
								}
								break;
							case "NEXT1OR2WD":
								if ((j 
									== (LastWord - 1))) 
								{
									if (((TheWords[(j - 1)] == SR3) 
										&& (TheTags[j] == SR0))) 
									{
										TheTags[j] = SR1;
									}
								}
								else if ((j 
									< (LastWord - 1))) 
								{
									if ((((TheWords[(j + 1)] == SR3) 
										|| (TheWords[(j + 2)] == SR3)) 
										&& (TheTags[j] == SR0))) 
									{
										TheTags[j] = SR1;
									}
								}
								break;
							case "NEXT2WD":
								if ((j 
									< (LastWord - 1))) 
								{
									if (((TheWords[(j + 2)] == SR3) 
										&& (TheTags[j] == SR0))) 
									{
										TheTags[j] = SR1;
									}
								}
								break;
							case "LBIGRAM":
								if ((j > 0)) 
								{
									if (((TheWords[(j - 1)] == SR3) 
										&& ((TheWords[j] == SR4) 
										&& (TheTags[j] == SR0)))) 
									{
										TheTags[j] = SR1;
									}
								}
								break;
							case "RBIGRAM":
								if ((j < LastWord)) 
								{
									if (((TheWords[j] == SR3) 
										&& ((TheWords[(j + 1)] == SR4) 
										&& (TheTags[j] == SR0)))) 
									{
										TheTags[j] = SR1;
									}
								}
								break;
							case "WDAND2BFR":
								if ((j > 1)) 
								{
									if (((TheWords[(j - 2)] == SR3) 
										&& ((TheWords[j] == SR4) 
										&& (TheTags[j] == SR0)))) 
									{
										TheTags[j] = SR1;
									}
								}
								break;
							case "WDAND2AFT":
								if ((j 
									< (LastWord - 1))) 
								{
									if (((TheWords[j] == SR3) 
										&& ((TheWords[(j + 2)] == SR4) 
										&& (TheTags[j] == SR0)))) 
									{
										TheTags[j] = SR1;
									}
								}
								break;
							case "WDPREVTAG":
								if ((j > 0)) 
								{
									if (((TheTags[(j - 1)] == SR3) 
										&& ((TheWords[j] == SR4) 
										&& (TheTags[j] == SR0)))) 
									{
										TheTags[j] = SR1;
									}
								}
								break;
							case "WDNEXTTAG":
								if ((j < LastWord)) 
								{
									if (((TheWords[j] == SR3) 
										&& ((TheTags[(j + 1)] == SR4) 
										&& (TheTags[j] == SR0)))) 
									{
										TheTags[j] = SR1;
									}
								}
								break;
							case "WDAND2TAGBFR":
								if ((j > 1)) 
								{
									if (((TheTags[(j - 2)] == SR3) 
										&& ((TheWords[j] == SR4) 
										&& (TheTags[j] == SR0)))) 
									{
										TheTags[j] = SR1;
									}
								}
								break;
							case "WDAND2TAGAFT":
								if ((j 
									< (LastWord - 1))) 
								{
									if (((TheWords[j] == SR3) 
										&& ((TheTags[(j + 2)] == SR4) 
										&& (TheTags[j] == SR0)))) 
									{
										TheTags[j] = SR1;
									}
								}
								break;
						}
					}
				}
			}
		}
	}
}
