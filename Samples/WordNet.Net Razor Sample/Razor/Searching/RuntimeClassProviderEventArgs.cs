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

namespace Razor.Searching
{
	/// <summary>
	/// Summary description for RuntimeClassProviderEventArgs.
	/// </summary>
	public class RuntimeClassProviderEventArgs: System.EventArgs 
	{
		private System.Reflection.Assembly _assembly;
		private System.Type _type;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="assembly">the Assembly containing the Type</param>
		/// <param name="type">a Type contained in the Assembly</param>
		public RuntimeClassProviderEventArgs(System.Reflection.Assembly assembly, System.Type type)
		{
			_assembly = assembly;
			_type = type;
		}

		/// <summary>
		/// The assembly housing the type exporting the public classes 
		/// </summary>
		public System.Reflection.Assembly Assembly
		{
			get
			{
				return _assembly;
			}
		}

		public System.Type Type
		{
			get
			{
				return _type;
			}
		}
	}

	public delegate void RuntimeClassProviderEventHandler(object sender,  RuntimeClassProviderEventArgs e);
}
