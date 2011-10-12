using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
namespace WordNetControls
{
	///*
	// * This file is a part of the WordNet.Net open source project.
	// * 
	// * Copyright (C) 2005 Malcolm Crowe, Troy Simpson 
	// * 
	// * Project Home: http://www.ebswift.com
	// *
	// * This library is free software; you can redistribute it and/or
	// * modify it under the terms of the GNU Lesser General Public
	// * License as published by the Free Software Foundation; either
	// * version 2.1 of the License, or (at your option) any later version.
	// *
	// * This library is distributed in the hope that it will be useful,
	// * but WITHOUT ANY WARRANTY; without even the implied warranty of
	// * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
	// * Lesser General Public License for more details.
	// *
	// * You should have received a copy of the GNU Lesser General Public
	// * License along with this library; if not, write to the Free Software
	// * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
	// * 
	// * */

	//
	// Created by SharpDevelop.
	// User: simpsont
	// Date: 10/02/2006
	// Time: 10:25 AM
	// 
	// To change this template use Tools | Options | Coding | Edit Standard Headers.
	//
	partial class TreeControl : System.Windows.Forms.UserControl
	{

		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components;

		/// <summary>
		/// Disposes resources used by the control.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TreeControl));
			this.TreeView1 = new System.Windows.Forms.TreeView();
			this.wnIcons = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			//
			//TreeView1
			//
			this.TreeView1.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right);
			this.TreeView1.ImageIndex = 0;
			this.TreeView1.ImageList = this.wnIcons;
			this.TreeView1.Location = new System.Drawing.Point(0, 0);
			this.TreeView1.Name = "TreeView1";
			this.TreeView1.SelectedImageIndex = 0;
			this.TreeView1.ShowNodeToolTips = true;
			this.TreeView1.Size = new System.Drawing.Size(350, 381);
			this.TreeView1.TabIndex = 18;
			//
			//wnIcons
			//
			this.wnIcons.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("wnIcons.ImageStream");
			this.wnIcons.TransparentColor = System.Drawing.Color.Transparent;
			this.wnIcons.Images.SetKeyName(0, "");
			this.wnIcons.Images.SetKeyName(1, "");
			this.wnIcons.Images.SetKeyName(2, "MISC18.ICO");
			this.wnIcons.Images.SetKeyName(3, "BINOCULR.ICO");
			this.wnIcons.Images.SetKeyName(4, "MISC08.ICO");
			this.wnIcons.Images.SetKeyName(5, "MISC28.ICO");
			this.wnIcons.Images.SetKeyName(6, "");
			this.wnIcons.Images.SetKeyName(7, "WATER.ICO");
			this.wnIcons.Images.SetKeyName(8, "EAR.ICO");
			this.wnIcons.Images.SetKeyName(9, "MISC33.ICO");
			this.wnIcons.Images.SetKeyName(10, "MISC44.ICO");
			this.wnIcons.Images.SetKeyName(11, "SECUR06.ICO");
			this.wnIcons.Images.SetKeyName(12, "MOUSE01.ICO");
			this.wnIcons.Images.SetKeyName(13, "PC02.ICO");
			this.wnIcons.Images.SetKeyName(14, "KEY04.ICO");
			this.wnIcons.Images.SetKeyName(15, "MOUSE04.ICO");
			this.wnIcons.Images.SetKeyName(16, "DRAG3PG.ICO");
			this.wnIcons.Images.SetKeyName(17, "NET07.ICO");
			this.wnIcons.Images.SetKeyName(18, "RULERS.ICO");
			this.wnIcons.Images.SetKeyName(19, "POINT06.ICO");
			this.wnIcons.Images.SetKeyName(20, "POINT10.ICO");
			this.wnIcons.Images.SetKeyName(21, "HOUSE.ICO");
			this.wnIcons.Images.SetKeyName(22, "MAIL08.ICO");
			this.wnIcons.Images.SetKeyName(23, "MISC22.ICO");
			this.wnIcons.Images.SetKeyName(24, "GRAPH01.ICO");
			this.wnIcons.Images.SetKeyName(25, "CRDFLE04.ICO");
			this.wnIcons.Images.SetKeyName(26, "BULLSEYE.ICO");
			this.wnIcons.Images.SetKeyName(27, "MISC29.ICO");
			this.wnIcons.Images.SetKeyName(28, "MOUSE01.ICO");
			this.wnIcons.Images.SetKeyName(29, "PC02.ICO");
			this.wnIcons.Images.SetKeyName(30, "CRDFLE12.ICO");
			this.wnIcons.Images.SetKeyName(31, "GRAPH11.ICO");
			this.wnIcons.Images.SetKeyName(32, "CLIP07.ICO");
			this.wnIcons.Images.SetKeyName(33, "PLANE.ICO");
			this.wnIcons.Images.SetKeyName(34, "CLOUD.ICO");
			this.wnIcons.Images.SetKeyName(35, "FOLDER04.ICO");
			this.wnIcons.Images.SetKeyName(36, "SECUR05.ICO");
			this.wnIcons.Images.SetKeyName(37, "BOOK02.ICO");
			this.wnIcons.Images.SetKeyName(38, "");
			//
			//TreeControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.TreeView1);
			this.Name = "TreeControl";
			this.Size = new System.Drawing.Size(350, 381);
			this.ResumeLayout(false);

		}
		internal System.Windows.Forms.ImageList wnIcons;
		internal System.Windows.Forms.TreeView TreeView1;

	}
}
