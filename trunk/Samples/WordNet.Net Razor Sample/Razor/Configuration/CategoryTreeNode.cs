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
using System.Windows.Forms;

namespace Razor.Configuration
{
	/// <summary>
	/// Summary description for CategoryTreeNode.
	/// </summary>
	public class CategoryTreeNode : TreeNode 
	{
		private Hashtable _table;

		/// <summary>
		/// Initializes a new instance of the CategoryTreeNode class
		/// </summary>
		/// <param name="text"></param>
		public CategoryTreeNode(string text) : base(text)
		{
			_table = new Hashtable();			
		}

		/// <summary>
		/// Binds a category to the node, returns false if the category is already bound
		/// </summary>
		/// <param name="category">The category to bind to the node</param>
		/// <returns></returns>
		public bool BindToCategory(XmlConfigurationCategory category)
		{
			if (!this.IsBoundToCategory(category))
			{
				_table.Add(category.Fullpath, category);
				return true;
			}
			return false;
		}
		
		/// <summary>
		/// Determines if the category is bound to the node
		/// </summary>
		/// <param name="category">The category to check bindings for</param>
		/// <returns></returns>
		public bool IsBoundToCategory(XmlConfigurationCategory category)
		{
			if (category != null)
				return _table.ContainsKey(category.Fullpath);			
			return false;
		}

		/// <summary>
		/// Gets the categories bound to this node
		/// </summary>
		public Hashtable Categories
		{
			get
			{
				return _table;
			}
		}
	}
}
