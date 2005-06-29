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
using Razor.Attributes;
using Razor.SnapIns;

namespace Razor.Searching
{
	public enum SnapInLocationAlgorithms
	{
		LocateUsingEnumeration,
		LocateUsingMetadata
	}

	/// <summary>
	/// Yeah, my meager attempt at creative naming, just thought Spyder is a cool name, since it's gonna crawl thru appdomains and files searching for meta data or types of snapins to load.
	/// </summary>
	[Serializable()]
	public class SnapInProvider
	{
		private static SnapInLocationAlgorithms _algorithm = SnapInLocationAlgorithms.LocateUsingMetadata;

		private ArrayList _array;
		private Type _type;
		private IProgressViewer _progressViewer;
		private FileInfo _file;
		
		/// <summary>
		/// Initializes a new instance of the SnapInProvider class
		/// </summary>
		public SnapInProvider()
		{

		}

		/// <summary>
		/// Executes a search and returns 
		/// </summary>
		/// <param name="search"></param>
		/// <returns></returns>
		public ArrayList SearchForTypes(Search search, Type type, IProgressViewer progressViewer)
		{			
			_array = new ArrayList();
			_progressViewer = progressViewer;

			try
			{
				if (search == null)
					throw new ArgumentNullException("search", "The search cannot be null.");

				if (type == null)
					throw new ArgumentNullException("type", "The type to search for cannot be null.");

				// save the type that we are searching for
				_type = type;

				// bind to the search's events
				search.FileFound += new SearchEventHandler(this.OnFileFound);

				// start the search
				search.FindFiles();				
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			// return the array list of items found
			return _array;
		}

		/// <summary>
		/// Occurs when a file is found that matches the search pattern
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnFileFound(object sender, SearchEventArgs e)
		{
			try
			{	
				_file = e.File;

//				if (_outputWindow != null)							
//					_outputWindow.Write("Searching for SnapIns...\nSearching for SnapIns in '" + _file.Name + "'\n\n");
			
//				System.Diagnostics.Trace.WriteLine("Searching for SnapIns in '" + _file.FullName + "'", this.GetType().Name);
				
				RuntimeClassProvider provider = new RuntimeClassProvider();
				provider.PublicClassDiscovered += new RuntimeClassProviderEventHandler(this.OnPublicClassDiscovered);

				switch(_algorithm)
				{
				case SnapInLocationAlgorithms.LocateUsingEnumeration:
					provider.DiscoverTypesUsingEnumeration(e.File);
					break;
				case SnapInLocationAlgorithms.LocateUsingMetadata:
					provider.DiscoverTypesUsingMetaData(e.File);
					break;
				};							
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPublicClassDiscovered(object sender, RuntimeClassProviderEventArgs e)
		{
			try
			{
				// does type support the interface we are searching for? ISnapIn?
				if (e.Type.GetInterface(_type.FullName) == null)
					return;

				try
				{
					// confirm the type is visible, some types may be marked with the SnapInVisibilityAttribute
					if (this.IsTypeVisibleForHosting(e))
						_array.Add(e);
				}
				catch(System.Exception systemException)
				{
					System.Diagnostics.Trace.WriteLine(systemException);
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		private bool IsTypeVisibleForHosting(RuntimeClassProviderEventArgs e)
		{
			try
			{
				// ok, yes, now make sure that no one has hidden it from the SnapInHostingEngine
				SnapInAttributeReader r = new SnapInAttributeReader(e.Type);
				SnapInVisibilityAttribute a = r.GetSnapInHostVisibilityAttribute();

				if (a != null)
					return a.Visible;					
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return true;
		}

		#region Static Methods
        
		public static SnapInLocationAlgorithms LocationAlgorithm
		{
			get
			{				
				return _algorithm;
			}
			set
			{
				_algorithm = value;
			}
		}

//		/// <summary>
//		/// Finds and creates all SnapIn classes discovered using the specified search.
//		/// </summary>
//		/// <param name="search"></param>
//		/// <param name="outputWindow"></param>
//		/// <returns></returns>
//		public static SnapInInfoCollection FindAndCreateSnapIns(Search search, IProgressViewer progressViewer)
//		{
//			SnapInInfoCollection snapIns = new SnapInInfoCollection();
//			try
//			{								
//				AppDomain domain = AppDomain.CreateDomain(Guid.NewGuid().ToString());
//				SnapInProvider provider = (SnapInProvider)domain.CreateInstanceFromAndUnwrap(System.Reflection.Assembly.GetExecutingAssembly().Location, typeof(SnapInProvider).FullName);
//				ArrayList array = provider.SearchForTypes(search, typeof(ISnapIn), progressViewer);
//				AppDomain.Unload(domain);
//
//				if (array != null)
//				{
//					foreach(RuntimeClassProviderEventArgs e in array)
//					{
//						object runTimeObject = SnapInProvider.CreateSnapInFrom(e.Assembly, e.Type, progressViewer);
//						if (runTimeObject != null)
//						{
//							snapIns.Add(e.Assembly, e.Type, (ISnapIn)runTimeObject);						
//							
////							if (outputWindow != null)
////								outputWindow.Write("Created SnapIn...Created SnapIn '" + e.Type.FullName + "'"); // from '" + e.Assembly.Location + "'");
////	
////							System.Diagnostics.Trace.WriteLine("Created SnapIn '" + e.Type.FullName + "' from '" + e.Assembly.Location + "'", typeof(SnapInProvider).Name);
//						}
//					}
//				}
//			}
//			catch(System.Exception systemException)
//			{
//				System.Diagnostics.Trace.WriteLine(systemException);
//			}
//			return snapIns;
//		}

		public static ArrayList FindSnapIns(Search search, IProgressViewer progressViewer)
		{
			try
			{
				AppDomain domain = AppDomain.CreateDomain(Guid.NewGuid().ToString());
				SnapInProvider provider = (SnapInProvider)domain.CreateInstanceFromAndUnwrap(System.Reflection.Assembly.GetExecutingAssembly().Location, typeof(SnapInProvider).FullName);
				ArrayList array = provider.SearchForTypes(search, typeof(ISnapIn), progressViewer);
				AppDomain.Unload(domain);
				return array;
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return new ArrayList();
		}
		
//		public static object CreateSnapIn(Type t, SnapInHostingEngine hostingEngine, IProgressViewer progressViewer)
//		{
//			try
//			{
//				Type[] types = new Type[] {typeof(SnapInHostingEngine)};
////				ConstructorInfo ci = t.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.HasThis, types, null);
//				ConstructorInfo ci = t.GetConstructor(types);
//				if (ci != null)
//				{
//					object instance = ci.Invoke(new object[] {hostingEngine});
//					if (instance != null)
//						return instance;
//				}
//			}
//			catch(System.Exception systemException)
//			{
//				if (t != null)
//					System.Diagnostics.Trace.WriteLine("Failed to create an instance of '" + t.FullName + "'");
//				System.Diagnostics.Trace.WriteLine(systemException);
//			}
//			return null;
//		}
//
//		/// <summary>
//		/// Creates an instance of a SnapIn from the specified Assembly of the specified Type
//		/// </summary>
//		/// <param name="assembly"></param>
//		/// <param name="type"></param>
//		/// <returns></returns>
//		public static object CreateSnapInFrom(System.Reflection.Assembly assembly, System.Type type, IProgressViewer progressViewer)
//		{
//			try
//			{
////				if (outputWindow != null)
////					outputWindow.Write("Creating SnapIn...Creating SnapIn '" + type.FullName + "' from '" + assembly.Location + "'");
//				
//
//				return assembly.CreateInstance(type.FullName);
//			}
//			catch(System.Exception systemException)
//			{
//				if (type != null)
//					System.Diagnostics.Trace.WriteLine("Failed to create an instance of '" + type.FullName + "'");
//				System.Diagnostics.Trace.WriteLine(systemException);
//			}
//			return null;
//		}
	
		#endregion
	}
}
