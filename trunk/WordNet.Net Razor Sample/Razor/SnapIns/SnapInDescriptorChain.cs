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

namespace Razor.SnapIns
{
	/// <summary>
	/// Summary description for SnapInDescriptorLink.
	/// </summary>
	public class SnapInDescriptorLink
	{
		private SnapInDescriptor _descriptor;
		private ArrayList _links;

		/// <summary>
		/// Initializes a new instance of the SnapInDescriptorLink class
		/// </summary>
		public SnapInDescriptorLink(SnapInDescriptor descriptor)
		{
			_descriptor = descriptor;			
			_links = new ArrayList();
		}

		/// <summary>
		/// Gets the descriptor that is the source of this chain
		/// </summary>
		public SnapInDescriptor Descriptor
		{
			get
			{
				return _descriptor;
			}
		}

		/// <summary>
		/// Gets an ArrayList of descriptors that are directly depending on the descriptor 
		/// </summary>
		public ArrayList Links
		{
			get
			{
				return _links; // .ToArray(typeof(SnapInDescriptor)) as SnapInDescriptor[];
			}
		}
	}
}
