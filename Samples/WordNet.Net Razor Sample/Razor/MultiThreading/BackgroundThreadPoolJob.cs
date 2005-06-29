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
	#region BackgroundThreadPoolJob

	/// <summary>
	/// Defines a Job that will be executed by the BackgroundThreadPool
	/// </summary>
	public class BackgroundThreadPoolJob
	{		
		protected internal Guid _id;
		protected internal string _name;
		protected internal BackgroundThreadPoolJobStates _state;
		protected internal BackgroundThreadStartInfo _startInfo;
		protected internal DateTime _dateTimeStarted;
		protected internal DateTime _dateTimeStopped;
		protected internal bool _finished;
		protected internal bool _cancelled;

		/// <summary>
		/// Initializes a new instance of the BackgroundThreadPoolJob class
		/// </summary>
		/// <param name="startInfo">The start information regarding thread arguments and a callback method</param>
		public BackgroundThreadPoolJob(string name, BackgroundThreadStartInfo startInfo)
		{
			_id = Guid.NewGuid();
			_name = name;
			_startInfo = startInfo;
			_state = BackgroundThreadPoolJobStates.Waiting;
		}
		
		/// <summary>
		/// Initializes a new instance of the BackgroundThreadPoolJob class
		/// </summary>
		/// <param name="args">An array of objects passed to the thread when it is started</param>
		/// <param name="onRun">A callback method to be called by the thread</param>
		public BackgroundThreadPoolJob(string name, bool allowThreadAbortExceptions, object stateObject, object[] args, BackgroundThreadStartEventHandler onRun, BackgroundThreadEventHandler onFinished) : this(name, new BackgroundThreadStartInfo(allowThreadAbortExceptions, stateObject, args, onRun, onFinished))
		{
			
		}
		
		#region My Public Properties

		/// <summary>
		/// Returns the unique identifier for this job instance
		/// </summary>
		public Guid Id
		{
			get
			{
				return _id;
			}
		}

		/// <summary>
		/// Returns the Name of the Job
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}
				
		/// <summary>
		/// Returns the status of the job
		/// </summary>
		public BackgroundThreadPoolJobStates State
		{
			get
			{
				return _state;
			}
			//			set
			//			{
			//				_state = value;
			//			}
		}

		/// <summary>
		/// Returns the thread start information for this job instance
		/// </summary>
		public BackgroundThreadStartInfo StartInfo
		{
			get
			{
				return _startInfo;
			}
		}
		
		/// <summary>
		/// Gets or sets the date and time that this job was started
		/// </summary>
		public DateTime DateTimeStarted
		{
			get
			{
				return _dateTimeStarted;
			}
			//			set
			//			{
			//				_dateTimeStarted = value;
			//			}
		}
		
		/// <summary>
		/// Gets or sets the date and time that this job was stopped
		/// </summary>
		public DateTime DateTimeStopped
		{
			get
			{
				return _dateTimeStopped;
			}
			//			set
			//			{
			//				_dateTimeStopped = value;
			//			}
		}
		
		/// <summary>
		/// Gets or sets a flag indicating whether the job was finished
		/// </summary>
		public bool Finished
		{
			get
			{
				return _finished;
			}
			//			set
			//			{
			//				_finished = value;
			//			}
		}
		
		/// <summary>
		/// Gets or sets a flag indicating whether the job was cancelled
		/// </summary>
		public bool Cancelled
		{
			get
			{
				return _cancelled;
			}
			//			set
			//			{
			//				_cancelled = value;				
			//			}
		}

		#endregion
	}
	
	#endregion

}
