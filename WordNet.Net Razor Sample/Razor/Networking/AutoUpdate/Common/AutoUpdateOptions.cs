using System;

namespace Razor.Networking.AutoUpdate.Common
{
	/// <summary>
	/// Summary description for AutoUpdateOptionNames.
	/// </summary>
	public enum AutoUpdateOptionNames
	{
		AutoCheck,
		AutoDownload,
		AutoInstall,
		AutoSwitch,
		AutoUpdateAlternatePath,
		AlternatePath,
		WebServiceUrl
	}

	/// <summary>
	/// Summary description for AutoUpdateOptions.
	/// </summary>
	public class AutoUpdateOptions 
	{
		protected bool _automaticallyCheckForUpdates;
		protected bool _automaticallyDownloadUpdates;
		protected bool _automaticallyInstallUpdates;
		protected bool _automaticallySwitchToNewVersion;
		protected bool _automaticallyUpdateAlternatePath;
		protected string _downloadPath;
		protected string _alternatePath;
		protected string _webServiceUrl;

		/// <summary>
		/// Initializes a new instance of the AutoUpdateOptions class
		/// </summary>
		public AutoUpdateOptions() 
		{
			
		}

		/// <summary>
		/// Gets or sets a flag to indicate whether the engine should automatically check for updates
		/// </summary>
		public bool AutomaticallyCheckForUpdates
		{
			get
			{
				return _automaticallyCheckForUpdates;
			}
			set
			{
				_automaticallyCheckForUpdates = value;
			}
		}

		/// <summary>
		/// Gets or sets a flag to indicate whether the engine should automatically download updates
		/// </summary>
		public bool AutomaticallyDownloadUpdates
		{
			get
			{
				return _automaticallyDownloadUpdates;
			}
			set
			{
				_automaticallyDownloadUpdates = value;
			}
		}

		/// <summary>
		/// Gets or sets a flag to indicate whether the engine should automatically install updates
		/// </summary>
		public bool AutomaticallyInstallUpdates
		{
			get
			{
				return _automaticallyInstallUpdates;
			}
			set
			{
				_automaticallyInstallUpdates = value;
			}
		}

		/// <summary>
		/// Gets or sets a flag to indicate whether the engine should automatically switch to the new version after installing
		/// </summary>
		public bool AutomaticallySwitchToNewVersion
		{
			get
			{
				return _automaticallySwitchToNewVersion;
			}
			set
			{
				_automaticallySwitchToNewVersion = value;
			}
		}
		
		/// <summary>
		/// Gets or sets a flag to indicate whether the engine should copy the .update to the alternate path after it installs the update
		/// </summary>
		public bool AutomaticallyUpdateAlternatePath
		{
			get
			{
				return _automaticallyUpdateAlternatePath;
			}
			set
			{
				_automaticallyUpdateAlternatePath = value;
			}
		}

		/// <summary>
		/// Gets or sets the Unc path where the updates will be downloaded on the local system
		/// </summary>
		public string DownloadPath
		{
			get
			{
				return _downloadPath;
			}
			set
			{
				_downloadPath = value;
			}
		}

		/// <summary>
		/// Gets or sets a Unc path where updates may be downloaded or uploaded
		/// </summary>
		public string AlternatePath
		{
			get
			{
				return _alternatePath;
			}
			set
			{
				_alternatePath = value;
			}
		}

		/// <summary>
		/// Gets or sets the Url where the AutoUpdate web service can be located (Ex: http://www.depcollc.com/webservices/AutoUpdate/AutoUpdateWebService.asmx")
		/// </summary>
		public string WebServiceUrl
		{
			get
			{
				return _webServiceUrl;
			}
			set
			{
				_webServiceUrl = value;
			}
		}
	}

	/// <summary>
	/// Defines the default AutoUpdate options
	/// </summary>
	public class DefaultAutoUpdateOptions : AutoUpdateOptions 
	{
		/// <summary>
		/// Initializes a new instance of the DefaultAutoUpdateOptions class
		/// </summary>
		public DefaultAutoUpdateOptions() : base()
		{
			base.AutomaticallyCheckForUpdates = true;
			base.AutomaticallyDownloadUpdates = false;
			base.AutomaticallyInstallUpdates = true;
			base.AutomaticallySwitchToNewVersion = false;
			base.AutomaticallyUpdateAlternatePath = true;
			base.AlternatePath = string.Empty;
			base.WebServiceUrl = AutoUpdateWebServiceProxy.DefaultWebServiceUrl;
		}
	}
}