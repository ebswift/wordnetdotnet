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
using Razor.Configuration;

namespace Razor.Features
{
	/// <summary>
	/// Summary description for ConfigurationFeature.
	/// </summary>
	public class ConfigurationFeature : Feature
	{
		public ConfigurationFeature() : base()
		{
			
		}

		public ConfigurationFeature(string name, string description) : base(name, description)
		{

		}

		public ConfigurationFeature(string configurationName, string description, FeatureActions action) : base(configurationName, description, configurationName, action)
		{

		}

		public ConfigurationFeature(string name, string description, string configurationName, FeatureActions action) : base(name, description, configurationName, action)
		{
			
		}

		public string ConfigurationName
		{
			get
			{
				return (string)base.Tag;
			}
		}
	}
}
