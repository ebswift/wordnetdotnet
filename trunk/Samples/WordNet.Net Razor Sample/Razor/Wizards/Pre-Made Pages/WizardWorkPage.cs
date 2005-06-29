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
	/// Summary description for WizardWorkPage.
	/// </summary>
	public class WizardWorkPage : WizardPageBase	
	{
		private Razor.InformationPanel _infoPanel;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Initializes a new instance of the WizardWorkPage class
		/// </summary>
		public WizardWorkPage() : base()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(WizardWorkPage));
			this._infoPanel = new Razor.InformationPanel();
			this.SuspendLayout();
			// 
			// _infoPanel
			// 
			this._infoPanel.BackColor = System.Drawing.Color.White;
			this._infoPanel.Description = "";
			this._infoPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this._infoPanel.Image = ((System.Drawing.Image)(resources.GetObject("_infoPanel.Image")));
			this._infoPanel.Location = new System.Drawing.Point(0, 0);
			this._infoPanel.Name = "_infoPanel";
			this._infoPanel.Size = new System.Drawing.Size(502, 85);
			this._infoPanel.TabIndex = 0;
			this._infoPanel.Title = "";
			// 
			// WizardWorkPage
			// 
			this.Controls.Add(this._infoPanel);
			this.Name = "WizardWorkPage";
			this.Size = new System.Drawing.Size(502, 311);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Returns the InformationPanel used to display information about the current step
		/// </summary>
		public Razor.InformationPanel InfoPanel
		{
			get
			{
				return _infoPanel;
			}
		}
		
		/// <summary>
		/// Gets or sets the Title in the InformationPanel (ie. The bolded text.)
		/// </summary>
		public string InfoTitle
		{
			get
			{
				return _infoPanel.Title;
			}
			set
			{
				_infoPanel.Title = value;
			}
		}

		/// <summary>
		/// Gets or sets the Description in the InformationPanel (ie. The non-bolded normal text under the title.)
		/// </summary>
		public string InfoDescription
		{
			get
			{
				return _infoPanel.Description;
			}
			set
			{
				_infoPanel.Description = value;
			}
		}

		/// <summary>
		/// Gets or sets the Image in the InformationPanel (ie. The image in the upper right corner.)
		/// </summary>
		public Image InfoImage
		{
			get
			{
				return _infoPanel.Image;
			}
			set
			{
				_infoPanel.Image = value;
			}
		}
	}
}
