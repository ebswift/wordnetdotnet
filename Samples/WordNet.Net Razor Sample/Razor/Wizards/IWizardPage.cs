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
	/// Provides the mimimum required methods and properties needed to implement a WizardPage
	/// </summary>
	public interface IWizardPage
	{
		/// <summary>
		/// Gets or sets the Wizard hosting the page
		/// </summary>
		IWizard Wizard { get; set; }

		/// <summary>
		/// Gets or sets a flag that indicates whether the page is active or not
		/// </summary>
		bool Active { get; set; }

		/// <summary>
		/// Gets or sets the location in the navigation map, when the page was requested
		/// </summary>
		WizardNavigationLocation CurrentLocation { get; }

		/// <summary>
		/// Called when the page becomes the active page in the Wizard
		/// </summary>
		/// <param name="previousPage"></param>
		/// <param name="currentLocation"></param>
		void Activate(IWizardPage previousPage, WizardNavigationLocation currentLocation, WizardNavigationReasons reason);

		/// <summary>
		/// Called when the page becomes ready to allow redirection from within the page from the wizard itself
		/// </summary>
		/// <param name="previousPage"></param>
		/// <param name="currentLocation"></param>
		/// <param name="reason"></param>
		void ReadyToPerformRedirections(IWizardPage previousPage, WizardNavigationLocation currentLocation, WizardNavigationReasons reason);

		/// <summary>
		/// Called when the page gets deactivated
		/// </summary>
		void Deactivate(WizardNavigationReasons reason);		

		/// <summary>
		/// Sets styles based on an AND'd combination of flags
		/// </summary>
		/// <param name="styleIfTrue"></param>
		/// <param name="styleIfFalse"></param>
		/// <param name="flags"></param>
		/// <returns></returns>
		bool SetButtonStyleIfAllAreTrue(WizardButtonStyle styleIfTrue, WizardButtonStyle styleIfFalse, params bool[] flags);

		/// <summary>
		/// Sets styles based on an OR'd combination of flags
		/// </summary>
		/// <param name="styleIfTrue"></param>
		/// <param name="styleIfFalse"></param>
		/// <param name="flags"></param>
		/// <returns></returns>
		bool SetButtonStyleIfAnyAreTrue(WizardButtonStyle styleIfTrue, WizardButtonStyle styleIfFalse, params bool[] flags);
		
		/// <summary>
		/// Virtual method that should be overridden to set the Next button's style
		/// </summary>
		void SetMyNextButtonStyle(params object[] args);

		/// <summary>
		/// Virtual method that should be overridden to set the selected path 
		/// </summary>
		void SetMySelectedPath(params object[] args);
	}
}
