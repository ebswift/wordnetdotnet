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
	/// Summary description for WizardPageEventArgs.
	/// </summary>
	public class WizardPageEventArgs : EventArgs
	{
		protected IWizardPage _wizardPage;

		/// <summary>
		/// Initializes a new instance of the WizardPageEventArgs class
		/// </summary>
		/// <param name="wizardPage">The page involved</param>
		/// <param name="direction">The direction of the change</param>
		public WizardPageEventArgs(IWizardPage wizardPage)
		{
			_wizardPage = wizardPage;
		}

		/// <summary>
		/// Returns the wizard page tied to this event
		/// </summary>
		public IWizardPage WizardPage
		{
			get
			{
				return _wizardPage;
			}
		}
	}

	public delegate void WizardPageEventHandler(object sender, WizardPageEventArgs e);	
}
