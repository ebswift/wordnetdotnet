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
	#region BackgroundThreadPoolThread

	/// <summary>
	/// Summary description of BackgroundThreadPoolThread.
	/// </summary>
	public class BackgroundThreadPoolThread : BackgroundThread 
	{
		protected BackgroundThreadPoolJob _job;

		/// <summary>
		/// Initializes a new instance of the BackgroundThreadPoolThread class
		/// </summary>
		/// <param name="job"></param>
		public BackgroundThreadPoolThread(BackgroundThreadPoolJob job) : base()
		{
			_job = job;
					
			// wire the job's start info to the thread events
			base.Run += _job.StartInfo.Run;			
			if (_job.StartInfo.Finished != null)
				base.Finished += _job.StartInfo.Finished;

			// determine if the thread will allow ThreadAbortExceptions to be throw
			base.AllowThreadAbortException = _job.StartInfo.AllowThreadAbortExceptions;

			// start the thread automatically
			base.Start(true, _job.StartInfo.Args);								
		}

		#region My Overrides

		/// <summary>
		/// Override the OnRun method to capture the date and time that the job was started and to change it's state
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void OnRun(object sender, BackgroundThreadStartEventArgs e)
		{
			_job._dateTimeStarted = DateTime.Now;
			_job._state = BackgroundThreadPoolJobStates.Running;
 
			base.OnRun (sender, e);
		}


		/// <summary>
		/// Override the OnFinished method to capture the date and time that the job finishes and to change it's state
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void OnFinished(object sender, BackgroundThreadEventArgs e)
		{
			// mark the job as finished and the time that it finished at
			_job._dateTimeStopped = DateTime.Now;
			_job._finished = true;
			_job._state = BackgroundThreadPoolJobStates.Finished;

			base.OnFinished (sender, e);
		}

		#endregion

		#region My Public Properties

		/// <summary>
		/// Returns the job that this thread will execute
		/// </summary>
		public BackgroundThreadPoolJob Job
		{
			get
			{
				return _job;
			}
		}

		#endregion
	}
	
	#endregion

}
