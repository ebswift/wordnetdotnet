using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Razor.Networking;

namespace Razor.Networking.Icmp
{
	/// <summary>
	/// Summary description for PingerDialog.
	/// </summary>
	public class PingerDialog : System.Windows.Forms.Form
	{
		protected Pinger _pinger;
		protected bool _autoStartPinging;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button _buttonAction;
		private System.Windows.Forms.TextBox _textBoxAddress;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox _textBoxPingCount;
		private System.Windows.Forms.RichTextBox _textBoxOutput;
		private System.Windows.Forms.Button _buttonCancel;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Initializes a new instance of the PingerDialog class
		/// </summary>
		public PingerDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			this.InitializeComponent();
			this.StartPosition = FormStartPosition.CenterParent;

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_textBoxAddress.TextChanged += new EventHandler(OnTextBoxTextChanged);
			_textBoxPingCount.TextChanged += new EventHandler(OnTextBoxTextChanged);
			_textBoxOutput.LinkClicked += new LinkClickedEventHandler(OnLinkClicked);

			_buttonAction.Click += new EventHandler(OnButtonActionClicked);
			_buttonCancel.Click += new EventHandler(OnButtonCloseClicked);
		}		
		
		/// <summary>
		/// Initializes a new instance of the PingerDialog class
		/// </summary>
		/// <param name="address"></param>
		/// <param name="startPinging"></param>
		public PingerDialog(string address, bool autoStartPinging)
		{
			//
			// Required for Windows Form Designer support
			//
			this.InitializeComponent();
			this.StartPosition = FormStartPosition.CenterParent;

			_pinger = new Pinger();
			_pinger.Exception += new Razor.Networking.ExceptionEventHandler(OnPingException);
			_pinger.PingStarted += new PingerEventHandler(OnPingStarted);
			_pinger.PingResult += new PingerResultEventHandler(OnPingResult);
			_pinger.PingStatistics += new PingerStatisticsEventHandler(OnPingStatistics);
			_pinger.PingFinished += new EventHandler(OnPingFinished);

			_textBoxAddress.Text = address;
			_autoStartPinging = autoStartPinging;

			_textBoxAddress.TextChanged += new EventHandler(OnTextBoxTextChanged);
			_textBoxPingCount.TextChanged += new EventHandler(OnTextBoxTextChanged);
			_textBoxOutput.LinkClicked += new LinkClickedEventHandler(OnLinkClicked);
			
			_buttonAction.Click += new EventHandler(OnButtonActionClicked);
			_buttonCancel.Click += new EventHandler(OnButtonCloseClicked);			
		}		

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				this.StopPing();

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
			this.label1 = new System.Windows.Forms.Label();
			this._buttonAction = new System.Windows.Forms.Button();
			this._textBoxAddress = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this._textBoxOutput = new System.Windows.Forms.RichTextBox();
			this._textBoxPingCount = new System.Windows.Forms.TextBox();
			this._buttonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(135, 15);
			this.label1.TabIndex = 0;
			this.label1.Text = "IP Address or DNS Name";
			// 
			// _buttonAction
			// 
			this._buttonAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonAction.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._buttonAction.Location = new System.Drawing.Point(275, 300);
			this._buttonAction.Name = "_buttonAction";
			this._buttonAction.TabIndex = 1;
			this._buttonAction.Text = "Ping";
			// 
			// _textBoxAddress
			// 
			this._textBoxAddress.Location = new System.Drawing.Point(10, 30);
			this._textBoxAddress.Name = "_textBoxAddress";
			this._textBoxAddress.Size = new System.Drawing.Size(130, 20);
			this._textBoxAddress.TabIndex = 4;
			this._textBoxAddress.Text = "255.255.255.255";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(155, 10);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(65, 15);
			this.label2.TabIndex = 5;
			this.label2.Text = "Ping Count";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(10, 60);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 15);
			this.label3.TabIndex = 6;
			this.label3.Text = "Output";
			// 
			// _textBoxOutput
			// 
			this._textBoxOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._textBoxOutput.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._textBoxOutput.Location = new System.Drawing.Point(10, 80);
			this._textBoxOutput.Name = "_textBoxOutput";
			this._textBoxOutput.ShowSelectionMargin = true;
			this._textBoxOutput.Size = new System.Drawing.Size(425, 210);
			this._textBoxOutput.TabIndex = 7;
			this._textBoxOutput.Text = "";
			// 
			// _textBoxPingCount
			// 
			this._textBoxPingCount.Location = new System.Drawing.Point(155, 30);
			this._textBoxPingCount.Name = "_textBoxPingCount";
			this._textBoxPingCount.Size = new System.Drawing.Size(65, 20);
			this._textBoxPingCount.TabIndex = 8;
			this._textBoxPingCount.Text = "4";
			// 
			// _buttonCancel
			// 
			this._buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._buttonCancel.Location = new System.Drawing.Point(355, 300);
			this._buttonCancel.Name = "_buttonCancel";
			this._buttonCancel.TabIndex = 9;
			this._buttonCancel.Text = "Close";
			// 
			// PingerDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(442, 332);
			this.Controls.Add(this._buttonCancel);
			this.Controls.Add(this._textBoxPingCount);
			this.Controls.Add(this._textBoxOutput);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this._textBoxAddress);
			this.Controls.Add(this._buttonAction);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "PingerDialog";
			this.Text = "Ping";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Override the loading phase and start pinging if possible when instructed
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);

			// if the dialog is supposed to ping automatically
			if (_autoStartPinging)
				// instruct it to take the actions necessary
				this.TakeAction();
		}


		/// <summary>
		/// Gets or sets the address to ping
		/// </summary>
		public string Address
		{
			get
			{
				return _textBoxAddress.Text;
			}
			set
			{
				_textBoxAddress.Text = value;
			}
		}

		/// <summary>
		/// Occurs when the 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTextBoxTextChanged(object sender, EventArgs e)
		{
			// grab the source of the event
			TextBox textBox = (TextBox)sender;

			// everything is ok if the text for the textbox isn't null
			bool ok = (textBox.Text != null && textBox.Text != string.Empty);

			// base the button's enabled state upon this principle
			_buttonAction.Enabled = ok;
		}

		/// <summary>
		/// Occurs when the action button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnButtonActionClicked(object sender, EventArgs e)
		{
			this.TakeAction();
		}		

		/// <summary>
		/// Occurs when the cancel button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnButtonCloseClicked(object sender, EventArgs e)
		{
			this.Close();
		}

		/// <summary>
		/// Starts the ping process
		/// </summary>
		/// <param name="address"></param>
		/// <param name="timesToPing"></param>
		protected void StartPing(string address, int timesToPing)
		{			
			_pinger.BeginPinging(address, timesToPing);					
		}

		/// <summary>
		/// Stops the ping process
		/// </summary>
		protected void StopPing()
		{			
			if (_pinger != null)
			{				
				_pinger = null;
			}		
		}

		/// <summary>
		/// Either starts or cancels the ping process
		/// </summary>
		protected void TakeAction()
		{
			try
			{
				_buttonAction.Enabled = false;

				// parse the address and the text
				string address = _textBoxAddress.Text;
				int timesToPing = int.Parse(_textBoxPingCount.Text);

				// start pinging
				_pinger.BeginPinging(address, timesToPing);
			}			
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
				MessageBox.Show(this.ParentForm, ex.ToString(), "Exception Encountered", MessageBoxButtons.OK, MessageBoxIcon.Error); 
			}
		}

		/// <summary>
		/// Handles the ping exceptions
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPingException(object sender, ExceptionEventArgs e)
		{
			_textBoxOutput.AppendText(string.Format("Exception Encountered:\n\t{0}\n", e.Exception.ToString()));
		}

		/// <summary>
		/// Handles the ping start
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPingStarted(object sender, PingerEventArgs e)
		{	
			// clear the output window
			_textBoxOutput.Text = null;

			// state who/what we are pinging
			if (e.IsHostname)
				_textBoxOutput.AppendText(string.Format("Pinging {0} [{1}] with {2} bytes of data:\n\n", e.Address, e.Destination.ToString(), 32));
			else
				_textBoxOutput.AppendText(string.Format("Pinging {0} with {1} bytes of data:\n\n", e.Destination.ToString(), 32));
		}

		/// <summary>
		/// Handles ths ping results
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPingResult(object sender, PingerResultEventArgs e)
		{
			// display the status of the ping result
			if (e.TimedOut)
				_textBoxOutput.AppendText("Request timed out.\n");			
			else
				_textBoxOutput.AppendText(string.Format("Reply from {0}: bytes={1} time={2}ms\n", e.Destination.ToString(), e.BytesReceived, e.ElapsedMilliseconds));

		}

		/// <summary>
		/// Handles the ping statistics
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPingStatistics(object sender, PingerStatisticsEventArgs e)
		{			
			// display the packet statistics
			_textBoxOutput.AppendText(string.Format("\nPing statistics for {0}:\n\tPackets: Sent = {1}, Received = {2}, Lost = {3} ({4}% loss)\n", e.Destination.ToString(), e.PacketsSent, e.PacketsReceived, e.PacketsLost, e.PercentLost));            

			// display the time statistics
			_textBoxOutput.AppendText(string.Format("Approximate round trip times in milli-seconds:\n\tMinimium = {0}ms, Maximum = {1}ms, Average = {2}ms\n", e.MinimumElapsedMilliseconds, e.MaximumElapsedMilliseconds, e.AverageElapsedMilliseconds));
		}

		/// <summary>
		/// Handles the ping finished events
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPingFinished(object sender, EventArgs e)
		{
			_buttonAction.Enabled = true; 
		}

		/// <summary>
		/// Handles links that are clicked from the output window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnLinkClicked(object sender, LinkClickedEventArgs e)
		{
			try
			{
				// try and start the link using shellexecute
				Process.Start(e.LinkText);
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
				MessageBox.Show(this.ParentForm, ex.ToString(), "Exception Encountered", MessageBoxButtons.OK, MessageBoxIcon.Error); 
			}
		}
	}
}
