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
using Razor.Configuration;

namespace Razor.Wizards
{
	/// <summary>
	/// Summary description for WizardButton.
	/// </summary>
	public class WizardButton : System.Windows.Forms.Button 
	{
		protected WizardButtons _button;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Initializes a new instance of the WizardButton class
		/// </summary>
		public WizardButton() : base()
		{
			this.InitializeComponent();			
			this.FlatStyle = FlatStyle.System;
			this.SetStyle(new WizardButtonStyle(WizardButtons.None, true, false));
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
			components = new System.ComponentModel.Container();
		}
		#endregion

		/// <summary>
		/// Gets or sets the wizarding style of the button (May not be a combination in this context)
		/// </summary>
		public WizardButtons Button
		{
			get
			{
				return _button;
			}
			set
			{
				_button = value;

				this.SetStyle(new WizardButtonStyle(_button, this.Visible, this.Enabled));
			}
		}
		
		/// <summary>
		/// Applies the specified style to the button
		/// </summary>
		/// <param name="style"></param>
		public void SetStyle(WizardButtonStyle style)
		{
			switch(style.Button)
			{										
			case WizardButtons.Help:				
			case WizardButtons.Back:				
			case WizardButtons.Next:
			case WizardButtons.Finish:
			case WizardButtons.Cancel:
				this.Text = style.Button.ToString();
				break;
			case WizardButtons.None:
			default:
				this.Text = null;
				break;
			};		
			
			this._button = style.Button;
			this.Visible = style.Visible;
			this.Enabled = style.Enabled;
		}

		/// <summary>
		/// Returns the button's style
		/// </summary>
		/// <returns></returns>
		public WizardButtonStyle GetStyle()
		{
			return new WizardButtonStyle(_button, this.Visible, this.Enabled);
		}
	}
}
