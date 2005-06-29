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
using System.Diagnostics;
using System.Reflection;
using Razor.Attributes;

namespace Razor.Wizards
{
	/// <summary>
	/// Summary description for WizardPageAttributeReader.
	/// </summary>
	public class WizardPageAttributeReader : AttributeReader
	{
		protected Type _type;

		/// <summary>
		/// Initializes a new instance of the WizardPageAttributeReader class
		/// </summary>
		/// <param name="type">The WizardPage Type to read metadata from</param>
		public WizardPageAttributeReader(Type type)
		{			
			// make sure the type is not null
			if (type == null)
				throw new ArgumentNullException("Type type", "Metadata cannot be retrieved from a missing Type reference");

			_type = type;
		}

		/// <summary>
		/// Returns the title supplied by the WizardPageTitleAttribute 
		/// </summary>
		/// <returns></returns>
		public string GetTitle()
		{
			try
			{
				object[] attributes = _type.GetCustomAttributes(typeof(WizardPageTitleAttribute), false);
				if (attributes != null)
				{
					WizardPageTitleAttribute a = attributes[0] as WizardPageTitleAttribute;
					if (a != null)
						return a.Title;
				}
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
			return string.Empty;
		}

		/// <summary>
		/// Returns the button style list that applies to the wizard page
		/// </summary>
		/// <returns></returns>
		public WizardButtonStyleList GetButtonStyleList()
		{
			WizardButtonStyleList styleList = new WizardButtonStyleList();
			try
			{
				object[] attributes = _type.GetCustomAttributes(typeof(WizardPageButtonStyleAttribute), false);
				if (attributes != null)
					foreach(WizardPageButtonStyleAttribute a in attributes)
						styleList.Add(a.Style);				
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
			return styleList;
		}

//		/// <summary>
//		/// Returns whether the paths are allowed to be searched for a default path
//		/// </summary>
//		/// <returns></returns>
//		public bool GetManuallySelectsPath()
//		{			
//			try
//			{
//				object[] attributes = _type.GetCustomAttributes(typeof(WizardPageManuallySelectsPathAttribute), false);
//				if (attributes != null)
//					if (attributes.Length > 0)
//						return true;
//			}
//			catch(Exception ex)
//			{
//				Trace.WriteLine(ex);
//			}
//			return false;
//		}
	}
}
