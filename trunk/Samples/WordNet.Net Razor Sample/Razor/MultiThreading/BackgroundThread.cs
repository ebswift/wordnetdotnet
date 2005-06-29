/*
 * This file is a part of the Razor Framework.
 * 
 * Copyright (C) 2004 Mark (Code6) Belles 
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
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace Razor.MultiThreading
{
	#region BackgroundThread

	/// <summary>
	/// Defines a wrapper class around the .Net Thread class which makes multi-threading safer and easier
	/// </summary>
	public class BackgroundThread : IDisposable
	{
		protected bool _disposed;
		protected object[] _args;
		protected bool _allowThreadAbortException;
		protected ManualResetEvent _runningEvent;
		protected ManualResetEvent _completedEvent;
		protected Thread _processingThread;
		
		/// <summary>
		/// Occurs when the background processing should occur for the thread
		/// </summary>
		public event BackgroundThreadStartEventHandler Run;
		public event BackgroundThreadEventHandler Finished;

		/// <summary>
		/// Initializes a new instance of the BackgroundThread class
		/// </summary>
		public BackgroundThread()
		{

		}
		
		/// <summary>
		/// Initializes a new instance of the BackgroundThread class
		/// </summary>
		/// <param name="isBackground">A flag that indicates if the thread will be a background thread or not</param>
		/// <param name="args">An array of arguments to pass to the thread when it is started</param>
		/// <param name="onRun">A callback method that will be called when the thread runs</param>
		/// <param name="onFinished">A callback method that will be called when the thread finishes</param>
		public BackgroundThread(bool isBackground, object[] args, BackgroundThreadStartEventHandler onRun, BackgroundThreadEventHandler onFinished)
		{
			// the callback method is required now, if you waited until the thread starts, the thread will blow by the method 
			// that calls the callback before you can wire up to it, so we have to have it now
			if (onRun == null)
				throw new ArgumentNullException("onRun", "A callback method is required.");

			// auto wire the callback to our event
			this.Run += onRun;

			// auto start the thread
			this.Start(isBackground, args);
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					if (this.IsRunning)
						this.Stop();
				}
				_disposed = true;
			}
		}

		#endregion

		#region My Public Methods

		/// <summary>
		/// Starts the thread using the specified arguments
		/// </summary>
		/// <param name="isBackground">A flag that indicates whether the thread is a background thread or not</param>
		/// <param name="args">An array of arguments to pass to the thread when it starts</param>
		public virtual void Start(bool isBackground, object[] args)	
		{
			// if the thread is running
			if (this.IsRunning)
				// stop it before we continue
				this.Stop();
			
			// save the args that will be passed to the thread
			_args = args;

			// create an event for when the thread has started running
			_runningEvent = new ManualResetEvent(false);	

			// create an event for when the thread has finished running
			_completedEvent = new ManualResetEvent(false);

			// create a new thread aimed at our internal callback method
			_processingThread = new Thread(new ThreadStart(this.ThreadProc));

			_processingThread.Name = string.Format("{0} '0x{1}'", this.GetType().Name, this.GetHashCode().ToString("X"));

			// it is a background thread an will not keep the main thread alive
			_processingThread.IsBackground = isBackground;

			// start the thread
			_processingThread.Start();
			
			// and wait for the thread to start running
			_runningEvent.WaitOne();	
		}

		/// <summary>
		/// Stops the thread if it is already running
		/// </summary>
		/// <param name="stateObject"></param>
		public virtual void Stop()
		{
			try
			{
				if (_runningEvent != null)
				{
					if (_processingThread != null)
					{						
						if (_processingThread.IsAlive)
						{
							if (_allowThreadAbortException)
								_processingThread.Abort();
							else
								_processingThread.Suspend();
						}						
					}
												
					_processingThread = null;
					_runningEvent = null;
					_completedEvent = null;
					_args = null;
				}			
			}
			catch(ThreadAbortException)
			{
				
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}
		
		#endregion

		#region My Public Properties

		/// <summary>
		/// Gets or sets a flag that determines if the thread will be aborted vs suspended when stopping manually, thereby allowing/disallowing a ThreadAbortException to be thrown in the functions being executed by the thread
		/// </summary>
		public virtual bool AllowThreadAbortException
		{
			get
			{
				return _allowThreadAbortException;
			}
			set
			{
				_allowThreadAbortException = value;
			}
		}

		/// <summary>
		/// Returns a flag that indicates whether the thread is a background thread or not
		/// </summary>
		public virtual bool IsBackground
		{
			get
			{
				if (_processingThread == null)
					return true;
				return _processingThread.IsBackground;
			}
		}

		/// <summary>
		/// Returns an array of arguments to pass to the thread when it starts
		/// </summary>
		public virtual object[] Args
		{
			get
			{
				return _args;
			}
		}

		/// <summary>
		/// Returns a flag indicating whether the thread is running or not
		/// </summary>
		public virtual bool IsRunning
		{
			get
			{
				return (_runningEvent != null);
			}
		}
		
		/// <summary>
		/// Returns a flag indicating whether the thread has completed it's run or not
		/// </summary>
		public virtual bool IsFinished
		{
			get
			{
				if (_completedEvent == null)
					return true;

				if (_completedEvent.WaitOne(10, false))
					return true;

				return false;
			}
		}

		/// <summary>
		/// Waits indefinitely for the thread to finish
		/// </summary>
		public virtual void WaitToFinish()
		{
			if (this.IsRunning)
				if (_completedEvent != null)
					_completedEvent.WaitOne();
		}

		#endregion
				
		#region My Protected Methods

		/// <summary>
		/// Provides the internal method on which background processing will occur
		/// </summary>
		protected virtual void ThreadProc()
		{
			try
			{
				// set the running event to signal that the thread is running
				_runningEvent.Set();
				
				// call our virtual method which will raise the Run event
				// or if overridden in a derived class will allow for custom background processing
				this.OnRun(this, new BackgroundThreadStartEventArgs(_args));
			}
			catch(ThreadAbortException)
			{
				/*
				 * just in case somehow this thread get's aborted
				 * we'll be ready
				 * */
				//				this.MarkAsFinished();		
			}
			catch(Exception ex)
			{
				/*
				 * and in case there is some other error (most likely from a derived class messing up the override for OnRun), we'll catch that too
				 * */
				Trace.WriteLine(ex);
			}
			finally
			{
				// and lastly mark the fact that the thread is finished
				this.MarkAsFinished();				
			}
		}
		
		/// <summary>
		/// Marks the thread as being finished running
		/// </summary>
		protected virtual void MarkAsFinished()
		{
			try
			{
				// only fire the finished event one time
				if (!this.IsFinished)
					this.OnFinished(this, new BackgroundThreadEventArgs(this));		
				
				// mark the completion of the thread. might be as a result of an abort call, or the thread may just be finished executing
				if (_completedEvent != null)
					_completedEvent.Set();			
				
				_runningEvent = null;
			}
			catch(Exception ex)
			{
				if (ex.GetType() != typeof(ThreadAbortException))
					Debug.WriteLine(ex);
			}
		}

		/// <summary>
		/// Provides a virtual method to override and perform custom processing on a background thread
		/// </summary>
		/// <param name="args"></param>
		protected virtual void OnRun(object sender, BackgroundThreadStartEventArgs e)
		{
			try
			{
				if (this.Run != null)
					this.Run(sender, e);
			}
			catch(ThreadAbortException)
			{
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}			
		}		

		/// <summary>
		/// Raises the Finished event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnFinished(object sender, BackgroundThreadEventArgs e)
		{
			try
			{
				if (this.Finished != null)
					this.Finished(sender, e);
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}

		#endregion
	}
	
	#endregion

}
