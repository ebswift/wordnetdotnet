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
using System.Collections;
using System.Windows.Forms;

namespace Razor.Wizards
{
	/// <summary>
	/// Summary description for IWizard.
	/// </summary>
	public interface IWizard
	{
		event WizardEventHandler		WizardStarting;		
		event WizardPageEventHandler	WizardNavigatedToPage;
		event WizardEventHandler		WizardNeedsHelp;				
		event WizardCancelEventHandler	BeforeWizardCancelled;
		event WizardEventHandler		WizardCancelled;				
		event WizardCancelEventHandler	BeforeWizardFinished;
		event WizardEventHandler		WizardFinished;

		Button HelpButton { get; }
		Button BackButton { get; }
		Button NextButton { get; }
		Button CancelButton { get; }

		string Title { get; set; }
		WizardNavigationMap NavigationMap { get; }		
		WizardPageDescriptorList PagesUsed { get; }
		IWizardPage CurrentPage { get; }
		void SetButtonStyle(WizardButtonStyle style);
		WizardButtonStyle GetButtonStyle(WizardButtons button);
		
		Hashtable PropertyBag{ get; }
		bool ContainsProperty(object key);
		void RemoveProperty(object key);
		object ReadProperty(object key);
		void WriteProperty(object key, object value);

		bool CancelWizard();
		bool FinishWizard();
		bool Start();
		bool CanGoBack();
		bool GoBack();
		bool GoForward();
		bool Goto(WizardNavigationLocation location, bool keepInHistory, WizardNavigationReasons reason, bool throwExceptions);
		WizardPageDescriptor GetPageDescriptor(WizardNavigationLocation location);
		bool SetButtonStylesForPage(WizardPageDescriptor descriptor, bool throwExceptions);
		bool SitePage(IWizardPage wizardPage, bool throwExceptions);
		bool UnSitePage(IWizardPage wizardPage, bool throwExceptions);
		bool ActivatePage(IWizardPage wizardPage, IWizardPage previousPage, WizardNavigationLocation location, WizardNavigationReasons reason, bool throwExceptions);
		bool DeactivatePage(IWizardPage wizardPage, WizardNavigationReasons reason, bool throwExceptions);
	}
}
