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
	/// An attribute that contains the name of the company responsible for a SnapIn
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public class SnapInCompanyAttribute : Attribute
	{
		private string _name;

		/// <summary>
		/// Initializes a new instance of the SnapInCompanyAttribute class
		/// </summary>
		/// <param name="companyName"></param>
		public SnapInCompanyAttribute(string companyName)
		{
			_name = companyName;
		}

		/// <summary>
		/// Returns the name of the SnapIn's company
		/// </summary>
		public string CompanyName
		{
			get
			{
				return _name;
			}
		}
	}
}
