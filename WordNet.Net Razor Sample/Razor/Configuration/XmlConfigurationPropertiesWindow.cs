/*
 * This file is a part of the Razor Framework.
 * 
 * Copyright (C) 2004 Mark (Code6) Belles 
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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Razor.Configuration
{
	/// <summary>
	/// Summary description for XmlConfigurationPropertiesWindow.
	/// </summary>
	public class XmlConfigurationPropertiesWindow : System.Windows.Forms.Form
	{	
		protected bool _triggeredByButton;
		protected System.Windows.Forms.Button _buttonApply;
		protected System.Windows.Forms.Button _buttonCancel;
		protected System.Windows.Forms.Button _buttonOK;

		private Razor.Configuration.XmlConfigurationView _xmlConfigurationView;
		private System.ComponentModel.IContainer components;

		/// <summary>
		/// Initializes a new instance of the XmlConfigurationPropertiesWindow class
		/// </summary>
		public XmlConfigurationPropertiesWindow()
		{
			this.InitializeComponent();			

			this._buttonApply.Click += new System.EventHandler(this._buttonApply_Click);
			this._buttonCancel.Click += new System.EventHandler(this._buttonCancel_Click);
			this._buttonOK.Click += new System.EventHandler(this._buttonOK_Click);

			_xmlConfigurationView.ConfigurationChanged += new XmlConfigurationElementEventHandler(this.OnConfigurationChanged);
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
			this.components = new System.ComponentModel.Container();
			this._buttonApply = new System.Windows.Forms.Button();
			this._buttonCancel = new System.Windows.Forms.Button();
			this._buttonOK = new System.Windows.Forms.Button();
			this._xmlConfigurationView = new Razor.Configuration.XmlConfigurationView();
			this.SuspendLayout();
			// 
			// _buttonApply
			// 
			this._buttonApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonApply.Enabled = false;
			this._buttonApply.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._buttonApply.Location = new System.Drawing.Point(505, 335);
			this._buttonApply.Name = "_buttonApply";
			this._buttonApply.TabIndex = 1;
			this._buttonApply.Text = "Apply";
			this._buttonApply.Click += new System.EventHandler(this._buttonApply_Click);
			// 
			// _buttonCancel
			// 
			this._buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._buttonCancel.Location = new System.Drawing.Point(425, 335);
			this._buttonCancel.Name = "_buttonCancel";
			this._buttonCancel.TabIndex = 2;
			this._buttonCancel.Text = "Cancel";
			this._buttonCancel.Click += new System.EventHandler(this._buttonCancel_Click);
			// 
			// _buttonOK
			// 
			this._buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._buttonOK.Location = new System.Drawing.Point(345, 335);
			this._buttonOK.Name = "_buttonOK";
			this._buttonOK.TabIndex = 3;
			this._buttonOK.Text = "OK";
			this._buttonOK.Click += new System.EventHandler(this._buttonOK_Click);
			// 
			// _xmlConfigurationView
			// 
			this._xmlConfigurationView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._xmlConfigurationView.Location = new System.Drawing.Point(10, 10);
			this._xmlConfigurationView.Name = "_xmlConfigurationView";
			this._xmlConfigurationView.PlaceElementsIntoEditMode = true;
			this._xmlConfigurationView.Size = new System.Drawing.Size(575, 315);
			this._xmlConfigurationView.TabIndex = 4;
			// 
			// XmlConfigurationPropertiesWindow
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(592, 366);
			this.Controls.Add(this._xmlConfigurationView);
			this.Controls.Add(this._buttonOK);
			this.Controls.Add(this._buttonCancel);
			this.Controls.Add(this._buttonApply);
			this.MinimizeBox = false;
			this.Name = "XmlConfigurationPropertiesWindow";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Configuration Property Pages";
			this.ResumeLayout(false);

		}
		#endregion
		
		/// <summary>
		/// Occurs as the window is closing
		/// </summary>
		/// <param name="e"></param>
		protected override void OnClosing(CancelEventArgs e)
		{
			// cancel any changes if the window is not closing by a button
			if (!_triggeredByButton)
				_xmlConfigurationView.CancelEdit();

			base.OnClosing (e);
		}

		/// <summary>
		/// Occurs when the Cancel button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void _buttonCancel_Click(object sender, System.EventArgs e)
		{
			_triggeredByButton = true;
			this.DialogResult = DialogResult.Cancel;
			_xmlConfigurationView.CancelEdit();
			this.Close();
		}

		/// <summary>
		/// Occurs when the OK button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void _buttonOK_Click(object sender, System.EventArgs e)
		{				
			_triggeredByButton = true;
			this.DialogResult = DialogResult.OK;
			_xmlConfigurationView.ApplyChanges();
			_xmlConfigurationView.CancelEdit();
			this.Close();
		}

		/// <summary>
		/// Occurs when the Apply button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void _buttonApply_Click(object sender, System.EventArgs e)
		{
			_xmlConfigurationView.ApplyChanges();			
			_buttonApply.Enabled = false;
		}

		/// <summary>
		/// Occurs when a change is made to a configuration thru the XmlConfigurationView
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnConfigurationChanged(object sender, XmlConfigurationElementEventArgs e)
		{
			if (e.Element.IsBeingEdited)
				_buttonApply.Enabled = true;
		}	

		/// <summary>
		/// Returns the XmlConfigurationView control that displays/edits the configurations
		/// </summary>
		public XmlConfigurationView XmlConfigurationView
		{
			get
			{
				return _xmlConfigurationView;
			}
		}

		/// <summary>
		/// Gets or sets the selected configurations for the XmlConfigurationView which displays/edits the configurations
		/// </summary>
		public XmlConfigurationCollection SelectedConfigurations
		{
			get
			{
				return _xmlConfigurationView.SelectedConfigurations;
			}
			set
			{
				_xmlConfigurationView.SelectedConfigurations = value;
			}
		}

		#region Wrappers for backwards compatibility, becase i'm a backstabbing ass monkey and like to change things and break things :)

		public void AddConfiguration(XmlConfiguration configuration)
		{
			_xmlConfigurationView.AddConfiguration(configuration);
		}

		public void RemoveConfiguration(XmlConfiguration configuration, bool keepLocationIfPossible)
		{
			_xmlConfigurationView.RemoveConfiguration(configuration, keepLocationIfPossible);
		}

		public void RefreshDisplay()
		{
			_xmlConfigurationView.RefreshDisplay();
		}

		#endregion
	}
}
