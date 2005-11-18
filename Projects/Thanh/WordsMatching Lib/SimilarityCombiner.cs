using System;

namespace WordsMatching
{
	/// <summary>
	/// cosine, dice, jaccard co-efficients
	/// aggregation
	/// </summary>
	public class SimilarityCombiner
	{

		static public float MatchingAverage(float[] scores, int size1, int size2)
		{
			if (size1 == 0 || size2 == 0) return 0;			
			float sum=0;
			foreach (float i in scores) sum += i;

			return (2*sum )/(size1 + size2);			

		}

		static public float Dice(float[] scores, int size1, int size2)
		{
			if (size1 == 0 || size2 == 0) return 0;
			float THRESHOLD=0.4F;
			int count=0;
			foreach (float i in scores)
				if (i >= THRESHOLD) ++count;

			return (2*count )/(size1 + size2);			
		}

	}
}
