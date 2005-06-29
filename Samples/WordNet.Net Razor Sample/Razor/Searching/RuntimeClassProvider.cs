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
using System.IO;
using System.Reflection;
using System.Collections;
using System.Threading;
using Razor.Attributes;

namespace Razor.Searching
{
	/// <summary>
	/// Provides methods and events for finding public creatable types from .NET assemblies for the purposes of dynamic creation.
	/// </summary>
	public class RuntimeClassProvider
	{
		/// <summary>
		/// This event is raised each time a public non-abstract class is discovered in an assembly.
		/// </summary>
		public event RuntimeClassProviderEventHandler PublicClassDiscovered;

		/// <summary>
		/// Initializes a new instance of the RuntimeClassProvider class
		/// </summary>
		public RuntimeClassProvider()
		{			
		}

//		/// <summary>
//		/// Attempts to load the file as a .NET assembly, then enumerate the public non-abstract classes inside the assembly.
//		/// </summary>
//		/// <param name="fileInfo">the <see cref="System.IO.FileInfo">FileInfo</see> object that points to the assembly to load from</param>
//		/// <returns>an array of types that are public classes exported by the assembly</returns>
		//		public ArrayList GetPublicClasses(FileInfo fileInfo)
		//		{
		//			ArrayList publicClasses = new ArrayList();
		//
		//			try
		//			{
		//				// the search should be configured to only search for dlls
		////				// first a little extension check... this should help speed things up a bit
		////				switch(fileInfo.Extension)
		////				{
		////					case ".dll":
		////						// ok to continue
		////						break;
		////					
		////					default:
		////						return publicClasses;
		////						break;
		////				};
		//
		//				// try and load the file as a .net assembly
		//				Assembly assembly = Assembly.LoadFrom(fileInfo.FullName);
		//
		//				// get the exported types from the assembly
		//				Type[] types = assembly.GetExportedTypes();
		//				
		//				if (types == null)
		//					return publicClasses;
		//				
		//				foreach(Type t in types)
		//					// A public class that is not abstract
		//					if (t.IsClass && t.IsPublic && (!t.IsAbstract))
		//					{						
		//						publicClasses.Add(t);
		//						OnPublicClassDiscovered(this, new RuntimeClassProviderEventArgs(assembly, t));
		//					}
		//				
		//				types = null;
		//				assembly = null;
		//			}
		//			catch(BadImageFormatException)
		//			{
		//				// fileInfo.FullName is not a valid .NET assembly
		//			}
		//			catch(System.Exception systemException) 
		//			{
		//				System.Diagnostics.Trace.WriteLine(systemException); 
		//			}	
		//			return publicClasses;
		//		}

		/// <summary>
		/// Attempts to load the file as a .NET assembly, then enumerate the public non-abstract classes inside the assembly.
		/// </summary>
		/// <param name="fileInfo">the <see cref="System.IO.FileInfo">FileInfo</see> object that points to the assembly to load from</param>
		/// <returns>an array of types that are public classes exported by the assembly</returns>
		public void DiscoverTypesUsingEnumeration(FileInfo fileInfo)
		{
			try
			{
				// try and load the file as a .net assembly
				Assembly assembly = Assembly.LoadFrom(fileInfo.FullName);

				// get the exported types from the assembly
				Type[] types = assembly.GetExportedTypes();
				
				if (types == null)
					return;
				
				foreach(Type t in types)
					// A public class that is not abstract
					if (t.IsClass && t.IsPublic && (!t.IsAbstract))
						this.OnPublicClassDiscovered(this, new RuntimeClassProviderEventArgs(assembly, t));
				
				types = null;
				assembly = null;
			}
			catch(BadImageFormatException)
			{
				// fileInfo.FullName is not a valid .NET assembly
			}
			catch(System.Exception systemException) 
			{
				System.Diagnostics.Trace.WriteLine(systemException); 
			}	
		}
		
		public void DiscoverTypesUsingMetaData(FileInfo fileInfo)
		{
			try
			{
				// try and load the file as a .net assembly
				Assembly assembly = Assembly.LoadFrom(fileInfo.FullName);				
				
				AssemblyAttributeReader r = new AssemblyAttributeReader(assembly);
				SnapInExportedFromAssemblyAttribute[] exports = r.GetExportedSnapInAttributes();
				foreach(SnapInExportedFromAssemblyAttribute attribute in exports)
				{
					Type t = attribute.Type;
					if (t != null)
						this.OnPublicClassDiscovered(this, new RuntimeClassProviderEventArgs(assembly, t));
				}

				assembly = null;
			}
			catch(BadImageFormatException)
			{
				// fileInfo.FullName is not a valid .NET assembly
			}
			catch(System.Exception systemException) 
			{
				System.Diagnostics.Trace.WriteLine(systemException); 
			}	
		}

		//		public Type[] GetPublicClasses(FileInfo fileInfo)
		//		{
		//			try
		//			{
		//				// try and load the file as a .net assembly
		//				Assembly assembly = Assembly.LoadFrom(fileInfo.FullName);
		//
		//				// get the exported types from the assembly
		//				Type[] types = assembly.GetExportedTypes();
		//				
		//				if (types == null)
		//					return;
		//				
		//				foreach(Type t in types)
		//					// A public class that is not abstract
		//					if (t.IsClass && t.IsPublic && (!t.IsAbstract))
		//						OnPublicClassDiscovered(this, new RuntimeClassProviderEventArgs(assembly, t));
		//				
		//				types = null;
		//				assembly = null;
		//			}
		//			catch(BadImageFormatException)
		//			{
		//				// fileInfo.FullName is not a valid .NET assembly
		//			}
		//			catch(System.Exception systemException) 
		//			{
		//				System.Diagnostics.Trace.WriteLine(systemException); 
		//			}
		//		}

		/// <summary>
		/// Raises the PublicClassDiscovered event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnPublicClassDiscovered(object sender, RuntimeClassProviderEventArgs e)
		{
			try
			{
				if (this.PublicClassDiscovered != null)
					this.PublicClassDiscovered(sender, e);
			}
			catch(System.Exception systemException) 
			{
				System.Diagnostics.Trace.WriteLine(systemException); 
			}	
		}
	}
}
