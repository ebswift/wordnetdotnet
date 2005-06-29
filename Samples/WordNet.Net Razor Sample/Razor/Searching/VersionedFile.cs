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
using System.Collections;

namespace Razor.Searching
{
	/// <summary>
	/// Summary description for VersionedFile.
	/// </summary>
	public class VersionedFile
	{
		private Version _version;
		private FileInfo _file;

		public VersionedFile(Version version, FileInfo file)
		{
			_version = version;
			_file = file;
		}

		public Version Version
		{
			get
			{
				return _version;
			}
		}

		public FileInfo File 
		{
			get	
			{
				return _file;
			}
		}

		public static VersionedFile[] Sort(VersionedFile[] files)
		{
			// front to back - 1 
			for(int i = 0; i < files.Length - 1; i++)
			{
				// front + 1 to back
				for(int j = i + 1; j < files.Length; j++)
				{			
					if (files[i].Version < files[j].Version)
					{											 
						// swap i with j, where i=1 and j=2
						VersionedFile file = files[j];
						files[j] = files[i];
						files[i] = file;
					}													
				}
			}
			return files;
		}

		/// <summary>
		/// Converts the array of files to an array of versioned files
		/// </summary>
		/// <param name="files"></param>
		/// <returns></returns>
		public static VersionedFile[] CreateVersionedFiles(FileInfo[] files)
		{			
			ArrayList array = new ArrayList();
			foreach(FileInfo file in files)
			{				
				try
				{
					string name = file.Name.Replace(file.Extension, null);
					Version version = new Version(name);
					VersionedFile vf = new VersionedFile(version, file);
					array.Add(vf);
				}
				catch{}
			}
			return array.ToArray(typeof(VersionedFile)) as VersionedFile[];
		}

		public static VersionedFile[] CreateVersionedFiles(string prependedTextToRemove, FileInfo[] files)
		{
			ArrayList array = new ArrayList();
			foreach(FileInfo file in files)
			{				
				try
				{
					// strip the file extention
					string name = file.Name.Replace(file.Extension, null);
					
					// remove prepended text here
					name = name.Replace(prependedTextToRemove, null);
					
					// create a version from the file name that's remaining
					Version version = new Version(name);
					VersionedFile vf = new VersionedFile(version, file);
					array.Add(vf);
				}
				catch{}
			}
			return array.ToArray(typeof(VersionedFile)) as VersionedFile[];
		}

		/// <summary>
		/// Returns the latest version available in the array of files
		/// </summary>
		/// <param name="versionedFiles"></param>
		/// <returns></returns>
		public static VersionedFile GetLatestVersion(VersionedFile[] versionedFiles)
		{
			if (versionedFiles != null)
			{ 
				if (versionedFiles.Length > 0)
				{
					return versionedFiles[0];
				}
			}
			return null;
		}
	}
}
