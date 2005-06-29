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
	/// Summary description for PathPermissionViewer.
	/// </summary>
	public class PathPermissionViewer : System.Windows.Forms.UserControl
	{
		private SecurityAccessRight _right;
		private System.Windows.Forms.ListView _listView;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PathPermissionViewer()
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
			this._listView = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.SuspendLayout();
			// 
			// _listView
			// 
			this._listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this.columnHeader1,
																						this.columnHeader2,
																						this.columnHeader3});
			this._listView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._listView.HideSelection = false;
			this._listView.Location = new System.Drawing.Point(0, 0);
			this._listView.Name = "_listView";
			this._listView.Size = new System.Drawing.Size(185, 225);
			this._listView.TabIndex = 0;
			this._listView.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Permission";
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Allow";
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Deny";
			// 
			// PathPermissionViewer
			// 
			this.Controls.Add(this._listView);
			this.Name = "PathPermissionViewer";
			this.Size = new System.Drawing.Size(185, 225);
			this.ResumeLayout(false);

		}
		#endregion

		public void Display(SecurityAccessRight right)
		{
			try
			{
				if (_right != null)
					_right.Dispose();
				_right = right;
				_listView.BeginUpdate();
				_listView.Items.Clear();
				
				bool isDirectory = right.PertainsToADirectory();				

				this.AddItemForPermission(_right, SecurityAccessRights.FILE_GENERIC_EXECUTE);
				this.AddItemForPermission(_right, SecurityAccessRights.FILE_GENERIC_READ);
				this.AddItemForPermission(_right, SecurityAccessRights.FILE_GENERIC_WRITE);

//				this.AddItemForPermission(_right, !isDirectory, SecurityAccessRights.FILE_READ_DATA, SecurityAccessRights.FILE_LIST_DIRECTORY);
//				this.AddItemForPermission(_right, !isDirectory, SecurityAccessRights.FILE_WRITE_DATA, SecurityAccessRights.FILE_ADD_FILE);
//				this.AddItemForPermission(_right, !isDirectory, SecurityAccessRights.FILE_APPEND_DATA, SecurityAccessRights.FILE_ADD_SUBDIRECTORY);
//				this.AddItemForPermission(_right, !isDirectory, SecurityAccessRights.FILE_EXECUTE, SecurityAccessRights.FILE_TRAVERSE);				
				this.AddItemForPermission(_right, SecurityAccessRights.DELETE);
//				this.AddItemForPermission(_right, SecurityAccessRights.WRITE_DAC);
//				this.AddItemForPermission(_right, SecurityAccessRights.WRITE_OWNER);
//				this.AddItemForPermission(_right, SecurityAccessRights.FILE_READ_EA);
//				this.AddItemForPermission(_right, SecurityAccessRights.FILE_WRITE_EA);
//				this.AddItemForPermission(_right, SecurityAccessRights.WRITE_OWNER);
//				this.AddItemForPermission(_right, SecurityAccessRights.FILE_READ_ATTRIBUTES);
//				this.AddItemForPermission(_right, SecurityAccessRights.FILE_WRITE_ATTRIBUTES);	
			}
			catch
			{

			}
			finally
			{
				_listView.EndUpdate();
			}	
		}

		public ListViewItem AddItemForPermission(SecurityAccessRight right, bool isDirectory, SecurityAccessRights filePermission, SecurityAccessRights folderPermission)
		{
			SecurityAccessRights permission = (isDirectory ? folderPermission : filePermission);
			ListViewItem item = null;							
			item = new ListViewItem(EnumHelper.GetEnumValueDescription(permission, typeof(SecurityAccessRights)));
			item.Tag = permission;
			item.SubItems.Add((right.Assert(permission) ? "X" : ""));
			_listView.Items.Add(item);
			return item;
		}

		public ListViewItem AddItemForPermission(SecurityAccessRight right, SecurityAccessRights permission)
		{
			ListViewItem item = null;				
			item = new ListViewItem(EnumHelper.GetEnumValueDescription(permission, typeof(SecurityAccessRights)));
			item.Tag = permission;
			item.SubItems.Add((right.Assert(permission) ? "X" : ""));
			_listView.Items.Add(item);
			return item;
		}
	}
}
