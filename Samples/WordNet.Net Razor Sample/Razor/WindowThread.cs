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
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
//using Razor.MultiThreading;

namespace Razor
{
	/// <summary>
	/// Summary description for WindowThread.
	/// </summary>
	public class WindowThread : IDisposable
	{
		protected bool _disposed;
		protected bool _windowOk;
		protected Form _window;
		protected ManualResetEvent _started;
		protected bool _secureWindow;
		protected bool _topMost;
		protected delegate void RunCallback(object[] args);
	
		public event WindowThreadEventHandler WindowCreated;
		public event WindowThreadExceptionEventHandler Exception;

		/// <summary>
		/// Initializes a new instance of the WindowThread class
		/// </summary>
		public WindowThread()
		{

		}

		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Override the dispose so that we can close the window if need be
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				//				Trace.WriteLine("Disposing of Window Thread...");

				if (disposing)
				{
					try
					{
						if (_window != null)
						{
							if (_window.InvokeRequired)
								_window.Invoke(new MethodInvoker(_window.Close), new object[] {});
							else
								_window.Close();
						}
					}
					catch(Exception ex)
					{
						Trace.WriteLine(ex);
					}
				}
				_disposed = true;
			}
		}

		#endregion
		
		#region My Public Properties

		/// <summary>
		/// Returns the Window that we are showing on the background thread
		/// </summary>
		public Form Window
		{
			get
			{
				return _window;
			}
		}		

		/// <summary>
		/// Gets or sets a value that determines if the Window is a security risk and should be protected via the Window Manager
		/// </summary>
		public bool SecureWindow
		{
			get
			{
				return _secureWindow;
			}
			set
			{
				_secureWindow = value;
			}
		}

		/// <summary>
		/// Gets or sets a flag that indicates whether the window will be top most when it is displayed
		/// </summary>
		public bool TopMost
		{
			get
			{
				return _topMost;
			}
			set
			{
				_topMost = value;
			}
		}

		/// <summary>
		/// Rerturns a value that indicates whether the window is safe to use or not (Passed creation and security checks)
		/// </summary>
		public bool WindowIsSafeToUse
		{
			get
			{
				return _windowOk;
			}
		}

		#endregion

		#region My Public Methods

		/// <summary>
		/// Shows the Window asynchronously on a background thread
		/// </summary>
		/// <returns></returns>
		public bool ShowAsynchronously()
		{
			return this.Start(new object[] {});
		}

		/// <summary>
		/// Shows the Window asynchronously on a background thread with the specified owner
		/// </summary>
		/// <param name="owner">The owner of the form, when the dialog is shown modally</param>
		/// <returns></returns>
		public bool ShowAsynchronously(IWin32Window owner)
		{
			return this.Start(new object[] {owner});
		}

		#endregion

		#region My Protected Methods

		/// <summary>
		/// Internally starts the asynchronous delegate that will drive the Form's message pump on a background thread
		/// </summary>
		/// <param name="args">An array of object arguments to be passed to the Run method</param>
		/// <returns></returns>
		protected bool Start(object[] args)
		{
			try
			{
				_windowOk = false;
				_started = new ManualResetEvent(false);

				RunCallback callback = new RunCallback(this.Run);

				IAsyncResult ar = callback.BeginInvoke(args, new AsyncCallback(OnRunEnd), this);
				if (ar != null)
				{
					_started.WaitOne();
					return true;
				}
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
			return false;
		}

		/// <summary>
		/// Handles the end of the thread running
		/// </summary>
		/// <param name="ar"></param>
		protected void OnRunEnd(IAsyncResult ar)
		{
			//			if (ar.IsCompleted)
			//			{
			//				
			//			}
		}

		/// <summary>
		/// Override the background thread's actions to create and show a window modally
		/// </summary>
		/// <param name="args"></param>
		protected void Run(object[] args)
		{		
			try
			{					
				// if the window managed by this thread has not been created
				if (_window == null)					
					// call the virtual method to create the window
					this.GetWindowType(out _window);
  
				// if the window has not been created by this point, throw an exception
				if (_window == null)
					throw new ArgumentNullException("Window", "No window instance was provided for the thread to show.");

				// signal the window's creation
				this.OnWindowCreated(this, new WindowThreadEventArgs(_window));
				
				// if a window was created
				DialogResult result = DialogResult.Abort;

				// try and snag the owner
				IWin32Window owner = null;
				if (args != null)
					if (args.Length > 0)
						owner = args[0] as IWin32Window;
				
				// if the window is a secure window
				if (_secureWindow)
					// check to see if access is denied via the Window Manager
					if (!SnapIns.SnapInHostingEngine.Instance.WindowManager.CanShow(_window, args))
						// access is denied if the window manager tells us it cannot be shown
						throw new WindowThreadAccessDeniedException("The current user has not been cranted access to this window.");
				
				// make it top most if we must
				if (_topMost)
					_window.TopMost = true;

				// notify the calling thread that the background thread has started, and that the window is ok
				_windowOk = true;
				_started.Set();

				// show the window modally w/wo the owner
				if (owner != null)
					result = _window.ShowDialog(owner);
				else
					result = _window.ShowDialog();														
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
				this.OnException(this, new WindowThreadExceptionEventArgs(ex));
			}
			finally
			{
				// make sure the window is destroyed
				_window = null;

				if (_started != null)
					_started.Set();
			}			
		}

		/// <summary>
		/// This method returns a new instance of a class deriving from the Form class that will be used to show a window from
		/// </summary>
		/// <param name="window">The object instance that derives from the Form class for the Window to be shown</param>
		protected virtual void GetWindowType(out Form window)
		{
			window = new System.Windows.Forms.Form();
		}

		/// <summary>
		/// Raises the WindowCreated event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnWindowCreated(object sender, WindowThreadEventArgs e)
		{
			try
			{
				if (this.WindowCreated != null)
					this.WindowCreated(sender, e);
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		/// <summary>
		/// Raises the Exception event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnException(object sender, WindowThreadExceptionEventArgs e)
		{
			try
			{
				if (this.Exception != null)
					this.Exception(sender, e);
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		#endregion
	}

	#region WindowThreadEventArgs

	public class WindowThreadEventArgs : EventArgs
	{
		protected Form _window;

		public WindowThreadEventArgs(Form window) : base()
		{
			_window = window;
		}

		public Form Window
		{
			get
			{
				return _window;
			}
		}
	}

	public delegate void WindowThreadEventHandler(object sender, WindowThreadEventArgs e);

	#endregion	

	#region WindowThreadExceptionEventArgs

	public class WindowThreadExceptionEventArgs : EventArgs
	{
		protected Exception _ex;

		public WindowThreadExceptionEventArgs(Exception ex)
		{
			_ex = ex;
		}

		public Exception Exception 
		{
			get
			{
				return _ex;
			}
		}
	}

	public delegate void WindowThreadExceptionEventHandler(object sender, WindowThreadExceptionEventArgs e);

	#endregion

	#region WindowThreadAccessDeniedException

	public class WindowThreadAccessDeniedException : Exception
	{
		public WindowThreadAccessDeniedException(string message) : base(message)
		{

		}
	}	

	#endregion
}
