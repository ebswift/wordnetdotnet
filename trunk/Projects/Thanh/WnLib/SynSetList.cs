using System;
using System.Collections;
using Wnlib;

namespace Wnlib
{
	/// <summary>
	/// 
	/// </summary>	
	public class SynSetList : CollectionBase
	{
		public SynSetList()
		{

		}

		~SynSetList()
		{

		}

		public int Add(SynSet item)
		{
			
			return List.Add(item);
		}
		public void Insert(int index, SynSet item)
		{
			List.Insert(index, item);
		}
		public void Remove(SynSet item)
		{
			List.Remove(item);
		} 
		public bool Contains(SynSet item)
		{
			return List.Contains(item);
		}
		public int IndexOf(SynSet item)
		{
			return List.IndexOf(item);
		}
		public void CopyTo(SynSet[] array, int index)
		{
			List.CopyTo(array, index);
			
		}
		
		public SynSet this[int index]
		{
			get { return (SynSet)List[index]; }
			set { List[index] = value; }
		}

		public virtual void Dispose()
		{
			
		}
	}
}
