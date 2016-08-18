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
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Collections;
using System.Data;
using System.Drawing;
using System.IO;

namespace WordNet
{
    public partial class Wildcard : System.Windows.Forms.Form
    {
        internal System.Windows.Forms.RadioButton radScrabble;
        private System.Windows.Forms.StatusBar statusBar1;
        internal System.Windows.Forms.Button Button4;
        internal System.Windows.Forms.Button btnSearch;
        internal System.Windows.Forms.RadioButton radSoundsLike;
        internal System.Windows.Forms.RadioButton radWildcard;
        private System.Windows.Forms.CheckBox chkSearchScrabble;
        private System.Windows.Forms.DataGrid dataGrid1;
        internal System.Windows.Forms.GroupBox GroupBox1;
        internal System.Windows.Forms.Label Label1;
        private System.Windows.Forms.TextBox withEventsField_TextBox1;
        internal System.Windows.Forms.TextBox TextBox1;
        /*
        {
            get { return withEventsField_TextBox1; }
            set {
                if (withEventsField_TextBox1 != null) {
                    withEventsField_TextBox1.KeyDown -= TextBox1_KeyDown;
                }
                withEventsField_TextBox1 = value;
                if (withEventsField_TextBox1 != null) {
                    withEventsField_TextBox1.KeyDown += TextBox1_KeyDown;
                }
            }
        }
        */
        internal System.Windows.Forms.RadioButton radAnagram;
        private System.Data.DataSet dataSet1;
        internal System.Windows.Forms.RadioButton radRegularExpression;
        private System.Windows.Forms.Button withEventsField_Button3;
        internal System.Windows.Forms.Button Button3;
        /*
        {
            get { return withEventsField_Button3; }
            set {
                if (withEventsField_Button3 != null) {
                    withEventsField_Button3.Click -= Button3_Click;
                }
                withEventsField_Button3 = value;
                if (withEventsField_Button3 != null) {
                    withEventsField_Button3.Click += Button3_Click;
                }
            }
        }
        */
        internal System.Windows.Forms.Button Button2;

        private int nall = 0; // all word types
        private int nn = 0; // noun
        private int nv = 0; // verb
        private int na = 0; // adjective
        private int nr = 0; //
        public wnb.StartForm Caller;
        private SaveFileDialog saveFileDialog1;
        private string dictPath;
        #region " Windows Form Designer generated code "

        public Wildcard() : base()
        {

            //This call is required by the Windows Form Designer.
            InitializeComponent();

            //Add any initialization after the InitializeComponent() call
            dataGrid1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
        }

        //Form overrides dispose to clean up the component list.
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if ((components != null))
                {
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
        [System.Diagnostics.DebuggerStepThrough()]
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Wildcard));
            this.Button2 = new System.Windows.Forms.Button();
            this.Button3 = new System.Windows.Forms.Button();
            this.radRegularExpression = new System.Windows.Forms.RadioButton();
            this.dataSet1 = new System.Data.DataSet();
            this.radAnagram = new System.Windows.Forms.RadioButton();
            this.TextBox1 = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.radSoundsLike = new System.Windows.Forms.RadioButton();
            this.radScrabble = new System.Windows.Forms.RadioButton();
            this.radWildcard = new System.Windows.Forms.RadioButton();
            this.dataGrid1 = new System.Windows.Forms.DataGrid();
            this.chkSearchScrabble = new System.Windows.Forms.CheckBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.Button4 = new System.Windows.Forms.Button();
            this.statusBar1 = new System.Windows.Forms.StatusBar();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).BeginInit();
            this.GroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
            this.SuspendLayout();
            // 
            // Button2
            // 
            this.Button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Button2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Button2.Location = new System.Drawing.Point(352, 32);
            this.Button2.Name = "Button2";
            this.Button2.Size = new System.Drawing.Size(75, 23);
            this.Button2.TabIndex = 5;
            this.Button2.Text = "Save";
            this.Button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // Button3
            // 
            this.Button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Button3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Button3.Location = new System.Drawing.Point(432, 32);
            this.Button3.Name = "Button3";
            this.Button3.Size = new System.Drawing.Size(75, 23);
            this.Button3.TabIndex = 6;
            this.Button3.Text = "OK";
            this.Button3.Click += new System.EventHandler(this.Button3_Click);
            // 
            // radRegularExpression
            // 
            this.radRegularExpression.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.radRegularExpression.Location = new System.Drawing.Point(88, 16);
            this.radRegularExpression.Name = "radRegularExpression";
            this.radRegularExpression.Size = new System.Drawing.Size(112, 24);
            this.radRegularExpression.TabIndex = 1;
            this.radRegularExpression.Text = "Regular Expression";
            // 
            // dataSet1
            // 
            this.dataSet1.DataSetName = "NewDataSet";
            this.dataSet1.Locale = new System.Globalization.CultureInfo("en-AU");
            // 
            // radAnagram
            // 
            this.radAnagram.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.radAnagram.Location = new System.Drawing.Point(208, 16);
            this.radAnagram.Name = "radAnagram";
            this.radAnagram.Size = new System.Drawing.Size(96, 24);
            this.radAnagram.TabIndex = 2;
            this.radAnagram.Text = "Anagram";
            // 
            // TextBox1
            // 
            this.TextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBox1.Location = new System.Drawing.Point(56, 8);
            this.TextBox1.Name = "TextBox1";
            this.TextBox1.Size = new System.Drawing.Size(368, 20);
            this.TextBox1.TabIndex = 1;
            this.TextBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyDown);
            // 
            // Label1
            // 
            this.Label1.Location = new System.Drawing.Point(8, 8);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(64, 23);
            this.Label1.TabIndex = 0;
            this.Label1.Text = "Search:";
            // 
            // GroupBox1
            // 
            this.GroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupBox1.Controls.Add(this.radSoundsLike);
            this.GroupBox1.Controls.Add(this.radScrabble);
            this.GroupBox1.Controls.Add(this.radAnagram);
            this.GroupBox1.Controls.Add(this.radRegularExpression);
            this.GroupBox1.Controls.Add(this.radWildcard);
            this.GroupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.GroupBox1.Location = new System.Drawing.Point(0, 56);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(512, 48);
            this.GroupBox1.TabIndex = 7;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Select Search Type:";
            // 
            // radSoundsLike
            // 
            this.radSoundsLike.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.radSoundsLike.Location = new System.Drawing.Point(384, 16);
            this.radSoundsLike.Name = "radSoundsLike";
            this.radSoundsLike.Size = new System.Drawing.Size(80, 24);
            this.radSoundsLike.TabIndex = 4;
            this.radSoundsLike.Text = "Sounds Like";
            // 
            // radScrabble
            // 
            this.radScrabble.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.radScrabble.Location = new System.Drawing.Point(288, 16);
            this.radScrabble.Name = "radScrabble";
            this.radScrabble.Size = new System.Drawing.Size(88, 24);
            this.radScrabble.TabIndex = 3;
            this.radScrabble.Text = "Scrabble";
            // 
            // radWildcard
            // 
            this.radWildcard.Checked = true;
            this.radWildcard.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.radWildcard.Location = new System.Drawing.Point(8, 16);
            this.radWildcard.Name = "radWildcard";
            this.radWildcard.Size = new System.Drawing.Size(104, 24);
            this.radWildcard.TabIndex = 0;
            this.radWildcard.TabStop = true;
            this.radWildcard.Text = "Wildcard";
            // 
            // dataGrid1
            // 
            this.dataGrid1.AccessibleName = "DataGrid";
            this.dataGrid1.AccessibleRole = System.Windows.Forms.AccessibleRole.Table;
            this.dataGrid1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGrid1.DataMember = "";
            this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dataGrid1.Location = new System.Drawing.Point(0, 104);
            this.dataGrid1.Name = "dataGrid1";
            this.dataGrid1.Size = new System.Drawing.Size(512, 240);
            this.dataGrid1.TabIndex = 11;
            this.dataGrid1.CurrentCellChanged += new System.EventHandler(this.DataGrid1CurrentCellChanged);
            this.dataGrid1.Resize += new System.EventHandler(this.DataGrid1Resize);
            // 
            // chkSearchScrabble
            // 
            this.chkSearchScrabble.Location = new System.Drawing.Point(123, 31);
            this.chkSearchScrabble.Name = "chkSearchScrabble";
            this.chkSearchScrabble.Size = new System.Drawing.Size(223, 24);
            this.chkSearchScrabble.TabIndex = 8;
            this.chkSearchScrabble.Text = "Scrabble Allowed Words (With Score)";
            this.chkSearchScrabble.CheckedChanged += new System.EventHandler(this.ChkSearchScrabbleCheckedChanged);
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSearch.Location = new System.Drawing.Point(432, 8);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.Text = "Search";
            this.btnSearch.Click += new System.EventHandler(this.btnSearchClick);
            // 
            // Button4
            // 
            this.Button4.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Button4.Location = new System.Drawing.Point(8, 32);
            this.Button4.Name = "Button4";
            this.Button4.Size = new System.Drawing.Size(92, 23);
            this.Button4.TabIndex = 2;
            this.Button4.Text = "Part of Speech";
            this.Button4.Visible = false;
            this.Button4.Click += new System.EventHandler(this.Button4Click);
            // 
            // statusBar1
            // 
            this.statusBar1.Location = new System.Drawing.Point(0, 344);
            this.statusBar1.Name = "statusBar1";
            this.statusBar1.Size = new System.Drawing.Size(512, 22);
            this.statusBar1.TabIndex = 10;
            this.statusBar1.Text = "WordNetDT Advanced Search";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "Text files (*.txt)|*.txt";
            // 
            // Wildcard
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(512, 366);
            this.Controls.Add(this.dataGrid1);
            this.Controls.Add(this.statusBar1);
            this.Controls.Add(this.chkSearchScrabble);
            this.Controls.Add(this.GroupBox1);
            this.Controls.Add(this.Button3);
            this.Controls.Add(this.Button2);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.Button4);
            this.Controls.Add(this.TextBox1);
            this.Controls.Add(this.Label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Wildcard";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Advanced Search";
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).EndInit();
            this.GroupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

    }
}
