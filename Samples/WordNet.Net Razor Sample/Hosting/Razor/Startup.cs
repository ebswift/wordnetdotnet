using System;
using System.Windows.Forms;
using Razor.SnapIns;

namespace Razor
{	
	/// <summary>
	/// The Startup class contains a single static method called Main which should be used as the applications main entry point.
	/// </summary>
	public class Startup
	{
		/// <summary>
		/// The main application entry point, thru which command line arguments will be passed
		/// </summary>
		/// <param name="args">An array of strings representing any command line arguments that were passed at startup</param>
		[STAThread()]
		public static void Main(string[] args)
		{		
			bool tracedExceptionThrown = false;
			try
			{	
				// define the data paths
				string subPath = @"CodeReflection\Razor";
				
				// safely use a hosting engine configured with an additional common data path and additional local user data path
				using(SnapInHostingEngine host = new SnapInHostingEngine(subPath, subPath))
				{
					try
					{
						// run the hosting engine using the command line and the currently executing assembly (aka. the exe for the process)
						host.Run(args, System.Reflection.Assembly.GetExecutingAssembly());					
					}
					catch(System.Exception systemException)
					{
						// flag the fact that we are going to trace this exception and rethrow it
						tracedExceptionThrown = true;
						
						// give the loggers a chance to catch it if they have been successfully loaded before the 
						// host is disposed of and the logging sub system detached from the Trace and Debug output
						System.Diagnostics.Trace.WriteLine(systemException);
						
						// rethrow the exception so that it may be displayed for the user
						throw systemException;
					}
				}		
			}
			catch(System.Exception systemException)
			{
				// if the exception hasn't already been traced
				if (!tracedExceptionThrown)
				{
					tracedExceptionThrown = true;
					// trace it now
					System.Diagnostics.Trace.WriteLine(systemException);
				}
				
				// also, since it's more likely we'll not have a debugger attached, display the exception to the user
				string info = string.Format("The following exception was thrown by '{0}'.\n\nPlease refer to the log files for further information.\n\n{1}", Application.ProductName, systemException.ToString());

				System.Windows.Forms.MessageBox.Show(null, info, "Application Exception");

				// exit the current thread to force safe application shutdown
				Application.ExitThread();
			}
			finally
			{
				// one final trace to let everyone know we have shutdown completely
				System.Diagnostics.Trace.WriteLine("'" + Application.ProductName + "' has " + (tracedExceptionThrown ? "terminated because of an exception." : "exited gracefully."));
			}
		}
	}
}
