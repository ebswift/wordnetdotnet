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
using System.Windows.Forms;

namespace Razor.Configuration
{
	/// <summary>
	/// Summary description for PathCreationEngineEventArgs.
	/// </summary>
	public class PathCreationEngineEventArgs : System.EventArgs 
	{
		private object _userData;
		private string _path;
		private DialogResult _result;
		private System.Exception _systemException;
				
		public PathCreationEngineEventArgs(string path)
		{			
			_path = path;
		}

		public PathCreationEngineEventArgs(string path, DialogResult result)
		{
			_path = path;
			_result = result;
		}

		public object UserData
		{
			get
			{
				return _userData;
			}
			set
			{
				_userData = value;
			}
		}

		public string Path
		{
			get
			{
				return _path;
			}
			set
			{
				_path = value;
			}
		}

		public DialogResult Result
		{
			get
			{
				return _result;
			}
			set
			{
				_result = value;
			}
		}

		public System.Exception Exception
		{
			get
			{
				return _systemException;
			}
			set
			{
				_systemException = value;
			}
		}
	}

	public delegate void PathCreationEngineEventHandler(object sender, PathCreationEngineEventArgs e);
}
