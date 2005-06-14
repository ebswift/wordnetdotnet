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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;

namespace Razor
{
	/// <summary>
	/// The various types of formatters supported by the SerializationProvider class
	/// </summary>
	public enum FormatterTypes: int
	{
		/// <summary>
		/// Specifies the BinaryFormatter formatter
		/// </summary>
		Binary,

		/// <summary>
		/// Specifies the SoapFormatter formatter
		/// </summary>
		Soap
	}

	/// <summary>
	/// Provides methods for serializing and deserializing objects using binary or soap formatters.
	/// </summary>
	public class SerializationProvider
	{
		private static bool _throwExceptions = false;

		/// <summary>
		/// Gets or sets whether caught exceptions are thrown
		/// </summary>
		public static bool ThrowExceptions
		{
			get
			{
				return _throwExceptions;
			}
			set
			{
				_throwExceptions = value;
			}
		}
		
		public static void Serialize(object sender, FormatterTypes type, Stream stream)
		{
			try
			{
				// select the desired formatter (ie. Soap or Binary)
				IFormatter formatter = GetFormatter(type);
								
				formatter.Serialize(stream, sender);
			}
			catch(System.Exception systemException)
			{				
				System.Diagnostics.Trace.WriteLine(systemException);

				if (_throwExceptions)
					throw(systemException);
			}
		}

		/// <summary>
		/// Serializes an object to a file using a formatter
		/// </summary>
		/// <param name="sender">the object to serialize</param>
		/// <param name="type">the type of formatter to use</param>
		/// <param name="filename">the file to write to</param>
		public static void Serialize(object sender, FormatterTypes type, string filename) 
		{
			try
			{
				// select the desired formatter (ie. Soap or Binary)
				IFormatter formatter = GetFormatter(type);
				
				Stream stream = File.Open(filename, FileMode.Create);

				formatter.Serialize(stream, sender);

				stream.Close();
			}
			catch(System.Exception systemException)
			{				
				System.Diagnostics.Trace.WriteLine(systemException);

				if (_throwExceptions)
					throw(systemException);
			}
		}

		/// <summary>
		/// Serializes an object to a string using a formatter
		/// </summary>
		/// <param name="sender">the object to serialize</param>
		/// <param name="type">the type of formatter to use</param>
		/// <returns>a string containing the serialized object</returns>
		public static string Serialize(object sender, FormatterTypes type)
		{
			string buffer = null;
			try
			{
				// select the desired formatter (ie. Soap or Binary)
				IFormatter formatter = GetFormatter(type);

				MemoryStream stream = new MemoryStream();

				formatter.Serialize(stream, sender);

				buffer = System.Text.Encoding.ASCII.GetString(stream.GetBuffer());

				stream.Close();
			}
			catch(System.Exception systemException)
			{				
				System.Diagnostics.Trace.WriteLine(systemException);

				if (_throwExceptions)
					throw(systemException);
			}

			return buffer;
		}
		
		/// <summary>
		/// Serializes an object to a graph
		/// </summary>
		/// <param name="graph"></param>
		/// <returns></returns>
		public static string Serialize(object graph)
		{
			string buffer = null;
			try
			{
				IFormatter formatter = new SoapFormatter();	
				MemoryStream stream = new MemoryStream();
				formatter.Serialize(stream, graph);
				buffer = System.Text.Encoding.ASCII.GetString(stream.GetBuffer());
				stream.Close();
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return buffer;
		}

		/// <summary>
		/// Deserializes a graph to an object
		/// </summary>
		/// <param name="buffer"></param>
		/// <returns></returns>
		public static object Deserialize(string buffer)
		{
			object obj = null;
			try
			{				
				IFormatter formatter = new SoapFormatter();	
				byte[] bytes = System.Text.Encoding.ASCII.GetBytes(buffer);
				MemoryStream stream = new MemoryStream(bytes);
				obj = formatter.Deserialize(stream);
				stream.Close();
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return obj;
		}

		public static object Deserialize(FormatterTypes type, byte[] buffer)
		{
			object obj = null;
			try
			{
				// select the desired formatter (ie. Soap or Binary)
				IFormatter formatter = GetFormatter(type);
				//				formatter.Binder = new SnapInsSerializationBinder(null);

				Stream stream = new MemoryStream(buffer);

				obj = formatter.Deserialize(stream);

				stream.Close();				
			}
			catch(System.Exception systemException)
			{				
				System.Diagnostics.Trace.WriteLine(systemException);

				if (_throwExceptions)
					throw(systemException);
			}
			return obj;
		}

		//		public static object Deserialize(FormatterTypes formatterType, byte[] buffer, System.Runtime.Serialization.SerializationBinder binder)
		//		{
		//			object obj = null;
		//			try
		//			{
		//				// select the desired formatter (ie. Soap or Binary)
		//				IFormatter formatter = GetFormatter(formatterType);
		//				
		////				formatter.Binder = binder;
		//
		//				Stream stream = new MemoryStream(buffer);
		//				
		//				obj = formatter.Deserialize(stream);
		//
		//				stream.Close();				
		//			}
		//			catch(System.Exception systemException)
		//			{				
		//				System.Diagnostics.Trace.WriteLine(systemException);
		//
		//				if (_throwExceptions)
		//					throw(systemException);
		//			}
		//			return obj;
		//		}

		/// <summary>
		/// Deserializes an object from a file using a formatter
		/// </summary>
		/// <param name="type">the type of formatter to use</param>
		/// <param name="filename">the file to load the serialized object from</param>
		/// <returns>the deserialized object from the file</returns>
		public static object Deserialize(FormatterTypes type, string filename)
		{
			object obj = null;
			try
			{
				// select the desired formatter (ie. Soap or Binary)
				IFormatter formatter = GetFormatter(type);
				//				formatter.Binder = new SnapInsSerializationBinder(null);
				Stream stream = File.Open(filename, FileMode.Open);

				obj = formatter.Deserialize(stream);

				stream.Close();				
			}
			catch(System.Exception systemException)
			{				
				System.Diagnostics.Trace.WriteLine(systemException);

				if (_throwExceptions)
					throw(systemException);
			}
			return obj;			
		}

		/// <summary>
		/// Deserializes an object from a string using a formatter
		/// </summary>
		/// <param name="buffer">the string containing the serialized object</param>
		/// <param name="type">the type of formatter to use</param>
		/// <returns>the deserialized object from the string</returns>
		public static object Deserialize(string buffer, FormatterTypes type)
		{
			object obj = null;
			try
			{
				// select the desired formatter (ie. Soap or Binary)
				IFormatter formatter = GetFormatter(type);
				//				formatter.Binder = new SnapInsSerializationBinder(null);

				byte[] bytes = System.Text.Encoding.ASCII.GetBytes(buffer);

				MemoryStream stream = new MemoryStream(bytes);

				obj = formatter.Deserialize(stream);

				stream.Close();

				return obj;
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);

				if (_throwExceptions)
					throw(systemException);
			}
			return obj;
		}
		
		/// <summary>
		/// Gets a new formatter of the specified type
		/// </summary>
		/// <param name="type">the type of formatter to instanciate</param>
		/// <returns>an instanciated formatter object</returns>
		private static IFormatter GetFormatter(FormatterTypes type)
		{
			if (type == FormatterTypes.Binary)
				return new BinaryFormatter();

			if (type == FormatterTypes.Soap)
				return new SoapFormatter();					

			// default so we don't have failure
			return new SoapFormatter();
		}
	}
}
