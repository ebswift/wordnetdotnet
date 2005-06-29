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
	/// Summary description for CancellableProgressWindow.
	/// </summary>
	public class CancellableProgressWindow : System.Windows.Forms.Form, IProgressViewer
																	 {
		private System.Windows.Forms.Label _labelText;
		private Razor.InformationPanel _informationPanel;
		private System.Windows.Forms.Button buttonCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public event EventHandler Cancelled;

		public CancellableProgressWindow()
		{
			this.InitializeComponent();
			this.SetMarqueeMoving(true, true);
			this.buttonCancel.Click += new EventHandler(OnCancelButtonClicked);
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
			this.buttonCancel = new System.Windows.Forms.Button();
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
			// buttonCancel
			// 
			this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonCancel.Location = new System.Drawing.Point(160, 140);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Text = "Cancel";
			// 
			// CancellableProgressWindow
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(394, 169);
			this.ControlBox = false;
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this._labelText);
			this.Controls.Add(this._informationPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "CancellableProgressWindow";
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

		private void OnCancelButtonClicked(object sender, EventArgs e)
		{
			try
			{
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
