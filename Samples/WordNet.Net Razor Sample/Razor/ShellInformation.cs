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
using System.Runtime.InteropServices;

namespace Razor
{
	/// <summary>
	/// Defines the predefined sizes of Icon's extracted using SHGetFileInfo
	/// </summary>
	[Flags]
	public enum IconSizes
	{
		/// <summary>
		/// Extract the Icon as a large Icon (Usually defined as 32x32)
		/// </summary>
		LargeIconSize				= ShellInformation.SHGFI_LARGEICON,

		/// <summary>
		/// Extract the Icon as a small Icon (Usually defined as 16x16)
		/// </summary>
		SmallIconSize				= ShellInformation.SHGFI_SMALLICON,

		/// <summary>
		/// Extract the Icon at the predefined Shell size
		/// </summary>
		ShellIconSize				= 0x0004
	};
		
	/// <summary>
	/// Defines the predefined styles that can be applied to Icon's extracted using SHGetFileInfo
	/// </summary>
	[Flags]
	public enum IconStyles
	{
		/// <summary>
		/// Normal Icon
		/// </summary>
		NormalIconStyle				= ShellInformation.SHGFI_ICON,

		/// <summary>
		/// Includes the shortcut overlay
		/// </summary>
		LinkOverlayIconStyle		= ShellInformation.SHGFI_LINKOVERLAY,

		/// <summary>
		/// Applies a color matrix to create a selected look
		/// </summary>
		SelectedIconStyle			= ShellInformation.SHGFI_SELECTED,

		/// <summary>
		/// Gets the open version of the Icon, usually applied to folders that can display open and closed versions of their icons
		/// </summary>
		OpenIconStyle				= ShellInformation.SHGFI_OPENICON
	};
		
//	/// <summary>
//	/// 
//	/// </summary>
//	[Flags]
//	public enum FileAttributes
//	{
//		UnspecifiedFileAttribute	= 0x0000,
//		ReadOnlyFileAttribute		= 0x0001,
//		HiddenFileAttribute			= 0x0002,
//		SystemFileAttribute			= 0x0004,
//		DirectoryFileAttribute		= 0x0010,
//		ArchiveFileAttribute		= 0x0020,
//		NormalFileAttribute			= 0x0080,
//		TemporaryFileAttribute		= 0x0100,
//		CompressedFileAttribute		= 0x0800
//	};

	/// <summary>
	/// Contains information about a file object. 
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct SHFILEINFO 
	{
		public IntPtr hIcon;
		public int iIcon;
		public int dwAttributes;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string szDisplayName;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
		public string szTypeName;
	};

	/// <summary>
	/// Summary description for ShellInformation.
	/// </summary>
	public class ShellInformation
	{
		private const int MAX_PATH = 256;

		//		private const int BIF_RETURNONLYFSDIRS		=	0x0001;
		//		private const int BIF_DONTGOBELOWDOMAIN		=	0x0002;
		//		private const int BIF_STATUSTEXT			=	0x0004;
		//		private const int BIF_RETURNFSANCESTORS		=	0x0008;
		//		private const int BIF_EDITBOX				=	0x0010;
		//		private const int BIF_VALIDATE				=	0x0020;
		//		private const int BIF_NEWDIALOGSTYLE		=	0x0040;
		//		private const int BIF_USENEWUI				=	(BIF_NEWDIALOGSTYLE | BIF_EDITBOX);
		//		private const int BIF_BROWSEINCLUDEURLS		=	0x0080;
		//		private const int BIF_BROWSEFORCOMPUTER		=	0x1000;
		//		private const int BIF_BROWSEFORPRINTER		=	0x2000;
		//		private const int BIF_BROWSEINCLUDEFILES	=	0x4000;
		//		private const int BIF_SHAREABLE				=	0x8000;

		internal const int SHGFI_ICON				= 0x000000100;     // get icon
		internal const int SHGFI_DISPLAYNAME		= 0x000000200;     // get display name
		internal const int SHGFI_TYPENAME          	= 0x000000400;     // get type name
		internal const int SHGFI_ATTRIBUTES        	= 0x000000800;     // get attributes
		internal const int SHGFI_ICONLOCATION      	= 0x000001000;     // get icon location
		internal const int SHGFI_EXETYPE           	= 0x000002000;     // return exe type
		internal const int SHGFI_SYSICONINDEX      	= 0x000004000;     // get system icon index
		internal const int SHGFI_LINKOVERLAY       	= 0x000008000;     // put a link overlay on icon
		internal const int SHGFI_SELECTED          	= 0x000010000;     // show icon in selected state
		internal const int SHGFI_ATTR_SPECIFIED    	= 0x000020000;     // get only specified attributes
		internal const int SHGFI_LARGEICON         	= 0x000000000;     // get large icon
		internal const int SHGFI_SMALLICON         	= 0x000000001;     // get small icon
		internal const int SHGFI_OPENICON          	= 0x000000002;     // get open icon
		internal const int SHGFI_SHELLICONSIZE     	= 0x000000004;     // get shell size icon
		internal const int SHGFI_PIDL              	= 0x000000008;     // pszPath is a pidl
		internal const int SHGFI_USEFILEATTRIBUTES 	= 0x000000010;     // use passed dwFileAttribute
		internal const int SHGFI_ADDOVERLAYS       	= 0x000000020;     // apply the appropriate overlays
		internal const int SHGFI_OVERLAYINDEX      	= 0x000000040;     // Get the index of the overlay
	
		[DllImport("Shell32.dll")]
		private static extern IntPtr SHGetFileInfo(string pszPath, int dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);
		
		[DllImport("User32.dll")]
		private static extern int DestroyIcon(IntPtr hIcon);
		
		[DllImport("gdi32.dll")]
		private static extern int DeleteObject(IntPtr hObject);
		
		public static Icon GetIconFromPath(string path, IconSizes size, IconStyles style, FileAttributes attributes)
		{
			SHFILEINFO shfi = new SHFILEINFO();
			uint uFlags = SHGFI_ICON;
			
			uFlags |= (uint)size;
			uFlags |= (uint)style;
			if (attributes != 0) 
				uFlags |= SHGFI_USEFILEATTRIBUTES; 
			
			SHGetFileInfo(path,	(int)attributes, ref shfi, (uint)Marshal.SizeOf(shfi), uFlags);

			Icon icon = (Icon)Icon.FromHandle(shfi.hIcon).Clone();
			DeleteObject(shfi.hIcon);
			return icon;
		}

		/// <summary>
		/// Returns a Bitmap from a file system path
		/// </summary>
		/// <param name="path">The path can be a fully qualified path name, non-existent file, or file extension</param>
		/// <param name="size">The size of the bitmap to return</param>
		/// <param name="style">The style of the bitmap to retrieve</param>
		/// <param name="attributes">Extra attributes to specify for the path</param>
		/// <returns></returns>
		public static Bitmap GetBitmapFromPath(string path,	IconSizes size,	IconStyles style, FileAttributes attributes)
		{
			SHFILEINFO shfi = new SHFILEINFO();
			uint uFlags = SHGFI_ICON;
			
			uFlags |= (uint)size;
			uFlags |= (uint)style;
			if (attributes != 0)
				uFlags |= SHGFI_USEFILEATTRIBUTES; 
			
			SHGetFileInfo(path,	(int)attributes, ref shfi, (uint)Marshal.SizeOf(shfi), uFlags);
						
			Bitmap bitmap = (Bitmap)Bitmap.FromHicon(shfi.hIcon).Clone();			
			DeleteObject(shfi.hIcon);
			return bitmap;
		}

		/// <summary>
		/// Returns a Image from a file system path
		/// </summary>
		/// <param name="path">The path can be a fully qualified path name, non-existent file, or file extension</param>
		/// <param name="size">The size of the bitmap to return</param>
		/// <param name="style">The style of the bitmap to retrieve</param>
		/// <param name="attributes">Extra attributes to specify for the path</param>
		/// <returns></returns>
		public static Image GetImageFromPath(string path, IconSizes size, IconStyles style, FileAttributes attributes)
		{
			SHFILEINFO shfi = new SHFILEINFO();
			uint uFlags = SHGFI_ICON;
			
			uFlags |= (uint)size;
			uFlags |= (uint)style;
			if (attributes != 0)
				uFlags |= SHGFI_USEFILEATTRIBUTES; 
			
			SHGetFileInfo(path,	(int)attributes, ref shfi, (uint)Marshal.SizeOf(shfi), uFlags);
						
			Image image = (Image)Bitmap.FromHicon(shfi.hIcon).Clone();			
			DeleteObject(shfi.hIcon);
			return image;
		}

		/// <summary>
		/// Gets the type name for the path specified
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static string GetTypeName(string path)
		{
			SHFILEINFO shfi = new SHFILEINFO();
			IntPtr p = SHGetFileInfo(path, (int)FileAttributes.Normal, ref shfi, (uint)Marshal.SizeOf(shfi), (uint)(SHGFI_TYPENAME | SHGFI_USEFILEATTRIBUTES));
			return shfi.szTypeName;
		}

		/// <summary>
		/// Gets the display name for the path specified
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static string GetDisplayName(string path)
		{
			SHFILEINFO shfi = new SHFILEINFO();
			IntPtr p = SHGetFileInfo(path, (int)FileAttributes.Normal, ref shfi, (uint)Marshal.SizeOf(shfi), (uint)(SHGFI_DISPLAYNAME | SHGFI_USEFILEATTRIBUTES));
			return shfi.szDisplayName;
		}

		/// <summary>
		/// Determines if a path is a directory
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static bool IsDirectory(string path)
		{
			try
			{
				FileAttributes attributes = File.GetAttributes(path);				
				return ((attributes & System.IO.FileAttributes.Directory) == System.IO.FileAttributes.Directory);				
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return false;
		}

		/// <summary>
		/// Returns the last write time for a path
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string GetLastWriteTime(string path)
		{		
			try
			{
				DateTime last = System.IO.File.GetLastWriteTime(path);
				return last.ToString();
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return "<Unknown>";
		}
	}
}
