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
	//
	// Created by SharpDevelop.
	// User: Troy
	// Date: 4/07/2006
	// Time: 7:21 AM
	// 
	// To change this template use Tools | Options | Coding | Edit Standard Headers.
	//
	partial class WordNetColourFormat : System.Windows.Forms.UserControl
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
			this.Canvas = new System.Windows.Forms.WebBrowser();
			this.SuspendLayout();
			//
			//Canvas
			//
			this.Canvas.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Canvas.Location = new System.Drawing.Point(0, 0);
			this.Canvas.MinimumSize = new System.Drawing.Size(20, 20);
			this.Canvas.Name = "Canvas";
			this.Canvas.Size = new System.Drawing.Size(405, 347);
			this.Canvas.TabIndex = 0;
			//
			//WordNetColourFormat
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.Canvas);
			this.Name = "WordNetColourFormat";
			this.Size = new System.Drawing.Size(405, 347);
			this.ResumeLayout(false);

		}
		public System.Windows.Forms.WebBrowser Canvas;
	}
}
