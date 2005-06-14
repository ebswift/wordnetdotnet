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
using System.Windows.Forms;

namespace Razor
{
	/// <summary>
	/// Summary description for WindowManager.
	/// </summary>
	public class WindowManager
	{
		private static WindowManager _theInstance;
		private Hashtable _managedWindows;

		/// <summary>
		/// Occurs when before a window is show, and it may possibly need to be cancelled for any reason (security is the primary focus)
		/// </summary>
		public event WindowCancelEventHandler CanShowWindow;
		
		/// <summary>
		/// Returns the currently executing instance of the WindowManager class
		/// </summary>
		/// <returns></returns>
		public static WindowManager GetExecutingInstance()
		{
			return _theInstance;
		}

		/// <summary>
		/// Initializes a new instance of the WindowManager class
		/// </summary>
		public WindowManager()
		{
			_theInstance = this;
			_managedWindows = new Hashtable();
		}

		#region Public Methods

		/// <summary>
		/// Determines if a window can be show, provides external listeners to block a window for whatever reason
		/// </summary>
		/// <param name="window">The window that will be show</param>
		/// <param name="args">An array of user defined arguments</param>
		/// <returns></returns>
		public bool CanShow(Form window, params object[] args)
		{
			this.AssertValidWindow(window);

			WindowCancelEventArgs e = new WindowCancelEventArgs(false, window, args);
			this.OnCanShowWindow(this, e);
			return !e.Cancel;		
		}
		
		/// <summary>
		/// Begins tracking the lifetime of the window using the specified key
		/// </summary>
		/// <param name="window">The window to track and keep alive</param>
		/// <param name="key">The key by which the window will be identified</param>
		public void BeginTrackingLifetime(Form window, string key)
		{
			this.AssertValidWindow(window);
			this.AssertValidKey(key);
			this.AssertUniqueKey(key);

			window.Closed += new EventHandler(this.OnManageWindowClosed);
			_managedWindows.Add(key, window);
		}

		/// <summary>
		/// Ends tracking the lifetime of the window using the specified key
		/// </summary>
		/// <param name="key">The key by which the window will be identified</param>
		public void EndTrackingLifetime(string key)
		{			
			this.AssertValidKey(key);
			
			Form window = this.Find(key);
			if (window != null)
			{
				window.Closed -= new EventHandler(this.OnManageWindowClosed);
				_managedWindows.Remove(key);			
			}
		}			

		/// <summary>
		/// Returns the window identified using the specified key, otherwise returns null if no window is found
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public Form Find(string key)
		{
			if (_managedWindows.ContainsKey(key))
				return (Form)_managedWindows[key];
			return null;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// A hashtable of windows that the WindowManager is currently tracking
		/// </summary>
		public Hashtable ManagedWindows
		{
			get
			{
				return _managedWindows;
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Raises the CanShowWindow event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCanShowWindow(object sender, WindowCancelEventArgs e)
		{
			try
			{
				if (this.CanShowWindow != null)
					this.CanShowWindow(sender, e);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Debug.WriteLine(systemException);
			}
		}		

		/// <summary>
		/// Asserts the window is created and not disposed
		/// </summary>
		/// <param name="window"></param>
		private void AssertValidWindow(Form window)
		{
			if (window == null)
				throw new ArgumentNullException("Window", "Cannot use a window that has not been created.");

			if (window.IsDisposed)
				throw new ObjectDisposedException("Window", "Cannot use a window that has been disposed.");
		}

		/// <summary>
		/// Asserts the key is not null or empty string
		/// </summary>
		/// <param name="key"></param>
		private void AssertValidKey(string key)
		{
			if (key == null || key == string.Empty)
				throw new ArgumentNullException("Key", "The key cannot be null or empty string.");
		}

		/// <summary>
		/// Asserts the key is unique to the list of windows currently being tracked
		/// </summary>
		/// <param name="key"></param>
		private void AssertUniqueKey(string key)
		{
			if (_managedWindows.ContainsKey(key))
				throw new ArgumentException("Key", "The key must be unique.");
		}

		/// <summary>
		/// Occurs when a window we are tracking closes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnManageWindowClosed(object sender, EventArgs e)
		{
			Form window = sender as Form;
			if (window != null)
			{
				bool found = false;
				object key = null;
				foreach(DictionaryEntry entry in _managedWindows)
				{
					Form managedWindow = entry.Value as Form;
					if (managedWindow == window)
					{						
						key = entry.Key;
						found = true;
						break;
					}
				}
				
				if (found)
				{
					window.Closed -= new EventHandler(this.OnManageWindowClosed);
					_managedWindows.Remove(key);	
				}
			}
		}

		#endregion
	}

//	/// <summary>
//	/// 
//	/// </summary>
//	public class WindowEventArgs : System.EventArgs 
//	{
//		private Form _window;
//		private object[] _args;
//
//		public WindowEventArgs(Form window, params object[] args) : base()
//		{
//
//		}
//
//		public Form Window
//		{
//			get
//			{
//				return _window;
//			}
//		}
//
//		public object[] Args
//		{
//			get
//			{
//				return _args;
//			}
//		}
//	}
//
//	/// <summary>
//	/// 
//	/// </summary>
//	public delegate void WindowEventHandler(object sender, WindowEventArgs e);

	#region WindowCancelEventArgs

	/// <summary>
	/// 
	/// </summary>
	public class WindowCancelEventArgs : System.ComponentModel.CancelEventArgs
	{
		private Form _window;
		private object[] _args;

		public WindowCancelEventArgs(bool cancel, Form window, params object[] args) : base(cancel)
		{
			_window = window;
			_args = args;
		}

		public Form Window
		{
			get
			{
				return _window;
			}
		}

		public object[] Args
		{
			get
			{
				return _args;
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public delegate void WindowCancelEventHandler(object sender, WindowCancelEventArgs e);

	#endregion
}
