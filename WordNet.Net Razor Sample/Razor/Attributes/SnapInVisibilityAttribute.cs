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

namespace Razor.Attributes
{
	/// <summary>
	/// An attribute that contains a flag that determines whether a SnapIn is visible to the HostingEngine
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public class SnapInVisibilityAttribute : Attribute
	{
		private bool _visible;

		/// <summary>
		/// Initializes a new instance of the SnapInVisibilityAttribute class
		/// </summary>
		/// <param name="visible"></param>
		public SnapInVisibilityAttribute(bool visible)
		{
			_visible = visible;
		}

		/// <summary>
		/// Returns a flag that indicates whether the SnapIn is visible to the SnapInHostingEngine
		/// </summary>
		public bool Visible
		{
			get
			{
				return _visible;
			}
		}

		public override string ToString()
		{
			return "'SnapIn HostingEngine Visible': " + _visible.ToString();
		}
	}
}
