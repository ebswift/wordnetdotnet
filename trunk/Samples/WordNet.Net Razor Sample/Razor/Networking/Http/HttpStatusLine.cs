using System;

namespace Razor.Networking.Http
{
	/// <summary>
	/// 
	///     The first line of a Response message is the HttpStatus-Line, consisting
	///		of the protocol version followed by a numeric status code and its
	///		associated textual phrase, with each element separated by SP
	///		characters. No CR or LF is allowed except in the final CRLF sequence.
	///
	///			HttpStatus-Line = HTTP-Version SP HttpStatus-Code SP Reason-Phrase CRLF
	///			
	/// </summary>
	/// 
	[Serializable()]
	public class HttpStatusLine
	{	
		protected HttpProtocolVersion _protocolVersion;
		protected HttpStatus _status;

		/// <summary>
		/// Returns a string in the format 'HttpProtocolVersion SP HttpStatus-Code SP Reason-Phrase CRLF'
		/// </summary>
		public const string STRING_FORMAT = "{0} {1}{2}";

		/// <summary>
		/// Initializes a new instance of the HttpStatusLine class
		/// </summary>
		/// <param name="code">The status-code of the response</param>
		/// <param name="reason">The reason-phrase to describe the status-code</param>
		public HttpStatusLine(int code, string reason) : this(new HttpProtocolVersion(), code, reason)
		{
						
		}
		
		/// <summary>
		/// Initializes a new instance of the HttpStatusLine class
		/// </summary>
		/// <param name="protocolVersion">The protocol version in use</param>
		/// <param name="code">The status-code of the response</param>
		/// <param name="reason">The reason-phrase to describe the status-code</param>
		public HttpStatusLine(HttpProtocolVersion protocolVersion, int code, string reason)
		{
			this.ProtocolVersion = protocolVersion;
			_status = new HttpStatus(code, reason);			
		}

		/// <summary>
		/// Initializes a new instance of the HttpStatusLine class
		/// </summary>
		/// <param name="protocolVersion">The protocol version in use</param>
		/// <param name="status">The status-code and reason-phrase</param>
		public HttpStatusLine(HttpProtocolVersion protocolVersion, HttpStatus status)
		{
			if (protocolVersion == null)
				throw new ArgumentNullException("protocolVersion");

			if (status == null)
				throw new ArgumentNullException("status");

			_protocolVersion = protocolVersion;
			_status = status;
		}

		/// <summary>
		/// Initializes a new instance of the HttpStatusLine class
		/// </summary>
		/// <param name="status"></param>
		public HttpStatusLine(HttpStatus status) : this(new HttpProtocolVersion(), status)
		{

		}

		/// <summary>
		/// Returns the protocol version in use
		/// </summary>
		public HttpProtocolVersion ProtocolVersion
		{
			get
			{
				return _protocolVersion;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException("ProtocolVersion");

				_protocolVersion = value;
			}
		}

		/// <summary>
		/// Return the Http status for the response
		/// </summary>
		public HttpStatus Status
		{
			get
			{
				return _status;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException("Status");

				_status = value;
			}
		}

		/// <summary>
		/// Returns a string in the format 'Status-Line = HTTP-Version SP Status-Code SP Reason-Phrase CRLF'
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format(STRING_FORMAT, _protocolVersion, _status, HttpControlChars.CRLF);
		}

		/// <summary>
		/// Parses a string in the format 'HTTP-Version SP Status-Code SP Reason-Phrase CRLF' into an HttpStatusLine instance
		/// </summary>
		/// <example>
		/// HTTP/1.1 200 OK\r\n
		/// </example>
		/// <param name="value">The string to parse. May contain CRLF.</param>
		/// <returns></returns>
		public static HttpStatusLine Parse(string value)
		{
			int firstSpace = value.IndexOf(HttpControlChars.SP, 0);
			string a = value.Substring(0, firstSpace);
			string b = value.Substring(++firstSpace);            
			
			HttpProtocolVersion protocolVersion = HttpProtocolVersion.Parse(a);
			HttpStatus status = HttpStatus.Parse(b);
			
			return new HttpStatusLine(protocolVersion, status);			
		}
	}	
}
