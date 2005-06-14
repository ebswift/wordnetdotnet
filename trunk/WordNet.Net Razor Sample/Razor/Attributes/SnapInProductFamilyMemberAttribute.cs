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
	/// An attribute that specifies a SnapIn is a member of a product family 
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
	public class SnapInProductFamilyMemberAttribute : Attribute
	{
		private string _productFamily;

		/// <summary>
		/// Initializes a new instance of the SnapInProductFamilyMemberAttribute class
		/// </summary>
		/// <param name="productFamily"></param>
		public SnapInProductFamilyMemberAttribute(string productFamily)
		{
			_productFamily = productFamily;
		}

		/// <summary>
		/// Returns the name of the product family of the SnapIn
		/// </summary>
		public string ProductFamily
		{
			get
			{
				return _productFamily;
			}
		}		
	}
}
