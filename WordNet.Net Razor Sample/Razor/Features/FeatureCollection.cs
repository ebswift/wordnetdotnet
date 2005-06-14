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

namespace Razor.Features
{
	/// <summary>
	/// A collection of Features
	/// </summary>
	public class FeatureCollection : CollectionBase
	{
		public FeatureCollection()
		{
			
		}

		public FeatureCollection(params Feature[] features)
		{
			this.AddRange(features);
		}

		public FeatureCollection(FeatureCollection features)
		{
			this.AddRange(features);
		}

		public int Add(Feature f)
		{
			return base.InnerList.Add(f);			
		}

		public void AddRange(ICollection features)
		{
			base.InnerList.AddRange(features);			
		}

		public void Remove(Feature f)
		{
			base.InnerList.Remove(f);
		}

		public void RemoveRange(int index, int count)
		{
			base.InnerList.RemoveRange(index, count);
		}

		public bool Contains(Feature f)
		{
			foreach(Feature feature in base.InnerList)
				if (object.Equals(f, feature))
					return true;
			return false;
		}
	}
}
