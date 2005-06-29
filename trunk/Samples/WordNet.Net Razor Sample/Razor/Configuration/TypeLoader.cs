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
using System.Reflection;
using System.IO;

namespace Razor.Configuration
{
	/// <summary>
	/// Summary description for TypeLoader.
	/// </summary>
	[Serializable()]
	public class TypeLoader
	{
		public TypeLoader()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// Loads a Type specified by it's name, using the specified assembly name as the source
		/// </summary>
		/// <param name="typename">The name of the type (may be full or partial)</param>
		/// <param name="assemblyname">The name of the assembly (may be full or partial, may include the entire path or not)</param>
		/// <returns></returns>
		public Type LoadType(string typename, string assemblyname)
		{
			try
			{
				Type t = null;
				Assembly a = this.LoadAssembly(assemblyname);
				if (a != null)
				{
					t = a.GetType(typename, false, true);					
					if (t != null)
						return t;
				}			
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return null;
		}

		/// <summary>
		/// Loads the specified assembly into the current appdomain
		/// </summary>
		/// <param name="assemblyname">The name of the assembly (may be full or partial, may include the entire path or not)</param>
		/// <returns></returns>
		public Assembly LoadAssembly(string assemblyname)
		{			
			Assembly assembly = null;
			try
			{
				FileInfo file = new FileInfo(assemblyname);
				if (!file.Exists)
					assembly = Assembly.LoadWithPartialName(assemblyname);				
				else
					assembly = Assembly.LoadFrom(assemblyname);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}			
			return assembly;
		}

		public static Type LoadTypeInSeparateAppDomain(string typename, string assemblyname)
		{
			try
			{
				/// first try loading the type the easy way
				Type t = Type.GetType(typename, false, true);
				if (t == null)
				{
					/// it didn't load, so try loading the reference and then finding it
					/// but do it in a separate app domain so that we don't incur the wrath of the overhead monkey
					AppDomain domain = AppDomain.CreateDomain(Guid.NewGuid().ToString());
					TypeLoader loader = (TypeLoader)domain.CreateInstanceFromAndUnwrap(System.Reflection.Assembly.GetExecutingAssembly().Location, typeof(TypeLoader).FullName);
					t = loader.LoadType(typename, assemblyname);
					AppDomain.Unload(domain);					
				}
				return t;
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return null;
		}

		public static Type GetType(XmlConfigurationOption option)
		{		
			try
			{
				Type t = null;
				if (option.ValueAssemblyQualifiedName != null)
				{			
					t = Type.GetType(option.ValueAssemblyQualifiedName, false, true);
					if (t != null)
						return t;					
				}

				object value = option.Value;
				if ((value != null) && (((string)value) != string.Empty))
				{
					t = value.GetType();
					if (t != null)
						return t;
				}
			}
			catch(System.Exception)
			{
//				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return Type.Missing as Type;
		}	

	}
}
