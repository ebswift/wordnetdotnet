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
using System.IO;
using System.Reflection;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using Razor.Configuration;
using Razor;
using Razor.Searching;

namespace Razor
{
	/// <summary>
	/// Summary description for VersioningBootStrap.
	/// </summary>
	public class VersioningBootStrap : IRunnable
	{
		#region IRunnable Members

		/// <summary>
		/// Unused, we need the executing assembly to be passed down to us
		/// </summary>
		/// <param name="args"></param>
		public void Run(string[] args)
		{
			throw new Exception("Do not use this method, use the overloaded method Run(args, executable) instead.");
		}

		public void Run(string[] args, System.Reflection.Assembly executable)
		{		
			bool tracedExceptionThrown = false;				
			try
			{	
				using (SplashWindowThread splashThread = new SplashWindowThread(executable, false))
				{
					splashThread.ShowAsynchronously();			
					splashThread.Window.SetMarqueeMoving(true, true);
					
					ProgressViewer.SetExtendedDescription(splashThread.Window, "Bootstrap: Parsing command line...");

					// create a new command line parsing engine
					CommandLineParsingEngine pe = new CommandLineParsingEngine(args);
					
					// determine if we are going to keep the old versions
					bool keepOld = pe.ToBoolean("keepold");

					// the process id of an app
					int pid = pe.ToInt32("pid");

					// whether we should wait on the specified pid to die before launching new version
					bool wait = pe.ToBoolean("wait");

					ProgressViewer.SetExtendedDescription(splashThread.Window, "Bootstrap: Searching for runnable version...");

					// create a search for all of the subdirectories
					Search search = new Search("Versions", Application.StartupPath, "*", false, false);

					// find all of the directories
					DirectoryInfo[] directories = search.GetDirectories();

					// create versioned files around each directory that can be parsed to a version
					VersionedDirectory[] versionedDirectories = this.CreateVersionedFiles(directories);

					// if we have been instructed to wait on the process specified by pid to die, do it now
					if (wait && pid != 0)
					{
						try
						{
							// snag it and wait on it to exit
							Process p = Process.GetProcessById(pid);
							if (p != null)
							{
								ProgressViewer.SetExtendedDescription(splashThread.Window, "Bootstrap: Closing previous instance...");
								p.WaitForExit();
							}
						}
						catch(System.Exception systemException)
						{
							System.Diagnostics.Trace.WriteLine(systemException);
						}
					}
				
					ProgressViewer.SetExtendedDescription(splashThread.Window, "Bootstrap: Selecting latest runnable version...");

					// try and start the newest version
					VersionedDirectory versionStarted;
					bool startedVersion = this.StartNewestVersion(executable, versionedDirectories, out versionStarted, splashThread.Window);
					// this will fall back upon older versions until it runs out of versions or one of the versions starts
					if (!startedVersion)
					{
						string exeName = string.Format("{0}.exe", executable.GetName().Name);
						ExceptionEngine.DisplayException(null, "BootStrap failed for " + exeName, MessageBoxIcon.Stop,  MessageBoxButtons.OK, null, 
							"No suitable executable was found or able to be started.");
					}

					// if we're not keeping the old versions
					if (!keepOld)
					{
						// delete the older versions 
						if (!this.DeleteOlderVersions(versionedDirectories, versionStarted, splashThread.Window))
						{
							// um, who cares if we can't delete the older versions
							// also we need to see about security rights to the directories
						}
					}
					
					// if we started a version
					if (startedVersion)
						// notify that we are transferring control now to it...
						ProgressViewer.SetExtendedDescription(splashThread.Window, "Bootstrap: Transferring control to version " + versionStarted.Version.ToString() + "...");					
				}
			}
			catch(System.Exception systemException)
			{
				tracedExceptionThrown = true;
				System.Diagnostics.Trace.WriteLine(systemException);
				System.Windows.Forms.MessageBox.Show(null, systemException.ToString(), "Application Exiting");
				Application.ExitThread();
			}
			finally
			{
				System.Diagnostics.Trace.WriteLine("'" + Application.ProductName + (tracedExceptionThrown ? "' has terminated because of an exception." : "' has exited gracefully."));
			}
		}

		#endregion

		/// <summary>
		/// Parses the names of the directories and creates a VersionedDirectory for each directory whose name could be parsed into a Version
		/// </summary>
		/// <param name="directories"></param>
		/// <returns></returns>
		private VersionedDirectory[] CreateVersionedFiles(DirectoryInfo[] directories)
		{
			try
			{
				ArrayList array = new ArrayList();
				foreach(DirectoryInfo directory in directories)
				{
					try
					{
						Version version = new Version(directory.Name);
						VersionedDirectory versionedFile = new VersionedDirectory(version, directory);
						array.Add(versionedFile);
					}
					catch
					{

					}
				}
				
				VersionedDirectory[] versionedFiles = array.ToArray(typeof(VersionedDirectory)) as VersionedDirectory[];
				versionedFiles = VersionedDirectory.Sort(versionedFiles);
				return versionedFiles;
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return null;
		}

		/// <summary>
		/// Attempts to start the newest version of the executable named the same thing as the executing assembly
		/// </summary>
		/// <param name="versionedDirectories"></param>
		/// <param name="versionStarted"></param>
		/// <returns></returns>
		private bool StartNewestVersion(
			Assembly executable, 
			VersionedDirectory[] versionedDirectories, 
			out VersionedDirectory versionStarted,
			IProgressViewer progressViewer)
		{
			versionStarted = null;

			try
			{
				// get the name of this assembly which will be what we look for to start
				string assemblyName = string.Format("{0}.exe", executable.GetName().Name);
				
				// will start with the newest version because they are sorted
				foreach(VersionedDirectory versionToStart in versionedDirectories)
				{					
					// format the path to the version we are going to attempt to start
					string path = Path.Combine(versionToStart.Directory.FullName, assemblyName);

					bool started = false;
					
					// if the file exists
					if (File.Exists(path))
					{
						ProgressViewer.SetExtendedDescription(progressViewer, "Bootstrap: Starting version " + versionToStart.Version.ToString() + "...");

						Process p = new Process();
						p.StartInfo.FileName = path;
						p.StartInfo.Arguments = System.Environment.CommandLine;
						p.StartInfo.WorkingDirectory = versionToStart.Directory.FullName;
						p.StartInfo.WindowStyle = Process.GetCurrentProcess().StartInfo.WindowStyle;
						
						// try to start this version
						started = p.Start();
					}

					// if a version was started
					if (started)
					{						
						// keep track of which version we started
						versionStarted = versionToStart;
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

		private bool DeleteOlderVersions(
			VersionedDirectory[] versionedDirectories,
			VersionedDirectory versionStarted,
			IProgressViewer progressViewer)
		{
			try
			{
				// grab the newest version
				VersionedDirectory newestVersion = versionedDirectories[0];

				// loop thru and delete the oldest versions not in use
				foreach(VersionedDirectory version in versionedDirectories)
				{
					// keep the newest version and the one that started
					if (version != newestVersion && version != versionStarted)
					{
						try
						{
							ProgressViewer.SetExtendedDescription(progressViewer, "Bootstrap: Removing older version " + version.Version.ToString() + "...");

							// recursively delete this version directory
							Directory.Delete(version.Directory.FullName, true);
						}
						catch(System.Exception systemException)
						{
							System.Diagnostics.Trace.WriteLine(systemException);
						}
					}
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return false;
		}
	}
}
