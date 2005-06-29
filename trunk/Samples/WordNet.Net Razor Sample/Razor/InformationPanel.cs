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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Threading;

namespace Razor
{
	/// <summary>
	/// Summary description for InformationPanel.
	/// </summary>
	[Serializable()]
//	[TypeConverter(typeof(InformationPanelTypeConverter))]
	public class InformationPanel : System.Windows.Forms.UserControl 
	{
		private System.Windows.Forms.Label _labelTitle;		
		private System.Windows.Forms.Label _labelDescription;
		private System.Windows.Forms.PictureBox _pictureBox;
		private Razor.MarqueeControl _marquee;
//		private Thread _stripThread;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public InformationPanel()
		{
			this.InitializeComponent();						
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(InformationPanel));
			this._labelTitle = new System.Windows.Forms.Label();
			this._pictureBox = new System.Windows.Forms.PictureBox();
			this._labelDescription = new System.Windows.Forms.Label();
			this._marquee = new Razor.MarqueeControl();
			this.SuspendLayout();
			// 
			// _labelTitle
			// 
			this._labelTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._labelTitle.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._labelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this._labelTitle.Location = new System.Drawing.Point(10, 10);
			this._labelTitle.Name = "_labelTitle";
			this._labelTitle.Size = new System.Drawing.Size(305, 20);
			this._labelTitle.TabIndex = 0;
			// 
			// _pictureBox
			// 
			this._pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._pictureBox.BackColor = System.Drawing.Color.Transparent;
			this._pictureBox.Location = new System.Drawing.Point(325, 10);
			this._pictureBox.Name = "_pictureBox";
			this._pictureBox.Size = new System.Drawing.Size(64, 64);
			this._pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this._pictureBox.TabIndex = 1;
			this._pictureBox.TabStop = false;
			// 
			// _labelDescription
			// 
			this._labelDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._labelDescription.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._labelDescription.Location = new System.Drawing.Point(30, 35);
			this._labelDescription.Name = "_labelDescription";
			this._labelDescription.Size = new System.Drawing.Size(285, 40);
			this._labelDescription.TabIndex = 2;
			// 
			// _marquee
			// 
			this._marquee.Dock = System.Windows.Forms.DockStyle.Bottom;
			this._marquee.FrameRate = 33;
			this._marquee.ImageToScroll = ((System.Drawing.Image)(resources.GetObject("_marquee.ImageToScroll")));
			this._marquee.IsScrolling = false;
			this._marquee.Location = new System.Drawing.Point(0, 80);
			this._marquee.Name = "_marquee";
			this._marquee.Size = new System.Drawing.Size(400, 5);
			this._marquee.StepSize = 10;
			this._marquee.TabIndex = 0;
			// 
			// InformationPanel
			// 
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this._marquee);
			this.Controls.Add(this._labelDescription);
			this.Controls.Add(this._pictureBox);
			this.Controls.Add(this._labelTitle);
			this.Name = "InformationPanel";
			this.Size = new System.Drawing.Size(400, 85);
			this.ResumeLayout(false);

		}
		#endregion
		
		public override void Refresh()
		{
			base.Refresh ();

			foreach(Control c in this.Controls)
				c.Refresh();			
		}

		public string Title
		{
			get
			{
				return _labelTitle.Text;
			}
			set
			{
				_labelTitle.Text = value;
				this.Refresh();
			}
		}

		public string Description
		{
			get
			{
				return _labelDescription.Text;
			}
			set
			{
				_labelDescription.Text = value;
				this.Refresh();
			}
		}

		public Image Image
		{
			get
			{
				return _pictureBox.Image;
			}
			set
			{
				_pictureBox.Image = value;
				this.Refresh();
			}
		}

		public PictureBox ImagePictureBox
		{
			get
			{
				return _pictureBox;
			}
		}

//		[Browsable(true)]
//		[TypeConverter(typeof(MarqueeControlTypeConverter))]
		public MarqueeControl Marquee
		{
			get
			{
				return _marquee;
			}
		}
	}
}
