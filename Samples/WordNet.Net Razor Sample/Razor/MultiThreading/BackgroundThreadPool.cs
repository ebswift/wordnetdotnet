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
	#region BackgroundThreadPool

	/// <summary>
	/// Summary description for BackgroundThreadPool.
	/// </summary>
	public class BackgroundThreadPool : IDisposable
	{
		public const int DefaultMaximumNumberOfThreads = 25;
		protected bool _disposed;
		protected int _maxThreads;
		protected BackgroundThread _processingThread;
		protected BackgroundThreadPoolJobQueue _jobQueue;
		protected ArrayList _threads;

		/// <summary>
		/// Initializes a new instance of the BackgroundThreadPool class
		/// </summary>
		public BackgroundThreadPool()
		{
			_threads = new ArrayList();
			_jobQueue = new BackgroundThreadPoolJobQueue();
			_maxThreads = BackgroundThreadPool.DefaultMaximumNumberOfThreads;
			this.StartProcessingJobs();
		}
		
		/// <summary>
		/// Initializes a new instance of the BackgroundThreadPool class
		/// </summary>
		/// <param name="maxThreads">The maximum numer of threads to use</param>
		public BackgroundThreadPool(int maxThreads) 
		{
			_threads = new ArrayList();
			_jobQueue = new BackgroundThreadPoolJobQueue();
			_maxThreads = maxThreads;
			this.StartProcessingJobs();
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
					// dispose of the processing thread
					_processingThread.Dispose();
					_processingThread = null;

					// dispose of all of the thread pool threads
					this.DestroyThreads(false /* all threads */);
				}
				_disposed = true;
			}
		}

		#endregion
		
		#region My Protected Methods

		/// <summary>
		/// Starts a background thread that will monitor the job queue and spawn background threads to process the jobs in the queue
		/// </summary>
		protected virtual void StartProcessingJobs()
		{
			_processingThread = new BackgroundThread();
			_processingThread.AllowThreadAbortException = true;
			_processingThread.Run += new BackgroundThreadStartEventHandler(OnProcessJobs);
			_processingThread.Start(true, new object[] {_maxThreads});			
		}
		
		/// <summary>
		/// Returns a thread safe synchronized list of threads
		/// </summary>
		protected ArrayList ThreadList
		{
			get
			{
				return ArrayList.Synchronized(_threads);
			}
		}

		/// <summary>
		/// Monitors the jobs in the job queue and delegates threads to service the waiting jobs
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnProcessJobs(object sender, BackgroundThreadStartEventArgs e)
		{
			try
			{
				// continuously monitor the job queue and thread count
				while(true)
				{		
					// lock the thread list
					lock(this.ThreadList.SyncRoot)
					{
						// if we haven't reached the max thread limit
						if (this.ThreadList.Count < _maxThreads)
						{				
							// lock the job queue
							lock(this.JobQueue.SyncRoot)
							{
								// if there are jobs waiting
								if (this.JobQueue.Count > 0)
								{
									// dequeue the next waiting job
									BackgroundThreadPoolJob job = this.JobQueue.Dequeue();

									// create a new background thread pool thread to process the job
									BackgroundThreadPoolThread thread = new BackgroundThreadPoolThread(job);															

									// and finally add the thread to our list of threads
									this.ThreadList.Add(thread);
								}
							}
						}	
   					
						this.DestroyThreads(true /* only the finished ones */);
					}

					Thread.Sleep(100);
				}
			}
			catch(ThreadAbortException)
			{
				// the processing thread is aborting
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		/// <summary>
		/// Destroys the specified thread
		/// </summary>
		/// <param name="thread"></param>
		/// <param name="force"></param>
		protected virtual void DestroyThread(BackgroundThread thread, bool force)
		{
			try
			{
				if (thread != null)
					if (thread.IsFinished || force)
						thread.Dispose();
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

		/// <summary>
		/// Returns a thread safe wrapper around the job queue
		/// </summary>
		public BackgroundThreadPoolJobQueue JobQueue
		{
			get
			{
				return _jobQueue;
			}
		}
		
		/// <summary>
		/// Creates a thread pool job and enqueues it for the thread pool to execute
		/// </summary>
		/// <param name="startInfo">The background thread start info that will be executed as a thread pool job</param>
		/// <returns></returns>
		public virtual BackgroundThreadPoolJob QueueJob(string name, BackgroundThreadStartInfo startInfo)
		{
			// lock the queue
			lock(this.JobQueue.SyncRoot)
			{
				// create a new job
				BackgroundThreadPoolJob job = new BackgroundThreadPoolJob(name, startInfo);
				
				// and enqueue the job to be processed
				this.JobQueue.Enqueue(job);

				// return the job that was created and enqueued
				return job;
			}
		}
		
		/// <summary>
		/// Enqueues a thread pool job to be executed by the thread pool
		/// </summary>
		/// <param name="job"></param>
		public virtual void QueueJob(BackgroundThreadPoolJob job)
		{
			// lock the queue
			lock(this.JobQueue.SyncRoot)
			{				
				// and enqueue the job to be processed
				this.JobQueue.Enqueue(job);
			}
		}

		/// <summary>
		/// Cancels a job if it has not been started already
		/// </summary>
		/// <param name="job"></param>
		public virtual bool CancelJob(BackgroundThreadPoolJob job)
		{
			// first check in the queue
         
			// then check for a running job     
			return false;
		}

		/// <summary>
		/// Destroys the specified threads
		/// </summary>
		/// <param name="onlyFinishedThreads"></param>
		public virtual void DestroyThreads(bool onlyFinishedThreads)
		{
			try
			{
				// lock the thread list
				lock(this.ThreadList.SyncRoot)
				{
					// enum the threads
					for(int i = 0; i < this.ThreadList.Count; i++)
					{
						// get the thread at each index
						BackgroundThread thread = (BackgroundThread)this.ThreadList[i];
						bool kill = true;

						// if we're only killing threads that are finished, and this one is not finished, skip it
						if (onlyFinishedThreads)
							if (!thread.IsFinished)
								kill = false;

						// if this thread is supposed to die
						if (kill)
						{
							// kill it and remove it from our list, then backup 1 because the list shrank and go again
							this.DestroyThread((BackgroundThread)this.ThreadList[i], true);
							this.ThreadList.RemoveAt(i);                   
							i--;
						}
					}
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
	}

	#endregion

}
