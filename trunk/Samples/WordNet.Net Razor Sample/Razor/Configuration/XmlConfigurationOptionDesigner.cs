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
	/// Summary description for XmlConfigurationOptionDesigner.
	/// </summary>
	public class XmlConfigurationOptionDesigner : ComponentDesigner
	{			
//		public override ICollection AssociatedComponents
//		{
//			get
//			{
//				return new XmlConfigurationOption[] {(XmlConfigurationOption)base.Component};
//			}
//		}

		public override void Initialize(IComponent component)
		{
			base.Initialize (component);

			XmlConfigurationOption option = base.Component as XmlConfigurationOption;
			if (option != null)
			{
				this.HasChanges = option.HasChanges;
			}
		}	

		protected override void PreFilterProperties(IDictionary properties)
		{
			base.PreFilterProperties (properties);

			properties["HasChanges"] = TypeDescriptor.CreateProperty(
				typeof(XmlConfigurationOptionDesigner), 
				(PropertyDescriptor)properties["HasChanges"],
				new Attribute[0]);

			properties.Remove("Value");

//			XmlConfigurationOption option = base.Component as XmlConfigurationOption;
//			if (option != null)
//			{			
//				ValuePropertyDescriptor p = properties["Value"];
//					(ValuePropertyDescriptor)TypeDescriptor.CreateProperty(
//                    typeof(XmlConfigurationOption), 
//					(PropertyDescriptor)properties["HasChanges"], 
//					CategoryAttribute.Design,
//					DesignOnlyAttribute.Yes);		
//		
//				p.Option = option;
//
//				properties["Value"] = p;
//			}
		}	
				
		public bool HasChanges
		{
			get
			{
				return (bool)this.ShadowProperties["HasChanges"];
			}
			set
			{
				this.ShadowProperties["HasChanges"] = value;
			}
		}
	}		
}
