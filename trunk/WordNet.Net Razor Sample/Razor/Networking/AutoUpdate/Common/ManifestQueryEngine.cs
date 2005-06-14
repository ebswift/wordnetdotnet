using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Razor;
using Razor.Searching;

namespace Razor.Networking.AutoUpdate.Common
{
	/// <summary>
	/// Summary description for ManifestQueryEngine.
	/// </summary>
	public class ManifestQueryEngine
	{
		/// <summary>
		/// Queries the latest version of a particular product
		/// </summary>
		/// <param name="updatesPath">The path to the updates</param>
		/// <param name="productName">The name of the product to update</param>
		/// <param name="currentVersion">The current version of the product that is checking for updates</param>
		/// <param name="productId">The id of the product that is checking for updates</param>
		/// <returns></returns>		
		public static XmlDocument QueryLatestVersionEx(string updatesPath, string productName, string currentVersion, string productId)
		{
			/*
			 * we really don't need all the information, but it would be nice to log who is trying to update
			 * */			
			try
			{				
				// log information about this event to the system's event log
//				Debug.WriteLine(string.Format("The product '{0}' version '{1}' with Id '{2}' checked for updates at {3}.", productName, currentVersion, productId, DateTime.Now.ToString()));
				
				// there must be a path to the updates folder
				if (updatesPath == null || updatesPath == string.Empty)
					return null;

				// append the product name to the updates path 
				string path = Path.Combine(updatesPath, productName);

				// if the directory doesn't exist, bail with null
				if (!Directory.Exists(path))
					return null;
											
				// create a search for manifest files
				Search search = new Search("Manifest Files", path, "*.Manifest", false, false);

				// run the search
				FileInfo[] files = search.GetFiles();

				// create versioned files from the results
				VersionedFile[] versionedFiles = VersionedFile.CreateVersionedFiles(string.Format("{0}-", productName), files);

				// sort them
				versionedFiles = VersionedFile.Sort(versionedFiles);

				// grab the latest version
				VersionedFile latestVersion = VersionedFile.GetLatestVersion(versionedFiles);
				
				// assuming there is a version available
				if (latestVersion != null)
				{					
					// create a new xml document to hold the response
					XmlDocument doc = new XmlDocument();

					// load the document with the xml
					doc.Load(latestVersion.File.FullName);

					// return the doc, which will return the document element, 
					// which is the pure xml inside the soap headers of the web service response
					// skipping anymore encoding issues.
					return doc;				
				}				
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
			return null;
		}		
	}
}
