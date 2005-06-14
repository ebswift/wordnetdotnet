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

namespace Razor
{
	/// <summary>
	/// An enumeration for the different startup options provided by the StartupManager class
	/// </summary>
	public enum StartupOptions
	{
		/// <summary>
		/// No startup option
		/// </summary>
		None = 0,

		/// <summary>
		/// Start using the registry HKEY_LOCAL_MACHINE run key
		/// </summary>
		AllUsers = 1,

		/// <summary>
		/// Start using the registry HKEY_CURRENT_USER run key
		/// </summary>
		CurrentUser = 2,

		//		/// <summary>
		//		/// Start using a shortcut in the allusers startup folder
		//		/// </summary>
		//		AllUsersStartup = 3,
		//		
		//		/// <summary>
		//		/// Start using a shortcut in the current user's startup folder
		//		/// </summary>
		//		CurrentUserStartup = 4
	}

	/// <summary>
	/// Provides methods for adding and removing entries from the registry for the purposes of starting an appliation or document when the windows shell starts up.
	/// </summary>
	public class StartupManager
	{
		/// <summary>
		/// The last exception throw by one of the methods of this class
		/// </summary>
		private Exception _lastException;

		/// <summary>
		/// Initializes a new instance of the StartupProvider class
		/// </summary>
		public StartupManager()
		{
		
		}

		/// <summary>
		/// Gets the last exception thrown by methods of this class
		/// </summary>
		public Exception LastException
		{
			get
			{
				return _lastException;
			}
		}

		/// <summary>
		/// Adds a name/value pair to the HKCU\Run key in the registry
		/// </summary>
		/// <param name="valueName">The name of the value</param>
		/// <param name="value">The data for the value</param>
		/// <returns></returns>
		public bool AddCurrentUserStartupItem(string valueName, string value)
		{
			try
			{
				string subkey = @"Software\Microsoft\Windows\CurrentVersion\Run";
				Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(subkey, true);				
				regKey.SetValue(valueName, value);
				return true;

			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
				_lastException = systemException;
				return false;
			}
		}

		/// <summary>
		/// Removes a name/value pair from the HKLM\Run key in the registry
		/// </summary>
		/// <param name="valueName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool RemoveCurrentUserStartupItem(string valueName)
		{
			try
			{
				string subkey = @"Software\Microsoft\Windows\CurrentVersion\Run";
				Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(subkey, true);				
				regKey.DeleteValue(valueName, false);
				return true;
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
				_lastException = systemException;
				return false;
			}
		}

		/// <summary>
		/// Adds a name/value pair to the HKLM\Run key in the registry
		/// </summary>
		/// <param name="valueName">The name of the value</param>
		/// <param name="value">The data for the value</param>
		/// <returns></returns>
		public bool AddLocalMachineStartupItem(string valueName, string value)
		{
			try
			{
				string subkey = @"Software\Microsoft\Windows\CurrentVersion\Run";
				Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(subkey, true);				
				regKey.SetValue(valueName, value);
				return true;
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
				_lastException = systemException;
				return false;
			}
		}

		/// <summary>
		/// Removes a name/value pair from the HKLM\Run key in the registry
		/// </summary>
		/// <param name="valueName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool RemoveLocalMachineStartupItem(string valueName)
		{
			try
			{
				string subkey = @"Software\Microsoft\Windows\CurrentVersion\Run";
				Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(subkey, true);				
				regKey.DeleteValue(valueName, false);
				return true;
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
				_lastException = systemException;
				return false;
			}
		}

		/// <summary>
		/// Changes the startup option
		/// </summary>
		/// <param name="previousOption"></param>
		/// <param name="newOption"></param>
		/// <returns></returns>
		public bool ChangeStartupOption(StartupOptions previousOption, string previousStartupItemName, StartupOptions newOption, string startupItemName, string startupItemValue)
		{
			// if there is no change then we don't need to do anything
			//			if (previousOption == newOption)
			//				return true;

			// remove the previous option 
			switch(previousOption)
			{
			case StartupOptions.None:
				break;

			case StartupOptions.AllUsers:
				this.RemoveLocalMachineStartupItem(previousStartupItemName);
				break;

			case StartupOptions.CurrentUser:
				this.RemoveCurrentUserStartupItem(previousStartupItemName);
				break;

				//				case StartupOptions.AllUsersStartup:
				//					break;
				//
				//				case StartupOptions.CurrentUserStartup:
				//					break;
			};
			
			bool result = false;

			// add the new option
			switch(newOption)
			{
			case StartupOptions.None:
				this.RemoveLocalMachineStartupItem(previousStartupItemName);
				this.RemoveCurrentUserStartupItem(previousStartupItemName);
				break;

			case StartupOptions.AllUsers:
				result = this.AddLocalMachineStartupItem(startupItemName, startupItemValue);
				break;

			case StartupOptions.CurrentUser:
				result = this.AddCurrentUserStartupItem(startupItemName, startupItemValue);
				break;

				//				case StartupOptions.AllUsersStartup:
				//					break;
				//
				//				case StartupOptions.CurrentUserStartup:
				//					break;				
			};

			return result;
		}
	}
}
