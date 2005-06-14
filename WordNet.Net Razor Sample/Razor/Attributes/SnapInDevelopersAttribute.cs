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
	/// An attribute that contains the names of the developers responsible for a SnapIn
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public class SnapInDevelopersAttribute : Attribute
	{
		private string[] _names;

		/// <summary>
		/// Initializes a new instance of the SnapInDevelopersAttribute class
		/// </summary>
		/// <param name="names"></param>
		public SnapInDevelopersAttribute(params string[] names)
		{
			_names = names;
		}

		/// <summary>
		/// Initializes a new instance of the SnapInDevelopersAttribute class
		/// </summary>
		/// <param name="name"></param>
		public SnapInDevelopersAttribute(string name)
		{
			_names = new string[] {name};
		}

		/// <summary>
		/// Returns an array of strings that contains the names of the SnapIn's developers
		/// </summary>
		public string[] DevelopersNames
		{
			get
			{
				return _names;
			}
		}		
	}
}
