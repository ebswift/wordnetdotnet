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
using Razor.Attributes;

namespace Razor.SnapIns
{
	/// <summary>
	/// Summary description for SnapInDescriptorPropertyWindow.
	/// </summary>
	public class SnapInDescriptorPropertyWindow : System.Windows.Forms.Form
	{		
		private SnapInDescriptor _descriptor;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.ListBox listBoxDevelopers;
		private System.Windows.Forms.Button buttonStop;
		private System.Windows.Forms.Button buttonStart;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.ListBox listBoxProductFamilies;
		private System.Windows.Forms.TreeView treeViewReferences;
		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.Label labelVersion;
		private System.Windows.Forms.Label labelCompany;
		private System.Windows.Forms.TextBox textBoxPath;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.CheckBox checkBoxIsMissingDependency;
		private System.Windows.Forms.CheckBox checkBoxHasADependencyThatIsMissingDependency;
		private System.Windows.Forms.CheckBox checkBoxIsCircularlyDependent;
		private System.Windows.Forms.CheckBox checkBoxHasADependencyThatIsCircularlyDependent;
		private System.Windows.Forms.Button buttonUninstall;
		private System.Windows.Forms.CheckBox checkBoxIsUninstalled;
		private Razor.InformationPanel _informationPanel;
		private System.Windows.Forms.TabControl tabControl2;
		private System.Windows.Forms.TabPage tabPage4;
		private System.Windows.Forms.TabPage tabPage5;
		private System.Windows.Forms.TreeView treeViewReferenced;
		private System.Windows.Forms.Button buttonReInstall;
		private System.ComponentModel.IContainer components;

		public SnapInDescriptorPropertyWindow(SnapInDescriptor descriptor)
		{
			this.InitializeComponent();
			
			// save a reference to the descriptors
			_descriptor = descriptor;
			
			// display all of the information about the descriptor
			this.DisplayDescriptor(descriptor);

			this.UpdateButtonsBasedOnAdvice();			
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SnapInDescriptorPropertyWindow));
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.textBoxPath = new System.Windows.Forms.TextBox();
			this.labelCompany = new System.Windows.Forms.Label();
			this.labelVersion = new System.Windows.Forms.Label();
			this.listBoxProductFamilies = new System.Windows.Forms.ListBox();
			this.label9 = new System.Windows.Forms.Label();
			this.listBoxDevelopers = new System.Windows.Forms.ListBox();
			this.label13 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.tabControl2 = new System.Windows.Forms.TabControl();
			this.tabPage4 = new System.Windows.Forms.TabPage();
			this.treeViewReferences = new System.Windows.Forms.TreeView();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.tabPage5 = new System.Windows.Forms.TabPage();
			this.treeViewReferenced = new System.Windows.Forms.TreeView();
			this.label6 = new System.Windows.Forms.Label();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.checkBoxIsUninstalled = new System.Windows.Forms.CheckBox();
			this.checkBoxHasADependencyThatIsCircularlyDependent = new System.Windows.Forms.CheckBox();
			this.checkBoxIsCircularlyDependent = new System.Windows.Forms.CheckBox();
			this.checkBoxHasADependencyThatIsMissingDependency = new System.Windows.Forms.CheckBox();
			this.checkBoxIsMissingDependency = new System.Windows.Forms.CheckBox();
			this.buttonUninstall = new System.Windows.Forms.Button();
			this.buttonStop = new System.Windows.Forms.Button();
			this.buttonStart = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this._informationPanel = new Razor.InformationPanel();
			this.buttonReInstall = new System.Windows.Forms.Button();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tabControl2.SuspendLayout();
			this.tabPage4.SuspendLayout();
			this.tabPage5.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Location = new System.Drawing.Point(10, 90);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(570, 235);
			this.tabControl1.TabIndex = 2;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.textBoxPath);
			this.tabPage1.Controls.Add(this.labelCompany);
			this.tabPage1.Controls.Add(this.labelVersion);
			this.tabPage1.Controls.Add(this.listBoxProductFamilies);
			this.tabPage1.Controls.Add(this.label9);
			this.tabPage1.Controls.Add(this.listBoxDevelopers);
			this.tabPage1.Controls.Add(this.label13);
			this.tabPage1.Controls.Add(this.label11);
			this.tabPage1.Controls.Add(this.label5);
			this.tabPage1.Controls.Add(this.label4);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(562, 209);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "General";
			// 
			// textBoxPath
			// 
			this.textBoxPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxPath.BackColor = System.Drawing.SystemColors.Control;
			this.textBoxPath.Location = new System.Drawing.Point(95, 60);
			this.textBoxPath.Name = "textBoxPath";
			this.textBoxPath.Size = new System.Drawing.Size(460, 20);
			this.textBoxPath.TabIndex = 35;
			this.textBoxPath.Text = "";
			// 
			// labelCompany
			// 
			this.labelCompany.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.labelCompany.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelCompany.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelCompany.Location = new System.Drawing.Point(95, 10);
			this.labelCompany.Name = "labelCompany";
			this.labelCompany.Size = new System.Drawing.Size(460, 20);
			this.labelCompany.TabIndex = 33;
			// 
			// labelVersion
			// 
			this.labelVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.labelVersion.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelVersion.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelVersion.Location = new System.Drawing.Point(95, 35);
			this.labelVersion.Name = "labelVersion";
			this.labelVersion.Size = new System.Drawing.Size(460, 20);
			this.labelVersion.TabIndex = 32;
			// 
			// listBoxProductFamilies
			// 
			this.listBoxProductFamilies.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxProductFamilies.BackColor = System.Drawing.SystemColors.Control;
			this.listBoxProductFamilies.Location = new System.Drawing.Point(95, 145);
			this.listBoxProductFamilies.Name = "listBoxProductFamilies";
			this.listBoxProductFamilies.Size = new System.Drawing.Size(460, 56);
			this.listBoxProductFamilies.TabIndex = 29;
			// 
			// label9
			// 
			this.label9.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label9.Location = new System.Drawing.Point(10, 60);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(80, 20);
			this.label9.TabIndex = 22;
			this.label9.Text = "Path";
			// 
			// listBoxDevelopers
			// 
			this.listBoxDevelopers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxDevelopers.BackColor = System.Drawing.SystemColors.Control;
			this.listBoxDevelopers.Location = new System.Drawing.Point(95, 85);
			this.listBoxDevelopers.Name = "listBoxDevelopers";
			this.listBoxDevelopers.Size = new System.Drawing.Size(460, 56);
			this.listBoxDevelopers.TabIndex = 20;
			// 
			// label13
			// 
			this.label13.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label13.Location = new System.Drawing.Point(10, 35);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(80, 20);
			this.label13.TabIndex = 18;
			this.label13.Text = "Version";
			// 
			// label11
			// 
			this.label11.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label11.Location = new System.Drawing.Point(10, 145);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(81, 20);
			this.label11.TabIndex = 16;
			this.label11.Text = "Product Family";
			// 
			// label5
			// 
			this.label5.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label5.Location = new System.Drawing.Point(10, 85);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(80, 20);
			this.label5.TabIndex = 11;
			this.label5.Text = "Developers";
			// 
			// label4
			// 
			this.label4.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label4.Location = new System.Drawing.Point(10, 10);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(80, 20);
			this.label4.TabIndex = 10;
			this.label4.Text = "Company";
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.tabControl2);
			this.tabPage2.Controls.Add(this.label6);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Size = new System.Drawing.Size(562, 209);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Dependencies";
			// 
			// tabControl2
			// 
			this.tabControl2.Controls.Add(this.tabPage4);
			this.tabControl2.Controls.Add(this.tabPage5);
			this.tabControl2.Location = new System.Drawing.Point(5, 40);
			this.tabControl2.Name = "tabControl2";
			this.tabControl2.SelectedIndex = 0;
			this.tabControl2.Size = new System.Drawing.Size(555, 165);
			this.tabControl2.TabIndex = 5;
			// 
			// tabPage4
			// 
			this.tabPage4.Controls.Add(this.treeViewReferences);
			this.tabPage4.Location = new System.Drawing.Point(4, 22);
			this.tabPage4.Name = "tabPage4";
			this.tabPage4.Size = new System.Drawing.Size(547, 139);
			this.tabPage4.TabIndex = 0;
			this.tabPage4.Text = "Dependencies";
			// 
			// treeViewReferences
			// 
			this.treeViewReferences.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeViewReferences.ImageList = this.imageList;
			this.treeViewReferences.Location = new System.Drawing.Point(0, 0);
			this.treeViewReferences.Name = "treeViewReferences";
			this.treeViewReferences.Size = new System.Drawing.Size(547, 139);
			this.treeViewReferences.TabIndex = 3;
			this.treeViewReferences.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewReferences_BeforeExpand);
			// 
			// imageList
			// 
			this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.imageList.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// tabPage5
			// 
			this.tabPage5.Controls.Add(this.treeViewReferenced);
			this.tabPage5.Location = new System.Drawing.Point(4, 22);
			this.tabPage5.Name = "tabPage5";
			this.tabPage5.Size = new System.Drawing.Size(547, 139);
			this.tabPage5.TabIndex = 1;
			this.tabPage5.Text = "SnapIns that depend on this SnapIn";
			// 
			// treeViewReferenced
			// 
			this.treeViewReferenced.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeViewReferenced.ImageList = this.imageList;
			this.treeViewReferenced.Location = new System.Drawing.Point(0, 0);
			this.treeViewReferenced.Name = "treeViewReferenced";
			this.treeViewReferenced.Size = new System.Drawing.Size(547, 139);
			this.treeViewReferenced.TabIndex = 5;
			this.treeViewReferenced.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewReferenced_BeforeExpand);
			// 
			// label6
			// 
			this.label6.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label6.Location = new System.Drawing.Point(10, 10);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(550, 30);
			this.label6.TabIndex = 0;
			this.label6.Text = "Some SnapIns depend on other SnapIns to function properly. If a SnapIn is stopped" +
				", uninstalled, or not functioning properly, the SnapIn(s) that depend upon this " +
				"SnapIn will be affected.";
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.checkBoxIsUninstalled);
			this.tabPage3.Controls.Add(this.checkBoxHasADependencyThatIsCircularlyDependent);
			this.tabPage3.Controls.Add(this.checkBoxIsCircularlyDependent);
			this.tabPage3.Controls.Add(this.checkBoxHasADependencyThatIsMissingDependency);
			this.tabPage3.Controls.Add(this.checkBoxIsMissingDependency);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Size = new System.Drawing.Size(562, 209);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Troubleshooting";
			// 
			// checkBoxIsUninstalled
			// 
			this.checkBoxIsUninstalled.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.checkBoxIsUninstalled.Location = new System.Drawing.Point(10, 10);
			this.checkBoxIsUninstalled.Name = "checkBoxIsUninstalled";
			this.checkBoxIsUninstalled.Size = new System.Drawing.Size(545, 25);
			this.checkBoxIsUninstalled.TabIndex = 4;
			this.checkBoxIsUninstalled.Text = "Is uninstalled";
			this.checkBoxIsUninstalled.CheckedChanged += new System.EventHandler(this.OnCheckBoxCheckChanged);
			// 
			// checkBoxHasADependencyThatIsCircularlyDependent
			// 
			this.checkBoxHasADependencyThatIsCircularlyDependent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.checkBoxHasADependencyThatIsCircularlyDependent.Location = new System.Drawing.Point(10, 130);
			this.checkBoxHasADependencyThatIsCircularlyDependent.Name = "checkBoxHasADependencyThatIsCircularlyDependent";
			this.checkBoxHasADependencyThatIsCircularlyDependent.Size = new System.Drawing.Size(545, 24);
			this.checkBoxHasADependencyThatIsCircularlyDependent.TabIndex = 3;
			this.checkBoxHasADependencyThatIsCircularlyDependent.Text = "Has a dependency that is circularly dependent";
			this.checkBoxHasADependencyThatIsCircularlyDependent.CheckedChanged += new System.EventHandler(this.OnCheckBoxCheckChanged);
			// 
			// checkBoxIsCircularlyDependent
			// 
			this.checkBoxIsCircularlyDependent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.checkBoxIsCircularlyDependent.Location = new System.Drawing.Point(10, 100);
			this.checkBoxIsCircularlyDependent.Name = "checkBoxIsCircularlyDependent";
			this.checkBoxIsCircularlyDependent.Size = new System.Drawing.Size(545, 24);
			this.checkBoxIsCircularlyDependent.TabIndex = 2;
			this.checkBoxIsCircularlyDependent.Text = "Is circularly dependent";
			this.checkBoxIsCircularlyDependent.CheckedChanged += new System.EventHandler(this.OnCheckBoxCheckChanged);
			// 
			// checkBoxHasADependencyThatIsMissingDependency
			// 
			this.checkBoxHasADependencyThatIsMissingDependency.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.checkBoxHasADependencyThatIsMissingDependency.Location = new System.Drawing.Point(10, 70);
			this.checkBoxHasADependencyThatIsMissingDependency.Name = "checkBoxHasADependencyThatIsMissingDependency";
			this.checkBoxHasADependencyThatIsMissingDependency.Size = new System.Drawing.Size(545, 24);
			this.checkBoxHasADependencyThatIsMissingDependency.TabIndex = 1;
			this.checkBoxHasADependencyThatIsMissingDependency.Text = "Has a dependency that is missing a dependency";
			this.checkBoxHasADependencyThatIsMissingDependency.CheckedChanged += new System.EventHandler(this.OnCheckBoxCheckChanged);
			// 
			// checkBoxIsMissingDependency
			// 
			this.checkBoxIsMissingDependency.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.checkBoxIsMissingDependency.Location = new System.Drawing.Point(10, 40);
			this.checkBoxIsMissingDependency.Name = "checkBoxIsMissingDependency";
			this.checkBoxIsMissingDependency.Size = new System.Drawing.Size(545, 24);
			this.checkBoxIsMissingDependency.TabIndex = 0;
			this.checkBoxIsMissingDependency.Text = "Is missing a dependency";
			this.checkBoxIsMissingDependency.CheckedChanged += new System.EventHandler(this.OnCheckBoxCheckChanged);
			// 
			// buttonUninstall
			// 
			this.buttonUninstall.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonUninstall.Location = new System.Drawing.Point(250, 335);
			this.buttonUninstall.Name = "buttonUninstall";
			this.buttonUninstall.TabIndex = 9;
			this.buttonUninstall.Text = "Uninstall";
			this.buttonUninstall.Click += new System.EventHandler(this.buttonUninstall_Click);
			// 
			// buttonStop
			// 
			this.buttonStop.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonStop.Location = new System.Drawing.Point(90, 335);
			this.buttonStop.Name = "buttonStop";
			this.buttonStop.TabIndex = 8;
			this.buttonStop.Text = "Stop";
			this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
			// 
			// buttonStart
			// 
			this.buttonStart.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonStart.Location = new System.Drawing.Point(10, 335);
			this.buttonStart.Name = "buttonStart";
			this.buttonStart.TabIndex = 7;
			this.buttonStart.Text = "Start";
			this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonOK.Location = new System.Drawing.Point(505, 335);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 3;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// _informationPanel
			// 
			this._informationPanel.BackColor = System.Drawing.Color.White;
			this._informationPanel.Description = "";
			this._informationPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this._informationPanel.Image = ((System.Drawing.Image)(resources.GetObject("_informationPanel.Image")));
			this._informationPanel.Location = new System.Drawing.Point(0, 0);
			this._informationPanel.Name = "_informationPanel";
			this._informationPanel.Size = new System.Drawing.Size(592, 85);
			this._informationPanel.TabIndex = 37;
			this._informationPanel.Title = "";
			// 
			// buttonReInstall
			// 
			this.buttonReInstall.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonReInstall.Location = new System.Drawing.Point(170, 335);
			this.buttonReInstall.Name = "buttonReInstall";
			this.buttonReInstall.TabIndex = 38;
			this.buttonReInstall.Text = "Reinstall";
			this.buttonReInstall.Click += new System.EventHandler(this.buttonReInstall_Click);
			// 
			// SnapInDescriptorPropertyWindow
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(592, 366);
			this.Controls.Add(this.buttonReInstall);
			this.Controls.Add(this._informationPanel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.buttonUninstall);
			this.Controls.Add(this.buttonStop);
			this.Controls.Add(this.buttonStart);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "SnapInDescriptorPropertyWindow";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "SnapIn Properties";
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.tabControl2.ResumeLayout(false);
			this.tabPage4.ResumeLayout(false);
			this.tabPage5.ResumeLayout(false);
			this.tabPage3.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void DisplayDescriptor(SnapInDescriptor descriptor)
		{
//			using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
//			{
//				descriptor.MetaData.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Icon);
//				this.Icon = new Icon(ms, 16, 16);
//			}
//			_informationPanel.Image = descriptor.MetaData.Image;
			_informationPanel.Title = descriptor.MetaData.Title;
			_informationPanel.Description = descriptor.MetaData.Description;

//			this.pictureBoxImage.Image = descriptor.MetaData.Image;						
//			this.labelTitle.Text = descriptor.MetaData.Title;
//			this.labelDescription.Text = descriptor.MetaData.Description;
			this.labelCompany.Text = descriptor.MetaData.CompanyName;
			this.labelVersion.Text = descriptor.MetaData.Version.ToString();
			this.textBoxPath.Text = descriptor.Type.Assembly.Location;

			foreach(string developer in descriptor.MetaData.Developers)
				this.listBoxDevelopers.Items.Add(developer);

			foreach(string productFamily in descriptor.MetaData.ProductFamilies)
				this.listBoxProductFamilies.Items.Add(productFamily);

			this.DisplayDependencyInformation();

			this.DisplayTroubleshootingInformation();
		}

		private void UpdateButtonsBasedOnAdvice()
		{
			bool canStart;
			bool canStop;
			bool canReinstall;
			bool canUninstall;

			SnapInDescriptor.AdviseOnActionsThatCanBeTaken(_descriptor, out canStart, out canStop, out canReinstall, out canUninstall);

			this.buttonStart.Enabled = canStart;
			this.buttonStop.Enabled  = canStop;
			this.buttonReInstall.Enabled = canReinstall;
			this.buttonUninstall.Enabled = canUninstall;

//			/// if there is a snapin selected
//			if (_descriptor != null)
//			{			
//				if (_descriptor.IsUninstalled)
//				{
//					this.buttonStart.Enabled = false;
//					this.buttonStop.Enabled  = false;
//					this.buttonReInstall.Enabled = true;
//					this.buttonUninstall.Enabled = false;
//				}
//				else
//				{
//					this.buttonStart.Enabled = !_descriptor.IsStarted;
//					this.buttonStop.Enabled  = _descriptor.IsStarted;
//					this.buttonReInstall.Enabled = false;
//					this.buttonUninstall.Enabled = true;
//				}
//			}
//			else
//			{
//				/// but you can't do anything without a snapin selected
//				this.buttonStart.Enabled = false;
//				this.buttonStop.Enabled = false;
//				this.buttonReInstall.Enabled = false;
//				this.buttonUninstall.Enabled = false;
//			}
		}

		private void DisplayDependencyInformation()
		{
			this.DisplayReferencesFor(treeViewReferences.Nodes, _descriptor);
			this.DisplayReferencesTo(treeViewReferenced.Nodes, _descriptor);
		}

		private void DisplayReferencesFor(TreeNodeCollection nodes, SnapInDescriptor descriptor)
		{
			foreach(Type t in descriptor.Dependencies)
			{
				SnapInDescriptor dependencyDescriptor = SnapInHostingEngine.GetExecutingInstance().FindDescriptorByType(t);
				if (dependencyDescriptor != null)
				{
					TreeNode node = new TreeNode(dependencyDescriptor.MetaData.Title);
					node.Tag = dependencyDescriptor;
					if (dependencyDescriptor.IsUninstalled)
						node.ForeColor = SystemColors.GrayText;
					node.Nodes.Add(string.Empty);
					nodes.Add(node);
				}
			}
		}
		
		private void DisplayReferencesTo(TreeNodeCollection nodes, SnapInDescriptor descriptor)
		{
			SnapInDescriptor[] descriptors = SnapInHostingEngine.GetExecutingInstance().SnapInDescriptors;
            
			foreach(SnapInDescriptor otherDescriptor in descriptors)
			{
				foreach(Type t in otherDescriptor.Dependencies)
				{
					if (t == descriptor.Type)
					{
						TreeNode node = new TreeNode(otherDescriptor.MetaData.Title);
						node.Tag = otherDescriptor;
						if (otherDescriptor.IsUninstalled)
							node.ForeColor = SystemColors.GrayText;
						node.Nodes.Add(string.Empty);
						nodes.Add(node);
					}
				}
			}			
		}	
		
		private void DisplayTroubleshootingInformation()
		{
			this.checkBoxIsUninstalled.Checked = _descriptor.IsUninstalled;
			this.checkBoxIsMissingDependency.Checked = _descriptor.IsMissingDependency;
			this.checkBoxHasADependencyThatIsMissingDependency.Checked = _descriptor.IsDependentOnTypeThatIsMissingDependency;
			this.checkBoxIsCircularlyDependent.Checked = _descriptor.IsCircularlyDependent;
			this.checkBoxHasADependencyThatIsCircularlyDependent.Checked = _descriptor.IsDependentOnTypeThatIsCircularlyDependent;
		}

		private void treeViewReferences_BeforeExpand(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			e.Node.Nodes.Clear();
			this.DisplayReferencesFor(e.Node.Nodes, e.Node.Tag as SnapInDescriptor);
		}

		private void treeViewReferenced_BeforeExpand(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			e.Node.Nodes.Clear();
			this.DisplayReferencesTo(e.Node.Nodes, e.Node.Tag as SnapInDescriptor);
		}

		private void buttonStop_Click(object sender, System.EventArgs e)
		{
			SnapInHostingEngine.StopWithProgress(_descriptor);				
			this.UpdateButtonsBasedOnAdvice();			
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void buttonStart_Click(object sender, System.EventArgs e)
		{
			SnapInHostingEngine.StartWithProgress(_descriptor);
			this.UpdateButtonsBasedOnAdvice();			
		}

		private void OnCheckBoxCheckChanged(object sender, System.EventArgs e)
		{
			// disallow the changing of the check state for all checkboxes
			this.DisplayTroubleshootingInformation();		
		}

		private void buttonUninstall_Click(object sender, System.EventArgs e)
		{
			SnapInHostingEngine.StopAndUninstallWithProgress(_descriptor);
			this.UpdateButtonsBasedOnAdvice();	
		}

		private void buttonReInstall_Click(object sender, System.EventArgs e)
		{
			SnapInHostingEngine.ReinstallWithProgress(_descriptor);
			this.UpdateButtonsBasedOnAdvice();
		}
	}
}
