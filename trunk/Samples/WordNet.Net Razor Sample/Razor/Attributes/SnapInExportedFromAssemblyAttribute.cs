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
	/// An attribute which specifies that a SnapIn is exported from a .Net assembly
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true)]
	public class SnapInExportedFromAssemblyAttribute : Attribute 
	{
		private Type _type;

		/// <summary>
		/// Initializes a new instance of the SnapInAttribute class
		/// </summary>
		/// <param name="t">The type of SnapIn that is exported from this Assembly</param>
		public SnapInExportedFromAssemblyAttribute(Type t)
		{
			_type = t;
		}

		/// <summary>
		/// Gets the type of SnapIn that is exported from this Assembly
		/// </summary>
		/// <returns></returns>
		public Type Type
		{
			get
			{
				return _type;
			}
		}

		public override string ToString()
		{
			return "'SnapIn Exported from Assembly': " + _type.FullName;			
		}
	}
}
