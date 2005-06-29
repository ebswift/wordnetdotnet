using System;

namespace Razor.Networking.Http
{
	/// <summary>
	/// Summary description for HttpRequestLine.
	/// </summary>
	[Serializable()]
	public class HttpRequestLine
	{
		protected string _method;
		protected string _requestUri;
		protected HttpProtocolVersion _protocolVersion;

		/// <summary>
		/// Returns a string in the format 'Method SP Request-Uri SP Http-Version CRLF'
		/// </summary>
		public const string STRING_FORMAT = "{0} {1} {2}{3}";

		/// <summary>
		/// Initializes a new instance of the HttpRequestLine class
		/// </summary>
		public HttpRequestLine()
		{
			this.Method = HttpMethods.Get;
			this.RequestUri = @"/";
			this.ProtocolVersion = new HttpProtocolVersion();
		}

		/// <summary>
		/// Initializes a new instance of the HttpRequestLine class
		/// </summary>
		/// <param name="method">The method of the request</param>
		/// <param name="requestUri">The request-uri</param>
		/// <param name="protocolVersion">The protocol version</param>
		public HttpRequestLine(string method, string requestUri, string protocolVersion)
		{
			this.Method = method;
			this.RequestUri = requestUri;
			_protocolVersion = new HttpProtocolVersion(protocolVersion);
		}

		/// <summary>
		/// Initializes a new instance of the HttpRequestLine class
		/// </summary>
		/// <param name="method">The method of the request</param>
		/// <param name="requestUri">The request-uri</param>
		/// <param name="protocolVersion">The protocol version</param>
		public HttpRequestLine(string method, string requestUri, HttpProtocolVersion protocolVersion)
		{
			this.Method = method;
			this.RequestUri = requestUri;
			this.ProtocolVersion = protocolVersion;
		}

		/// <summary>
		/// Gets or sets the method contained in this request line
		/// </summary>
		public string Method
		{
			get
			{
				return _method;
			}
			set
			{
				HttpUtils.ValidateToken(@"Method", value);
				value = HttpUtils.TrimLeadingAndTrailingSpaces(value);
				_method = value;
			}
		}

		/// <summary>
		/// Gets or sets the request-uri contained in this request line
		/// </summary>
		public string RequestUri
		{
			get
			{
				return _requestUri;
			}
			set
			{
				HttpUtils.ValidateToken(@"RequestUri", value);
				value = HttpUtils.TrimLeadingAndTrailingSpaces(value);
				_requestUri = value;
			}
		}

		/// <summary>
		/// Gets or sets the protocol version contained in this request line
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
		/// Returns a string in the format 'Method SP Request-Uri SP Http-Version CRLF'
		/// </summary>
		public override string ToString()
		{
			return string.Format(STRING_FORMAT, _method, _requestUri, _protocolVersion, HttpControlChars.CRLF);
		}
		
		/// <summary>
		/// Parses a string in the format 'Method SP Request-Uri SP Http-Version CRLF' into an HttpRequestLine instance
		/// </summary>
		/// <param name="value">The string to parse. May contain CRLF.</param>
		/// <returns></returns>
		public static HttpRequestLine Parse(string value)
		{
			string[] parts = value.Split(' ');
			HttpProtocolVersion protocolVersion = HttpProtocolVersion.Parse(parts[2]);
			return new HttpRequestLine(parts[0], parts[1], protocolVersion);
		}
	}
}
