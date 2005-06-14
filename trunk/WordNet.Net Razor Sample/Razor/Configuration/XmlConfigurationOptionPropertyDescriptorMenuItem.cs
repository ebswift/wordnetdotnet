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
using System.Windows.Forms;

namespace Razor.Configuration
{
	/// <summary>
	/// Summary description for XmlConfigurationOptionPropertyDescriptorMenuItem.
	/// </summary>
	public class XmlConfigurationOptionPropertyDescriptorMenuItem : MenuItem 
	{
		private XmlConfigurationOption _option;
		
		public XmlConfigurationOptionPropertyDescriptorMenuItem(string text, EventHandler onClick, XmlConfigurationOption option) : base(text, onClick)
		{
			_option = option;
		}

		public XmlConfigurationOption Option
		{
			get
			{
				return _option;
			}
		}
	}
}
