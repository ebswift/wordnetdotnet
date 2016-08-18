using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using Microsoft.VisualBasic;
//Imports Wnlib
using WordNetClasses;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace WordNet.wnb
{
    public partial class StartForm : System.Windows.Forms.Form
    {
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.MenuItem MenuItem3;
        private System.Windows.Forms.MenuItem MenuItem2;
        private System.Windows.Forms.MenuItem MenuItem7;
        internal System.Windows.Forms.Label Label4;
        private System.Windows.Forms.Button Button3;
        private System.Windows.Forms.Button Button2;
        private System.Windows.Forms.Button Button1;
        private System.Windows.Forms.Button Button5;
        private System.Windows.Forms.Button Button4;
        internal System.Windows.Forms.MenuItem MenuItem17;
        internal System.Windows.Forms.MenuItem mnuHistory;
        internal System.Windows.Forms.StatusBar StatusBar1;
        internal System.Windows.Forms.MenuItem MenuItem4;
        internal System.Windows.Forms.Label Label1;
        private System.Windows.Forms.MenuItem MenuItem12;
        private System.Windows.Forms.MenuItem MenuItem13;
        private System.Windows.Forms.MenuItem MenuItem10;
        private System.Windows.Forms.MenuItem MenuItem11;
        private System.Windows.Forms.MenuItem MenuItem16;
        internal System.Windows.Forms.MenuItem MenuItem1;
        private System.Windows.Forms.MenuItem MenuItem14;
        private System.Windows.Forms.MenuItem MenuItem15;
        internal System.Windows.Forms.MenuItem MenuItem6;
        private System.Windows.Forms.MenuItem MenuItem5;
        private System.Windows.Forms.MenuItem MenuItem18;
        private System.Windows.Forms.Button btnAdvanced;
        private System.Windows.Forms.MenuItem MenuItem9;
        private System.Windows.Forms.MenuItem MenuItem8;
        internal System.Windows.Forms.TextBox TextBox1;
        internal System.Windows.Forms.TextBox TextBox2;
        internal System.Windows.Forms.SaveFileDialog SaveFileDialog1;
        internal System.Windows.Forms.MainMenu MainMenu1;
        private System.Windows.Forms.Button btnSearch;
        //WordNetClasses.WN wnc = new WordNetClasses.WN();
        //= New AdvancedOptions
        AdvancedOptions f3;
        ArrayList history = new ArrayList();
        object pbobject = new object();
        public int maxhistory = 10;
        internal System.Windows.Forms.WebBrowser HtmlViewer1;
        // html without word wrap escaping table
        public string rawFeedback;

        #region " Windows Form Designer generated code "

        public StartForm() : base()
        {
            Closing += StartForm_Closing;
            Load += Form1_Load;

            //This call is required by the Windows Form Designer.
            InitializeComponent();
            HtmlViewer1.Navigate("about:blank");
            //HtmlViewer1.Document.OpenNew(false);
            //TODO: re-implement silent
            //HtmlViewer1.Silent = True
            //Add any initialization after the InitializeComponent() call
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
        //[System.Diagnostics.DebuggerStepThrough()]
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StartForm));
            this.btnSearch = new System.Windows.Forms.Button();
            this.MainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.MenuItem1 = new System.Windows.Forms.MenuItem();
            this.MenuItem2 = new System.Windows.Forms.MenuItem();
            this.MenuItem6 = new System.Windows.Forms.MenuItem();
            this.MenuItem7 = new System.Windows.Forms.MenuItem();
            this.MenuItem8 = new System.Windows.Forms.MenuItem();
            this.MenuItem9 = new System.Windows.Forms.MenuItem();
            this.mnuHistory = new System.Windows.Forms.MenuItem();
            this.MenuItem3 = new System.Windows.Forms.MenuItem();
            this.MenuItem13 = new System.Windows.Forms.MenuItem();
            this.MenuItem10 = new System.Windows.Forms.MenuItem();
            this.MenuItem11 = new System.Windows.Forms.MenuItem();
            this.MenuItem12 = new System.Windows.Forms.MenuItem();
            this.MenuItem4 = new System.Windows.Forms.MenuItem();
            this.MenuItem5 = new System.Windows.Forms.MenuItem();
            this.MenuItem14 = new System.Windows.Forms.MenuItem();
            this.MenuItem15 = new System.Windows.Forms.MenuItem();
            this.MenuItem16 = new System.Windows.Forms.MenuItem();
            this.MenuItem17 = new System.Windows.Forms.MenuItem();
            this.MenuItem18 = new System.Windows.Forms.MenuItem();
            this.SaveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.TextBox2 = new System.Windows.Forms.TextBox();
            this.TextBox1 = new System.Windows.Forms.TextBox();
            this.btnAdvanced = new System.Windows.Forms.Button();
            this.Label1 = new System.Windows.Forms.Label();
            this.StatusBar1 = new System.Windows.Forms.StatusBar();
            this.Button4 = new System.Windows.Forms.Button();
            this.Button5 = new System.Windows.Forms.Button();
            this.Button1 = new System.Windows.Forms.Button();
            this.Button2 = new System.Windows.Forms.Button();
            this.Button3 = new System.Windows.Forms.Button();
            this.Label4 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.HtmlViewer1 = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSearch.Location = new System.Drawing.Point(304, 8);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(56, 23);
            this.btnSearch.TabIndex = 13;
            this.btnSearch.Text = "Search";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // MainMenu1
            // 
            this.MainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.MenuItem1,
            this.mnuHistory,
            this.MenuItem3,
            this.MenuItem4});
            // 
            // MenuItem1
            // 
            this.MenuItem1.Index = 0;
            this.MenuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.MenuItem2,
            this.MenuItem6,
            this.MenuItem7,
            this.MenuItem8,
            this.MenuItem9});
            this.MenuItem1.Text = "File";
            // 
            // MenuItem2
            // 
            this.MenuItem2.Index = 0;
            this.MenuItem2.Text = "Advanced Search";
            this.MenuItem2.Click += new System.EventHandler(this.MenuItem2_Click);
            // 
            // MenuItem6
            // 
            this.MenuItem6.Index = 1;
            this.MenuItem6.Text = "-";
            // 
            // MenuItem7
            // 
            this.MenuItem7.Index = 2;
            this.MenuItem7.Text = "Save Current Display";
            this.MenuItem7.Click += new System.EventHandler(this.MenuItem7_Click);
            // 
            // MenuItem8
            // 
            this.MenuItem8.Index = 3;
            this.MenuItem8.Text = "Clear Current Display";
            this.MenuItem8.Click += new System.EventHandler(this.MenuItem8_Click);
            // 
            // MenuItem9
            // 
            this.MenuItem9.Index = 4;
            this.MenuItem9.Text = "Exit";
            this.MenuItem9.Click += new System.EventHandler(this.MenuItem9_Click);
            // 
            // mnuHistory
            // 
            this.mnuHistory.Index = 1;
            this.mnuHistory.Text = "History";
            // 
            // MenuItem3
            // 
            this.MenuItem3.Index = 2;
            this.MenuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.MenuItem13,
            this.MenuItem10,
            this.MenuItem11,
            this.MenuItem12});
            this.MenuItem3.Text = "Options";
            // 
            // MenuItem13
            // 
            this.MenuItem13.Checked = true;
            this.MenuItem13.Index = 0;
            this.MenuItem13.Text = "Word Wrap";
            this.MenuItem13.Click += new System.EventHandler(this.MenuItem13_Click);
            // 
            // MenuItem10
            // 
            this.MenuItem10.Index = 1;
            this.MenuItem10.Text = "Show help with each search";
            this.MenuItem10.Click += new System.EventHandler(this.MenuItem10_Click);
            // 
            // MenuItem11
            // 
            this.MenuItem11.Index = 2;
            this.MenuItem11.Text = "Show descriptive gloss";
            this.MenuItem11.Click += new System.EventHandler(this.MenuItem11_Click);
            // 
            // MenuItem12
            // 
            this.MenuItem12.Index = 3;
            this.MenuItem12.Text = "Advanced search options";
            this.MenuItem12.Click += new System.EventHandler(this.MenuItem12_Click);
            // 
            // MenuItem4
            // 
            this.MenuItem4.Index = 3;
            this.MenuItem4.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.MenuItem5,
            this.MenuItem14,
            this.MenuItem15,
            this.MenuItem16,
            this.MenuItem17,
            this.MenuItem18});
            this.MenuItem4.Text = "Help";
            // 
            // MenuItem5
            // 
            this.MenuItem5.Index = 0;
            this.MenuItem5.Text = "Help on using WordNet browser";
            this.MenuItem5.Click += new System.EventHandler(this.MenuItem5_Click);
            // 
            // MenuItem14
            // 
            this.MenuItem14.Index = 1;
            this.MenuItem14.Text = "Help on WordNet terminology";
            this.MenuItem14.Click += new System.EventHandler(this.MenuItem14_Click);
            // 
            // MenuItem15
            // 
            this.MenuItem15.Index = 2;
            this.MenuItem15.Text = "WordNet License (Princeton)";
            this.MenuItem15.Click += new System.EventHandler(this.MenuItem15_Click);
            // 
            // MenuItem16
            // 
            this.MenuItem16.Index = 3;
            this.MenuItem16.Text = "WordNetDT License (ebswift.com)";
            this.MenuItem16.Click += new System.EventHandler(this.MenuItem16_Click);
            // 
            // MenuItem17
            // 
            this.MenuItem17.Index = 4;
            this.MenuItem17.Text = "-";
            // 
            // MenuItem18
            // 
            this.MenuItem18.Index = 5;
            this.MenuItem18.Text = "About WordNet browser";
            this.MenuItem18.Click += new System.EventHandler(this.MenuItem18_Click);
            // 
            // SaveFileDialog1
            // 
            this.SaveFileDialog1.Filter = "Text files (*.txt)|*.txt";
            // 
            // TextBox2
            // 
            this.TextBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBox2.Location = new System.Drawing.Point(544, 8);
            this.TextBox2.Name = "TextBox2";
            this.TextBox2.Size = new System.Drawing.Size(40, 20);
            this.TextBox2.TabIndex = 8;
            this.TextBox2.Text = "0";
            // 
            // TextBox1
            // 
            this.TextBox1.AcceptsReturn = true;
            this.TextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBox1.Location = new System.Drawing.Point(80, 8);
            this.TextBox1.Name = "TextBox1";
            this.TextBox1.Size = new System.Drawing.Size(216, 20);
            this.TextBox1.TabIndex = 1;
            this.TextBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyDown);
            // 
            // btnAdvanced
            // 
            this.btnAdvanced.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdvanced.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnAdvanced.Location = new System.Drawing.Point(368, 8);
            this.btnAdvanced.Name = "btnAdvanced";
            this.btnAdvanced.Size = new System.Drawing.Size(88, 23);
            this.btnAdvanced.TabIndex = 18;
            this.btnAdvanced.Text = "Advanced";
            this.btnAdvanced.Click += new System.EventHandler(this.BtnAdvancedClick);
            // 
            // Label1
            // 
            this.Label1.Location = new System.Drawing.Point(0, 8);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(80, 23);
            this.Label1.TabIndex = 0;
            this.Label1.Text = "Search Word:";
            // 
            // StatusBar1
            // 
            this.StatusBar1.Location = new System.Drawing.Point(0, 464);
            this.StatusBar1.Name = "StatusBar1";
            this.StatusBar1.Size = new System.Drawing.Size(592, 22);
            this.StatusBar1.TabIndex = 9;
            this.StatusBar1.Text = "WordNetDT";
            // 
            // Button4
            // 
            this.Button4.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Button4.Location = new System.Drawing.Point(400, 32);
            this.Button4.Name = "Button4";
            this.Button4.Size = new System.Drawing.Size(64, 23);
            this.Button4.TabIndex = 5;
            this.Button4.Text = "Adjective";
            this.Button4.Visible = false;
            this.Button4.Click += new System.EventHandler(this.Button4Click);
            // 
            // Button5
            // 
            this.Button5.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Button5.Location = new System.Drawing.Point(472, 32);
            this.Button5.Name = "Button5";
            this.Button5.Size = new System.Drawing.Size(48, 23);
            this.Button5.TabIndex = 6;
            this.Button5.Text = "Adverb";
            this.Button5.Visible = false;
            this.Button5.Click += new System.EventHandler(this.Button5Click);
            // 
            // Button1
            // 
            this.Button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Button1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Button1.Location = new System.Drawing.Point(224, 32);
            this.Button1.Name = "Button1";
            this.Button1.Size = new System.Drawing.Size(75, 23);
            this.Button1.TabIndex = 15;
            this.Button1.Text = "Overview";
            this.Button1.Visible = false;
            this.Button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // Button2
            // 
            this.Button2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Button2.Location = new System.Drawing.Point(304, 32);
            this.Button2.Name = "Button2";
            this.Button2.Size = new System.Drawing.Size(40, 23);
            this.Button2.TabIndex = 3;
            this.Button2.Text = "Noun";
            this.Button2.Visible = false;
            this.Button2.Click += new System.EventHandler(this.Button2Click);
            // 
            // Button3
            // 
            this.Button3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Button3.Location = new System.Drawing.Point(352, 32);
            this.Button3.Name = "Button3";
            this.Button3.Size = new System.Drawing.Size(40, 23);
            this.Button3.TabIndex = 4;
            this.Button3.Text = "Verb";
            this.Button3.Visible = false;
            this.Button3.Click += new System.EventHandler(this.Button3Click);
            // 
            // Label4
            // 
            this.Label4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label4.Location = new System.Drawing.Point(0, 56);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(592, 408);
            this.Label4.TabIndex = 17;
            this.Label4.Text = "Searching, Please Wait";
            this.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label2
            // 
            this.Label2.Location = new System.Drawing.Point(0, 32);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(296, 23);
            this.Label2.TabIndex = 2;
            // 
            // Label3
            // 
            this.Label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Label3.Location = new System.Drawing.Point(480, 8);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(100, 23);
            this.Label3.TabIndex = 7;
            this.Label3.Text = "Senses:";
            // 
            // HtmlViewer1
            // 
            this.HtmlViewer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.HtmlViewer1.Location = new System.Drawing.Point(4, 61);
            this.HtmlViewer1.MinimumSize = new System.Drawing.Size(20, 20);
            this.HtmlViewer1.Name = "HtmlViewer1";
            this.HtmlViewer1.Size = new System.Drawing.Size(588, 403);
            this.HtmlViewer1.TabIndex = 19;
            this.HtmlViewer1.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.HtmlViewer1_BeforeNavigate2);
            // 
            // StartForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(592, 486);
            this.Controls.Add(this.HtmlViewer1);
            this.Controls.Add(this.btnAdvanced);
            this.Controls.Add(this.Button1);
            this.Controls.Add(this.TextBox2);
            this.Controls.Add(this.TextBox1);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.StatusBar1);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.Button5);
            this.Controls.Add(this.Button4);
            this.Controls.Add(this.Button3);
            this.Controls.Add(this.Button2);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.Label4);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Menu = this.MainMenu1;
            this.Name = "StartForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WordNetDT";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
