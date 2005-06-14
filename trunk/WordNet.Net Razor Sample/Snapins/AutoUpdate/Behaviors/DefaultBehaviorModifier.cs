using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Razor;
using Razor.SnapIns;
using Razor.Networking.AutoUpdate;
using Razor.Networking.AutoUpdate.Behaviors;
using Razor.Networking.AutoUpdate.Common;
using Razor.SnapIns.AutoUpdateOptions;

namespace Razor.SnapIns.AutoUpdate.Behaviors
{
	/// <summary>
	/// Modifies the behavior of an AutoUpdateManager by displaying prompts and progress dialogs for the steps of the AutoUpdate process
	/// </summary>
	public class DefaultBehaviorModifier : BehaviorModifier
	{
		protected AutoUpdateManager _autoUpdateManager;
		protected CancelOneProgressWindowThread _progressWindowThread;
		protected bool _userInitiated;

		#region Interop

		[DllImport(@"User32")]
		private static extern int IsWindowVisible(IntPtr hWnd);
		private const int TRUE = 1;
		
		#endregion

		/// <summary>
		/// Initializes a new instance of the DefaultBehaviorModifier class
		/// </summary>
		public DefaultBehaviorModifier()
		{
			
		}

		#region My Overrides

		/// <summary>
		/// Binds the BehaviorManager to the AutoUpdateManager
		/// </summary>
		/// <param name="autoUpdateManager"></param>
		/// <returns></returns>
		public override bool BindTo(AutoUpdateManager autoUpdateManager)
		{
			if (autoUpdateManager == null)
				throw new ArgumentNullException("autoUpdateManager");			

			try
			{
				_autoUpdateManager = autoUpdateManager;
				_autoUpdateManager.AutoUpdateProcessStarted += new AutoUpdateManagerEventHandler(OnProcessStarted);
				_autoUpdateManager.AutoUpdateProcessEnded += new AutoUpdateManagerEventHandler(OnProcessEnded);

				return true;
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
			return false;
		}

		/// <summary>
		/// Releases the bindings from the AutoUpdateManager
		/// </summary>
		/// <param name="autoUpdateManager"></param>
		/// <returns></returns>
		public override bool Release(AutoUpdateManager autoUpdateManager)
		{
			if (autoUpdateManager == null)
				throw new ArgumentNullException("autoUpdateManager");

			try
			{
				_autoUpdateManager.AutoUpdateProcessStarted -= new AutoUpdateManagerEventHandler(OnProcessStarted);
				_autoUpdateManager.AutoUpdateProcessEnded -= new AutoUpdateManagerEventHandler(OnProcessEnded);
				_autoUpdateManager = null;

				return true;	
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
			return false;
		}

		#endregion

		#region My Public Properties
		
		/// <summary>
		/// Gets or sets a flag that indicates whether the auto-update process was user initiated or not
		/// </summary>
		public bool UserIntiated
		{
			get
			{
				return _userInitiated;
			}
			set
			{
				_userInitiated = value;
			}
		}
		
		#endregion

		/// <summary>
		/// Occurs when the auto-update process starts
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnProcessStarted(object sender, AutoUpdateManagerEventArgs e)
		{			
			AutoUpdateManager manager = (AutoUpdateManager)sender;			
			manager.NoLaterVersionAvailable += new AutoUpdateManagerEventHandler(OnNoLaterVersionAvailable);			
			manager.BeforeDownload += new AutoUpdateManagerWithDownloadDescriptorCancelEventHandler(OnBeforeDownload);
			manager.BeforeInstall += new AutoUpdateManagerWithDownloadDescriptorCancelEventHandler(OnBeforeInstall);         
			manager.BeforeUpdateAlternatePath += new AutoUpdateManagerWithDownloadDescriptorCancelEventHandler(OnBeforeUpdateAlternatePath);
			manager.BeforeSwitchToLatestVersion += new AutoUpdateManagerWithDownloadDescriptorCancelEventHandler(OnBeforeSwitchToLatestVersion);
			manager.AfterDownload += new AutoUpdateManagerWithDownloadDescriptorEventHandler(OnAfterDownload);
			manager.AfterInstall += new AutoUpdateManagerWithDownloadDescriptorEventHandler(OnAfterInstall);
			manager.Exception += new AutoUpdateExceptionEventHandler(OnException);
		}

		/// <summary>
		/// Occurs when the autoupdate process has finished
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnProcessEnded(object sender, AutoUpdateManagerEventArgs e)
		{        
			// close and dispose of any progress window's being displayed at this point
			if (_progressWindowThread != null)
			{
				_progressWindowThread.Window.Cancelled -= new EventHandler(OnProgressWindowCancelled);
				_progressWindowThread.Dispose();
				_progressWindowThread = null;
			}

			AutoUpdateManager manager = (AutoUpdateManager)sender;			
			manager.NoLaterVersionAvailable -= new AutoUpdateManagerEventHandler(OnNoLaterVersionAvailable);
			manager.BeforeDownload -= new AutoUpdateManagerWithDownloadDescriptorCancelEventHandler(OnBeforeDownload);
			manager.BeforeInstall -= new AutoUpdateManagerWithDownloadDescriptorCancelEventHandler(OnBeforeInstall);
			manager.BeforeUpdateAlternatePath -= new AutoUpdateManagerWithDownloadDescriptorCancelEventHandler(OnBeforeUpdateAlternatePath);
			manager.BeforeSwitchToLatestVersion -= new AutoUpdateManagerWithDownloadDescriptorCancelEventHandler(OnBeforeSwitchToLatestVersion);        
			manager.AfterDownload -= new AutoUpdateManagerWithDownloadDescriptorEventHandler(OnAfterDownload);
			manager.AfterInstall -= new AutoUpdateManagerWithDownloadDescriptorEventHandler(OnAfterInstall);
			manager.Exception -= new AutoUpdateExceptionEventHandler(OnException);

			_userInitiated = false;
		}

		/// <summary>
		/// Occurs when it is determined that there are no later versions available
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnNoLaterVersionAvailable(object sender, AutoUpdateManagerEventArgs e)
		{
			if (!_userInitiated)
				return;

			// further abstraction could be performed for localization
			string caption = "Auto Update";
			string message = "There are no available updates at this time.";

			// determine if the application has a main window			
			IWin32Window owner = SnapInHostingEngine.Instance.ApplicationContext.MainForm as IWin32Window;
			if (owner != null)
				if (IsWindowVisible(owner.Handle) != TRUE)
					owner = null;
						
			if (owner != null)
				MessageBox.Show(owner, message, caption, MessageBoxButtons.OK,	MessageBoxIcon.Information);
			else
				MessageBox.Show(message, caption, MessageBoxButtons.OK,	MessageBoxIcon.Information);
		}

		/// <summary>
		/// Occurs before
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnBeforeDownload(object sender, AutoUpdateManagerWithDownloadDescriptorCancelEventArgs e)
		{     
			// if the event hasn't already been marked for cancellation
			// and the options do not say to automatically take the action
			// we must prompt the user for the proper actions
			if (!AutoUpdateOptionsSnapIn.Instance.AutoUpdateOptions.AutomaticallyDownloadUpdates && !e.Cancel)
			{
				// we are overriding the options that are in use by the autoupdate manager at this point
				e.OverrideOptions = true;

				// create the window
				BeforeUpdateDownloadedWindow window = new BeforeUpdateDownloadedWindow(e.DownloadDescriptor);            
				IWin32Window owner = SnapInHostingEngine.Instance.ApplicationContext.MainForm as IWin32Window;
				if (owner != null)
					if (IsWindowVisible(owner.Handle) != TRUE)
						owner = null;

				if (owner != null)
					window.ShowDialog(owner);
				else
					window.ShowDialog();
				e.Cancel = window.Cancel;
                        
				// if they haven't cancelled
				if (!e.Cancel)
				{
					// determine if they have chosen to automatically download future updates
					AutoUpdateOptionsSnapIn.Instance.AutoUpdateOptions.AutomaticallyDownloadUpdates = window.Auto;              
				}
			}

			if (!e.Cancel)
			{
				// show a cancellable progress window
				_progressWindowThread = new CancelOneProgressWindowThread();               
				_progressWindowThread.ShowAsynchronously();
				_progressWindowThread.Window.Cancelled += new EventHandler(OnProgressWindowCancelled);
               
				// update the information displayed with the progress dialog
				ProgressViewer.SetTitle(_progressWindowThread.Window, "Auto-Update: Status");
				ProgressViewer.SetHeading(_progressWindowThread.Window, string.Format("Downloading {0} Version {1}", e.DownloadDescriptor.Manifest.Product.Name, e.DownloadDescriptor.Manifest.Product.Version.ToString()));
				ProgressViewer.SetDescription(_progressWindowThread.Window, "This operation could take several seconds...");
               
				// return the progress viewer implemented by the progress window
				e.ProgressViewer = (IProgressViewer)_progressWindowThread.Window;       
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAfterDownload(object sender, AutoUpdateManagerWithDownloadDescriptorEventArgs e)
		{
			// close and dispose of any progress window's being displayed at this point
			if (_progressWindowThread != null)
			{
				_progressWindowThread.Window.Cancelled -= new EventHandler(OnProgressWindowCancelled);
				_progressWindowThread.Dispose();
				_progressWindowThread = null;
			}
		}

		/// <summary>
		/// Occurs before the update is installed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnBeforeInstall(object sender, AutoUpdateManagerWithDownloadDescriptorCancelEventArgs e)
		{
			// if the event hasn't already been marked for cancellation
			// and the options do not say to automatically take the action
			// we must prompt the user for the proper actions
			if (!AutoUpdateOptionsSnapIn.Instance.AutoUpdateOptions.AutomaticallyInstallUpdates && !e.Cancel)
			{
				// we are overriding the options that are in use by the autoupdate manager at this point
				e.OverrideOptions = true;

				// create the window
				BeforeUpdateInstalledWindow window = new BeforeUpdateInstalledWindow(e.DownloadDescriptor.Manifest.Product.Name, e.DownloadDescriptor.Manifest.Product.Version);           
				IWin32Window owner = SnapInHostingEngine.Instance.ApplicationContext.MainForm as IWin32Window;
				if (owner != null)
					if (IsWindowVisible(owner.Handle) != TRUE)
						owner = null;

				if (owner != null)
					window.ShowDialog(owner);
				else
					window.ShowDialog();
				e.Cancel = window.Cancel;

				// if they haven't cancelled
				if (!e.Cancel)
				{
					// determine if they have chosen to automatically download future updates
					AutoUpdateOptionsSnapIn.Instance.AutoUpdateOptions.AutomaticallyInstallUpdates = window.Auto;               
				}
			}

			if (!e.Cancel)
			{
				// show a cancellable progress window
				_progressWindowThread = new CancelOneProgressWindowThread();
				_progressWindowThread.ShowAsynchronously();
				_progressWindowThread.Window.Cancelled += new EventHandler(OnProgressWindowCancelled);

				// update the information displayed with the progress dialog
				ProgressViewer.SetTitle(_progressWindowThread.Window, "Auto-Update: Status");
				ProgressViewer.SetHeading(_progressWindowThread.Window, string.Format("Installing {0} Version {1}", e.DownloadDescriptor.Manifest.Product.Name, e.DownloadDescriptor.Manifest.Product.Version.ToString()));
				ProgressViewer.SetDescription(_progressWindowThread.Window, "This operation could take several seconds...");

				// return the progress viewer implemented by the progress window
				e.ProgressViewer = (IProgressViewer)_progressWindowThread.Window;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAfterInstall(object sender, AutoUpdateManagerWithDownloadDescriptorEventArgs e)
		{
			// close and dispose of any progress window's being displayed at this point
			if (_progressWindowThread != null)
			{
				_progressWindowThread.Window.Cancelled -= new EventHandler(OnProgressWindowCancelled);
				_progressWindowThread.Dispose();
				_progressWindowThread = null;
			}
		}

		/// <summary>
		/// Occurs before the alternate path is updated
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnBeforeUpdateAlternatePath(object sender, AutoUpdateManagerWithDownloadDescriptorCancelEventArgs e)
		{
			if (!AutoUpdateOptionsSnapIn.Instance.AutoUpdateOptions.AutomaticallyInstallUpdates && !e.Cancel)
			{           
				if (AutoUpdateOptionsSnapIn.Instance.AutoUpdateOptions.AlternatePath != null &&
					AutoUpdateOptionsSnapIn.Instance.AutoUpdateOptions.AlternatePath != string.Empty)
				{
					// we are overriding the options that are in use by the autoupdate manager at this point
					e.OverrideOptions = true;

					// create the window
					BeforeUpdateCopiedToAlternatePathWindow window = new BeforeUpdateCopiedToAlternatePathWindow(e.DownloadDescriptor.Manifest.Product.Name, e.DownloadDescriptor.Manifest.Product.Version);           
					IWin32Window owner = SnapInHostingEngine.Instance.ApplicationContext.MainForm as IWin32Window;
					if (owner != null)
						if (IsWindowVisible(owner.Handle) != TRUE)
							owner = null;

					if (owner != null)
						window.ShowDialog(owner);
					else
						window.ShowDialog();
					e.Cancel = window.Cancel;

					// if they haven't cancelled
					if (!e.Cancel)
						// determine if they have chosen to automatically download future updates
						AutoUpdateOptionsSnapIn.Instance.AutoUpdateOptions.AutomaticallyUpdateAlternatePath = window.Auto;
				}
			}
		}

		/// <summary>
		/// Occurs before the application switches to the latest version
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnBeforeSwitchToLatestVersion(object sender, AutoUpdateManagerWithDownloadDescriptorCancelEventArgs e)
		{
			// if the event hasn't already been marked for cancellation
			// and the options do not say to automatically take the action
			// we must prompt the user for the proper actions
			if (!AutoUpdateOptionsSnapIn.Instance.AutoUpdateOptions.AutomaticallySwitchToNewVersion && !e.Cancel)
			{
				// we are overriding the options that are in use by the autoupdate manager at this point
				e.OverrideOptions = true;

				// create the window
				BeforeUpdateSwitchedToWindow window = new BeforeUpdateSwitchedToWindow(e.DownloadDescriptor.Manifest.Product.Name, e.DownloadDescriptor.Manifest.Product.Version);            
				IWin32Window owner = SnapInHostingEngine.Instance.ApplicationContext.MainForm as IWin32Window;
				if (owner != null)
					if (IsWindowVisible(owner.Handle) != TRUE)
						owner = null;

				if (owner != null)
					window.ShowDialog(owner);
				else
					window.ShowDialog();
				e.Cancel = window.Cancel;

				// if they haven't cancelled
				if (!e.Cancel)
					// determine if they have chosen to automatically download future updates
					AutoUpdateOptionsSnapIn.Instance.AutoUpdateOptions.AutomaticallySwitchToNewVersion = window.Auto;
			}
		}

		/// <summary>
		/// Occurs when an unexpected error occurs within the auto-update manager
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnException(object sender, AutoUpdateExceptionEventArgs e)
		{
			// close and dispose of any progress window's being displayed at this point
			if (_progressWindowThread != null)
			{
				_progressWindowThread.Window.Cancelled -= new EventHandler(OnProgressWindowCancelled);
				_progressWindowThread.Dispose();
				_progressWindowThread = null;
			}

			string caption = "AutoUpdate Encountered Exception";
			string message = e.Exception.ToString();
			// determine if the application has a main window			
			IWin32Window owner = SnapInHostingEngine.Instance.ApplicationContext.MainForm as IWin32Window;
			if (owner != null)
				if (IsWindowVisible(owner.Handle) != TRUE)
					owner = null;

			if (owner != null)
				MessageBox.Show(owner, message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
			else
				MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		/// <summary>
		/// Occurs when the progress window is cancelled for one reason or another
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnProgressWindowCancelled(object sender, EventArgs e)
		{
			// end the process of checking for updates. hopefully this will not fuck up too much stuff
			_autoUpdateManager.EndCheckingForUpdates();

			// close and dispose of any progress window's being displayed at this point
			if (_progressWindowThread != null)
			{
				_progressWindowThread.Window.Cancelled -= new EventHandler(OnProgressWindowCancelled);
				_progressWindowThread.Dispose();
				_progressWindowThread = null;
			}
			
			/*
			 * this is why there is a reference to the auto update manager
			 * as the sender of this event contains to reference or object to retrieve the manager from
			 * */
			_autoUpdateManager.NoLaterVersionAvailable -= new AutoUpdateManagerEventHandler(OnNoLaterVersionAvailable);
			_autoUpdateManager.BeforeDownload -= new AutoUpdateManagerWithDownloadDescriptorCancelEventHandler(OnBeforeDownload);
			_autoUpdateManager.BeforeInstall -= new AutoUpdateManagerWithDownloadDescriptorCancelEventHandler(OnBeforeInstall);
			_autoUpdateManager.BeforeUpdateAlternatePath -= new AutoUpdateManagerWithDownloadDescriptorCancelEventHandler(OnBeforeUpdateAlternatePath);
			_autoUpdateManager.BeforeSwitchToLatestVersion -= new AutoUpdateManagerWithDownloadDescriptorCancelEventHandler(OnBeforeSwitchToLatestVersion);        
			_autoUpdateManager.AfterDownload -= new AutoUpdateManagerWithDownloadDescriptorEventHandler(OnAfterDownload);
			_autoUpdateManager.AfterInstall -= new AutoUpdateManagerWithDownloadDescriptorEventHandler(OnAfterInstall);			
			_autoUpdateManager.Exception -= new AutoUpdateExceptionEventHandler(OnException);
		}
	}
}
