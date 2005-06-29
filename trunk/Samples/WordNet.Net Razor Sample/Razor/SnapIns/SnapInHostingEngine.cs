/*
 * This file is a part of the Razor Framework.
 * 
 * Copyright (C) 2003 Mark (Code6) Belles 
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 * 
 * */

using System;
using System.Collections;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Razor;
using Razor.Configuration;
using Razor.Attributes;
using Razor.Searching;
using Razor.Features;
using Razor.Tracing;

namespace Razor.SnapIns
{	
    /// <summary>
    /// The hosting engine provides facilities to host runtime instances of classes exported from managed assemblies that implement the ISnapIn interface. 
    /// </summary>
    //	[System.Diagnostics.DebuggerStepThrough()]
    public class SnapInHostingEngine : IRunnable, IDisposable
    {	
        public const int DefaultMaxLogFiles = 9;
        public const string DefaultLogFileNamingScheme = "Log_{0}.txt";
        public const string DefaultLogFileSearchingPattern = "Log*.txt";
        private static SnapInHostingEngine _theInstance;
        private static string _commonDataPath;
        private static string _localUserDataPath;
        private static string _logsDataPath;
        private static string _logFilename;
        private static string _commonConfigurationFilename;
        private static string _localUserConfigurationFilename;
        private static string _configurationEngineConfigurationFilename;
        private static string _installationEngineConfigurationFilename;
        private static bool _verbose;
        private static bool _troubleshoot;
        private static bool _noLog;		
        private static bool _noMessageLoop;
        private static bool _noEncrypt;
        private bool _started;
        private bool _stopped;
        private Assembly _startingExecutable;
        private FileStream _logFileStream;
        private FormattedTextWriterTraceListener _logFileListener;		
        private XmlConfiguration _configurationEngineConfiguration;
        private XmlConfiguration _installationEngineConfiguration;
        private XmlConfiguration _commonConfiguration;
        private XmlConfiguration _localUserConfiguration;
        private SnapInDescriptor[] _descriptors;
        private CommandLineParsingEngine _commandLineParsingEngine;
        private SplashWindowThread _splashThread;
        private WindowManager _windowManger;
        private XmlConfigurationManager _xmlConfigurationManager;
        private MenuItemSecurityManager _menuItemSecurityManager;
        private ApplicationInstanceManager _instanceManager;
        private SnapInApplicationContext _applicationContext;

        [DllImport("User32.dll")]
        private extern static short GetAsyncKeyState(int key);	

        #region Public Events

        /// <summary>
        /// Fires when a SnapIn is started
        /// </summary>
        public event SnapInDescriptorEventHandler SnapInStarted;

        /// <summary>
        /// Fires when a SnapIn is stopped
        /// </summary>
        public event SnapInDescriptorEventHandler SnapInStopped;

        /// <summary>
        /// Fires when a SnapIn is installed
        /// </summary>
        public event SnapInDescriptorEventHandler SnapInInstalled;

        /// <summary>
        /// Fires when a SnapIn is uninstalled
        /// </summary>
        public event SnapInDescriptorEventHandler SnapInUninstalled;											

        /// <summary>
        /// Fires after all of the SnapIns have been started
        /// </summary>
        public event EventHandler AfterSnapInsStarted;
        
        /// <summary>
        /// Fires before all of the SnapIns are stopped
        /// </summary>
        public event EventHandler BeforeSnapInsStopped;

        /// <summary>
        /// Fires when a restart is pending the current hosting engine's death. The bootstrap is waiting.
        /// </summary>
        public event EventHandler RestartPending;

        #endregion
		
        /// <summary>
        /// Static constructor for the hosting engine
        /// </summary>
        /// <remarks>
        /// Formats the default data paths, and binds to the events of the PathCreationEngine and the Feature engine
        /// </remarks>
        static SnapInHostingEngine()
        {
            // check for the troubleshooting keyboard shortcut
            _troubleshoot = SnapInHostingEngine.IsKeyDown(Keys.ControlKey);		
		
            // retrieve the default common data path
            _commonDataPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData);

            // retrieve the default local user data path
            _localUserDataPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);

            // bind to the path creation engine's events
            PathCreationEngine.CreateDirectoryFailed += new PathCreationEngineEventHandler(SnapInHostingEngine.OnPathCreationEngineCreateDirectoryFailed);
            PathCreationEngine.DeleteFileFailed += new PathCreationEngineEventHandler(SnapInHostingEngine.OnPathCreationEngineDeleteFileFailed);

            // bind to the feature engine's events
            FeatureEngine.BuildingFeatureList += new FeatureCollectionEventHandler(SnapInHostingEngine.OnFeatureEngineBuildingFeatureList);
            //			FeatureEngine.BeforeActionTakenForFeature += new FeatureCancelEventHandler(SnapInHostingEngine.FeatureEngine_BeforeActionTakenForFeature);
            FeatureEngine.TakeActionForFeature += new FeatureEventHandler(SnapInHostingEngine.OnFeatureEngineTakeActionForFeature);
            //			FeatureEngine.AfterActionTakenForFeature += new FeatureEventHandler(SnapInHostingEngine.FeatureEngine_AfterActionTakenForFeature);
        }

        /// <summary>
        /// Initializes a new instance of the SnapInHostingEngine class
        /// </summary>
        /// <param name="additionalCommonAppDataPath">Additional path in which data common to all users will be stored. Example: "Company Name\Application Name\Version X.X.X"</param>
        /// <param name="additionalLocalUserAppDataPath">Additional path in which data particular to the current user will be stored. Example: "Company Name\Application Name\Version X.X.X"</param>
        public SnapInHostingEngine(string additionalCommonAppDataPath, string additionalLocalUserAppDataPath)
        {
            // thunk to this instance, the one and only
            _theInstance = this;

            // create a new application context
            _applicationContext = new SnapInApplicationContext();

            // create a new instance manager
            _instanceManager = new ApplicationInstanceManager();

            // combine the common app data path and the additional path to form this instance's common app data path
            _commonDataPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData), additionalCommonAppDataPath);

            // combine the local user app data path and the additional path to form this instance's local user app data path
            _localUserDataPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), additionalLocalUserAppDataPath);			

            // combine the local user path with logs to form the logs path
            _logsDataPath = Path.Combine(_localUserDataPath, "Logs");
			
            // combine the log path with the file name to form the full log path						
            _logFilename = Path.Combine(_logsDataPath, "Log.txt");

            // format the common configuration filename
            _commonConfigurationFilename = Path.Combine(_commonDataPath, SnapInHostingEngine.DefaultCommonConfigurationFilename);

            // format the local user configuration filename
            _localUserConfigurationFilename = Path.Combine(_localUserDataPath, SnapInHostingEngine.DefaultLocalUserConfigurationFilename);

            // format the configuration engine's configuration filename
            _configurationEngineConfigurationFilename = Path.Combine(_commonDataPath, ConfigurationEngine.DefaultConfigurationFilename);

            // format the installation engine's configuration filename
            _installationEngineConfigurationFilename = Path.Combine(_localUserDataPath, InstallationEngine.DefaultConfigurationFilename);

            // wire up to our own snapin events so that we can catch the events first and proxy some work before anyone else gets the events
            this.SnapInInstalled += new SnapInDescriptorEventHandler(OnInternalSnapInInstalled);
            this.SnapInUninstalled += new SnapInDescriptorEventHandler(OnInternalSnapInUninstalled);

            // create new window manager
            _windowManger = new WindowManager();

            // create a new xml configuration manager
            _xmlConfigurationManager = new XmlConfigurationManager();
            _xmlConfigurationManager.EnumeratingConfigurations += new XmlConfigurationManagerEventHandler(OnConfigurationManagerEnumeratingConfigurations);

            // create a new menu item security manager
            _menuItemSecurityManager = new MenuItemSecurityManager();			
        }		

        #region IDisposable Members

        /// <summary>
        /// Override the default Dispose method, suppress finalization for this class, and call the internal Dispose method
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Handles internal finalization, removes any trace and debug listeners if present
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.UninstallTraceListener();
            }
        }

        #endregion

        #region IRunnable Members
		
        /// <summary>
        /// Runs the current instance of the hosting engine. This creates a blocking thread message loop until a SnapIn calls Application.ExitThread in the context of the hosting engine.
        /// </summary>
        /// <param name="args">The command line arguments to pass to the hosting engine. See the CommandLineParsingEngine for capabilities and syntaxes supported.</param>
        /// <param name="executable">The assembly for the executable that is starting the hosting engine.</param>
        public void Run(string[] args, System.Reflection.Assembly executable)
        {
            try
            {
                // if we are not the only instance		
                if (!_instanceManager.IsOnlyInstance)
                {
                    // send the command line to the previous instance, and then bail
                    _instanceManager.SendCommandLineToPreviousInstance(args);
                    return;
                }

                _startingExecutable = executable;			
                _splashThread = new SplashWindowThread(executable);
                _splashThread.ShowAsynchronously();
                _splashThread.Window.SetMarqueeMoving(true, true);

                this.Run(args);
            }
            catch(System.Exception systemException)
            {
                System.Diagnostics.Trace.WriteLine(systemException);
            }
        }

        /// <summary>
        /// Runs the current instance of the hosting engine. This creates a blocking thread message loop until a SnapIn calls Application.ExitThread in the context of the hosting engine.
        /// </summary>
        /// <param name="args">The command line arguments to pass to the hosting engine. See the CommandLineParsingEngine for capabilities and syntaxes supported.</param>
        public void Run(string[] args)
        {	
            try
            {					
                // create the command line parsing engine
                _commandLineParsingEngine = new CommandLineParsingEngine(args);									

                // parse the command line for debugging flags
                this.ParseCommandLineForDebuggingFlags(_splashThread.Window);

                #region Create directories

                // create the common app data path, bail if it fails
                if (!PathCreationEngine.CreateDirectory(_commonDataPath, this, "CommonDataPath"))
                    return;			
				
                // create the local user app data path, bail if it fails
                if (!PathCreationEngine.CreateDirectory(_localUserDataPath, this, "LocalUserDataPath"))
                    return;																				
				
                // create the local user logs path, bail if it fails
                if (!PathCreationEngine.CreateDirectory(_logsDataPath, this, "LogsDataPath"))
                    return;

                #endregion

                // rotate the log files
                this.RotateLogFiles();

                // install the custom trace listener so that we can write to our log files
                this.InstallTraceListener(_splashThread.Window);

                // if we are in trouble shooting mode, then show the feature and trouble shooting dialog
                if (SnapInHostingEngine.TroubleshootingMode)
                    FeatureEngine.ShowFeatureWindow(this);					

                #region Read Configuration Files		

                // read or create the configuration engine configuration
                ProgressViewer.SetExtendedDescription(_splashThread.Window, "Reading configuration " + ConfigurationEngine.DefaultConfigurationName);
                if (!ConfigurationEngine.ReadOrCreateConfiguration(SnapInHostingEngine.VerboseMode, ConfigurationEngine.DefaultConfigurationName, SnapInHostingEngine.ConfigurationEngineConfigurationFilename, out _configurationEngineConfiguration, (_noEncrypt ? null : new RijndaelEncryptionEngine()), new XmlConfigurationEventHandler(this.OnFormatConfigurationeEngineConfiguration)))
                    return;				
				
                if (_configurationEngineConfiguration != null)
                {
                    _configurationEngineConfiguration.TimeToSave += new EventHandler(OnConfigurationEngineConfigurationTimeToSave);
                }

                // read or create the installation engine configuration
                ProgressViewer.SetExtendedDescription(_splashThread.Window, "Reading configuration " + InstallationEngine.DefaultConfigurationName);
                if (!ConfigurationEngine.ReadOrCreateConfiguration(SnapInHostingEngine.VerboseMode, InstallationEngine.DefaultConfigurationName, SnapInHostingEngine.InstallationEngineConfigurationFilename, out _installationEngineConfiguration, (_noEncrypt ? null : new RijndaelEncryptionEngine()), new XmlConfigurationEventHandler(this.OnFormatInstallationEngineConfiguration)))
                    return;				

                if (_installationEngineConfiguration != null)
                {
                    _installationEngineConfiguration.TimeToSave += new EventHandler(OnInstallationEngineConfigurationTimeToSave);
                }

                // read or create the common configuration
                ProgressViewer.SetExtendedDescription(_splashThread.Window, "Reading configuration " + SnapInHostingEngine.DefaultCommonConfigurationName);
                if (!ConfigurationEngine.ReadOrCreateConfiguration(SnapInHostingEngine.VerboseMode, SnapInHostingEngine.DefaultCommonConfigurationName, SnapInHostingEngine.CommonConfigurationFilename, out _commonConfiguration, (_noEncrypt ? null : new RijndaelEncryptionEngine()), new XmlConfigurationEventHandler(this.OnFormatCommonConfiguration)))
                    return;				

                if (_commonConfiguration != null)
                {
                    _commonConfiguration.TimeToSave += new EventHandler(OnCommonConfigurationTimeToSave);
                }

                // read or create the local user configuration
                ProgressViewer.SetExtendedDescription(_splashThread.Window, "Reading configuration " + SnapInHostingEngine.DefaultLocalUserConfigurationName);
                if (!ConfigurationEngine.ReadOrCreateConfiguration(SnapInHostingEngine.VerboseMode, SnapInHostingEngine.DefaultLocalUserConfigurationName, SnapInHostingEngine.LocalUserConfigurationFilename, out _localUserConfiguration, (_noEncrypt ? null : new RijndaelEncryptionEngine()), new XmlConfigurationEventHandler(this.OnFormatLocalUserConfiguration)))
                    return;
				
                if (_localUserConfiguration != null)
                {
                    _localUserConfiguration.TimeToSave += new EventHandler(OnLocalUserConfigurationTimeToSave);
                }

                #endregion
				
                #region Register Configuration Files with the ConfigurationEngine

                // register the installation engine configuration
                ConfigurationEngine.RegisterFile(this.ConfigurationEngineConfiguration, InstallationEngine.DefaultConfigurationName, SnapInHostingEngine.InstallationEngineConfigurationFilename );
				
                // register the common configuration
                ConfigurationEngine.RegisterFile(this.ConfigurationEngineConfiguration, SnapInHostingEngine.DefaultCommonConfigurationName, SnapInHostingEngine.CommonConfigurationFilename);
				
                // register the local user configuration
                ConfigurationEngine.RegisterFile(this.ConfigurationEngineConfiguration, SnapInHostingEngine.DefaultLocalUserConfigurationName, SnapInHostingEngine.LocalUserConfigurationFilename);

                #endregion
							
                // search for and create snapins that have not been uninstalled
                _descriptors = SnapInHostingEngine.SearchForSnapIns(new Search("SnapIns", Application.StartupPath, "*.dll", false, true), _splashThread.Window);  									
				
                // mark all of the descriptors that are missing dependencies
                SnapInDescriptor.MarkDescriptorsThatAreMissingDependencies(_descriptors);

                // sort the snapins from the least dependent first to the most dependent last
                SnapInDescriptor.Sort(_descriptors, true);

                // mark all of the descriptors that are circularly dependent according to the dependency attributes
                SnapInDescriptor.MarkDescriptorsThatAreCircularlyDependent(_descriptors);

                // mark all of the descriptors that have dependencies, which are they themselves circularly dependent
                SnapInDescriptor.MarkDescriptorsThatHaveDependenciesThatAreCircularlyDependent(_descriptors); 
				
                // mark all of the descriptors that have dependencies, 
                SnapInDescriptor.MarkDescriptorsThatHaveDependenciesThatAreMissingADependency(_descriptors);
				
                // start each SnapIn
                foreach(SnapInDescriptor descriptor in _descriptors)
                    SnapInHostingEngine.Start(descriptor, true, _splashThread.Window);

                // close the splash window
                _splashThread.Window.Close();

                // force garbage collection so we can get things rolling
                GC.Collect();

                // fire off the event to notify the world that all of the snapins have been started
                this.OnAfterSnapInsStarted(this, System.EventArgs.Empty);

                // if we are not supposed to ignore the message loop
                if (!_noMessageLoop)
                {
                    // run the main thread message loop
                    if (_applicationContext.HasOnlyOneMainForm)
                        Application.Run(_applicationContext.MainForm);                
                    else
                        Application.Run(_applicationContext);                
                }

                // fire off the event to notify the world that all of the snapins are about to be stopped
                this.OnBeforeSnapInsStopped(this, System.EventArgs.Empty);

                // sort the snapins from the most dependent first to the least dependent last
                SnapInDescriptor.Sort(_descriptors, false);
				
                // stop each SnapIn
                foreach(SnapInDescriptor descriptor in _descriptors)
                    SnapInHostingEngine.Stop(descriptor, true, null);
				
                #region Write Configuration Files

                // write the hosting engine configuration
                if (!ConfigurationEngine.WriteConfiguration(SnapInHostingEngine.VerboseMode, SnapInHostingEngine.ConfigurationEngineConfigurationFilename, SnapInHostingEngine.GetExecutingInstance().ConfigurationEngineConfiguration, (_noEncrypt ? null : new RijndaelEncryptionEngine())))
                    return;

                // write the hosting engine configuration
                if (!ConfigurationEngine.WriteConfiguration(SnapInHostingEngine.VerboseMode, SnapInHostingEngine.InstallationEngineConfigurationFilename, _installationEngineConfiguration, (_noEncrypt ? null : new RijndaelEncryptionEngine())))
                    return;

                // write the common configuration
                if (!ConfigurationEngine.WriteConfiguration(SnapInHostingEngine.VerboseMode, SnapInHostingEngine.CommonConfigurationFilename, SnapInHostingEngine.GetExecutingInstance().CommonConfiguration, (_noEncrypt ? null : new RijndaelEncryptionEngine())))
                    return;

                // write the local user configuration
                if (!ConfigurationEngine.WriteConfiguration(SnapInHostingEngine.VerboseMode, SnapInHostingEngine.LocalUserConfigurationFilename, SnapInHostingEngine.GetExecutingInstance().LocalUserConfiguration, (_noEncrypt ? null : new RijndaelEncryptionEngine())))
                    return;

                #endregion
            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex);
            }
            finally
            {
                _instanceManager.Dispose();
            }
        }

        #endregion			
		
        #region My Public Methods
		
        /// <summary>
        /// Restarts the application using the boostrapper
        /// </summary>
        public virtual void RestartUsingBootstrap()
        {
            try
            {
                // grab the starting assembly
                Assembly assembly = _startingExecutable;
				
                // grab its assembly name
                AssemblyName assemblyName = assembly.GetName();

                // look at the startup directory
                DirectoryInfo directory = new DirectoryInfo(System.Windows.Forms.Application.StartupPath);
                
                // jump to it's parent, that is going to be the download path for all updates (*.update)
                string bootstrapPath = Path.GetDirectoryName(directory.FullName);

                // start the bootstrap with the instructions to wait for us to quit
                string bootStrapFilename = Path.Combine(bootstrapPath, assemblyName.Name + ".exe");

                // format the bootstrap's command line and tell it to wait on this process
                string commandLine = string.Format("/pid:{0} /wait", System.Diagnostics.Process.GetCurrentProcess().Id.ToString());

                // start the bootstrapper
                System.Diagnostics.Process.Start(bootStrapFilename, commandLine);

                this.OnRestartPending(this, EventArgs.Empty);
            }
            catch(System.Exception systemException)
            {
                System.Diagnostics.Trace.WriteLine(systemException);
            }
        }

        /// <summary>
        /// Returns the descriptor for the Type specified
        /// </summary>
        /// <param name="type">The descriptor Type to search for</param>
        /// <returns></returns>
        public SnapInDescriptor FindDescriptorByType(Type type)
        {
            foreach(SnapInDescriptor descriptor in _descriptors)
                if (Type.Equals(descriptor.Type, type))
                    return descriptor;
            return null;
        }

        /// <summary>
        /// Returns the descriptor for the Type specified
        /// </summary>
        /// <param name="typename">The Type.FullName of the descriptor Type to search for</param>
        /// <returns></returns>
        public SnapInDescriptor FindDescriptorByTypeName(string typename)
        {
            foreach(SnapInDescriptor descriptor in _descriptors)
                if (string.Compare(descriptor.Type.FullName, typename, true) == 0)
                    return descriptor;
            return null;
        }

        /// <summary>
        /// Shows a ConfigurationWindow as a modal dialog with the specified owner
        /// </summary>
        /// <param name="owner">The window that will own the dialog when it is shown</param>
        /// <returns></returns>
        public DialogResult ShowConfigurationWindow(IWin32Window owner)
        {			
            DialogResult result = DialogResult.Cancel;
			
            // create a new xml configuration properties window
            XmlConfigurationPropertiesWindow window = new XmlConfigurationPropertiesWindow();
				
            // ask the window manager if we can show the window
            if (_windowManger.CanShow(window, new object[] {}))
            {
                // it can be shown, so now let us enumerate the available configurations
                XmlConfiguration[] configurations = _xmlConfigurationManager.EnumConfigurations(new XmlConfiguration[] {});
					
                // ask the window manager to track this window for us
                _windowManger.BeginTrackingLifetime(window, SnapInHostingEngine.WindowKeys.ConfigurationWindowKey);

                // select the available configurations into the xml configuration properties window
                window.SelectedConfigurations = new XmlConfigurationCollection(configurations);

                // display a local warning if permissions aren't sufficient
                window.XmlConfigurationView.DisplayWarningIfLocalFilePermissionsAreInsufficient();

                // show the window modally
                result = (owner == null ? window.ShowDialog() : window.ShowDialog(owner));
            }		

            return result;
        }

        /// <summary>
        /// Shows the SnapIns window using the specified window as it's owner
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public DialogResult ShowSnapInsWindow(IWin32Window owner)
        {
            DialogResult result = DialogResult.Cancel;

            // create the snapins window
            SnapInDescriptorsWindow window = new SnapInDescriptorsWindow(_descriptors);

            // ask the window manager if we can show the window
            if (_windowManger.CanShow(window, new object[] {}))
            {
                // ask the window manager to track this window for us
                _windowManger.BeginTrackingLifetime(window, SnapInHostingEngine.WindowKeys.SnapInsWindowKey);

                // show the window modally
                result = (owner == null ? window.ShowDialog() : window.ShowDialog(owner));
            }

            return result;
        }

        #endregion

        #region My Public Properties

        /// <summary>
        /// Returns the ApplicationContext that is run to keep the hosting engine alive after all SnapIns are started
        /// </summary>
        public SnapInApplicationContext ApplicationContext
        {
            get
            {
                return _applicationContext;
            }
        }

        /// <summary>
        /// Returns the ApplicationInstanceManager that will be used to maintain single instance functionality for this hosting engine
        /// </summary>
        public ApplicationInstanceManager InstanceManager
        {
            get
            {
                return _instanceManager;
            }
        }

        /// <summary>
        /// Returns a flag that indicates whether the current hosting engine is started
        /// </summary>
        public bool Started
        {
            get
            {
                return _started;
            }
        }

        /// <summary>
        /// Returns a flag that indicates whether the current hosting engine is stopped
        /// </summary>
        public bool Stopped
        {
            get
            {
                return _stopped;
            }
        }
		
        /// <summary>
        /// Gets the assembly that started the hosting engine
        /// </summary>
        public System.Reflection.Assembly StartingExecutable
        {
            get
            {
                return _startingExecutable;
            }
        }

        /// <summary>
        /// Gets the form that is implementing the IProgressViewer interface and being used as the splash window for the hosting engine
        /// </summary>
        public Form SplashWindow
        {
            get
            {
                return _splashThread.Window as Form;
            }
        }

        /// <summary>
        /// Gets the engine used for parsing command line arguments
        /// </summary>
        public CommandLineParsingEngine CommandLineParsingEngine
        {
            get
            {
                return _commandLineParsingEngine;
            }
        }

        /// <summary>
        /// Gets the configuration used by the ConfigurationEngine
        /// </summary>
        public XmlConfiguration ConfigurationEngineConfiguration
        {
            get
            {
                return _configurationEngineConfiguration;
            }
        }

        /// <summary>
        /// Gets the configuration used by the InstallationEngine
        /// </summary>
        public XmlConfiguration InstallationEngineConfiguration
        {
            get
            {
                return _installationEngineConfiguration;	
            }
        }

        /// <summary>
        /// Gets the configuration that is common to all users
        /// </summary>
        public XmlConfiguration CommonConfiguration
        {
            get
            {
                return _commonConfiguration;
            }
        }

        /// <summary>
        /// Gets the configuration that is specific to the current user
        /// </summary>
        public XmlConfiguration LocalUserConfiguration
        {
            get
            {
                return _localUserConfiguration; 
            }
        }

        /// <summary>
        /// Gets the collection of SnapIns hosted by this SnapInHostingEngine
        /// </summary>
        public SnapInDescriptor[] SnapInDescriptors
        {
            get
            {				
                return _descriptors;
            }
        }

        /// <summary>
        /// Returns the executing instance of the WindowManager
        /// </summary>
        public WindowManager WindowManager
        {
            get
            {
                return _windowManger;
            }
        }

        /// <summary>
        /// Returns the executing instance of the XmlConfigurationManager
        /// </summary>
        public XmlConfigurationManager XmlConfigurationManager
        {
            get
            {
                return _xmlConfigurationManager;
            }
        }

        /// <summary>
        /// Returns the executing instance of the MenuItemSecurityManager
        /// </summary>
        public MenuItemSecurityManager MenuItemSecurityManager
        {
            get
            {
                return _menuItemSecurityManager;
            }
        }

        /// <summary>
        /// Returns the current application version. First looks at the current folder, determines if it is the Debug, Release, or Current, and falls back upon the App.Config files, and then falls back upon the starting assembly version attribute.
        /// </summary>
        /// <returns></returns>
        public Version AppVersion
        {			
            get
            {
                string[] folderNames = {"Debug", "Release", "Current"};
                try
                {				
                    DirectoryInfo di = new DirectoryInfo(Application.StartupPath);
                    string szVersion = di.Name;
					
                    bool useAssembly = false;
                    foreach(string folderName in folderNames)
                    {
                        if (string.Compare(di.Name, folderName, true) == 0)
                        {
                            useAssembly = true;
                            break;
                        }
                    }
					
                    Version v = null;
                    if (!useAssembly)
                    {
                        try
                        {
                            v = new Version(szVersion);
                        }
                        catch(Exception)
                        {
                            useAssembly = true;
                        }
                    }
					
                    if (useAssembly)
                    {
                        AppSettingsReader reader = new AppSettingsReader();
                        try
                        {
                            szVersion = (string)reader.GetValue("AppVersion", typeof(string));
                            v = new Version(szVersion);
                        }
                        catch(Exception) { /* no point */}

                        // and yet, still as if being tortured by the gods, we have no version
                        if (v == null)
                        {
                            // we must fall back upon the lonely executable that started it all, and hope to read a version from it
                            AssemblyAttributeReader r = new AssemblyAttributeReader(SnapInHostingEngine.GetExecutingInstance().StartingExecutable);
                            szVersion = r.GetAssemblyVersion().ToString();					
                        }
                    }
									
                    v = new Version(szVersion);				
                    return v;
                }
                catch(System.Exception systemException)
                {
                    System.Diagnostics.Trace.WriteLine(systemException);
                }
                return null;
            }
        }

        #endregion Public Properties

        #region My Protected Methods

        /// <summary>
        /// Formats a configuration for use as the configuration engine's configuration 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnFormatConfigurationeEngineConfiguration(object sender, XmlConfigurationEventArgs e)
        {
            e.Element = ConfigurationEngine.DefaultConfigurationFormat;
        }	

        /// <summary>
        /// Formats a configuration for use as the hosting engine's configuration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnFormatInstallationEngineConfiguration(object sender, XmlConfigurationEventArgs e)
        {
            e.Element = InstallationEngine.DefaultConfigurationFormat;
        }	

        /// <summary>
        /// Formats a configuration for use as the common configuration 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnFormatCommonConfiguration(object sender, XmlConfigurationEventArgs e)
        {
            e.Element = SnapInHostingEngine.DefaultCommonConfigurationFormat;
        }
		
        /// <summary>
        /// Formats a configuration for use as the local user's configuration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnFormatLocalUserConfiguration(object sender, XmlConfigurationEventArgs e)
        {
            e.Element = SnapInHostingEngine.DefaultLocalUserConfigurationFormat;
        }

        /// <summary>
        /// Raises the SnapInStarted event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnSnapInStarted(object sender, SnapInDescriptorEventArgs e)
        {
            try
            {
                if (this.SnapInStarted != null)
                    this.SnapInStarted(sender, e);
            }
            catch(System.Exception systemException)
            {
                System.Diagnostics.Trace.WriteLine(systemException);
            }
        }

        /// <summary>
        /// Raises the SnapInStopped event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnSnapInStopped(object sender, SnapInDescriptorEventArgs e)
        {
            try
            {
                if (this.SnapInStopped != null)
                    this.SnapInStopped(sender, e);
            }
            catch(System.Exception systemException)
            {
                System.Diagnostics.Trace.WriteLine(systemException);
            }
        }

        /// <summary>
        /// Raises the SnapInInstalled event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnSnapInInstalled(object sender, SnapInDescriptorEventArgs e)
        {
            try
            {
                if (this.SnapInInstalled != null)
                    this.SnapInInstalled(sender, e);
            }
            catch(System.Exception systemException)
            {
                System.Diagnostics.Trace.WriteLine(systemException);
            }
        }

        /// <summary>
        /// Raises the SnapInUninstalled event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnSnapInUninstalled(object sender, SnapInDescriptorEventArgs e)
        {
            try
            {
                if (this.SnapInUninstalled != null)
                    this.SnapInUninstalled(sender, e);
            }
            catch(System.Exception systemException)
            {
                System.Diagnostics.Trace.WriteLine(systemException);
            }
        }		

        /// <summary>
        /// Raises the AfterSnapInsStarted event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnAfterSnapInsStarted(object sender, EventArgs e)
        {
            try
            {
                _started = true;

                if (this.AfterSnapInsStarted != null)
                    this.AfterSnapInsStarted(sender, e);				
            }
            catch(System.Exception systemException)
            {
                System.Diagnostics.Trace.WriteLine(systemException);
            }
        }

        /// <summary>
        /// Raises the BeforeSnapInsStopped event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnBeforeSnapInsStopped(object sender, EventArgs e)
        {
            try
            {
                _stopped = true;

                if (this.BeforeSnapInsStopped != null)
                    this.BeforeSnapInsStopped(sender, e);
            }
            catch(System.Exception systemException)
            {
                System.Diagnostics.Trace.WriteLine(systemException);
            }
        }

        /// <summary>
        /// Raises the RestartPending event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnRestartPending(object sender, EventArgs e)
        {
            try
            {
                if (this.RestartPending != null)
                    this.RestartPending(sender, e);
            }
            catch(System.Exception systemException)
            {
                System.Diagnostics.Trace.WriteLine(systemException);
            }
        }

        #endregion

        #region My Private Methods
		
        /// <summary>
        /// Parses the command line for supported debugging flags and sets their values if found
        /// </summary>
        private void ParseCommandLineForDebuggingFlags(IProgressViewer progressViewer)
        {
            try
            {
                ProgressViewer.SetExtendedDescription(progressViewer, "Parsing command line and app config...");

                // flags for reading from the command line and the app config file
                bool cmdLineVerbose, appConfigVerbose;
                bool cmdLineTroubleshoot, appConfigTroubleshoot;
                bool cmdLineNoLog, appConfigNoLog;
                bool cmdLineNoMessageLoop, appConfigNoMessageLoop;
                bool cmdLineNoEncrypt, appConfigNoEncrypt;

                // read options from the command line (NOTE: Command Line is case sensitive so these flags must be presented in LOWER case to function)
                cmdLineVerbose = _commandLineParsingEngine.ToBoolean("verbose");
                cmdLineTroubleshoot = _commandLineParsingEngine.ToBoolean("troubleshoot");
                cmdLineNoLog = _commandLineParsingEngine.ToBoolean("nolog");
                cmdLineNoMessageLoop = _commandLineParsingEngine.ToBoolean("nomessageloop");
                cmdLineNoEncrypt = _commandLineParsingEngine.ToBoolean("noencrypt");

                // read options from the app config file
                AppSettingsReader r = new AppSettingsReader();
                appConfigVerbose = bool.Parse((string)r.GetValue("Verbose", typeof(string)));
                appConfigTroubleshoot = bool.Parse((string)r.GetValue("Troubleshoot", typeof(string)));
                appConfigNoLog = bool.Parse((string)r.GetValue("NoLog", typeof(string)));
                appConfigNoMessageLoop = bool.Parse((string)r.GetValue("NoMessageLoop", typeof(string)));
                appConfigNoEncrypt = bool.Parse((string)r.GetValue("NoEncrypt", typeof(string)));

                // set flags from either command line flags or app config file
                _verbose = (cmdLineVerbose || appConfigVerbose);
                _troubleshoot = (cmdLineTroubleshoot || appConfigTroubleshoot);
                _noLog = (cmdLineNoLog || appConfigNoLog);
                _noMessageLoop = (cmdLineNoMessageLoop || appConfigNoMessageLoop);
                _noEncrypt = (cmdLineNoEncrypt || appConfigNoEncrypt);
            }
            catch(System.Exception systemException)
            {
                System.Diagnostics.Trace.WriteLine(systemException);
            }
        }

        /// <summary>
        /// Rotates the log files into the backup log files
        /// </summary>
        private void RotateLogFiles()
        {
            /*
             * Log File Naming Convention
             * 
             * string.Format("Log{0}.txt", number);
             * 
             * */

            int maxFiles = DefaultMaxLogFiles;
            string pattern = DefaultLogFileNamingScheme;

            try
            {
                /*
                 * the first step in backup is deleting the oldest file
                 * which in our case will be "LogX.txt" where X is maxFiles
                 * */
                string oldest = Path.Combine(_logsDataPath, string.Format(pattern, maxFiles));
                this.FileDelete(oldest);
				
                string oldName = null;
                string newName = null;				

                // now work backwards from the next to oldest file down to the oldest 				
                for(int i = maxFiles - 1; i > 1; i--)
                {
                    oldName = Path.Combine(_logsDataPath, string.Format(pattern, i));
                    newName = Path.Combine(_logsDataPath, string.Format(pattern, i+1));
                    this.FileMove(oldName, newName);				
                }
				
                /*
                 * now move the current log to the backup log
                 * */
                oldName = Path.Combine(_logsDataPath, "Log.txt");
                newName = Path.Combine(_logsDataPath, string.Format(pattern, 2));
                this.FileMove(oldName, newName);								
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Deletes any old log files that occur outside of the max log files setting
        /// </summary>
        private void DeleteOldLogFiles()
        {
            /*
             * Log File Naming Convention
             * 
             * string.Format("Log{0}.txt", number);
             * 
             * */

            int maxFiles = DefaultMaxLogFiles;
            string pattern = DefaultLogFileSearchingPattern;

            try
            {
                // create a search for files that fit the log file naming scheme
                Search search = new Search("Log Files", _logsDataPath, pattern, false, true);
				
                // search for the files
                FileInfo[] files = search.GetFiles();
				
                foreach(FileInfo file in files)
                {
                    try
                    {
                        // break the file name down into just the number of the log file
                        string name = file.Name;
                        name = name.Replace("Log_", null);
                        name = name.Replace(file.Extension, null);

                        // try and figure out the number of the log file
                        int number = int.Parse(name);

                        // try and delete any file that is a log file backup greater that the max number of log files
                        if (number > maxFiles)
                            this.FileDelete(file.FullName);
                    }
                    catch(FormatException)
                    {
                        continue;
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Deletes the file specified by the path
        /// </summary>
        /// <param name="path"></param>
        private void FileDelete(string path)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(string.Format("An exception was thrown while trying to delete '{0}'.\n\t{1}", path, ex.ToString()));
            }
        }

        /// <summary>
        /// Moves a file from one path to another
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        private void FileMove(string oldName, string newName)
        {
            try
            {			
                this.FileDelete(newName);

                if (File.Exists(oldName))
                    File.Move(oldName, newName);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(string.Format("An exception was thrown while trying to move '{0}' to '{1}'.\n\t{2}", oldName, newName, ex.ToString()));	
            }
        }

        /// <summary>
        /// Installs our logging trace listener if the NoLogging flag is not set
        /// </summary>
        private void InstallTraceListener(IProgressViewer progressViewer)
        {
            ProgressViewer.SetExtendedDescription(progressViewer, "Configuring custom Debug and Trace listeners...");

            if (!_noLog)
            {	
                //				try
                //				{													
                //					using(SecurityAccessRight right = new SecurityAccessRight(_logsDataPath))
                //					{
                //						if (right.Assert(SecurityAccessRights.FILE_WRITE_DATA))
                //						{
                _logFileStream = new FileStream(_logFilename, FileMode.Create, FileAccess.Write, System.IO.FileShare.Read);			
                _logFileListener = new FormattedTextWriterTraceListener(_logFileStream);
                _logFileListener.Name = "LogFileListener";							
                System.Diagnostics.Trace.Listeners.Add(_logFileListener);
                System.Diagnostics.Trace.AutoFlush = true;
                System.Diagnostics.Debug.AutoFlush = true;
                //						}
                //					}				
                //				}
                //				catch(System.Exception systemException)
                //				{
                //					System.Diagnostics.Trace.WriteLine(systemException);
                //				}				
            }
        }

        /// <summary>
        /// Uninstalls our logging trace listener if it was installed
        /// </summary>
        private void UninstallTraceListener()
        {
            if (!_noLog)
            {
                if (_logFileListener != null)
                {
                    System.Diagnostics.Trace.Listeners.Remove(_logFileListener.Name);
                    //					_logFileStream.Close();
                    _logFileListener.Dispose();
                }
            }
        }

        #endregion

        #region My Path Engine Events

        /// <summary>
        /// Handles the CreateDirectoryFailed event from the PathCreationEngine
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal static void OnPathCreationEngineCreateDirectoryFailed(object sender, PathCreationEngineEventArgs e)
        {
            if (e.UserData != null)
            {
                string key = e.UserData as string;
                if (key != null)
                {
                    switch (key)
                    {
                        case "CommonDataPath":
						
                            if (_verbose)
                            {
                                e.Result = ExceptionEngine.DisplayException(
                                    null,
                                    "Exception encountered - Unable to create common data directory",
                                    MessageBoxIcon.Error,
                                    MessageBoxButtons.AbortRetryIgnore, 
                                    e.Exception,
                                    "The path '" + e.Path + "' must exist in order to save data common to all users.",
                                    "Ignoring this exception could result in a loss of data or further exceptions.",					
                                    "Typical settings may include network, security, or other application related settings.",
                                    "Data in this path should be considered vital to the functionality and performance of the application.",					
                                    "WARNING: Aborting this operation will exit the application!");
                            }
                            else
                            {
                                e.Result = DialogResult.Ignore;
                            }
                            break;

                        case "LocalUserDataPath":						
						
                            if (_verbose)
                            {
                                e.Result = ExceptionEngine.DisplayException(
                                    null,
                                    "Exception encountered - Unable to create local user data directory",
                                    MessageBoxIcon.Error,
                                    MessageBoxButtons.AbortRetryIgnore, 
                                    e.Exception,
                                    "The path '" + e.Path + "' must exist in order to save data particular to the current user '" + System.Environment.UserName + "'.",
                                    "Ignoring this exception could result in a loss of data or further exceptions.",					
                                    "Typical settings may include window positions and other user interface related settings.",
                                    "Data in this path should be considered non-vital to the functionality and performance of the application.",					
                                    "WARNING: Aborting this operation will exit the application!");
                            }
                            else
                            {
                                e.Result = DialogResult.Ignore;
                            }
                            break;

                        case "LogsDataPath":

                            if (_verbose)
                            {
                                e.Result = ExceptionEngine.DisplayException(
                                    null,
                                    "Exception encountered - Unable to create the logs directory",
                                    MessageBoxIcon.Error,
                                    MessageBoxButtons.AbortRetryIgnore, 
                                    e.Exception,
                                    "The path '" + e.Path + "' must exist in order to save a log file containing debugging information.",
                                    "Ignoring this exception will not affect the performance of the application, however no debugging information will be saved for this instance of the program.",					
                                    "Data in this path should be considered usefull for debugging purposes only.",					
                                    "WARNING: Aborting this operation will exit the application!");
                            }
                            else
                            {
                                e.Result = DialogResult.Ignore;
                            }

                            if (e.Result == DialogResult.Ignore)
                                _noLog = true;

                            break;
                    };


                }
            }
        }

        /// <summary>
        /// Handles the DeleteFileFailed event from the PathCreationEngine
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal static void OnPathCreationEngineDeleteFileFailed(object sender, PathCreationEngineEventArgs e)
        {
            if (e.UserData != null)
            {
                string key = e.UserData as string;
                if (key != null)
                {
                    switch (key)
                    {
                        case SnapInHostingEngine.DefaultCommonConfigurationName:
						
                            if (_verbose)
                            {
                                e.Result = ExceptionEngine.DisplayException(
                                    null,
                                    "Exception encountered - Unable to delete configuration",
                                    MessageBoxIcon.Error,
                                    MessageBoxButtons.OK, 
                                    e.Exception,
                                    "The configuration '" + SnapInHostingEngine.DefaultCommonConfigurationName + "' could not be deleted.");
                            }
                            else
                            {
                                e.Result = DialogResult.OK;
                            }
                            break;

                        case SnapInHostingEngine.DefaultLocalUserConfigurationName:					
						
                            if (_verbose)
                            {
                                e.Result = ExceptionEngine.DisplayException(
                                    null,
                                    "Exception encountered - Unable to delete configuration",
                                    MessageBoxIcon.Error,
                                    MessageBoxButtons.OK, 
                                    e.Exception,
                                    "The configuration '" + SnapInHostingEngine.DefaultLocalUserConfigurationName + "' could not be deleted.");
                            }
                            else
                            {
                                e.Result = DialogResult.OK;
                            }
                            break;
                    };
                }
            }
        }

        #endregion

        #region My Feature Events

        /// <summary>
        /// Handles the BuildingFeatureList event from the FeatureEngine
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal static void OnFeatureEngineBuildingFeatureList(object sender, FeatureCollectionEventArgs e)
        {
            SnapInHostingEngine hostingEngine = SnapInHostingEngine.GetExecutingInstance();

            // add features for resetting the configuration files
            e.Features.Add(new ConfigurationFeature(SnapInHostingEngine.DefaultCommonConfigurationName, "The common configuration stores options common to all users.", FeatureActions.ResetToDefault));
            e.Features.Add(new ConfigurationFeature(SnapInHostingEngine.DefaultLocalUserConfigurationName, "The local user configuration stores options specific to the current user.", FeatureActions.ResetToDefault));

            // since this event may fire before we have loaded any snapins (as a result of the _troubleshoot flag from the command line or app config )
            // we need to make sure we descriptors before we try this
            if ( hostingEngine.SnapInDescriptors != null )
            {
                // add features for reinstalling the snapins
                foreach(SnapInDescriptor descriptor in hostingEngine.SnapInDescriptors)
                {
                    try
                    {
                        SnapInAttributeReader r = new SnapInAttributeReader(descriptor.Type);
                        SnapInTitleAttribute ta = r.GetSnapInTitleAttribute();
                        SnapInDescriptionAttribute da = r.GetSnapInDescriptionAttribute();

                        string name = descriptor.Type.FullName;
                        string description = null;

                        if (ta != null)
                            name = ta.Title;

                        if (da != null)
                            description = da.Description;

                        e.Features.Add(new SnapInFeature(name, description, descriptor.Type, FeatureActions.Reinstall));
                    }
                    catch(System.Exception systemException)
                    {
                        System.Diagnostics.Trace.WriteLine(systemException);
                    }
                }
            }
        }

        //		/// <summary>
        //		/// Handles the BeforeActionTakenForFeature event from the FeatureEngine
        //		/// </summary>
        //		/// <param name="sender"></param>
        //		/// <param name="e"></param>
        //		internal static void FeatureEngine_BeforeActionTakenForFeature(object sender, FeatureCancelEventArgs e)
        //		{
        //
        //		}

        /// <summary>
        ///  Handles the TakeActionForFeature event from the FeatureEngine
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal static void OnFeatureEngineTakeActionForFeature(object sender, FeatureEventArgs e)
        {									
            if (e.Feature != null)
            {
                // is the feature a configuration? if so try and delete the file
                if (e.Feature.GetType() == typeof(ConfigurationFeature))
                {
                    ConfigurationFeature cf = e.Feature as ConfigurationFeature;
                    if (cf != null)
                    {
                        switch(cf.ConfigurationName)
                        {
                            case SnapInHostingEngine.DefaultCommonConfigurationName:
                                if (e.Feature.Action == FeatureActions.ResetToDefault)
                                {
                                    if (PathCreationEngine.DeleteFile(SnapInHostingEngine.CommonConfigurationFilename, SnapInHostingEngine.GetExecutingInstance(), SnapInHostingEngine.DefaultCommonConfigurationName))
                                    {																		
                                        // read or create the common configuration
                                        XmlConfiguration configuration;									
                                        ConfigurationEngine.ReadOrCreateConfiguration(_verbose, SnapInHostingEngine.DefaultCommonConfigurationName, SnapInHostingEngine.CommonConfigurationFilename, out configuration, null, new XmlConfigurationEventHandler(SnapInHostingEngine.GetExecutingInstance().OnFormatCommonConfiguration));
                                        SnapInHostingEngine.GetExecutingInstance()._commonConfiguration = configuration;

                                        if (SnapInHostingEngine.GetExecutingInstance()._commonConfiguration != null)
                                        {
                                            SnapInHostingEngine.GetExecutingInstance()._commonConfiguration.TimeToSave += new EventHandler(SnapInHostingEngine.GetExecutingInstance().OnCommonConfigurationTimeToSave);
                                        }
                                    }
                                }
                                break;

                            case SnapInHostingEngine.DefaultLocalUserConfigurationName:
                                if (e.Feature.Action == FeatureActions.ResetToDefault)
                                {
                                    if (PathCreationEngine.DeleteFile(SnapInHostingEngine.LocalUserConfigurationFilename, SnapInHostingEngine.GetExecutingInstance(), SnapInHostingEngine.DefaultLocalUserConfigurationName))
                                    {									
                                        // read or create the local user configuration, use a place holder object because of referencing issues
                                        XmlConfiguration configuration;									
                                        ConfigurationEngine.ReadOrCreateConfiguration(_verbose, SnapInHostingEngine.DefaultLocalUserConfigurationName, SnapInHostingEngine.LocalUserConfigurationFilename, out configuration, null, new XmlConfigurationEventHandler(SnapInHostingEngine.GetExecutingInstance().OnFormatLocalUserConfiguration));									
                                        SnapInHostingEngine.GetExecutingInstance()._localUserConfiguration = configuration;

                                        if (SnapInHostingEngine.GetExecutingInstance()._localUserConfiguration != null)
                                        {
                                            SnapInHostingEngine.GetExecutingInstance()._localUserConfiguration.TimeToSave += new EventHandler(SnapInHostingEngine.GetExecutingInstance().OnLocalUserConfigurationTimeToSave);
                                        }
                                    }
                                }
                                break;
                        };
                    }
                }

                if (e.Feature.GetType() == typeof(SnapInFeature))
                {
                    SnapInFeature sf = e.Feature as SnapInFeature;
                    if (sf != null)
                    {						
                        SnapInDescriptor descriptor = SnapInHostingEngine.GetExecutingInstance().FindDescriptorByType(sf.Type);
                        if (descriptor != null)
                        {
                            if (e.Feature.Action == FeatureActions.Reinstall)
                                SnapInHostingEngine.Install(descriptor, true, null);
                        }
                    }
                }
            }
        }

        //		// <summary>
        //		/// Handles the AfterActionTakenForFeature event from the FeatureEngine
        //		/// </summary>
        //		/// <param name="sender"></param>
        //		/// <param name="e"></param>
        //		internal static void FeatureEngine_AfterActionTakenForFeature(object sender, FeatureEventArgs e)
        //		{
        //
        //		}

        #endregion

        #region My SnapIn Events

        /// <summary>
        /// Handles our own SnapInInstalled event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnInternalSnapInInstalled(object sender, SnapInDescriptorEventArgs e)
        {
            // mark all of the descriptors that are missing dependencies
            SnapInDescriptor.MarkDescriptorsThatAreMissingDependencies(_descriptors);
        }

        /// <summary>
        /// Handles our own SnapInUninstalled event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnInternalSnapInUninstalled(object sender, SnapInDescriptorEventArgs e)
        {
            // mark all of the descriptors that are missing dependencies
            SnapInDescriptor.MarkDescriptorsThatAreMissingDependencies(_descriptors);
        }

        #endregion

        #region My Configuration Events

        /// <summary>
        /// Occurs when the xml configuration manager is enumerating configurations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected internal virtual void OnConfigurationManagerEnumeratingConfigurations(object sender, XmlConfigurationManagerEventArgs e)
        {
            e.Configurations.Add(new XmlConfiguration[] {_installationEngineConfiguration, _commonConfiguration, _localUserConfiguration});
        }	

        /// <summary>
        /// Occurs when it is time to save the configuration engine's configuration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected internal virtual void OnConfigurationEngineConfigurationTimeToSave(object sender, EventArgs e)
        {
            // write the hosting engine configuration
            ConfigurationEngine.WriteConfiguration(SnapInHostingEngine.VerboseMode, SnapInHostingEngine.ConfigurationEngineConfigurationFilename, SnapInHostingEngine.GetExecutingInstance().ConfigurationEngineConfiguration, (_noEncrypt ? null : new RijndaelEncryptionEngine()));
        }

        /// <summary>
        /// Occurs when it is time to save the installation engine's configuration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected internal virtual void OnInstallationEngineConfigurationTimeToSave(object sender, EventArgs e)
        {
            // write the hosting engine configuration
            ConfigurationEngine.WriteConfiguration(SnapInHostingEngine.VerboseMode, SnapInHostingEngine.InstallationEngineConfigurationFilename, _installationEngineConfiguration, (_noEncrypt ? null : new RijndaelEncryptionEngine()));
        }

        /// <summary>
        /// Occurs when it is time to save the common configuration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected internal virtual void OnCommonConfigurationTimeToSave(object sender, EventArgs e)
        {
            // write the common configuration
            ConfigurationEngine.WriteConfiguration(SnapInHostingEngine.VerboseMode, SnapInHostingEngine.CommonConfigurationFilename, SnapInHostingEngine.GetExecutingInstance().CommonConfiguration, (_noEncrypt ? null : new RijndaelEncryptionEngine()));
        }

        /// <summary>
        /// Occurs when it is time to save the local user configuration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected internal virtual void OnLocalUserConfigurationTimeToSave(object sender, EventArgs e)
        {
            // write the local user configuration
            ConfigurationEngine.WriteConfiguration(SnapInHostingEngine.VerboseMode, SnapInHostingEngine.LocalUserConfigurationFilename, SnapInHostingEngine.GetExecutingInstance().LocalUserConfiguration, (_noEncrypt ? null : new RijndaelEncryptionEngine()));
        }

        #endregion

        #region My Static Properties

        /// <summary>
        /// Returns the one and only instance of the hosting engine
        /// </summary>
        /// <returns></returns>
        public static SnapInHostingEngine GetExecutingInstance()
        {
            return _theInstance;			
        }

        /// <summary>
        /// Returns the one and only instance of the hosting engine
        /// </summary>
        public static SnapInHostingEngine Instance
        {
            get
            {
                return _theInstance;
            }
        }

        /// <summary>
        /// Determines if a particular key is depressed on the keyboard
        /// </summary>
        /// <param name="key">The key whos state will be determined to be down or not</param>
        /// <returns></returns>
        public static bool IsKeyDown(System.Windows.Forms.Keys key)
        {
            if (GetAsyncKeyState((int)key) < 0)
                return true;
            return false;
        }

        /// <summary>
        /// Gets a value that determines if the trouble shooting mode has been enabled for the SnapInHostingEngine
        /// </summary>
        public static bool TroubleshootingMode
        {
            get
            {
                return _troubleshoot;
            }
        }

        /// <summary>
        /// Gets a value that determines if the verbose mode has been enabled for the SnapInHostingEngine
        /// </summary>
        public static bool VerboseMode
        {
            get
            {
                return _verbose;
            }
        }

        /// <summary>
        /// Gets a value that determines if the no logging mode has been enabled for the SnapInHostingEngine
        /// </summary>
        public static bool NoLogFiles
        {
            get
            {
                return _noLog;
            }
        }

        /// <summary>
        /// Gets or sets a flag that determines if the host engine
        /// </summary>
        public static bool NoMessageLoop
        {
            get
            {
                return _noMessageLoop;
            }
            set
            {
                _noMessageLoop = value;
            }
        }

        /// <summary>
        /// Gets or sets a flag that determines if the configuration files will be encrypted
        /// </summary>
        public static bool NoEncryption
        {
            get
            {
                return _noEncrypt;
            }
            set
            {
                _noEncrypt = value;
            }
        }
		
        /// <summary>
        /// Gets the path for the application data that is shared among all users
        /// </summary>
        public static string CommonDataPath
        {
            get
            {
                return _commonDataPath;
            }
        }

        /// <summary>
        /// Gets the path for the application data for the current user (non-roaming users only)
        /// </summary>
        public static string LocalUserDataPath
        {
            get
            {
                return _localUserDataPath;
            }
        }

        /// <summary>
        /// Gets the path to the log files
        /// </summary>
        public static string LogsDataPath
        {
            get
            {
                return _logsDataPath;
            }
        }

        /// <summary>
        /// Gets the default name for a Common configuration
        /// </summary>
        public const string DefaultCommonConfigurationName = @"CommonConfiguration";
			
        /// <summary>
        /// Gets the default filename for a Common configuration
        /// </summary>
        public const string DefaultCommonConfigurationFilename = SnapInHostingEngine.DefaultCommonConfigurationName + ".xml";
        
        /// <summary>
        /// Gets an XmlConfiguration object that contains the default format a Common configuration
        /// </summary>
        public static XmlConfiguration DefaultCommonConfigurationFormat
        {
            get
            {
                XmlConfiguration configuration = new XmlConfiguration();
                configuration.ElementName = SnapInHostingEngine.DefaultCommonConfigurationName;
                return configuration;
            }
        }

        /// <summary>
        /// Gets the default name for a Local User configuration
        /// </summary>
        public const string DefaultLocalUserConfigurationName = @"LocalUserConfiguration";
	
        /// <summary>
        /// Gets the default filename for a Local User configuration
        /// </summary>
        public const string DefaultLocalUserConfigurationFilename = SnapInHostingEngine.DefaultLocalUserConfigurationName + ".xml";
			
        /// <summary>
        /// Gets an XmlConfiguration object that contains the default format for a Local User configuration
        /// </summary>
        public static XmlConfiguration DefaultLocalUserConfigurationFormat
        {
            get
            {
                XmlConfiguration configuration = new XmlConfiguration();
                configuration.ElementName = SnapInHostingEngine.DefaultLocalUserConfigurationName;
                return configuration;
            }
        }

        /// <summary>
        /// Gets the full filename including path to the common configuration file
        /// </summary>
        public static string CommonConfigurationFilename
        {
            get
            {
                return _commonConfigurationFilename;
            }
        }

        /// <summary>
        /// Gets the full filename including the path to the local user configuration file
        /// </summary>
        public static string LocalUserConfigurationFilename
        {
            get
            {
                return _localUserConfigurationFilename;
            }
        }

        /// <summary>
        /// Gets the full filename including the path to the configuration engine's configuration file
        /// </summary>
        public static string ConfigurationEngineConfigurationFilename
        {
            get
            {
                return _configurationEngineConfigurationFilename;
            }
        }

        /// <summary>
        /// Gets the full filename including the path to the installation engine's configuration file
        /// </summary>
        public static string InstallationEngineConfigurationFilename
        {
            get
            {
                return _installationEngineConfigurationFilename;
            }
        }		

        #endregion

        #region My Static Methods
		
        /// <summary>
        /// Searches the application path for assemblies that contain SnapIns. Returns a collection of SnapIns that match our criteria.
        /// </summary>
        /// <param name="search">The search to execute</param>
        /// <param name="ignoreUninstalledVersions">A flag indicating whether we should ignore uninstalled versions of a SnapIn</param>
        /// <param name="progressViewer">The progress viewer by which we can report information about our progress</param>
        /// <returns></returns>
        public static SnapInDescriptor[] SearchForSnapIns(Search search, IProgressViewer progressViewer)
        {
            ArrayList descriptors = new ArrayList();
            try
            {
                ProgressViewer.SetExtendedDescription(progressViewer, "Searching assemblies for exported SnapIns...");

                // set the location algorithm, the quickest is the metadata algorithm
                SnapInProvider.LocationAlgorithm = SnapInLocationAlgorithms.LocateUsingMetadata;
				
                // find the types that constitute eligible SnapIns
                ArrayList snapInsFound = SnapInProvider.FindSnapIns(search, progressViewer);
				
                // iterate over the provided types and create those that suffice our search parameters
                foreach(RuntimeClassProviderEventArgs classProviderArgs in snapInsFound)
                {																			
                    try
                    {
                        // create an instance of the type we found
                        object instance = SnapInHostingEngine.CreateSnapIn(classProviderArgs.Type, progressViewer);
                        if (instance != null)
                        {		
                            SnapInDescriptor descriptor = new SnapInDescriptor(classProviderArgs.Type, (ISnapIn)instance);

                            // lookup the versions of this type that are uninstalled
                            Version[] versions = InstallationEngine.GetUninstalledVersionsOfType(SnapInHostingEngine.GetExecutingInstance().InstallationEngineConfiguration, classProviderArgs.Type);
				
                            // if the current version is included in the versions, then we should skip this one
                            if (InstallationEngine.IsVersionOfTypeIncluded(versions, classProviderArgs.Type))
                                descriptor._isUninstalled = true;
							
                            descriptors.Add(descriptor);
                        }
                    }
                    catch(System.Exception systemException)
                    {
                        System.Diagnostics.Trace.WriteLine(systemException);
                    }					
                }
            }
            catch(System.Exception systemException)
            {
                System.Diagnostics.Trace.WriteLine(systemException);
            }
            return descriptors.ToArray(typeof(SnapInDescriptor)) as SnapInDescriptor[];
        }
		
        /// <summary>
        /// Creates an object instance from the specified Type using the default constructor
        /// </summary>
        /// <param name="t">The Type to create an instance of</param>
        /// <param name="progressViewer"></param>
        /// <returns></returns>
        internal static object CreateSnapIn(Type t, IProgressViewer progressViewer)
        {
            try
            {
                ConstructorInfo ci = t.GetConstructor(Type.EmptyTypes);
                if (ci != null)
                {
                    ProgressViewer.SetExtendedDescription(progressViewer, "Creating instance of '" + t.Name + "'");

                    object instance = ci.Invoke(new object[] {});
                    if (instance != null)
                        return instance;
                }
            }
            catch(System.Exception systemException)
            {
                if (t != null)
                    System.Diagnostics.Trace.WriteLine("Failed to create an instance of '" + t.FullName + "'");
                System.Diagnostics.Trace.WriteLine(systemException);
            }
            return null;
        }
		
        /// <summary>
        /// Installs or upgrades the SnapIn specified by the descriptor
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="ignoreHistory"></param>
        /// <param name="progressViewer"></param>
        internal static void Install(SnapInDescriptor descriptor, bool ignoreHistory, IProgressViewer progressViewer)
        {
            try
            {
                // get the current running instance of the engine
                SnapInHostingEngine engine = SnapInHostingEngine.GetExecutingInstance();

                bool installed = false;
					
                // ignore the history
                if (!ignoreHistory)
                {
                    // get the versions already installed
                    Version[] versions = InstallationEngine.GetInstalledVersionsOfType(engine.InstallationEngineConfiguration, descriptor.Type);

                    DateTime installDate;
                    int runCount;

                    // if the current version is not included in the installed versions
                    if (!InstallationEngine.IsVersionOfTypeIncluded(versions, descriptor.Type))
                    {
                        // the current version wasn't installed, but it could be an upgrade
                        if (InstallationEngine.IsVersionOfTypeAnUpgrade(versions, descriptor.Type))
                        {
                            System.Diagnostics.Trace.WriteLineIf(_verbose, string.Format("Upgrading SnapIn '{0}'", descriptor.Type.FullName));

                            // fire upgrade events
                            descriptor.SnapIn.OnUpgradeCommonOptions(null, System.EventArgs.Empty);
                            descriptor.SnapIn.OnUpgradeLocalUserOptions(null, System.EventArgs.Empty);
                        }

                        // the type needs to be installed						
                        InstallationEngine.InstallVersionOfType(engine.InstallationEngineConfiguration, descriptor.Type, out installDate);
                        descriptor._installDate = installDate;

                        // flag the installation
                        installed = true;				
                    }

                    // increment the run count
                    InstallationEngine.IncrementRunCountForVersionOfType(engine.InstallationEngineConfiguration, descriptor.Type, out installDate, out runCount);
                    descriptor._installDate = installDate;
                    descriptor._runCount = runCount;
                }

                // if we are ignoring the history of the snapin or it was just installed
                if (ignoreHistory || installed)
                {
                    ProgressViewer.SetExtendedDescription(progressViewer, "Installing " + descriptor.MetaData.Title);

                    System.Diagnostics.Trace.WriteLineIf(_verbose, "Installing " + descriptor.MetaData.Title);

                    // fire the installation events 
                    descriptor.SnapIn.OnInstallCommonOptions(null, System.EventArgs.Empty);
                    descriptor.SnapIn.OnInstallLocalUserOptions(null, System.EventArgs.Empty);
                    descriptor._isUninstalled = false;

                    if (installed)
                    {
                        SnapInHostingEngine.GetExecutingInstance().OnSnapInInstalled(SnapInHostingEngine.GetExecutingInstance(), new SnapInDescriptorEventArgs(descriptor));

                        ProgressViewer.SetExtendedDescription(progressViewer, descriptor.MetaData.Title + " installed successfully.");
                    }
                }
            }
            catch(System.Exception systemException)
            {
                System.Diagnostics.Trace.WriteLine(systemException);
            }
        }

        /// <summary>
        /// Uninstalls the SnapIn specified by the descriptor
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="progressViewer"></param>
        internal static void Uninstall(SnapInDescriptor descriptor, IProgressViewer progressViewer)
        {
            try
            {
                // get the current running instance of the engine
                SnapInHostingEngine engine = SnapInHostingEngine.GetExecutingInstance();

                // get the versions that are uninstalled
                Version[] versions = InstallationEngine.GetUninstalledVersionsOfType(engine.InstallationEngineConfiguration, descriptor.Type);

                // if the current version is uninstalled
                if (!InstallationEngine.IsVersionOfTypeIncluded(versions, descriptor.Type))
                {
                    ProgressViewer.SetExtendedDescription(progressViewer, "Uninstalling " + descriptor.MetaData.Title);

                    System.Diagnostics.Trace.WriteLineIf(_verbose, "Uninstalling " + descriptor.MetaData.Title);

                    // uninstall it
                    InstallationEngine.UninstallVersionOfType(engine.InstallationEngineConfiguration, descriptor.Type);						

                    // fire the uninstall events
                    descriptor.SnapIn.OnUninstallCommonOptions(null, System.EventArgs.Empty);
                    descriptor.SnapIn.OnUninstallLocalUserOptions(null, System.EventArgs.Empty);					
                    descriptor._isUninstalled = true;

                    SnapInHostingEngine.GetExecutingInstance().OnSnapInUninstalled(SnapInHostingEngine.GetExecutingInstance(), new SnapInDescriptorEventArgs(descriptor));

                    ProgressViewer.SetExtendedDescription(progressViewer, descriptor.MetaData.Title + " uninstalled successfully.");
                }				
            }
            catch(System.Exception systemException)
            {
                System.Diagnostics.Trace.WriteLine(systemException);
            }
        }
			
        /// <summary>
        /// Allows the SnapIn specified by the descriptor to read it's common and local user options
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="progressViewer"></param>
        internal static void TriggerReadOptions(SnapInDescriptor descriptor, IProgressViewer progressViewer)
        {
            try
            {
                System.Diagnostics.Trace.WriteLineIf(_verbose, string.Format("Allowing SnapIn '{0}' to read options", descriptor.Type.FullName));

                descriptor.SnapIn.OnReadCommonOptions(null, System.EventArgs.Empty);
                descriptor.SnapIn.OnReadLocalUserOptions(null, System.EventArgs.Empty);								
            }
            catch(System.Exception systemException)
            {
                System.Diagnostics.Trace.WriteLine(systemException);
            }
        }

        /// <summary>
        /// Allows the SnapIn specified by the descriptor to write it's common and local user options
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="progressViewer"></param>
        internal static void TriggerWriteOptions(SnapInDescriptor descriptor, IProgressViewer progressViewer)
        {
            try
            {
                System.Diagnostics.Trace.WriteLineIf(_verbose, string.Format("Allowing SnapIn '{0}' to write options", descriptor.Type.FullName));

                descriptor.SnapIn.OnWriteCommonOptions(null, System.EventArgs.Empty);
                descriptor.SnapIn.OnWriteLocalUserOptions(null, System.EventArgs.Empty);								
            }
            catch(System.Exception systemException)
            {
                System.Diagnostics.Trace.WriteLine(systemException);
            }
        }

        /// <summary>
        /// Starts the SnapIn specified by the descriptor (Handles installation, upgrades, and reading options)
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="progressViewer"></param>
        public static bool Start(SnapInDescriptor descriptor, bool startDependenciesFirst, IProgressViewer progressViewer)
        {
            try
            {
                if (descriptor != null)
                {
                    // if the snapin is not missing a dependency, cicularly referencing another snapin, or the same for it's dependencies
                    if (!descriptor.IsUninstalled &&
                        !descriptor.IsMissingDependency &&
                        !descriptor.IsCircularlyDependent &&
                        !descriptor.IsDependentOnTypeThatIsMissingDependency &&
                        !descriptor.IsDependentOnTypeThatIsCircularlyDependent)
                    {
                        if (!descriptor.IsStarted)
                        {
                            bool linksStarted = true;
                            if (startDependenciesFirst)
                            {
                                // create the chain
                                SnapInDescriptorLink link = SnapInHostingEngine.CreateDependencyChainForSnapInsThatThisSnapInDependsOn(descriptor, SnapInHostingEngine.GetExecutingInstance().SnapInDescriptors);

                                // start the link
                                linksStarted = SnapInHostingEngine.StartLink(link, progressViewer);

                                if (!linksStarted)
                                {
                                    // something fucked up, one the snapins we depend on did not start
                                    // so we cannot start
                                }
                            }

                            if (linksStarted && !descriptor.IsStarted)
                            {
                                ProgressViewer.SetExtendedDescription(progressViewer, "Starting the '" + descriptor.MetaData.Title + "' SnapIn...");

                                // install the snapin, which will cover upgrades, and run counting
                                SnapInHostingEngine.Install(descriptor, false, progressViewer);

                                // trigger read options
                                SnapInHostingEngine.TriggerReadOptions(descriptor, progressViewer);

                                // start the snapin
                                System.Diagnostics.Trace.WriteLineIf(_verbose, string.Format("Starting SnapIn '{0}'", descriptor.Type.FullName));
                                descriptor.SnapIn.OnStart(null, System.EventArgs.Empty);								
                                descriptor._isStarted = true;	
					
                                // raise the start event
                                SnapInHostingEngine.GetExecutingInstance().OnSnapInStarted(SnapInHostingEngine.GetExecutingInstance(), new SnapInDescriptorEventArgs(descriptor));

                                ProgressViewer.SetExtendedDescription(progressViewer, descriptor.MetaData.Title + " started successfully.");

                                return true;
                            }
                        }
                        // it's already started
                        return true;
                    }
                }
            }
            catch(System.Exception systemException)
            {
                System.Diagnostics.Trace.WriteLine(systemException);
            }
            return false;
        }

        /// <summary>
        /// Stops the SnapIn specified by the descriptor (Handles stopping dependents first optionally, and the writing of options)
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="progressViewer"></param>
        public static bool Stop(SnapInDescriptor descriptor, bool stopDependentsFirst, IProgressViewer progressViewer)
        {
            try
            {			
                if (descriptor.IsStarted)
                {
                    bool linksStopped = true;
                    if (stopDependentsFirst)
                    {
                        // create the chain
                        SnapInDescriptorLink link = SnapInHostingEngine.CreateDependencyChainForSnapInsThatDependOnThisSnapIn(descriptor, SnapInHostingEngine.GetExecutingInstance().SnapInDescriptors);
						
                        // stop each link in the chain, this method will walk the chain recursively until all links have been stopped
                        linksStopped = SnapInHostingEngine.StopLink(link, progressViewer);

                        if (!linksStopped)
                        {
                            // something fucked up, one the snapins that depends on us did not stop
                            // so we cannot stop
                        }
                    }
							
                    if (linksStopped && descriptor.IsStarted)
                    {
                        ProgressViewer.SetExtendedDescription(progressViewer, "Stopping the '" + descriptor.MetaData.Title + "' SnapIn...");

                        // stop the snapin
                        System.Diagnostics.Trace.WriteLineIf(_verbose, string.Format("Stopping SnapIn '{0}'", descriptor.Type.FullName));
                        descriptor.SnapIn.OnStop(null, System.EventArgs.Empty);				
                        descriptor._isStarted = false;

                        // raise the stop event
                        SnapInHostingEngine.GetExecutingInstance().OnSnapInStopped(SnapInHostingEngine.GetExecutingInstance(), new SnapInDescriptorEventArgs(descriptor));

                        // force the snapin to write it's options
                        SnapInHostingEngine.TriggerWriteOptions(descriptor, progressViewer);

                        ProgressViewer.SetExtendedDescription(progressViewer, descriptor.MetaData.Title + " stopped successfully.");

                        return true;
                    }
                }
                // it's already stopped
                return true;
            }
            catch(System.Exception systemException)
            {
                System.Diagnostics.Trace.WriteLine(systemException);
            }
            return false;
        }
		
        /// <summary>
        /// (Upwards) Creates a chain of descriptors for all of the snapins that this snapin depends upon, this is recursive and will graph the entire tree
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="descriptors"></param>
        /// <returns></returns>
        public static SnapInDescriptorLink CreateDependencyChainForSnapInsThatThisSnapInDependsOn(SnapInDescriptor descriptor, SnapInDescriptor[] descriptors)
        {
            // create a new link for the descriptor specified
            SnapInDescriptorLink link = new SnapInDescriptorLink(descriptor);

            // find any descriptors that this descriptor depends on
            foreach(Type dependency in descriptor.Dependencies)
            {
                foreach(SnapInDescriptor otherDescriptor in descriptors)
                {
                    if (Type.Equals(otherDescriptor.Type, dependency))
                    {
                        // find all of the dependency's chain, and do it resursiviely (Upwards)
                        SnapInDescriptorLink nextLink = SnapInHostingEngine.CreateDependencyChainForSnapInsThatThisSnapInDependsOn(otherDescriptor, descriptors);
                        link.Links.Add(nextLink);
                    }	
                }
            }
            return link;
        }

        /// <summary>
        /// (Downwards) Creates a chain of descriptors for all of the snapins that depend on the specified snapin, this is recursive and will graph the entire tree
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="descriptors"></param>
        /// <returns></returns>
        public static SnapInDescriptorLink CreateDependencyChainForSnapInsThatDependOnThisSnapIn(SnapInDescriptor descriptor, SnapInDescriptor[] descriptors)
        {
            // create a new link for the descriptor specified
            SnapInDescriptorLink link = new SnapInDescriptorLink(descriptor);

            // find any other descriptors that depend on the specified descriptor passed in
            foreach(SnapInDescriptor otherDescriptor in descriptors)
            {
                foreach(Type dependency in otherDescriptor.Dependencies)
                {
                    if (Type.Equals(descriptor.Type, dependency))
                    {
                        // find all of that dependency's chain, and do it recursively (Downwards)
                        SnapInDescriptorLink nextLink = SnapInHostingEngine.CreateDependencyChainForSnapInsThatDependOnThisSnapIn(otherDescriptor, descriptors);							
                        link.Links.Add(nextLink);
                    }
                }
            }
            return link;
        }

        internal static bool StartLink(SnapInDescriptorLink link, IProgressViewer progressViewer)
        {
            if (link != null)
            {
                // start all of the links that this link depends on first
                bool linksStarted = true;
                foreach(SnapInDescriptorLink l in link.Links)
                {
                    // stop this link's dependency chain
                    linksStarted = SnapInHostingEngine.StartLink(l, progressViewer);
                    if (!linksStarted)
                        break;
                }

                // if all of the links were started
                if (linksStarted)
                    // we can finally start the link and back out
                    return SnapInHostingEngine.Start(link.Descriptor, false, progressViewer);
            }
            return false;
        }

        /// <summary>
        /// Follows the link to the end of the chain, recursively, and stops each link after it's dependencies have been stopped
        /// </summary>
        /// <param name="link"></param>
        /// <param name="progressViewer"></param>
        internal static bool StopLink(SnapInDescriptorLink link, IProgressViewer progressViewer)
        {
            if (link != null)
            {
                // stop the links that depend on this link first
                bool linksStopped = true;
                foreach(SnapInDescriptorLink l in link.Links)
                {
                    // start this link's dependency chain
                    linksStopped = SnapInHostingEngine.StopLink(l, progressViewer);
                    if (!linksStopped)
                        break;
                }
				
                // if all of the links that depend on this have stopped
                if (linksStopped)
                    // then finally stop this link, and back out
                    return SnapInHostingEngine.Stop(link.Descriptor, false, progressViewer);
            }
            return false;
        }

        public static bool StartWithProgress(SnapInDescriptor descriptor)
        {
            bool result = false;
            try
            {
                using (ProgressWindowThread thread = new ProgressWindowThread())
                {
                    thread.ShowAsynchronously();
                    ProgressViewer.SetTitle(thread.Window, "SnapIn Hosting Engine Progress...");
                    ProgressViewer.SetHeading(thread.Window, "Starting " + descriptor.MetaData.Title);
                    ProgressViewer.SetDescription(thread.Window, "This operation may take several seconds.");					
					
                    System.Threading.Thread.Sleep(1000);
                    result = SnapInHostingEngine.Start(descriptor, true, thread.Window);
                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch(System.Exception systemException)
            {
                System.Diagnostics.Trace.WriteLine(systemException);
            }
            return result;
        }

        public static bool StopWithProgress(SnapInDescriptor descriptor)
        {
            bool result = false;
            try
            {
                using (ProgressWindowThread thread = new ProgressWindowThread())
                {
                    thread.ShowAsynchronously();
                    ProgressViewer.SetTitle(thread.Window, "SnapIn Hosting Engine Progress...");
                    ProgressViewer.SetHeading(thread.Window, "Stopping " + descriptor.MetaData.Title);
                    ProgressViewer.SetDescription(thread.Window, "This operation may take several seconds.");					
					
                    System.Threading.Thread.Sleep(1000);
                    result = SnapInHostingEngine.Stop(descriptor, true, thread.Window);
                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch(System.Exception systemException)
            {
                System.Diagnostics.Trace.WriteLine(systemException);
            }
            return result;
        }

        public static bool StopAndUninstallWithProgress(SnapInDescriptor descriptor)
        {
            bool result = false;
            try
            {
                using (ProgressWindowThread thread = new ProgressWindowThread())
                {
                    thread.ShowAsynchronously();
                    ProgressViewer.SetTitle(thread.Window, "SnapIn Hosting Engine Progress...");
                    ProgressViewer.SetHeading(thread.Window, "Uninstalling " + descriptor.MetaData.Title);
                    ProgressViewer.SetDescription(thread.Window, "This operation may take several seconds.");					
					
                    System.Threading.Thread.Sleep(1000);
                    SnapInHostingEngine.Uninstall(descriptor, thread.Window);
                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch(System.Exception systemException)
            {
                System.Diagnostics.Trace.WriteLine(systemException);
            }
            return result;
        }

        public static void ReinstallWithProgress(SnapInDescriptor descriptor)
        {
            try
            {
                using (ProgressWindowThread thread = new ProgressWindowThread())
                {
                    thread.ShowAsynchronously();
                    ProgressViewer.SetTitle(thread.Window, "SnapIn Hosting Engine Progress...");
                    ProgressViewer.SetHeading(thread.Window, "Installing " + descriptor.MetaData.Title);
                    ProgressViewer.SetDescription(thread.Window, "This operation may take several seconds.");	
				
                    System.Threading.Thread.Sleep(1000);
                    SnapInHostingEngine.Install(descriptor, false, thread.Window);
                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch(System.Exception systemException)
            {
                System.Diagnostics.Trace.WriteLine(systemException);
            }
        }

        #endregion

        #region My Window Keys Class

        // these are used with the window manager

        /// <summary>
        /// This class contains the keys to the various windows that will use the WindowManager class for location and permissions
        /// </summary>
        public class WindowKeys
        {
            public const string ConfigurationWindowKey = "{a12210b1-7198-403f-b4c8-273197444a30}";
            public const string SnapInsWindowKey = "{1bf41cf6-6baa-4dbc-b40b-77f742083a4a}";
            public const string FeaturesWindowKey = "{426dd570-39fe-4f66-8639-114f6d0ba46b}";
        }

        #endregion
    }
}
