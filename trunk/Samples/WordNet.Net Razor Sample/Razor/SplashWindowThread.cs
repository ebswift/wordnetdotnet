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
using System.Reflection;
using System.Windows.Forms;

namespace Razor
{
	/// <summary>
	/// Summary description for SplashWindowThread.
	/// </summary>
	public class SplashWindowThread : WindowThread
	{
		private Assembly _assembly;
		private bool _showVersion;

		public SplashWindowThread(Assembly assembly) : base()
		{
			_assembly = assembly;
			_showVersion = true;
		}

		public SplashWindowThread(Assembly assembly, bool showVersion) : this(assembly)
		{
			_showVersion = showVersion;
		}

		public new SplashWindow Window
		{
			get
			{
				return this._window as SplashWindow;
			}
		}

		protected override void GetWindowType(out Form window)
		{
			window = new SplashWindow(_assembly, _showVersion);
		}

	}
}
