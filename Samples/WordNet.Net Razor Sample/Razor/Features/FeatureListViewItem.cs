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
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using System.Text;
using Razor.Configuration;

namespace Razor.Features
{
	/// <summary>
	/// Summary description for FeatureListViewItem.
	/// </summary>
	public class FeatureListViewItem : System.Windows.Forms.ListViewItem 
	{
		private Feature _feature; 

		public FeatureListViewItem(Feature feature) : base()
		{
			_feature = feature;
			// just check the features that are resetting, these are most likely the common features such as configuration files
//			if (_feature.Action == FeatureActions.ResetToDefault || _feature.Action == FeatureActions.ResetToBlank)
//                base.Checked = true;
			base.Text = _feature.Name; 
			base.SubItems.Add(EnumHelper.GetEnumValueDescription(_feature.Action, typeof(FeatureActions)));
			base.SubItems.Add(_feature.Description);			
//			this.ParseMetadataForActionDescription();
		}

		public Feature Feature
		{
			get
			{											
				return _feature;
			}
		}

		

	}
}
