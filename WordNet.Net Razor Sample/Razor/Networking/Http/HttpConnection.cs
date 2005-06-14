using System;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Razor.MultiThreading;

namespace Razor.Networking.Http
{
    /// <summary>
    /// Provides a multithreaded HTTP/1.1 based client connection to a HTTP/1.1 compatible server.
    /// </summary>
    public class HttpConnection : MarshalByRefObject, IDisposable 
    {				
        #region Vars

        // vars
        protected bool						_disposed;		
        protected bool						_verbose;
        protected bool						_disconnected;
        protected bool						_closeConnectionOnReceivingThreadAbort;
        protected bool						_isServerSideConnection;
        protected Socket					_socket;
        protected Guid						_id;
        protected HttpRequestDispatcher		_dispatcher;
        protected HttpMessageWriter			_messageWriter;
        protected HttpMessageReader			_messageReader;
        protected ManualResetEvent			_stopEvent;
        protected ManualResetEvent			_doneReceiving;		
        protected BackgroundThread			_thread;
        protected Exception					_lastException;
		
        // constants
        protected const string MY_TRACE_CATEGORY = "HttpConnection";

        #endregion

        #region My Public Events

        /// <summary>
        /// Fires when the connection opens a socket (TCP) connection to a remote peer
        /// </summary>
        public event HttpConnectionEventHandler Opened;

        /// <summary>
        /// Fires when the connection closes a socket (TCP) connection to a remote peer
        /// </summary>
        public event HttpConnectionEventHandler Closed;

        /// <summary>
        /// Fires when the connection encounters an exception thrown by the underlying socket (TCP)
        /// </summary>
        public event ExceptionEventHandler Exception;	
		
        #endregion

        /// <summary>
        /// Initializes a new instance of the HttpConnection class (Servers use this constructor)
        /// </summary>
        /// <param name="socket">The socket to be used for the lifetime of the connection</param>
        /// <param name="verbose">A flag that specifies if the connection is verbose</param>
        public HttpConnection(Socket socket, bool verbose) 
        {
            if (socket == null)
                throw new ArgumentNullException("socket", "A connection cannot be created using a null socket.");

            _verbose = verbose;
            _socket = socket;
            _id = Guid.NewGuid();				
            _dispatcher = new HttpRequestDispatcher(false);
            _messageWriter = new HttpMessageWriter();	
            _messageReader = new HttpMessageReader();
        }
		
        /// <summary>
        /// Initializes a new instance of the HttpConnection class (Clients use this contructor)
        /// </summary>
        /// <param name="verbose">A flag that specifies if the connection is verbose</param>
        public HttpConnection(bool verbose) 
        {
            _verbose = verbose;
            _id = Guid.NewGuid();	
            _dispatcher = new HttpRequestDispatcher(false);
            _messageWriter = new HttpMessageWriter();
            _messageReader = new HttpMessageReader();
        }

        /// <summary>
        /// Initializes a new instance of the HttpConnection class (Clients use this contructor)
        /// </summary>
        /// <param name="ep">The remote peer to connect to</param>
        /// <param name="beginReceiving">A flag that specifies whether the connection should automatically begin receiving</param>
        /// <param name="verbose">A flag that specifies if the connection is verbose</param>
        /// <param name="sendTimeout">A send timeout</param>
        /// <param name="recvTimeout">A recv timeout</param>
        public HttpConnection(IPEndPoint ep, bool beginReceiving, bool verbose, int sendTimeout, int recvTimeout)
        {
            if (ep == null)
                throw new ArgumentNullException("ep");

            _verbose = verbose;			
            _id = Guid.NewGuid();				
            _dispatcher = new HttpRequestDispatcher(false);
            _messageWriter = new HttpMessageWriter();	
            _messageReader = new HttpMessageReader();

            this.Open(ep, beginReceiving, sendTimeout, recvTimeout);
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
                    this.Close();
                }
                _disposed = true;
            }
        }

        #endregion

        #region My Overrides

        /// <summary>
        /// Make this object last forever when it is remoted
        /// </summary>
        /// <returns></returns>
        public override object InitializeLifetimeService()
        {
            return null;
        }

        #endregion

        #region My Public Methods 

        /// <summary>
        /// Opens a connection by connecting the internal socket to the remote endpoint specified
        /// </summary>
        /// <param name="ep">The remote peer to connect to</param>
        /// <param name="beginReceiving">A flag that indicates whether the connection should begin trying to receive data automatically</param>
        /// <returns></returns>
        public virtual void Open(IPEndPoint ep, bool beginReceiving) 
        {
            // create a new tcp based stream socket
            _socket = HttpUtils.CreateTcpSocket(true, HttpOptions.SendTimeout, HttpOptions.RecvTimeout);
						 
            //			Debug.WriteLineIf(_verbose, string.Format("Opening Tcp connection '{0}' to '{1}'.", _id, ep.ToString()), MY_TRACE_CATEGORY);
						 
            // connect to the remote end point
            _socket.Connect(ep);				                    
			
            // begin the connection
            this.BeginSession(beginReceiving);
        }

        /// <summary>
        /// Opens a connection by connecting the internal socket to the remote endpoint specified
        /// </summary>
        /// <param name="ep">The remote peer to connect to</param>
        /// <param name="beginReceiving">A flag that indicates whether the connection should begin trying to receive data automatically</param>
        /// <param name="sendTimeout">A send timeout</param>
        /// <param name="recvTimeout">A recv timeout</param>
        public virtual void Open(IPEndPoint ep, bool beginReceiving, int sendTimeout, int recvTimeout)
        {
            // create a new tcp based stream socket
            _socket = HttpUtils.CreateTcpSocket(true, sendTimeout, recvTimeout);
						 
            //			Debug.WriteLineIf(_verbose, string.Format("Opening Tcp connection '{0}' to '{1}'.", _id, ep.ToString()), MY_TRACE_CATEGORY);

            // connect to the remote end point
            _socket.Connect(ep);				                    
			
            // begin the connection
            this.BeginSession(beginReceiving);
        }
				
        /// <summary>
        /// Disconnects the connection socket from the remote server (Calls EndSession internally)
        /// </summary>
        public virtual void Close() 
        {			
            // disconnecting also ends the connection
            this.EndSession();
        }
		
        /// <summary>
        /// Returns the last exception that has occurred on the connection
        /// </summary>
        /// <returns></returns>
        public virtual Exception GetLastException()
        {
            if (_lastException == null)
                _lastException = new HttpErrorSuccessException();

            return _lastException;
        }

        #endregion

        #region My Protected Internal Methods

        /// <summary>
        /// Begins a Tcp connection with a remote client (Begins a background thread that will read data from the socket
        /// </summary>
        protected internal virtual void BeginSession(bool beginReceiving) 
        {	
            // reset the disconnected flag
            _disconnected = false;

            // if we are supposed to asynchronously begin receiving data
            if (beginReceiving) 
                this.BeginReceiving();
			
            // raise the connected event
            this.OnOpened(this, new HttpConnectionEventArgs(this));
        }

        /// <summary>
        /// Ends a Tcp connection with a remote client (Ensures the server socket has been shutdown properly and closed)
        /// </summary>
        protected internal virtual void EndSession() 
        {
            try 
            {
                if (_disconnected)
                    return;								
								
                // if the sesssion is still alive
                if (this.IsAlive) 
                {
                    HttpUtils.ShutdownSocket(_socket);
                    HttpUtils.CloseSocket(_socket);
                    _socket = null;					
                    if (!_disconnected)
                        _disconnected = true;										
                }
				
                this.EndReceiving(false /* disconnect will happen automatically because we closed the socket */);

                // raise the disconnected event
                this.OnClosed(this, new HttpConnectionEventArgs(this));
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
        /// Returns a flag that indicates whether the conneciton is currently receiving
        /// </summary>
        public bool IsReceiving
        {
            get
            {
                return _thread != null;
            }		
        }		

        /// <summary>
        /// Starts a background thread that will receive messages for the connection
        /// </summary>
        public virtual void BeginReceiving()
        {
            if (this.IsReceiving)
                return;

            _closeConnectionOnReceivingThreadAbort = true;
            _stopEvent = new ManualResetEvent(false);
            _thread = new BackgroundThread();
            _thread.AllowThreadAbortException = true;
            _thread.Run += new BackgroundThreadStartEventHandler(OnReadFromSocket);
            _thread.Start(true /* background thread */, new object[] {});
        }

        /// <summary>
        /// Stops the background thread that is receiving messages for the connection
        /// </summary>
        /// <param name="disconnect">A flag that indicates whether the connection should be closed</param>
        public virtual void EndReceiving(bool closeConnection)
        {
            _closeConnectionOnReceivingThreadAbort = closeConnection;

            // signal a stop, we might get lucky and ease the thread out
            if (_stopEvent != null)
                _stopEvent.Set();

            // and finally kill the receiving thread
            if (_thread != null) 
            {
                _thread.Dispose();
                _thread = null;				
            }
            _stopEvent = null;
        }

        #endregion

        /// <summary>
        /// Tries to receive data from the network on a background thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnReadFromSocket(object sender, BackgroundThreadStartEventArgs threadStartArgs) 
        {
            try 
            {	               
                // create a new message reader
                _messageReader = new HttpMessageReader();
				
                while(true) 
                {
                    try 
                    {			
                        //						if (!_isServerSideConnection)
                        //							Debug.WriteLine(string.Format("User-Agent connection '{0}' trying to read next incoming message from '{1}'", _id.ToString(), this.RemoteAddress.ToString()), MY_TRACE_CATEGORY);
                        //						else
                        //							Debug.WriteLine(string.Format("Server-Side connection '{0}' trying to read next incoming message from '{1}'", _id.ToString(), this.RemoteAddress.ToString()), MY_TRACE_CATEGORY);

                        // read a single message
                        HttpMessage message = null;
							
                        // lock the reader
                        lock(_messageReader)
                        {
                            // read a message
                            message = _messageReader.Read(_socket, _stopEvent);
                        }

                        // what type of message is this ?						
                        switch(message.Type)
                        {
                                /*
                                 * process the request
                                 * */
                            case HttpMessageTypes.HttpRequest:
                            {	
                                // create a request event args
                                HttpRequestCancelEventArgs e = new HttpRequestCancelEventArgs(new HttpRequest(message), false);					
								
                                // process the request by dispatching it and then responding if need be
                                this.OnRequestReceived(this, e);								
                                break;
                            }
                                /*
                                 * process the response
                                 * */
                            case HttpMessageTypes.HttpResponse:
                            {
                                this.OnResponseReceived(this, new HttpResponseEventArgs(new HttpResponse(message)));
                                break;
                            }
                                /*
                                 * an unknown message type
                                 * */
                            default:
                            {	
                                // hopefully this will never happen!
                                Debug.WriteLine(string.Format("A message of unknown type was received from '{0}'...\n{1}", message));	
                                break;
                            }							
                        };
                    }
                        /*
                         * how does this get generated?
                         * 
                         * 3/10/05 - Code6
                         * I'm in the middle of fixing the half-open/half-closed problem with connections being left in the FIN_WAIT_2 state
                         * this could very much be part of the problem... 
                         * */
                    catch(HttpMessageReaderAbortedException)
                    {
                        // the reader was aborted
                        // we'll just catch this exception, and the next line will test for the abort
                        // and then break out of the loop
                    }
										
                    // see if we should stop receiving
                    if (_stopEvent != null)
                        if (_stopEvent.WaitOne(1, false)) 
                        {
                            /*
                            * we have recieved a signal that we should stop receiving
                            * and disconnect the current connection
                            * */
                            if (_closeConnectionOnReceivingThreadAbort)
                                this.Close();
                            return;
                        }							
                }
												
            }
            catch(ThreadAbortException) 
            {
                /*
                 * the thread is aborting
                 * */
                if (_closeConnectionOnReceivingThreadAbort)
                    this.Close();
            }
            catch(ObjectDisposedException) 
            {
                /*
                 * this side (the local peer) of the connection has closed the socket
                 * */				
                this.Close();
            }
            catch(SocketException ex) 
            {				
                // if the connection is reset, or a blocking call was cancelled with a call to cancelblockingcall
                //				if (ex.ErrorCode == (int)SocketErrors.WSAECONNRESET ||
                //					ex.ErrorCode == (int)SocketErrors.WSAEINTR) 
                //				{
                this.Close();
                //					return;
                //				}

                // notify that this connection has encountered an exception
                this.OnException(this, new ExceptionEventArgs(ex));
            }
            catch(HttpConnectionClosedByPeerException) 
            {					
                /*
                 * the other side (the remote peer) of the connection has closed the socket
                 * */
                this.Close();			
            }
            catch(Exception ex) 
            {
                // notify that this connection has encountered an exception
                this.OnException(this, new ExceptionEventArgs(ex));
            }
            //			finally
            //			{
            //				Debug.WriteLineIf(!_isServerSideConnection, string.Format("*** exiting receiving thread loop for connection '{0}'", _id.ToString()), MY_TRACE_CATEGORY);
            //			}
        }

        /// <summary>
        /// Occurs when a request is received
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnRequestReceived(object sender, HttpRequestCancelEventArgs e)
        {	
            //			if (!_isServerSideConnection)
            //			{
            //				Debug.WriteLine(string.Format("User-Agent connection '{0}' auto-logging request '{1}'", _id.ToString(), HttpUtils.StripCRLF(e.Request.FirstLine)), MY_TRACE_CATEGORY);
            //				Debug.WriteIf(_verbose, e.Request.ToString(false));
            //			}
            //			else
            //			{
            //				Debug.WriteLine(string.Format("Server-Side connection '{0}' auto-logging request '{1}'", _id.ToString(), HttpUtils.StripCRLF(e.Request.FirstLine)), MY_TRACE_CATEGORY);
            //				Debug.WriteIf(_verbose, e.Request.ToString(false));
            //			}

            // dispatch the request out to be handled
            if (_dispatcher != null)
                _dispatcher.DispatchRequest(sender, ref e);
            else
                Debug.WriteLine(string.Format("Received a message, however there is no dispatcher available to handle the message for connection '{0}'.", _id.ToString()));

            // did someone cancel the request?
            if (e.Cancel)
                return; // do not send a response, some will have done this manually

            /*
             * IMPORTANT!!! - We are providing a mechanism to override the default HTTP/1.1 specification's behaviour.
             * The specs state explicitly under all conditions a response must be sent to a request. We have certain scenarious where
             * we only need to send a request, no further processing is required as to the result of the request by the user-agent, so
             * the 'Reponse-Needed' header can be included to prevent the 'Server' from responding to the request.
             * */ 			 
            if (e.Request.ResponseNeeded)
            {
                // if there is a response prepared let's send it now
                HttpRequest request = e.Request;
                HttpResponse response = e.Response;

                // send a response to the request
                this.SendResponseToRequest(ref request, ref response);
            }
        }
		
        /// <summary>
        /// Occurs when a response is received
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnResponseReceived(object sender, HttpResponseEventArgs e)
        {
            //			try
            //			{
            //				// trace the response
            //				if (!_isServerSideConnection)
            //				{
            //					Debug.WriteLine(string.Format("User-Agent connection '{0}' auto-logging response '{1}'", _id.ToString(), HttpUtils.StripCRLF(e.Response.FirstLine)), MY_TRACE_CATEGORY);
            //					Debug.WriteIf(_verbose, e.Response.ToString(false));
            //				}
            //				else
            //				{
            //					Debug.WriteLine(string.Format("Server-Side connection '{0}' auto-logging response '{1}'", _id.ToString(), HttpUtils.StripCRLF(e.Response.FirstLine)), MY_TRACE_CATEGORY);
            //					Debug.WriteIf(_verbose, e.Response.ToString(false));
            //				}
            //				
            //				// if the response is bad, trace the message body
            //				if (!HttpUtils.Succeeded(e.Response))
            //                    Debug.WriteLine(e.Response.GetBodyAsString(HttpUtils.Encoding));
            //			}
            //			catch(Exception ex)
            //			{
            //				Debug.WriteLine(ex);
            //			}
        }		

        /// <summary>
        /// Sends the specified request using the connection's socket
        /// </summary>
        /// <param name="request"></param>
        public virtual void SendRequest(
            HttpRequest request,			
            HttpMessageProgressEventHandler onProgress,
            object stateObject)
        {
            //			Debug.WriteLineIf(_verbose, "Sending request...", MY_TRACE_CATEGORY);			
            //			Debug.WriteIf(_verbose, request.ToString(false));

            // lock the writer
            lock(_messageWriter)
            {
                // send the message
                _messageWriter.Write(_socket, null, request, onProgress, stateObject);
            }
        }

        /// <summary>
        /// Sends the specified response using the connection's socket
        /// </summary>
        /// <param name="response"></param>
        public virtual void SendResponseToRequest(ref HttpRequest request, ref HttpResponse response)
        {
            try
            {
                // if the request has asked that the connection be kept-alive, we'll abide by the request
                if (HttpUtils.Contains(request.Connection, HttpConnections.KeepAlive))
                    // so instruct the response to notify the user-agent that the connection will be kept alive
                    response.Connection = HttpConnections.KeepAlive;

                //				Debug.WriteLineIf(_verbose, "Sending response...", MY_TRACE_CATEGORY);			
                //				Debug.WriteIf(_verbose, response.ToString(false));
				
                // lock the writer
                lock(_messageWriter)
                {
                    // send the message
                    _messageWriter.Write(_socket, null, response);
                }
				
                // next check the request to see if the connection should be closed after the response was sent
                if (HttpUtils.Contains(request.Connection, HttpConnections.Close))
                {
                    // yup, they wanted us to close the connection automatically so lets do that now
                    this.Close();
                    return;
                }
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Sends the request and waits for a response
        /// </summary>
        /// <param name="request">The request to be processed by the server and for which we want a response.</param>
        /// <returns></returns>
        public virtual HttpResponse GetResponse(
            HttpRequest request,
            HttpMessageProgressEventHandler onSendProgress,
            HttpMessageProgressEventHandler onRecvProgress,
            object stateObject)
        {
            try
            {
                // send the request
                this.SendRequest(request, onSendProgress, stateObject);			

                // lock the reader
                HttpResponse response = null;
                lock(_messageReader)
                {
                    // receive the response
                    HttpMessage message = _messageReader.Read(_socket, null, onRecvProgress, stateObject);
                    if (message.Type == HttpMessageTypes.HttpResponse)
                        response = new HttpResponse(message);
                }

                //				if (response != null)
                //				{
                //					Debug.WriteLineIf(_verbose, "Logging response...", MY_TRACE_CATEGORY);
                //					Debug.WriteIf(_verbose, response.ToString(false));
                //				}

                return response;
            }
            catch(Exception ex) 
            {
                // notify that this connection has encountered an exception				
                this.OnException(this, new ExceptionEventArgs(ex));
                this.Close();
            }
            return null;
        }

        #region My Public Properties

        /// <summary>
        /// Gets or sets the message dispatcher that will dispense incoming messages by command to subscribers
        /// </summary>
        public HttpRequestDispatcher RequestDispatcher 
        {
            get 
            {
                return _dispatcher;
            }
            set 
            {
                _dispatcher = value;
            }
        }
		
        /// <summary>
        /// Returns the socket used by this connection
        /// </summary>
        public Socket Socket 
        {
            get 
            {
                return _socket;
            }
        }

        /// <summary>
        /// Gets or sets a flag that indicates whether this connection is verbose
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
        /// Returns a flag that indicates whether this connection is a server side or user-agent side connection
        /// </summary>
        public bool IsServerSideConnection
        {
            get
            {
                return _isServerSideConnection;
            }
            set
            {
                _isServerSideConnection = value;
            }
        }

        /// <summary>
        /// Returns a flag that indicates whether the connection is alive
        /// </summary>
        public bool IsAlive 
        {
            get 
            {
                return _socket != null;
            }
        }

        /// <summary>
        /// Returns a Guid that uniquely identifies this connection
        /// </summary>
        public Guid Id 
        {
            get 
            {
                return _id;
            }
        }

        public IPAddress LocalAddress
        {
            get
            {
                IPEndPoint ep = (IPEndPoint)_socket.LocalEndPoint;
                if (ep != null)
                    return ep.Address;
                return IPAddress.Loopback;
            }
        }

        public IPAddress RemoteAddress
        {
            get
            {								
                IPEndPoint ep = (IPEndPoint)_socket.RemoteEndPoint;
                if (ep != null)
                    return ep.Address;
                return IPAddress.Loopback;
            }
        }

        public int LocalPort
        {
            get
            {
                IPEndPoint ep = (IPEndPoint)_socket.LocalEndPoint;
                if (ep != null)
                    return ep.Port;
                return 0;
            }
        }

        public int RemotePort
        {
            get
            {
                IPEndPoint ep = (IPEndPoint)_socket.RemoteEndPoint;
                if (ep != null)
                    return ep.Port;
                return 0;
            }
        }

        #endregion 
		
        #region My Event Raising Methods

        /// <summary>
        /// Raises the Exception event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected internal virtual void OnException(object sender, ExceptionEventArgs e) 
        {
            // save the exception so that we can retrieve it later if we want to
            _lastException = e.Exception;

            try
            {
                //				Debug.WriteLineIf(_verbose, string.Format("Exception encountered on Tcp connection '{0}' of '{1}'.", _id, e.Exception.ToString()), MY_TRACE_CATEGORY);

                if (this.Exception != null)
                    this.Exception(sender, e);
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex) 
            {
                Trace.WriteLine(ex, MY_TRACE_CATEGORY);	
            }
        }

        /// <summary>
        /// Raises the Opened event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnOpened(object sender, HttpConnectionEventArgs e) 
        {
            try 
            {
                if (this.Opened != null)
                    this.Opened(sender, e);
            }
            catch(ThreadAbortException)
            {
            }
            catch(Exception ex) 
            {
                Trace.WriteLine(ex, MY_TRACE_CATEGORY);
            }
        }

        /// <summary>
        /// Raises the Closed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnClosed(object sender, HttpConnectionEventArgs e) 
        {
            try 
            {
                if (this.Closed != null)
                    this.Closed(sender, e);
            }
            catch(ThreadAbortException)
            {
            }
            catch(Exception ex) 
            {
                Trace.WriteLine(ex, MY_TRACE_CATEGORY);
            }
        }

        #endregion

		
    }
}
