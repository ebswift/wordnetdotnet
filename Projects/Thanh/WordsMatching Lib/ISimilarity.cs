using System;

namespace WordsMatching
{
	/// <summary>
	/// Summary description for IEditDistance.
	/// </summary>
	interface ISimilarity
	{
		float GetSimilarity(string string1, string string2);
	}
}
