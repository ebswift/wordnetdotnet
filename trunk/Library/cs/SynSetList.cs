/*
 * This file is a part of the WordNet.Net open source project.
 * 
 * Copyright (C) 2005 Malcolm Crowe, Troy Simpson, Thanh Dao
 * 
 * Project Home: http://www.ebswift.com
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 * 
 * */

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
		private ArrayList _synSets = new ArrayList();
		public SynSetList()
		{

		}

		~SynSetList() 
		{

		}

		public virtual void Dispose()
		{
			_synSets = null;
		}

		/// 
		/// <param name="item"></param>
		public int Add(SynSet item)
		{
			return _synSets.Add(item);
		}

		public int Count
		{
			get
			{
				return _synSets.Count;	
			}
			
		}

		/// 
		/// <param name="item"></param>
		public void Remove(SynSet item)
		{
			_synSets.Remove(item);
		}

		/// 
		/// <param name="index"></param>
		/// <param name="item"></param>
		public void Insert(int index, SynSet item)
		{
			_synSets.Insert(index, item);
		}

		/// 
		/// <param name="item"></param>
		public bool Contains(SynSet item)
		{
			return _synSets.Contains(item);
		}

		/// 
		/// <param name="item"></param>
		public int IndexOf(SynSet item)
		{
			return _synSets.IndexOf(item);
		}

		/// 
		/// <param name="array"></param>
		/// <param name="index"></param>
		public void CopyTo(SynSet[] array, int index)
		{
			_synSets.CopyTo(array,index);
		}

		/// 
		/// <param name="index"></param>
		public SynSet this[int index]
		{

			get
			{
				if(index>=_synSets.Count)
					throw new Exception("This index is out of the range");
				else
					return (SynSet)_synSets[index];
			}

			set
			{
				if(index>=_synSets.Count)
					throw new Exception("This index is out of the range");
				else
					_synSets[index] = value;
			}
		}
	}
}
