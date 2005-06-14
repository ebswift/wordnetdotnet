#define ALIAS_DEBUG

using System;
using System.Diagnostics;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Net;
using System.Threading;
using System.Reflection;
using System.Windows.Forms;

using Razor.Configuration;
using Razor;
using Razor.SnapIns;
using Razor.Attributes;
using Razor.MultiThreading;
using Razor.Networking.AutoUpdate.Common;
using Razor.Networking.AutoUpdate.Common.Xml;

using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace Razor.Networking.AutoUpdate
{	
    /// <summary>
    /// Summary description for AutoUpdateManager.
    /// </summary>
    public class AutoUpdateManager 
    {
        /// <summary>
        /// The options used to control the behaviour of the manager and downloaders
        /// </summary>
        protected AutoUpdateOptions _options;			

        /// <summary>
        /// The list of downloaders the manager will try to use to download later versions of the specified product
        /// </summary>
        protected AutoUpdateDownloaderList _downloaders;

        /// <summary>
        /// The description of the product to be updated
        /// </summary>
        protected AutoUpdateProductDescriptor _productToUpdate;

        /// <summary>
        /// The background thread upon which all work in the auto update process will occur
        /// </summary>
        protected BackgroundThread _thread;

        protected const string MY_TRACE_CATEGORY = @"'AutoUpdateManager'";

        #region My Public Events

        /// <summary>
        /// Occurs when the manager has started with its auto update process
        /// </summary>
        public event AutoUpdateManagerEventHandler AutoUpdateProcessStarted;

        /// <summary>
        /// Occurs before the manager queries the downloaders for their latest version
        /// </summary>
        public event AutoUpdateManagerCancelEventHandler BeforeQueryForLatestVersion;	
		
        /// <summary>
        /// Occurs after the manager has queried the downloaders and determined the latest version available 
        /// </summary>
        public event AutoUpdateManagerWithDownloadDescriptorEventHandler AfterQueryForLatestVersion;	

        /// <summary>
        /// Occurs when no later version is available for download
        /// </summary>
        public event AutoUpdateManagerEventHandler NoLaterVersionAvailable;								

        /// <summary>
        /// Occurs before the manager downloads an update
        /// </summary>
        public event AutoUpdateManagerWithDownloadDescriptorCancelEventHandler BeforeDownload;			

        /// <summary>
        /// Occurs after the manager has downloaded an update
        /// </summary>
        public event AutoUpdateManagerWithDownloadDescriptorEventHandler AfterDownload;				

        /// <summary>
        /// Occurs before the manager installs an update
        /// </summary>
        public event AutoUpdateManagerWithDownloadDescriptorCancelEventHandler BeforeInstall;			

        /// <summary>
        /// Occurs after the manager has installed an update
        /// </summary>
        public event AutoUpdateManagerWithDownloadDescriptorEventHandler AfterInstall;					

        /// <summary>
        /// Occurs before the manager attempts to copy the update to the alternate path
        /// </summary>
        public event AutoUpdateManagerWithDownloadDescriptorCancelEventHandler BeforeUpdateAlternatePath;

        /// <summary>
        /// Occurs after the manager has updated the alternate path
        /// </summary>
        public event AutoUpdateManagerWithDownloadDescriptorEventHandler AfterUpdateAlternatePath;

        /// <summary>
        /// Occurs before the manager switches to the latest version
        /// </summary>
        public event AutoUpdateManagerWithDownloadDescriptorCancelEventHandler BeforeSwitchToLatestVersion;		

        /// <summary>
        /// Occurs when the manager is ready to allow the bootstrap to switch to the latest version
        /// </summary>
        public event AutoUpdateManagerWithDownloadDescriptorEventHandler SwitchToLatestVersion;

        /// <summary>
        /// Occurs when the manager encounters an exception
        /// </summary>
        public event AutoUpdateExceptionEventHandler Exception;

        /// <summary>
        /// Occurs when the manager is finished with it's auto update process
        /// </summary>
        public event AutoUpdateManagerEventHandler AutoUpdateProcessEnded;									

        #endregion

        /// <summary>
        /// Initializes a new instance of the AutoUpdateManager class
        /// </summary>
        /// <param name="options">The options that will control the behaviour of the engine</param>
        public AutoUpdateManager(AutoUpdateOptions options) 
        {
            // we can't do anything without options to control our behavior
            Debug.Assert(options != null);

            /*
             * the default options will be used
             * to update the current hosting engine 
             * and download into the bootstrap directory along side the other versions of this hosting engine
             * */
            _options = options;					
            _productToUpdate = AutoUpdateProductDescriptor.FromAssembly(SnapInHostingEngine.Instance.StartingExecutable, SnapInHostingEngine.Instance.AppVersion);			
            _downloaders = new AutoUpdateDownloaderList();
            _downloaders.AddRange(this.CreateDownloadersForInternalUse());
            if (_options.DownloadPath == null || _options.DownloadPath == string.Empty)
                _options.DownloadPath = this.GetBootstrapPath();		
        }

        /// <summary>
        /// Initializes a new instance of the AutoUpdateManager class
        /// </summary>
        /// <param name="options">The options that will control the behaviour of the engine</param>
        /// <param name="productToUpdate">A product descriptor that will be used as the product to find updates for</param>
        public AutoUpdateManager(AutoUpdateOptions options, AutoUpdateProductDescriptor productToUpdate)
        {
            Debug.Assert(options != null);
            Debug.Assert(productToUpdate != null);

            /*
             * however if we wanted to not do the norm
             * and create an update engine that could update another app
             * then we are all about it, makes no never mind at all
             * */
            _options = options;
            _productToUpdate = productToUpdate;
            _downloaders = new AutoUpdateDownloaderList();
            _downloaders.AddRange(this.CreateDownloadersForInternalUse());
            if (_options.DownloadPath == null || _options.DownloadPath == string.Empty)
                _options.DownloadPath = this.GetBootstrapPath();		
        }	

        #region My Public Properties

        /// <summary>
        /// Gets or sets the options used by the AutoUpdateManager
        /// </summary>
        public AutoUpdateOptions Options
        {
            get
            {
                return _options;
            }
            set
            {
                Debug.Assert(value != null);
                _options = value;
                if (_options.DownloadPath == null || _options.DownloadPath == string.Empty)
                    _options.DownloadPath = this.GetBootstrapPath();	
            }
        }
		
        /// <summary>
        /// Gets or sets the product descriptor that describes the product that the autoupdate engine will try and find updates for
        /// </summary>
        public AutoUpdateProductDescriptor ProductToUpdate
        {
            get
            {
                return _productToUpdate;
            }
            set
            {
                Debug.Assert(value != null);
                _productToUpdate = value;
            }
        }
		
        /// <summary>
        /// Gets or sets the list of downloads the manager will use to find and download updates for the product specified
        /// </summary>
        public AutoUpdateDownloaderList Downloaders
        {
            get
            {
                return _downloaders;
            }
            set
            {
                Debug.Assert(value != null);
                _downloaders = value;
            }
        }

        /// <summary>
        /// Returns a flag that indicates whether the engine is running (ie. checking, downloading, intalling... that sort of thing)
        /// </summary>
        public bool IsRunning
        {
            get
            {
                if (_thread == null)
                    return false;

                return _thread.IsRunning;
            }
        }

        #endregion 
		
        #region My Public Methods

        /// <summary>
        /// Asyncronously begins a background thread which maintains a list of AutoUpdateManagers to check for available updates
        /// </summary>
        public void BeginCheckingForUpdates()
        {	           
            // if the thread is null reset it
            if (_thread == null)
            {
                // each instance of the engine will use a background thread to perform it's work
                _thread = new BackgroundThread();
                _thread.Run += new BackgroundThreadStartEventHandler(OnThreadRun);
                _thread.Finished += new BackgroundThreadEventHandler(OnThreadFinished);	
                _thread.AllowThreadAbortException = true;
            }

            // if the thread is not running
            if (!_thread.IsRunning)
                // start it up
                _thread.Start(true, new object[] {});			
        }

        /// <summary>
        /// Syncronously ends a previous call that began the check for updates
        /// </summary>
        public void EndCheckingForUpdates()
        {
            // if the thread is running 
            if (_thread.IsRunning)
                // stut it down
                _thread.Stop();
        }
						
        #endregion

        #region My Protected Options		

        /// <summary>
        /// Returns the product's bootstrap path, which will be where all .update files are downloaded and installed from.
        /// </summary>
        /// <returns></returns>
        public virtual string GetBootstrapPath()
        {
            // look at the startup directory
            DirectoryInfo directory = new DirectoryInfo(System.Windows.Forms.Application.StartupPath);
	                
            // jump to it's parent, that is going to be the download path for all updates (*.update)
            return Path.GetDirectoryName(directory.FullName);
        }

        /// <summary>
        /// Returns an array of downloads that this instance of the AutoUpdateManager 
        /// </summary>
        /// <returns></returns>
        protected virtual AutoUpdateDownloader[] CreateDownloadersForInternalUse()
        {
            // create an array of downloaders 
            return new AutoUpdateDownloader[] {new UncPathAutoUpdateDownloader(), new HttpAutoUpdateDownloader()};
        }

        /// <summary>
        /// Sorts the available downloads and returns the newest download descriptor as the one that should be downloaded
        /// </summary>
        /// <param name="updates"></param>
        /// <returns></returns>
        protected virtual AutoUpdateDownloadDescriptor SelectTheDownloadWithTheNewestUpdate(AutoUpdateDownloadDescriptor[] downloadDescriptors)
        {
            try
            {
                // if there are no downloads
                if (downloadDescriptors == null)
                    // then simply say so
                    return null;

                if (downloadDescriptors.Length > 0)
                {
                    // otherwise, sort them into descending order with the newest version at index zero.
                    downloadDescriptors = AutoUpdateDownloadDescriptor.Sort(downloadDescriptors);
				
                    // simply return the first one
                    return downloadDescriptors[0];
                }
            }
            catch(ThreadAbortException)
            {

            }

            // otherwise don't
            return null;
        }		
		
        /// <summary>
        /// Installs the .update file specified by decrypting it and then unziping the contents to a new versioned directory (ie. 1.0.0.1)
        /// </summary>
        /// <param name="progressViewer"></param>
        /// <param name="updateFilename"></param>
        /// <returns></returns>
        protected virtual bool InstallUpdate(IProgressViewer progressViewer, AutoUpdateDownloadDescriptor downloadDescriptor)
        {
            string zipFilename = null;
            try
            {
                Debug.WriteLine(string.Format("Preparing to install update from '{0}'.", downloadDescriptor.DownloadedPath), MY_TRACE_CATEGORY);

                // decrypt the .update file first				
                if (!this.DecryptToZip(progressViewer, downloadDescriptor, out zipFilename))
                    return false;

                // then unzip the .zip file
                if (this.Unzip(progressViewer, downloadDescriptor, zipFilename))
                {	// delete the zip file
                    File.Delete(zipFilename);
                    return true;
                }
            }
            catch(ThreadAbortException)
            {
                try
                {
                    // delete the zip file
                    File.Delete(zipFilename);
                }
                catch(Exception)
                {

                }
            }

            return false;
        }

        /// <summary>
        /// Decrypts the .update file to a .zip file using the name of the .update file as a base in the same directory as the .update file
        /// </summary>
        /// <param name="progressViewer"></param>
        /// <param name="updateFilename"></param>
        /// <param name="zipFilename"></param>
        /// <returns></returns>
        protected virtual bool DecryptToZip(IProgressViewer progressViewer, AutoUpdateDownloadDescriptor downloadDescriptor, out string zipFilename)
        {
            zipFilename = null;
            try
            {						
                ProgressViewer.SetExtendedDescription(progressViewer, "Parsing update...");

                // save the path to the update
                string updateFilename = downloadDescriptor.DownloadedPath;

                // format the .zip file
                zipFilename = Path.Combine(Path.GetDirectoryName(updateFilename), Path.GetFileName(updateFilename).Replace(Path.GetExtension(updateFilename), null) + ".zip");
				
                // it is rijndael encrypted
                RijndaelEncryptionEngine ee = new RijndaelEncryptionEngine();
				
                // decrypt the .update file to a .zip file
                Debug.WriteLine(string.Format("Converting the update into an archive.\n\tThe archive will exist at '{0}'.", zipFilename), MY_TRACE_CATEGORY);
                ee.Decrypt(updateFilename, zipFilename);				
				
                return true;
            }
            catch(ThreadAbortException)
            {

            }
            return false;
        }

        /// <summary>
        /// Runs the specified file if it exists
        /// </summary>
        /// <param name="progressViewer"></param>
        /// <param name="file"></param>
        protected virtual void RunFileIfFound(IProgressViewer progressViewer, string file)
        {
            try
            {
                bool fileExists = File.Exists(file);
                Trace.WriteLine(string.Format("Preparing to start a process.\n\tThe file '{0}' {1}.", file, (fileExists ? "exists" : "does not exist")), MY_TRACE_CATEGORY);				

                if (fileExists)
                {
                    ProgressViewer.SetExtendedDescription(progressViewer, string.Format("Creating process '{0}'...",Path.GetFileName(file)));	

                    ProcessStartInfo pi = new ProcessStartInfo();

                    pi.FileName = file;
                    pi.WorkingDirectory = new FileInfo(file).DirectoryName;

                    Process p = Process.Start(pi);
                    if (p != null)
                    {
                        p.WaitForExit(); 
                    }
                }	
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex);
            }
            finally
            {
                ProgressViewer.SetExtendedDescription(progressViewer, null);
            }
        }

        /// <summary>
        /// Unzips the .zip file to a directory of it's own using the name of the .zip file as a base
        /// </summary>
        /// <param name="progressViewer"></param>
        /// <param name="zipFilename"></param>
        /// <returns></returns>
        protected virtual bool Unzip(IProgressViewer progressViewer, AutoUpdateDownloadDescriptor downloadDescriptor, string zipFilename)
        {
            ZipInputStream zipStream = null;
            FileStream fs = null;
            string newVersionPath = null;

            try
            {												
                // calculate the rootname 
                string rootName = Path.GetFileName(zipFilename).Replace(Path.GetExtension(zipFilename), null);

                // the text to remove includes the name of the product and a dash
                string prependedTextToRemove = string.Format("{0}-", downloadDescriptor.Manifest.Product.Name);
				
                // remove that and we're left with a version
                rootName = rootName.Replace(prependedTextToRemove, null);
				
                // extract here
                string rootPath = Path.GetDirectoryName(zipFilename);
				
                // the destination where the files will be unzipped for the new version
                newVersionPath = Path.Combine(rootPath, rootName);
				
                // make sure the directory where the new version will be extracted exists
                bool folderExists = Directory.Exists(newVersionPath);
                Debug.WriteLine(string.Format("Confirming the new version's path.\n\tThe folder '{0}' {1}.", newVersionPath, (folderExists ? "already exists" : "does not exist")), MY_TRACE_CATEGORY);
                if (!folderExists)
                {
                    Debug.WriteLine(string.Format("Creating the new verion's folder '{0}'.", newVersionPath), MY_TRACE_CATEGORY);
                    Directory.CreateDirectory(newVersionPath);
                }

                // try and find the postbuildevent.bat file
                string postBuildFile = Path.Combine(rootPath, "PostBuildEvent.bat");

                // open the zip file using a zip input stream
                Debug.WriteLine(string.Format("Opening the archive '{0}' for reading.", zipFilename), MY_TRACE_CATEGORY);
                zipStream = new ZipInputStream(File.OpenRead(zipFilename));								
				
                // ready each zip entry
                ZipEntry zipEntry;
                while((zipEntry = zipStream.GetNextEntry()) != null)
                {					
                    try
                    {
                        string zipEntryFilename = Path.Combine(rootPath, zipEntry.Name);
                        zipEntryFilename = zipEntryFilename.Replace("/", "\\");
	                    
                        // trace the entry to where it is going
                        Debug.WriteLine(string.Format("Extracting '{0}' to '{1}'.", zipEntry.Name, zipEntryFilename), MY_TRACE_CATEGORY);

                        if (zipEntry.IsDirectory)
                        {
                            Debug.WriteLine(string.Format("Creating the folder '{0}'.", zipEntryFilename), MY_TRACE_CATEGORY);
                            Directory.CreateDirectory(zipEntryFilename);
                        }
                        else
                        {
                            ProgressViewer.SetExtendedDescription(progressViewer, "Extracting " + zipEntry.Name + "...");
							
                            // make sure the directory exists
                            FileInfo fi = new FileInfo(zipEntryFilename);
                            DirectoryInfo di = fi.Directory;
                            if (!Directory.Exists(di.FullName))
                                Directory.CreateDirectory(di.FullName);
                            fi = null;
                            di = null;

                            // create each file
                            fs = File.Create(zipEntryFilename);
                            int size = 2048;
                            byte[] data = new byte[size];
                            while(true)
                            {
                                size = zipStream.Read(data, 0, data.Length);
                                if (size > 0)
                                    fs.Write(data, 0, size);
                                else
                                    break;
                            }
                            // close the extracted file
                            fs.Close();
                            fs = null;
                        }
                    }					
                    catch(Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }

                // close the zip stream
                zipStream.Close();
				
                RunFileIfFound(progressViewer, postBuildFile);

                return true;
            }
            catch(ThreadAbortException)
            {
                try
                {					
                    // make sure the streams are closed
                    if (zipStream != null)
                        zipStream.Close();

                    if (fs != null)
                        fs.Close();

                    // delete the root folder of the new install upon cancellation
                    Directory.Delete(newVersionPath, true);
                }
                catch(Exception)
                {

                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                // make sure the streams are closed
                if (zipStream != null)
                    zipStream.Close();

                if (fs != null)
                    fs.Close();
            }

            return false;
        }

        /// <summary>
        /// Adjusts the url where the .update can be found, using the product descriptor and the alternate download path 
        /// </summary>
        /// <remarks>
        /// By default the url probably points to some web url. When the update is copied to the alternate download path
        /// we want the download to occur from that path, so the url in the manifest must be manipulated to point the download
        /// to the update in the alternate path.
        /// </remarks>
        /// <param name="options"></param>
        /// <param name="downloadDescriptor"></param>
        public virtual void AdjustUrlOfUpdateInManifest(AutoUpdateOptions options, AutoUpdateDownloadDescriptor downloadDescriptor)
        {
            /*
             * we're supposed to be adjusting the the url where the update can be downloaded
             * to point to the alternate path
             * where ideally the manifest file will reside alongside the update
             * 
             * the problem is that the manifest file contains a url to where the update was originally downloaded
             * most likely some web server somewhere, which after the manifest is copied to some network file share
             * we won't want to use, more likely in an effort to keep the downloaders from going over the web
             * we'll try and tweak the url to just be the unc path of the download as it resides next to the 
             * manifest in the alternate download path
             * 
             * */

            try
            {

                // if there's no alternate path, don't worry about adjusting anything
                // as nothing is going to be copied anyways
                if (options.AlternatePath == null || options.AlternatePath == string.Empty)
                    // if there
                    return;

                // redirect the url of the update to the alternate location
                downloadDescriptor.Manifest.UrlOfUpdate = string.Format("{0}\\{1}\\{1}-{2}.Update", options.AlternatePath, downloadDescriptor.Manifest.Product.Name, downloadDescriptor.Manifest.Product.Version.ToString());			
            }
            catch(ThreadAbortException)
            {

            }
        }

        /// <summary>
        /// Creates a copy of the manifest file in the alternate path
        /// </summary>
        /// <param name="downloadDescriptor"></param>
        public virtual void CreateCopyOfManifestInAlternatePath(IProgressViewer progressViewer, AutoUpdateDownloadDescriptor downloadDescriptor)
        {
            try
            {
                // if the alternate path is not set, then we don't have to do this
                if (downloadDescriptor.Options.AlternatePath == null ||	downloadDescriptor.Options.AlternatePath == string.Empty)
                    return;

                // format a path to the product's alternate path
                string altPath = Path.Combine(downloadDescriptor.Options.AlternatePath, downloadDescriptor.Manifest.Product.Name);

                // if the path doesn't exist, just bail, we don't create alternate paths
                bool folderExists = Directory.Exists(altPath);
                Debug.WriteLine(string.Format("Confirming the product's 'Alternate Download Path' folder.\n\tThe folder '{0}' {1}.", altPath, (folderExists ? "already exists" : "does not exist")), MY_TRACE_CATEGORY);
                if (!folderExists)
                {
                    Debug.WriteLine(string.Format("Creating the product's 'Alternate Download Path' folder at '{0}'.", altPath), MY_TRACE_CATEGORY);
                    Directory.CreateDirectory(altPath);
                }
				
                // format a path to the file in the alternate path
                string dstPath = Path.Combine(altPath, string.Format("{0}-{1}.manifest", downloadDescriptor.Manifest.Product.Name, downloadDescriptor.Manifest.Product.Version.ToString()));

                bool fileExists = File.Exists(dstPath);
                Debug.WriteLine(string.Format("Preparing to copy the manifest to the product's 'Alternate Download Path' folder.\n\tThe file '{0}' {1}.", dstPath, (fileExists ? "already exists" : "does not exist")), MY_TRACE_CATEGORY);
							
                // otherwise write the manifest to the alternate path
                ProgressViewer.SetExtendedDescription(progressViewer, "Creating a backup copy of the manifest file.");
                Debug.WriteLine(string.Format("Copying the manifest to '{0}'.", dstPath), MY_TRACE_CATEGORY);
                XmlAutoUpdateManifestWriter.Write(downloadDescriptor.Manifest, dstPath, System.Text.Encoding.UTF8);				
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Creates a copy of the update file in the alternate path
        /// </summary>
        /// <param name="progressViewer"></param>
        /// <param name="updateFilename"></param>
        /// <returns></returns>
        public virtual void CreateCopyOfUpdateInAlternatePath(IProgressViewer progressViewer, AutoUpdateDownloadDescriptor downloadDescriptor)
        {				 													
            try
            {
                // if the alternate path is not set, then we don't have to do this
                if (downloadDescriptor.Options.AlternatePath == null ||	downloadDescriptor.Options.AlternatePath == string.Empty)
                    return;

                // take the alternate path
                string altPath = Path.Combine(downloadDescriptor.Options.AlternatePath, downloadDescriptor.Manifest.Product.Name);

                // see if the folder exists
                bool folderExists = Directory.Exists(altPath);				
                Debug.WriteLine(string.Format("Confirming the product's 'Alternate Download Path' folder.\n\tThe folder '{0}' {1}.", altPath, (folderExists ? "already exists" : "does not exist")), MY_TRACE_CATEGORY);
                if (!folderExists)
                {
                    Debug.WriteLine(string.Format("Creating the product's 'Alternate Download Path' folder at '{0}'.", altPath), MY_TRACE_CATEGORY);
                    Directory.CreateDirectory(altPath);
                }
	
                // format the backup filename from the alternate path, and the url where the update
                string dstPath = Path.Combine(altPath, Path.GetFileName(downloadDescriptor.Manifest.UrlOfUpdate));

                // see if the file already exists
                bool fileExists = File.Exists(dstPath);
                Debug.WriteLine(string.Format("Preparing to copy the update to the product's 'Alternate Download Path' folder.\n\tThe file '{0}' {1}.", dstPath, (fileExists ? "already exists" : "does not exist")), MY_TRACE_CATEGORY);
				
                // copy the .update we downloaded to the backup location in the alternate path directory
                ProgressViewer.SetExtendedDescription(progressViewer, "Creating a backup copy of the update file.");
                Debug.WriteLine(string.Format("Copying the update to '{0}'.", dstPath), MY_TRACE_CATEGORY);				
                File.Copy(downloadDescriptor.DownloadedPath, dstPath, false);								
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }		

        /// <summary>
        /// Occurs when the engine's background thread runs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ea"></param>
        protected virtual void OnThreadRun(object sender, BackgroundThreadStartEventArgs threadStartEventArgs)
        {
            bool downloaded = false;
            bool installed = false;
            bool finalized = false;
            AutoUpdateDownloadDescriptor recommendedUpdateDescriptor = null;

            try
            {
                /*
                 * Raise the AutoUpdateProcessStarted event
                 * */
                AutoUpdateManagerEventArgs startedArgs = new AutoUpdateManagerEventArgs(this, null);
                this.OnAutoUpdateProcessStarted(this, startedArgs);

                #region Step 1. QueryForLatestVersion

                /*
                 * Raise the BeforeQueryForLatestVersion event
                 * */
                AutoUpdateManagerCancelEventArgs beforeQueryArgs = new AutoUpdateManagerCancelEventArgs(this, startedArgs.ProgressViewer, false);
                this.OnBeforeQueryForLatestVersion(this, beforeQueryArgs);

                // create an array list to hold all of the available updates
                ArrayList listOfAvailableDownloads = new ArrayList();				
				
                // use the downloaders to check for downloads
                foreach(AutoUpdateDownloader downloader in _downloaders)
                {
                    try
                    {
                        // query the latest update available for the specified product
                        AutoUpdateDownloadDescriptor updateAvailable;						
						
                        // if the downloader finds an update is available
                        if (downloader.QueryLatestVersion(beforeQueryArgs.ProgressViewer, _options, _productToUpdate, out updateAvailable))														
                            // add it to the list of downloads available
                            listOfAvailableDownloads.Add(updateAvailable);						
                    }
                    catch(ThreadAbortException ex)
                    {
                        throw new Exception(ex.Message, ex);
                    }
                    catch(Exception ex)
                    {
                        Trace.WriteLine(ex);
                    }
                }

                // create a simple array of the updates that are available for download
                AutoUpdateDownloadDescriptor[] availableDownloads = listOfAvailableDownloads.ToArray(typeof(AutoUpdateDownloadDescriptor)) as AutoUpdateDownloadDescriptor[];

                // sort and select the download that contains the newest version
                recommendedUpdateDescriptor = this.SelectTheDownloadWithTheNewestUpdate(availableDownloads);

                /*
                 * Raise the AfterQueryForLatestVersion event
                 * */
                AutoUpdateManagerWithDownloadDescriptorEventArgs afterQueryArgs = new AutoUpdateManagerWithDownloadDescriptorEventArgs(this, beforeQueryArgs.ProgressViewer, recommendedUpdateDescriptor);
                this.OnAfterQueryForLatestVersion(this, afterQueryArgs);

                // if the manager could not find a suitable recomendation for downloading, we're done 
                if (recommendedUpdateDescriptor == null)
                {
                    /*
                     * Raise the NoLaterVersionAvailable event
                     * */
                    this.OnNoLaterVersionAvailable(this, new AutoUpdateManagerEventArgs(this, afterQueryArgs.ProgressViewer));
                    return;
                }
				
                // or if the product to update is newer or equal to the version of the recommended update picked for downloading
                if (_productToUpdate.Version >= recommendedUpdateDescriptor.Manifest.Product.Version)
                {
                    /*
                     * Raise the NoLaterVersionAvailable event
                     * */
                    this.OnNoLaterVersionAvailable(this, new AutoUpdateManagerEventArgs(this, afterQueryArgs.ProgressViewer));
                    return;
                }
                
                #endregion

                #region Step 2. Download

                /*
                 * Create the path including the filename where the .Update file will be downloaded to locally
                 * (ex: "C:\Program Files\Razor\1.0.0.0.update")				  
                 * */
                recommendedUpdateDescriptor.DownloadedPath = Path.Combine(_options.DownloadPath, Path.GetFileName(recommendedUpdateDescriptor.Manifest.UrlOfUpdate));

                /*
                 * Raise the BeforeDownload event
                 * */
                AutoUpdateManagerWithDownloadDescriptorCancelEventArgs beforeDownloadArgs = new AutoUpdateManagerWithDownloadDescriptorCancelEventArgs(this, afterQueryArgs.ProgressViewer, recommendedUpdateDescriptor, false);
                this.OnBeforeDownload(this, beforeDownloadArgs);
				
                // bail if the download was cancelled
                if (beforeDownloadArgs.Cancel)
                    return;				
				
                // use the downloader that found the update to download it
                // the update may be available via some proprietary communications channel (http, ftp, or Unc paths)
                downloaded = recommendedUpdateDescriptor.Downloader.Download(beforeDownloadArgs.ProgressViewer, recommendedUpdateDescriptor);

                /*
                 * Raise the AfterDownload event
                 * */
                AutoUpdateManagerWithDownloadDescriptorEventArgs afterDownloadArgs = new AutoUpdateManagerWithDownloadDescriptorEventArgs(this, beforeDownloadArgs.ProgressViewer, recommendedUpdateDescriptor);
                afterDownloadArgs.OperationStatus = downloaded;
                this.OnAfterDownload(this, afterDownloadArgs);
				
                // if the download failed bail out
                if (!downloaded)
                    return;

                #endregion

                #region Step 3. Install

                /*
                 * Raise the BeforeInstall event
                 * */
                AutoUpdateManagerWithDownloadDescriptorCancelEventArgs beforeInstallArgs = new AutoUpdateManagerWithDownloadDescriptorCancelEventArgs(this, afterDownloadArgs.ProgressViewer, recommendedUpdateDescriptor, false);
                this.OnBeforeInstall(this, beforeInstallArgs);

                // if the installation was not cancelled
                if (!beforeInstallArgs.Cancel)
                {				
                    // install the update
                    installed = this.InstallUpdate(beforeInstallArgs.ProgressViewer, recommendedUpdateDescriptor);

                    // if the update was installed, now is the time to finalize the installation
                    if (installed)
                    {
                        // all the downloader to finalize the install, there may be things to do after the installation is complete
                        // depending upon the source of the download, and again since it's plugable only the downloader will know how to deal with it
                        // and by default it will just delete the downloaded .update file
                        finalized = recommendedUpdateDescriptor.Downloader.FinalizeInstallation(beforeInstallArgs.ProgressViewer, recommendedUpdateDescriptor);
                    }    															
                }

                /*
                 * Raise the AfterInstall event
                 * */				
                AutoUpdateManagerWithDownloadDescriptorEventArgs afterInstallArgs = new AutoUpdateManagerWithDownloadDescriptorEventArgs(this, beforeInstallArgs.ProgressViewer, recommendedUpdateDescriptor);				
                afterInstallArgs.OperationStatus = installed && finalized;																	
                this.OnAfterInstall(this, afterInstallArgs);
				
                #endregion

                #region Step 4. Update Alternate Path

                /*
                 * Raise the X event
                 * */
                AutoUpdateManagerWithDownloadDescriptorCancelEventArgs beforeUpdateAltPathArgs = new AutoUpdateManagerWithDownloadDescriptorCancelEventArgs(this, afterInstallArgs.ProgressViewer, recommendedUpdateDescriptor, false);
                this.OnBeforeUpdateAlternatePath(this, beforeUpdateAltPathArgs);

                if (!beforeUpdateAltPathArgs.Cancel)
                {
                    // copy the manifest & the update there 
                    this.AdjustUrlOfUpdateInManifest(_options, recommendedUpdateDescriptor);
                    this.CreateCopyOfManifestInAlternatePath(beforeUpdateAltPathArgs.ProgressViewer, recommendedUpdateDescriptor);
                    this.CreateCopyOfUpdateInAlternatePath(beforeUpdateAltPathArgs.ProgressViewer, recommendedUpdateDescriptor);
                }
				
                // delete the downloaded .update file, don't leave it laying around
                File.Delete(recommendedUpdateDescriptor.DownloadedPath);

                AutoUpdateManagerWithDownloadDescriptorEventArgs afterUpdateAltPathArgs = new AutoUpdateManagerWithDownloadDescriptorEventArgs(this, beforeUpdateAltPathArgs.ProgressViewer, recommendedUpdateDescriptor);
                this.OnAfterUpdateAlternatePath(this, afterUpdateAltPathArgs);

                #endregion

                #region Step 5. Switch to Latest Version

                if (installed)
                {
                    /*
                     * Raise the BeforeSwitchToLatestVersion event
                     * */
                    AutoUpdateManagerWithDownloadDescriptorCancelEventArgs beforeSwitchedArgs = new AutoUpdateManagerWithDownloadDescriptorCancelEventArgs(this, afterUpdateAltPathArgs.ProgressViewer, recommendedUpdateDescriptor, false);
                    this.OnBeforeSwitchToLatestVersion(this, beforeSwitchedArgs);

                    // if switching to the latest version was not cancelled
                    if (!beforeSwitchedArgs.Cancel)
                    {
                        /*
                        * Raise the SwitchToLatestVersion event
                        * */
                        AutoUpdateManagerWithDownloadDescriptorEventArgs switchToArgs = new AutoUpdateManagerWithDownloadDescriptorEventArgs(this, beforeSwitchedArgs.ProgressViewer, recommendedUpdateDescriptor);
                        this.OnSwitchToLatestVersion(this, switchToArgs);

                        // the rest should be history because the AutoUpdateSnapIn should catch that event and switch to the latest version using the bootstrap
                    }
                }

                #endregion
            }
            catch(ThreadAbortException)
            {
                Debug.WriteLine("The AutoUpdateManager has encountered a ThreadAbortException.\n\tThe auto-update thread has been aborted.", MY_TRACE_CATEGORY);

                try
                {
                    // delete the downloaded .update file, don't leave it laying around
                    File.Delete(recommendedUpdateDescriptor.DownloadedPath);
                }
                catch(Exception)
                {

                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
                this.OnException(this, new AutoUpdateExceptionEventArgs(ex));				
            }
        }

        /// <summary>
        /// Occurs when the thread is finished
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnThreadFinished(object sender, BackgroundThreadEventArgs e)
        {
            // signal that the process has ended
            this.OnAutoUpdateProcessEnded(this, new AutoUpdateManagerEventArgs(this, null));
        }		
		
        #endregion

        #region My Event Raising Virtual Methods
		
        /// <summary>
        /// Raises the AutoUpdateProcessStarted event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnAutoUpdateProcessStarted(object sender, AutoUpdateManagerEventArgs e)
        {
            try
            {
                Debug.WriteLine(string.Format("Starting auto-update process at '{0}'.", DateTime.Now.ToString()), MY_TRACE_CATEGORY);

                if (this.AutoUpdateProcessStarted != null)
                    this.AutoUpdateProcessStarted(sender, e);
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        /// <summary>
        /// Raises the BeforeQueryForLatestVersion event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnBeforeQueryForLatestVersion(object sender, AutoUpdateManagerCancelEventArgs e)
        {
            try
            {
                if (this.BeforeQueryForLatestVersion != null)
                    this.BeforeQueryForLatestVersion(sender, e);
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        /// <summary>
        /// Raises the AfterQueryForLatestVersion event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnAfterQueryForLatestVersion(object sender, AutoUpdateManagerWithDownloadDescriptorEventArgs e)
        {
            try
            {
                if (this.AfterQueryForLatestVersion != null)
                    this.AfterQueryForLatestVersion(sender, e);
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        /// <summary>
        /// Raises the NoLaterVersionAvailable event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnNoLaterVersionAvailable(object sender, AutoUpdateManagerEventArgs e)
        {
            try
            {
                if (this.NoLaterVersionAvailable != null)
                    this.NoLaterVersionAvailable(sender, e);
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        /// <summary>
        /// Raises the BeforeDownload event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnBeforeDownload(object sender, AutoUpdateManagerWithDownloadDescriptorCancelEventArgs e)
        {
            try
            {
                if (this.BeforeDownload != null)
                    this.BeforeDownload(sender, e);

                // cancel it if it's not supposed to download automatically
                if (!e.OverrideOptions && !e.Cancel)
                    if (!_options.AutomaticallyDownloadUpdates)
                        e.Cancel = true;
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        /// <summary>
        /// Raises the AfterDownload event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnAfterDownload(object sender, AutoUpdateManagerWithDownloadDescriptorEventArgs e)
        {
            try
            {
                if (this.AfterDownload != null)
                    this.AfterDownload(sender, e);
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        /// <summary>
        /// Raises the BeforeInstall event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnBeforeInstall(object sender, AutoUpdateManagerWithDownloadDescriptorCancelEventArgs e)
        {
            try
            {				
                if (this.BeforeInstall != null)
                    this.BeforeInstall(sender, e);

                // cancel if it's not supposed to install automatically
                if (!e.OverrideOptions && !e.Cancel)
                    if (!_options.AutomaticallyInstallUpdates)
                        e.Cancel = true;
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        /// <summary>
        /// Raises the AfterInstall event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnAfterInstall(object sender, AutoUpdateManagerWithDownloadDescriptorEventArgs e)
        {
            try
            {
                if (this.AfterInstall != null)
                    this.AfterInstall(sender, e);
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        /// <summary>
        /// Raises the BeforeUpdateAlternatePath event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnBeforeUpdateAlternatePath(object sender, AutoUpdateManagerWithDownloadDescriptorCancelEventArgs e)
        {
            try
            {				
                if (this.BeforeUpdateAlternatePath != null)
                    this.BeforeUpdateAlternatePath(sender, e);

                // cancel if it's not supposed to update the alternate path automatically
                if (!e.OverrideOptions && !e.Cancel)
                    if (!_options.AutomaticallyUpdateAlternatePath)
                        e.Cancel = true;
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        /// <summary>
        /// Raises the AfterInstall event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnAfterUpdateAlternatePath(object sender, AutoUpdateManagerWithDownloadDescriptorEventArgs e)
        {
            try
            {
                if (this.AfterUpdateAlternatePath != null)
                    this.AfterUpdateAlternatePath(sender, e);
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        /// <summary>
        /// Raises the BeforeSwitchToLatestVersion event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnBeforeSwitchToLatestVersion(object sender, AutoUpdateManagerWithDownloadDescriptorCancelEventArgs e)
        {
            try
            {				
                if (this.BeforeSwitchToLatestVersion != null)
                    this.BeforeSwitchToLatestVersion(sender, e);

                // cancel if it's not supposed to switch to the latest version automatically
                if (!e.OverrideOptions && !e.Cancel)
                    if (!_options.AutomaticallySwitchToNewVersion)
                        e.Cancel = true;
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        /// <summary>
        /// Raises the SwitchToLatestVersion event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnSwitchToLatestVersion(object sender, AutoUpdateManagerWithDownloadDescriptorEventArgs e)
        {
            try
            {
                if (this.SwitchToLatestVersion != null)
                    this.SwitchToLatestVersion(sender, e);
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        /// <summary>
        /// Raises the Exception event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnException(object sender, AutoUpdateExceptionEventArgs e)
        {
            try
            {
                if (this.Exception != null)
                    this.Exception(sender, e);
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        /// <summary>
        /// Raises the AutoUpdateProcessEnded event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnAutoUpdateProcessEnded(object sender, AutoUpdateManagerEventArgs e)
        {
            try
            {
                Debug.WriteLine(string.Format("Ending auto-update process at '{0}'.", DateTime.Now.ToString()), MY_TRACE_CATEGORY);

                if (this.AutoUpdateProcessEnded != null)
                    this.AutoUpdateProcessEnded(sender, e);
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        #endregion				
    }	


    #region AutoUpdateManagerEventArgs

    /// <summary>
    /// 
    /// </summary>
    public class AutoUpdateManagerEventArgs : EventArgs
    {
        protected AutoUpdateManager _manager;
        protected IProgressViewer _progressViewer;			

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="progressViewer"></param>
        public AutoUpdateManagerEventArgs(AutoUpdateManager manager, IProgressViewer progressViewer) : base()
        {
            _manager = manager;
            _progressViewer = progressViewer;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual AutoUpdateManager Manager
        {
            get
            {
                return _manager;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual IProgressViewer ProgressViewer
        {
            get
            {
                return _progressViewer;
            }
            set
            {
                _progressViewer = value;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public delegate void AutoUpdateManagerEventHandler(object sender, AutoUpdateManagerEventArgs e);

    #endregion

    #region AutoUpdateManagerCancelEventArgs

    /// <summary>
    /// 
    /// </summary>
    public class AutoUpdateManagerCancelEventArgs : AutoUpdateManagerEventArgs
    {
        protected bool _cancel;
        protected bool _overrideOptions;
		
        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="progressViewer"></param>
        /// <param name="cancel"></param>
        public AutoUpdateManagerCancelEventArgs(AutoUpdateManager manager, IProgressViewer progressViewer, bool cancel) : base(manager, progressViewer)
        {
            _cancel = cancel;
        }

        /// <summary>
        /// Gets or sets a flag that determines whether this event should be cancelled
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

        /// <summary>
        /// Gets or sets a flag that determines if the cancel flag will override options in use by the AutoUpdateManager when the event is fired
        /// </summary>
        public bool OverrideOptions
        {
            get
            {
                return _overrideOptions;
            }
            set
            {
                _overrideOptions = value;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public delegate void AutoUpdateManagerCancelEventHandler(object sender, AutoUpdateManagerCancelEventArgs e);

    #endregion

    #region AutoUpdateManagerWithDownloadDescriptorEventArgs

    /// <summary>
    /// 
    /// </summary>
    public class AutoUpdateManagerWithDownloadDescriptorEventArgs : AutoUpdateManagerEventArgs
    {
        protected AutoUpdateDownloadDescriptor _downloadDescriptor;
        protected bool _operationStatus;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="progressViewer"></param>
        /// <param name="downloadDescriptor"></param>
        public AutoUpdateManagerWithDownloadDescriptorEventArgs(AutoUpdateManager manager, IProgressViewer progressViewer, AutoUpdateDownloadDescriptor downloadDescriptor) : base(manager, progressViewer)
        {
            _downloadDescriptor = downloadDescriptor;
        }

        /// <summary>
        /// 
        /// </summary>
        public AutoUpdateDownloadDescriptor DownloadDescriptor
        {
            get
            {
                return _downloadDescriptor;
            }
        }

        public bool OperationStatus
        {
            get
            {
                return _operationStatus;
            }
            set
            {
                _operationStatus = value;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public delegate void AutoUpdateManagerWithDownloadDescriptorEventHandler(object sender, AutoUpdateManagerWithDownloadDescriptorEventArgs e);

    #endregion

    #region AutoUpdateManagerWithDownloadDescriptorCancelEventArgs

    /// <summary>
    /// 
    /// </summary>
    public class AutoUpdateManagerWithDownloadDescriptorCancelEventArgs : AutoUpdateManagerWithDownloadDescriptorEventArgs
    {
        protected bool _cancel;
        protected bool _overrideOptions;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="progressViewer"></param>
        /// <param name="downloadDescriptor"></param>
        /// <param name="cancel"></param>
        public AutoUpdateManagerWithDownloadDescriptorCancelEventArgs(AutoUpdateManager manager, IProgressViewer progressViewer, AutoUpdateDownloadDescriptor downloadDescriptor, bool cancel) : base(manager, progressViewer, downloadDescriptor)
        {
            _cancel = cancel;
        }

        /// <summary>
        /// 
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

        /// <summary>
        /// Gets or sets a flag that determines if the cancel flag will override options in use by the AutoUpdateManager when the event is fired
        /// </summary>
        public bool OverrideOptions
        {
            get
            {
                return _overrideOptions;
            }
            set
            {
                _overrideOptions = value;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public delegate void AutoUpdateManagerWithDownloadDescriptorCancelEventHandler(object sender, AutoUpdateManagerWithDownloadDescriptorCancelEventArgs e);

    #endregion
}
