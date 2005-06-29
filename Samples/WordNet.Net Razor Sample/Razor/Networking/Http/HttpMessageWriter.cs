using System;
using System.Collections;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Razor.Networking.Http
{
	/// <summary>
	/// Provides a way to sent any HttpMessage over a connected socket using the message's 'Transfer-Encoding'.
	/// </summary>
	public class HttpMessageWriter
	{				
		public const int MAX_SEGMENT_SIZE_CHUNKED = 8184; // 8192 minus the 8 bytes for the chunk control chars in the chunk headers
		public const int MAX_SEGMENT_SIZE_NONCHUNKED = 8192;

		protected const string MY_TRACE_CATEGORY = @"'HttpMessageWriter'";

		/// <summary>
		/// Initializes a new instance of the HttpMessageWriter class
		/// </summary>
		public HttpMessageWriter()
		{
			
		}

		public void Write(Socket socket, ManualResetEvent abortEvent, HttpMessage message)
		{
			this.Write(socket, abortEvent, message, null, null);
		}

		/// <summary>
		/// Writes the message to the socket
		/// </summary>
		/// <param name="socket"></param>
		/// <param name="abortEvent"></param>
		/// <param name="message"></param>
		public void Write(Socket socket, ManualResetEvent abortEvent, HttpMessage message, HttpMessageProgressEventHandler onProgress, object stateObject)
		{			
			// send the headers of the message
			this.SendHeaders(socket, message, onProgress, stateObject);

			// send the body of the message
			this.SendBody(socket, abortEvent, message, onProgress, stateObject);
		}

		/// <summary>
		/// Checks the message for the required headers based on its type, fills in the ones that are missing if any
		/// </summary>
		/// <param name="socket"></param>
		/// <param name="message"></param>
		internal protected void CheckHeaders(Socket socket, HttpMessage message)
		{
			/*
			 * The following instructions must be followed exactly to adhere to the HTTP/1.1 specs as stated in RFC 2616.
			 * 
			 * The following sending rules apply to Requests and Response messages.
			 * 
			 * Requests: (Fields that must be set)
			 * - Host
			 * - User-Agent
			 * - Content-Length		(If Body)
			 * - Content-Type		(If Body)
			 * 
			 * Responses:
			 * - Server
			 * - Content-Length		(If Body)
			 * - Content-Type		(If Body)
			 * 
			 * So let's check our headers out now, before we go any further
			 * */
			switch(message.Type)
			{
				case HttpMessageTypes.HttpRequest:
					this.CheckRequiredRequestHeaders(socket, (HttpRequest)message);
					break;

				case HttpMessageTypes.HttpResponse:
					this.CheckRequiredResponseHeaders((HttpResponse)message);
					break;			
			};
		}

		/// <summary>
		/// Checks the headers of a request message
		/// </summary>
		/// <param name="socket"></param>
		/// <param name="request"></param>
		internal protected void CheckRequiredRequestHeaders(Socket socket, HttpRequest request)
		{
			// make sure the host header is filled in
			if (HttpUtils.IsEmptryString(request.Host))
//				lock(socket)
					request.Host = this.GetHostFromEndPointString(socket.LocalEndPoint.ToString());
			
			// make sure the user-agent header is filled in	
			if (HttpUtils.IsEmptryString(request.UserAgent))
				request.UserAgent = new RazorClientProtocolVersion().ToString();

			// if there is no body, or that body is zero length we are finished
			if (request.Body == null)
				return;	
		
			// if the body is zero length no problem
			if (request.Body.Length == 0)
				return;

			// however if there is a body with X length, we must ensure that the request's content-length header is filled in properly
			if (request.ContentLength != request.Body.Length)
				request.ContentLength = request.Body.Length;	
			
			// lastly, warn if we are going to send a request with no content-type, it's just not cool
			if (HttpUtils.IsEmptryString(request.ContentType))
				Debug.WriteLine(string.Format("The request '{0}' contains a body, but does not contain a valid 'content-type'. Please set the 'content-type' header field, or use the SetBody(XXX) methods of the HttpMessage class when setting the message body.", request.FirstLine), MY_TRACE_CATEGORY);
		}

		/// <summary>
		/// Checks the headers of a response message
		/// </summary>
		/// <param name="response"></param>
		internal protected void CheckRequiredResponseHeaders(HttpResponse response)
		{
			// make sure the server header is filled in
			if (HttpUtils.IsEmptryString(response.Server))
				response.Server = new RazorServerProtocolVersion().ToString();

			// if there is no body, or that body is zero length we are finished
			if (response.Body == null)
				return;	
		
			// if the body is zero length no problem
			if (response.Body.Length == 0)
				return;

			// however if there is a body with X length, we must ensure that the response's content-length header is filled in properly
			if (response.ContentLength != response.Body.Length)
				response.ContentLength = response.Body.Length;	
			
			// lastly, warn if we are going to send a response with no content-type, it's just not cool
			if (HttpUtils.IsEmptryString(response.ContentType))
				Debug.WriteLine(string.Format("The response '{0}' contains a body, but does not contain a valid 'content-type'. Please set the 'content-type' header field, or use the SetBody(XXX) methods of the HttpMessage class when setting the message body.", response.FirstLine), MY_TRACE_CATEGORY);
		}

		/// <summary>
		/// Returns the host from an endpoint formatted string
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		internal protected string GetHostFromEndPointString(string value)
		{
			string[] parts = value.Split(':');
			return parts[0];
		}

		/// <summary>
		/// Writes the message's headers to the socket
		/// </summary>
		/// <param name="socket"></param>
		/// <param name="message"></param>
		internal protected int SendHeaders(Socket socket, HttpMessage message, HttpMessageProgressEventHandler onProgress, object stateObject)
		{
			// check the headers of the message
			this.CheckHeaders(socket, message);

			/*
			 * The only thing we can reliably send at the start of every message are the headers
			 * Technically we should be carefull how many bytes the headers are, but at this point
			 * until i see it fail, i'm gonna take it easy on the paranoia factor, sometimes i can only defend so much before i'm wasting time... :P
			 * 
			 * So send the headers
			 * */
			byte[] bytes = message.ToByteArray(true /* get just the headers */, false /* do not include the body */);

			// send the header bytes
			int bytesSent = HttpUtils.SendBytes(socket, bytes);

			// notify the callback that we have sent the headers
			this.OnProgress(onProgress, this, new HttpMessageProgressEventArgs(message, true, bytes, bytesSent, stateObject));

			return bytesSent;
		}

		/// <summary>
		/// Writes the message's body to the socket
		/// </summary>
		/// <param name="socket"></param>
		/// <param name="message"></param>
		internal protected void SendBody(Socket socket, ManualResetEvent abortEvent, HttpMessage message, HttpMessageProgressEventHandler onProgress, object stateObject)
		{
			// bail if there is no message body
			if (message.Body == null)
				return;

			// bail if there is a message body, with no length
			if (message.Body.Length == 0)
				return;

			/*
			 * In order to properly send the body, we must first determine if the transfer-encoding is chunked or not
			 * */
			if (HttpUtils.Contains(message.TransferEncoding, HttpTransferEncodings.Chunked))
			{
				// chunked
				this.SendBodyChunked(socket, abortEvent, message, onProgress, stateObject);
			}
			else
			{
				// non-chunked
				this.SendBodyNonChunked(socket, abortEvent, message, onProgress, stateObject);
			}
		}

		/// <summary>
		/// Sends the body of a chunked transfer-encoded message
		/// </summary>
		/// <param name="socket"></param>
		/// <param name="abortEvent"></param>
		/// <param name="message"></param>
		internal protected void SendBodyChunked(Socket socket, ManualResetEvent abortEvent, HttpMessage message, HttpMessageProgressEventHandler onProgress, object stateObject)
		{
			int segmentNumber = 0;
			int totalBytesSent = 0;
			byte[] body = message.ToByteArray(false /* no headers */, true /* just the body */);

			while(true)
			{				
				// if we have sent the entire message then we can abortEvent
				if (totalBytesSent > body.Length)
					break;

				// start out with a full segment size (this is just the buffer size by which we will send - 8k seems to be really common although i have no idea why other than that it's a power of 2...)
				int chunkSize = MAX_SEGMENT_SIZE_CHUNKED;

				// adjust how much we need to send to complete the message
				if (chunkSize > (body.Length - totalBytesSent))
					chunkSize = (body.Length - totalBytesSent);
					
				// bump up the segment number
				segmentNumber++;

				// create a chunk of data to send
				byte[] bytes = new byte[chunkSize];
				Buffer.BlockCopy(body, totalBytesSent, bytes, 0, chunkSize);

				// create a chunk around the data
				HttpChunk chunk = new HttpChunk(bytes);

				// send the data
				int bytesSent = HttpUtils.SendBytes(socket, chunk.ToByteArray());
				
				// if any data was sent
				if (bytesSent > 0)
				{
					// figure out how much as data, minus the chunks control chars
					int actualBytesSent = bytesSent - chunk.GetNonDataByteCount();

					// if the chunksize is zero or less then we'll return what was sent
					if (chunkSize > 0)
						// all other times it'll not count the non data byte count in the chunk
						bytesSent = actualBytesSent;           
				}

				// tally the number of bytes we have sent
				totalBytesSent += bytesSent;

				// notify the callback of our progress
				this.OnProgress(onProgress, this, new HttpMessageProgressEventArgs(message, false, bytes, totalBytesSent, stateObject));
				
				// clear that buffer immediately
				bytes = null;				
							
				// see if we have been instructed to abortEvent reading
				if (abortEvent != null)
					if (abortEvent.WaitOne(1, false))						
						throw new HttpMessageWriterAbortedException(message);									
			}
		}

		/// <summary>
		/// Sends the body of a non-chunked transfer-encoded message
		/// </summary>
		/// <param name="socket"></param>
		/// <param name="abortEvent"></param>
		/// <param name="message"></param>
		internal protected void SendBodyNonChunked(Socket socket, ManualResetEvent abortEvent, HttpMessage message, HttpMessageProgressEventHandler onProgress, object stateObject)
		{
			int segmentNumber = 0;
			int totalBytesSent = 0;		
			byte[] body = message.ToByteArray(false /* no headers */, true /* just the body */);
			while(true)
			{
				// if we have sent the entire message then we can abortEvent
				if (totalBytesSent == body.Length)
					break;

				// start out with a full segment size (this is just the buffer size by which we will send - 8k seems to be really common although i have no idea why other than that it's a power of 2...)
				int chunkSize = MAX_SEGMENT_SIZE_NONCHUNKED;

				// adjust how much we need to send to complete the message
				if (chunkSize > (body.Length - totalBytesSent))
					chunkSize = (body.Length - totalBytesSent);
					
				// bump up the segment number
				segmentNumber++;

				// create the chunk to send
				byte[] bytes = new byte[chunkSize];
				Buffer.BlockCopy(body, totalBytesSent /* offset */, bytes, 0, chunkSize);

				// try and send the segment
				int bytesSent = HttpUtils.SendBytes(socket, bytes);

				// update the stream position which is the total number of bytes received
				totalBytesSent += bytesSent;

				// notify the callback of our progress
				this.OnProgress(onProgress, this, new HttpMessageProgressEventArgs(message, false, bytes, totalBytesSent, stateObject));
				
				// clear that buffer immediately
				bytes = null;				
									
				// see if we have been instructed to abortEvent reading
				if (abortEvent != null)
					if (abortEvent.WaitOne(1, false))						
						throw new HttpMessageWriterAbortedException(message);					
			} // while (true)
		}

		/// <summary>
		/// Notifies the callback method of our progress while reading in the message
		/// </summary>
		/// <param name="onProgress"></param>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnProgress(HttpMessageProgressEventHandler onProgress, object sender, HttpMessageProgressEventArgs e)
		{			
			if (onProgress != null)
				onProgress(sender, e);			
		}
	}

	#region HttpMessageWriterAbortedException

	/// <summary>
	/// Defines an exception that is thrown by the writer if it is aborted
	/// </summary>
	public class HttpMessageWriterAbortedException : Exception
	{
		protected HttpMessage _message;

		/// <summary>
		/// Initializes a new instance of the HttpMessageWriterAbortedException class
		/// </summary>
		/// <param name="message"></param>
		public HttpMessageWriterAbortedException(HttpMessage message) : base(string.Format("The writer was aborted while sending the message '{0}'.", message.ToString(false)))
		{
			_message = message;
		}

		/// <summary>
		/// Returns the message that was in context when the abort was executed
		/// </summary>
		public HttpMessage Context
		{
			get
			{
				return _message;
			}
		}
	}

	#endregion
}
