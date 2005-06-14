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
using System.Threading;
using System.Diagnostics;

namespace Razor.MultiThreading
{	
	/// <summary>
	/// Summary description for ManagedThread.
	/// </summary>
	public class ManagedThread : IDisposable
	{
		protected bool _disposed;
		protected Thread _thread;
		protected bool _traceAbortException;
		protected ManualResetEvent _started;
		protected ManualResetEvent _stopped;
		protected ManualResetEvent _runComplete;	
		protected object[] _args;
		protected bool _joinWhenDisposed;
		protected bool _autoSetStarted = true;

		/// <summary>
		/// Initializes a new instance of the ManagedThread class
		/// </summary>
		public ManagedThread()
		{
			
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
					this.Stop(null, _joinWhenDisposed); // ensure that when we get disposed, that we stop what we were doing!!!
				}
				_disposed = true;
			}
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets whether the ThreadAbortException is traced if it is thrown
		/// </summary>
		public bool TraceAbortException
		{
			get
			{
				return _traceAbortException;
			}
			set
			{
				_traceAbortException = value;
			}
		}

		/// <summary>
		/// Gets or sets whether the background thread will join the foreground thread when the object is disposed
		/// </summary>
		public bool JoinWhenDisposed
		{
			get
			{
				return _joinWhenDisposed;
			}
			set
			{
				_joinWhenDisposed = value;
			}
		}

		/// <summary>
		/// Determines if the thread is alive. Returns false if the thread is not running. 
		/// </summary>
		public bool IsRunning
		{
			get
			{
				try
				{			
					if (_thread != null)
					{
						bool stopped = _stopped.WaitOne(0, true);
						bool stopRequested = ((_thread.ThreadState & System.Threading.ThreadState.StopRequested) == System.Threading.ThreadState.StopRequested);
						bool stopped2 = ((_thread.ThreadState & System.Threading.ThreadState.Stopped) == System.Threading.ThreadState.Stopped);
						bool abortRequested = ((_thread.ThreadState & System.Threading.ThreadState.AbortRequested) == System.Threading.ThreadState.AbortRequested);
						return (_thread.IsAlive && !stopped);
					}					
				}
				catch(ObjectDisposedException)
				{
					Trace.WriteLine("The thread has been disposed, and is therefore no longer alive.");
				}
				catch(System.Exception systemException)
				{
					System.Diagnostics.Trace.WriteLine(systemException);
				}
				return false;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Starts the thread on it's own path of execution
		/// </summary>
		/// <param name="isBackgroundThread">A flag that determines if the thread is a background thread</param>
		/// <param name="args">User defined arguments to be passed to thread upon execution</param>
		/// <returns></returns>
		public bool Start(bool isBackgroundThread, params object[] args)
		{
			try
			{										
				_args = args;
				_started = new ManualResetEvent(false);	
				_runComplete = new ManualResetEvent(false);
				_stopped = new ManualResetEvent(false);
//				_suspended = new ManualResetEvent(false);
//				_resumed = new ManualResetEvent(false);
				_thread = new Thread(new ThreadStart(this.RunProc));
				_thread.IsBackground = isBackgroundThread;
				_thread.Name = this.GetType().FullName;
				_thread.Start();				
				_started.WaitOne();

				return true;
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
			return false;
		}

		/// <summary>
		/// Stops the thread. The execution path has no control if the thread is aborted.
		/// </summary>
		/// <param name="exceptionState"></param>
		/// <returns></returns>
		public bool Stop(object exceptionState, bool join)
		{
			try
			{
				// if the thread is running
				if (this.IsRunning)
				{
					// abort the thread
					_thread.Abort(exceptionState);

					// wait for the thread to terminate
					if (join) _thread.Join();

					// destroy the thread
					_thread = null;
				}
				
				return true;
			}
//			catch(ThreadStateException)
//			{
//
//			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return false;
		}
		
		/// <summary>
		/// Blocks the current thread until the thread's work is completed
		/// </summary>
		public void WaitForRunToComplete()
		{
			try
			{
				if (this.IsRunning)
					if (_runComplete != null)
						_runComplete.WaitOne();					
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}			

		#endregion

		#region Virtual Methods

		/// <summary>
		/// Virtual method for overriding the Run method
		/// </summary>
		/// <param name="args"></param>
		protected virtual void Run(object[] args)
		{
			
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Internal thread procedure
		/// </summary>
		private void RunProc()
		{
			try
			{
				if (_autoSetStarted)
					_started.Set();

				try 
				{ 
					this.Run(_args); 
				}	
				catch(Exception) {}
				
				if (_runComplete != null)
					_runComplete.Set();
			}
			catch(System.Threading.ThreadAbortException e)
			{
				Trace.WriteLineIf(_traceAbortException, e);
			}
			catch(System.Exception e)
			{
				System.Diagnostics.Trace.WriteLine(e);
			}
			finally
			{
				_stopped.Set();
			}
		}

		#endregion
	}
}
