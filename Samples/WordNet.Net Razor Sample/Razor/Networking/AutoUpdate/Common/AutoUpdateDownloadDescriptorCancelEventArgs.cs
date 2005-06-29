using System;

namespace Razor.Networking.AutoUpdate.Common
{
	/// <summary>
	/// Summary description for AutoUpdateDownloadDescriptorCancelEventArgs.
	/// </summary>
	public class AutoUpdateDownloadDescriptorCancelEventArgs : AutoUpdateDownloadDescriptorEventArgs
	{
		protected bool _cancel;

		/// <summary>
		/// Initializes a new instance of the 
		/// </summary>
		/// <param name="cancel"></param>
		/// <param name="updateDescriptor"></param>
		public AutoUpdateDownloadDescriptorCancelEventArgs(bool cancel, AutoUpdateDownloadDescriptor downloadDescriptor) : base(downloadDescriptor)
		{
			_cancel = cancel;
		}

		/// <summary>
		/// Gets or sets a flag that indicates whether the event should be cancelled or not
		/// </summary>
		public bool Cancel
		{
			get
			{
				return _cancel;
			}
			set
			{
				_cancel = value;
			}
		}
	}

	public delegate void AutoUpdateDownloadDescriptorCancelEventHandler(object sender, AutoUpdateDownloadDescriptorCancelEventArgs e);
}
