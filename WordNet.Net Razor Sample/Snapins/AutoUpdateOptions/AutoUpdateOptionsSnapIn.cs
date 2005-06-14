using System;
using System.Diagnostics;
using Razor.Configuration;
using Razor.Attributes;
using Razor.SnapIns;
using Razor.Networking.AutoUpdate.Common;

namespace Razor.SnapIns.AutoUpdateOptions
{
    /// <summary>
    /// Summary description for AutoUpdateOptionsSnapIn.
    /// </summary>
    [SnapInTitle("Auto-Update Options")]	
    [SnapInDescription("Installs the default autoupdate options, and provides methods for reading and writing autoupdate options.")]
    [SnapInCompany("CodeReflection")]
    [SnapInDevelopers("Mark (Code6) Belles")]
    [SnapInVersion("1.0.0")]
    public class AutoUpdateOptionsSnapIn : SnapIn 
    {
        protected static AutoUpdateOptionsSnapIn _theInstance;
        protected Razor.Networking.AutoUpdate.Common.AutoUpdateOptions _options;

        /// <summary>
        /// Occurs when an AutoUpdate option changes
        /// </summary>
        public event XmlConfigurationElementEventHandler AutoUpdateOptionChanged;

        /// <summary>
        /// Returns the currently executing instance of the NetworkOptionsSnapIn
        /// </summary>
        public static AutoUpdateOptionsSnapIn Instance
        {
            get
            {
                return _theInstance;
            }
        }

        /// <summary>
        /// Initializes a new instance of the AutoUpdateOptionsSnapIn class
        /// </summary>
        public AutoUpdateOptionsSnapIn() : base()
        {
            _theInstance = this;
            base.InstallCommonOptions += new EventHandler(AutoUpdateOptionsSnapIn_InstallCommonOptions);
            base.ReadCommonOptions += new EventHandler(AutoUpdateOptionsSnapIn_ReadCommonOptions);
            base.Start += new EventHandler(AutoUpdateOptionsSnapIn_Start);
            base.Stop += new EventHandler(AutoUpdateOptionsSnapIn_Stop);
        }

        #region SnapIn Events
		
        /// <summary>
        /// Occurs when the SnapIn should install it's common options
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoUpdateOptionsSnapIn_InstallCommonOptions(object sender, EventArgs e)
        {
            this.WriteOptions(new DefaultAutoUpdateOptions());
        }

        /// <summary>
        /// Occurs when the SnapIn should read its common options
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoUpdateOptionsSnapIn_ReadCommonOptions(object sender, EventArgs e)
        {
            this.ReadMyCommonOptions();
        }
		
        /// <summary>
        /// Occurs when the SnapIn starts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoUpdateOptionsSnapIn_Start(object sender, EventArgs e)
        {
            SnapInHostingEngine.Instance.CommonConfiguration.Changed += new XmlConfigurationElementEventHandler(CommonConfiguration_Changed);
        }

        /// <summary>
        /// Occurs when the SnapIn stops
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoUpdateOptionsSnapIn_Stop(object sender, EventArgs e)
        {
            SnapInHostingEngine.Instance.CommonConfiguration.Changed -= new XmlConfigurationElementEventHandler(CommonConfiguration_Changed);
        }
		
        #endregion

        #region Overrides

        /// <summary>
        /// Reads the autoupdate options
        /// </summary>
        protected override void ReadMyCommonOptions()
        {
            _options = this.ReadOptions();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reads the auto update options
        /// </summary>
        /// <returns></returns>
        public Razor.Networking.AutoUpdate.Common.AutoUpdateOptions ReadOptions()
        {
            try
            {
                // create new default options
                DefaultAutoUpdateOptions defaultOptions = new DefaultAutoUpdateOptions();

                // create new options
                Razor.Networking.AutoUpdate.Common.AutoUpdateOptions options = new Razor.Networking.AutoUpdate.Common.AutoUpdateOptions();

                // get the common config
                XmlConfiguration configuration = SnapInHostingEngine.Instance.CommonConfiguration;
                Debug.Assert(configuration != null);

                // get the autoupdate category
                XmlConfigurationCategory category = configuration.Categories[@"AutoUpdate", true];
                Debug.Assert(category != null);

                XmlConfigurationOption option = null;

                // check
                if ((option = category.Options[AutoUpdateOptionNames.AutoCheck.ToString()]) == null)
                {
                    option = category.Options[AutoUpdateOptionNames.AutoCheck.ToString(), true, defaultOptions.AutomaticallyCheckForUpdates];
                    option.DisplayName = @"Automatically Check for Updates";
                    option.Category = @"General";
                    option.Description = @"Determines whether the AutoUpdateManager automatically checks for updates on startup.";											
                }
                try { options.AutomaticallyCheckForUpdates = (bool)option.Value; } 
                catch(Exception ex) { Debug.WriteLine(ex); }
                option = null;

                // download
                if ((option = category.Options[AutoUpdateOptionNames.AutoDownload.ToString()]) == null)
                {
                    option = category.Options[AutoUpdateOptionNames.AutoDownload.ToString(), true, defaultOptions.AutomaticallyDownloadUpdates];
                    option.DisplayName = @"Automatically Download Updates";
                    option.Category = @"General";
                    option.Description = @"Determines whether the AutoUpdateManager automatically downloads available updates without prompting.";											
                }
                try { options.AutomaticallyDownloadUpdates = (bool)option.Value; } 
                catch(Exception ex) { Debug.WriteLine(ex); }
                option = null;

                // install
                if ((option = category.Options[AutoUpdateOptionNames.AutoInstall.ToString()]) == null)
                {
                    option = category.Options[AutoUpdateOptionNames.AutoInstall.ToString(), true, defaultOptions.AutomaticallyInstallUpdates];
                    option.DisplayName = @"Automatically Install Updates";
                    option.Category = @"General";
                    option.Description = @"Determines whether the AutoUpdateManager automatically installs the updates after downloading.";											
                }
                try { options.AutomaticallyInstallUpdates = (bool)option.Value; } 
                catch(Exception ex) { Debug.WriteLine(ex); }
                option = null;
				
                // switch
                if ((option = category.Options[AutoUpdateOptionNames.AutoSwitch.ToString()]) == null)
                {
                    option = category.Options[AutoUpdateOptionNames.AutoSwitch.ToString(), true, defaultOptions.AutomaticallySwitchToNewVersion];
                    option.DisplayName = @"Automatically Switch to Newest Version";
                    option.Category = @"General";
                    option.Description = @"Determines whether the AutoUpdateManager automatically switches to the newest version after installation.";											
                }
                try { options.AutomaticallySwitchToNewVersion = (bool)option.Value; } 
                catch(Exception ex) { Debug.WriteLine(ex); }
                option = null;

                // update alt path
                if ((option = category.Options[AutoUpdateOptionNames.AutoUpdateAlternatePath.ToString()]) == null)
                {
                    option = category.Options[AutoUpdateOptionNames.AutoUpdateAlternatePath.ToString(), true, defaultOptions.AutomaticallyUpdateAlternatePath];
                    option.DisplayName = @"Automatically Update Alternate Path";
                    option.Category = @"General";
                    option.Description = @"Determines whether the AutoUpdateManager automatically creates backup copies of the .Manifest and .Update files after installation.";											
                }
                try { options.AutomaticallyUpdateAlternatePath = (bool)option.Value; } 
                catch(Exception ex) { Debug.WriteLine(ex); }
                option = null;

                // alt path
                if ((option = category.Options[AutoUpdateOptionNames.AlternatePath.ToString()]) == null)
                {
                    option = category.Options[AutoUpdateOptionNames.AlternatePath.ToString(), true, defaultOptions.AlternatePath];
                    option.DisplayName = @"Alternate Download Path";
                    option.Category = @"General";
                    option.Description = @"This alternate path (url or unc path) will be checked first before attempting to use the web service url to locate updates.";																
                }
                option.EditorAssemblyQualifiedName = typeof(Razor.Configuration.PathOptionEditor).AssemblyQualifiedName;
                try { options.AlternatePath = (string)option.Value; } 
                catch(Exception ex) { Debug.WriteLine(ex); }
                option = null;

                // url
                if ((option = category.Options[AutoUpdateOptionNames.WebServiceUrl.ToString()]) == null)
                {
                    option = category.Options[AutoUpdateOptionNames.WebServiceUrl.ToString(), true, defaultOptions.WebServiceUrl];
                    option.DisplayName = @"Web Service Url";
                    option.Category = @"General";
                    option.Description = @"The url specifying where the AutoUpdate Web Service can be located.";											
                }
                try { options.WebServiceUrl = (string)option.Value; } 
                catch(Exception ex) { Debug.WriteLine(ex); }
                option = null;

                return options;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return null;
        }

        /// <summary>
        /// Writes the autoupdate options 
        /// </summary>
        /// <param name="optionName"></param>
        /// <param name="options"></param>
        public void WriteOptions(AutoUpdateOptionNames optionName, Razor.Networking.AutoUpdate.Common.AutoUpdateOptions options)
        {
            try
            {
                // get the common config
                XmlConfiguration configuration = SnapInHostingEngine.Instance.CommonConfiguration;
                Debug.Assert(configuration != null);

                // get the autoupdate category
                XmlConfigurationCategory category = configuration.Categories[@"AutoUpdate", true];
                Debug.Assert(category != null);

                XmlConfigurationOption option = null;

                switch(optionName)
                {
                    case AutoUpdateOptionNames.AutoCheck:
                        // check
                        if ((option = category.Options[AutoUpdateOptionNames.AutoCheck.ToString()]) == null)
                        {
                            option = category.Options[AutoUpdateOptionNames.AutoCheck.ToString(), true, options.AutomaticallyCheckForUpdates];
                            option.DisplayName = @"Automatically Check for Updates";
                            option.Category = @"General";
                            option.Description = @"Determines whether the AutoUpdateManager automatically checks for updates on startup.";	
                        }
                        option.Value = options.AutomaticallyCheckForUpdates;
                        break;

                    case AutoUpdateOptionNames.AutoDownload:
                        // download
                        if ((option = category.Options[AutoUpdateOptionNames.AutoDownload.ToString()]) == null)
                        {
                            option = category.Options[AutoUpdateOptionNames.AutoDownload.ToString(), true, options.AutomaticallyDownloadUpdates];
                            option.DisplayName = @"Automatically Download Updates";
                            option.Category = @"General";
                            option.Description = @"Determines whether the AutoUpdateManager automatically downloads available updates without prompting.";											
                        }
                        option.Value = options.AutomaticallyDownloadUpdates;
                        break;

                    case AutoUpdateOptionNames.AutoInstall:
                        // install
                        if ((option = category.Options[AutoUpdateOptionNames.AutoInstall.ToString()]) == null)
                        {
                            option = category.Options[AutoUpdateOptionNames.AutoInstall.ToString(), true, options.AutomaticallyInstallUpdates];
                            option.DisplayName = @"Automatically Install Updates";
                            option.Category = @"General";
                            option.Description = @"Determines whether the AutoUpdateManager automatically installs the updates after downloading.";											
                        }
                        option.Value = options.AutomaticallyInstallUpdates;
                        break;

                    case AutoUpdateOptionNames.AutoSwitch:
                        // switch
                        if ((option = category.Options[AutoUpdateOptionNames.AutoSwitch.ToString()]) == null)
                        {
                            option = category.Options[AutoUpdateOptionNames.AutoSwitch.ToString(), true, options.AutomaticallySwitchToNewVersion];
                            option.DisplayName = @"Automatically Switch to Newest Version";
                            option.Category = @"General";
                            option.Description = @"Determines whether the AutoUpdateManager automatically switches to the newest version after installation.";											
                        }
                        option.Value = options.AutomaticallySwitchToNewVersion;
                        break;

                    case AutoUpdateOptionNames.AutoUpdateAlternatePath:
                        // update alt path
                        if ((option = category.Options[AutoUpdateOptionNames.AutoUpdateAlternatePath.ToString()]) == null)
                        {
                            option = category.Options[AutoUpdateOptionNames.AutoUpdateAlternatePath.ToString(), true, options.AutomaticallyUpdateAlternatePath];
                            option.DisplayName = @"Automatically Update Alternate Path";
                            option.Category = @"General";
                            option.Description = @"Determines whether the AutoUpdateManager automatically creates backup copies of the .Manifest and .Update files after installation.";											
                        }
                        option.Value = options.AutomaticallyUpdateAlternatePath;
                        break;

                    case AutoUpdateOptionNames.AlternatePath:
                        // alt path
                        if ((option = category.Options[AutoUpdateOptionNames.AlternatePath.ToString()]) == null)
                        {
                            option = category.Options[AutoUpdateOptionNames.AlternatePath.ToString(), true, options.AlternatePath];
                            option.DisplayName = @"Alternate Download Path";
                            option.Category = @"General";
                            option.Description = @"This alternate path (url or unc path) will be checked first before attempting to use the web service url to locate updates.";																		
                        }
                        option.Value = options.AlternatePath;
                        option.EditorAssemblyQualifiedName = typeof(Razor.Configuration.PathOptionEditor).AssemblyQualifiedName;
                        break;

                    case AutoUpdateOptionNames.WebServiceUrl:
                        // url
                        if ((option = category.Options[AutoUpdateOptionNames.WebServiceUrl.ToString()]) == null)
                        {
                            option = category.Options[AutoUpdateOptionNames.WebServiceUrl.ToString(), true, options.WebServiceUrl];
                            option.DisplayName = @"Web Service Url";
                            option.Category = @"General";
                            option.Description = @"The url specifying where the AutoUpdate Web Service can be located.";											
                        }
                        option.Value = options.WebServiceUrl;
                        break;
                };
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
		
        /// <summary>
        /// Writes the autoupdate options
        /// </summary>
        /// <param name="options"></param>
        public void WriteOptions(Razor.Networking.AutoUpdate.Common.AutoUpdateOptions options)
        {
            try
            {
                // get the common config
                XmlConfiguration configuration = SnapInHostingEngine.Instance.CommonConfiguration;
                Debug.Assert(configuration != null);

                // get the autoupdate category
                XmlConfigurationCategory category = configuration.Categories[@"AutoUpdate", true];
                Debug.Assert(category != null);

                XmlConfigurationOption option = null;

                // check
                if ((option = category.Options[AutoUpdateOptionNames.AutoCheck.ToString()]) == null)
                {
                    option = category.Options[AutoUpdateOptionNames.AutoCheck.ToString(), true, options.AutomaticallyCheckForUpdates];
                    option.DisplayName = @"Automatically Check for Updates";
                    option.Category = @"General";
                    option.Description = @"Determines whether the AutoUpdateManager automatically checks for updates on startup.";	
                }
                option.Value = options.AutomaticallyCheckForUpdates;
                option = null;

                // download
                if ((option = category.Options[AutoUpdateOptionNames.AutoDownload.ToString()]) == null)
                {
                    option = category.Options[AutoUpdateOptionNames.AutoDownload.ToString(), true, options.AutomaticallyDownloadUpdates];
                    option.DisplayName = @"Automatically Download Updates";
                    option.Category = @"General";
                    option.Description = @"Determines whether the AutoUpdateManager automatically downloads available updates without prompting.";											
                }
                option.Value = options.AutomaticallyDownloadUpdates;
                option = null;
				
                // install
                if ((option = category.Options[AutoUpdateOptionNames.AutoInstall.ToString()]) == null)
                {
                    option = category.Options[AutoUpdateOptionNames.AutoInstall.ToString(), true, options.AutomaticallyInstallUpdates];
                    option.DisplayName = @"Automatically Install Updates";
                    option.Category = @"General";
                    option.Description = @"Determines whether the AutoUpdateManager automatically installs the updates after downloading.";											
                }
                option.Value = options.AutomaticallyInstallUpdates;
                option = null;
				
                // switch
                if ((option = category.Options[AutoUpdateOptionNames.AutoSwitch.ToString()]) == null)
                {
                    option = category.Options[AutoUpdateOptionNames.AutoSwitch.ToString(), true, options.AutomaticallySwitchToNewVersion];
                    option.DisplayName = @"Automatically Switch to Newest Version";
                    option.Category = @"General";
                    option.Description = @"Determines whether the AutoUpdateManager automatically switches to the newest version after installation.";											
                }
                option.Value = options.AutomaticallySwitchToNewVersion;
                option = null;
				
                // update alt path
                if ((option = category.Options[AutoUpdateOptionNames.AutoUpdateAlternatePath.ToString()]) == null)
                {
                    option = category.Options[AutoUpdateOptionNames.AutoUpdateAlternatePath.ToString(), true, options.AutomaticallyUpdateAlternatePath];
                    option.DisplayName = @"Automatically Update Alternate Path";
                    option.Category = @"General";
                    option.Description = @"Determines whether the AutoUpdateManager automatically creates backup copies of the .Manifest and .Update files after installation.";											
                }
                option.Value = options.AutomaticallyUpdateAlternatePath;
                option = null;
				
                // alt path
                if ((option = category.Options[AutoUpdateOptionNames.AlternatePath.ToString()]) == null)
                {
                    option = category.Options[AutoUpdateOptionNames.AlternatePath.ToString(), true, options.AlternatePath];
                    option.DisplayName = @"Alternate Download Path";
                    option.Category = @"General";
                    option.Description = @"This alternate path (url or unc path) will be checked first before attempting to use the web service url to locate updates.";											
                }
                option.Value = options.AlternatePath;
                option = null;
				
                // url
                if ((option = category.Options[AutoUpdateOptionNames.WebServiceUrl.ToString()]) == null)
                {
                    option = category.Options[AutoUpdateOptionNames.WebServiceUrl.ToString(), true, options.WebServiceUrl];
                    option.DisplayName = @"Web Service Url";
                    option.Category = @"General";
                    option.Description = @"The url specifying where the AutoUpdate Web Service can be located.";											
                }
                option.Value = options.WebServiceUrl;
                option = null;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
		
        /// <summary>
        /// Deletes the autoupdate category
        /// </summary>
        public void DeleteCategory()
        {
            try
            {
                // get the common config
                XmlConfiguration configuration = SnapInHostingEngine.Instance.CommonConfiguration;
                Debug.Assert(configuration != null);

                // get the autoupdate category
                XmlConfigurationCategory category = configuration.Categories[@"AutoUpdate", true];
                if (category != null)
                    category.Remove();
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
		
        #endregion

        #region Common Configuration Events

        /// <summary>
        /// Occurs when a change is made to the common configuration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommonConfiguration_Changed(object sender, XmlConfigurationElementEventArgs e)
        {
            // bail if the element is being edited
            if (e.Element.IsBeingEdited)
                return;

            // we only care about options
            if (e.Element.GetElementType() == XmlConfigurationElementTypes.XmlConfigurationOption)
            {
                string[] optionNames = Enum.GetNames(typeof(AutoUpdateOptionNames));
                foreach(string optionName in optionNames)
                    if (string.Compare(e.Element.Fullpath, SnapInHostingEngine.DefaultCommonConfigurationName + @"\AutoUpdate\" + optionName, true) == 0)
                    {						
                        // read the network options
                        this.ReadMyCommonOptions();

                        // pass the event on
                        this.OnAutoUpdateOptionsChanged(this, e);
                        break;
                    }
            }		
        }

        #endregion

        #region

        /// <summary>
        /// Raises the AutoUpdateOptionChanged event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnAutoUpdateOptionsChanged(object sender, XmlConfigurationElementEventArgs e)
        {
            try
            {
                if (this.AutoUpdateOptionChanged != null)
                    this.AutoUpdateOptionChanged(sender, e);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns the AutoUpdate options
        /// </summary>
        public Razor.Networking.AutoUpdate.Common.AutoUpdateOptions AutoUpdateOptions
        {
            get
            {
                return _options;
            }
        }

        #endregion
    }
}
