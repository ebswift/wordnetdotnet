using System;
using System.IO;
using System.Collections;

namespace Wnlib
{
	/// <summary>
	/// Summary description for SemCor.
	/// </summary>
	public class SemCorList : CollectionBase
	{
		public SemCorList()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public int Add(SemCor item)
		{
			
			return List.Add(item);
		}
		public void Insert(int index, SemCor item)
		{
			List.Insert(index, item);
		}
		public void Remove(SemCor item)
		{
			List.Remove(item);
		} 
		public bool Contains(SemCor item)
		{
			return List.Contains(item);
		}
		public int IndexOf(SemCor item)
		{
			return List.IndexOf(item);
		}
		public void CopyTo(SemCor[] array, int index)
		{
			List.CopyTo(array, index);
			
		}
		
		public SemCor this[int index]
		{
			get { return (SemCor)List[index]; }
			set { List[index] = value; }
		}

		public virtual void Dispose()
		{
			
		}
	}
}
