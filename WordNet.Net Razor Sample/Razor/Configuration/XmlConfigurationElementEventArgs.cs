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
	/// EventArgs class for the XmlConfigurationElementEventHandler delegate
	/// </summary>
	public class XmlConfigurationElementEventArgs : System.EventArgs 
	{
		private XmlConfigurationElement _element;
		private XmlConfigurationElementActions _action;
		
		/// <summary>
		/// Represents an event with no event data.
		/// </summary>
		public new static readonly XmlConfigurationElementEventArgs Empty = new XmlConfigurationElementEventArgs(new XmlConfigurationElement(), XmlConfigurationElementActions.None);

		/// <summary>
		/// Initializes a new instance of the XmlConfigurationElementEventArgs class
		/// </summary>
		/// <param name="element">The element being affected by this action</param>
		/// <param name="action">The action affecting this element</param>
		public XmlConfigurationElementEventArgs(XmlConfigurationElement element, XmlConfigurationElementActions action)
		{
			_element = element;
			_action = action;
		}

		/// <summary>
		/// Gets the element that is affected by this event
		/// </summary>
		public XmlConfigurationElement Element
		{
			get
			{
				return _element;
			}		
			set
			{
				_element = value;
			}
		}

		/// <summary>
		/// Gets the action that is affecting this element.
		/// </summary>
		public XmlConfigurationElementActions Action
		{
			get
			{
				return _action;				
			}
			set
			{
				_action = value;
			}
		}		
	}

	/// <summary>
	/// Delegate for the XmlConfigurationElementEventArgs class
	/// </summary>
	public delegate void XmlConfigurationElementEventHandler(object sender, XmlConfigurationElementEventArgs e);
}
