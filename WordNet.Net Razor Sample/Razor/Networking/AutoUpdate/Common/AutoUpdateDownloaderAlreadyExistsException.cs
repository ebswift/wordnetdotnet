using System;

namespace Razor.Networking.AutoUpdate.Common
{
	/// <summary>
	/// Summary description for AutoUpdateDownloaderAlreadyExistsException.
	/// </summary>
	public class AutoUpdateDownloaderAlreadyExistsException : Exception
	{
		protected AutoUpdateDownloader _downloader;

		public AutoUpdateDownloaderAlreadyExistsException(AutoUpdateDownloader downloader) : base("A downloader with the same Id already exists.")		
		{
			_downloader = downloader;
		}

		public AutoUpdateDownloader Downloader
		{
			get
			{
				return _downloader;
			}
		}
	}
}
