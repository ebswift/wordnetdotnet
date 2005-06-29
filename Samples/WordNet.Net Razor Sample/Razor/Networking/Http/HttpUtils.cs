using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Razor.Networking.Http
{
    /// <summary>
    /// Summary description for HttpUtils.
    /// </summary>
    public class HttpUtils
    {
        /// <summary>
        /// Returns the default encoding used by the Http namespace (UTF-8 Encoding)
        /// </summary>
        public static Encoding Encoding
        {
            get
            {
                return System.Text.Encoding.UTF8;
            }
        }

        /// <summary>
        /// Determines if the string is null
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullString(string value)
        {
            return (value == null);
        }

        /// <summary>
        /// Determines if the string is empty
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEmptryString(string value)
        {
            return (value == string.Empty);
        }

        /// <summary>
        /// Determines if the string contains a CR
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ContainsCR(string value)
        {
            return (value.IndexOf(HttpControlChars.CR, 0) > 0);
        }

        /// <summary>
        /// Determines if the string contains a LF
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ContainsLF(string value)
        {
            return (value.IndexOf(HttpControlChars.LF, 0) > 0);
        }	
		
        /// <summary>
        /// Determines if the source string contains the value specified
        /// </summary>
        /// <param name="source">The string to search</param>
        /// <param name="value">The value to search for</param>
        /// <returns></returns>
        public static bool Contains(string source, string value)
        {
            if (string.Compare(source, value, true) == 0)
                return true;

            int index = source.IndexOf(value);
            return (index > 0);
        }

        /// <summary>
        /// Removes any instances of the CR|LF characters
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string StripCRLF(string value)
        {
            value = value.Replace(HttpControlChars.CR, null);
            value = value.Replace(HttpControlChars.LF, null);
            return value;
        }
        
        /// <summary>
        /// Removes any leading and trailing CR|LF characters in the string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string TrimLeadingAndTrailingSpaces(string value)
        {
            return value.Trim();
        }

        /// <summary>
        /// Ensures that the string is not empty, null, and does not contain any CR|LF chars
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void ValidateToken(string name, string value)
        {
            if (HttpUtils.IsNullString(value) || HttpUtils.IsEmptryString(value))
                throw new ArgumentException(string.Format("The parameter '{0}' cannot be null or empty.", name));

            if (HttpUtils.ContainsCR(value) || HttpUtils.ContainsLF(value))
                throw new ArgumentException(string.Format("The parameter '{0}' cannot contain carrage returns '\\r' or line feed '\\n' characters.", name), name);
        }

        /// <summary>
        /// Combines the two buffers into a single large buffer
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static byte[] Combine(byte[] a, byte[] b)
        {
            byte[] buffer = new byte[a.Length + b.Length];
            Buffer.BlockCopy(a, 0, buffer, 0, a.Length);
            Buffer.BlockCopy(b, 0, buffer, a.Length, b.Length);
            return buffer;			
        }

        /// <summary>
        /// Clones a buffer into a copy of itself
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] Clone(byte[] buffer, int startIndex, int length)
        {
            byte[] tempBuffer = new byte[length];
            if (tempBuffer.Length > 0)
                Buffer.BlockCopy(buffer, startIndex, tempBuffer, 0, length);
            return tempBuffer;			
        }

        /// <summary>
        /// Determines if the request succeeded based on the status code of the response
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static bool Succeeded(HttpResponse response)
        {
            if (response == null)
                return false;

            if (response.Status.Code == new OkStatus().Code)
                return true;

            return false;
        }

        /// <summary>
        /// Resolves an address and port to an IPEndPoint.
        /// </summary>
        /// <param name="address">The address to resolve. My be an IPv4 or IPv6 dotted quad or hex notation, or a valid dns hostname.</param>
        /// <param name="port">The remote port number</param>
        /// <returns></returns>
        public static IPEndPoint Resolve(string address, int port, object sender, AddressResolutionEventHandler onResolving, object stateObject)
        {
            #region Addresss Resolution Events

            if (onResolving != null)
            {
                try
                {
                    onResolving(sender, new AddressResolutionEventArgs(address, port, stateObject));
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex);

                }
            }

            #endregion

            try
            {
                // first try and parse the address out
                // it may be a IPv4 dotted quad or in IPv6 colon-hex notation
                IPAddress ipAddress = IPAddress.Parse(address);

                // return a new end point without ever hitting dns
                return new IPEndPoint(ipAddress, port);
            }
            catch(Exception ex)
            {
                // try first then fall back on dns because connecting via ip's should be faster and try to bypass dns all together
                if (ex.GetType() == typeof(System.Threading.ThreadAbortException))
                    throw ex;
            }

            // resolve the address using DNS
            IPHostEntry he = Dns.Resolve(address);

            // create and return a new IP end point based on the address and port
            return new IPEndPoint(he.AddressList[0], port);
        }

        /// <summary>
        /// Receives the number of bytes specified from the socket before returning
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="numBytes"></param>
        /// <returns></returns>
        public static byte[] ReceiveBytes(Socket socket, int numBytes, int maxBytes)
        {
            Debug.WriteLine(string.Format("Receiving {0} bytes...", numBytes));

            // cap the number of bytes we will read at one time if necessary
            if (numBytes > maxBytes)
                numBytes = maxBytes;

            // if we're going to try and receive 0 or less bytes of data, it's prolly becuase the 
            // previous wait returned with zero...
            if (numBytes <= 0)
                throw new HttpConnectionClosedByPeerException();

            // construct a new buffer into which we'll receive the bytes
            byte[] buffer = new byte[numBytes];
            int numReceived = 0;

            //			lock(socket)
        {
            // receive the bytes specified
            numReceived = socket.Receive(buffer, 0, numBytes, SocketFlags.None);
        }

            // if we receive zero bytes that means the other side closed the connection
            if (numReceived <= 0)
                throw new HttpConnectionClosedByPeerException();

            // adjust it down if we didn't read all of them 
            if (numReceived < numBytes)
            {
                // create a new smaller buffer
                byte[] tempBuffer = new byte[numReceived];				

                // copy what we did get from the main buffer to the temp buffer
                if (numReceived > 0)
                    Buffer.BlockCopy(buffer, 0, tempBuffer, 0, numReceived);

                // and finally make the main buffer a clone of the temp (effectively shrinking the main buffer)
                buffer = tempBuffer;
            }

            return buffer;
        }

        /// <summary>
        /// Waits indefinitely for some data to be available on the socket for reading
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="abortEvent"></param>
        /// <returns></returns>
        public static int WaitForAvailableBytes(Socket socket, ManualResetEvent abortEvent)
        {		
            /* 100 ms = 100000; */
            /*  10 ms = 10000; */
            const int microseconds = 10000; 

            // while there isn't any data available
            //			lock(socket)
        {
            int available = 0;
            bool readable = false;
            bool broken = false;

            // wait until there are bytes available, or the socket is readable, or the socket errors out
            while(true)
            {
                // keep polling for data to read
                available = socket.Available;
                readable = socket.Poll(microseconds, SelectMode.SelectRead);
                broken = socket.Poll(microseconds, SelectMode.SelectError);
					
                // any of these conditions can let us out of this loop of hell
                if (available > 0 || readable || broken)
                    break;

                // see if we can bail
                if (abortEvent != null)
                    if (abortEvent.WaitOne(100, false /* stay in context */))
                        throw new OperationAbortedException();
            }
			
            //				if (available > 0)
            //					Debug.WriteLine(string.Format("{0} bytes available", available));
            //
            //				if (readable)
            //					Debug.WriteLine("The socket is readable.");
            //
            //				if (broken)
            //					Debug.WriteLine("The socket connection is broken.");

            // return the number of bytes available
            return socket.Available;
        }
        }

        /// <summary>
        /// Waits indifinitely for the specified number of bytes to become available for reading on the specified socket
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="abortEvent"></param>
        /// <param name="bytesNeeded"></param>
        /// <returns></returns>
        public static int WaitForAvailableBytes(Socket socket, ManualResetEvent abortEvent, int bytesNeeded)
        {		
            /* 100ms */
            const int microseconds = 10000; 

            //			lock(socket)
        {
            int available = 0;
            bool readable = false;
            bool broken = false;

            // wait until there are bytes available, or the socket is readable, or the socket errors out
            while(true)
            {
                // keep polling for data to read
                available = socket.Available;
                readable = socket.Poll(microseconds, SelectMode.SelectRead);
                broken = socket.Poll(microseconds, SelectMode.SelectError);
					
                // any of these conditions can let us out of this loop of hell
                if (available >= bytesNeeded || readable || broken)
                    break;

                // see if we can bail
                if (abortEvent != null)
                    if (abortEvent.WaitOne(100, false /* stay in context */))
                        throw new OperationAbortedException();
            }
			
            //				if (available > 0)
            //					Debug.WriteLine(string.Format("{0} bytes available", available));
            //
            //				if (readable)
            //					Debug.WriteLine("The socket is readable.");
            //
            //				if (broken)
            //					Debug.WriteLine("The socket connection is broken.");
				
            // return the number of bytes available
            return socket.Available;
        }
        }

        /// <summary>
        /// Sends the bytes specified from the source buffer over the socket
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="sourceBuffer"></param>
        /// <param name="offset"></param>
        /// <param name="chunksize"></param>
        /// <returns></returns>
        public static int SendBytes(Socket socket, byte[] sourceBuffer, int offset, int chunksize)
        {
            int bytesSent = 0;
            //			lock(socket)
        {
            bytesSent = socket.Send(sourceBuffer, offset, chunksize, SocketFlags.None);
        }
					
            return bytesSent;
        }

        /// <summary>
        /// Sends the bytes specified over the socket
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="sourceBuffer"></param>
        /// <returns></returns>
        public static int SendBytes(Socket socket, byte[] sourceBuffer)
        {
            int bytesSent = 0;
            //			lock(socket)
        {
            bytesSent = socket.Send(sourceBuffer, SocketFlags.None);				
        }
					
            return bytesSent;
        }

        /// <summary>
        /// Formats the number into KB 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string FormatInKiloBytes(int bytes)
        {
            decimal kb = 0;
            if (bytes > 0)
            {
                kb = Convert.ToDecimal(bytes / 1000.0);
                kb = Math.Round(kb);
                kb = Math.Max(1, kb);
            }

            int nkb = Convert.ToInt32(kb);

            return nkb.ToString("###,###,##0") + " KB";	
        }

        /// <summary>
        /// Calculates what percentage of the total the value is
        /// </summary>
        /// <param name="value"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static int GetPercent(int value, int total)
        {
            try
            {
                int percent =  (int)((((double)value) / ((double)total)) * 100d);

                return percent;
            }
            catch
            {

            }
            return 0;
        }

        /// <summary>
        /// Creates a new Tcp socket
        /// </summary>
        /// <param name="reuseAddress">A flag that indicates whether the socket can reuse addresses</param>
        /// <param name="sendTimeout">A send timeout value (milliseconds)</param>
        /// <param name="recvTimeout">A recv timeout value (milliseconds)</param>
        /// <returns></returns>
        public static Socket CreateTcpSocket(bool reuseAddress, int sendTimeout, int recvTimeout)
        {
            // create a new tcp socket
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // reuse address
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, reuseAddress ? 1 : 0);

            // send timeout
            if (sendTimeout > 0)
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, sendTimeout);

            // recv timeout
            if (recvTimeout > 0)
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, recvTimeout);

            return socket;
        }

        /// <summary>
        /// Shutsdown a socket's ability to send and receive for the local peer (this side)
        /// </summary>
        public static bool ShutdownSocket(Socket socket)
        {
            try
            {				
                socket.Shutdown(SocketShutdown.Send /* never do both */);
                return true;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return false;
        }

        /// <summary>
        /// Closes the socket and releases all resource associated with the socket
        /// </summary>
        public static bool CloseSocket(Socket socket)
        {
            try
            {				
                socket.Close();
                return true;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return false;
        }
    }
}


