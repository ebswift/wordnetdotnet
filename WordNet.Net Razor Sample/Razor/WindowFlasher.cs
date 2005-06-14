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
using System.Runtime.InteropServices;

namespace Razor
{
	using UINT	= System.UInt32;
	using HWND	= System.IntPtr;
	using DWORD = System.Int32;

	/// <summary>
	/// Summary description for WindowFlasher.
	/// </summary>
	public class WindowFlasher
	{
		private enum BOOL
		{
			FALSE = 0,
			TRUE  = 1
		}

		private struct FLASHINFO
		{			
			public UINT  cbSize;
			public HWND  hwnd;
			public DWORD dwFlags;
			public UINT  uCount;
			public DWORD dwTimeout;
		}

		[DllImport("User32")]
		private static extern BOOL FlashWindowEx(ref FLASHINFO pfwi);

		private const DWORD FLASHW_STOP			= 0;
		private const DWORD FLASHW_CAPTION		= 0x00000001;
		private const DWORD FLASHW_TRAY			= 0x00000002;
		private const DWORD FLASHW_ALL			= (FLASHW_CAPTION | FLASHW_TRAY);
		private const DWORD FLASHW_TIMER		= 0x00000004;
		private const DWORD FLASHW_TIMERNOFG	= 0x0000000C;
		
		public static bool FlashWindow(HWND hWnd)
		{
			FLASHINFO fwi;
			fwi.cbSize		= (UINT)Marshal.SizeOf(typeof(FLASHINFO));	// size
			fwi.hwnd		= hWnd;									// the window to flash
			fwi.dwFlags		= FLASHW_ALL;							// flash the caption and tray
			fwi.uCount		= 5;									// how many times to flash it
			fwi.dwTimeout	= 0;									// use the default cursor blink rate

			return (FlashWindowEx(ref fwi) == BOOL.TRUE);
		}	

		public static bool FlashWindow(HWND hWnd, int flashCount)
		{
			FLASHINFO fwi;
			fwi.cbSize		= (UINT)Marshal.SizeOf(typeof(FLASHINFO));	// size
			fwi.hwnd		= hWnd;									// the window to flash
			fwi.dwFlags		= FLASHW_ALL;							// flash the caption and tray
			fwi.uCount		= (UINT)flashCount;							// how many times to flash it
			fwi.dwTimeout	= 0;									// use the default cursor blink rate

			return (FlashWindowEx(ref fwi) == BOOL.TRUE);
		}

		public static bool FlashWindow(HWND hWnd, bool flashUntilForeground)
		{
			FLASHINFO fwi;
			fwi.cbSize		= (UINT)Marshal.SizeOf(typeof(FLASHINFO));	// size
			fwi.hwnd		= hWnd;									// the window to flash
			fwi.dwFlags		= (flashUntilForeground ? FLASHW_ALL | FLASHW_TIMERNOFG : FLASHW_ALL);							// flash the caption and tray
			fwi.uCount		= 5;									// how many times to flash it
			fwi.dwTimeout	= 0;									// use the default cursor blink rate

			return (FlashWindowEx(ref fwi) == BOOL.TRUE);
		}

	}
}
