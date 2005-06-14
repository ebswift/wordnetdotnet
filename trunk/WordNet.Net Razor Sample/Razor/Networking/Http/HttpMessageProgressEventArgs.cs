using System;
using System.Diagnostics;
using System.Collections;

namespace Razor.Networking.Http
{
	/// <summary>
	/// Summary description for HttpMessageProgressEventArgs.
	/// </summary>
	public class HttpMessageProgressEventArgs
	{
		protected HttpMessage _message;
		protected bool _justHeaders;
		protected byte[] _bytes;			
		protected int _totalBytes;
		protected object _stateObject;

		/// <summary>
		/// Initializes a new instance of the X class
		/// </summary>
		/// <param name="message">The message being processed</param>
		/// <param name="justHeaders">A flag to indicated that only the message headers have been processed</param>
		/// <param name="bytes">The bytes that were just processed</param>
		/// <param name="totalBytes">The total number of bytes that have been processed</param>
		public HttpMessageProgressEventArgs(HttpMessage message, bool justHeaders, byte[] bytes, int totalBytes)
		{
			_message = message;
			_justHeaders = justHeaders;
			_bytes = bytes;
			_totalBytes = totalBytes;
		}

		/// <summary>
		/// Initializes a new instance of the HttpMessageProgressEventArgs class
		/// </summary>
		/// <param name="message">The message being processed</param>
		/// <param name="justHeaders">A flag to indicated that only the message headers have been processed</param>
		/// <param name="bytes">The bytes that were just processed</param>
		/// <param name="totalBytes">The total number of bytes that have been processed</param>
		/// <param name="stateObject">A user defined object that can be used to hold state information about the event</param>
		public HttpMessageProgressEventArgs(HttpMessage message, bool justHeaders, byte[] bytes, int totalBytes, object stateObject)
		{
			_message = message;
			_justHeaders = justHeaders;
			_bytes = bytes;
			_totalBytes = totalBytes;
			_stateObject = stateObject;
		}

		/// <summary>
		/// Returns the message that is being received
		/// </summary>
		public HttpMessage Message
		{
			get
			{
				return _message;				
			}
		}

		/// <summary>
		/// Returns a flag that indicates the progress just reflects upon the headers
		/// </summary>
		public bool JustHeaders
		{
			get
			{
				return _justHeaders;
			}
		}

		/// <summary>
		/// Returns the array of bytes that have just been received from the connection, but not yet processed
		/// </summary>
		public byte[] Bytes
		{
			get
			{
				return _bytes;
			}
		}

		/// <summary>
		/// Returns the total number of bytes that have been processed thus far
		/// </summary>
		public int TotalBytes
		{
			get
			{
				return _totalBytes;
			}
		}		

		/// <summary>
		/// Returns a user defined state object that can be used to determine the context of the event
		/// </summary>
		public object StateObject
		{
			get
			{
				return _stateObject;
			}
		}
	}

	public delegate void HttpMessageProgressEventHandler(object sender, HttpMessageProgressEventArgs e);
}
