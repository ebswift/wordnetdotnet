using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Razor.SnapIns.AutoUpdate.Behaviors
{
	/// <summary>
	/// Summary description for BeforeUpdateCopiedToAlternatePathWindow.
	/// </summary>
	public class BeforeUpdateCopiedToAlternatePathWindow : BeforeOperationCompletedWindow
	{
		private string _appName;
		private Version _version;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Initializes a new instance of the BeforeUpdateSwitchedToWindow class
		/// </summary>
		/// <param name="appName">The name of the app</param>
		/// <param name="version">The version of the app</param>
		public BeforeUpdateCopiedToAlternatePathWindow(string appName, Version version) : base()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			this.Text = "Auto-Update: Update Ready for Backup";
			_appName = appName;
			_version = version;
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
			// 
			// BeforeUpdateCopiedToAlternatePathWindow
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(492, 416);
			this.IsExpanded = true;
			this.Name = "BeforeUpdateCopiedToAlternatePathWindow";

		}
		#endregion

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);

			base.EnableDetails(false);
		}

		protected override string GetOperationText()
		{
			return string.Format("{0} {1} Update", _appName, _version.ToString());
		}

		protected override string GetOperationDescriptionText()
		{
			return "Would you like to copy the update to your 'Alternate Download Path'?";
		}

		protected override string GetYesAnswerText()
		{
			return "Yes, make a copy of the update for me now.";
		}

		protected override string GetNoAnswerText()
		{
			return "No, I don't want a copy of the update.";
		}

		protected override string GetAutoAnswerText()
		{
			return "Do not ask me again. Always make a copy of the update.";
		}
	}
}
