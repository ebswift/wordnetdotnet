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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Razor
{
	#region FullScreenCapableWindow Class

	/// <summary>
	/// An inherited window class that adds the capability to go fullscreen, and provides events to facilitate this action
	/// </summary>
	public class FullScreenCapableWindow : System.Windows.Forms.Form
	{
		private bool _isFullScreen;		
		private Size _size;
		private FormWindowState _windowState;
		private Point _location;
		private System.ComponentModel.Container components = null;

		public event FullScreenCancelEventHandler BeforeFullScreenChanged;
		public event FullScreenEventHandler AfterFullScreenChanged;

		/// <summary>
		/// Initializes a new instance of the FullScreenCapableWindow
		/// </summary>
		public FullScreenCapableWindow()
		{
			this.InitializeComponent();

			_size = this.Size;
			_location = this.Location;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
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
			this.Size = new System.Drawing.Size(300,300);
			this.Text = "FullScreenCapableWindow";
		}
		#endregion

		#region Overrides

		/// <summary>
		/// intercept the size changes for this window and update our size if the window is normal, ie. not max'd or min'd
		/// </summary>
		/// <param name="e"></param>
		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged (e);

			if (this.WindowState == FormWindowState.Normal)
			{
				_size = this.Size;
			}
		}

		/// <summary>
		/// Occurs when the window's location has changed, and update our location if the window is normal, ie. not max'd or min'd
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLocationChanged(EventArgs e)
		{
			base.OnLocationChanged (e);

			if (this.WindowState == FormWindowState.Normal)
			{
				_location = this.Location;			
			}
		}
        
		/// <summary>
		/// Occurs when the window is double clicked
		/// </summary>
		/// <param name="e"></param>
		protected override void OnDoubleClick(EventArgs e)
		{
			base.OnDoubleClick (e);

			// toggle our fullscreen state
			this.ToggleFullScreen();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Toggles the window from fullscreen to restored and back
		/// </summary>
		public void ToggleFullScreen()
		{		
			// switch if we can
			if (this.SwitchToFullScreen(_isFullScreen, !_isFullScreen))
				// toggle the full screen state
				_isFullScreen = !_isFullScreen;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets whether the window is full screen or not
		/// </summary>
		public bool IsFullScreen
		{
			get
			{
				return _isFullScreen;
			}
			set
			{
				// switch if we can
				if (this.SwitchToFullScreen(_isFullScreen, value))
					// toggle the full screen state
					_isFullScreen = !_isFullScreen;
			}
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Raises the BeforeFullScreenChanged event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnBeforeFullScreenChanged(object sender, FullScreenCancelEventArgs e)
		{
			try
			{
				if (this.BeforeFullScreenChanged != null)
					this.BeforeFullScreenChanged(sender, e);				
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		/// <summary>
		/// Raises the AfterFullScreenChanged event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnAfterFullScreenChanged(object sender, FullScreenEventArgs e)
		{
			try
			{
				if (this.AfterFullScreenChanged != null)
					this.AfterFullScreenChanged(sender, e);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}
        
		#endregion

		#region Private Methods

		/// <summary>
		/// Sets the attributes of the window to better accomodate fullscreen operation, or restores it to the previous restored state
		/// </summary>
		/// <param name="isFullScreen"></param>
		private bool SwitchToFullScreen(bool isfullScreenBefore, bool isFullScreenAfter)
		{
			// raise the before changed event
			FullScreenCancelEventArgs ce = new FullScreenCancelEventArgs(_isFullScreen, false);
			this.OnBeforeFullScreenChanged(this, ce);
			
			// if the change wasn't cancelled
			if (!ce.Cancel)
			{					
				// modify the fullscreen state
				if (isFullScreenAfter)
				{				
					// it is supposed to be fullscreen
					_windowState = this.WindowState;
					this.FormBorderStyle = FormBorderStyle.None;
					this.WindowState = FormWindowState.Maximized;	
				}
				else
				{						
					// it isn't supposed to be fullscreen
					this.WindowState = _windowState;	
					this.FormBorderStyle = FormBorderStyle.Sizable;
					this.Location = _location;		
					this.Size = _size;			
				}

				// raise the after changed event
				this.OnAfterFullScreenChanged(this, new FullScreenEventArgs(isFullScreenAfter));
				
				// changes were made
				return true;
			}

			// no change was made
			return false;
		}

		#endregion
	}


	#endregion

	#region FullScreenEventArgs Class

	/// <summary>
	/// EventArgs for the FullScreenEventHandler delegate
	/// </summary>
	public class FullScreenEventArgs : System.EventArgs 
	{
		private bool _isFullScreen;

		public FullScreenEventArgs(bool isFullScreen) : base()
		{
			_isFullScreen = isFullScreen;
		}

		public bool IsFullScreen
		{
			get
			{
				return _isFullScreen;
			}
		}		
	}

	/// <summary>
	/// Delegate for the FullScreenEventArgs class
	/// </summary>
	public delegate void FullScreenEventHandler(object sender, FullScreenEventArgs e);	

	#endregion

	#region FullScreenCancelEventArgs Class

	/// <summary>
	/// CancelEventArgs for the FullScreenCancelEventHandler delegate
	/// </summary>
	public class FullScreenCancelEventArgs : System.ComponentModel.CancelEventArgs 
	{
		private bool _isFullScreen;

		public FullScreenCancelEventArgs(bool isFullScreen, bool cancel) : base(cancel)
		{
			_isFullScreen = isFullScreen;
		}

		public bool IsFullScreen
		{
			get
			{
				return _isFullScreen;
			}
		}		
	}

	/// <summary>
	/// Delegate for the FullScreenCancelEventArgs class
	/// </summary>
	public delegate void FullScreenCancelEventHandler(object sender, FullScreenCancelEventArgs e);	

	#endregion
}
