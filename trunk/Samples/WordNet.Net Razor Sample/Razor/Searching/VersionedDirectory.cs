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
using System.IO;

namespace Razor.Searching
{
	/// <summary>
	/// Summary description for VersionedDirectory.
	/// </summary>
	public class VersionedDirectory
	{
		private Version _version;
		private DirectoryInfo _directory;

		public VersionedDirectory(Version version, DirectoryInfo directory)
		{
			_version = version;
			_directory = directory;
		}

		public Version Version
		{
			get
			{
				return _version;
			}
		}

		public DirectoryInfo Directory 
		{
			get	
			{
				return _directory;
			}
		}

		public static VersionedDirectory[] Sort(VersionedDirectory[] directories)
		{
			// front to back - 1 
			for(int i = 0; i < directories.Length - 1; i++)
			{
				// front + 1 to back
				for(int j = i + 1; j < directories.Length; j++)
				{			
					if (directories[i].Version < directories[j].Version)
					{											 
						// swap i with j, where i=1 and j=2
						VersionedDirectory directory = directories[j];
						directories[j] = directories[i];
						directories[i] = directory;
					}													
				}
			}
			return directories;
		}
	}
}
