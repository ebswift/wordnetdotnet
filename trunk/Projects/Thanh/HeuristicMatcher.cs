/*
Maximize the total weight of bipartite grapth 
Heuristic method
Author: Thanh Ngoc Dao - Thanh.dao@gmx.net
Copyright (c) 2005 by Thanh Ngoc Dao.
*/

using System;

namespace WordsMatching
{
	/// <summary>
	/// Summary description for HeuristicMatcher.
	/// </summary>
	public class HeuristicMatcher
	{
		public HeuristicMatcher()
		{
		}
		
		public static float[][] Transpose(float[][] simMatrix)
		{
			if (simMatrix == null)
				return null;
			int m=simMatrix.Length;
			int n=simMatrix[0].Length;
			float[][] transMatrix=new float[n][];
			for (int i=0; i < n; i++)
			{
				transMatrix[i]=new float[m];
			}
			for (int i=0; i < m; i++)
			{
				for (int j=0; j < n; j++)
					transMatrix[j][i]=simMatrix[i][j];
			}
			
			return transMatrix;
		}
		
		
		public static float ComputeSetSimilarity(float[][] simMatrix, int setStrategy, float threshold)
		{
			if (simMatrix == null)
				return 0.0F;
			int m=simMatrix.Length;
			int n=simMatrix[0].Length;
			float sim=0.0F;
			if (setStrategy == 1)
			{
				float maxSim_i=0.0F;
				float maxSim_j=0.0F;
				float sumSim_i=0.0F;
				float sumSim_j=0.0F;
				for (int i=0; i < m; i++)
				{
					maxSim_i=0.0F;
					for (int j=0; j < n; j++)
						if (maxSim_i < simMatrix[i][j])
							maxSim_i=simMatrix[i][j];
					
					sumSim_i += maxSim_i;
				}
				
				for (int j=0; j < n; j++)
				{
					maxSim_j=0.0F;
					for (int i=0; i < m; i++)
						if (maxSim_j < simMatrix[i][j])
							maxSim_j=simMatrix[i][j];
					
					sumSim_j += maxSim_j;
				}
								
				sim=(sumSim_i + sumSim_j)/(float) (m + n);
			}
			else if (setStrategy == 2)
			{
				int matchCount_i=0;
				int matchCount_j=0;
				for (int i=0; i < m; i++)
				{
					for (int j=0; j < n; j++)
					{
						if (simMatrix[i][j] <= threshold)
							continue;
						matchCount_i++;
						break;
					}
				}
				
				for (int j=0; j < n; j++)
				{
					for (int i=0; i < m; i++)
					{
						if (simMatrix[i][j] <= threshold)
							continue;
						matchCount_j++;
						break;
					}
				}
								
				sim=(float) (matchCount_i + matchCount_j)/(float) (m + n);
			}
			return sim;
		}
		
		public static float ComputeSetSimilarity(float[][] simMatrix, int setStrategy)
		{
			float sim=0.0F;
			if (setStrategy == 1)
				sim=ComputeSetSimilarity(simMatrix, 1, 0.0F);
			else
				sim=ComputeSetSimilarity(simMatrix, 2, 0.5F);
			return sim;
		}
		
	}	

}
