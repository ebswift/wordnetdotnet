using System;
using System.Text.RegularExpressions;

namespace BrillTagger
{
	/// <summary>
	/// Summary description for Formatter.
	/// </summary>
	public class Formatter
	{
		public Formatter()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static string FormatText(string TheText, bool Clean) 
		{
			// This is not quite the orthodox Penn TreeBank conventions
			// but it will work and looks (to me) rather nicer, for example I don't like their "``" quotes
			TheText = (TheText + " ");
			TheText = TheText.Replace("([!$%^&*()+={}\\[\\]~#@:;<>,?/\\\\])", " $1 ");
			TheText = TheText.Replace("...", " & ");
			// In general we want to separate the . from the end of a word
			TheText = TheText.Replace("\\. ", " . ");
			// But
			// If you just replaced Tyrone J. Jones with Tyrone J . Jones, fix it
			TheText = TheText.Replace(" [A-Z] . ", " $1. ");
			// Keep a.m. e.g. Feel free to expand the list as much as you wish...
			TheText = TheText.Replace("(a.m|p.m|e.g|i.e|etc|p.s|U.S.A|Mr|Mrs|Ms|Dr|Prof|Inc|Co|Corp) \\.", "$1. ");
			TheText = TheText.Replace("``", "\"");
			TheText = TheText.Replace("\'\'", "\"");
			TheText = TheText.Replace("\"", " \" ");
			TheText = TheText.Replace("&", " & ");
			TheText = TheText.Replace("--", " -- ");
			// Open out Leading and Trailing single quotes
			// before you cope with 's and so forth
			TheText = TheText.Replace("([\\s])\'(\\w)", "$1 \' $2");
			TheText = TheText.Replace("([^ \'])\' ", "$1 \' ");
			// Note how useful Regex is for case sensitive words using the [xX] form
			TheText = TheText.Replace("\'([SsMmDd]) ", " \'$1 ");
			TheText = TheText.Replace("\'(ll|LL|re|RE|ve|VE) ", " \'$1 ");
			TheText = TheText.Replace("(n\'t|N\'T) ", " $1 ");
			TheText = TheText.Replace(" ([cC])annot", "$1an not");
			TheText = TheText.Replace(" ([dD])\'ye", " $1\' ye");
			TheText = TheText.Replace(" ([gG])imme ", " $im me ");
			TheText = TheText.Replace(" ([gG])onna ", " $1on na ");
			TheText = TheText.Replace(" ([gG])otta ", " $1ot ta ");
			TheText = TheText.Replace(" ([lL])emme ", " $1em me ");
			TheText = TheText.Replace(" ([mM])ore\'n", " $1ore \'n");
			TheText = TheText.Replace(" \'([tT])is", " $1 is ");
			TheText = TheText.Replace(" \'([tT])was", " $1 was ");
			TheText = TheText.Replace(" ([wW])anna ", " $1an na ");
			if (Clean) 
			{
				TheText = TheText.Replace(" [\'\"] ", " ");
			}
			while ((TheText.IndexOf("  ") != -1)) 
			{
				TheText = TheText.Replace("  ", " ");
			}
			return TheText.Trim();
		}
	}
}
