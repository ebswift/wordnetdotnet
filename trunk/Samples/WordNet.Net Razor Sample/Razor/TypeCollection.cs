/*
 * This file is a part of the Razor Framework.
 * 
 * Copyright (C) 2003 Mark (Code6) Belles 
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

namespace Razor
{	
	/// <summary>
	/// Provides an event args class based upon the Type class.
	/// </summary>
	public class TypeEventArgs : ObjectEventArgs
	{
		/// <summary>
		/// Initializes a new instance of the TypeEventArgs class
		/// </summary>
		/// <param name="context">The context of this event</param>
		/// <param name="action">The action that describes this event context</param>
		public TypeEventArgs(Type context, ObjectActions action) : base((object)context, action)
		{

		}

		/// <summary>
		/// Gets the Type that is the context of the event
		/// </summary>
		public new Type Context
		{
			get
			{
				return (Type)base.Context;
			}
			set
			{
				base.Context = (object)value;
			}
		}
	}

	/// <summary>
	/// Provides a strongly typed collection of Type objects.
	/// </summary>
	public class TypeCollection : CollectionBase
	{
		public event ObjectEventHandler Changed;

		/// <summary>
		/// Initializes a new instance of the Type Collection
		/// </summary>
		public TypeCollection()
		{
			
		}

		/// <summary>
		/// Adds a Type to the collection
		/// </summary>
		/// <param name="type">The Type to add</param>
		public void Add(Type type)
		{
			if (!base.InnerList.Contains(type))
			{				
				base.InnerList.Add(type);
				this.OnChanged(this, new TypeEventArgs(type, ObjectActions.Added));
			}
		}

		/// <summary>
		/// Adds an array of Types to the collection
		/// </summary>
		/// <param name="types"></param>
		public void AddRange(Type[] types)
		{
			foreach(Type t in types)
				this.Add(t);			
		}

		public void Insert(int index, Type type)
		{
			if (!base.InnerList.Contains(type))
			{
				base.InnerList.Insert(index, type);
				this.OnChanged(this, new TypeEventArgs(type, ObjectActions.Added));
			}
		}

		/// <summary>
		/// Removes a Type from the collection
		/// </summary>
		/// <param name="type"></param>
		public void Remove(Type type)
		{
			if (base.InnerList.Contains(type))
			{
				base.InnerList.Remove(type);
				this.OnChanged(this, new TypeEventArgs(type, ObjectActions.Removed));
			}
		}

		/// <summary>
		/// Determines if the collection contains the specified Type
		/// </summary>
		/// <param name="type">The Type to search the collection for</param>
		/// <returns></returns>
		public bool Contains(Type type)
		{
			return base.InnerList.Contains(type);
		}

		/// <summary>
		/// Removes all of the Types from the collection. Clears the entire list.
		/// </summary>
		public new void Clear()
		{
			base.InnerList.Clear();
		}

		public Type this[int index]
		{
			get
			{
				return base.InnerList[index] as Type;
			}
		}

		/// <summary>
		/// Raises the Changed event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnChanged(object sender, ObjectEventArgs e)
		{
			try
			{
				if (this.Changed != null)
					this.Changed(sender, e);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}


	}
}
