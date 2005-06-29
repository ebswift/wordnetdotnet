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
using System.Runtime.InteropServices;

namespace Razor.Timing
{	
	/// <summary>
	/// Summary description for PerformanceCounter.
	/// </summary>	
	public class PerformanceCounter
	{
		[DllImport("Kernel32.dll")]
		private static extern bool QueryPerformanceCounter(ref long lpPerformanceCount);
		[DllImport("Kernel32.dll")]
		private static extern bool QueryPerformanceFrequency(ref long lpFrequency);  
		
		private long _startCount = 0;
		
		public event PerformanceCounterEventHandler CounterStopped;
		
		/// <summary>
		/// Initializes a new instance of the PerformanceCounter class
		/// </summary>
		public PerformanceCounter()
		{

		}
			
		/// <summary>
		/// Initializes a new instance of the PerformanceCounter class
		/// </summary>
		/// <param name="autoStart">A flag that indicates whether the counter should start automatically</param>
		/// <param name="onCounterStopped">A callback to be called when the counter stops</param>
		public PerformanceCounter(bool autoStart, PerformanceCounterEventHandler onCounterStopped)
		{
			this.CounterStopped += onCounterStopped;

			if (autoStart)
				this.Start();
		}

		/// <summary>
		/// Starts the counter
		/// </summary>
		public void Start()
		{
			QueryPerformanceCounter(ref _startCount);
		}

		/// <summary>
		/// Stops the counter
		/// </summary>
		public void Stop()
		{			
			this.OnCounterStopped(this, new PerformanceCounterEventArgs(this.SecondsElapsed));
		}

		/// <summary>
		/// Clears the counter without stopping it
		/// </summary>
		public void Reset()
		{
			QueryPerformanceCounter(ref _startCount);
		}

		/// <summary>
		/// Returns the number of seconds that have elapsed ((value - startcount) / frequency)
		/// </summary>
		public float SecondsElapsed
		{
			get
			{				
				return ((float)(this.Value - _startCount)/(float)this.Frequency);
			}
		}

		/// <summary>
		/// Returns the frequency of the underlying performance counter
		/// </summary>
		private long Frequency 
		{
			get 
			{
				long frequency = 0;
				QueryPerformanceFrequency(ref frequency);
				return frequency;
			}
		}

		/// <summary>
		/// Returns the value of the underlying performance counter
		/// </summary>
		private long Value
		{
			get
			{
				long value = 0;
				QueryPerformanceCounter(ref value);
				return value;
			}
		}

		/// <summary>
		/// Raises the CounterStopped event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnCounterStopped(object sender, PerformanceCounterEventArgs e)
		{
			try
			{
				if (this.CounterStopped != null)
					this.CounterStopped(sender, e);
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		public override string ToString()
		{
			return String.Format("{0} Seconds", this.SecondsElapsed);
		}
	}
}
