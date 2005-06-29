/*
 * This file is a part of the Razor Framework.
 * 
 * Copyright (C) 2004 Mark (Code6) Belles 
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
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace Razor.MultiThreading
{
	#region BackgroundThreadList

	public class BackgroundThreadList : CollectionBase
	{
		/// <summary>
		/// This operator allows a list to be explicitly cast to an array of it's contents
		/// </summary>
		/// <example>
		/// BackgroundThreadList list = new BackgroundThreadList();
		/// BackgroundThread[] threads = (BackgroundThread[])list;
		/// </example>
		/// <param name="list"></param>
		/// <returns></returns>
		public static explicit operator BackgroundThread[] (BackgroundThreadList list)
		{
			if (list == null)
				throw new ArgumentNullException("list");

			return list.InnerList.ToArray(typeof(BackgroundThread)) as BackgroundThread[];
		}

		/// <summary>
		/// Initializes a new instance of the BackgroundThreadList class
		/// </summary>
		public BackgroundThreadList()
		{

		}

		/// <summary>
		/// Adds a thread to the list
		/// </summary>
		/// <param name="thread"></param>
		public virtual void Add(BackgroundThread thread)
		{
			if (this.Contains(thread))
				throw new BackgroundThreadAlreadyExistsException(thread);

			base.InnerList.Add(thread);
		}

		/// <summary>
		/// Removes a thread from the list
		/// </summary>
		/// <param name="thread"></param>
		public virtual void Remove(BackgroundThread thread)
		{
			if (this.Contains(thread))
				base.InnerList.Remove(thread);
		}

		/// <summary>
		/// Determines if the list contains the thread
		/// </summary>
		/// <param name="thread"></param>
		/// <returns></returns>
		public virtual bool Contains(BackgroundThread thread)
		{
			if (thread == null)
				throw new ArgumentNullException("thread");

			foreach(BackgroundThread t in base.InnerList)
				if (object.Equals(t, thread))
					return true;
			return false;
		}

		/// <summary>
		/// Returns the BackgroundThread at the specified index
		/// </summary>
		public virtual BackgroundThread this[int index]
		{
			get
			{
				return base.InnerList[index] as BackgroundThread;
			}
		}
	}


	#endregion

}
