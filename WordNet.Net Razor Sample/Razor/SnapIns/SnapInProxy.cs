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

namespace Razor.SnapIns
{
	/// <summary>
	/// Provides an abstract class that should be inherited for determining if a Type has been loaded into the current AppDomain.
	/// </summary>
	public abstract class SnapInProxy
	{		
		private System.Exception _lastException;

		/// <summary>
		/// Override this method and return the Type that will be tested
		/// </summary>
		/// <returns></returns>
		protected abstract Type GetTypeForReference();
		
		public static bool operator ==(SnapInProxy proxy, SnapInDescriptor descriptor)
		{
//			if (proxy == null)
//				return false;

			Type proxyType = proxy.GetTypeForReferenceInternal();
			if (proxyType == null)
				return false;

			return Type.Equals(proxyType, descriptor.Type);
		}

		public static bool operator !=(SnapInProxy proxy, SnapInDescriptor descriptor)
		{
//			if (proxy == null)
//				return false;

			Type proxyType = proxy.GetTypeForReferenceInternal();
			if (proxyType == null)
				return false;

			return !Type.Equals(proxyType, descriptor.Type);
		}
				
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			Type t = obj.GetType();
			if (t == null)
				return false;

			if (t == typeof(SnapInDescriptor))
			{
				return (this == obj);
			}

			return base.Equals (obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

		/// <summary>
		/// Returns an instance of the ISnapIn that supports the Type specified by the SubClass of this SnapInProxy.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="hostingEngine"></param>
		/// <returns></returns>
		public bool GetInstance(out object instance)
		{
			instance = null;
			try
			{
				SnapInDescriptor descriptor = SnapInHostingEngine.GetExecutingInstance().FindDescriptorByType(this.GetTypeForReferenceInternal());
				if (descriptor != null)
				{
					instance = descriptor.SnapIn;
					return true;
				}
			}
			catch(System.Exception systemException)
			{
				_lastException = systemException;
			}	
			return false;
		}

		/// <summary>
		/// Internally wraps a call to the abstract method with exception handling, chances are that if this object is in use it's not going to have the dll loaded and it'll bomb.
		/// This way gives us a safe way to call it without having to constantly re-code this type of thing everywhere we want to test
		/// </summary>
		/// <returns></returns>
		private Type GetTypeForReferenceInternal()
		{
			_lastException = null;
			try
			{
				// try and call the abstract method
				return this.GetTypeForReference();
			}
			catch(System.Exception systemException)
			{
				// save the exception so we can look at it laster
				_lastException = systemException;				
				//				System.Diagnostics.Trace.WriteLine(_lastException);
			}
			return null;
		}

		/// <summary>
		/// Gets whether the assembly that contains the type is loaded into the current AppDomain. False indicates any attempt to reference the Type will result in an exception, most notably a System.IO.FileNotFoundException on the dll that contains the Type.
		/// </summary>
		public bool IsLoaded
		{
			get
			{
				return (this.GetTypeForReferenceInternal() != null);	
			}
		}		

		/// <summary>
		/// Gets the last exception that was caught by calling IsLoaded
		/// </summary>
		public System.Exception LastException
		{
			get
			{
				return _lastException;
			}
		}
		
		/// <summary>
		/// Gets whether an exception was caught, and there is more information to be retrieved in the exception object.
		/// </summary>
		public bool HasMoreInformation
		{
			get
			{
				return (_lastException != null);
			}
		}
	}	
}
