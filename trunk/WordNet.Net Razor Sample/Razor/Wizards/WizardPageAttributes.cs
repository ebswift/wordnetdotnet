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

namespace Razor.Wizards
{
	/// <summary>
	/// Summary description for WizardPageTitleAttribute
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public class WizardPageTitleAttribute : Attribute 
	{
		protected string _title;

		/// <summary>
		/// Initializes a new instance of the WizardPageTitleAttribute Class
		/// </summary>
		/// <param name="title">A title displayed for the Wizard Page</param>
		public WizardPageTitleAttribute(string title) : base()
		{
			_title = title;
		}
		
		public string Title
		{
			get
			{
				return _title;
			}
		}
	}

//	/// <summary>
//	/// 
//	/// </summary>
//	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
//	public class WizardPageManuallySelectsPathAttribute : Attribute
//	{		
//		public WizardPageManuallySelectsPathAttribute() : base()
//		{
//	
//		}	
//	}

	/// <summary>
	/// Summary description for WizardPageButtonStyleAttribute
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
	public class WizardPageButtonStyleAttribute : Attribute 
	{
		protected WizardButtonStyle _style;

		/// <summary>
		/// Initializes a new instance of the WizardPageButtonStyleAttribute class
		/// </summary>
		/// <param name="supportedStyles">A combination of styles that the Wizard Page supports</param>
		public WizardPageButtonStyleAttribute(WizardButtons button, bool visible, bool enabled) : base()
		{
			_style = new WizardButtonStyle(button, visible, enabled);
		}
		
		/// <summary>
		/// Returns the button style to modify
		/// </summary>
		public WizardButtonStyle Style
		{
			get
			{
				return _style;
			}
		}
	}
}
