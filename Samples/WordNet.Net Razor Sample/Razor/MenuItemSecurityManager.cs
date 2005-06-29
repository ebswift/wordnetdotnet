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
using System.Windows.Forms;

namespace Razor
{
	/// <summary>
	/// Summary description for MenuItemSecurityManager.
	/// </summary>
	public class MenuItemSecurityManager
	{
		private static MenuItemSecurityManager _theInstance;

		/// <summary>
		/// Occurs when a tool needs verification that it can be clicked
		/// </summary>
		public event MenuItemCancelEventHandler CanClickMenuItem;

		/// <summary>
		/// Returns the currently executing instance of the ToolSecurityManager
		/// </summary>
		/// <returns></returns>
		public static MenuItemSecurityManager GetExecutingInstance()
		{
			return _theInstance;
		}

		/// <summary>
		/// Initializes a new instance of the ToolSecurityManager class
		/// </summary>
		public MenuItemSecurityManager()
		{
			_theInstance = this;
		}

		/// <summary>
		/// Determines if the specified tool can be clicked
		/// </summary>
		/// <param name="tool"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public bool CanClick(object sender, MenuItem menuItem, params object[] args)
		{
			this.AssertValidMenuItem(menuItem);
	
			MenuItemCancelEventArgs e = new MenuItemCancelEventArgs(false, sender, menuItem, args);
			this.OnCanClickMenuItem(this, e);
			return !e.Cancel;
		}

		/// <summary>
		/// Asserts the tool is valid
		/// </summary>
		/// <param name="tool"></param>
		private void AssertValidMenuItem(MenuItem menuItem)
		{
			if (menuItem == null)
				throw new ArgumentNullException("MenuItem", "Cannot use a menu item that has not been created.");					
		}

		/// <summary>
		/// Raises the CanClickMenuItem event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCanClickMenuItem(object sender, MenuItemCancelEventArgs e)
		{
			try
			{
				if (this.CanClickMenuItem != null)
					this.CanClickMenuItem(sender, e);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Debug.WriteLine(systemException);
			}
		}
	}

	#region MenuItemCancelEventArgs

	/// <summary>
	/// EventArgs class for the MenuItemCancelEventHandler delegate
	/// </summary>
	public class MenuItemCancelEventArgs : System.ComponentModel.CancelEventArgs
	{
		private object _sender;
		private MenuItem _menuItem;
		private object[] _args;

		public MenuItemCancelEventArgs(bool cancel, object sender, MenuItem menuItem, params object[] args) : base(cancel)
		{
			_sender = sender;
			_menuItem = menuItem;
			_args = args;
		}

		public object Sender
		{
			get
			{
				return _sender;
			}
		}

		public MenuItem MenuItem
		{
			get
			{
				return _menuItem;
			}
		}

		public object[] Args
		{
			get
			{
				return _args;
			}
		}
	}

	/// <summary>
	/// Delegate for the MenuItemCancelEventArgs class
	/// </summary>
	public delegate void MenuItemCancelEventHandler(object sender, MenuItemCancelEventArgs e);

	#endregion
}
