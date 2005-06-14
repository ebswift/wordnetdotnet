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
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;

namespace Razor.Configuration
{
	/// <summary>
	/// This component is responsible for writing an XmlConfiguration object to a System.IO.Stream.
	/// </summary>
	public class XmlConfigurationWriter : System.ComponentModel.Component
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Occurs when an exception is incurred while writing the value of an option
		/// </summary>
		public event XmlConfigurationWriterEventHandler CannotWriteValue;
		
		/// <summary>
		/// Initializes a new instance of the XmlConfigurationWriter class
		/// </summary>
		/// <param name="container"></param>
		public XmlConfigurationWriter(System.ComponentModel.IContainer container)
		{
			///
			/// Required for Windows.Forms Class Composition Designer support
			///
			container.Add(this);
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		
		/// <summary>
		/// Initializes a new instance of the XmlConfigurationWriter class
		/// </summary>
		public XmlConfigurationWriter()
		{
			///
			/// Required for Windows.Forms Class Composition Designer support
			///
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		
		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}

		#endregion

		/// <summary>
		/// Writes an entire configuration file using the specified configuration and stream.
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="stream"></param>
		/// <param name="alwaysPersist"></param>
		public void Write(XmlConfiguration configuration, Stream stream, bool alwaysPersist)
		{
			try
			{
				/// create a new xml document
				XmlDocument doc = new XmlDocument();

				/// create the root element
				XmlElement root = doc.CreateElement(@"ConfigurationFile");
				
				/// append the root element as the first element
				doc.AppendChild(root);
								
				/// mark the xml as version 1.0 compliant
				XmlDeclaration versionDeclaration = doc.CreateXmlDeclaration(@"1.0", null, null);
				
				/// insert the version element as the first element
				doc.InsertBefore(versionDeclaration, root);
				
				this.WriteConfiguration(doc, root, configuration, alwaysPersist);
																
				/// save the xml document to the stream
				doc.Save(stream);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.Write(systemException);
			}
		}
		
		
		/// <summary>
		/// Writes a configuration to the XmlDocument
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="parent"></param>
		/// <param name="configuration"></param>
		/// <param name="alwaysPersist"></param>
		private void WriteConfiguration(XmlDocument doc, XmlElement parent, XmlConfiguration configuration, bool alwaysPersist)
		{
			try
			{
				if (configuration.Persistent || alwaysPersist)
				{
//					System.Diagnostics.Trace.WriteLine("Writing configuration '" + configuration.ElementName + "'");

					/// create an element for the category				
					XmlElement child = doc.CreateElement(@"Configuration");

					/// write the properties of this category
					child.SetAttribute(@"ElementName", configuration.ElementName);
					child.SetAttribute(@"HasChanges", XmlConvert.ToString(configuration.HasChanges));
					child.SetAttribute(@"Category", configuration.Category);
					child.SetAttribute(@"Description", configuration.Description);
					child.SetAttribute(@"DisplayName", configuration.DisplayName);
					child.SetAttribute(@"Hidden", XmlConvert.ToString(configuration.Hidden));
				
					/// append the child to our parent
					parent.AppendChild(child);

					this.WriteCategories(doc, child, configuration.Categories, alwaysPersist);
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		
		/// <summary>
		/// Writes a collection of categories to the XmlDocument
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="parent"></param>
		/// <param name="categories"></param>
		/// <param name="alwaysPersist"></param>
		private void WriteCategories(XmlDocument doc, XmlElement parent, XmlConfigurationCategoryCollection categories, bool alwaysPersist)
		{
			try
			{				
				/// write each category in this category collection
				foreach(XmlConfigurationCategory category in categories)
					this.WriteCategory(doc, parent, category, alwaysPersist);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		
		/// <summary>
		/// Writes a category to the XmlDocument
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="parent"></param>
		/// <param name="category"></param>
		/// <param name="alwaysPersist"></param>
		private void WriteCategory(XmlDocument doc, XmlElement parent, XmlConfigurationCategory category, bool alwaysPersist)
		{
			try
			{
				if (category.Persistent || alwaysPersist)
				{
					/// create an element for the category				
					XmlElement child = doc.CreateElement(@"Category");

					/// write the properties of this category
					child.SetAttribute(@"ElementName", category.ElementName);
					child.SetAttribute(@"HasChanges", XmlConvert.ToString(category.HasChanges));
					child.SetAttribute(@"Category", category.Category);
					child.SetAttribute(@"Description", category.Description);
					child.SetAttribute(@"DisplayName", category.DisplayName);
					child.SetAttribute(@"Hidden", XmlConvert.ToString(category.Hidden));
					
					/// append the child to our parent
					parent.AppendChild(child);

					/// write the options into the category
					this.WriteOptions(doc, child, category.Options, alwaysPersist);

					/// write recursively into the categories
					this.WriteCategories(doc, child, category.Categories, alwaysPersist);
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		
		/// <summary>
		/// Writes a collection of options to the XmlDocument
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="parent"></param>
		/// <param name="options"></param>
		/// <param name="alwaysPersist"></param>
		private void WriteOptions(XmlDocument doc, XmlElement parent, XmlConfigurationOptionCollection options, bool alwaysPersist)
		{
			try
			{
				/// write each option in this category
				foreach(XmlConfigurationOption option in options)
					this.WriteOption(doc, parent, option, alwaysPersist);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		
		/// <summary>
		/// Writes an option to the XmlDocument
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="parent"></param>
		/// <param name="option"></param>
		/// <param name="alwaysPersist"></param>
		private void WriteOption(XmlDocument doc, XmlElement parent, XmlConfigurationOption option, bool alwaysPersist)
		{
			try
			{
				if (option.Persistent || alwaysPersist)
				{
					/// create an element for the option
					XmlElement child = doc.CreateElement(@"Option");

					/// write the properties of this option
					child.SetAttribute(@"ElementName", option.ElementName);
					child.SetAttribute(@"HasChanges", XmlConvert.ToString(option.HasChanges));
					child.SetAttribute(@"Category", option.Category);
					child.SetAttribute(@"Description", option.Description);
					child.SetAttribute(@"DisplayName", option.DisplayName);				
					child.SetAttribute(@"Hidden", XmlConvert.ToString(option.Hidden));	
					child.SetAttribute(@"Readonly", XmlConvert.ToString(option.Readonly));
					child.SetAttribute(@"ShouldSerializeValue", XmlConvert.ToString(option.ShouldSerializeValue));
					child.SetAttribute(@"ValueAssemblyQualifiedName", option.ValueAssemblyQualifiedName);
//					child.SetAttribute(@"ReferencedAssemblyName", option.ReferencedAssemblyName);
					child.SetAttribute(@"EditorAssemblyQualifiedName", option.EditorAssemblyQualifiedName);
					
					/// create a text node for the value, as we are most likely unsure of what is contained in the actual value
					XmlText text = doc.CreateTextNode("Value");			

					/// try and serialize the value if we can, otherwise use the XmlConvert class to convert the value to a string
					if (option.ShouldSerializeValue)
						text.Value = this.GetSerializedValue(option);
					else
						text.Value = this.GetConvertableValue(option);

					/// add the text for the value to the element
					child.AppendChild(text);

					/// add the child to the parent
					parent.AppendChild(child);
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		
		/// <summary>
		/// Gets the value to write using the XmlConvert.ToString() method on the Option.Value property.
		/// </summary>
		/// <param name="option"></param>
		/// <returns></returns>
		private string GetConvertableValue(XmlConfigurationOption option)
		{
			try
			{
				if (option != null)
				{
					if (option.Value != null)
					{
						Type t = option.Value.GetType();
						if (t != null)
						{
							if (t == typeof(string))
								return (string)option.Value;

							if (t == typeof(bool))
								return XmlConvert.ToString((bool)option.Value);							

							if (t == typeof(int))
								return XmlConvert.ToString((int)option.Value);

							if (t == typeof(long))
								return XmlConvert.ToString((long)option.Value);
							
							if (t == typeof(decimal))
								return XmlConvert.ToString((decimal)option.Value);

							if (t == typeof(double))
								return XmlConvert.ToString((double)option.Value);

							if (t == typeof(byte))
								return XmlConvert.ToString((byte)option.Value);	

							if (t == typeof(char))
								return XmlConvert.ToString((char)option.Value);	

							if (t == typeof(System.DateTime))
								return XmlConvert.ToString((System.DateTime)option.Value);

							if (t == typeof(System.Guid))
								return XmlConvert.ToString((System.Guid)option.Value);

							if (t == typeof(short))
								return XmlConvert.ToString((short)option.Value);

							if (t == typeof(sbyte))
								return XmlConvert.ToString((sbyte)option.Value);

							if (t == typeof(float))
								return XmlConvert.ToString((float)option.Value);

							if (t == typeof(System.TimeSpan))
								return XmlConvert.ToString((System.TimeSpan)option.Value);

							if (t == typeof(ushort))
								return XmlConvert.ToString((ushort)option.Value);

							if (t == typeof(uint))
								return XmlConvert.ToString((uint)option.Value);

							if (t == typeof(ulong))
								return XmlConvert.ToString((ulong)option.Value);

							return option.Value.ToString();
						}
					}
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
				this.OnCannotWriteValue(this, new XmlConfigurationWriterEventArgs(systemException, option));
			}
			return string.Empty;
		}

		
		/// <summary>
		/// Gets the value to write using a BinaryFormatter to serialize the Object.Value property.
		/// </summary>
		/// <param name="option"></param>
		/// <returns></returns>
		private string GetSerializedValue(XmlConfigurationOption option)
		{
			string buffer = null;
			try
			{
				Type t = Type.GetType(option.ValueAssemblyQualifiedName);
				if (t != null)
				{
					if (EncodingEngine.Base64Encode(option.Value, t, out buffer))
						return buffer;
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
				this.OnCannotWriteValue(this, new XmlConfigurationWriterEventArgs(systemException, option));
			}
			return buffer;
		}
		
//		/// <summary>
//		/// Gets the value to write using a BinaryFormatter to serialize the Object.Value property.
//		/// </summary>
//		/// <param name="option"></param>
//		/// <returns></returns>
//		private string GetSerializedValue(XmlConfigurationOption option)
//		{
//			string buffer = null;
//			try
//			{
//				/// this should really use reflection to determine if the type supports serialization				
//				BinaryFormatter formatter = new BinaryFormatter();
//				MemoryStream stream = new MemoryStream();
//				formatter.Serialize(stream, option.Value);
//				buffer = System.Text.Encoding.ASCII.GetString(stream.GetBuffer());
//				stream.Close();
//			}
//			catch(System.Exception systemException)
//			{
//				System.Diagnostics.Trace.WriteLine(systemException);
//				this.OnCannotWriteValue(this, new XmlConfigurationWriterEventArgs(systemException, option));
//			}
//			return buffer;
//		}

		/// <summary>
		/// Raises the CannotWriteValue event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnCannotWriteValue(object sender, XmlConfigurationWriterEventArgs e)
		{
			try
			{
				if (this.CannotWriteValue != null)
					this.CannotWriteValue(sender, e);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}		
	}	
}
