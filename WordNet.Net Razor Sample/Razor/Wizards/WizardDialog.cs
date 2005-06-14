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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Razor.Wizards
{
	/// <summary>
	/// Summary description for WizardDialog.
	/// </summary>
	public class WizardDialog : System.Windows.Forms.Form
	{
		/// <summary>
		/// The internal wizard that will be controlling the dialog
		/// </summary>
		protected Razor.Wizards.Wizard _wizard;

		protected bool _wizardCancelled;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Initializes a new instance of the WizardDialog class
		/// </summary>
		public WizardDialog()
		{
			this.InitializeComponent();	

			this.AcceptButton = _wizard.NextButton;
//			this.CancelButton = _wizard.CancelButton;
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._wizard = new Razor.Wizards.Wizard();
			this.SuspendLayout();
			// 
			// _wizard
			// 
			this._wizard.Dock = System.Windows.Forms.DockStyle.Fill;
			this._wizard.Location = new System.Drawing.Point(0, 0);
			this._wizard.Name = "_wizard";
			this._wizard.Size = new System.Drawing.Size(502, 356);
			this._wizard.TabIndex = 0;
			this._wizard.Title = "My Wizard";
			// 
			// WizardDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(502, 356);
			this.ControlBox = false;
			this.Controls.Add(this._wizard);
			this.MinimumSize = new System.Drawing.Size(450, 360);
			this.Name = "WizardDialog";
			this.Text = "Wizard";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Override the window load event and start the wizard
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);

			// set the window title to the wizard title so that we know what wizard we're looking at
			this.Text = _wizard.Title;

//			this._wizard.WizardNeedsHelp += new WizardEventHandler(OnWizardNeedsHelp);
//			this._wizard.WizardStarting += new WizardPageEventHandler(OnWizardStarting);
//			this._wizard.WizardNavigatedToPage += new WizardPageEventHandler(OnWizardNavigatedToPage);
			
			this._wizard.WizardCancelled += new WizardEventHandler(OnWizardCancelled);
			this._wizard.WizardFinished += new WizardEventHandler(OnWizardFinished);

			try
			{
				// start the wizard
				_wizard.Start();
			}
			catch(Exception ex)
			{
				MessageBox.Show(this, ex.ToString());
			}
		}

		/// <summary>
		/// Returns the IWizard interface for the current Wizard hosted upon this dialog
		/// </summary>
		public IWizard Wizard
		{
			get
			{
				return _wizard;
			}
		}

//		private void OnWizardNeedsHelp(object sender, WizardEventArgs e)
//		{
//
//		}

//		private void OnWizardStarting(object sender, EventArgs e)
//		{
//
//		}

//		private void OnWizardNavigatedToPage(object sender, EventArgs e)
//		{
//
//		}

		/// <summary>
		/// Occurs when the Wizard is cancelled
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnWizardCancelled(object sender, WizardEventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		/// <summary>
		/// Occurs when the Wizard is finished
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnWizardFinished(object sender, WizardEventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}
	}
}
