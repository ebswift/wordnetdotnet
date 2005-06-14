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
using System.Drawing;
using System.Windows.Forms;
using Razor.Configuration;

namespace Razor
{
//	using Window = System.Windows.Forms.Form;

	/// <summary>
	/// Summary description for PersistenceEngine.
	/// </summary>
	public class PersistenceEngine
	{
		/// <summary>
		/// Reads properties for the window from the configuration. Properties include Size, Location, and WindowState.
		/// </summary>
		/// <param name="window">The window whose properties were previously persisted</param>
		/// <param name="configuration">The configuration to read from</param>
		public static void Read(Form window, XmlConfiguration configuration)
		{
			if (window != null)
			{
				if (configuration != null)
				{
					string categoryName = @"Persisted Objects\Windows\" + window.GetType().FullName;
					XmlConfigurationCategory category = null;
					
					// load the category
					if ((category = configuration.Categories[categoryName]) != null)
					{
						XmlConfigurationOption option = null;

						// load the location
						if ((option = category.Options["Location"]) != null)
						{
							window.Location = (Point)option.Value;
						}
						
						if ((option = category.Options["Size"]) != null)
						{
							window.Size = (Size)option.Value;	
						}					

						if ((option = category.Options["WindowState"]) != null)
						{
							window.WindowState = (FormWindowState)option.Value;
						}
					}					
				}
			}							
		}

		/// <summary>
		/// Writes properties of the window to the configuration. Properties include Size, Location, and WindowState.
		/// </summary>
		/// <param name="window">The window whose properties will be persisted</param>
		/// <param name="configuration">The configuration to write to</param>
		public static void Write(Form window, XmlConfiguration configuration)
		{
			if (window != null)
			{
				if (configuration != null)
				{
					string categoryName = @"Persisted Objects\Windows\" + window.GetType().FullName;
					XmlConfigurationCategory category = null;
					
					// load the category
					if ((category = configuration.Categories[categoryName, false]) == null)
					{
						category = configuration.Categories[categoryName, true];
//						category.Hidden = true;   
					}

					XmlConfigurationOption option = null;

					// load the location
					if ((option = category.Options["Location", false]) == null)
					{
						option = category.Options["Location", true, window.Location];
						option.Category = "Layout";
						option.Description = "The coordinates of the upper-left corner of the window relative to it's container.";						
						option.ShouldSerializeValue = true;
					}
					option.Value = window.Location;

					if ((option = category.Options["Size", false]) == null)
					{
						option = category.Options["Size", true, window.Size];
						option.Category = "Layout";
						option.Description = "The size of the window.";
						option.ShouldSerializeValue = true;
					}
					option.Value = window.Size;

					if ((option = category.Options["WindowState", false]) == null)
					{
						option = category.Options["WindowState", true, window.WindowState];
						option.Category = "Layout";
						option.Description = "The state of the window (ie. Maximized, Minimized, Normal).";
					}
					option.Value = window.WindowState;
				}
			}				
		}

		/// <summary>
		/// Writes properties of the window to the configuration. Properties include Size, Location, and WindowState.
		/// </summary>
		/// <param name="window">The window whose properties will be written with the following pameters</param>
		/// <param name="size">The size of the window</param>
		/// <param name="location">The location of the window</param>
		/// <param name="windowState">The state of the window</param>
		/// <param name="configuration">The configuration to write to</param>
		public static void Write(Form window, Size size, Point location, FormWindowState windowState, XmlConfiguration configuration)
		{
			if (window != null)
			{
				if (configuration != null)
				{
					string categoryName = @"Persisted Objects\Windows\" + window.GetType().FullName;
					XmlConfigurationCategory category = null;
					
					// load the category
					if ((category = configuration.Categories[categoryName, false]) == null)
					{
						category = configuration.Categories[categoryName, true];
//						category.Hidden = true;   
					}

					XmlConfigurationOption option = null;

					// load the location
					if ((option = category.Options["Location", false]) == null)
					{
						option = category.Options["Location", true, window.Location];
						option.Category = "Layout";
						option.Description = "The coordinates of the upper-left corner of the window relative to it's container.";						
						option.ShouldSerializeValue = true;
					}
					option.Value = location;

					if ((option = category.Options["Size", false]) == null)
					{
						option = category.Options["Size", true, window.Size];
						option.Category = "Layout";
						option.Description = "The size of the window.";
						option.ShouldSerializeValue = true;
					}
					option.Value = size;

					if ((option = category.Options["WindowState", false]) == null)
					{
						option = category.Options["WindowState", true, window.WindowState];
						option.Category = "Layout";
						option.Description = "The state of the window (ie. Maximized, Minimized, Normal).";
//						option.ShouldSerializeValue = true;
					}
					option.Value = windowState;
				}
			}			
		}
	}
}
