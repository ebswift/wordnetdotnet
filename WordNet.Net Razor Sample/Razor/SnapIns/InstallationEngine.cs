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
using System.Collections;
using Razor.Configuration;
using Razor.Attributes;

namespace Razor.SnapIns
{
	/// <summary>
	/// Contains the logic for tracking the installation history for SnapIns.
	/// </summary>
	public class InstallationEngine
	{
		/// <summary>
		/// Defines the valid category names found inside an InstallationEngine configuration
		/// </summary>
		public enum CategoryNames
		{
			Installed,
			Uninstalled
		}

		/// <summary>
		/// Gets the default name for an InstallationEngine configuration
		/// </summary>
		public const string DefaultConfigurationName = @"InstallationEngineConfiguration";

		/// <summary>
		/// Gets the default filename for an InstallationEngine configuration
		/// </summary>
		public const string DefaultConfigurationFilename = InstallationEngine.DefaultConfigurationName + ".xml";
			
		/// <summary>
		/// Gets an XmlConfiguration object that contains the default format (Categories and Name) for an InstallationEngine configuration
		/// </summary>
		public static XmlConfiguration DefaultConfigurationFormat
		{
			get
			{
				XmlConfiguration configuration = new XmlConfiguration();
				configuration.ElementName = InstallationEngine.DefaultConfigurationName;	
				XmlConfigurationCategory category = configuration.Categories[@"SnapIns", true];
				category.Categories.Add(CategoryNames.Installed.ToString());
				category.Categories.Add(CategoryNames.Uninstalled.ToString());
				return configuration;
			}
		}

		/// <summary>
		/// Parses the category specified in the configuration specified for the type specified, and returns an array of Versions that are listed in the category.
		/// </summary>
		/// <param name="configuration">The configuration to parse</param>
		/// <param name="type">The type to search for</param>
		/// <param name="categoryName">The category to search in</param>
		/// <returns></returns>
		public static Version[] GetListedVersionsOfType(XmlConfiguration configuration, Type type, CategoryNames categoryName)
		{
			try
			{				
				if (configuration != null)
				{					
					if (type != null)
					{
						// start in the specified category
						XmlConfigurationCategory searchCategory = configuration.Categories[@"SnapIns\" + categoryName.ToString()];
						if (searchCategory != null)
						{
							// enumerate the type category, each subcategory is the full name of a type that is listed
							foreach(XmlConfigurationCategory typeCategory in searchCategory.Categories)
							{
								// compare the category's element name to the full name of the type specified
								if (string.Compare(typeCategory.ElementName, type.FullName, false) == 0)
								{
									// enumerate the version category, each subcategory is the version number of the type
									ArrayList array = new ArrayList();
									foreach(XmlConfigurationCategory versionCategory in typeCategory.Categories)
									{
										Version v = new Version(versionCategory.ElementName);
										array.Add(v);
									}
									return array.ToArray(typeof(Version)) as Version[];
								}
							}
						}
					}
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}			
			return new Version[] {};
		}

		/// <summary>
		/// Parses the "Installed" category in the configuration specified for the type specified, and returns an array of Versions for each Version of the Type that is listed as "Installed"
		/// </summary>
		/// <param name="configuration">The configuration to parse</param>
		/// <param name="type">The type to search for</param>
		/// <returns></returns>
		public static Version[] GetInstalledVersionsOfType(XmlConfiguration configuration, Type type)
		{			
			return InstallationEngine.GetListedVersionsOfType(configuration, type, CategoryNames.Installed);
		}

		/// <summary>
		/// Parses the "Uninstalled" category in the configuration specified for the type specified, and returns an array of Versions for each Version of the Type that is listed as "Uninstalled"
		/// </summary>
		/// <param name="configuration">The configuration to parse</param>
		/// <param name="type">The type to search for</param>
		/// <returns></returns>
		public static Version[] GetUninstalledVersionsOfType(XmlConfiguration configuration, Type type)
		{
			return InstallationEngine.GetListedVersionsOfType(configuration, type, CategoryNames.Uninstalled);
		}

		/// <summary>
		/// Determines if the specified Type's Version (queried via attributes and reflection) is included in the array of Versions specified
		/// </summary>
		/// <param name="versions">The array of versions to search</param>
		/// <param name="type">The type to search for</param>
		/// <returns></returns>
		public static bool IsVersionOfTypeIncluded(Version[] versions, Type type)
		{
			try
			{
				if (type != null)
				{
					SnapInAttributeReader reader = new SnapInAttributeReader(type);
					if (reader != null)
					{
						SnapInVersionAttribute versionAttribute = reader.GetSnapInVersionAttribute();
						if (versionAttribute != null)
						{
							foreach(Version version in versions)
							{
								if (Version.Equals(version, versionAttribute.Version))
									return true;
							}
						}
					}					
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return false;
		}

		/// <summary>
		/// Determines if the specified Type's Version is an upgrade from the specified versions
		/// </summary>
		/// <param name="versions">The array of versions to compare</param>
		/// <param name="type">The type whos version to compare against</param>
		/// <returns></returns>
		public static bool IsVersionOfTypeAnUpgrade(Version[] versions, Type type)
		{
			try
			{
				if (type != null)
				{
					SnapInAttributeReader reader = new SnapInAttributeReader(type);
					if (reader != null)
					{
						SnapInVersionAttribute versionAttribute = reader.GetSnapInVersionAttribute();
						if (versionAttribute != null)
						{
							bool newer = true;
							foreach(Version version in versions)
							{				
								if (version > versionAttribute.Version)
									newer = false;								
							}
							return newer;
						}
					}					
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return false;
		}

		/// <summary>
		/// Returns a category for the type including it's version (defaulted to "1.0.0.0" for versionless types) in the specified configuration, using the specified type, and specified category name.
		/// </summary>
		/// <param name="configuration">The configuration in which the category will be found</param>
		/// <param name="type">The type for which the category will be named</param>
		/// <param name="categoryName">The category name in which version will be found</param>
		/// <returns></returns>
		public static XmlConfigurationCategory GetExistingCategoryForTypeVersion(XmlConfiguration configuration, Type type, CategoryNames categoryName)
		{
			try
			{
				if (configuration != null)
				{					
					if (type != null)
					{
						// start in the specified category
						XmlConfigurationCategory searchCategory = configuration.Categories[@"SnapIns\" + categoryName.ToString()];
						if (searchCategory != null)
						{
							SnapInAttributeReader reader = new SnapInAttributeReader(type);
							if (reader != null)
							{
								Version version = null;
								SnapInVersionAttribute versionAttribute = reader.GetSnapInVersionAttribute();
								if (versionAttribute != null)
									version = versionAttribute.Version;
								else
									version = new Version(1, 0, 0, 0);
								
								string path = string.Format("{0}\\{1}\\{2}", @"SnapIns\" + categoryName.ToString(), type.FullName, version.ToString());
								return configuration.Categories[path, false];
							}
						}
					}
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return null;
		}

		/// <summary>
		/// Creates a category for the type including it's version (defaulted to "1.0.0.0" for versionless types) in the specified configuration, using the specified type, and specified category name.
		/// </summary>
		/// <param name="configuration">The configuration in which the category will be created</param>
		/// <param name="type">The type for which the category will be created</param>
		/// <param name="categoryName">The category name in which creation will occur</param>
		/// <returns></returns>
		public static XmlConfigurationCategory CreateCategoryForTypeVersion(XmlConfiguration configuration, Type type, CategoryNames categoryName)
		{
			try
			{
				if (configuration != null)
				{					
					if (type != null)
					{
						// start in the specified category
						XmlConfigurationCategory searchCategory = configuration.Categories[@"SnapIns\" + categoryName.ToString()];
						if (searchCategory != null)
						{
							SnapInAttributeReader reader = new SnapInAttributeReader(type);
							if (reader != null)
							{
								Version version = null;
								SnapInVersionAttribute versionAttribute = reader.GetSnapInVersionAttribute();
								if (versionAttribute != null)
									version = versionAttribute.Version;
								else
									version = new Version(1, 0, 0, 0);
								
								string path = string.Format("{0}\\{1}\\{2}", @"SnapIns\" + categoryName.ToString(), type.FullName, version.ToString());
								return configuration.Categories[path, true];
							}
						}
					}
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return null;
		}

		/// <summary>
		/// Creates an installation record for the specified Type
		/// </summary>
		/// <param name="configuration">The configuration in which the category will be created</param>
		/// <param name="type">The type for which the category will be created</param>
		/// <returns></returns>
		public static XmlConfigurationCategory InstallVersionOfType(XmlConfiguration configuration, Type type, out DateTime installDate)
		{
			installDate = DateTime.Now;

			// remove the uninstall entry for the type
			InstallationEngine.RemoveUninstalledEntryForType(configuration, type);

			// create the install entry for the type
			XmlConfigurationCategory category = InstallationEngine.CreateCategoryForTypeVersion(configuration, type, CategoryNames.Installed);
			if (category != null)
			{
				XmlConfigurationOption option = null;

				// create the install date 
				option = category.Options[@"InstallDate"];
				if (option == null)
				{					
					option = category.Options[@"InstallDate", true, installDate];
					option.Category = @"Installation Notes";
					option.Description = @"This date marks the date and time on which the SnapIn was installed.";
					option.ShouldSerializeValue = true;
				}
				option = null;
				
				// create the run count
				option = category.Options[@"RunCount"];
				if (option == null)
				{
					option = category.Options[@"RunCount", true, 0];
					option.Category = @"Installation Notes";
					option.Description = @"This number indicates the number of times the SnapIn has executed.";
				}
				option = null;
			}
			return category;
		}

		/// <summary>
		/// Creates an uninstallation record for the specified Type
		/// </summary>
		/// <param name="configuration">The configuration in which the category will be created</param>
		/// <param name="type">The type for which the category will be created</param>
		/// <returns></returns>
		public static XmlConfigurationCategory UninstallVersionOfType(XmlConfiguration configuration, Type type)
		{
			// remove the install entry for the type
			InstallationEngine.RemoveInstalledEntryForType(configuration, type);

			// create the uninstall entry for the type
			XmlConfigurationCategory category = InstallationEngine.CreateCategoryForTypeVersion(configuration, type, CategoryNames.Uninstalled);
			if (category != null)
			{
				XmlConfigurationOption option = null;

				// create the install date 
				option = category.Options[@"UninstallDate"];
				if (option == null)
				{
					option = category.Options[@"UninstallDate", true, DateTime.Now.ToString()];
					option.Category = @"Installation Notes";
					option.Description = @"This date marks the date and time on which the SnapIn was uninstalled.";
				}
				option = null;
			}
			return category;
		}

		/// <summary>
		/// Attempts to increment the RunCount option for the specified version of the specified Type
		/// </summary>
		/// <param name="configuration">The configuration in which the option will be found</param>
		/// <param name="type">The Type for which the RunCount value will be incremented</param>
		/// <returns></returns>
		public static bool IncrementRunCountForVersionOfType(XmlConfiguration configuration, Type type, out DateTime installDate, out int runCount)
		{
			installDate = DateTime.Now;
			runCount = 0;
			try
			{
				XmlConfigurationCategory category = InstallationEngine.GetExistingCategoryForTypeVersion(configuration, type, CategoryNames.Installed);
				if (category != null)
				{
					XmlConfigurationOption option = null;
					
					option = category.Options[@"InstallDate"];
					if (option != null)
					{
						installDate = (DateTime)option.Value;
					}

					option = category.Options[@"RunCount"];
					if (option != null)
					{
						// retrieve, increment, and set the run count
						runCount = (int)option.Value;
						runCount++; 
						option.Value = runCount;												
					}
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}	
			return false;
		}

		public static bool RemoveInstalledEntryForType(XmlConfiguration configuration, Type type)
		{
			XmlConfigurationCategory category = null;
			
			// find and remove the install category for this type
			category = InstallationEngine.GetExistingCategoryForTypeVersion(configuration, type, CategoryNames.Installed);
			if (category != null)
			{
				category.Remove();
				category = null;
			}
			return true;
		}

		public static bool RemoveUninstalledEntryForType(XmlConfiguration configuration, Type type)
		{
			XmlConfigurationCategory category = null;
			
			// find and remove the install category for this type
			category = InstallationEngine.GetExistingCategoryForTypeVersion(configuration, type, CategoryNames.Uninstalled);
			if (category != null)
			{
				category.Remove();
				category = null;
			}
			return true;
		}
	}
}
