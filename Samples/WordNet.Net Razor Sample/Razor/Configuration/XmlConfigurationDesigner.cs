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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

namespace Razor.Configuration
{
	/// <summary>
	/// Summary description for XmlConfigurationDesigner.
	/// </summary>
	public class XmlConfigurationDesigner : ComponentDesigner
	{
		private DesignerVerbCollection _verbs;
//		private XmlConfiguration _configuration;

		public override void Initialize(IComponent component)
		{
			base.Initialize (component);
		}


		public override DesignerVerbCollection Verbs
		{
			get
			{				
				_verbs = new DesignerVerbCollection();
				_verbs.AddRange(base.Verbs);
				_verbs.Add(new System.ComponentModel.Design.DesignerVerb("Load", null));
				_verbs.Add(new System.ComponentModel.Design.DesignerVerb("Save", null));
				return _verbs;
			}
		}

		public override ICollection AssociatedComponents
		{
			get
			{
				/// gather all sited sub components so that the designers can move/cut/delete/copy them all together
				System.Collections.ArrayList sitedSubComponents = new System.Collections.ArrayList();				
//				XmlConfigurationOptionCollection options = ((XmlConfigurationCategory)base.Component).Options;
				XmlConfigurationCategoryCollection categories = ((XmlConfigurationCategory)base.Component).Categories;

//				/// only gather sited options
//				foreach(XmlConfigurationOption option in options)
//					if (option.Site != null)
//						sitedSubComponents.Add(option);

				/// only gather sited categories
				foreach(XmlConfigurationCategory category in categories)
					if (category.Site != null)
						sitedSubComponents.Add(category);
											
				return sitedSubComponents;	
			}
		}

	}
}
