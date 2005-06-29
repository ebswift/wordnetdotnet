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
using System.Diagnostics;

namespace Razor.Configuration
{
	/// <summary>
	/// Summary description for EventTracing.
	/// </summary>
	public class EventTracing
	{
		private static bool _enabled = false;

		public static bool Enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				_enabled = value;
			}
		}

		public static void TraceMethod(object caller)
		{
			if (!_enabled)
				return;

			try
			{
				StackFrame f = new StackFrame(1);
				string message = string.Format("{0}.{1} called at {2}", f.GetMethod().DeclaringType.Name, f.GetMethod().Name, DateTime.Now.ToLongTimeString());
				System.Diagnostics.Trace.WriteLine(message);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		public static void TraceMethodAndDelegate(object sender, System.Delegate eventHandler)
		{
			if (!_enabled)
				return;

			try
			{
				StackFrame f = new StackFrame(1);
				Delegate[] eventHandlers = (eventHandler != null ? eventHandler.GetInvocationList() : new Delegate[] {});
				
				// string target = (eventHandler != null ? eventHandlers[0].Target.GetType().Name : <

				string message = string.Format("{0}.{1} called, preparing to call {3} delegates.", f.GetMethod().DeclaringType.Name, f.GetMethod().Name, DateTime.Now.ToLongTimeString(), eventHandlers.Length.ToString());
				System.Diagnostics.Trace.WriteLine(message);

				//	eventHandler.Target.GetType().Name 
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}
	}
}
