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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing;
using System.Collections;

namespace Razor.Configuration
{
	/// <summary>
	/// Summary description for XmlConfigurationOptionPropertyTab.
	/// </summary>
	public class XmlConfigurationOptionPropertyTab : PropertyTab
	{
		public XmlConfigurationOptionPropertyTab()
		{
			
		}

		public override string TabName
		{
			get
			{
				return "Value Tab";
			}
		}

		public override System.Drawing.Bitmap Bitmap
		{
			get
			{
				return new Bitmap(base.Bitmap, new Size(16, 16));
			}
		}

		public override bool CanExtend(object extendee)
		{
			return extendee is XmlConfigurationOption;
		}

		public override PropertyDescriptorCollection GetProperties(object component)
		{
			return base.GetProperties (component);
		}

		public override PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes)
		{
			return this.GetProperties(null, component, attributes);
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object component, Attribute[] attributes)
		{					
			XmlConfigurationOption option = component as XmlConfigurationOption;
			if (option == null)
			{
				TypeConverter tc = TypeDescriptor.GetConverter(option);
				if (tc != null)
				{
					return tc.GetProperties(context, option, attributes);
				}
				else
				{
					return TypeDescriptor.GetProperties(option, attributes);
				}
			}

			ArrayList propList = new ArrayList();
			propList.Add(new ValuePropertyDescriptor(option));				
			PropertyDescriptor[] props = (PropertyDescriptor[])propList.ToArray(typeof(PropertyDescriptor));			
			PropertyDescriptorCollection c = new PropertyDescriptorCollection(props);
			return c;			
		}


	}
}
