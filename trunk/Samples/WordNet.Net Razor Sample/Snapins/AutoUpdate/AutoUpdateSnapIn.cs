using System;
using System.Diagnostics;
using Razor.Configuration;
using Razor.Attributes;
using Razor.SnapIns;
using Razor.Networking.AutoUpdate.Common;
using Razor.Networking.AutoUpdate;
using Razor.SnapIns.AutoUpdateOptions;

namespace Razor.SnapIns.AutoUpdate
{
    /// <summary>
    /// Summary description for AutoUpdateSnapIn.
    /// </summary>
    [SnapInTitle("Auto-Update")]	
    [SnapInDescription("Provides functionality for implementing automatic updates")]
    [SnapInCompany("CodeReflection")]
    [SnapInDevelopers("Mark (Code6) Belles")]
    [SnapInVersion("1.0.1")]
    [SnapInDependency(typeof(AutoUpdateOptionsSnapIn))]
    public class AutoUpdateSnapIn : SnapIn
    {
        protected static AutoUpdateSnapIn _theInstance;
        protected AutoUpdateManager _manager;

        /// <summary>
        /// Returns the one and only 
        /// </summary>
        public static AutoUpdateSnapIn Instance
        {
            get
            {
                return _theInstance;
            }
        }

        /// <summary>
        /// Initializes a new instance of the AutoUpdateSnapIn class
        /// </summary>
        public AutoUpdateSnapIn() : base()
        {
            _theInstance = this;
			
            base.Start += new EventHandler(AutoUpdateSnapIn_Start);
            base.Stop += new EventHandler(AutoUpdateSnapIn_Stop);
			
            SnapInHostingEngine.Instance.AfterSnapInsStarted += new EventHandler(OnSnapInHostingEngine_AfterSnapInsStarted);
        }

        #region My SnapIn Events

        /// <summary>
        /// Occurs when the snapin starts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoUpdateSnapIn_Start(object sender, EventArgs e)
        {
            this.StartMyServices();
        }

        /// <summary>
        /// Occurs when the snapin stops
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoUpdateSnapIn_Stop(object sender, EventArgs e)
        {
            this.StopMyServices();
        }

        /// <summary>
        /// Occurs after all of the snapins have started
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSnapInHostingEngine_AfterSnapInsStarted(object sender, EventArgs e)
        {
            try
            {				
                // if the snapin hosting engine is started
                if (SnapInHostingEngine.Instance.Started)
                {
                    // and the options state that we should automatically check for updates
                    if (AutoUpdateOptionsSnapIn.Instance.AutoUpdateOptions.AutomaticallyCheckForUpdates)
                    {
                        // then begin checking for updates
                        _manager.BeginCheckingForUpdates();
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
		
        #endregion

        #region My Overrides

        /// <summary>
        /// Starts my services
        /// </summary>
        protected override void StartMyServices()
        {
            try
            {
                // receive notification when the autoupdate options change
                AutoUpdateOptionsSnapIn.Instance.AutoUpdateOptionChanged += new XmlConfigurationElementEventHandler(OnAutoUpdateOptionsChanged);

                // create a new autoupdate manager using the options supplied to us by the options snapin
                _manager = new AutoUpdateManager(AutoUpdateOptionsSnapIn.Instance.AutoUpdateOptions);
								
                /* 
                 * wire up to the events of the autoupdate manager to control it's behavior 
                 * the events may be cancelled here by looking at the autoupdate options
                 * */
                _manager.SwitchToLatestVersion += new AutoUpdateManagerWithDownloadDescriptorEventHandler(OnAutoUpdateManagerSwitchToLatestVersion);
				
                // if the snapin hosting engine is started
                if (SnapInHostingEngine.Instance.Started)
                {
                    // and the options state that we should automatically check for updates
                    if (AutoUpdateOptionsSnapIn.Instance.AutoUpdateOptions.AutomaticallyCheckForUpdates)
                    {
                        // then begin checking for updates
                        _manager.BeginCheckingForUpdates();
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Stops my services
        /// </summary>
        protected override void StopMyServices()
        {
            try
            {
                // receive notification when the autoupdate options change
                AutoUpdateOptionsSnapIn.Instance.AutoUpdateOptionChanged -= new XmlConfigurationElementEventHandler(OnAutoUpdateOptionsChanged);

                /* 
                 * unwire up to the events of the autoupdate manager to control it's behavior 
                 * */
                _manager.SwitchToLatestVersion -= new AutoUpdateManagerWithDownloadDescriptorEventHandler(OnAutoUpdateManagerSwitchToLatestVersion);

                // cancel any existing checks
                _manager.EndCheckingForUpdates();
                _manager = null;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        #endregion

        #region My Public Properties

        /// <summary>
        /// Return the AutoUpdateManager that is responsible for checking for updates for this hosting engine (Aka. This product)
        /// </summary>
        public AutoUpdateManager AutoUpdateManager
        {
            get
            {
                return _manager;
            }
        }

        #endregion

        /// <summary>
        /// Uses the ApplicationWindow to restart the program using the bootstrap
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAutoUpdateManagerSwitchToLatestVersion(object sender, AutoUpdateManagerWithDownloadDescriptorEventArgs e)
        {
            // restart the hosting engine
            SnapInHostingEngine.Instance.RestartUsingBootstrap();
        }

        /// <summary>
        /// Occurs when an auto update option changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAutoUpdateOptionsChanged(object sender, XmlConfigurationElementEventArgs e)
        {
            // bail if it's being edited
            if (e.Element.IsBeingEdited)
                return;

            if (_manager != null)
                _manager.Options = AutoUpdateOptionsSnapIn.Instance.AutoUpdateOptions;
        }
    }
}

