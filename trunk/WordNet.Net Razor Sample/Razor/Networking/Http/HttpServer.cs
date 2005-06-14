using System;
using System.Diagnostics;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Razor.MultiThreading;

namespace Razor.Networking.Http
{
	/// <summary>
	/// Provides a multithreaded HTTP/1.1 based server implementation
	/// </summary>
	public class HttpServer : IDisposable
	{     
		protected bool _disposed;
		protected bool _started;
		protected bool _verbose;
		protected ManualResetEvent _stopEvent;
		protected ManualResetEvent _listenStopEvent;
		protected HttpConnectionList _connections;
	    protected HttpRequestDispatcher _dispatcher;
		protected BackgroundThread _thread;          
		protected Socket _listeningSocket;

		#region Operating System Major Versions

		private enum OperatingSystemMajorVersions
		{
			Windows_95 = 4, 
			Windows_98 = 4,
			Windows_Me = 4, 
			Windows_NT_3 = 3, 
			Windows_NT_4 = 4, 
			Windows_2000 = 5, 
			Windows_XP = 5, 
			Windows_Server_2003 = 5
		}

		#endregion

		/// <summary>
		/// Occurs when the server encounters an unexpected and unhandled exception
		/// </summary>
		public event ExceptionEventHandler Exception;

		/// <summary>
		/// 
		/// </summary>
		protected const string MY_TRACE_CATEGORY = @"'HttpServer'";

		/// <summary>
		/// Initializes a new instance of the HttpServer class
		/// </summary>
		/// <param name="verbose">A flag that indicates the verbosity level the server will use when logging</param>
		public HttpServer(bool verbose)
		{
			_verbose = verbose;
			_connections = new HttpConnectionList();
			_dispatcher = new HttpRequestDispatcher(this.CanOSSupportAspNet);
		}
            
		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{              
					this.Stop(true /* end the current sessions */);       
				}
				_disposed = true;
			}
		}

		#endregion

		/// <summary>
		/// Returns a flag that indicates whether the version of windows we are running on can support ASP.NET hosting
		/// </summary>
		public bool CanOSSupportAspNet
		{
			get
			{
				// check the os version
				Version v = System.Environment.OSVersion.Version;	

				// if it's 9x or NT, there is no support for asp.net
				if (v.Major == (int)OperatingSystemMajorVersions.Windows_95		||
					v.Major == (int)OperatingSystemMajorVersions.Windows_98		||
					v.Major == (int)OperatingSystemMajorVersions.Windows_Me		||
					v.Major == (int)OperatingSystemMajorVersions.Windows_NT_3	||
					v.Major == (int)OperatingSystemMajorVersions.Windows_NT_4)
				{
					return false;
				}
				else
				{
					// but if it's 2k or XP or better, then we're all good to go
					return true;
				}
			}
		}

		/// <summary>
		/// Starts the server (Incoming connections will be accepted while the server is started)
		/// </summary>
		/// <param name="ep"></param>
		/// <returns></returns>
		public bool Start(IPEndPoint ep)
		{
			try
			{
				_thread = new BackgroundThread();
				_thread.AllowThreadAbortException = true;
				_thread.Run += new BackgroundThreadStartEventHandler(OnThreadRun);
				_thread.Start(true, new object[] {ep});

				return true;
			}
			catch(Exception ex)
			{
				this.OnException(this, new ExceptionEventArgs(ex));
			}
			return false;
		}

		/// <summary>
		/// Stops the server (No incoming connections will be accepted while the server is stopped)
		/// </summary>
		/// <returns></returns>
		public bool Stop(bool endCurrentSessions)
		{
			try
			{                       
				if (endCurrentSessions)
					this.EndCurrentSessions();

				if (_thread != null)
				{
					_thread.Dispose();
					_thread = null;
				}

				if (_listeningSocket != null)
				{
					try 
					{
						// shutdown and close the socket
						//             _socket.Shutdown(SocketShutdown.Both);
						_listeningSocket.Close();
					}
					catch(SocketException ex) 
					{
						Debug.WriteLineIf(_verbose, string.Format("An exception was encountered while attempting to shutdown & close the server's listening socket.\n\t{0}", ex.ToString()), MY_TRACE_CATEGORY);
					}
					_listeningSocket = null;
				}
            
				_dispatcher = null;

				return true;
			}
			catch(Exception ex)
			{
				this.OnException(this, new ExceptionEventArgs(ex));
			}        
			return false;
		}
            
		/// <summary>
		/// Runs the background thread that listens for incoming connections
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnThreadRun(object sender, BackgroundThreadStartEventArgs e)
		{
			try
			{
				IPEndPoint ep = (IPEndPoint)e.Args[0];
            
				Debug.Assert(ep != null);
				Trace.WriteLineIf(_verbose, string.Format("Binding to the end point '{0}'.", ep.ToString()), MY_TRACE_CATEGORY);
                  
				_listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				_listeningSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
				_listeningSocket.Bind(ep);
            
				Trace.WriteLineIf(_verbose, string.Format("Listening for inbound connections on '{0}'.", ep.ToString()), MY_TRACE_CATEGORY);

				while(true)
				{           
					_listeningSocket.Listen(100); // pending connections queue length

					// accept the connection
					Socket socket = _listeningSocket.Accept();
      
					Trace.WriteLineIf(_verbose, string.Format("Accepted an inbound connection from '{0}'.", socket.RemoteEndPoint.ToString()), MY_TRACE_CATEGORY);

					// create a new connection for the connection
					HttpConnection connection = new HttpConnection(socket, _verbose);
					connection.RequestDispatcher = _dispatcher;
					connection.Exception += new ExceptionEventHandler(this.OnConnectionException);
					connection.Opened += new HttpConnectionEventHandler(this.OnConnectionOpened);
					connection.Closed += new HttpConnectionEventHandler(this.OnConnectionClosed); 
					connection.IsServerSideConnection = true;
					connection.BeginSession(true);                                       
				}  
			}
			catch(ThreadAbortException)
			{
			}
			catch(Exception ex)
			{
				this.OnException(this, new ExceptionEventArgs(ex));
			}
		}
            
		/// <summary>
		/// Ends all of the current sessions that are alive and removes them from the list of server sessions
		/// </summary>
		protected virtual void EndCurrentSessions()
		{        
			try
			{           
				lock(_connections.InnerList.SyncRoot)
				{
					if (_connections.Count > 0)
					{                                                                       
						// remove all sessions that are not alive
						for(int i = 0; i < _connections.Count; i++)
						{
							// if it's alive, kill it, which will raise an event an remove itself
							if (_connections[i].IsAlive)
							{
								_connections[i].EndSession();
								i--;
								continue;
							}

							// if it's not alive, just remove it
							if (!_connections[i].IsAlive)
							{
								_connections.RemoveAt(i);
								i--;
								continue;
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				this.OnException(this, new ExceptionEventArgs(ex));
			}
		}
      
		#region My Public Properties

		/// <summary>
		/// Gets or sets a flag the determines if the server is running in verbose mode 
		/// </summary>
		public bool Verbose
		{
			get
			{
				return _verbose;
			}
			set
			{
				_verbose = value;
			}
		}

		/// <summary>
		/// Returns the request dispatcher that routes requests between handlers and connections
		/// </summary>
		public HttpRequestDispatcher RequestDispatcher
		{
			get
			{
				return _dispatcher;
			}
		}
      
		#endregion

		#region My Event Raising Methods

		/// <summary>
		/// Raises the Exception event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected internal void OnException(object sender, ExceptionEventArgs e)
		{

			// trace the exception if the server is in verbose mode
			Trace.WriteLineIf(_verbose, string.Format("Encountered the following exception.\n\t{0}", e.Exception.ToString()), MY_TRACE_CATEGORY);

			try
			{
				if (this.Exception == null)
					return;

				Delegate[] delegates = this.Exception.GetInvocationList();
				if (delegates != null)
				{
					foreach(Delegate d in delegates)
					{
						try
						{
							ExceptionEventHandler handler = (ExceptionEventHandler)d;
							handler(sender, e);
						}
						catch(Exception ex)
						{
							Trace.WriteLine(ex);
						}
					}
				}
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex); 
			}
		}

		/// <summary>
		/// Handles any exceptions thrown by active sessions 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnConnectionException(object sender, ExceptionEventArgs e)
		{
			this.OnException(sender, e);
		}

		/// <summary>
		/// Handles any connection that connects to a remote end point
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnConnectionOpened(object sender, HttpConnectionEventArgs e)
		{

			// trace the connection id and the command it received
			Trace.WriteLineIf(_verbose, string.Format("Connected connection '{0}' from '{1}'.", e.Connection.Id, e.Connection.Socket.RemoteEndPoint), MY_TRACE_CATEGORY);     

			try
			{
				lock(_connections.InnerList.SyncRoot)
				{
					// track the connection
					_connections.Add(e.Connection);
				}
			}
			catch(Exception ex)
			{
				this.OnException(this, new ExceptionEventArgs(ex));
			}
		}

		/// <summary>
		/// Handles any connection that disconnects from a remote end point
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnConnectionClosed(object sender, HttpConnectionEventArgs e)
		{

			// trace the connection id and the command it received
			Trace.WriteLineIf(_verbose, string.Format("Disconnected connection '{0}'.", e.Connection.Id), MY_TRACE_CATEGORY);

			try
			{
				lock(_connections.InnerList.SyncRoot)
				{
					// find the connection in our list that has disconnected
					HttpConnection connection = _connections[e.Connection.Id];
					if (connection != null)
					{
						// dispose of it
						connection.Dispose();

						// remove it from our connection list
						_connections.Remove(connection);
					}
				}
			}
			catch(Exception ex)
			{
				this.OnException(this, new ExceptionEventArgs(ex));
			}
		}  

		#endregion     
	}  
}
