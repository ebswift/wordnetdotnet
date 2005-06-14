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
	/// EventArgs class for the XmlConfigurationOptionEventHandler delegate
	/// </summary>
	public class XmlConfigurationOptionEventArgs : XmlConfigurationElementEventArgs 
	{
		/// <summary>
		/// Initializes a new instance of the XmlConfigurationOptionEventArgs class
		/// </summary>
		/// <param name="option">The option being affected by this action</param>
		/// <param name="action">The action affecting this option</param>
		public XmlConfigurationOptionEventArgs(XmlConfigurationOption option, XmlConfigurationElementActions action) : base(option, action)
		{
			
		}

		/// <summary>
		/// Gets the option affected by this event
		/// </summary>
		public new XmlConfigurationOption Element
		{
			get
			{
				return (XmlConfigurationOption)base.Element;
			}
			set
			{
				base.Element = (XmlConfigurationOption)value;
			}
		}
	}

	/// <summary>
	/// Delegate for the XmlConfigurationOptionEventArgs class
	/// </summary>
	public delegate void XmlConfigurationOptionEventHandler(object sender, XmlConfigurationOptionEventArgs e);
}
