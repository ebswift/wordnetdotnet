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
using System.IO;
using System.Collections;
using System.ComponentModel;

namespace Razor.Configuration
{
	/// <summary>
	/// Summary description for XmlConfigurationCollection.
	/// </summary>
	public class XmlConfigurationCollection : CollectionBase
	{
		/// <summary>
		/// Initializes a new instance of the XmlConfigurationCollection class
		/// </summary>
		public XmlConfigurationCollection()
		{
			//
			// TODO: Add constructor logic here
			//
		}

//		public XmlConfigurationCollection(XmlConfiguration[] configurations)
//		{
//			this.Add(configurations);
//		}
		
		public XmlConfigurationCollection(params XmlConfiguration[] configurations)
		{
			this.Add(configurations);
		}

		public int Add(XmlConfiguration configuration)
		{
			if (this.Contains(configuration))
				throw new ArgumentException("ElementName already exists. ElementName in collection: " + configuration.ElementName + " ElementName being added: " + configuration.ElementName);

			int index = base.InnerList.Add(configuration);
			
			/// bind to events 

			return 0;
		}
		
		public void Add(XmlConfiguration[] configurations)
		{
			if (configurations != null)
			{
				//				throw new ArgumentNullException("configuration");

				foreach(XmlConfiguration configuration in configurations)
				{
					try
					{
						this.Add(configuration);
					}
					catch(System.Exception systemException)
					{
						System.Diagnostics.Trace.WriteLine(systemException);
					}
				}
			}
		}

		public void Remove(XmlConfiguration configuration)
		{
			if (this.Contains(configuration))
			{
				base.InnerList.Remove(configuration);
			}
		}

		public bool Contains(XmlConfiguration configuration)
		{
			foreach(XmlConfiguration config in base.InnerList)
				if (config.ElementName == configuration.ElementName)
					return true;
			return false;
		}

		public XmlConfiguration[] ToArray()
		{
			return base.InnerList.ToArray(typeof(XmlConfiguration)) as XmlConfiguration[];
		}
		
		/// <summary>
		/// Gets or sets an XmlConfiguration using an index
		/// </summary>
		[Browsable(false)]
		public XmlConfiguration this[int index]
		{
			get
			{
				return base.InnerList[index] as XmlConfiguration;
			}
			set
			{
				base.InnerList[index] = value;
			}
		}

		/// <summary>
		/// Gets or sets an XmlConfiguration using a elementName
		/// </summary>
		/// <param name="elementName">The elementName of the configuration to get or set</param>
		public XmlConfiguration this[string elementName]
		{
			get
			{
				foreach(XmlConfiguration configuration in base.InnerList)
					if (configuration.ElementName == elementName)
						return configuration;
				return null;
			}
			set
			{
				for(int i = 0; i < base.InnerList.Count; i++)
					if (((XmlConfiguration)base.InnerList[i]).ElementName == value.ElementName)
					{
						base.InnerList[i] = value;
						return;
					}
			}
		}		
		
		/// <summary>
		/// Gets an XmlConfiguration using a elementName, optionally creates a new or loads an existing configuration (.xml) file using the path specified, and optionally adds it to the collection if new 
		/// </summary>
		/// <param name="elementName">The elementName by which this collection will be accessed</param>
		/// <param name="createIfNotFound">A flag indicating whether the configuration should be created if it does not exist</param>
		/// <param name="path">The path to the configuration</param>		
		public XmlConfiguration this[string elementName, bool createIfNotFound, bool addToCollectionIfNew, string path]
		{
			get
			{
				/// try and find the configuration in the collection
				XmlConfiguration configuration = this[elementName];
				
				/// if it's not in the collection
				if (configuration == null)
				{
					/// perhaps it does in the filesystem, if so then read it, and optionally add it to the collection
					if (File.Exists(path))
					{
						try
						{
							FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
							XmlConfigurationReader reader = new XmlConfigurationReader();
							configuration = reader.Read(stream);
							configuration.Path = path;								
							stream.Close();

							if (addToCollectionIfNew)
								this.Add(configuration);

							return configuration;
						} 
						catch (System.Exception systemException)
						{
							System.Diagnostics.Trace.WriteLine(systemException);
						}
					}

					/// apparently it doesnt' exist in the filesystem
					if (createIfNotFound)
					{
						/// so create a new file
						configuration = new XmlConfiguration();						
						configuration.Path = path;	
						configuration.ElementName = elementName;

						/// save the blank config
						Directory.CreateDirectory(path);
						FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write);
						XmlConfigurationWriter writer = new XmlConfigurationWriter();
						writer.Write(configuration, stream, false);
						stream.Close();						

						/// add it to the config if so instructed
						if (addToCollectionIfNew)
							this.Add(configuration);
					}
				}
				return configuration;
			}
		}		
	}
}
