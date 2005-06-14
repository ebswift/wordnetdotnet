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
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

namespace Razor
{
	/// <summary>
	/// Summary description for ShellImageListManager.
	/// </summary>
	public class ShellImageListManager
	{
		protected Hashtable _extensions;
		protected IconSizes _size;
		protected IconStyles _style;

		/// <summary>
		/// Initializes a new instance of the ShellImageListManager class
		/// </summary>
		public ShellImageListManager()
		{
			_extensions = new Hashtable();
			_size = IconSizes.ShellIconSize;
			_style = IconStyles.NormalIconStyle;
		}	

		#region Virtual Methods

		public virtual int GetIconIndex(ImageList imageList, string path)
		{
			string extension;
			bool useNormalAttribs = false;
			System.IO.FileAttributes attributes = System.IO.FileAttributes.Normal;

			try
			{
				attributes = File.GetAttributes(path);

				if ((attributes & System.IO.FileAttributes.Directory) == System.IO.FileAttributes.Directory)
				{
					DirectoryInfo directory = new DirectoryInfo(path);
					extension = directory.Extension;
					if (extension == null || extension == string.Empty)
						extension = @"Folder";
				}
				else
				{
					FileInfo file = new FileInfo(path);
					// be carefull on these certain extensions! they will prolly contain different icons
					switch(file.Extension)
					{
					case ".exe": extension = file.FullName;	break;					
					case ".lnk": extension = file.FullName;	break;					
					case ".url": extension = file.FullName;	break;	
					case ".ico": extension = file.FullName;	break;	
					case ".cur": extension = file.FullName;	break;	
					case ".ani": extension = file.FullName;	break;	
					case ".msc": extension = file.FullName;	break;	
					default: extension = file.Extension; break;
					}
				}
			}
			catch
			{
				extension = path;
				useNormalAttribs = true;
			}

			if (_extensions.ContainsKey(extension))
				return (int)_extensions[extension];

			int index = imageList.Images.Count;

			FileAttributes attribs = FileAttributes.Normal;
			if (!useNormalAttribs)
				attribs = ((attributes & FileAttributes.Directory) == FileAttributes.Directory ? FileAttributes.Directory : FileAttributes.Normal);
			
			imageList.Images.Add(ShellInformation.GetImageFromPath(path, _size, _style, attribs));

			if (!_extensions.ContainsKey(extension))
				_extensions.Add(extension, index);

			return index;
		}

		public virtual int GetIconIndex(ImageList imageList, string path, bool extractNew)
		{
			string extension;
			bool useNormalAttribs = false;
			System.IO.FileAttributes attributes = System.IO.FileAttributes.Normal;

			try
			{
				attributes = File.GetAttributes(path);

				if ((attributes & System.IO.FileAttributes.Directory) == System.IO.FileAttributes.Directory)
				{
					DirectoryInfo directory = new DirectoryInfo(path);
					extension = directory.Extension;
					if (extension == null || extension == string.Empty)
						extension = @"Folder";
				}
				else
				{
					FileInfo file = new FileInfo(path);
					// be carefull on these certain extensions! they will prolly contain different icons
					switch(file.Extension)
					{
					case ".exe": extension = file.FullName;	break;					
					case ".lnk": extension = file.FullName;	break;					
					case ".url": extension = file.FullName;	break;	
					case ".ico": extension = file.FullName;	break;	
					case ".cur": extension = file.FullName;	break;	
					case ".ani": extension = file.FullName;	break;	
					case ".msc": extension = file.FullName;	break;	
					default: extension = file.Extension; break;
					}
				}
			}
			catch
			{
				extension = path;
				useNormalAttribs = true;
			}

			if (!extractNew)
				if (_extensions.ContainsKey(extension))
					return (int)_extensions[extension];

			int index = imageList.Images.Count;

			FileAttributes attribs = FileAttributes.Normal;
			if (!useNormalAttribs)
				attribs = ((attributes & FileAttributes.Directory) == FileAttributes.Directory ? FileAttributes.Directory : FileAttributes.Normal);

			imageList.Images.Add(ShellInformation.GetIconFromPath(path, _size, _style, attribs));

			if (!_extensions.ContainsKey(extension))
				_extensions.Add(extension, index);

			return index;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the size of the icon extracted for a given path
		/// </summary>
		public IconSizes Size
		{
			get
			{
				return _size;
			}
			set
			{
				_size = value;
			}
		}

		/// <summary>
		/// Gets or sets the style of the icon extracted for a give path
		/// </summary>
		public IconStyles Style
		{
			get
			{
				return _style;
			}
			set
			{
				_style = value;
			}
		}

		#endregion
	}

	/// <summary>
	/// Summary description for SmallShellImageListManager.
	/// </summary>
	public class SmallShellImageListManager : ShellImageListManager
	{
		/// <summary>
		/// Initializes a new instance of the XXX class
		/// </summary>
		public SmallShellImageListManager() : base()
		{
			_size = IconSizes.SmallIconSize;
		}		
	}

	/// <summary>
	/// Summary description for SmallShellImageListManager.
	/// </summary>
	public class LargeShellImageListManager : ShellImageListManager
	{
		/// <summary>
		/// Initializes a new instance of the XXX class
		/// </summary>
		public LargeShellImageListManager() : base()
		{
			_size = IconSizes.LargeIconSize;
		}	
	}
}
