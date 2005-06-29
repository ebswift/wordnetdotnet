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
	#region BackgroundThreadPoolJobQueue

	/// <summary>
	/// Summary description for BackgroundThreadPoolJobQueue.
	/// </summary>
	public class BackgroundThreadPoolJobQueue : Queue 
	{	
		/// <summary>
		/// Initializes a new instance of the BackgroundThreadPoolJobQueue class
		/// </summary>
		public BackgroundThreadPoolJobQueue() : base()
		{

		}
		
		/// <summary>
		/// Adds a job to the queue
		/// </summary>
		/// <param name="job"></param>
		public void Enqueue(BackgroundThreadPoolJob job)
		{
			if (this.Contains(job))
				throw new BackgroundThreadPoolJobAlreadyQueuedException(job);

			base.Enqueue(job);
		}

		/// <summary>
		/// Removes the next job from the queue
		/// </summary>
		/// <returns></returns>
		public new BackgroundThreadPoolJob Dequeue()
		{
			return base.Dequeue() as BackgroundThreadPoolJob;
		}
		
		/// <summary>
		/// Returns the contents of the queue as an array of jobs
		/// </summary>
		/// <returns></returns>
		public new BackgroundThreadPoolJob[] ToArray()
		{
			return base.ToArray() as BackgroundThreadPoolJob[];
		}

		/// <summary>
		/// Determines if the job exists in the queue
		/// </summary>
		/// <param name="job"></param>
		/// <returns></returns>
		public bool Contains(BackgroundThreadPoolJob job)
		{
			IEnumerator it = base.GetEnumerator();
			while(it.MoveNext())
				if (Guid.Equals(((BackgroundThreadPoolJob)it.Current).Id, job.Id))
					return true;
			return false;
		}	
	}
	
	#endregion

}
