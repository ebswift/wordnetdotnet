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
using System.Xml.XPath;
using System.Runtime.Serialization.Formatters.Binary;

namespace Razor.Configuration
{
	/// <summary>
	/// This component is responsible for reading an XmlConfiguration from a System.IO.Stream.
	/// </summary>
	public class XmlConfigurationReader : System.ComponentModel.Component
	{
		/// <summary>
		/// Occurs when an exception is encountered while reading the value of an option
		/// </summary>
		public event XmlConfigurationReaderEventHandler CannotReadValue;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Initializes a new instance of the XmlConfigurationReader class
		/// </summary>
		/// <param name="container"></param>
		public XmlConfigurationReader(System.ComponentModel.IContainer container)
		{
			///
			/// Required for Windows.Forms Class Composition Designer support
			///
			container.Add(this);
			InitializeComponent();		
		}

		/// <summary>
		/// Initializes a new instance of the XmlConfigurationReader class
		/// </summary>
		public XmlConfigurationReader()
		{
			///
			/// Required for Windows.Forms Class Composition Designer support
			///
			InitializeComponent();
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
		/// Reads an XmlConfiguration from a System.IO.Stream
		/// </summary>
		/// <param name="stream">The stream that contains the XmlConfiguration file</param>
		/// <returns></returns>
		public XmlConfiguration Read(Stream stream)
		{
			try
			{
				// create a new xml document
				XmlDocument doc = new XmlDocument();
				
				// load it from the stream
				doc.Load(stream);

				// create a new xpath navigator so that we can traverse the elements inside the xml
				XPathNavigator navigator = doc.CreateNavigator();
				
				// move to the version element
				navigator.MoveToFirstChild(); 

				// move to the file format description element
				navigator.MoveToNext();						

				// create a new configuration
				XmlConfiguration configuration = new XmlConfiguration();
				
				// begin initialization so that events do not fire inside the configuration
				configuration.BeginInit();

				// using the xpath navigator, read the xml document and turn it into a configuration
				this.ReadConfiguration(navigator, configuration);
				
				// end initialization
				configuration.EndInit();

				return configuration;
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}		
			return null;
		}
		
		/// <summary>
		/// Reads an XmlConfiguration from a System.IO.Stream
		/// </summary>
		/// <param name="stream">The stream that contains the XmlConfiguration file</param>
		/// <param name="configuration">The configuration to load into</param>
		/// <returns></returns>
		public XmlConfiguration Read(Stream stream, XmlConfiguration configuration)
		{
			try
			{
				/// create a new xml document
				XmlDocument doc = new XmlDocument();

				/// load it from the stream
				doc.Load(stream);

				/// create a new xpath navigator so that we can traverse the elements inside the xml
				XPathNavigator navigator = doc.CreateNavigator();
				
				/// move to the version element
				navigator.MoveToFirstChild(); 

				/// move to the file format description element
				navigator.MoveToNext();	
					
				this.ReadConfiguration(navigator, configuration);

				return configuration;
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}		
			return null;
		}

		/// <summary>
		/// Reads a configuration using the specified XPathNavigator
		/// </summary>
		/// <param name="navigator"></param>
		/// <returns></returns>
		private XmlConfiguration ReadConfiguration(XPathNavigator navigator, XmlConfiguration configuration)
		{
			if (navigator.MoveToFirstChild())
			{
				if (string.Compare(navigator.Name, @"Configuration", true) == 0)
				{
					// does the cateogry have attributes, it should!
					if (navigator.HasAttributes)
					{
						// break off yet another clone to navigate the attributes of this element
						XPathNavigator attributesNavigator = navigator.Clone();
						if (attributesNavigator.MoveToFirstAttribute())
						{
							configuration.ElementName = attributesNavigator.Value;

							while(attributesNavigator.MoveToNextAttribute())
							{
								switch(attributesNavigator.Name)
								{
								case @"HasChanges":
									configuration.HasChanges = XmlConvert.ToBoolean(attributesNavigator.Value);
									break;
								case @"Category":
									configuration.Category = attributesNavigator.Value;
									break;
								case @"Description":
									configuration.Description = attributesNavigator.Value;
									break;
								case @"DisplayName":
									configuration.DisplayName = attributesNavigator.Value;
									break;
								case @"Hidden":
									configuration.Hidden = XmlConvert.ToBoolean(attributesNavigator.Value);
									break;
								};						
							}
						}
					}
				}
			}

			// recursively read the categories within this configuration file
			this.ReadCategories(navigator, configuration.Categories);

			return configuration;
		}

		/// <summary>
		/// Reads an XmlConfigurationCategoryCollection using the specified XPathNavigator
		/// </summary>
		/// <param name="navigator"></param>
		/// <returns></returns>
		private XmlConfigurationCategoryCollection ReadCategories(XPathNavigator navigator, XmlConfigurationCategoryCollection categories)
		{			
			if (navigator.HasChildren)
			{
				if (navigator.MoveToFirstChild())
				{
					// is this element a category node?
					if (string.Compare(navigator.Name, @"Category", true) == 0)
					{
						// so read it
						XmlConfigurationCategory category = new XmlConfigurationCategory();
						category.BeginInit();
						category.Parent = categories;
						
						this.ReadCategory(navigator, category);						
						
						// and add it to the current collection of categories
						categories.Add(category);
						category.EndInit();
					}					
				}
			}

			while (navigator.MoveToNext())
			{	
				// is this element a category node?
				if (string.Compare(navigator.Name, @"Category", true) == 0)
				{
					// so read it
					XmlConfigurationCategory category = new XmlConfigurationCategory();
					category.BeginInit();
					category.Parent = categories;
					
					this.ReadCategory(navigator, category);					

					// and add it to the current collection of categories
					categories.Add(category);
					category.EndInit();
				}
			}
			
			return categories;
		}

		/// <summary>
		/// Reads an XmlConfigurationCategory using the specified XPathNavigator
		/// </summary>
		/// <param name="navigator"></param>
		/// <returns></returns>
		private XmlConfigurationCategory ReadCategory(XPathNavigator navigator, XmlConfigurationCategory category)
		{
			// break off a clone so that the starting cursor doesn't lose it's place
			XPathNavigator categoryNavigator = navigator.Clone();

			// does the cateogry have attributes, it should!
			if (categoryNavigator.HasAttributes)
			{
				// break off yet another clone to navigate the attributes of this element
				XPathNavigator attributesNavigator = categoryNavigator.Clone();
				if (attributesNavigator.MoveToFirstAttribute())
				{
					category.ElementName = attributesNavigator.Value;

					while(attributesNavigator.MoveToNextAttribute())
					{
						switch(attributesNavigator.Name)
						{
						case @"HasChanges":
							category.HasChanges = XmlConvert.ToBoolean(attributesNavigator.Value);
							break;
						case @"Category":
							category.Category = attributesNavigator.Value;
							break;
						case @"Description":
							category.Description = attributesNavigator.Value;
							break;
						case @"DisplayName":
							category.DisplayName = attributesNavigator.Value;
							break;
						case @"Hidden":
							category.Hidden = XmlConvert.ToBoolean(attributesNavigator.Value);
							break;
						};						
					}
				}
			}

			XmlConfigurationOption option = null;
			XPathNavigator optionNavigator = navigator.Clone();
			if (optionNavigator.HasChildren)
			{
				if (optionNavigator.MoveToFirstChild())
				{										
					option = new XmlConfigurationOption();
					option.BeginInit();
					if (this.ReadOption(optionNavigator, option) != null)
						category.Options.Add(option);											
					option.EndInit();
					
					while (optionNavigator.MoveToNext())
					{
						option = new XmlConfigurationOption();
						option.BeginInit();
						if (this.ReadOption(optionNavigator, option) != null)
							category.Options.Add(option);						
						option.EndInit();
					}					
				}
			}

			if (navigator.HasChildren)
			{
				this.ReadCategories(categoryNavigator, category.Categories);	
			}
						
			return category;
		}	

		private XmlConfigurationOption ReadOption(XPathNavigator navigator, XmlConfigurationOption option)
		{
			string value = null;

			try
			{
				if (string.Compare(navigator.Name, @"Option", true) == 0)
				{
					#region Attributes

					XPathNavigator optionNavigator = navigator.Clone();	
				
					value = optionNavigator.Value;

					if (optionNavigator.HasAttributes)
					{
						XPathNavigator attributesNavigator = optionNavigator.Clone();
						if (attributesNavigator.MoveToFirstAttribute())
						{
							option.ElementName = attributesNavigator.Value;

							while(attributesNavigator.MoveToNextAttribute())
							{
								switch(attributesNavigator.Name)
								{
									case @"HasChanges":
										option.HasChanges = XmlConvert.ToBoolean(attributesNavigator.Value);
										break;
									case @"Category":
										option.Category = attributesNavigator.Value;
										break;
									case @"Description":
										option.Description = attributesNavigator.Value;
										break;
									case @"DisplayName":
										option.DisplayName = attributesNavigator.Value;
										break;
									case @"Hidden":
										option.Hidden = XmlConvert.ToBoolean(attributesNavigator.Value);
										break;
									case @"Readonly":
										option.Readonly = XmlConvert.ToBoolean(attributesNavigator.Value);
										break;
									case @"ShouldSerializeValue":
										option.ShouldSerializeValue = XmlConvert.ToBoolean(attributesNavigator.Value);
										break;
									case @"ValueAssemblyQualifiedName":
										option.ValueAssemblyQualifiedName = attributesNavigator.Value;
										break;
									case @"EditorAssemblyQualifiedName":
										option.EditorAssemblyQualifiedName = attributesNavigator.Value;
										break;
								};						
							}
						}
					}
					
					#endregion

					#region Value

					// if the option is serialized
					if (option.ShouldSerializeValue)
					{
						// it should be encoded in base 64, so decode it
						option.Value = this.GetSerializedValue(option, value);
						return option;
					}
					
					// otherwise figure out why type of object it is
					Type t = TypeLoader.GetType(option);
					if (t != null)
					{
						if (t.IsEnum)
						{
							option.Value = Enum.Parse(t, value, true);	
							return option;
						}
						
						if (t == typeof(System.String))
							option.Value = (string)value;
						if (t == typeof(System.Boolean))
							option.Value = (object) XmlConvert.ToBoolean(value);
						if (t == typeof(System.Int32))
							option.Value = XmlConvert.ToInt32(value);
						if (t == typeof(System.Int64))
							option.Value = XmlConvert.ToInt64(value);
						if (t == typeof(System.Decimal))
							option.Value = XmlConvert.ToDecimal(value);
						if (t == typeof(System.Double))
							option.Value = XmlConvert.ToDouble(value);
						if (t == typeof(System.Byte))
							option.Value = XmlConvert.ToByte(value);
						if (t == typeof(System.Char))
							option.Value = XmlConvert.ToChar(value);
						if (t == typeof(System.DateTime))
							option.Value = XmlConvert.ToDateTime(value);
						if (t == typeof(System.Guid))
							option.Value = XmlConvert.ToGuid(value);
						if (t == typeof(System.Int16))
							option.Value = XmlConvert.ToInt16(value);
						if (t == typeof(System.SByte))
							option.Value = XmlConvert.ToSByte(value);
						if (t == typeof(System.Single))
							option.Value = XmlConvert.ToSingle(value);
						if (t == typeof(System.UInt16))
							option.Value = XmlConvert.ToUInt16(value);
						if (t == typeof(System.UInt32))
							option.Value = XmlConvert.ToUInt32(value);
						if (t == typeof(System.UInt64))
							option.Value = XmlConvert.ToUInt64(value);						
					}
										
					#endregion

					return option;
				}
			}
			catch(Exception ex)
			{								
				this.OnCannotReadValue(this, new XmlConfigurationReaderEventArgs(ex, option, value));
//				Debug.WriteLine(ex);				
			}
			return null;
		}

		/// <summary>
		/// Deserializes an object assuming that the string contains the base64 encoded data for the object
		/// </summary>
		/// <param name="option"></param>
		/// <param name="buffer"></param>
		/// <returns></returns>
		private object GetSerializedValue(XmlConfigurationOption option, string buffer)
		{			
			object instance = null;
			try
			{
				// try and base 64 decode the string
				if (EncodingEngine.Base64Decode(buffer, out instance))
					return instance;
			}
			catch(Exception ex)
			{				
				this.OnCannotReadValue(this, new XmlConfigurationReaderEventArgs(ex, option, buffer));
//				Debug.WriteLine(ex);
			}
			return instance;
		}

		/// <summary>
		/// Raises the CannotWriteValue event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnCannotReadValue(object sender, XmlConfigurationReaderEventArgs e)
		{
			try
			{
				string message = string.Format("The value for the option '{0}' in the category '{1}' could not be read due to the following exception.\n\t{2}\n\tThe buffer contained '{3}' when the exception was thrown.", e.Option.ElementName, e.Option.Category, e.Exception, e.Buffer);
				Trace.WriteLine(message);
			}
			catch(Exception) {}

			try
			{			
				if (this.CannotReadValue != null)
					this.CannotReadValue(sender, e);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}
	}
}
