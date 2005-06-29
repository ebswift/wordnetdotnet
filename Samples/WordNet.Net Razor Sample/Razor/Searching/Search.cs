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
using System.Runtime.Serialization;
using System.Diagnostics;

namespace Razor.Searching
{
	/// <summary>
	/// Provides properties, methods, and events for searching for files on a Windows PC.
	/// </summary>
	[Serializable()]
	public class Search: System.Runtime.Serialization.ISerializable 
	{
		private string _text;
		private string _searchPath;
		private string _searchPattern;
		private bool _recursive;
		private bool _searchHiddenFilesAndFolders;
		private System.IO.FileInfo[] _searchResults;

		/// <summary>
		/// This event is fired for each file that is found that met the search criteria.
		/// </summary>
		public event SearchEventHandler FileFound;

		/// <summary>
		/// Initializes a new instance of the Search class
		/// </summary>
		public Search()
		{
			SetDefaultValues(null, null);
			_text = null;
			//			_searchPath = null;
			//			_searchPattern = "";
			_recursive = false;
			_searchHiddenFilesAndFolders = false;
			_searchResults = null;			
		}

		/// <summary>
		/// Initializes a new instance of the Search class
		/// </summary>
		/// <param name="searchPath">the path to search</param>
		/// <param name="searchPattern">the pattern of files to search for</param>
		public Search(string text,string searchPath, string searchPattern, bool recursive, bool searchHiddenFilesAndFolders)
		{
			SetDefaultValues(searchPath, searchPattern);
			_text = text;
			//			_searchPath = searchPath.TrimEnd(System.IO.Path.InvalidPathChars);
			//			_searchPattern = searchPattern;
			_recursive = recursive;
			_searchHiddenFilesAndFolders = searchHiddenFilesAndFolders;
			_searchResults = null;			
		}
				
		/// <summary>
		/// Applies default values to the path and pattern if they are invalid.
		/// </summary>
		private void SetDefaultValues(string searchPath, string searchPattern)
		{
			if ((searchPath == null) || (searchPath == String.Empty))
				_searchPath = System.Windows.Forms.Application.StartupPath;
				//				_searchPath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
			else
				_searchPath = searchPath.TrimEnd(System.IO.Path.InvalidPathChars);

			if ((searchPattern == null) || (searchPattern == String.Empty))
				_searchPattern = "*.dll";
			else
				_searchPattern = searchPattern;
		}
		/// <summary>
		/// Gets or sets the text for this search
		/// </summary>
		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
			}
		}

		/// <summary>
		/// Gets or sets the path to search
		/// </summary>
		public string SearchPath
		{
			get
			{
				return _searchPath;
			}
			set
			{				
				SetDefaultValues(value, _searchPattern);
			}
		}

		/// <summary>
		/// Gets or sets the pattern of files to search for
		/// </summary>
		public string SearchPattern
		{
			get
			{
				return _searchPattern;
			}
			set
			{				
				SetDefaultValues(_searchPath, value);
			}
		}
	
		/// <summary>
		/// Gets or sets whether the search is recursive
		/// </summary>
		public bool Recursive
		{
			get
			{
				return _recursive;
			}
			set 
			{
				_recursive = value;
			}
		}

		/// <summary>
		/// Gets or sets whether the search should include hidden files or folders
		/// </summary>
		public bool SearchHiddenFilesAndFolders
		{
			get
			{
				return _searchHiddenFilesAndFolders;
			}
			set
			{
				_searchHiddenFilesAndFolders = value;
			}
		}

		/// <summary>
		/// Gets the results of the last search. Only valid when calling the GetFiles method that does not return a void.
		/// </summary>
		public System.IO.FileInfo[] SearchResults
		{
			get
			{
				return _searchResults;
			}
		}

		/// <summary>
		/// Gets a FileInfo[] of files that can be found according to the search path and pattern of this Search instance
		/// </summary>
		/// <returns></returns>
		public System.IO.FileInfo[] GetFiles()
		{		
			// call the internal get files method and cache the results
			ArrayList searchResults = this.GetFiles(_searchPath, _searchPattern, _recursive, _searchHiddenFilesAndFolders);
			
			// 
			if (searchResults != null)
			{
				_searchResults = new FileInfo[searchResults.Count];
				// copy the results back to a strongly typed array 
				searchResults.CopyTo(_searchResults);
			}
			else
				_searchResults = new FileInfo[0];

			// return the results of the search
			return _searchResults;			
		}	
		
		public DirectoryInfo[] GetDirectories()
		{
			// get directory info for the path
			System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(_searchPath);

			// bail if the directory doesn't exist
			if (!directory.Exists)
				return null;

			return this.GetDirectories(directory, _searchPattern, _searchHiddenFilesAndFolders);
		}

		private DirectoryInfo[] GetDirectories(DirectoryInfo searchDirectory, string searchPattern, bool searchHiddenFilesAndFolders)
		{
			ArrayList searchResults = new ArrayList();

			// split the search patterns
			string[] searchPatterns = searchPattern.Split(new char[] {';','|'});

			// get files for each pattern
			foreach(string pattern in searchPatterns)
			{	
				if (pattern == null)
					continue;				

				// get the files that match the pattern	
				DirectoryInfo[] directories = searchDirectory.GetDirectories(pattern);

				foreach(DirectoryInfo directory in directories)
				{			
					// skip the file if it is hidden and we aren't searching hidden files or folders
					if (Searching.IsFileOrFolderHidden(directory) && !searchHiddenFilesAndFolders)
						continue;

					// add this file
					searchResults.Add(directory);
					
//					OnFileFound(this, new SearchEventArgs(directory));
				}
			}

			return searchResults.ToArray(typeof(DirectoryInfo)) as DirectoryInfo[];
		}

		/// <summary>
		/// gets the files from a path that matches a search pattern
		/// </summary>
		/// <param name="directory"></param>
		/// <param name="searchPattern"></param>
		/// <returns></returns>
		private ArrayList GetFiles(System.IO.DirectoryInfo directory, string searchPattern, bool searchHiddenFilesAndFolders)
		{
			ArrayList searchResults = new ArrayList();

			// split the search patterns
			string[] searchPatterns = searchPattern.Split(new char[] {';','|'});

			// get files for each pattern
			foreach(string pattern in searchPatterns)
			{	
				if (pattern == null)
					continue;				

				// get the files that match the pattern	
				FileInfo[] files = directory.GetFiles(pattern);

				foreach(FileInfo file in files)
				{			
					// skip the file if it is hidden and we aren't searching hidden files or folders
					if (Searching.IsFileOrFolderHidden(file) && !searchHiddenFilesAndFolders)
						continue;

					// add this file
					searchResults.Add(file);
					
					OnFileFound(this, new SearchEventArgs(file));
				}
			}

			return searchResults;
		}

		/// <summary>
		/// gets files from a path that match a search pattern 
		/// </summary>
		/// <param name="path"></param>
		/// <param name="extension"></param>
		/// <returns></returns>
		private ArrayList GetFiles(string searchPath, string searchPattern, bool recursive, bool searchHiddenFilesAndFolders)
		{
			ArrayList searchResults = new ArrayList();

			// get directory info for the path
			System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(searchPath);

			// bail if the directory doesn't exist
			if (!directory.Exists)
				return null;		

			// search for the files
			ArrayList fileSearchResults = this.GetFiles(directory, searchPattern, searchHiddenFilesAndFolders);
			
			// save them if any were found
			if (fileSearchResults != null)
				searchResults.AddRange(fileSearchResults);
			
			fileSearchResults = null;
				
			// go recursive if the search requires it
			if (recursive)
			{
				// get all of the directories in the current directory
				DirectoryInfo[] directories = directory.GetDirectories();

				// walk thru each file in the results
				foreach (DirectoryInfo subDirectory in directories)
				{
					if (Searching.IsFileOrFolderHidden(subDirectory) && !searchHiddenFilesAndFolders)
						continue;

					// call ourself recursively
					ArrayList recursiveSearchResults = this.GetFiles( System.IO.Path.Combine(searchPath, subDirectory.Name), searchPattern, recursive, searchHiddenFilesAndFolders);

					// add the recursive search results to the overall search results
					if (recursiveSearchResults != null)
						searchResults.AddRange(recursiveSearchResults);

					recursiveSearchResults = null;
				}
			}		

			// return all of the files found
			return searchResults;
		}
		
		/// <summary>
		/// Executes a search using the path, pattern, and recursion
		/// </summary>
		public void FindFiles()
		{
			this.FindFiles(_searchPath, _searchPattern, _recursive, _searchHiddenFilesAndFolders);
		}

		/// <summary>
		/// Internal method for executing a resultless search
		/// </summary>
		/// <param name="directory"></param>
		/// <param name="searchPattern"></param>
		/// <param name="searchHiddenFilesAndFolders"></param>
		private void FindFiles(System.IO.DirectoryInfo directory, string searchPattern, bool searchHiddenFilesAndFolders)
		{
			string[] searchPatterns = searchPattern.Split(new char[] {';', '|'});

			foreach(string pattern in searchPatterns)
			{
				if (pattern == null)
					continue;

				FileInfo[] files = directory.GetFiles(pattern);

				foreach(FileInfo file in files)
				{
					if (Searching.IsFileOrFolderHidden(file) && !searchHiddenFilesAndFolders)
						continue;

					this.OnFileFound(this, new SearchEventArgs(file));
				}
			}
		}
		
		/// <summary>
		/// Internal methods for executing a resultless search
		/// </summary>
		/// <param name="searchPath"></param>
		/// <param name="searchPattern"></param>
		/// <param name="recursive"></param>
		/// <param name="searchHiddenFilesAndFolders"></param>
		private void FindFiles(string searchPath, string searchPattern, bool recursive, bool searchHiddenFilesAndFolders)
		{
			System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(searchPath);

			if (!directory.Exists)
				return;

			this.FindFiles(directory, searchPattern, searchHiddenFilesAndFolders);

			if (recursive)
			{
				DirectoryInfo[] directories = directory.GetDirectories();

				foreach(DirectoryInfo subDirectory in directories)
				{
					if (Searching.IsFileOrFolderHidden(subDirectory) && !searchHiddenFilesAndFolders)
						continue;

					this.FindFiles(System.IO.Path.Combine(searchPath, subDirectory.Name), searchPattern, recursive, searchHiddenFilesAndFolders);					
				}
			}
		}

		/// <summary>
		/// Raises the FileFound event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnFileFound(object sender, SearchEventArgs e)
		{
			try
			{
				if (this.FileFound != null)
					this.FileFound(sender, e);
			}
			catch (System.Exception systemException) 
			{
				Trace.WriteLine(systemException); 
			}	
		}

		#region Implementation of ISerializable
		/// <summary>
		/// serialization
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public Search(SerializationInfo info, StreamingContext context)
		{
			_text = info.GetString("Text");
			_searchPath = info.GetString("SearchPath");
			_searchPattern = info.GetString("SearchPattern");
			_recursive = info.GetBoolean("Recursive");
			_searchResults = (FileInfo[])info.GetValue("SearchResults", typeof(FileInfo[]));
		}

		/// <summary>
		/// deserialization
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue("Text", _text, typeof(string));
			info.AddValue("SearchPath", _searchPath, typeof(string));
			info.AddValue("SearchPattern", _searchPattern, typeof(string));
			info.AddValue("Recursive", _recursive, typeof(bool));	
			info.AddValue("SearchResults", _searchResults, typeof(FileInfo[]));
		}
		#endregion
	}
}
