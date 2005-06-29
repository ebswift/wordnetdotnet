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

namespace Razor.Features
{
	/// <summary>
	/// Summary description for FeatureWindow.
	/// </summary>
	public class FeatureWindow : System.Windows.Forms.Form
	{
		private ListViewSortManager _sortManager;
		private System.Windows.Forms.ListView _listView;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.Button buttonSelectNone;
		private System.Windows.Forms.Button buttonSelectAll;
		private Razor.InformationPanel _informationPanel;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FeatureWindow()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			ArrayList array = new ArrayList();
			for(int i = 0; i < _listView.Columns.Count; i++)
				array.Add(typeof(ListViewTextCaseInsensitiveSort));
			_sortManager = new ListViewSortManager(_listView, (Type[])array.ToArray(typeof(Type)));
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FeatureWindow));
			this._listView = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonSelectNone = new System.Windows.Forms.Button();
			this.buttonSelectAll = new System.Windows.Forms.Button();
			this._informationPanel = new Razor.InformationPanel();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _listView
			// 
			this._listView.CheckBoxes = true;
			this._listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this.columnHeader1,
																						this.columnHeader2,
																						this.columnHeader3});
			this._listView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._listView.FullRowSelect = true;
			this._listView.Location = new System.Drawing.Point(0, 0);
			this._listView.Name = "_listView";
			this._listView.Size = new System.Drawing.Size(562, 199);
			this._listView.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this._listView.TabIndex = 0;
			this._listView.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Feature Name";
			this.columnHeader1.Width = 180;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Action";
			this.columnHeader2.Width = 120;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Feature Description";
			this.columnHeader3.Width = 180;
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.BackColor = System.Drawing.SystemColors.Control;
			this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonOK.Location = new System.Drawing.Point(425, 330);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 1;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.BackColor = System.Drawing.SystemColors.Control;
			this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonCancel.Location = new System.Drawing.Point(505, 330);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonSelectNone
			// 
			this.buttonSelectNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonSelectNone.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonSelectNone.Location = new System.Drawing.Point(90, 330);
			this.buttonSelectNone.Name = "buttonSelectNone";
			this.buttonSelectNone.TabIndex = 7;
			this.buttonSelectNone.Text = "Select None";
			this.buttonSelectNone.Click += new System.EventHandler(this.buttonSelectNone_Click);
			// 
			// buttonSelectAll
			// 
			this.buttonSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonSelectAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonSelectAll.Location = new System.Drawing.Point(10, 330);
			this.buttonSelectAll.Name = "buttonSelectAll";
			this.buttonSelectAll.TabIndex = 6;
			this.buttonSelectAll.Text = "Select All";
			this.buttonSelectAll.Click += new System.EventHandler(this.buttonSelectAll_Click);
			// 
			// _informationPanel
			// 
			this._informationPanel.BackColor = System.Drawing.Color.White;
			this._informationPanel.Description = "The following items represent features on which you may take certain actions upon" +
				". Common actions include resetting an option, toolbar layout, or reinstalling a " +
				"SnapIn. ";
			this._informationPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this._informationPanel.Image = ((System.Drawing.Image)(resources.GetObject("_informationPanel.Image")));
			this._informationPanel.Location = new System.Drawing.Point(0, 0);
			this._informationPanel.Name = "_informationPanel";
			this._informationPanel.Size = new System.Drawing.Size(592, 85);
			this._informationPanel.TabIndex = 8;
			this._informationPanel.Title = "Available Features";
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Location = new System.Drawing.Point(10, 95);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(570, 225);
			this.tabControl1.TabIndex = 9;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this._listView);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(562, 199);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Features";
			// 
			// FeatureWindow
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(592, 366);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this._informationPanel);
			this.Controls.Add(this.buttonSelectNone);
			this.Controls.Add(this.buttonSelectAll);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Name = "FeatureWindow";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Feature and Troubleshooting Management";
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Gets or sets the collection of features that are displayed in the dialog
		/// </summary>
		public FeatureCollection SelectedFeatures
		{
			get
			{
				FeatureCollection features = new FeatureCollection();
				foreach(ListViewItem item in this._listView.Items)
				{
					FeatureListViewItem featureItem = item as FeatureListViewItem;
					if (featureItem != null)
					{
						features.Add(featureItem.Feature);
					}
				}
				return features;
			}
			set
			{											
				this.DisplayFeatures(value);
			}
		}

		/// <summary>
		/// Gets the collection of features that have been checked in the dialog
		/// </summary>
		public FeatureCollection CheckedFeatures
		{
			get
			{
				FeatureCollection features = new FeatureCollection();
				foreach(ListViewItem item in this._listView.Items)
				{
					if (item.Checked)
					{
						FeatureListViewItem featureItem = item as FeatureListViewItem;
						if (featureItem != null)
						{
							features.Add(featureItem.Feature);
						}
					}
				}
				return features;	
			}			
		}

		/// <summary>
		/// Displays a collection of features in the dialog
		/// </summary>
		/// <param name="features"></param>
		private void DisplayFeatures(FeatureCollection features)
		{
			try
			{
				this._listView.BeginUpdate();
				this._listView.Items.Clear();

				if (features == null)
				{
					// if there are no items, reset the width of the headers to just something livable
					foreach(ColumnHeader h in this._listView.Columns)
						h.Width = 100;
				}
				else
				{
					// otherwise add each feature into the listview
					foreach(Feature f in features)
					{
						FeatureListViewItem item = new FeatureListViewItem(f);
						this._listView.Items.Add(item);
					}

					// and finally auto adjust the headers
					foreach(ColumnHeader h in this._listView.Columns)
						h.Width = -2;
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			finally
			{
				this._listView.EndUpdate();
			}
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void buttonSelectAll_Click(object sender, System.EventArgs e)
		{
			this.CheckAllItems(true);
		}	

		private void buttonSelectNone_Click(object sender, System.EventArgs e)
		{
			this.CheckAllItems(false);
		}
		
		private void CheckAllItems(bool check)
		{
			foreach(ListViewItem item in this._listView.Items)
				item.Checked = check;
		}
	}
}
