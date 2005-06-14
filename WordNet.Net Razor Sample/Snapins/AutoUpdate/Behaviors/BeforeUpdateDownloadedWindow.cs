using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Razor.Networking.AutoUpdate.Common;

namespace Razor.SnapIns.AutoUpdate.Behaviors
{
	/// <summary>
	/// Summary description for BeforeUpdateDownloadedWindow.
	/// </summary>
	public class BeforeUpdateDownloadedWindow : BeforeOperationCompletedWindow
	{
		protected AutoUpdateDownloadDescriptor _dd;
		private System.Windows.Forms.ImageList _imageList;
		private System.ComponentModel.IContainer components;

		/// <summary>
		/// Initializes a new instance of the BeforeUpdateDownloadedWindow class
		/// </summary>
		/// <param name="appName">The name of the app</param>
		/// <param name="version">The version of the app</param>
		public BeforeUpdateDownloadedWindow(AutoUpdateDownloadDescriptor downloadDescriptor) : base()
		{
			this.InitializeComponent();
			this.Text = "Auto-Update: Update Available for Download";
			_dd = downloadDescriptor;
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(BeforeUpdateDownloadedWindow));
			this._imageList = new System.Windows.Forms.ImageList(this.components);
			// 
			// _imageList
			// 
			this._imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this._imageList.ImageSize = new System.Drawing.Size(16, 16);
			this._imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_imageList.ImageStream")));
			this._imageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// BeforeUpdateDownloadedWindow
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(492, 416);
			this.MinimumSize = new System.Drawing.Size(0, 0);
			this.Name = "BeforeUpdateDownloadedWindow";

		}
		#endregion

		#region My Overrides

		/// <summary>
		/// Overrides the load to display the change summaries
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);

			this.DisplaySummary();
			base.TextBox.Text = _dd.Manifest.ToString();			
		}

		/// <summary>
		/// Returns the text displayed for the operation text
		/// </summary>
		/// <returns></returns>
		protected override string GetOperationText()
		{
			return string.Format("{0} {1} ({2})", _dd.Manifest.Product.Name, _dd.Manifest.Product.Version, _dd.Downloader.FormatFileLengthForDisplay(_dd.Manifest.SizeOfUpdate));			
		}

		/// <summary>
		/// Returns the text displayed for the operation description
		/// </summary>
		/// <returns></returns>
		protected override string GetOperationDescriptionText()
		{
			return "There is a newer version available.\nWould you like to download it now?";
		}

		/// <summary>
		/// Returns the text displayed for the yes answer
		/// </summary>
		/// <returns></returns>
		protected override string GetYesAnswerText()
		{
			return "Yes, download the update now.";
		}

		/// <summary>
		/// Returns the text displayed for the no answer
		/// </summary>
		/// <returns></returns>
		protected override string GetNoAnswerText()
		{
			return "No, I will upgrade later.";
		}

		/// <summary>
		/// Returns the text displayed for the auto answer
		/// </summary>
		/// <returns></returns>
		protected override string GetAutoAnswerText()
		{
			return "Do not ask me again. Always download any available updates.";
		}

		/// <summary>
		/// Returns the text component of the more info link
		/// </summary>
		/// <returns></returns>
		protected override string GetLinkText()
		{
			return _dd.Manifest.MoreInfo.Text;
		}

		/// <summary>
		/// Returns the href component of the more info link
		/// </summary>
		/// <returns></returns>
		protected override string GetLinkHref()
		{
			return _dd.Manifest.MoreInfo.Href;
		}

		#endregion

		protected virtual void DisplaySummary()
		{
			try
			{
				base.ListView.BeginUpdate();
				base.ListView.Items.Clear();
				base.ListView.Columns.Clear();
				base.ListView.Columns.Add("Title", -2, HorizontalAlignment.Left);
				base.ListView.Columns.Add("Type", -2, HorizontalAlignment.Right);
				base.ListView.Columns.Add("Posted By", -2, HorizontalAlignment.Left);
				base.ListView.Columns.Add("Date", -2, HorizontalAlignment.Left);				
				
				base.ListView.SmallImageList = _imageList;
				foreach(AutoUpdateChangeSummary changeSummary in _dd.Manifest.ChangeSummaries)
				{
					ChangeSummaryListViewItem item = new ChangeSummaryListViewItem(changeSummary);
					base.ListView.Items.Add(item);
				}

				foreach(ColumnHeader hdr in base.ListView.Columns)
					hdr.Width = -2;
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
			finally
			{
				base.ListView.EndUpdate();
			}
		}

		#region ChangeSummaryListViewItem

		/// <summary>
		/// Provides a ListViewItem that represents an AutoUpdateChangeSummary object
		/// </summary>
		private class ChangeSummaryListViewItem : ListViewItem 
		{
			protected AutoUpdateChangeSummary _changeSummary;

			/// <summary>
			/// Initializes a new instance of the ChangeSummaryListViewItem class
			/// </summary>
			/// <param name="changeSummary"></param>
			public ChangeSummaryListViewItem(AutoUpdateChangeSummary changeSummary) : base(changeSummary.Title)
			{
				_changeSummary = changeSummary;
				base.ImageIndex = 0;				
				base.SubItems.Add(_changeSummary.Type.ToString());
				base.SubItems.Add(_changeSummary.PostedBy);
				base.SubItems.Add(_changeSummary.DatePosted.ToString());
			}

			/// <summary>
			/// Return the change summary instance represented by this ListViewItem
			/// </summary>
			public AutoUpdateChangeSummary ChangeSummary
			{
				get
				{
					return _changeSummary;
				}
			}
		}

		#endregion
	}
}
