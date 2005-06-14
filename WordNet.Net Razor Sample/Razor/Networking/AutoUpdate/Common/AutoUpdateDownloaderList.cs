using System;
using System.Diagnostics;
using System.Collections;

namespace Razor.Networking.AutoUpdate.Common
{
	/// <summary>
	/// Summary description for AutoUpdateDownloaderList.
	/// </summary>
	public class AutoUpdateDownloaderList : CollectionBase 
	{
		/// <summary>
		/// Initializes a new instance of the AutoUpdateDownloaderList class
		/// </summary>
		public AutoUpdateDownloaderList()
		{
			
		}

		public void Add(AutoUpdateDownloader downloader)
		{
			if (downloader ==  null)
				throw new ArgumentNullException("downloader");

			if (this.Contains(downloader))
				throw new AutoUpdateDownloaderAlreadyExistsException(downloader);

			base.InnerList.Add(downloader);
		}

		public void AddRange(AutoUpdateDownloader[] downloaders)
		{
			if (downloaders == null)
				throw new ArgumentNullException("downloaders");

			foreach(AutoUpdateDownloader downloader in downloaders)
				this.Add(downloader);
		}

		public void Remove(AutoUpdateDownloader downloader)
		{
			if (downloader ==  null)
				throw new ArgumentNullException("downloader");

			if (this.Contains(downloader))
				base.InnerList.Remove(downloader);
		}

		public bool Contains(AutoUpdateDownloader downloader)
		{
			if (downloader ==  null)
				throw new ArgumentNullException("downloader");

			foreach(AutoUpdateDownloader existingDownloader in base.InnerList)
				if (string.Compare(existingDownloader.Id, downloader.Id, true) == 0)
					return true;

			return false;
		}

		public AutoUpdateDownloader this[int index]
		{
			get
			{
				return base.InnerList[index] as AutoUpdateDownloader;
			}
		}

		public AutoUpdateDownloader this[string id]
		{
			get
			{
				foreach(AutoUpdateDownloader existingDownloader in base.InnerList)
					if (string.Compare(existingDownloader.Id, id, true) == 0)
						return existingDownloader;

				return null;
			}
		}
	}
}
