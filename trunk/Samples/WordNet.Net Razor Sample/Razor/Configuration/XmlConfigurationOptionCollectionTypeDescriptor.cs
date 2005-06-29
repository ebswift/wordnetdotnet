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

namespace Razor.Configuration
{
	/// <summary>
	/// Summary description for XmlConfigurationOptionCollectionTypeDescriptor.
	/// </summary>
	public class XmlConfigurationOptionCollectionTypeDescriptor : ICustomTypeDescriptor
	{
		private Hashtable _table;
		private PropertyDescriptorCollection _descriptors;
		
		public XmlConfigurationOptionCollectionTypeDescriptor()
		{
			_table = new Hashtable();			
			_descriptors = new PropertyDescriptorCollection(null);
		}

		public XmlConfigurationOptionCollectionTypeDescriptor(Hashtable table) : this()
		{
			foreach(DictionaryEntry entry in table)
			{
				try
				{
					XmlConfigurationCategory category = entry.Value as XmlConfigurationCategory;
					if (category != null)
                        this.LoadOptions(category.Options);
				}
				catch(System.Exception systemException)
				{
					System.Diagnostics.Trace.WriteLine(systemException);
				}
			}
		}

		public XmlConfigurationOptionCollectionTypeDescriptor(XmlConfigurationCategoryCollection categories) : this()
		{
			foreach(XmlConfigurationCategory category in categories)
			{
				try
				{
					this.LoadOptions(category.Options);
				}
				catch(System.Exception systemException)
				{
					System.Diagnostics.Trace.WriteLine(systemException);
				}
			}
		}

		public XmlConfigurationOptionCollectionTypeDescriptor(XmlConfigurationCategory category) : this()
		{
			this.LoadOptions(category.Options);
		}

		public XmlConfigurationOptionCollectionTypeDescriptor(XmlConfigurationOptionCollection options) : this()
		{
			this.LoadOptions(options);
		}

		private void LoadOptions(XmlConfigurationOptionCollection options)
		{
			if (options == null)
				throw new ArgumentNullException("options");
			
			foreach(XmlConfigurationOption option in options)
			{
				try
				{
					if (!option.Hidden)
					{
						XmlConfigurationOptionPropertyDescriptor pd = new XmlConfigurationOptionPropertyDescriptor(option);                        
						_descriptors.Add(pd);
						_table.Add(pd, option);
					}
				}
				catch(System.Exception systemException)
				{
					System.Diagnostics.Trace.WriteLine(systemException);
				}
			}
		}

		#region ICustomTypeDescriptor Members

		public TypeConverter GetConverter()
		{
			// TODO:  Add XmlConfigurationOptionCollectionTypeDescriptor.GetConverter implementation
			return null;
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return new EventDescriptorCollection(null);
		}

		EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
		{
			return new EventDescriptorCollection(null);
		}

		public string GetComponentName()
		{
			// TODO:  Add XmlConfigurationOptionCollectionTypeDescriptor.GetComponentName implementation
			return null;
		}

		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			if (_table != null)
			{
				if (pd != null)
				{
					if (_table.ContainsKey(pd))
						return _table[pd];
				}
			}
			return null;
		}

		public AttributeCollection GetAttributes()
		{
			return new AttributeCollection(null);
		}

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			return _descriptors;
		}

		PropertyDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetProperties()
		{
			return _descriptors;
		}

		public object GetEditor(Type editorBaseType)
		{
			// TODO:  Add XmlConfigurationOptionCollectionTypeDescriptor.GetEditor implementation
			return null;
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			// TODO:  Add XmlConfigurationOptionCollectionTypeDescriptor.GetDefaultProperty implementation
			return null;
		}

		public EventDescriptor GetDefaultEvent()
		{
			// TODO:  Add XmlConfigurationOptionCollectionTypeDescriptor.GetDefaultEvent implementation
			return null;
		}

		public string GetClassName()
		{
			// TODO:  Add XmlConfigurationOptionCollectionTypeDescriptor.GetClassName implementation
			return null;
		}

		#endregion
	}
}
