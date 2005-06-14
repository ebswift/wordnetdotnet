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
	#region BackgroundThreadPoolJobStates

	/// <summary>
	/// Defines the various states that a ThreadPoolJob can exist in during its lifetime
	/// </summary>
	public enum BackgroundThreadPoolJobStates
	{
		/// <summary>
		/// The job is waiting to be serviced
		/// </summary>
		Waiting,

		/// <summary>
		/// The job is running
		/// </summary>
		Running,

		/// <summary>
		/// The job is finished
		/// </summary>
		Finished,

		/// <summary>
		/// The job has been cancelled
		/// </summary>
		Cancelled
	}

	#endregion

}
