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
using System.Reflection;

namespace Razor.Attributes
{
	/// <summary>
	/// Summary description for SnapInAttributeReader.
	/// </summary>
	public class SnapInAttributeReader : AttributeReader
	{
//		private Assembly _assembly;
		private Type _type;

		/// <summary>
		/// Initializes a new instance of the SnapInAttributeReader class
		/// </summary>
		/// <param name="a"></param>
		/// <param name="t"></param>
		public SnapInAttributeReader(/* Assembly a, */Type t)
		{
//			 _assembly = a;
			_type = t;
//			base.DumpAttributes(_type);
		}

		public SnapInImageAttribute GetSnapInImageAttribute()
		{
			try
			{
				object[] attributes = _type.GetCustomAttributes(typeof(SnapInImageAttribute), false);
				if (attributes.Length > 0)
					return attributes[0] as SnapInImageAttribute;	
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return null;
		}

		public SnapInTitleAttribute GetSnapInTitleAttribute()
		{
			try
			{
				object[] attributes = _type.GetCustomAttributes(typeof(SnapInTitleAttribute), false);
				if (attributes.Length > 0)
					return attributes[0] as SnapInTitleAttribute;	
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return null;
		}

		public SnapInDescriptionAttribute GetSnapInDescriptionAttribute()
		{
			try
			{
				object[] attributes = _type.GetCustomAttributes(typeof(SnapInDescriptionAttribute), false);
				if (attributes.Length > 0)
					return attributes[0] as SnapInDescriptionAttribute;	
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return null;
		}
		
		public SnapInCompanyAttribute GetSnapInCompanyAttribute()
		{
			try
			{
				object[] attributes = _type.GetCustomAttributes(typeof(SnapInCompanyAttribute), false);				
				if (attributes.Length > 0)
					return attributes[0] as SnapInCompanyAttribute;	
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return null;
		}

		public SnapInDevelopersAttribute GetSnapInDeveloperAttributes()
		{
			try
			{
				object[] attributes = _type.GetCustomAttributes(typeof(SnapInDevelopersAttribute), false);				
				if (attributes.Length > 0)
					return attributes[0] as SnapInDevelopersAttribute;	
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return null;
		}

		public SnapInVisibilityAttribute GetSnapInHostVisibilityAttribute()
		{
			try
			{		
				object[] attributes = _type.GetCustomAttributes(typeof(SnapInVisibilityAttribute), false);
				if (attributes.Length > 0)
					return attributes[0] as SnapInVisibilityAttribute;				
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return null;
		}

//		public SnapInProductFamilyAttribute[] GetSnapInProductFamilyAttribute()
//		{
//			try
//			{		
//				return (SnapInProductFamilyAttribute[])_type.GetCustomAttributes(typeof(SnapInProductFamilyAttribute), false);				
//			}
//			catch(System.Exception systemException)
//			{
//				System.Diagnostics.Trace.WriteLine(systemException);
//			}
//			return null;
//		}

		public SnapInProductFamilyMemberAttribute[] GetSnapInProductFamilyMemberAttribute()
		{
			try
			{		
				return (SnapInProductFamilyMemberAttribute[])_type.GetCustomAttributes(typeof(SnapInProductFamilyMemberAttribute), false);				
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return null;
		}

		public SnapInVersionAttribute GetSnapInVersionAttribute()
		{
			try
			{
				object[] attributes = _type.GetCustomAttributes(typeof(SnapInVersionAttribute), false);
				if (attributes.Length > 0)
					return attributes[0] as SnapInVersionAttribute;	
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return null;
		}

		public SnapInDependencyAttribute[] GetSnapInDependencyAttributes()
		{
			try
			{
				return (SnapInDependencyAttribute[])_type.GetCustomAttributes(typeof(SnapInDependencyAttribute), false);				
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return new SnapInDependencyAttribute[] {};
		}

		
		
		
		
	}
}
