using System;
using System.Collections;
using System.Diagnostics;

namespace Razor.Networking.Http
{
	/// <summary>
	/// Summary description for HttpResponse.
	/// </summary>
	public class HttpResponse : HttpMessage 
	{
		protected HttpStatusLine _statusLine;

		/// <summary>
		/// Initializes a new instance of the HttpResponse class
		/// </summary>
		public HttpResponse() : base()
		{
			_statusLine = new HttpStatusLine(new OkStatus()); // 200 OK
			this.InitHeaders();
		}

		/// <summary>
		/// Initializes a new instance of the HttpResponse class
		/// </summary>
		/// <param name="status"></param>
		public HttpResponse(HttpStatus status) : base()
		{
			_statusLine = new HttpStatusLine(status);
			this.InitHeaders();
		}

		/// <summary>
		/// Initializes a new instance of the HttpResponse class
		/// </summary>
		/// <param name="message"></param>
		public HttpResponse(HttpMessage message) : base(message)
		{
			_statusLine = HttpStatusLine.Parse(base.FirstLine);
//			this.InitHeaders(); // no very bad!!! using a message as a constructor means we've received a message and we're creating a response around it, so it will already have headers!!!
		}

		#region My Overrides

		/// <summary>
		/// Override the first line funcionality to include our response's status line object
		/// </summary>
		internal override string FirstLine
		{
			get
			{
				return _statusLine.ToString();
			}
			set
			{
				_statusLine = HttpStatusLine.Parse(value);
			}
		}

		#endregion

		#region My Public Properties

		/// <summary>
		/// Returns the protocol version in use
		/// </summary>
		public HttpProtocolVersion ProtocolVersion
		{
			get
			{
				return _statusLine.ProtocolVersion;
			}
			set
			{
				_statusLine.ProtocolVersion = value;
			}
		}

		/// <summary>
		/// Return the Http status for the response
		/// </summary>
		public HttpStatus Status
		{
			get
			{
				return _statusLine.Status;
			}
			set
			{
				_statusLine.Status = value;
			}
		}

		#region Response Headers

		/// <summary>
		/// Gets or sets the Response header 'Accept-Ranges'
		/// </summary>
		public string AcceptRanges
		{
			get
			{
				return this.ReadHeaderValue(HttpHeaders.ResponseHeaders.AcceptRanges);
			}
			set
			{
				this.WriteHeaderValue(HttpHeaders.ResponseHeaders.AcceptRanges, value, true);
			}
		}

		/// <summary>
		/// Gets or sets the Response header 'Age'
		/// </summary>
		public string Age
		{
			get
			{
				return this.ReadHeaderValue(HttpHeaders.ResponseHeaders.Age);
			}
			set
			{
				this.WriteHeaderValue(HttpHeaders.ResponseHeaders.Age, value, true);
			}
		}

		/// <summary>
		/// Gets or sets the Response header 'ETag'
		/// </summary>
		public string ETag
		{
			get
			{
				return this.ReadHeaderValue(HttpHeaders.ResponseHeaders.ETag);
			}
			set
			{
				this.WriteHeaderValue(HttpHeaders.ResponseHeaders.ETag, value, true);
			}
		}
		/// <summary>
		/// Gets or sets the Response header 'Accept-Ranges'
		/// </summary>
		public string Location
		{
			get
			{
				return this.ReadHeaderValue(HttpHeaders.ResponseHeaders.Location);
			}
			set
			{
				this.WriteHeaderValue(HttpHeaders.ResponseHeaders.Location, value, true);
			}
		}
		/// <summary>
		/// Gets or sets the Response header 'Proxy-Authenticate'
		/// </summary>
		public string ProxyAuthenticate
		{
			get
			{
				return this.ReadHeaderValue(HttpHeaders.ResponseHeaders.ProxyAuthenticate);
			}
			set
			{
				this.WriteHeaderValue(HttpHeaders.ResponseHeaders.ProxyAuthenticate, value, true);
			}
		}
		/// <summary>
		/// Gets or sets the Response header 'Retry-After'
		/// </summary>
		public string RetryAfter
		{
			get
			{
				return this.ReadHeaderValue(HttpHeaders.ResponseHeaders.RetryAfter);
			}
			set
			{
				this.WriteHeaderValue(HttpHeaders.ResponseHeaders.RetryAfter, value, true);
			}
		}
		/// <summary>
		/// Gets or sets the Response header 'Server'
		/// </summary>
		public string Server
		{
			get
			{
				return this.ReadHeaderValue(HttpHeaders.ResponseHeaders.Server);
			}
			set
			{
				this.WriteHeaderValue(HttpHeaders.ResponseHeaders.Server, value, true);
			}
		}
		/// <summary>
		/// Gets or sets the Response header 'Vary'
		/// </summary>
		public string Vary
		{
			get
			{
				return this.ReadHeaderValue(HttpHeaders.ResponseHeaders.Vary);
			}
			set
			{
				this.WriteHeaderValue(HttpHeaders.ResponseHeaders.Vary, value, true);
			}
		}
		/// <summary>
		/// Gets or sets the Response header 'WWW-Authenticate'
		/// </summary>
		public string WWWAuthenticate
		{
			get
			{
				return this.ReadHeaderValue(HttpHeaders.ResponseHeaders.WWWAuthenticate);
			}
			set
			{
				this.WriteHeaderValue(HttpHeaders.ResponseHeaders.WWWAuthenticate, value, true);
			}
		}

		#endregion

		#endregion

		/// <summary>
		/// Initializes the needed headers
		/// </summary>
		protected virtual void InitHeaders()
		{
			// this.ContentEncoding = HttpContentEncodings.i
			this.Connection = HttpConnections.Close;
			this.ContentType = MIME.Text.Plain;
			this.ContentLength = 0;
			this.Server = new RazorServerProtocolVersion().ToString();
		}
	}
}
