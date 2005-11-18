using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;

namespace TestTagger
{
    /// <summary>
    /// Delegate used by the events StdOutReceived and
    /// StdErrReceived...
    /// </summary>
    public delegate void DataReceivedHandler(object sender,
        DataReceivedEventArgs e);

    /// <summary>
    /// Event Args for above delegate
    /// </summary>
    public class DataReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// The text that was received
        /// </summary>
        public string Text;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="text">The text that was received for this event to be triggered.</param>
        public DataReceivedEventArgs(string text)
        {
            Text = text;
        }
    }

    /// <summary>
    /// This class can launch a process (like a bat file, perl
    /// script, etc) and return all of the StdOut and StdErr
    /// to GUI app for display in textboxes, etc.
    /// </summary>
    /// <remarks>
    /// This class (c) 2003 Michael Mayer
    /// Use it as you like (public domain licensing).
    /// Please post any bugs / fixes to the page where
    /// you downloaded this code.
    /// </remarks>
	public class ProcessCaller : AsyncOperation 
	{

        /// <summary>
        /// The command to run (should be made into a property)
        /// </summary>
        public string FileName;
        /// <summary>
        /// The Arguments for the cmd (should be made into a property)
        /// </summary>
        public string Arguments;

        /// <summary>
        /// The WorkingDirectory (should be made into a property)
        /// </summary>
        public string WorkingDirectory;

        /// <summary>
        /// Fired for every line of stdOut received.
        /// </summary>
        public event DataReceivedHandler StdOutReceived;

        /// <summary>
        /// Fired for every line of stdErr received.
        /// </summary>
        public event DataReceivedHandler StdErrReceived;

        /// <summary>
        /// Amount of time to sleep on threads while waiting
        /// for the process to finish.
        /// </summary>
        public int SleepTime = 500;

        /// <summary>
        /// The process used to run your task
        /// </summary>
        private Process process;

        /// <summary>
        /// Initialises a ProcessCaller with an association to the
        /// supplied ISynchronizeInvoke.  All events raised from this
        /// object will be delivered via this target.  (This might be a
        /// Control object, so events would be delivered to that Control's
        /// UI thread.)
        /// </summary>
        /// <param name="isi">An object implementing the
        /// ISynchronizeInvoke interface.  All events will be delivered
        /// through this target, ensuring that they are delivered to the
        /// correct thread.</param>
        public ProcessCaller(ISynchronizeInvoke isi)
            : base(isi)
        {
        }

// This constructor only works with changes to AsyncOperation...
//        /// <summary>
//        /// Initialises a ProcessCaller without an association to an
//        /// ISynchronizeInvoke.  All events raised from this object
//        /// will be delievered via the worker thread.
//        /// </summary>
//        public ProcessCaller()
//        {
//        }

        /// <summary>
        /// Launch a process, but do not return until the process has exited.
        /// That way we can kill the process if a cancel is requested.
        /// </summary>
        protected override void DoWork()
        {
            StartProcess();

            // Wait for the process to end, or cancel it
            while (! process.HasExited)
            {
                Thread.Sleep(SleepTime); // sleep
                if (CancelRequested)
                {
                    // Not a very nice way to end a process,
                    // but effective.
                    process.Kill();
                    AcknowledgeCancel();
                }
            }
        }

        /// <summary>
        /// This method is generally called by DoWork()
        /// which is called by the base classs Start()
        /// </summary>
        protected virtual void StartProcess()
        {
            // Start a new process for the cmd
            process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = FileName;
            process.StartInfo.Arguments = Arguments;
            process.StartInfo.WorkingDirectory = WorkingDirectory;
            process.Start();

            
            // Invoke stdOut and stdErr readers - each
            // has its own thread to guarantee that they aren't
            // blocked by, or cause a block to, the actual
            // process running (or the gui).
            new MethodInvoker(ReadStdOut).BeginInvoke(null, null);
            new MethodInvoker(ReadStdErr).BeginInvoke(null, null);

        }

        /// <summary>
        /// Handles reading of stdout and firing an event for
        /// every line read
        /// </summary>
        protected virtual void ReadStdOut()
        {
            string str;
            while ((str = process.StandardOutput.ReadLine()) != null)
            {
                FireAsync(StdOutReceived, this, new DataReceivedEventArgs(str));
            }
        }

        /// <summary>
        /// Handles reading of stdErr
        /// </summary>
        protected virtual void ReadStdErr()
        {
            string str;
            while ((str = process.StandardError.ReadLine()) != null)
            {
                FireAsync(StdErrReceived, this, new DataReceivedEventArgs(str));
            }
        }

	}
}
