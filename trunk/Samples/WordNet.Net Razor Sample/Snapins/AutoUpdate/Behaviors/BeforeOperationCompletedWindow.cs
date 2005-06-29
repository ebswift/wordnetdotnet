using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Razor.SnapIns.AutoUpdate.Behaviors
{
	/// <summary>
	/// Summary description for BeforeOperationCompletedWindow.
	/// </summary>
	public class BeforeOperationCompletedWindow :  System.Windows.Forms.Form
	{
		protected int _heightCollapsed = 250;
		protected int _heightExpanded  = 450;		
		protected bool _expanded;
		protected bool _triggeredByButton;

		private Razor.InformationPanel _informationPanel;
		private System.Windows.Forms.CheckBox _checkBoxAuto;
		private System.Windows.Forms.Button _buttonOK;
		private System.Windows.Forms.RadioButton _radioButtonNo;
		private System.Windows.Forms.RadioButton _radioButtonYes;
		protected System.Windows.Forms.LinkLabel _link;
		private System.Windows.Forms.TabControl _tabControl;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.ListView _listView;
		private System.Windows.Forms.Button _buttonDetails;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.RichTextBox _textBox;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Initializes a new instance of the class
		/// </summary>
		public BeforeOperationCompletedWindow()
		{			
			this.InitializeComponent();
						
			this.MinimumSize = new Size(500, 250);
			this.StartPosition = FormStartPosition.CenterParent;
			this.TopMost = true;
						
			_radioButtonYes.CheckedChanged += new EventHandler(OnRadioButtonCheckChanged);
			_radioButtonNo.CheckedChanged += new EventHandler(OnRadioButtonCheckChanged);
			_link.Click += new EventHandler(OnLinkClicked);
			_buttonOK.Click += new EventHandler(OnButtonOKClicked);	
			_buttonDetails.Click += new EventHandler(OnButtonDetailsClicked);
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(BeforeOperationCompletedWindow));
			this._informationPanel = new Razor.InformationPanel();
			this._checkBoxAuto = new System.Windows.Forms.CheckBox();
			this._buttonOK = new System.Windows.Forms.Button();
			this._radioButtonNo = new System.Windows.Forms.RadioButton();
			this._radioButtonYes = new System.Windows.Forms.RadioButton();
			this._buttonDetails = new System.Windows.Forms.Button();
			this._link = new System.Windows.Forms.LinkLabel();
			this._tabControl = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this._listView = new System.Windows.Forms.ListView();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this._textBox = new System.Windows.Forms.RichTextBox();
			this._tabControl.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.SuspendLayout();
			// 
			// _informationPanel
			// 
			this._informationPanel.BackColor = System.Drawing.Color.White;
			this._informationPanel.Description = "";
			this._informationPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this._informationPanel.Image = ((System.Drawing.Image)(resources.GetObject("_informationPanel.Image")));
			this._informationPanel.Location = new System.Drawing.Point(0, 0);
			this._informationPanel.Name = "_informationPanel";
			this._informationPanel.Size = new System.Drawing.Size(492, 85);
			this._informationPanel.TabIndex = 0;
			this._informationPanel.Title = "";
			// 
			// _checkBoxAuto
			// 
			this._checkBoxAuto.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._checkBoxAuto.Location = new System.Drawing.Point(10, 185);
			this._checkBoxAuto.Name = "_checkBoxAuto";
			this._checkBoxAuto.Size = new System.Drawing.Size(393, 25);
			this._checkBoxAuto.TabIndex = 3;
			// 
			// _buttonOK
			// 
			this._buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._buttonOK.Location = new System.Drawing.Point(408, 150);
			this._buttonOK.Name = "_buttonOK";
			this._buttonOK.TabIndex = 5;
			this._buttonOK.Text = "OK";
			// 
			// _radioButtonNo
			// 
			this._radioButtonNo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._radioButtonNo.Location = new System.Drawing.Point(10, 120);
			this._radioButtonNo.Name = "_radioButtonNo";
			this._radioButtonNo.Size = new System.Drawing.Size(473, 20);
			this._radioButtonNo.TabIndex = 6;
			// 
			// _radioButtonYes
			// 
			this._radioButtonYes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._radioButtonYes.Checked = true;
			this._radioButtonYes.Location = new System.Drawing.Point(10, 95);
			this._radioButtonYes.Name = "_radioButtonYes";
			this._radioButtonYes.Size = new System.Drawing.Size(473, 20);
			this._radioButtonYes.TabIndex = 4;
			this._radioButtonYes.TabStop = true;
			// 
			// _buttonDetails
			// 
			this._buttonDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonDetails.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._buttonDetails.Location = new System.Drawing.Point(408, 185);
			this._buttonDetails.Name = "_buttonDetails";
			this._buttonDetails.TabIndex = 7;
			this._buttonDetails.Text = "Details >>";
			// 
			// _link
			// 
			this._link.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._link.Cursor = System.Windows.Forms.Cursors.Hand;
			this._link.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._link.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
			this._link.Location = new System.Drawing.Point(10, 155);
			this._link.Name = "_link";
			this._link.Size = new System.Drawing.Size(393, 25);
			this._link.TabIndex = 8;
			// 
			// _tabControl
			// 
			this._tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._tabControl.Controls.Add(this.tabPage1);
			this._tabControl.Controls.Add(this.tabPage2);
			this._tabControl.Location = new System.Drawing.Point(10, 220);
			this._tabControl.Name = "_tabControl";
			this._tabControl.SelectedIndex = 0;
			this._tabControl.Size = new System.Drawing.Size(473, 190);
			this._tabControl.TabIndex = 9;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this._listView);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(465, 164);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Summary";
			// 
			// _listView
			// 
			this._listView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._listView.Location = new System.Drawing.Point(0, 0);
			this._listView.Name = "_listView";
			this._listView.Size = new System.Drawing.Size(465, 164);
			this._listView.TabIndex = 0;
			this._listView.View = System.Windows.Forms.View.Details;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this._textBox);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Size = new System.Drawing.Size(465, 164);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Manifest Xml";
			// 
			// _textBox
			// 
			this._textBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this._textBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._textBox.Location = new System.Drawing.Point(0, 0);
			this._textBox.Name = "_textBox";
			this._textBox.ShowSelectionMargin = true;
			this._textBox.Size = new System.Drawing.Size(465, 164);
			this._textBox.TabIndex = 0;
			this._textBox.Text = "";
			this._textBox.WordWrap = false;
			// 
			// BeforeOperationCompletedWindow
			// 
			this.AcceptButton = this._buttonOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(492, 416);
			this.Controls.Add(this._tabControl);
			this.Controls.Add(this._link);
			this.Controls.Add(this._buttonDetails);
			this.Controls.Add(this._checkBoxAuto);
			this.Controls.Add(this._buttonOK);
			this.Controls.Add(this._radioButtonNo);
			this.Controls.Add(this._radioButtonYes);
			this.Controls.Add(this._informationPanel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "BeforeOperationCompletedWindow";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Auto-Update:";
			this._tabControl.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		
		#region My Event Handlers and Overrides

		/// <summary>
		/// Occurs when the window loads
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);

			this.EnableDetails(true);
			this.ShowDetails(false);
			
			// call the overrides to get the text/links to display
			_informationPanel.Title = this.GetOperationText();
			_informationPanel.Description = this.GetOperationDescriptionText();
			_radioButtonYes.Text = this.GetYesAnswerText();
			_radioButtonNo.Text = this.GetNoAnswerText();
			_checkBoxAuto.Text = this.GetAutoAnswerText();
			_link.Text = this.GetLinkText();	
		}

		/// <summary>
		/// Occurs when the window resizes
		/// </summary>
		/// <param name="e"></param>
		protected override void OnResize(EventArgs e)
		{
			base.OnResize (e);

			// if the window is expanded
			if (_expanded)
				// we must save the height of the window for the next expansion
				_heightExpanded = this.Height;		
		}

		/// <summary>
		/// Occurs when the window closes
		/// </summary>
		/// <param name="e"></param>
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing (e);

			if (!_triggeredByButton)
			{
				_radioButtonNo.Checked = true;
				// don't know if the event will fire on that or not
			}
		}

		/// <summary>
		/// Occurs when the answers yes/no change
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnRadioButtonCheckChanged(object sender, EventArgs e)
		{
			_checkBoxAuto.Enabled = _radioButtonYes.Checked;
		}

		/// <summary>
		/// Occurs when the link is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnLinkClicked(object sender, EventArgs e)
		{
			Process p = this.ExecuteUrl(this.GetLinkHref());
		}

		/// <summary>
		/// Occurs when the OK button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnButtonOKClicked(object sender, EventArgs e)
		{
			_triggeredByButton = true;
			this.Close();
		}

		/// <summary>
		/// Occurs when the details button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnButtonDetailsClicked(object sender, EventArgs e)
		{
			// toggle the state of the details window
			this.IsExpanded = !this.IsExpanded;
		}

		#endregion

		#region My Virtual Methods

		/// <summary>
		/// Returns the text displayed for the operation 
		/// </summary>
		/// <returns></returns>
		protected virtual string GetOperationText()
		{
			return string.Empty;
		}

		/// <summary>
		/// Returns the text displayed for the operation description
		/// </summary>
		/// <returns></returns>
		protected virtual string GetOperationDescriptionText()
		{
			return string.Empty;
		}

		/// <summary>
		/// Returns the text displayed for the yes answer
		/// </summary>
		/// <returns></returns>
		protected virtual string GetYesAnswerText()
		{
			return string.Empty;
		}

		/// <summary>
		/// Returns the text displayed for the no answer
		/// </summary>
		/// <returns></returns>
		protected virtual string GetNoAnswerText()
		{
			return string.Empty;
		}

		/// <summary>
		/// Returns the text displayed on the hyperlink
		/// </summary>
		/// <returns></returns>
		protected virtual string GetLinkText()
		{
			return string.Empty;
		}

		/// <summary>
		/// Returns the url that will be run when the hyperlink is clicked
		/// </summary>
		/// <returns></returns>
		protected virtual string GetLinkHref()
		{
			return string.Empty;
		}

		/// <summary>
		/// Returns the text displayed for the auto answer
		/// </summary>
		/// <returns></returns>
		protected virtual string GetAutoAnswerText()
		{
			return string.Empty;
		}

		#endregion

		#region My Public Methods & Properties

		/// <summary>
		/// Returns whether the operation should be cancelled 
		/// </summary>
		public bool Cancel
		{
			get
			{
				return _radioButtonNo.Checked;
			}
		}

		/// <summary>
		/// Gets or sets the auto variable response value
		/// </summary>
		public bool Auto
		{
			get
			{
				return _checkBoxAuto.Checked;
			}
			set
			{
				_checkBoxAuto.Checked = value;
			}
		}

		/// <summary>
		/// Shows/Hides the details on the window
		/// </summary>
		/// <param name="visible"></param>
		public virtual void ShowDetails(bool visible)
		{
			this.SuspendLayout();			

			if (!visible)
			{
				// toggle the expanded flag before resizing
				_expanded = visible;

				// collapse
				this.Height = _heightCollapsed;	
				this.MaximumSize = new Size(int.MaxValue, _heightCollapsed);
				this.MinimumSize = new Size(500, _heightCollapsed);

				_buttonDetails.Text = @"Details >>>";
			}
			else
			{
				// expand
				this.MaximumSize = new Size(int.MaxValue, int.MaxValue);
				this.MinimumSize = new Size(500, _heightCollapsed + 100);
				this.Height = _heightExpanded;															

				// toggle the expanded flag after resizing
				_expanded = visible;

				_buttonDetails.Text = @"Details <<<";
			}
			
			this.ResumeLayout(true);					
		}

		/// <summary>
		/// Enables/Disables whether the window can show more details or not
		/// </summary>
		/// <param name="enabled"></param>
		public virtual void EnableDetails(bool enabled)
		{
			// if details are not enabled
			if (!enabled)
			{
				// make sure the window isn't showing them, so collapse it, and disable the button 
				this.IsExpanded = false;
				this._buttonDetails.Enabled = false;				
				this.FormBorderStyle = FormBorderStyle.FixedDialog;
				this.SizeGripStyle = SizeGripStyle.Hide;
			}
			else
			{
				// otherwise the details button is enabled
				this._buttonDetails.Enabled = true;
				this.FormBorderStyle = FormBorderStyle.Sizable;
				this.SizeGripStyle = SizeGripStyle.Show;
			}
		}

		/// <summary>
		/// Gets or sets the expanded state of the window (Aka, whether the details are visible)
		/// </summary>
		public virtual bool IsExpanded
		{
			get
			{
				return _expanded;
			}
			set
			{
				this.ShowDetails(value);
			}
		}

		/// <summary>
		/// Returns the tab control displayed in detail view
		/// </summary>
		public TabControl TabControl
		{
			get
			{
				return _tabControl;
			}
		}

		/// <summary>
		/// Returns the listview displayed in detail view
		/// </summary>
		public ListView ListView
		{
			get
			{
				return _listView;
			}
		}

		/// <summary>
		/// Returns the listview displayed in detail view
		/// </summary>
		public RichTextBox TextBox
		{
			get
			{
				return _textBox;
			}
		}		

		/// <summary>
		/// Starts a process defined by the specified url
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public Process ExecuteUrl(string url)
		{
			try
			{
				return Process.Start(url);				
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
				MessageBox.Show(string.Format("An exception was encountered trying to follow the url specified.\n\n\tThe url was '{0}'.\n\n\tThe exception was '{1}'.", url, ex.ToString()), "Exception Encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			return null;
		}

		#endregion
	}
}
