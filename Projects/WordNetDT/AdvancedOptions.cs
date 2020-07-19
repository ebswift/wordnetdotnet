/*
 * Copyright (C) 2005 Malcolm Crowe, Troy Simpson 
 * 
 * Project Home: http://www.ebswift.com
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
using System.Linq;
using Wnlib;
using System.Windows.Forms;
namespace WordNet
{

	public class AdvancedOptions : System.Windows.Forms.Form
	{

		#region " Windows Form Designer generated code "

		public AdvancedOptions() : base()
		{
			Validated += AdvancedOptions_Validated;
			Load += AdvancedOptions_Load;

			//This call is required by the Windows Form Designer.
			InitializeComponent();

			//Add any initialization after the InitializeComponent() call
			RBInit("-a", radioAoff, radioAon, radioABoth);
			RBInit("-o", radioOoff, radioOon, radioOBoth);
			RBInit("-s", radioSoff, radioSon, radioSBoth);
		}

		//Form overrides dispose to clean up the component list.
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if ((components != null)) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		//Required by the Windows Form Designer

		private System.ComponentModel.IContainer components;
		//NOTE: The following procedure is required by the Windows Form Designer
		//It can be modified using the Windows Form Designer.  
		//Do not modify it using the code editor.
		private System.Windows.Forms.Button withEventsField_button1;
        internal System.Windows.Forms.Button button1;
/*
        {
			get { return withEventsField_button1; }
			set {
				if (withEventsField_button1 != null) {
					withEventsField_button1.Click -= button1_Click;
				}
				withEventsField_button1 = value;
				if (withEventsField_button1 != null) {
					withEventsField_button1.Click += button1_Click;
				}
			}
		}
        */
		internal System.Windows.Forms.GroupBox groupBox3;
		internal System.Windows.Forms.RadioButton radioSBoth;
		internal System.Windows.Forms.RadioButton radioSon;
		internal System.Windows.Forms.RadioButton radioSoff;
		internal System.Windows.Forms.GroupBox groupBox2;
		internal System.Windows.Forms.RadioButton radioOBoth;
		internal System.Windows.Forms.RadioButton radioOon;
		internal System.Windows.Forms.RadioButton radioOoff;
		internal System.Windows.Forms.GroupBox groupBox1;
		internal System.Windows.Forms.RadioButton radioABoth;
		internal System.Windows.Forms.RadioButton radioAon;
		private System.Windows.Forms.RadioButton withEventsField_radioAoff;
        internal System.Windows.Forms.RadioButton radioAoff;
        /*
        {
			get { return withEventsField_radioAoff; }
			set {
				if (withEventsField_radioAoff != null) {
					withEventsField_radioAoff.CheckedChanged -= radioAoff_CheckedChanged;
				}
				withEventsField_radioAoff = value;
				if (withEventsField_radioAoff != null) {
					withEventsField_radioAoff.CheckedChanged += radioAoff_CheckedChanged;
				}
			}
		}
        */
		[System.Diagnostics.DebuggerStepThrough()]
		private void InitializeComponent()
		{
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioSBoth = new System.Windows.Forms.RadioButton();
            this.radioSon = new System.Windows.Forms.RadioButton();
            this.radioSoff = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioOBoth = new System.Windows.Forms.RadioButton();
            this.radioOon = new System.Windows.Forms.RadioButton();
            this.radioOoff = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioABoth = new System.Windows.Forms.RadioButton();
            this.radioAon = new System.Windows.Forms.RadioButton();
            this.radioAoff = new System.Windows.Forms.RadioButton();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button1.Location = new System.Drawing.Point(96, 240);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(64, 24);
            this.button1.TabIndex = 11;
            this.button1.Text = "OK";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioSBoth);
            this.groupBox3.Controls.Add(this.radioSon);
            this.groupBox3.Controls.Add(this.radioSoff);
            this.groupBox3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox3.Location = new System.Drawing.Point(8, 152);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(232, 72);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Tag = "-s";
            this.groupBox3.Text = "Sense number";
            // 
            // radioSBoth
            // 
            this.radioSBoth.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.radioSBoth.Location = new System.Drawing.Point(16, 48);
            this.radioSBoth.Name = "radioSBoth";
            this.radioSBoth.Size = new System.Drawing.Size(200, 16);
            this.radioSBoth.TabIndex = 6;
            this.radioSBoth.Tag = "ON";
            this.radioSBoth.Text = "Show with searches and overview";
            this.radioSBoth.CheckedChanged += new System.EventHandler(this.radioAoff_CheckedChanged);
            // 
            // radioSon
            // 
            this.radioSon.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.radioSon.Location = new System.Drawing.Point(16, 32);
            this.radioSon.Name = "radioSon";
            this.radioSon.Size = new System.Drawing.Size(128, 16);
            this.radioSon.TabIndex = 5;
            this.radioSon.Tag = "on";
            this.radioSon.Text = "Show with searches";
            this.radioSon.CheckedChanged += new System.EventHandler(this.radioAoff_CheckedChanged);
            // 
            // radioSoff
            // 
            this.radioSoff.Checked = true;
            this.radioSoff.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.radioSoff.Location = new System.Drawing.Point(16, 16);
            this.radioSoff.Name = "radioSoff";
            this.radioSoff.Size = new System.Drawing.Size(112, 16);
            this.radioSoff.TabIndex = 4;
            this.radioSoff.TabStop = true;
            this.radioSoff.Tag = "off";
            this.radioSoff.Text = "Don\'t show";
            this.radioSoff.CheckedChanged += new System.EventHandler(this.radioAoff_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioOBoth);
            this.groupBox2.Controls.Add(this.radioOon);
            this.groupBox2.Controls.Add(this.radioOoff);
            this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox2.Location = new System.Drawing.Point(8, 80);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(232, 72);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Tag = "-o";
            this.groupBox2.Text = "Synset location in database file";
            // 
            // radioOBoth
            // 
            this.radioOBoth.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.radioOBoth.Location = new System.Drawing.Point(16, 48);
            this.radioOBoth.Name = "radioOBoth";
            this.radioOBoth.Size = new System.Drawing.Size(200, 16);
            this.radioOBoth.TabIndex = 6;
            this.radioOBoth.Tag = "ON";
            this.radioOBoth.Text = "Show with searches and overview";
            this.radioOBoth.CheckedChanged += new System.EventHandler(this.radioAoff_CheckedChanged);
            // 
            // radioOon
            // 
            this.radioOon.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.radioOon.Location = new System.Drawing.Point(16, 32);
            this.radioOon.Name = "radioOon";
            this.radioOon.Size = new System.Drawing.Size(128, 16);
            this.radioOon.TabIndex = 5;
            this.radioOon.Tag = "on";
            this.radioOon.Text = "Show with searches";
            this.radioOon.CheckedChanged += new System.EventHandler(this.radioAoff_CheckedChanged);
            // 
            // radioOoff
            // 
            this.radioOoff.Checked = true;
            this.radioOoff.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.radioOoff.Location = new System.Drawing.Point(16, 16);
            this.radioOoff.Name = "radioOoff";
            this.radioOoff.Size = new System.Drawing.Size(112, 16);
            this.radioOoff.TabIndex = 4;
            this.radioOoff.TabStop = true;
            this.radioOoff.Tag = "off";
            this.radioOoff.Text = "Don\'t show";
            this.radioOoff.CheckedChanged += new System.EventHandler(this.radioAoff_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioABoth);
            this.groupBox1.Controls.Add(this.radioAon);
            this.groupBox1.Controls.Add(this.radioAoff);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox1.Location = new System.Drawing.Point(8, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(232, 72);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Tag = "-a";
            this.groupBox1.Text = "Lexical file information";
            // 
            // radioABoth
            // 
            this.radioABoth.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.radioABoth.Location = new System.Drawing.Point(16, 48);
            this.radioABoth.Name = "radioABoth";
            this.radioABoth.Size = new System.Drawing.Size(200, 16);
            this.radioABoth.TabIndex = 6;
            this.radioABoth.Tag = "ON";
            this.radioABoth.Text = "Show with searches and overview";
            this.radioABoth.CheckedChanged += new System.EventHandler(this.radioAoff_CheckedChanged);
            // 
            // radioAon
            // 
            this.radioAon.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.radioAon.Location = new System.Drawing.Point(16, 32);
            this.radioAon.Name = "radioAon";
            this.radioAon.Size = new System.Drawing.Size(128, 16);
            this.radioAon.TabIndex = 5;
            this.radioAon.Tag = "on";
            this.radioAon.Text = "Show with searches";
            this.radioAon.CheckedChanged += new System.EventHandler(this.radioAoff_CheckedChanged);
            // 
            // radioAoff
            // 
            this.radioAoff.Checked = true;
            this.radioAoff.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.radioAoff.Location = new System.Drawing.Point(16, 16);
            this.radioAoff.Name = "radioAoff";
            this.radioAoff.Size = new System.Drawing.Size(112, 16);
            this.radioAoff.TabIndex = 4;
            this.radioAoff.TabStop = true;
            this.radioAoff.Tag = "off";
            this.radioAoff.Text = "Don\'t show";
            this.radioAoff.CheckedChanged += new System.EventHandler(this.radioAoff_CheckedChanged);
            // 
            // AdvancedOptions
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(248, 270);
            this.ControlBox = false;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AdvancedOptions";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Advanced Options";
            this.TopMost = true;
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private void RBInit(string t, System.Windows.Forms.RadioButton a, System.Windows.Forms.RadioButton b, System.Windows.Forms.RadioButton c)
		{
			bool v = WNOpt.opt(t).flag;
			bool V1 = WNOpt.opt(t.ToUpper()).flag;
			if ((v)) {
				if ((V1)) {
					c.Checked = true;
				} else {
					b.Checked = true;
				}
			} else if (!V1) {
				a.Checked = true;
			}
		}

		private void radioAoff_CheckedChanged(System.Object sender, System.EventArgs e)
		{
			System.Windows.Forms.RadioButton b = (System.Windows.Forms.RadioButton)sender;
			GroupBox g = (GroupBox)b.Parent;
			string t = g.Tag.ToString();
			switch (b.Tag.ToString()) {
				case "off":
					WNOpt.opt(t).flag = false;
					WNOpt.opt(t.ToUpper()).flag = false;

					break;
				case "on":
					WNOpt.opt(t).flag = true;
					WNOpt.opt(t.ToUpper()).flag = false;

					break;
				case "ON":
					WNOpt.opt(t).flag = true;
					WNOpt.opt(t.ToUpper()).flag = true;

					break;
			}
		}

		private void button1_Click(System.Object sender, System.EventArgs e)
		{
			this.Hide();
		}


		private void AdvancedOptions_Load(System.Object sender, System.EventArgs e)
		{
		}

		private void AdvancedOptions_Validated(object sender, System.EventArgs e)
		{
			Application.EnableVisualStyles();
		}
	}
}
