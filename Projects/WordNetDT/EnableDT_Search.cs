/*
 * Created by SharpDevelop.
 * User: Troy Simpson
 * Date: 15/02/2005
 * Time: 7:46 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Metaphone;
using ebswift;

namespace EnableDT_Classes
{
	/// <summary>
	/// Description of MyClass.
	/// </summary>
	public class EnableDT_Search
	{
		// setup search word
		private static Metaphone.ShortDoubleMetaphone mphone = new ShortDoubleMetaphone();
		// setup compare word
		private static ShortDoubleMetaphone mphonecompare = new ShortDoubleMetaphone();

		public static ArrayList wngrep(string wordPassed, bool isScrabble) // of string
		{
			int i;
			string tmpstr;

			ArrayList al = new ArrayList();
			
			if(isScrabble)
				tmpstr = wordPassed; // use only the letters in the word to choose word list
			else
				tmpstr = "abcdefghijklmnopqrstuvwxyz";
			
			for(i = 0; i < tmpstr.Length; i ++) {
				if((tmpstr[i] >= 'a' && tmpstr[i] <= 'z') || (tmpstr[i] >= 'A' && tmpstr[i] <= 'Z'))
					al.AddRange(forLetter(wordPassed, tmpstr[i]));
			}
				
			return al;
		}
		
		private static ArrayList forLetter(string wordPassed, char ch)
		{
			ArrayList r = new ArrayList();
			StreamReader inputFile = new StreamReader("..\\..\\..\\..\\3rd_Party_Tools_Data\\Fullable\\" + ch + ".lst");
//			StreamReader inputFile = new StreamReader(".\\" + ch + ".lst");
			inputFile.BaseStream.Seek(0L,SeekOrigin.Begin);
			inputFile.DiscardBufferedData();
			string word = wordPassed.Replace(" ","_");
			string line;
			char c = (char)200; // regex / scrabble
			char c2 = (char)201; // soundslike
			bool regexflag = false;
			bool soundslikeflag = false;
			int soundslikestrength = 0;

			// if the string ends with chr 200 the search is a regular expression
			if(word.EndsWith(c.ToString())) 
			{
				regexflag = true;
				word = word.Replace(c.ToString(), "");
			}

			if(word.EndsWith(c2.ToString())) 
			{
				soundslikeflag = true;
				word = word.Replace(c2.ToString(), "");
				//soundslikestrength = Convert.ToInt16(Regex.Replace(word, "[[^a-zA-Z]", ""));
				soundslikestrength = Convert.ToInt16(Regex.Replace(word, "[[^a-zA-Z]", ""));
				word = word.Replace(Regex.Replace(word, "[[^a-zA-Z]", ""), "");
			}
		
			if(! regexflag && ! soundslikeflag) // non-regex search
			{
				while ((line=inputFile.ReadLine())!=null)
				{
					int lineLen = line.Length;
					line = line.Substring(0,lineLen);
					// TODO: change the .IndexOf to allow wildcards
					//				if (line.IndexOf(word)>=0)
					try 
					{
						if (ebswift.ebString.vbLike(line, word))
							r.Add(line.Replace("_"," "));
					} 
					catch 
					{
					}
				}
			} 
			else if(regexflag) 
			{ // regex search
				Regex rg = new Regex("a", RegexOptions.IgnoreCase);

				try 
				{
					rg = new Regex(word, RegexOptions.IgnoreCase);
				}
				catch
				{
					MessageBox.Show("Malformed regular expression.");
					r.Add("error");
					return r;
				}

				while ((line=inputFile.ReadLine())!=null)
				{
					int lineLen = line.Length;
					line = line.Substring(0,lineLen);
					// TODO: change the .IndexOf to allow wildcards
					//				if (line.IndexOf(word)>=0)
					try 
					{
						Match m;

						m = rg.Match(line);

						if (m.Success)
							r.Add(line.Replace("_"," "));
					} 
					catch 
					{
					}
				}
			}
			else if(soundslikeflag) // sounds like search
			{
				mphonecompare.computeKeys(word);

				while ((line=inputFile.ReadLine())!=null)
				{
					int lineLen = line.Length;
					line = line.Substring(0,lineLen);
					try 
					{
						if (soundsLike(line)) //, word)) //, soundslikestrength))
							r.Add(line.Replace("_"," "));
					} 
					catch 
					{
					}
				}
			}
			return r;
		}

		// does a sounds like comparison based on a strength level
		internal static bool soundsLike(string wordnetstr) //, string searchstr) //, int strength)
		{
			mphone.computeKeys(wordnetstr);

			if (mphonecompare.PrimaryShortKey == mphone.PrimaryShortKey)
			{
				//Primary-Primary match, that's level 3 (strongest)
				return true;
			}
			
			//						if((mphonecompare.PrimaryShortKey == mphone.PrimaryShortKey || mphonecompare.AlternateShortKey == mphone.PrimaryShortKey) || (mphonecompare.AlternateShortKey == nullpointer.Metaphone.ShortDoubleMetaphone.METAPHONE_INVALID_KEY && (mphonecompare.PrimaryShortKey == mphone.AlternateShortKey || mphonecompare.AlternateShortKey == mphone.AlternateShortKey))) 
			//							return true;
			//						else
			//							return false;

			//			switch(strength)
			//			{
			//				case 1: //Alternate-Alternate match, that's level 1 (minimal)
			//					if (mphone.AlternateKey != null &&
			//						mphonecompare.AlternateKey != null) 
			//					{
			//						if (mphone.AlternateKey == mphonecompare.AlternateKey)
			//						{
			//							return true;
			//						} 
			//					}
			//					break;
			//
			//				case 2:
			//					if (mphonecompare.AlternateKey != null) 
			//					{
			//						if (mphonecompare.AlternateKey == mphone.PrimaryKey) 
			//						{
			//							//Alternate-Primary match, that's level 2 (normal)
			//							return true;
			//						} 
			//					}
			//     
			//					if (mphone.AlternateKey != null) 
			//					{
			//						if (mphonecompare.PrimaryKey == mphone.AlternateKey)
			//						{
			//							//Primary-Alternate match, that's level 2 (normal)
			//							return true;
			//						} 
			//					}
			//					break;
			//
			//				case 3:
			//					if (mphonecompare.PrimaryKey == mphone.PrimaryKey)
			//					{
			//						//Primary-Primary match, that's level 3 (strongest)
			//						return true;
			//					}
			//					break;
			//			}

			return false;
		}
	}
}
