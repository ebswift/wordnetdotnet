using System;

namespace Razor.Networking.AutoUpdate.Common
{
	/// <summary>
	/// Provides information relating to a downloadable .Update for a specific product
	/// </summary>
	public class AutoUpdateDownloadDescriptor
	{
		protected AutoUpdateManifest _manifest;
		protected AutoUpdateDownloader _downloader;
		protected AutoUpdateOptions _options;
		protected string _downloadedPath;

		/// <summary>
		/// Initializes a new instance of the AutoUpdateDownloadDescriptor class
		/// </summary>
		public AutoUpdateDownloadDescriptor()
		{
			_manifest = new AutoUpdateManifest();
			_downloader = null;
			_options = null;
		}

		/// <summary>
		/// Initializes a new instance of the AutoUpdateDownloadDescriptor class
		/// </summary>
		/// <param name="manifest">A manifest file containing information about the product, and a summary of the changes new to the version specified</param>
		/// <param name="downloader">The downloader that will be responsible for downloading the .update</param>
		/// <param name="options">The options to be used by the downloader while downloading the .update file</param>
		public AutoUpdateDownloadDescriptor(AutoUpdateManifest manifest, AutoUpdateDownloader downloader, AutoUpdateOptions options)
		{
			_manifest = manifest;
			_downloader = downloader;
			_options = options;						
		}

		#region My Public Properties

		/// <summary>
		/// Gets or sets the manifest that describes the update for the product it goes to, and the changes contained in it
		/// </summary>
		public AutoUpdateManifest Manifest
		{
			get
			{
				return _manifest;
			}
			set
			{
				_manifest = value;
			}
		}

		/// <summary>
		/// Gets or sets the manager that will be used to manage the update described by this descriptor
		/// </summary>
		public AutoUpdateDownloader Downloader
		{
			get
			{
				return _downloader;
			}
			set
			{
				_downloader = value;    
			}
		}
		
		/// <summary>
		/// Gets or sets the options the be used to control the behavior of the auto update
		/// </summary>
		public AutoUpdateOptions Options
		{
			get
			{
				return _options;
			}
			set
			{
				_options = value;
			}
		}		
		
		/// <summary>
		/// Gets or sets the full path including filename where the .update file was downloaded
		/// </summary>
		public string DownloadedPath
		{
			get
			{
				return _downloadedPath;
			}
			set
			{
				_downloadedPath = value;
			}
		}

		#endregion

		#region My Public Static Methods

		/// <summary>
		/// Bubble sorts the elements in the descriptor array using their product verion (The newest version will be at element 0).
		/// </summary>
		/// <param name="updates"></param>
		/// <returns></returns>
		public static AutoUpdateDownloadDescriptor[] Sort(AutoUpdateDownloadDescriptor[] updates)
		{
			// front to back - 1 
			for(int i = 0; i < updates.Length - 1; i++)
			{
				// front + 1 to back
				for(int j = i + 1; j < updates.Length; j++)
				{			
					if (updates[i].Manifest.Product.Version < updates[j].Manifest.Product.Version)
					{											 
						// swap i with j, where i=1 and j=2
						AutoUpdateDownloadDescriptor update = updates[j];
						updates[j] = updates[i];
						updates[i] = update;
					}													
				}
			}
			return updates;
		}

		#endregion
	}
}
