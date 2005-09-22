/*
Maximize the total weight of bipartite grapth 
Author: Thanh Ngoc Dao - Thanh.dao@gmx.net
Copyright (c) 2005 by Thanh Ngoc Dao.
*/

using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace WordsMatching
{
	/// <summary>
	/// Summary description for StringMatcher.
	/// </summary>
	/// 

	public class BipartiteMatcher
	{				
		private string[] _leftTokens, _rightTokens;		
		private float[,] _cost;
		private float[] leftLabel, rightLabel;
		private int[] _previous, _incomming, _outgoing; //connect with the left and right
		
		private bool[] _leftVisited, _rightVisited;
		int leftLen, rightLen;
		bool _errorOccured=false;
		
		public BipartiteMatcher(string[] left, string[] right, float[ , ] cost)
		{
			if (left == null || right == null || cost == null)
			{
				_errorOccured=true;
				return;
			}

			_leftTokens=left;
			_rightTokens=right;
			if (_leftTokens.Length > _rightTokens.Length)
			{
				float [ , ] tmpCost=new float[_rightTokens.Length , _leftTokens.Length] ;
				for(int i=0; i < _rightTokens.Length ; i++)				
					for(int j=0; j < _leftTokens.Length ; j++)					
						tmpCost[i, j]=cost[j, i];
									
				_cost=(float[ , ]) tmpCost.Clone() ;

				string[] tmp=_leftTokens;
				_leftTokens=_rightTokens;
				_rightTokens=tmp;
			}
			else
				_cost=(float[ , ]) cost.Clone() ;
			

			MyInit();

			Make_Matching();
		}

		private void MyInit()
		{	
			Initialize();

			_leftVisited=new bool[leftLen + 1] ;
			_rightVisited=new bool[rightLen+1] ;
			_previous=new int[(leftLen+rightLen)+2] ;

		}

		private void Initialize()
		{
			leftLen=_leftTokens.Length - 1;
			rightLen=_rightTokens.Length - 1;
			
			leftLabel=new float[leftLen+1] ;
			rightLabel=new float[rightLen+1] ;
			for (int i=0; i < leftLabel.Length; i++) leftLabel[i]=0;
			for (int i=0; i < rightLabel.Length; i++) rightLabel[i]=0;
			
			//init distance
			for (int i=0; i <= leftLen; i++)
			{
				float maxLeft=float.MinValue;
				for(int j=0; j <= rightLen; j++)
				{		
					_cost[i, j]=(float)Math.Round(_cost[i, j], 2);
					if (_cost[i, j] > maxLeft) maxLeft=_cost[i, j];
				}		
				
				leftLabel[i]=maxLeft;
			}	

		}

		private void Flush()
		{
			for (int i=0; i < _previous.Length; i++) _previous[i]=-1;
			for (int i=0; i < _leftVisited.Length; i++) _leftVisited[i]=false;			
			for (int i=0; i < _rightVisited.Length; i++) _rightVisited[i]=false;
		}
		
		bool stop=false;
		bool FindPath(int source)
		{
			Flush();
			stop=false;
			Walk(source);
			return stop;

		}

		void Increase_Matchs(int li, int lj)
		{
			int[] tmpOut=(int[])_outgoing.Clone() ;
			int i,j,k;
			i=li; j=lj;
			_outgoing[i]=j; _incomming[j]=i;
			if (_previous[i] != -1)
			{
				do
				{
					j=tmpOut[i];
					k=_previous[i];
					_outgoing[k]=j; _incomming[j]= k;
					i=k;
				}while (_previous[i] != -1);
			}
		}
				

		private void Walk(int i)
		{			
			_leftVisited[i]=true;
			
			for (int j=0; j <= rightLen; j++)	
				if (stop) return;
				else
				{
					float tmp=(float)Math.Round(leftLabel[i] + rightLabel[j], 2);
					if (!_rightVisited[j] && (tmp == _cost[i, j]))
					{
						if (_incomming[j] == -1)// if found a path
						{					
							stop=true;
							Increase_Matchs(i, j);	
							return;
						}
						else
						{
							int k=_incomming[j];
							_rightVisited[j]=true;
							_previous[k]=i;
							Walk (k);					
						}
					}
				}
		}

		#region BreadFirst
		//		int FindPath(int source)
		//		{
		//			int head, tail, idxHead=0;
		//			int[] visited=new int[(leftLen+rightLen)+2] , 
		//				q=new int[(leftLen+rightLen)+2] ;
		//			head=0;
		//			for (int i=0; i < visited.Length; i++) visited[i]=0;
		//			Flush ();
		//								
		//			head=-1;
		//			tail=0;
		//			q[tail]=source;
		//			visited[source]=1;
		//			leftVisited[source]=true;
		//			int nMerge=leftLen + rightLen + 1;
		//
		//			while (head <= tail)
		//			{
		//				++head;
		//				idxHead=q[head];
		//
		//
		//				for (int j=0; j <= (leftLen + rightLen + 1); j++)
		//					if(visited[j] == 0)
		//				{
		//					if (j > leftLen) //j is stay at the RightSide
		//					{
		//						int idxRight=j - (leftLen + 1);
		//						if (idxHead <= leftLen &&  (leftLabel[idxHead] + rightLabel[idxRight] == cost[idxHead, idxRight]))
		//						{
		//							++tail;
		//							q[tail]=j;
		//							visited[j]=1;
		//							previous[j]=idxHead;
		//							rightVisited[idxRight]=true;
		//							if (In[idxRight] == -1) // pretty good, found a path															
		//								return j;		
		//							
		//						}
		//					}
		//					else if ( j <= leftLen) // is stay at the left
		//					{
		//						if (idxHead > leftLen && In[idxHead - (leftLen + 1)] == j)
		//						{
		//							++tail;
		//							q[tail]=j;
		//							visited[j]=1;
		//							previous[j]=idxHead;
		//							leftVisited[j]=true;
		//						}
		//					}
		//				}
		//			}
		//
		//			return -1;//not found
		//		}
		//
		//		void Increase_Matchs(int j)
		//		{			
		//			if (previous [j] != -1)
		//				do
		//				{
		//					int i=previous[j];
		//					Out[i]=j-(leftLen + 1);
		//					In[j-(leftLen + 1)]=i;
		//					j=previous[i];
		//				} while ( j != -1);
		//		}
		//

		#endregion

		float GetMinDeviation()
		{
			float min=float.MaxValue;

			for (int i=0; i <= leftLen; i++)
				if (_leftVisited[i])
				{
					for (int j=0; j <= rightLen; j++)
						if (!_rightVisited[j])
						{
							float tmp=(float)Math.Round(leftLabel[i] + rightLabel[j], 2);
							if (tmp - _cost[i, j] < min )
								min=tmp - _cost[i, j];
						}
				}

			return min ;
		}

		private void Relabels()
		{
			float dev=GetMinDeviation();

			for (int k=0; k <= leftLen; k++)
				if (_leftVisited[k])
				{
					leftLabel[k] -= dev;
					leftLabel[k] = (float) Math.Round(leftLabel[k], 2)  ;
				}

			for (int k=0; k <= rightLen; k++)
				if (_rightVisited[k])
				{
					rightLabel[k] += dev;
					rightLabel[k]  = (float) Math.Round(rightLabel[k] , 2)  ;
				}
		}
				
		private void Make_Matching()
		{
			_outgoing=new int[leftLen + 1] ;
			_incomming=new int[rightLen + 1] ;
			for (int i=0; i < _outgoing.Length; i++) _outgoing[i]=-1;
			for (int i=0; i < _incomming.Length; i++) _incomming[i]=-1;

			for (int k=0; k <= leftLen; k++)
				if (_outgoing[k] == -1)
				{
					bool found=false;
					do
					{
						found=FindPath(k);
						if (!found) Relabels();					

					}while (!found);
				}			
		}


		public float[] GetMatches()
		{
			float nTotal=0;
			float nA=0;
			Trace.Flush() ;
			float[] matches=new float[leftLen + 1];
			for (int i=0; i <= leftLen ; i++)
				if (_outgoing[i] != -1)
				{
					nTotal += _cost[i, _outgoing[i]];
					matches[i]=_cost[i, _outgoing[i]];
					if (_cost[i, _outgoing[i]] > 0)
					Trace.WriteLine (_leftTokens[i] + " <-> " + _rightTokens [_outgoing[i]] + " : " + _cost[i, _outgoing[i]]) ;
					float a=1.0F - System.Math.Max(_leftTokens[i].Length , _rightTokens[_outgoing[i]].Length ) != 0 ? _cost[i, _outgoing[i]]/System.Math.Max(_leftTokens[i].Length , _rightTokens[_outgoing[i]].Length ) : 1;
					nA += a;					
				}			
			return  matches;
		}

		public float GetScore()
		{
			float dis=0;
			
			float maxLen=rightLen  + 1;
//			int l1=0; int l2=0;
//			foreach (string s in _rightTokens) l1+=s.Length ;
//			foreach (string s in _leftTokens) l2+=s.Length ;
//			maxLen = Math.Max(l1, l2);

			if (maxLen > 0)			
				return dis/maxLen;
			else 
				return 1.0F;			
		}


		public float Score
		{
			get
			{
				if (_errorOccured) return 0;
				else
					return GetScore ();
			}
		}

	}
}
