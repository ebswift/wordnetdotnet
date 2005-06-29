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
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections;
using System.Security.Cryptography;

namespace Razor.Configuration
{
	/// <summary>
	/// Summary description for ConfigurationEngine.
	/// </summary>
	public class ConfigurationEngine
	{										
		private static System.Exception _lastException;

		/// <summary>
		/// Defines the valid category names found inside an InstallationEngine configuration
		/// </summary>
		private enum CategoryNames
		{
			Files
		}

		/// <summary>
		/// Gets the default name for an InstallationEngine configuration
		/// </summary>
		public const string DefaultConfigurationName = @"ConfigurationEngineConfiguration";

		/// <summary>
		/// Gets the default filename for an InstallationEngine configuration
		/// </summary>
		public const string DefaultConfigurationFilename = ConfigurationEngine.DefaultConfigurationName + ".xml";

		/// <summary>
		/// Gets an XmlConfiguration object that contains the default format (Categories and Name) for an InstallationEngine configuration
		/// </summary>
		public static XmlConfiguration DefaultConfigurationFormat
		{
			get
			{
				XmlConfiguration configuration = new XmlConfiguration();
				configuration.ElementName = ConfigurationEngine.DefaultConfigurationName;								
				configuration.Categories.Add(CategoryNames.Files.ToString());
				return configuration;
			}
		}
		
		/// <summary>
		/// Writes a configuration using the specified encryption engine to the specified path
		/// </summary>
		/// <param name="encryptionEngine">The encryption engine to use while writing the configuration, null if no encryption is desired</param>
		/// <param name="configuration">The confiruration to write</param>
		/// <param name="path">The path to write it to</param>
		/// <returns></returns>
		public static bool WriteConfiguration(FileEncryptionEngine encryptionEngine, XmlConfiguration configuration, string path)
		{	
			Stream stream = null;
			ConfigurationEngine.ResetLastException();
			try
			{
				if (configuration != null)
				{
					if (configuration.HasUnpersistedChanges())
					{
						configuration.AcceptChanges();						
						stream = (encryptionEngine != null ? encryptionEngine.CreateEncryptorStream(path) : new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None));
						XmlConfigurationWriter writer = new XmlConfigurationWriter();				
						writer.Write(configuration, stream, false);	
						configuration.SetHasUnpersistedChanges(false);
					}
					return true;			
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
				_lastException = systemException;
			}
			finally
			{
				try 
				{
					if (stream != null) stream.Close();
				}
				catch { }
			}
			return false;
		}

		/// <summary>
		/// Reads a configuration using the specified encryption engine from the specified path
		/// </summary>
		/// <param name="encryptionEngine">The encryption engine to use while reading the configuration, null if no decryption is desired</param>
		/// <param name="configuration">The configuration to be read into</param>
		/// <param name="path">The path to be read from</param>
		/// <returns></returns>
		public static bool ReadConfiguration(FileEncryptionEngine encryptionEngine, out XmlConfiguration configuration, string path)
		{
			Stream stream = null;
			ConfigurationEngine.ResetLastException();
			try
			{
				configuration = new XmlConfiguration();		
				stream = (encryptionEngine != null ? encryptionEngine.CreateDecryptorStream(path) : new FileStream(path, FileMode.Open, FileAccess.Read , FileShare.None));
				XmlConfigurationReader reader = new XmlConfigurationReader();
				configuration = reader.Read(stream);		
				configuration.Path = path;
				configuration.SetHasUnpersistedChanges(false);

				return true;	
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
				_lastException = systemException;
			}
			finally
			{
				try 
				{
					if (stream != null) stream.Close();
				}
				catch { }
			}
			configuration = null;
			return false;
		}

		/// <summary>
		/// Writes a configuration to a path using the specified encryption engine. Takes windows security into account and checks for write access before trying to write to the path.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="configuration"></param>
		/// <param name="encryptionEngine"></param>
		/// <returns></returns>
		public static bool WriteConfiguration(bool verbose, string path, XmlConfiguration configuration, FileEncryptionEngine encryptionEngine)
		{
			try
			{								
				Trace.WriteLineIf(verbose, "'Configuration Engine': Checking to see if the path '" + path + "' exists.");
				
				/// if the file exists, we need to try and read it
				if (System.IO.File.Exists(path))
				{
					Trace.WriteLineIf(verbose, "'Configuration Engine': The path '" + path + "' exists.");

					/// but first see if we have permissions to read it
					using(SecurityAccessRight right = new SecurityAccessRight(path))
					{
						Trace.WriteLineIf(verbose, "'Configuration Engine': Checking to see if the path '" + path + "' has write access.");

						/// if we don't have rights to the file						
						if (!right.AssertWriteAccess())
						{
							Trace.WriteLineIf(verbose, "'Configuration Engine': The path '" + path + "' does not have write access.");
							Trace.WriteLineIf(verbose, "'Configuration Engine': Prompting for user intervention for the path '" + path + "'.");

							if (verbose)
							{
								/// prompt to see what we should do about this
								DialogResult result = ExceptionEngine.DisplayException(
									null,
									"Write access denied - Unable to write to file",
									MessageBoxIcon.Error,
									MessageBoxButtons.AbortRetryIgnore,
									null,
									"Write access has been denied for the file '" + path + "'.",
									"Ignoring this exception may result in a loss of data if any options in this file were changed.");

								switch(result)
								{							
								case DialogResult.Abort:
									Trace.WriteLineIf(verbose, "'Configuration Engine': Aborting attempt to write to the path '" + path + "' because of user intervention.");
									return false;					

								case DialogResult.Retry:
									Trace.WriteLineIf(verbose, "'Configuration Engine': Retrying attempt to write to the path '" + path + "' because of user intervention.");
									return ConfigurationEngine.WriteConfiguration(verbose, path, configuration, encryptionEngine);					

								case DialogResult.Ignore:
									Trace.WriteLineIf(verbose, "'Configuration Engine': Ignoring attempt to write to the path '" + path + "' because of user intervention.");
									return true;
									//break;					
								};												
							}
							else
							{
								/// it failed, but we're not in verbose mode so who cares?
								return true;
							}
						}
						else
						{
							Trace.WriteLineIf(verbose, "'Configuration Engine': The path '" + path + "' has write access, preparing to write the configuration.");

							/// rights to write to the file
							/// ask the configuration engine to write our configuration file for us into our configuration 
							if (!ConfigurationEngine.WriteConfiguration(encryptionEngine, configuration, path))
							{
								Trace.WriteLineIf(verbose, "'Configuration Engine': Failed to write the configuration, throwing last exception from the ConfigurationEngine.");
								throw ConfigurationEngine.LastException;
							}
														
							/// ensure that the configuration has no changes visible
							if (configuration != null)
							{
								Trace.WriteLineIf(verbose, "'Configuration Engine': Succeeded in writing the configuration, accepting changes .");
								configuration.AcceptChanges();
							}

							return true;
						}
					}
				}
				else
				{
					Trace.WriteLineIf(verbose, "'Configuration Engine': The path '" + path + "' does not exist, preparing to write the configuration for the first time.");
					
					/// ask the configuration engine to write our configuration file for us into our configuration 
					if (!ConfigurationEngine.WriteConfiguration(encryptionEngine, configuration, path))
					{
						Trace.WriteLineIf(verbose, "'Configuration Engine': Failed to write the configuration, throwing last exception from the ConfigurationEngine.");
						throw ConfigurationEngine.LastException;
					}

					/// ensure that the configuration has no changes visible
					if (configuration != null)
					{
						Trace.WriteLineIf(verbose, "'Configuration Engine': Succeeded in writing the configuration, accepting changes .");
						configuration.AcceptChanges();
					}

					return true;
				}
			}
			catch(System.Exception systemException)
			{
				Trace.WriteLineIf(verbose, "'Configuration Engine': An unexpected exception was encountered while writing the configuration, dumping exception.");
				System.Diagnostics.Trace.WriteLine(systemException);
				Trace.WriteLineIf(verbose, "'Configuration Engine': Prompting for user intervention for the path '" + path + "'.");

				if (verbose)
				{
					/// failed for some reason writing the file
					/// prompt to see what we should do about this
					DialogResult result = ExceptionEngine.DisplayException(
						null,					
						"Exception encountered - Unable to write to file",
						MessageBoxIcon.Error,
						MessageBoxButtons.AbortRetryIgnore,
						systemException,
						"An exception was encountered while trying to write to the file '" + path + "'.",
						"Ignoring this exception may result in a loss of data if any options in this file were changed");							

					switch(result)
					{							
					case DialogResult.Abort:	
						Trace.WriteLineIf(verbose, "'Configuration Engine': Aborting attempt to write to the path '" + path + "' because of user intervention.");
						return false;					

					case DialogResult.Retry:
						Trace.WriteLineIf(verbose, "'Configuration Engine': Retrying attempt to write to the path '" + path + "' because of user intervention.");
						return ConfigurationEngine.WriteConfiguration(verbose, path, configuration, encryptionEngine);					

					case DialogResult.Ignore:
						Trace.WriteLineIf(verbose, "'Configuration Engine': Ignoring attempt to write to the path '" + path + "' because of user intervention.");
						return true;			
					};
				}
			}				
			return true;
		}

		/// <summary>
		/// Reads or creates an XmlConfiguration from a name, path, and/or a handler function to provide structure to a new configuration.
		/// </summary>
		/// <param name="name">The name that will be given to the configuration</param>
		/// <param name="path">The path to the file where the configuration is stored</param>
		/// <param name="configuration">The configuration that will be returned after creation or reading has finished</param>
		/// <param name="encryptionEngine">The encryption engine to use when reading the file</param>
		/// <param name="handler">The event handler to call if structure is needed for a new configuration</param>
		/// <returns>True if a configuration was created or read</returns>
		public static bool ReadOrCreateConfiguration(
			bool verbose,
			string name,
			string path, 
			out XmlConfiguration configuration, 
			FileEncryptionEngine encryptionEngine, 
			XmlConfigurationEventHandler handler)
		{
			configuration = null;

			Trace.WriteLineIf(verbose, "'Configuration Engine': Checking to see if the path '" + path + "' exists.");

			/// if the file exists, we need to try and read it
			if (System.IO.File.Exists(path))
			{	
				Trace.WriteLineIf(verbose, "'Configuration Engine': The path '" + path + "' exists.");

				try
				{
					/// but first see if we have permissions to read it
					using(SecurityAccessRight right = new SecurityAccessRight(path))
					{
						Trace.WriteLineIf(verbose, "'Configuration Engine': Checking to see if the path '" + path + "' has read access.");

						/// if we don't have rights to the file
						if (!right.AssertReadAccess())
						{
							Trace.WriteLineIf(verbose, "'Configuration Engine': The path '" + path + "' does not have write access.");
							Trace.WriteLineIf(verbose, "'Configuration Engine': Prompting for user intervention for the path '" + path + "'.");

							/// prompt to see what we should do about this
							DialogResult result = ExceptionEngine.DisplayException(
								null,
								"Read access denied - Unable to read from file",
								MessageBoxIcon.Error,
								MessageBoxButtons.AbortRetryIgnore,
								null,
								"Read access has been denied for the '" + name + "'.",
								"Ignoring this exception will result in a default set of options to be loaded in the '" + name + "' for this application instance.",					
								"If the file has write access enabled, it will be overwritten with the loaded options when the application exits.",					
								"WARNING: Aborting this operation will exit the application!");

							switch(result)
							{							
							case DialogResult.Abort:	
								Trace.WriteLineIf(verbose, "'Configuration Engine': Aborting attempt to read from the path '" + path + "' because of user intervention.");
								return false;					

							case DialogResult.Retry:
								Trace.WriteLineIf(verbose, "'Configuration Engine': Retrying attempt to read from the path '" + path + "' because of user intervention.");
								return ConfigurationEngine.ReadOrCreateConfiguration(verbose, name, path, out configuration, encryptionEngine, handler);					

							case DialogResult.Ignore:
								Trace.WriteLineIf(verbose, "'Configuration Engine': Ignoring attempt to read from the path '" + path + "' because of user intervention.");
								return true;
								//break;					
							};												
						}
						else
						{
							Trace.WriteLineIf(verbose, "'Configuration Engine': The path '" + path + "' has read access, preparing to read the configuration.");

							/// rights to read the file
							/// ask the configuration engine to read our configuration file for us into our configuration 
							if (!ConfigurationEngine.ReadConfiguration(encryptionEngine, out configuration, path))
							{
								Trace.WriteLineIf(verbose, "'Configuration Engine': Failed to read the configuration, throwing last exception from the ConfigurationEngine.");
								throw ConfigurationEngine.LastException;
							}

							/// let the configuration know where it lives							
							//							configuration.Path = path;

							/// ensure that the configuration has no changes visible
							if (configuration != null)
							{
								Trace.WriteLineIf(verbose, "'Configuration Engine': Succeeded in reading the configuration, accepting changes .");
								configuration.AcceptChanges();
							}

							return true;
						}
					}
				}
				catch(System.Exception systemException)
				{
					Trace.WriteLineIf(verbose, "'Configuration Engine': An unexpected exception was encountered while reading the configuration, dumping exception.");
					System.Diagnostics.Trace.WriteLine(systemException);
					Trace.WriteLineIf(verbose, "'Configuration Engine': Prompting for user intervention for the path '" + path + "'.");

					/// failed for some reason reading the file
					/// prompt to see what we should do about this
					DialogResult result = ExceptionEngine.DisplayException(
						null,					
						"Exception encountered - Unable to read from file",
						MessageBoxIcon.Error,
						MessageBoxButtons.AbortRetryIgnore,
						systemException,
						"An exception was encountered while trying to read '" + name + "'.",
						"Ignoring this exception will result in a default set of options to be loaded in the '" + name + "' for this application instance.",					
						"If the file has write access enabled, it will be overwritten with the loaded options when the application exits.",					
						"WARNING: Aborting this operation will exit the application!");

					switch(result)
					{							
					case DialogResult.Abort:	
						Trace.WriteLineIf(verbose, "'Configuration Engine': Aborting attempt to read from the path '" + path + "' because of user intervention.");
						return false;					

					case DialogResult.Retry:
						Trace.WriteLineIf(verbose, "'Configuration Engine': Retrying attempt to read from the path '" + path + "' because of user intervention.");
						return ConfigurationEngine.ReadOrCreateConfiguration(verbose, name, path, out configuration, encryptionEngine, handler);					

					case DialogResult.Ignore:
						Trace.WriteLineIf(verbose, "'Configuration Engine': Ignoring attempt to read from the path '" + path + "' because of user intervention.");
						break;			
					};
				}				
			}
			else
			{
				Trace.WriteLineIf(verbose, "'Configuration Engine': The path '" + path + "' does not exist.");
			}

			/// if for some reason the configuration hasn't been loaded yet
			if (configuration == null)
			{
				Trace.WriteLineIf(verbose, "'Configuration Engine': Creating new configuration named '" + name + "'.");
				configuration = new XmlConfiguration();
				configuration.ElementName = name;

				Trace.WriteLineIf(verbose, "'Configuration Engine': Checking for formatting callback for the configuration named '" + name + "'.");

				if (handler != null)
				{
					Trace.WriteLineIf(verbose, "'Configuration Engine': Formatting callback found for the configuration named '" + name + "', calling formatting callback to apply structure to the configuration.");
					try
					{
						XmlConfigurationEventArgs e = new XmlConfigurationEventArgs(configuration, XmlConfigurationElementActions.None);
						handler(null, e);
						configuration = e.Element;
					}
					catch(System.Exception systemException)
					{
						Trace.WriteLineIf(verbose, "'Configuration Engine': An unexpected exception was encountered while reading the configuration named '" + name + "', dumping exception.");
						System.Diagnostics.Trace.WriteLine(systemException);
					}
				}
			}
									
			Trace.WriteLineIf(verbose, "'Configuration Engine': Setting the path for the configuration named '" + name + "' and accepting changes to the configuration.");

			/// let the configuration know where it lives
			configuration.Path = path;

			/// ensure that the configuration has no changes visible
			configuration.AcceptChanges();

			return true;
		}

		/// <summary>
		/// Gets the last exception that occurred in the ConfigurationEngine
		/// </summary>
		public static System.Exception LastException
		{
			get
			{
				return _lastException;
			}
		}

		/// <summary>
		/// Resets the last exception in the ConfigurationEngine
		/// </summary>
		public static void ResetLastException()
		{
			_lastException = null;
		}
		
		/// <summary>
		/// Registers the file using the specified key in the specified configuration. If the key already exists, the file will be written to the existing key.
		/// </summary>
		/// <param name="configuration">The configuration in which the file will be registered</param>
		/// <param name="key">The key by which the file will be registered</param>
		/// <param name="filename">The filename that will be registered</param>
		/// <returns></returns>
		public static bool RegisterFile(XmlConfiguration configuration, string key, string filename)
		{
			ConfigurationEngine.ResetLastException();
			try
			{
				if (configuration != null)
				{
					if (key != null && key != string.Empty)
					{
						XmlConfigurationCategory category = configuration.Categories[CategoryNames.Files.ToString()];
						if (category != null)
						{	
							XmlConfigurationOption option = null;

							option = category.Options[key];
							if (option == null)
							{
								option = category.Options[key, true, filename];
								option.Category = @"Registered Files";
								option.Description = @"This indicates a configuration file that is registered and ready to be accessed via the ConfigurationEngine.";
							}
							option.Value = filename;
							return true;
						}
					}
				}
			}
			catch(System.Exception systemException)
			{
				_lastException = systemException;
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return false;
		}		

		/// <summary>
		/// Unregisters the file specified by the key supplied from the specified configuration
		/// </summary>
		/// <param name="configuration">The configuration in which the file will be unregistered</param>
		/// <param name="key">The key by which the file was originally registered</param>
		/// <returns></returns>
		public static bool UnregisterFile(XmlConfiguration configuration, string key)
		{
			ConfigurationEngine.ResetLastException();
			try
			{
				if (configuration != null)
				{
					if (key != null && key != string.Empty)
					{
						XmlConfigurationCategory category = configuration.Categories[CategoryNames.Files.ToString()];
						if (category != null)
						{	
							XmlConfigurationOption option = null;

							option = category.Options[key];
							if (option != null)
								category.Options.Remove(option);
							return true;
						}
					}
				}
			}
			catch(System.Exception systemException)
			{
				_lastException = systemException;
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return false;
		}		

		/// <summary>
		/// Determines if a file was registered using the specified key in the specified configuration
		/// </summary>
		/// <param name="configuration">The configuration in which the search will occur</param>
		/// <param name="key">The key by which the supposed file was registered</param>
		/// <returns></returns>
		public static bool IsFileRegistered(XmlConfiguration configuration, string key)
		{
			ConfigurationEngine.ResetLastException();
			try
			{
				if (configuration != null)
				{
					if (key != null && key != string.Empty)
					{
						XmlConfigurationCategory category = configuration.Categories[CategoryNames.Files.ToString()];
						if (category != null)
						{	
							XmlConfigurationOption option = null;

							option = category.Options[key];
							if (option != null)
								return true;
						}
					}
				}
			}
			catch(System.Exception systemException)
			{
				_lastException = systemException;
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return false;
		}

		/// <summary>
		/// Returns the filename using the specified key from the specified configuration 
		/// </summary>
		/// <param name="configuration">The configuration from which the filename will be extracted</param>
		/// <param name="key">The key by which the filename was saved</param>
		/// <returns></returns>
		public static string GetRegisteredFilename(XmlConfiguration configuration, string key)
		{
			ConfigurationEngine.ResetLastException();
			try
			{
				if (configuration != null)
				{
					if (key != null && key != string.Empty)
					{
						XmlConfigurationCategory category = configuration.Categories[CategoryNames.Files.ToString()];
						if (category != null)
						{	
							XmlConfigurationOption option = null;

							option = category.Options[key];
							if (option != null)
								return (string)option.Value;
						}
					}
				}
			}
			catch(System.Exception systemException)
			{
				_lastException = systemException;
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return string.Empty;
		}
		
		/// <summary>
		/// Returns a Hashtable containing the key and filename of each file that is registered in the specified configuration
		/// </summary>
		/// <param name="configuration">The configuration in which the search will occur</param>
		/// <returns></returns>
		public static Hashtable GetRegisteredFiles(XmlConfiguration configuration)
		{
			ConfigurationEngine.ResetLastException();
			try
			{
				if (configuration != null)
				{
					XmlConfigurationCategory category = configuration.Categories[CategoryNames.Files.ToString()];
					if (category != null)
					{	
						Hashtable table = new Hashtable();
						foreach(XmlConfigurationOption option in category.Options)
							table.Add(option.ElementName, (string)option.Value);
						return table;
					}
				}
			}
			catch(System.Exception systemException)
			{
				_lastException = systemException;
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return new Hashtable();
		}
	}
}
