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

namespace Razor.Wizards
{
	/// <summary>
	/// Summary description for WizardNavigationLocation.
	/// </summary>
	public class WizardNavigationLocation
	{
		protected Type _wizardPageType;
		protected WizardNavigationPathList _paths;

		/// <summary>
		/// Initializes a new instance of the WizardNavigationLocation class
		/// </summary>
		/// <param name="wizardPageType">The WizardPage Type that will handle this location in the navigation map</param>
		public WizardNavigationLocation(Type wizardPageType)
		{
			// make sure the type is not null
			if (wizardPageType == null)
				throw new ArgumentNullException("Type wizardPageType", "A location cannot be created for a missing Type reference");

			// throw an exception if the type does not implement IWizardPage
			if (wizardPageType.GetInterface(typeof(IWizardPage).FullName) == null)
				throw new ArgumentException("Type wizardPageType", "The type must implement the IWizardPage interface");

			_wizardPageType = wizardPageType;
			_paths = new WizardNavigationPathList();
		}

		/// <summary>
		/// Adds a route from this location to the location defined by the specified path
		/// </summary>
		/// <param name="path">The path to follow to reach the specified destination location</param>
		public void AddRoute(WizardNavigationPath path)
		{
			// add this path which describes some route
			_paths.Add(path);			
		}

		/// <summary>
		/// Adds a route from this location to the location defined by the specified path elements
		/// </summary>
		/// <param name="destination">The destination location</param>
		/// <param name="name">A key by which the route will be known</param>
		/// <param name="selected">A flag that indicates whether this route is the selected path</param>
		public void AddRoute(WizardNavigationLocation destination, string name, bool selected)
		{
			// add a new path to describe this route
			_paths.Add(new WizardNavigationPath(destination, name, selected));
		}

		/// <summary>
		/// Returns the Type for the WizardPage that will handle this location in the WizardNavigationMap
		/// </summary>
		public Type WizardPageType
		{
			get
			{
				return _wizardPageType;
			}
		}

		/// <summary>
		/// Returns a list of paths that are valid choices when selecting the next location as a destination from this location 
		/// </summary>
		public WizardNavigationPathList Paths
		{
			get
			{
				return _paths;
			}
		}
	}
}
