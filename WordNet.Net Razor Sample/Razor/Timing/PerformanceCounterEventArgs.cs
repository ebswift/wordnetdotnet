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

namespace Razor.Timing
{
	/// <summary>
	/// Summary description for PerformanceCounterEventArgs.
	/// </summary>	
	public class PerformanceCounterEventArgs: System.EventArgs
	{
		private float _seconds = 0;

		/// <summary>
		/// Initializes a new instance of the PerformanceCounterEventArgs class
		/// </summary>
		/// <param name="fSeconds"></param>
		public PerformanceCounterEventArgs(float seconds)
		{
			_seconds = seconds; 
		}
		
		/// <summary>
		/// Returns the number of seconds that have elapsed
		/// </summary>
		public float SecondsElapsed
		{
			get
			{
				return _seconds;
			}
		}
	}

	public delegate void PerformanceCounterEventHandler(object sender, PerformanceCounterEventArgs e);
}
