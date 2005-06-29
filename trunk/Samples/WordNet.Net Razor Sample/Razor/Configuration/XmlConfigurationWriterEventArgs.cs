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
using Razor.Configuration;

namespace Razor.Configuration
{
	/// <summary>
	/// Summary description for XmlConfigurationWriterEventArgs.
	/// </summary>
	public class XmlConfigurationWriterEventArgs
	{
		private System.Exception _systemException;
		private XmlConfigurationOption _option;

		public XmlConfigurationWriterEventArgs(System.Exception systemException, XmlConfigurationOption option)
		{
			_systemException = systemException;
			_option = option;
		}

		public System.Exception Exception
		{
			get
			{
				return _systemException;
			}
		}

		public XmlConfigurationOption Option
		{
			get
			{	
				return _option;
			}
		}
	}

	public delegate void XmlConfigurationWriterEventHandler(object sender, XmlConfigurationWriterEventArgs e);
}
