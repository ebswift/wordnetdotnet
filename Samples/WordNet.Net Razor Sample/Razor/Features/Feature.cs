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

namespace Razor.Features
{
	/// <summary>
	/// Summary description for Feature.
	/// </summary>
	public class Feature
	{
		private string _name;
		private string _description;
		private object _tag;
		private FeatureActions _action;

		public Feature()
		{
			
		}

		public Feature(string name, string description) : this()
		{
			_name = name;
			_description = description;
		}

		public Feature(string name, string description, object tag, FeatureActions action) : this(name, description)
		{
			_tag = tag;
			_action = action;
		}

		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		public string Description
		{
			get
			{
				return _description;
			}
			set
			{
				_description = value;
			}
		}

		public object Tag
		{
			get
			{
				return _tag;
			}
			set
			{
				_tag = value;
			}
		}
		
		public FeatureActions Action
		{
			get
			{
				return _action;
			}
			set
			{
				_action = value;
			}
		}		
	}
}
