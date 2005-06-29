using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Razor.Networking.PortMaps
{
	/// <summary>
	/// Summary description for PortMapViewer.
	/// </summary>
	public class PortMapViewer : System.Windows.Forms.UserControl
	{
		private enum ImageIndexes 
		{
			Root = 0,
			PortMap = 1,
			PortDescriptor = 2
		}

		private System.Windows.Forms.TreeView _treeView;
		private System.Windows.Forms.ImageList _imageList;
		private System.ComponentModel.IContainer components;

		public PortMapViewer()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(PortMapViewer));
			this._treeView = new System.Windows.Forms.TreeView();
			this._imageList = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// _treeView
			// 
			this._treeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._treeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._treeView.ImageList = this._imageList;
			this._treeView.Location = new System.Drawing.Point(0, 0);
			this._treeView.Name = "_treeView";
			this._treeView.Size = new System.Drawing.Size(150, 150);
			this._treeView.TabIndex = 0;
			// 
			// _imageList
			// 
			this._imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this._imageList.ImageSize = new System.Drawing.Size(16, 16);
			this._imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_imageList.ImageStream")));
			this._imageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// PortMapViewer
			// 
			this.Controls.Add(this._treeView);
			this.Name = "PortMapViewer";
			this.ResumeLayout(false);

		}
		#endregion

		public PortMapCollection PortMaps
		{			
			set
			{
				this.DisplayPortmaps(value);
			}
		}

		private void DisplayPortmaps(PortMapCollection portmaps)
		{
			if (portmaps != null)
			{
				TreeNode rootNode = _treeView.Nodes.Add("Port Maps");
				rootNode.ImageIndex = (int)ImageIndexes.Root;
				rootNode.SelectedImageIndex = (int)ImageIndexes.Root;

				foreach(PortMap portmap in portmaps)
				{
					TreeNode portmapNode = rootNode.Nodes.Add(portmap.Description);		
					portmapNode.ImageIndex = (int)ImageIndexes.PortMap;
					portmapNode.SelectedImageIndex = (int)ImageIndexes.PortMap;
					
					foreach(PortDescriptor descriptor in portmap.PortDescriptors)
					{
						TreeNode descriptorNode = portmapNode.Nodes.Add(descriptor.Description + " = " + descriptor.Port.ToString() + " (" + (descriptor.Offset >= 0 ? "+" : "-") + Math.Abs(descriptor.Offset).ToString() + ")");
						descriptorNode.ImageIndex = (int)ImageIndexes.PortDescriptor;
						descriptorNode.SelectedImageIndex = (int)ImageIndexes.PortDescriptor;
					}
				}
								
				rootNode.Expand();
			}
		}
	}
}
