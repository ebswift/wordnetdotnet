/*
 * This file is a part of the Razor Framework.
 * 
 * Copyright (C) 2004 Mark (Code6) Belles 
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

namespace Razor.Configuration
{
	/// <summary>
	/// Summary description for XmlConfigurationManager.
	/// </summary>
	public class XmlConfigurationManager
	{
		private static XmlConfigurationManager _theInstance;

		/// <summary>
		/// Fires when the XmlConfigurationManager is enumerating configurations
		/// </summary>
		public event XmlConfigurationManagerEventHandler EnumeratingConfigurations;
		
		/// <summary>
		/// Returns the currently executing instance of the XmlConfigurationManager class
		/// </summary>
		/// <returns></returns>
		public static XmlConfigurationManager GetExecutingInstance()
		{
			return _theInstance;
		}

		/// <summary>
		/// Initilizes a new instance of the 
		/// </summary>
		public XmlConfigurationManager()
		{
			_theInstance = this;
		}

		/// <summary>
		/// Allows a client to retrieve all of the configurations to be displayed in a configuration properties window, or for other purposes. All SnapIns should listen for the for the EnumeratingConfigurations event, and add any configurations to the event as needed.
		/// </summary>
		/// <param name="configurations"></param>
		/// <returns></returns>
		public XmlConfiguration[] EnumConfigurations(params XmlConfiguration[] configurations)
		{			
			XmlConfigurationManagerEventArgs e = new XmlConfigurationManagerEventArgs(configurations);
			this.OnEnumeratingConfigurations(this, e);
			return e.Configurations.ToArray();
		}

		/// <summary>
		/// Raises the EnumeratingConfigurations event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnEnumeratingConfigurations(object sender, XmlConfigurationManagerEventArgs e)
		{
			try
			{
				if (this.EnumeratingConfigurations != null)
					this.EnumeratingConfigurations(sender, e);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Debug.WriteLine(systemException);
			}
		}	
	}
}
