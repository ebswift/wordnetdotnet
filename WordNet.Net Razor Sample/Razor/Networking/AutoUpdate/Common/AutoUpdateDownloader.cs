using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using Razor;

namespace Razor.Networking.AutoUpdate.Common
{
	/// <summary>
	/// Summary description for AutoUpdateDownloader.
	/// </summary>
	public class AutoUpdateDownloader
	{
		protected string _id;
		protected int _segmentSize;
		public const int DefaultSegmentSize = 8192;

		/// <summary>
		/// Initializes a new instance of the AutoUpdateDownloader class
		/// </summary>
		public AutoUpdateDownloader()
		{
			_id = Guid.NewGuid().ToString();
			_segmentSize = DefaultSegmentSize;
		}

		#region My Public Properties

		/// <summary>
		/// Gets or sets the unique identier associated with this AutoUpdate Manger
		/// </summary>	
		public string Id
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the segment size used during downloading
		/// </summary>
		public int SegmentSize
		{
			get
			{
				return _segmentSize;
			}
			set
			{
				_segmentSize = value;
			}
		}

		#endregion

		#region My Public Virtual Methods

		/// <summary>
		/// Instructs the AutoUpdateDownloader to query for the latest version available 
		/// </summary>
		/// <param name="progressViewer">The progress viewer by which progress should be displayed</param>
		/// <param name="options">The options that affect this downloader</param>
		/// <param name="productToUpdate">The product descriptor for the product that should be updated</param>
		/// <param name="updateAvailable">The download descriptor that describes the download that could potentially occur</param>
		/// <returns></returns>
		public virtual bool QueryLatestVersion(
			IProgressViewer progressViewer,
			AutoUpdateOptions options,
			AutoUpdateProductDescriptor productToUpdate, 
			out AutoUpdateDownloadDescriptor updateAvailable)
		{			
			updateAvailable = null;

			return false;							
		}
		
		/// <summary>
		/// Instructs the AutoUpdateDownloader to download the update specified by the update descriptor
		/// </summary>
		/// <param name="progressViewer">The progress viewer by which progress should be displayed</param>
		/// <param name="downloadDescriptor">The download descriptor that describes the download that should occur</param>
		/// <returns></returns>
		public virtual bool Download(
			IProgressViewer progressViewer,
			AutoUpdateDownloadDescriptor downloadDescriptor)
		{
			FileStream localStream = null;
			Stream remoteStream = null;
			string myTraceCategory = string.Format("'{0}'", this.GetType().Name);

			try
			{
				// set the progress to zero for the start
				this.SetDownloadProgress(progressViewer, 0, downloadDescriptor.Manifest.SizeOfUpdate);

				// format the downloaded path where the .update file will be when the download is finished				
				downloadDescriptor.DownloadedPath = Path.Combine(downloadDescriptor.Options.DownloadPath, Path.GetFileName(downloadDescriptor.Manifest.UrlOfUpdate));

				Debug.WriteLine(string.Format("Preparing to download update.\n\tThe update will be downloaded from '{0}'.\n\tThe update will be downloaded to '{1}'.", downloadDescriptor.Manifest.UrlOfUpdate, downloadDescriptor.DownloadedPath), myTraceCategory);
				
				// if the url where this update is supposed to be located is not set, just quit as there isn't anything else we can do
				if (downloadDescriptor.Manifest.UrlOfUpdate == null || downloadDescriptor.Manifest.UrlOfUpdate == string.Empty)
					return false;

				// create a new web client to download the file
				WebClient wc = new WebClient();

				// open a remote stream to the download
				Debug.WriteLine(string.Format("Preparing to download update.\n\tOpening stream to the remote url '{0}'.", downloadDescriptor.Manifest.UrlOfUpdate), myTraceCategory);
				remoteStream = wc.OpenRead(downloadDescriptor.Manifest.UrlOfUpdate);

				// open a local file stream where the update will be downloaded
				Debug.WriteLine(string.Format("Preparing to download update.\n\tOpening stream to the local url '{0}'.", downloadDescriptor.DownloadedPath), myTraceCategory);
				localStream = new FileStream(downloadDescriptor.DownloadedPath, FileMode.Create, FileAccess.Write, FileShare.None);

				// if successfull we'll receive the data in segments
				if (remoteStream != null)
				{
					long bytesDownloaded = 0;
					while (true)
					{
						// figure out how many bytes we have to download
						long bytesToReceive = downloadDescriptor.Manifest.SizeOfUpdate - bytesDownloaded;

						// correct it if it's more than the segment size
						if (bytesToReceive > _segmentSize)
							bytesToReceive = (long)_segmentSize;

						byte[] segment = new byte[bytesToReceive];

						// read a segment off the socket
						int bytesReceived = remoteStream.Read(segment, 0, (int)bytesToReceive);

						// bail if nothing read
						if (bytesReceived == 0)
							break;

						// if we received anything
						if (bytesReceived > 0)
						{
							// write it to the update file
							localStream.Write(segment, 0, bytesReceived);
							// update the position
							bytesDownloaded += bytesReceived;
						}

						// update the progress viewer
						this.SetDownloadProgress(progressViewer, bytesDownloaded, downloadDescriptor.Manifest.SizeOfUpdate);
					}
				}

				Debug.WriteLine(string.Format("The update was successfully downloaded to '{0}'.", downloadDescriptor.DownloadedPath), myTraceCategory);

				return true;
			}
			catch(ThreadAbortException)
			{

			}
			catch(Exception ex)
			{							
				try 
				{ 
					if (localStream != null) 
						localStream.Close(); 

					if (remoteStream != null) 
						remoteStream.Close();
				} 
				catch(Exception)
				{
				
				}

				try
				{
					// something broke, make sure we delete the .update file
					File.Delete(downloadDescriptor.DownloadedPath);
				}
				catch(Exception)
				{

				}

				throw ex;
			}
			finally
			{
				/* 
				 * make sure the streams are closed
				 * */

				try 
				{ 
					if (localStream != null) localStream.Close(); 
					if (remoteStream != null) remoteStream.Close();
				} 
				catch {}

				try
				{
					
				}
				catch {}
			}

			// if it's made it this far something went wrong
			return false;

		}
		
		/// <summary>
		/// Instructs the AutoUpdateDownloader to cleanup after an install
		/// </summary>
		/// <param name="progressViewer">The progress viewer by which progress should be displayed</param>
		/// <param name="downloadDescriptor">The download descriptor that describes the download that occurred and was installed</param>
		public virtual bool FinalizeInstallation(			
			IProgressViewer progressViewer,
			AutoUpdateDownloadDescriptor downloadDescriptor)
		{
			return true;			
		}

		/// <summary>
		/// Formats the bytes into a more readable string format using KB instead of bytes
		/// </summary>
		/// <param name="dwFileSize"></param>
		/// <returns></returns>
		public virtual string FormatFileLengthForDisplay(long dwFileSize)
		{
			decimal fileSize = 0;
			if (dwFileSize > 0)
			{
				fileSize = Convert.ToDecimal(dwFileSize / 1000.0);
				fileSize = Math.Round(fileSize);
				fileSize = Math.Max(1, fileSize);
			}

			int fileSizeInt = Convert.ToInt32(fileSize);
			return fileSizeInt.ToString("###,###,##0") + " KB";	
		}

		/// <summary>
		/// Updates the display of the specified progress dialog
		/// </summary>
		/// <param name="progressViewer"></param>
		/// <param name="bytesReceived"></param>
		/// <param name="bytesTotal"></param>
		public virtual void SetDownloadProgress(IProgressViewer progressViewer, long bytesReceived, long bytesTotal)
		{								
			double p = ((double)bytesReceived / (double)bytesTotal) * 100;
			int percent = (int)p;
			
			string title = string.Format("AutoUpdate Progress..." + "({0}% Completed)", percent.ToString());
			string received = this.FormatFileLengthForDisplay(bytesReceived);
			string total = this.FormatFileLengthForDisplay(bytesTotal);
			string description = string.Format("Progress: ({0} of {1} downloaded)", received, total);

			ProgressViewer.SetTitle(progressViewer, title);
			ProgressViewer.SetExtendedDescription(progressViewer, description);							
		}

		#endregion
	}
}
