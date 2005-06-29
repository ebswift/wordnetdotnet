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

namespace Razor.Wizards
{
	/// <summary>
	/// Summary description for WizardPageDescriptor.
	/// </summary>
	public class WizardPageDescriptor
	{
		protected Type _type;		
		protected string _title;
		protected WizardButtonStyleList _buttonStyles;		
//		protected bool _manuallySelectsPath;
		protected IWizardPage _instance;

		/// <summary>
		/// Initializes a new instance of the WizardPageDescriptor class
		/// </summary>
		/// <param name="type">The type of the WizardPage that will be desribed (The Type must implement IWizardPage)</param>
		public WizardPageDescriptor(Type type)
		{
			// create an attribute reader to read the metadata supplied by the type
			WizardPageAttributeReader reader = new WizardPageAttributeReader(type);
			
			// throw an exception if the type does not implement IWizardPage
			if (type.GetInterface(typeof(IWizardPage).FullName) == null)
				throw new ArgumentException("Type type", "The type must implement the IWizardPage interface");
			
			// read any metadata supplied to us by the type about the wizard page contained therein
			_type = type;
			_title = reader.GetTitle();
			_buttonStyles = reader.GetButtonStyleList();
//			_manuallySelectsPath = reader.GetManuallySelectsPath();
		}
		
		/// <summary>
		/// Creates an instance of the descriptor's Type, saves a reference to the instance, and reads the Wizard Pages metadata (Not for external use)
		/// </summary>
		/// <param name="wizardPageHost">The Wizard that is hosting the page</param>
		/// <returns></returns>
		internal static void Create(IWizard wizard, WizardPageDescriptor descriptor)
		{
			// find the default constructor
			ConstructorInfo ci = descriptor.Type.GetConstructor(Type.EmptyTypes);

			// create an instance of the type
			object instance = ci.Invoke(null);

			// cast to a wizard page for ease of usage
			IWizardPage wizardPage = (IWizardPage)instance;

			// set the wizard that will be owning the wizard page
			wizardPage.Wizard = wizard;

			// set the instance created on the descriptor so that it may be referenced later
			descriptor.SetInstance(wizardPage);															
		}

		/// <summary>
		/// Sets the object instance to a created Wizard Page (Not for external use)
		/// </summary>
		/// <param name="wizardPage"></param>
		internal void SetInstance(IWizardPage wizardPage)
		{
			_instance = wizardPage;
		}

		/// <summary>
		/// Determines if an instance of the descriptor's Type has been created and is ready for use
		/// </summary>
		public bool IsNull
		{
			get
			{
				return _instance != null;
			}
		}	

		/// <summary>
		/// Returns the Wizard Page Type that this descriptor contains
		/// </summary>
		public Type Type
		{
			get
			{
				return _type;
			}
		}		

		/// <summary>
		/// Returns the title that should be displayed when the Wizard Page is activated
		/// </summary>
		public string Title
		{
			get
			{
				return _title;
			}
		}

		/// <summary>
		/// Returns the button styles that should be supported (available) when the Wizard Page is activated
		/// </summary>
		public WizardButtonStyleList ButtonStyles
		{
			get
			{
				return _buttonStyles;
			}
		}	

//		/// <summary>
//		/// Returns a flag indicating whether the page will manually select it's own path
//		/// </summary>
//		public bool ManuallySelectsPath
//		{
//			get
//			{
//				return _manuallySelectsPath;
//			}
//		}

		/// <summary>
		/// Returns the IWizardPage instance described by the descriptor
		/// </summary>
		public IWizardPage WizardPage
		{
			get
			{
				return _instance;
			}
		}

	}
}
