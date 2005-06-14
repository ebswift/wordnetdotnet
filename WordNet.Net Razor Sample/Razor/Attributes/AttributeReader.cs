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

namespace Razor.Attributes
{
	/// <summary>
	/// A base class for reading attributes from objects.
	/// </summary>
	public abstract class AttributeReader
	{

		public virtual void DumpAttributes(System.Reflection.Assembly a)
		{
			try
			{
				System.Diagnostics.Trace.WriteLine( "Dumping attributes for Assembly '" + a.FullName + "'..." );

				object[] attributes = a.GetCustomAttributes(false);
				if ( attributes != null )
				{
					foreach( object attribute in attributes )
					{
						System.Diagnostics.Trace.WriteLine( attribute );
					}
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine( systemException );
			}
		}

		public virtual void DumpAttributes(System.Type t)
		{
			try
			{
				System.Diagnostics.Trace.WriteLine( "Dumping attributes for Type '" + t.FullName + "'..." );

				object[] attributes = t.GetCustomAttributes(false);
				if ( attributes != null )
				{
					foreach( object attribute in attributes )
					{
						System.Diagnostics.Trace.WriteLine( attribute );
					}
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine( systemException );
			}
		}
	}
}
