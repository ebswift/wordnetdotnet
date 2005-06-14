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
using System.Diagnostics;
using System.Reflection;

namespace Razor.Attributes
{
	/// <summary>
	/// Provides a class that can read the System.Reflection.Assembly attributes from an assembly. These are found in AssemblyInfo.cs or .vb in .NET projects.
	/// </summary>
	public class AssemblyAttributeReader : AttributeReader
	{
		private Assembly _assembly;

		/// <summary>
		/// Initializes a new instance of the AssemblyAttributeReader class
		/// </summary>
		/// <param name="assembly">The Assembly for which infomation should be provided</param>
		public AssemblyAttributeReader(Assembly assembly)
		{
			_assembly = assembly;
//			base.DumpAttributes(_assembly);
		}
		
		/// <summary>
		/// Returns the assembly's version
		/// </summary>
		/// <returns></returns>
		public Version GetAssemblyVersion()
		{
			try
			{
				if (_assembly != null)
				{
					Version v = _assembly.GetName().Version;
//					System.Diagnostics.Trace.WriteLine(v.ToString());
					return v;
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return null;
		}

		/// <summary>
		/// Returns the assembly's ProductIdentifierAttribute
		/// </summary>
		/// <returns></returns>
		public ProductIdentifierAttribute GetProductIdentifierAttribute()
		{
			try
			{
				object[] attributes = _assembly.GetCustomAttributes(typeof(ProductIdentifierAttribute), false);
				if (attributes != null)
					if (attributes.Length > 0)
						return attributes[0] as ProductIdentifierAttribute;
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
			return null;
		}

		/// <summary>
		/// Returns the assembly's RequiresRegistrationAttribute
		/// </summary>
		/// <returns></returns>
		public RequiresRegistrationAttribute GetRequiresRegistrationAttribute()
		{
			try
			{
				object[] attributes = _assembly.GetCustomAttributes(typeof(RequiresRegistrationAttribute), false);
				if (attributes != null)
					if (attributes.Length > 0)
						return attributes[0] as RequiresRegistrationAttribute;
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
			return null;
		}

		/// <summary>
		/// Returns an array of the assembly's AssemblyCompanyAttribute
		/// </summary>
		/// <returns></returns>
		public AssemblyCompanyAttribute[] GetAssemblyCompanyAttributes()
		{
			try
			{
				return (AssemblyCompanyAttribute[])_assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return new AssemblyCompanyAttribute[] {};
		}

		/// <summary>
		/// Returns an array of the assembly's AssemblyConfigurationAttribute
		/// </summary>
		/// <returns></returns>
		public AssemblyConfigurationAttribute[] GetAssemblyConfigurationAttributes()
		{
			try
			{
				return (AssemblyConfigurationAttribute[])_assembly.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return new AssemblyConfigurationAttribute[] {};
		}

		/// <summary>
		/// Returns an array of the assembly's AssemblyCopyrightAttribute
		/// </summary>
		/// <returns></returns>
		public AssemblyCopyrightAttribute[] GetAssemblyCopyrightAttributes()
		{
			try
			{
				return (AssemblyCopyrightAttribute[])_assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return new AssemblyCopyrightAttribute[] {};
		}

		/// <summary>
		/// Returns an array of the assembly's SnapInExportedFromAssemblyAttribute
		/// </summary>
		/// <returns></returns>
		public SnapInExportedFromAssemblyAttribute[] GetExportedSnapInAttributes()
		{
			try
			{
				return (SnapInExportedFromAssemblyAttribute[])_assembly.GetCustomAttributes(typeof(SnapInExportedFromAssemblyAttribute), false);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return new SnapInExportedFromAssemblyAttribute[] {};
		}

		/// <summary>
		/// Returns an array of the assembly's SnapInProductFamilyMemberAttribute
		/// </summary>
		/// <returns></returns>
		public SnapInProductFamilyMemberAttribute[] GetSnapInProductMemberAttributes()
		{
			try
			{
				return (SnapInProductFamilyMemberAttribute[])_assembly.GetCustomAttributes(typeof(SnapInProductFamilyMemberAttribute), false);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return new SnapInProductFamilyMemberAttribute[] {};
		}
	}
}
