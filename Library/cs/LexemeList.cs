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
	public class LexemeList : CollectionBase
	{
		public LexemeList()
		{

		}

		~LexemeList()
		{

		}

		public int Add(Lexeme item)
		{
			
			return List.Add(item);
		}
		public void Insert(int index, Lexeme item)
		{
			List.Insert(index, item);
		}
		public void Remove(Lexeme item)
		{
			List.Remove(item);
		} 
		public bool Contains(Lexeme item)
		{
			return List.Contains(item);
		}
		public int IndexOf(Lexeme item)
		{
			return List.IndexOf(item);
		}
		public void CopyTo(Lexeme[] array, int index)
		{
			List.CopyTo(array, index);
			
		}
		
		public Lexeme this[int index]
		{
			get { return (Lexeme)List[index]; }
			set { List[index] = value; }
		}

		public virtual void Dispose()
		{
			
		}
	}
}
