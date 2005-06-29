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
	/// Utility class of static methods for saving and loading a search to the file system as xml files
	/// </summary>
	public class Searching
	{
		/// <summary>
		/// loads a search from a file
		/// </summary>
		/// <param name="search">the search object to load with the serialized data</param>
		/// <param name="filename">the filename containing the serialized data to load</param>
		public static void Load(ref Search search, string filename)
		{
			// use the SerializationProvider to deserialize the object from the file
			search = (Search)SerializationProvider.Deserialize(FormatterTypes.Soap, filename);
		}

		/// <summary>
		/// saves a search to a file
		/// </summary>
		/// <param name="search">the search object to serialize to the file</param>
		/// <param name="filename">the filename to save the search to</param>
		public static void Save(Search search, string filename)
		{
			// use the SerializationProvider to serialize the object to the file
			SerializationProvider.Serialize(search, FormatterTypes.Soap, filename);
		}

		/// <summary>
		/// determines whether a file or folder is hidden 
		/// </summary>
		/// <param name="fileOrFolder"></param>
		/// <returns></returns>
		public static bool IsFileOrFolderHidden(object target)
		{	
			// if it is a System.IO.FileInfo object
			if (target is FileInfo)
				if (( ((FileInfo)target).Attributes & System.IO.FileAttributes.Hidden) == System.IO.FileAttributes.Hidden)
					return true;
			
			// if it is a System.IO.DirectoryInfo object
			if (target is DirectoryInfo)
				if (( ((DirectoryInfo)target).Attributes & System.IO.FileAttributes.Hidden) == System.IO.FileAttributes.Hidden)
					return true;
			
			return false;								   
		}
	}
}
