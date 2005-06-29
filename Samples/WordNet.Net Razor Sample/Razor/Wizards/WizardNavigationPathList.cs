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
using System.Collections;

namespace Razor.Wizards
{
	/// <summary>
	/// Summary description for WizardNavigationPathList.
	/// </summary>
	public class WizardNavigationPathList : CollectionBase
	{
		private bool _pathsLocked;

		/// <summary>
		/// Initializes a new instance of the WizardNavigationPathList class
		/// </summary>
		public WizardNavigationPathList()
		{
			
		}

		/// <summary>
		/// Adds a path to the path list
		/// </summary>
		/// <param name="path">The path to add</param>
		public void Add(WizardNavigationPath path)
		{
			if (this.Contains(path))
				throw new ArgumentOutOfRangeException("WizardNavigationPath path", "The path list already contains a path with the same destination");

			path.PathSelected += new WizardNavigationPathEventHandler(OnPathSelected);
			base.InnerList.Add(path);
		}

		/// <summary>
		/// Removes a path from the path list
		/// </summary>
		/// <param name="path">The path to remove</param>
		public void Remove(WizardNavigationPath path)
		{
			if (this.Contains(path))
			{
				path.PathSelected -= new WizardNavigationPathEventHandler(OnPathSelected);
				base.InnerList.Remove(path);
			}
		}

		/// <summary>
		/// Determines if a path already exists by examining the destination locations of existing paths to the destination location of the specified path
		/// </summary>
		/// <param name="path">The path to examine and determine if it already exists</param>
		/// <returns></returns>
		public bool Contains(WizardNavigationPath path)
		{			
			if (path == null)
				throw new ArgumentNullException("WizardNavigationPath path", "The path cannot be null");

			foreach(WizardNavigationPath existingPath in base.InnerList)
				if (string.Compare(existingPath.Name, path.Name, true) == 0)
					return true;				
			return false;
		}		

		/// <summary>
		/// Determines if a path already exists
		/// </summary>
		/// <param name="pathName"></param>
		/// <returns></returns>
		public bool Contains(string pathName)
		{
			foreach(WizardNavigationPath existingPath in base.InnerList)
				if (string.Compare(existingPath.Name, pathName, true) == 0)
					return true;				
			return false;
		}

		/// <summary>
		/// Accesses the path in the list that contains the specified name, or null if no path by that name exists.
		/// </summary>
		public WizardNavigationPath this[string pathName]
		{
			get
			{
				foreach(WizardNavigationPath existingPath in base.InnerList)
					if (string.Compare(existingPath.Name, pathName, true) == 0)
						return existingPath;
				return null;
			}
		}

		/// <summary>
		/// Evaluates the paths contained in the list and returns the destination location from the selected path
		/// </summary>
		/// <returns></returns>
		public WizardNavigationLocation FindSelectedDestination()
		{
			foreach(WizardNavigationPath path in base.InnerList)
				if (path.Selected)
					return path.Destination;

			return null;
		}

		/// <summary>
		/// Deselects all paths in the list
		/// </summary>
		public void DeselectAllPaths()
		{
			_pathsLocked = true;

			// de-select the remaining paths, only one path may be selected at a time
			foreach(WizardNavigationPath existingPath in base.InnerList)				
					// make sure the path is not selected
					existingPath.Selected = false;

			_pathsLocked = false;
		}

		/// <summary>
		/// Tries to select the path with the specified name (fails if no path exists by that name)
		/// </summary>
		/// <param name="pathName">The path to select</param>
		/// <returns></returns>
		public bool TryAndSelectPath(string pathName)
		{
			bool selected = false;
			foreach(WizardNavigationPath path in base.InnerList)	
				if (string.Compare(path.Name, pathName, true) == 0)
				{
					path.Selected = true;
					selected = true;
					break;
				}

			Debug.WriteLine(string.Format("The path named '{0}' was {1} selected.", pathName, (selected ? "successfully" : "unsuccessfully")));

			return selected;
		}

		/// <summary>
		/// Returns the names of the paths in this list
		/// </summary>
		/// <returns></returns>
		public string[] GetPathNames()
		{
			string[] names = new string[base.InnerList.Count];
			for(int i = 0; i < base.InnerList.Count; i++)
				names[i] = ((WizardNavigationPath)base.InnerList[i]).Name;
			return names;
		}

		/// <summary>
		/// Occurs when a path in the path list is selected
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPathSelected(object sender, WizardNavigationPathEventArgs e)
		{
			if (_pathsLocked)
				return;
			
			// de-select the remaining paths, only one path may be selected at a time
			foreach(WizardNavigationPath existingPath in base.InnerList)
				// if the path is not the one that has been selected, then de-select it
				if (string.Compare(existingPath.Name, e.Path.Name, true) == 0)
					continue;
				else
					// make sure the path is not selected
					existingPath.Selected = false;
		}
	}
}
