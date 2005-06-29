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
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace Razor
{
	/// <summary>
	/// Summary description for CancelMultipleProgressWindow.
	/// </summary>
	public class CancelMultipleProgressWindow : System.Windows.Forms.Form, IProgressViewer
																	 {
		private System.Windows.Forms.Label _labelText;
		private Razor.InformationPanel _informationPanel;
		private System.Windows.Forms.Button _buttonCancel;
		private System.Windows.Forms.Button _buttonCancelAll;
		
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		
		/// <summary>
		/// Occurs when the current context should be cancelled
		/// </summary>
		public event EventHandler Cancelled;		

		/// <summary>
		/// Occurs when all of the current contexts should be cancelled
		/// </summary>
		public event EventHandler AllCancelled;

		/// <summary>
		/// Initializes a new instance of the CancelMultipleProgressWindow class
		/// </summary>
		public CancelMultipleProgressWindow()
		{
			this.InitializeComponent();
			this.SetMarqueeMoving(true, true);
			
			_buttonCancelAll.Click += new EventHandler(OnButtonCancelAllClicked);
			_buttonCancel.Click += new EventHandler(OnButtonCancelClicked);

			_buttonCancelAll.FlatStyle = FlatStyle.System;
			_buttonCancel.FlatStyle = FlatStyle.System;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				try
				{
					this._informationPanel.Marquee.IsScrolling = false;
				}
				catch(System.Exception systemException)
				{
					System.Diagnostics.Trace.WriteLine(systemException);
				}

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
			this._informationPanel = new Razor.InformationPanel();
			this._labelText = new System.Windows.Forms.Label();
			this._buttonCancel = new System.Windows.Forms.Button();
			this._buttonCancelAll = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _informationPanel
			// 
			this._informationPanel.BackColor = System.Drawing.Color.White;
			this._informationPanel.Description = "";
			this._informationPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this._informationPanel.Image = null;
			this._informationPanel.Location = new System.Drawing.Point(0, 0);
			this._informationPanel.Name = "_informationPanel";
			this._informationPanel.Size = new System.Drawing.Size(394, 85);
			this._informationPanel.TabIndex = 0;
			this._informationPanel.Title = "";
			// 
			// _labelText
			// 
			this._labelText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._labelText.Location = new System.Drawing.Point(10, 95);
			this._labelText.Name = "_labelText";
			this._labelText.Size = new System.Drawing.Size(375, 35);
			this._labelText.TabIndex = 1;
			// 
			// _buttonCancel
			// 
			this._buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._buttonCancel.Location = new System.Drawing.Point(205, 140);
			this._buttonCancel.Name = "_buttonCancel";
			this._buttonCancel.TabIndex = 2;
			this._buttonCancel.Text = "Cancel";
			// 
			// _buttonCancelAll
			// 
			this._buttonCancelAll.Location = new System.Drawing.Point(125, 140);
			this._buttonCancelAll.Name = "_buttonCancelAll";
			this._buttonCancelAll.TabIndex = 3;
			this._buttonCancelAll.Text = "Cancel All";
			// 
			// CancelMultipleProgressWindow
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(394, 169);
			this.ControlBox = false;
			this.Controls.Add(this._buttonCancelAll);
			this.Controls.Add(this._buttonCancel);
			this.Controls.Add(this._labelText);
			this.Controls.Add(this._informationPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "CancelMultipleProgressWindow";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.ResumeLayout(false);

		}
		#endregion

		#region IProgressViewer Members

		/// <summary>
		/// Sets the text displayed in the caption bar (ie. the window caption)
		/// </summary>
		/// <param name="text">The text to display</param>
		public void SetTitle(string text)
		{		
			if (_informationPanel.InvokeRequired)
			{
				this.Invoke(new SetTextEventHandler(this.SetTitle), new object[] {text});
				return;
			}	
						
			try
			{
				this.Text = text;
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		/// <summary>
		/// Sets the text displayed in the information panel's heading (ie. the big bolded font)
		/// </summary>
		/// <param name="text">The text to display</param>
		public void SetHeading(string text)
		{			
			if (_informationPanel.InvokeRequired)
			{
				this.Invoke(new SetTextEventHandler(this.SetHeading), new object[] {text});
				return;
			}	
				
			try
			{
				_informationPanel.Title = text;	
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}		

		/// <summary>
		/// Sets the text displayed in the information panel's description (ie. the text just under the heading in normal font)
		/// </summary>
		/// <param name="text">The text to display</param>
		public void SetDescription(string text)
		{
			if (_informationPanel.InvokeRequired)
			{
				this.Invoke(new SetTextEventHandler(this.SetDescription), new object[] {text});
				return;
			}
			
			try
			{
				_informationPanel.Description = text;		
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		/// <summary>
		/// Sets the text displayed as the extended description (ie. describe the exact status of the current action; such as "Searching for files...")
		/// </summary>
		/// <param name="text">The text to display</param>
		public void SetExtendedDescription(string text)
		{
			if (_labelText.InvokeRequired)
			{
				this.Invoke(new SetTextEventHandler(this.SetExtendedDescription), new object[] {text});
				return;
			}

			try
			{
				_labelText.Text = text;				
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}		

		/// <summary>
		/// Sets the image displayed in the information panel's image (ie. the image in the upper right corner of the information panel)
		/// </summary>
		/// <param name="image">The image to display (any format that the Image class supports, it will be maxed in size to a 48 x 48 image, and centered if smaller</param>
		public void SetImage(Image image)
		{
			if (_informationPanel.InvokeRequired)
			{
				this.Invoke(new SetImageEventHandler(this.SetImage), new object[] {image});
				return;
			}
			try
			{
				_informationPanel.Image = image;			
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		/// <summary>
		/// Sets the motion of the marque in the information panel (ie. the scrolling image underneat the information panel)
		/// </summary>
		/// <param name="moving">A flag to indicate whether the marquee is moving or not</param>
		/// <param name="reset">A flag to indicate whether the marque will be reset (if starting, it will be reset first; otherwise if stopping it will be reset after it stops</param>
		public void SetMarqueeMoving(bool moving, bool reset)
		{
			if (_informationPanel.InvokeRequired)
			{
				this.Invoke(new SetMarqueeMovingEventHandler(this.SetMarqueeMoving), new object[] {moving, reset});
				return;
			}	

			try
			{
				// reset it before it starts
				if (moving)
					if (reset)
						_informationPanel.Marquee.Reset();
					        
				// control the motion of the marquee
				_informationPanel.Marquee.IsScrolling = moving;

				// reset it after it has stopped
				if (!moving)
					if (reset)
						_informationPanel.Marquee.Reset();			
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		#endregion

		public virtual void DisableButtons()
		{
			_buttonCancelAll.Enabled = false;
			_buttonCancel.Enabled = false;
		}

		public virtual void EnableButtons()
		{
			_buttonCancelAll.Enabled = true;
			_buttonCancel.Enabled = true;
		}

		/// <summary>
		/// Raises the AllCancelled event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnButtonCancelAllClicked(object sender, EventArgs e)
		{
			try
			{		
				this.DisableButtons();
				if (this.AllCancelled != null)
					this.AllCancelled(sender, e);
			}
			catch(ThreadAbortException)
			{
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}

		/// <summary>
		/// Raise the Cancelled event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnButtonCancelClicked(object sender, EventArgs e)
		{
			try
			{
				this.DisableButtons();
				if (this.Cancelled != null)
					this.Cancelled(sender, e);
			}
			catch(ThreadAbortException)
			{
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}
	}
}
