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

namespace Razor.Wizards
{
	/// <summary>
	/// Summary description for WizardNavigationPath.
	/// </summary>
	public class WizardNavigationPath
	{
		protected string _name;
		protected bool _selected;
		protected WizardNavigationLocation _destination;
		public event WizardNavigationPathEventHandler PathSelected;

		/// <summary>
		/// Initializes a new instance of the WizardNavigationPath class
		/// </summary>
		/// <param name="destination">A destination location to associate with this path</param>
		public WizardNavigationPath(WizardNavigationLocation destination, string name, bool selected)
		{
			_destination = destination;			
			_name = name;
			_selected = selected;
		}

		/// <summary>
		/// Returns the destination location for this path (to be used by the Wizard when this path is selected during navigation)
		/// </summary>
		public WizardNavigationLocation Destination
		{
			get
			{
				return _destination;
			}
		}

		/// <summary>
		/// Gets or sets whether this path is the selected path (Only one path may be selected at a time in a path list, the list will manage this effect, you need only to set a single path selected in a path list, the rest will be de-selected)
		/// </summary>
		public bool Selected
		{
			get
			{
				return _selected;
			}
			set
			{
				_selected = value;

				// if this path is selected
				if (_selected)
				{				
					// trigger the event to notify the path list in which we are contained
					// this will allow the list to assert that one and only one path can be selected at a time
					this.OnPathSelected(this, new WizardNavigationPathEventArgs(this));
				}
			}
		}

		/// <summary>
		/// Returns the name of the path
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Raises the PathSelected event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnPathSelected(object sender, WizardNavigationPathEventArgs e)
		{
			try
			{
				if (this.PathSelected != null)
					this.PathSelected(sender, e);
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}
	}

	public class WizardNavigationPathEventArgs : EventArgs
	{
		protected WizardNavigationPath _path;

		public WizardNavigationPathEventArgs(WizardNavigationPath path) : base()
		{
			_path = path;
		}

		public WizardNavigationPath Path
		{
			get
			{
				return _path;
			}
		}
	}

	public delegate void WizardNavigationPathEventHandler(object sender, WizardNavigationPathEventArgs e);
}
