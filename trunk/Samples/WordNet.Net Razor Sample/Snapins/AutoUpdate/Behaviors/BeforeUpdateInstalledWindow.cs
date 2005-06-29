using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Razor.SnapIns.AutoUpdate.Behaviors
{
	/// <summary>
	/// Summary description for BeforeUpdateInstalledWindow.
	/// </summary>
	public class BeforeUpdateInstalledWindow :  BeforeOperationCompletedWindow
	{
		private string _appName;
		private Version _version;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Initializes a new instance of the BeforeUpdateInstalledWindow class
		/// </summary>
		/// <param name="appName">The name of the app</param>
		/// <param name="version">The version of the app</param>
		public BeforeUpdateInstalledWindow(string appName, Version version) : base()
		{
			this.InitializeComponent();
			this.Text = "Auto-Update: Update Ready for Installation";
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
			// BeforeUpdateInstalledWindow
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(494, 418);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.IsExpanded = true;
			this.Name = "BeforeUpdateInstalledWindow";

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
			return "The update is ready to install.\nWould you like to install it now?";
		}

		protected override string GetYesAnswerText()
		{
			return "Yes, install the update now.";
		}

		protected override string GetNoAnswerText()
		{
			return "No, I will install it later.";
		}

		protected override string GetAutoAnswerText()
		{
			return "Do not ask me again. Always install any available updates.";
		}
	}
}
