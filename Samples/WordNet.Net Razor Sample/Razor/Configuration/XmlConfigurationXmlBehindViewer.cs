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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Razor.Configuration
{
	/// <summary>
	/// Summary description for XmlConfigurationXmlBehindViewer.
	/// </summary>
	public class XmlConfigurationXmlBehindViewer : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.TextBox textBoxXml;
		private System.Windows.Forms.CheckBox checkBoxWordWrap;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public XmlConfigurationXmlBehindViewer()
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
			this.textBoxXml = new System.Windows.Forms.TextBox();
			this.checkBoxWordWrap = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// textBoxXml
			// 
			this.textBoxXml.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxXml.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.textBoxXml.Location = new System.Drawing.Point(5, 5);
			this.textBoxXml.Multiline = true;
			this.textBoxXml.Name = "textBoxXml";
			this.textBoxXml.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxXml.Size = new System.Drawing.Size(255, 205);
			this.textBoxXml.TabIndex = 0;
			this.textBoxXml.Text = "";
			this.textBoxXml.WordWrap = false;
			// 
			// checkBoxWordWrap
			// 
			this.checkBoxWordWrap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBoxWordWrap.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkBoxWordWrap.Location = new System.Drawing.Point(5, 215);
			this.checkBoxWordWrap.Name = "checkBoxWordWrap";
			this.checkBoxWordWrap.Size = new System.Drawing.Size(104, 15);
			this.checkBoxWordWrap.TabIndex = 1;
			this.checkBoxWordWrap.Text = "Word Wrap";
			this.checkBoxWordWrap.CheckedChanged += new System.EventHandler(this.checkBoxWordWrap_CheckedChanged);
			// 
			// XmlConfigurationXmlBehindViewer
			// 
			this.Controls.Add(this.checkBoxWordWrap);
			this.Controls.Add(this.textBoxXml);
			this.Name = "XmlConfigurationXmlBehindViewer";
			this.Size = new System.Drawing.Size(265, 235);
			this.ResumeLayout(false);

		}
		#endregion

		private void checkBoxWordWrap_CheckedChanged(object sender, System.EventArgs e)
		{
			this.textBoxXml.WordWrap = checkBoxWordWrap.Checked;
		}

		/// <summary>
		/// Gets or sets the Xml text to display in the XmlConfigurationXmlBehindViewer
		/// </summary>
		public string Xml
		{
			get
			{
				return this.textBoxXml.Text;
			}
			set
			{
				this.textBoxXml.Text = value;
			}
		}
	}
}
