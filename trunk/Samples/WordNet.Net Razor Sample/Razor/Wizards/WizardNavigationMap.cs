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
	/// Summary description for WizardNavigationMap.
	/// </summary>
	public class WizardNavigationMap
	{
		protected WizardNavigationLocation _start;

		/// <summary>
		/// Initializes a new instance of the WizardNavigationmap class
		/// </summary>
		public WizardNavigationMap()
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the WizardNavigationmap class
		/// </summary>
		/// <param name="startingLocation">The starting location for the navigation map</param>
		public WizardNavigationMap(WizardNavigationLocation start)
		{
			_start = start;
		}
		
		/// <summary>
		/// Gets or sets the starting location for the navigation map
		/// </summary>
		public WizardNavigationLocation Start
		{
			get
			{
				return _start;
			}
			set
			{
				_start = value;
			}
		}
	}

	/*
	 * A WizardNavigationMap is defined as having a starting location
	 * 
	 * Each location contains a Type that will be created on the fly using reflection and loaded as a WizardPage 
	 * and displayed in the Wizard.
	 * 
	 * Each location contains a list of paths that can be chosen by the WizardPage. 
	 * 
	 * Each path contains a location that will be navigated to when navigation occurs and requires forward movement in the wizard.
	 * 
	 * The wizard will maintain a single stack for backwards navigation. The stack will contain the location required to load/display 
	 * the appropriate WizardPage as pointed to by the location in the map.
	 * 
	 * Each WizardPage will be given the current Location in the map when it is activated, and must mark a path as the chosen path
	 * for the Wizard to follow when the next forward navigation event occurs.
	 * 
	 * Each Location will only contain a single chosen Path (the wizard will assert this to be true as individual paths are marked as chosen).
	 */

	/*
	 
	This is a simple visual representation of the elements contained in a WizardNavigationMap.
	
	The elements are as follows...
	
	Map (WizardNavigationMap)
		.Location (WizardNavigationLocation)
			.Paths(...) (List of locations, Or'd on Chosen, ie. One True the rest False)
				Path1 (WizardNavigationPath)
					.Chosen (T/F)
					.Location
						.Paths(...)
				Path2
					.Chosen (T/F)
					.Location
						.Paths(...)						
				Path3
					.Chosen (T/F)				
					.Location
						.Paths(...)					
	*/
}
