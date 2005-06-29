using System;

namespace Razor.Networking.AutoUpdate.Common
{
	/// <summary>
	/// Summary description for AutoUpdateDownloadDescriptorEventArgs.
	/// </summary>
	public class AutoUpdateDownloadDescriptorEventArgs : System.EventArgs 
	{
		protected AutoUpdateDownloadDescriptor _descriptor;

		/// <summary>
		/// Initializes a new instance of the AutoUpdateDownloadDescriptorEventArgs class
		/// </summary>
		/// <param name="updateDescriptor"></param>
		public AutoUpdateDownloadDescriptorEventArgs(AutoUpdateDownloadDescriptor downloadDescriptor) : base()
		{
			_descriptor = downloadDescriptor;
		}

		/// <summary>
		/// Returns the descriptor for the update
		/// </summary>
		public AutoUpdateDownloadDescriptor DownloadDescriptor
		{
			get
			{
				return _descriptor;
			}
		}
	}

	public delegate void AutoUpdateDownloadDescriptorEventHandler(object sender, AutoUpdateDownloadDescriptorEventArgs e);
}
