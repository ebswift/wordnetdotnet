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
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Razor.Attributes;
using Razor.SnapIns;

namespace Razor
{
	/// <summary>
	/// Summary description for SplashWindow.
	/// </summary>
	public class SplashWindow : System.Windows.Forms.Form, IProgressViewer
	{		
		private Razor.InformationPanel _informationPanel;		
		private System.Windows.Forms.Label _labelText;
		
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SplashWindow(System.Reflection.Assembly executable)
		{
			this.InitializeComponent();
			this.DisplayInformationAboutAssembly(executable, true);
			
			this.TopMost = true;
			this.ShowInTaskbar = true; // BUG: Windows 9x Fuckers!!!!!
		}

		public SplashWindow(System.Reflection.Assembly executable, bool showVersion)
		{
			this.InitializeComponent();
			this.DisplayInformationAboutAssembly(executable, showVersion);
			this.TopMost = true;
			this.ShowInTaskbar =  true; // BUG: Windows 9x Fuckers!!!!!
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SplashWindow));
			this._informationPanel = new Razor.InformationPanel();
			this._labelText = new System.Windows.Forms.Label();
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
			this._informationPanel.Size = new System.Drawing.Size(394, 85);
			this._informationPanel.TabIndex = 6;
			this._informationPanel.Title = "";
			// 
			// _labelText
			// 
			this._labelText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._labelText.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._labelText.Location = new System.Drawing.Point(8, 96);
			this._labelText.Name = "_labelText";
			this._labelText.Size = new System.Drawing.Size(378, 40);
			this._labelText.TabIndex = 8;
			// 
			// SplashWindow
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(394, 144);
			this.ControlBox = false;
			this.Controls.Add(this._labelText);
			this.Controls.Add(this._informationPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "SplashWindow";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Override the refresh to refresh everything, this is terribly important for splash screens as they tend to be running with a lot of system lag 
		/// </summary>
		public override void Refresh()
		{
			base.Refresh ();
			_informationPanel.Refresh();
			_labelText.Refresh();
		}
		
		/// <summary>
		/// Reads the meta data from assembly attributes and extracts the shell icon to display on the progress dialog
		/// </summary>
		/// <param name="assembly"></param>
		private void DisplayInformationAboutAssembly(System.Reflection.Assembly assembly, bool showVersion)
		{
			try
			{
				ProgressViewer.SetTitle(this, "Loading...");
				ProgressViewer.SetDescription(this, "This operation could take several seconds...");

				if (assembly != null)
				{
					// snag the name of the file minus path and extention and set it as the heading
					string filename = System.IO.Path.GetFileName(assembly.Location);
					filename = filename.Replace(System.IO.Path.GetExtension(assembly.Location), null);							
					ProgressViewer.SetHeading(this, filename);

					DirectoryInfo directory = new DirectoryInfo(Application.StartupPath);

//					// snag the version of the assembly, and tack it onto the heading
					AssemblyAttributeReader reader = new AssemblyAttributeReader(assembly);
//					Version v = reader.GetAssemblyVersion();
//					if (v != null)					
					ProgressViewer.SetHeading(this, filename + (showVersion ? " Version " + SnapInHostingEngine.Instance.AppVersion.ToString(): null));
									
					// snag the company that made the assembly, and set it in the title
					System.Reflection.AssemblyCompanyAttribute[] companyAttributes = reader.GetAssemblyCompanyAttributes();
					if (companyAttributes != null)
						if (companyAttributes.Length > 0)
							if (companyAttributes[0].Company != null && companyAttributes[0].Company != string.Empty)
                                ProgressViewer.SetTitle(this, companyAttributes[0].Company + " " + filename);

					// snag the image from the assembly, it should be an executable so...
					Icon largeIcon = ShellInformation.GetIconFromPath(assembly.Location, IconSizes.LargeIconSize, IconStyles.NormalIconStyle, FileAttributes.Normal);	
					if (largeIcon != null)							
						ProgressViewer.SetImage(this, largeIcon.ToBitmap() as Image);
				
					Icon smallIcon = ShellInformation.GetIconFromPath(assembly.Location, IconSizes.SmallIconSize, IconStyles.NormalIconStyle, FileAttributes.Normal);	
					if (smallIcon != null)				
						this.Icon = smallIcon;
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		#region IProgressViewer Members

		/// <summary>
		/// Sets the text displayed in the caption bar (ie. the window caption)
		/// </summary>
		/// <param name="text">The text to display</param>
		public void SetTitle(string text)
		{				
			this.Text = text;
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
			}	
			else
			{
				_informationPanel.Title = text;
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
			}	
			else
			{
				_informationPanel.Description = text;
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
			}	
			else
			{
				_labelText.Text = text;
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
			}	
			else
			{
				_informationPanel.Image = image;
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
			}	
			else
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
		}

		#endregion

	}
}
