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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Razor.Wizards
{
	/// <summary>
	/// Summary description for WizardStartPage.
	/// </summary>
	public class WizardStartPage : WizardPageBase
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Initializes a new instance of the WizardStartPage class
		/// </summary>
		public WizardStartPage() : base()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(WizardStartPage));
			// 
			// WizardStartPage
			// 
			this.BackColor = System.Drawing.Color.White;
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.BackImageAlignment = System.Drawing.ContentAlignment.TopLeft;
			this.Name = "WizardStartPage";
			this.Size = new System.Drawing.Size(502, 311);

		}
		#endregion

		/// <summary>
		/// Overrides the standard IWizardPage.Activate method so that a default route may be selected from the available paths
		/// </summary>
		/// <param name="previousPage"></param>
		/// <param name="currentLocation"></param>
		/// <param name="reason"></param>
		public override void Activate(IWizardPage previousPage, WizardNavigationLocation currentLocation, WizardNavigationReasons reason)
		{
			// always call to the base class first
			base.Activate (previousPage, currentLocation, reason);

			// assuming the current location isn't null			
			if (this.CurrentLocation != null)
				// by default just try and select the "Next" route in the path list
				this.CurrentLocation.Paths.TryAndSelectPath("Next");
		}

	}
}
