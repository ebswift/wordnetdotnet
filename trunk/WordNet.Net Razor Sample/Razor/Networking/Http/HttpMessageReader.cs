using System;
using System.Diagnostics;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Razor.Networking.Http
{    
	/// <summary>
	/// Summary description for HttpMessageReader.
	/// </summary>
	public class HttpMessageReader
	{
		/// <summary>
		/// The states the drive the reader
		/// </summary>
		private enum Processing
		{
			FirstLine,				// processing the first line
			HeaderLine,				// processing the headers of the message			
			ChunkSizeLine,			// processing the chunk size line
			ChunkData,				// processing the chunk data
			ChunkTrailerHeaderLine, // processing the chunk trailer headers of the chunked body
			Body,					// processing the body
			Finished
		}

		private Processing			_state;
//		private string				_firstLine;
//		private HttpHeaderList		_headers;
		private HttpChunk			_chunk;
		private HttpChunkedBody		_chunkedBody;
		private HttpByteParser		_parser;
		private byte[]				_previouslyReceivedBytes;
		private byte[]				_receivedBytes;
		protected int				_headerOffsetStart;
		protected int				_headerOffsetEnd;
		
		/// <summary>
		/// Defines the maximum buffer length
		/// </summary>
		public const int MAX_BUFFER_LENGTH  = 8192;

		/// <summary>
		/// Initializes a new instance of the X class
		/// </summary>
		public HttpMessageReader()
		{
			
		}

		/// <summary>
		/// Initializes internal variables used to read the message
		/// </summary>
		private void InitVars()
		{
			// reset our internal vars
			_state = Processing.FirstLine;
			_chunk = null;
			_chunkedBody = null;
            _parser = new HttpByteParser();
			_receivedBytes = null;
			_headerOffsetStart = -1;
			_headerOffsetEnd = -1;			
		}

		/// <summary>
		/// Destroys the internal variables used to read the message
		/// </summary>
		private void CleanupVars()
		{
			// reset our internal vars
			_chunk = null;
			_chunkedBody = null;
			_parser = null;
			_receivedBytes = null;
			_headerOffsetStart = -1;
			_headerOffsetEnd = -1;	
		}

		/// <summary>
		/// Reads an HttpMessage from the specified socket
		/// </summary>
		/// <param name="socket"></param>
		/// <returns></returns>
		public virtual HttpMessage Read(Socket socket)
		{
			return this.Read(socket, null, null, null);
		}

		/// <summary>
		/// Reads an HttpMessage from the specified socket
		/// </summary>
		/// <param name="socket"></param>
		/// <param name="abortEvent"></param>
		/// <returns></returns>
		public virtual HttpMessage Read(Socket socket, ManualResetEvent abortEvent)
		{
			return this.Read(socket, abortEvent, null, null);
		}

		/// <summary>
		/// Reads an HttpMessage from the specified socket
		/// </summary>
		/// <param name="socket">The socket to read</param>
		/// <param name="abortEvent">An event to watch in case of an abort</param>
		/// <returns></returns>
		public virtual HttpMessage Read(Socket socket, ManualResetEvent abortEvent, HttpMessageProgressEventHandler onProgress, object stateObject)
		{
			if (socket == null)
				throw new ArgumentNullException("socket");
			
			// setup our vars
			this.InitVars();			

			// create a new message
			HttpMessage message = new HttpMessage();

			try
			{
				// while we've not finished reading the message
				while(!this.IsFinished(abortEvent))
				{
					// keep receiving
					this.ReceiveData(socket, abortEvent, ref message, onProgress, stateObject);

					// keep processing
					this.ProcessData(abortEvent, ref message, onProgress, stateObject);				
				}
			}
			catch(OperationAbortedException)
			{
				throw new HttpMessageReaderAbortedException(message);
			}
			
			// set the message body
			message._body = this.ReconstructChunkedBodyIfNecessary(ref message);

			// clean up our vars
			this.CleanupVars();

			return message;
		}

		/// <summary>
		/// Waits for data to become available for reading on the socket, then reads it into the main buffer for processing
		/// </summary>
		private void ReceiveData(Socket socket, ManualResetEvent abortEvent, ref HttpMessage message, HttpMessageProgressEventHandler onProgress, object stateObject)
		{
			/*
			 * Data Receiving Phase
			 * This will receive data from the socket until we have enough data to process.
			 * */

			// wait for some bytes to become available
			int bytesAvailable = 0;
			byte[] bytesReceived = new byte[0];

			// if we don't have data left over from last time try and go with just that first
			if (_previouslyReceivedBytes == null)
			{
				// ok there was nothing left over, so we're back to receiving again
				bytesAvailable = HttpUtils.WaitForAvailableBytes(socket, abortEvent);
					
				// receive as many bytes as we can (a chunk as it were)
				bytesReceived = HttpUtils.ReceiveBytes(socket, bytesAvailable, MAX_BUFFER_LENGTH);
			}
						
			// if there was anything left over from last time
			if (_previouslyReceivedBytes != null)
			{				
				// combine the previous with this chunk
				bytesReceived = HttpUtils.Combine(_previouslyReceivedBytes, bytesReceived);					
			
				Debug.WriteLine(string.Format("Retaining '{0}' bytes from last message.", bytesReceived.Length));

				// reset the previous buffer
				_previouslyReceivedBytes = null;
			}

			// if we have a previous message buffer
			if (_receivedBytes != null)
			{
				// combine what we had, with what we just received
				byte[] buffer = HttpUtils.Combine(_receivedBytes, bytesReceived);
				_receivedBytes = buffer;			
			}
			else
				/*
				 * this is the first part of a new message
				 * */
				// otherwise just start with the chunk
				_receivedBytes = bytesReceived;				

			// if we have received the headers
			if (this.IsPastHeaders())
				// notify the callback that data has been received
				this.OnProgress(onProgress, this, new HttpMessageProgressEventArgs(message, false, bytesReceived, _receivedBytes.Length, stateObject));							
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

		/// <summary>
		/// Processes the raw data, either reading headers or chunked body data
		/// </summary>
		private void ProcessData(ManualResetEvent abortEvent, ref HttpMessage message, HttpMessageProgressEventHandler onProgress, object stateObject)
		{
			// at each pass in the processing phase set the buffer to be parsed by the parser to the buffer into which we received bytes
			// the previous parsing phase may have failed simply because we did not have enough data yet to fullfill the current state's needs
			// so we'll update what we have received and carry on from the last spot, just hopefully with more data... :P
			_parser.SetBuffer(_receivedBytes);
						
			bool stillParsing = true;

			// enter a processing loop and keep trying to parse data based on our current state until the phase fails
			while(stillParsing)
			{
				// look at our current state to determine what data we should be trying to process and parse
				switch(_state)
				{					
					case Processing.FirstLine:
						stillParsing = this.TryAndParseFirstLine(ref message, onProgress, stateObject);	
						break;
					
					case Processing.HeaderLine:
						stillParsing = this.TryAndParseHeaderLine(ref message, onProgress, stateObject);	
						break;
						
					case Processing.ChunkSizeLine:
						stillParsing = this.TryAndParseChunkSizeLine(ref message, onProgress, stateObject);	
						break;

					case Processing.ChunkData:
						stillParsing = this.TryAndParseChunkData(ref message, onProgress, stateObject);	
						break;

					case Processing.ChunkTrailerHeaderLine:
						stillParsing = this.TryAndParseTrailerHeaderLine(ref message, onProgress, stateObject);	
						break;

					case Processing.Body:
						stillParsing = this.TryAndParseBody(ref message, onProgress, stateObject);	
						break;

					case Processing.Finished:
						Debug.Assert(false, "We should be out of the processing loop if our state is 'Finished'.");
						break;
				};

				// and in case any of the methods change the state to finished, but think they are still processing
				if (stillParsing)
					// jump in and check so we don't get stuck in an endless loop of parsing
					stillParsing = !this.IsFinished(abortEvent);
			}
		}
        
		/// <summary>
		/// Tries to parse the first line for the message
		/// </summary>
		/// <returns></returns>
		private bool TryAndParseFirstLine(ref HttpMessage message, HttpMessageProgressEventHandler onProgress, object stateObject)
		{
			// try and parse a token from the data
			HttpByteParserToken token = _parser.GetNextToken();						
			if (token == null)
				return false;
			
			// the first line of any message is never empty
			if (token.IsEmpty)
				return false; 

			// success! store the first line of the message
			message._firstLine = token.ToString(HttpUtils.Encoding);

			// save the offset into the data where the headers start
			_headerOffsetStart = _parser.Index;

			// change state to processing headers!
			_state = Processing.HeaderLine;
			
			return true;
		}

		/// <summary>
		/// Tries to parse a header line for the message
		/// </summary>
		/// <returns></returns>
		private bool TryAndParseHeaderLine(ref HttpMessage message, HttpMessageProgressEventHandler onProgress, object stateObject)
		{
			// try and parse a token from the data
			HttpByteParserToken token = _parser.GetNextToken();						
			if (token == null)
				return false;
			
			// the first line of any message is never empty
			if (token.IsEmpty)
			{
				// save the offset into the data where the headers end
				_headerOffsetEnd = _parser.Index;

				// determine if the body is chunked, as we have all the headers now we can determine how the message is going to come in
				if (message.IsChunked)
					// change state to processing a chunk size line
					_state = Processing.ChunkSizeLine;
				else
					// change state to processing the body
					_state = Processing.Body;

				// notify the callback that we have received the headers
				this.OnProgress(onProgress, this, new HttpMessageProgressEventArgs(message, true, new byte[] {}, _receivedBytes.Length, stateObject));							
				
				// we're done with the headers
				return true; 
			}

			// success! this line is a header
			string line = token.ToString(HttpUtils.Encoding);

			// parse the line into a new header
			HttpHeader header = HttpHeader.Parse(line);

			// save the header
			message.Headers.Add(header);

			return true;
		}

		/// <summary>
		/// Tries to parse a chunk size line for the message
		/// </summary>
		/// <returns></returns>
		private bool TryAndParseChunkSizeLine(ref HttpMessage message, HttpMessageProgressEventHandler onProgress, object stateObject)
		{
			// try and parse a token from the data
			HttpByteParserToken token = _parser.GetNextToken();						
			if (token == null)
				return false;
			
			// the first line of any message is never empty
			if (token.IsEmpty)
				return false;

			// success! this line is a header
			string line = token.ToString(HttpUtils.Encoding);

			// parse the size line
			HttpChunkSizeLine chunkSizeLine = HttpChunkSizeLine.Parse(line);

			// create a new chunk
			_chunk = new HttpChunk(chunkSizeLine, null);
			
			// change state to processing chunk data
			_state = Processing.ChunkData;

			return true;
		}

		/// <summary>
		/// Tries to parse some chunk data for the message
		/// </summary>
		/// <returns></returns>
		private bool TryAndParseChunkData(ref HttpMessage message, HttpMessageProgressEventHandler onProgress, object stateObject)
		{
			// get bytes, and update the index...
			int waitingOn = _chunk.Size;
			int received = _receivedBytes.Length - 2 - _parser.Index;
			
			// if we received enough data to pull in a chunk, do that now
			if (received >= waitingOn)
			{
				// copy the end of the data out as chunk data
				byte[] data = HttpUtils.Clone(_receivedBytes, _parser.Index, _chunk.Size);

				// bump the parser past the end of the next \r\n
				_parser.Index += _chunk.Size + 2;

				// create a new chunked body
				if (_chunkedBody == null)
					_chunkedBody = new HttpChunkedBody();

				// assign the data to the chunk
				_chunk.Data = data;

				// todo: 

				// add the chunk to the body
				_chunkedBody.Chunks.Add(_chunk);

				// if the chunk is empty, it's the last chunk		
				bool empty = _chunk.IsEmpty;
				
				// destroy the chunk
				_chunk = null;

				if (empty)
				{
					// change state to processing chunk data
					_state = Processing.ChunkTrailerHeaderLine;
				}
				else
				{
					// go back to processing a chunk size line
					_state = Processing.ChunkSizeLine;
				}

				return true;				
			}

			return false;
		}

		/// <summary>
		/// Tries to parse a trailer header for the message
		/// </summary>
		/// <returns></returns>
		private bool TryAndParseTrailerHeaderLine(ref HttpMessage message, HttpMessageProgressEventHandler onProgress, object stateObject)
		{
			// try and parse a token from the data
			HttpByteParserToken token = _parser.GetNextToken();						
			if (token == null)
			{
				// change state to processing the body
				_state = Processing.Finished;

				// we're done with the headers
				return true; 
			}
			
			// the first line of any message is never empty
			if (token.IsEmpty)
			{
				// change state to processing the body
				_state = Processing.Finished;

				// we're done with the headers
				return true; 
			}

			// success! this line is a header
			string line = token.ToString(HttpUtils.Encoding);

			// parse the line into a new header
			HttpHeader header = HttpHeader.Parse(line);

			// save the header in the chunked body trailers
			_chunkedBody.TrailerHeaders.Add(header);

			return true;
		}

		/// <summary>
		/// Tries to parse the body for the message
		/// </summary>
		/// <returns></returns>
		private bool TryAndParseBody(ref HttpMessage message, HttpMessageProgressEventHandler onProgress, object stateObject)
		{
			// determine the content length of the message's entity
			int contentLength = message.ContentLength;

			// the number of bytes we have received thus far would be determined by looking at the different between what we have and where the header's stopped
			int postedBytesLength = _receivedBytes.Length - _headerOffsetEnd;

			// we could have potentially received more than just one message so the extra data would be denoted by the difference between the content length and the body which we received
			int extraBytesReceived = postedBytesLength - contentLength;

			// if we received extra bytes that do not belong to this message
			if (extraBytesReceived > 0)
			{				
				// create a new buffer to hold the exra
				_previouslyReceivedBytes = new byte[extraBytesReceived];

				// copy the tail of the message off as extra bytes
				Buffer.BlockCopy(_receivedBytes, _headerOffsetEnd + contentLength, _previouslyReceivedBytes, 0, extraBytesReceived);

				// now shrink the bytes received to be exactly what comprises the bytes that make up this message
				byte[] tempBuffer = new byte[_headerOffsetEnd + contentLength];
				Buffer.BlockCopy(_receivedBytes, 0, tempBuffer, 0, tempBuffer.Length);				
				_receivedBytes = tempBuffer;
			}

			// if we've read in enough data to get the body, or there's no body coming
			if ( (postedBytesLength >= contentLength && contentLength > 0) || (contentLength == 0))
			{				
				// change state to finished
				_state = Processing.Finished;		

				// we want to kick out of the processing loop
				return false;
			}

			// otherwise we simply need to wait on the socket to receive more data
			return false;
		}

		/// <summary>
		/// Returns a flag that determines if the reader is finished reading the message
		/// </summary>
		private bool IsFinished(ManualResetEvent abortEvent)
		{
			if (abortEvent != null)
				if (abortEvent.WaitOne(10, false))
					throw new OperationAbortedException();

			return (_state == Processing.Finished);	
		}	
		
		/// <summary>
		/// Returns a flag that indicates if the reader is past the headers of the message
		/// </summary>
		/// <returns></returns>
		private bool IsPastHeaders()
		{
			if (_state == Processing.FirstLine || _state == Processing.HeaderLine)
				return false;			
			return true;
		}

//		/// <summary>
//		/// Returns the content length of the message
//		/// </summary>
//		private int GetContentLength(ref HttpMessage message)
//		{
//			// if the headers don't contain the content length header, the length is zero
//			if (!message.Headers.Contains(HttpHeaders.EntityHeaders.ContentLength))
//				return 0;
//
//			// retrieve the header
//			HttpHeader header = message.Headers[HttpHeaders.EntityHeaders.ContentLength];
//			
//			try
//			{
//				// try and parse the content length
//				int contentLength = int.Parse(header.Value);
//				if (contentLength < 0)
//					contentLength = 0; // someone might be trying to screw us
//				return contentLength;
//			}
//			catch(Exception ex)
//			{
//				Debug.WriteLine(ex);
//			}
//
//			// fall back on zero if there is a failure
//			return 0;
//		}

		/// <summary>
		/// Returns a byte array that contains the bytes forming the message body
		/// </summary>
		private byte[] ReconstructChunkedBodyIfNecessary(ref HttpMessage message)
		{
			byte[] buffer = null;

			// if the message was chunked
			if (message.IsChunked)
			{
				// reasemble the body from the chunks
				// this is where the chunked body class will come in
				if (_chunkedBody != null)
					buffer = _chunkedBody.ToByteArray();
			}
			else
			{
				// if there is no content length specified
				if (message.ContentLength == 0)
					// bail out with no body
					return null;
				
				/*
				 * ---------------------
				 * |	 First Line	   | CRLF
				 * ---------------------
				 * |	Header Lines   |
				 * .....................
				 * .....................
				 * .....................
				 * .....................
				 * |				   | CRLF CRLF
				 * ---------------------
				 * |	   Body        |
				 * ---------------------
				 * */

				// copy the bytes that make up the body into a temp buffer
				buffer = new byte[_receivedBytes.Length - _headerOffsetEnd];
				Buffer.BlockCopy(_receivedBytes, _headerOffsetEnd, buffer, 0, buffer.Length);
			}

			return buffer;
		}
	}

	#region HttpMessageReaderAbortedException

	/// <summary>
	/// Defines an exception that is thrown by the writer if it is aborted
	/// </summary>
	public class HttpMessageReaderAbortedException : Exception
	{
		protected HttpMessage _message;

		/// <summary>
		/// Initializes a new instance of the HttpMessageWriterAbortedException class
		/// </summary>
		/// <param name="message"></param>
		public HttpMessageReaderAbortedException(HttpMessage message) : base(string.Format("The reader was aborted while receiving the message '{0}'.", message.ToString(false)))
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
