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

namespace Razor.SnapIns
{
	/// <summary>
	/// Summary description for SnapInDescriptorsWindow.
	/// </summary>
	public class SnapInDescriptorsWindow : System.Windows.Forms.Form
	{
		private ListViewSortManager _sortManager;
		private System.Windows.Forms.ImageList _smallImageList;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.ListView _listView;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private Razor.InformationPanel _informationPanel;
		private System.Windows.Forms.ContextMenu _contextMenu;
		private System.Windows.Forms.MenuItem menuItemStart;
		private System.Windows.Forms.MenuItem menuItemStop;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItemProperties;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItemReinstall;
		private System.Windows.Forms.MenuItem menuItemUninstall;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.Button buttonOK;
		private System.ComponentModel.IContainer components;

		public SnapInDescriptorsWindow(SnapInDescriptor[] descriptors)
		{
			this.InitializeComponent();
			this.InitializesSortManagers();			
			this.DisplayDescriptors(descriptors);			
//			this.SelectFirstItem();
			
			SnapInHostingEngine.GetExecutingInstance().SnapInStarted += new SnapInDescriptorEventHandler(SnapInDescriptorsWindow_SnapInStarted);
			SnapInHostingEngine.GetExecutingInstance().SnapInStopped += new SnapInDescriptorEventHandler(SnapInDescriptorsWindow_SnapInStopped);
			SnapInHostingEngine.GetExecutingInstance().SnapInInstalled += new SnapInDescriptorEventHandler(SnapInDescriptorsWindow_SnapInInstalled);
			SnapInHostingEngine.GetExecutingInstance().SnapInUninstalled += new SnapInDescriptorEventHandler(SnapInDescriptorsWindow_SnapInUninstalled);
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SnapInDescriptorsWindow));
			this._smallImageList = new System.Windows.Forms.ImageList(this.components);
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this._listView = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this._contextMenu = new System.Windows.Forms.ContextMenu();
			this.menuItemStart = new System.Windows.Forms.MenuItem();
			this.menuItemStop = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItemReinstall = new System.Windows.Forms.MenuItem();
			this.menuItemUninstall = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItemProperties = new System.Windows.Forms.MenuItem();
			this._informationPanel = new Razor.InformationPanel();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.buttonOK = new System.Windows.Forms.Button();
			this.tabControl1.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.SuspendLayout();
			// 
			// _smallImageList
			// 
			this._smallImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this._smallImageList.ImageSize = new System.Drawing.Size(16, 16);
			this._smallImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_smallImageList.ImageStream")));
			this._smallImageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Location = new System.Drawing.Point(10, 95);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(570, 230);
			this.tabControl1.TabIndex = 2;
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this._listView);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Size = new System.Drawing.Size(562, 204);
			this.tabPage3.TabIndex = 0;
			this.tabPage3.Text = "SnapIns";
			// 
			// _listView
			// 
			this._listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this.columnHeader1,
																						this.columnHeader2,
																						this.columnHeader3});
			this._listView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._listView.FullRowSelect = true;
			this._listView.Location = new System.Drawing.Point(0, 0);
			this._listView.Name = "_listView";
			this._listView.Size = new System.Drawing.Size(562, 204);
			this._listView.SmallImageList = this._smallImageList;
			this._listView.TabIndex = 1;
			this._listView.View = System.Windows.Forms.View.Details;
			this._listView.DoubleClick += new System.EventHandler(this.OnListViewDoubleClick);
			this._listView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnListViewMouseUp);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Title or Type";
			this.columnHeader1.Width = 253;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Description";
			this.columnHeader2.Width = 128;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Status";
			// 
			// _contextMenu
			// 
			this._contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.menuItemStart,
																						 this.menuItemStop,
																						 this.menuItem3,
																						 this.menuItemReinstall,
																						 this.menuItemUninstall,
																						 this.menuItem1,
																						 this.menuItemProperties});
			// 
			// menuItemStart
			// 
			this.menuItemStart.Enabled = false;
			this.menuItemStart.Index = 0;
			this.menuItemStart.Text = "Start";
			this.menuItemStart.Click += new System.EventHandler(this.OnMenuItemClick);
			// 
			// menuItemStop
			// 
			this.menuItemStop.Enabled = false;
			this.menuItemStop.Index = 1;
			this.menuItemStop.Text = "Stop";
			this.menuItemStop.Click += new System.EventHandler(this.OnMenuItemClick);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 2;
			this.menuItem3.Text = "-";
			// 
			// menuItemReinstall
			// 
			this.menuItemReinstall.Enabled = false;
			this.menuItemReinstall.Index = 3;
			this.menuItemReinstall.Text = "Reinstall";
			this.menuItemReinstall.Click += new System.EventHandler(this.OnMenuItemClick);
			// 
			// menuItemUninstall
			// 
			this.menuItemUninstall.Enabled = false;
			this.menuItemUninstall.Index = 4;
			this.menuItemUninstall.Text = "Uninstall";
			this.menuItemUninstall.Click += new System.EventHandler(this.OnMenuItemClick);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 5;
			this.menuItem1.Text = "-";
			// 
			// menuItemProperties
			// 
			this.menuItemProperties.Index = 6;
			this.menuItemProperties.Text = "Properties";
			this.menuItemProperties.Click += new System.EventHandler(this.OnMenuItemClick);
			// 
			// _informationPanel
			// 
			this._informationPanel.BackColor = System.Drawing.Color.White;
			this._informationPanel.Description = "The following SnapIns were loaded by the SnapIn Hosting Engine.  You may start, s" +
				"top, or view their properties by double clicking on a SnapIn listed below.";
			this._informationPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this._informationPanel.Image = ((System.Drawing.Image)(resources.GetObject("_informationPanel.Image")));
			this._informationPanel.Location = new System.Drawing.Point(0, 0);
			this._informationPanel.Name = "_informationPanel";
			this._informationPanel.Size = new System.Drawing.Size(592, 85);
			this._informationPanel.TabIndex = 5;
			this._informationPanel.Title = "Available SnapIns";
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonOK.Location = new System.Drawing.Point(505, 335);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 6;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// SnapInDescriptorsWindow
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(592, 366);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this._informationPanel);
			this.Controls.Add(this.tabControl1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Menu = this.mainMenu1;
			this.Name = "SnapInDescriptorsWindow";
			this.Text = "SnapIns";
			this.tabControl1.ResumeLayout(false);
			this.tabPage3.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		protected virtual void InitializesSortManagers()
		{
			Type[] sorters = new Type[_listView.Columns.Count];

			for (int i = 0; i < _listView.Columns.Count; i++)
				sorters[i] = typeof(ListViewTextSort);

			_sortManager = new ListViewSortManager(_listView, sorters);
			_sortManager.Sort(0);
		}

		private void DisplayDescriptors(SnapInDescriptor[] descriptors)
		{
			try
			{
				_listView.BeginUpdate();
				_listView.Items.Clear();
				_listView.Sorting = SortOrder.Ascending;

				foreach(SnapInDescriptor descriptor in descriptors)
				{
					ListViewItem item = new ListViewItem(descriptor.MetaData.Title);
					item.SubItems.Add(descriptor.MetaData.Description);
					item.SubItems.Add((descriptor.IsStarted ? "Started" : null));
					item.Tag = descriptor;
					
					// use the descriptors image if we can
					item.ImageIndex = _smallImageList.Images.Count;
					_smallImageList.Images.Add(descriptor.MetaData.Image);					
					
					// mark those that are uninstalled
					if (descriptor.IsUninstalled)
						item.ForeColor = SystemColors.GrayText;

					_listView.Items.Add(item);
				}

				_listView.EndUpdate();
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}        

		private void SelectFirstItem()
		{
			if (_listView.Items != null)
			{
				if (_listView.Items.Count > 0)
				{
					_listView.Items[0].Selected = true;
				}
			}
		}

		private void OnListViewDoubleClick(object sender, System.EventArgs e)
		{	
			try
			{
				ListViewItem item = _listView.SelectedItems[0];
				if (item != null)					
				{
					SnapInDescriptor descriptor = item.Tag as SnapInDescriptor;
					if (descriptor != null)
					{
						SnapInDescriptorPropertyWindow window = new SnapInDescriptorPropertyWindow(descriptor);
						window.ShowDialog(this);
					}					
				}
			}
			catch(System.Exception systemException)
			{				
				System.Diagnostics.Trace.WriteLine(systemException);
			}			
		}

		private void UpdateStatus(SnapInDescriptor descriptor)
		{
			foreach(ListViewItem item in _listView.Items)
			{
				SnapInDescriptor d = item.Tag as SnapInDescriptor;
				if (d != null)
				{
					if (d == descriptor)
					{
						item.SubItems[2].Text = (descriptor.IsStarted ? "Started" : null);
					}
				}
			}
		}

		private void UpdateContextMenu(SnapInDescriptor descriptor)
		{
			bool canStart;
			bool canStop;
			bool canReinstall;
			bool canUninstall;

			SnapInDescriptor.AdviseOnActionsThatCanBeTaken(descriptor, out canStart, out canStop, out canReinstall, out canUninstall);

			this.menuItemStart.Enabled = canStart;
			this.menuItemStop.Enabled  = canStop;
			this.menuItemReinstall.Enabled = canReinstall;
			this.menuItemUninstall.Enabled = canUninstall;
		}

		private void SnapInDescriptorsWindow_SnapInStarted(object sender, SnapInDescriptorEventArgs e)
		{
			this.UpdateStatus(e.Descriptor);
		}

		private void SnapInDescriptorsWindow_SnapInStopped(object sender, SnapInDescriptorEventArgs e)
		{
			this.UpdateStatus(e.Descriptor);
		}

		private void OnListViewMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				ListViewItem item = _listView.GetItemAt(e.X, e.Y);
				if (item != null)
				{
					SnapInDescriptor descriptor = item.Tag as SnapInDescriptor;
					if (descriptor != null)
					{
						this.UpdateContextMenu(descriptor);
						_contextMenu.Show(_listView, new Point(e.X, e.Y));
					}
				}
			}
		}

		private void OnMenuItemClick(object sender, System.EventArgs e)
		{
			SnapInDescriptor descriptor = null;

			try
			{
				ListViewItem item = _listView.SelectedItems[0];
				if (item != null)					
				{
					descriptor = item.Tag as SnapInDescriptor;										
				}
			}
			catch(System.Exception systemException)
			{				
				System.Diagnostics.Trace.WriteLine(systemException);
			}		

			MenuItem menuItem = sender as MenuItem;
			if (menuItem != null)
			{
				if (menuItem == menuItemStart)
				{
					if (descriptor != null)
					{
						SnapInHostingEngine.StartWithProgress(descriptor);					
					}
				}

				if (menuItem == menuItemStop)
				{
					if (descriptor != null)
					{
						SnapInHostingEngine.StopWithProgress(descriptor);
					}
				}

				if (menuItem == menuItemReinstall)
				{
					if (descriptor != null)
					{
						SnapInHostingEngine.ReinstallWithProgress(descriptor);
					}
				}

				if (menuItem == menuItemUninstall)
				{
					if (descriptor != null)
					{
						SnapInHostingEngine.StopAndUninstallWithProgress(descriptor);
					}
				}

				if (menuItem == menuItemProperties)
				{
					if (descriptor != null)
					{
						SnapInDescriptorPropertyWindow window = new SnapInDescriptorPropertyWindow(descriptor);
						window.ShowDialog(this);
					}
				}
			}
		}

		private void SnapInDescriptorsWindow_SnapInInstalled(object sender, SnapInDescriptorEventArgs e)
		{
			foreach(ListViewItem item in _listView.Items)
			{
				SnapInDescriptor descriptor = item.Tag as SnapInDescriptor;
				if (descriptor != null)
				{
					if (descriptor == e.Descriptor)
					{
						item.ForeColor = SystemColors.ControlText;
					}
				}
			}
		}

		private void SnapInDescriptorsWindow_SnapInUninstalled(object sender, SnapInDescriptorEventArgs e)
		{
			foreach(ListViewItem item in _listView.Items)
			{
				SnapInDescriptor descriptor = item.Tag as SnapInDescriptor;
				if (descriptor != null)
				{
					if (descriptor == e.Descriptor)
					{
						item.ForeColor = SystemColors.GrayText;
					}
				}
			}
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}
	}
}
