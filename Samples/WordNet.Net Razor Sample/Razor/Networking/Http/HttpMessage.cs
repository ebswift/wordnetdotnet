using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.IO;
using System.Xml;

namespace Razor.Networking.Http
{
	/// <summary>
	/// Summary description for HttpMessage.
	/// </summary>
	public class HttpMessage : MarshalByRefObject
	{						
		internal protected string _firstLine;
		internal protected HttpHeaderList _headers;
		internal protected byte[] _body;	

		/// <summary>
		/// Initializes a new instance of the HttpMessage class
		/// </summary>
		public HttpMessage()
		{
			_headers = new HttpHeaderList();
		}

		/// <summary>
		/// Initializes a new instance of the HttpMessage class
		/// </summary>
		/// <param name="firstLine"></param>
		/// <param name="headers"></param>
		public HttpMessage(string firstLine, HttpHeaderList headers)
		{			
			_firstLine = firstLine;
			_headers = headers;
			this.Body = null;
		}

		/// <summary>
		/// Initializes a new instance of the HttpMessage class
		/// </summary>
		/// <param name="firstLine"></param>
		/// <param name="headers"></param>
		/// <param name="body"></param>
		public HttpMessage(string firstLine, HttpHeaderList headers, byte[] body)
		{			
			_firstLine = firstLine;
			_headers = headers;
			this.Body = body;
		}
        
		/// <summary>
		/// Initializes a new instance of the HttpMessage class
		/// </summary>
		/// <param name="message"></param>
		public HttpMessage(HttpMessage message)
		{
			_firstLine = message.FirstLine;
			_headers = new HttpHeaderList();
			foreach(HttpHeader header in message.Headers)
				_headers.Add(new HttpHeader(header.Name, header.Value));
			this.Body = message.Body;
		}

		public override object InitializeLifetimeService()
		{
			return null;
		}


		#region My Public Properties

		/// <summary>
		/// Returns the type of this message
		/// </summary>
		public HttpMessageTypes Type
		{
			get
			{
				try
				{
					// requests are more common so we'll try that first
					HttpRequestLine requestLine = HttpRequestLine.Parse(this.FirstLine);
					return HttpMessageTypes.HttpRequest;
				}
				catch 
				{
					try
					{
						// followed closely by responses
						HttpStatusLine statusLine = HttpStatusLine.Parse(this.FirstLine);
						return HttpMessageTypes.HttpResponse;
					}
					catch 
					{
						// hmmm, it somehow was parsed into a message but we can't figure out what the hell it is
						// so we're going to say it's an unknown type
						return HttpMessageTypes.Unknown;
					}
				}
			}
		}
		
		/// <summary>
		/// Gets or sets the first line of the message
		/// </summary>
		internal virtual string FirstLine
		{
			get
			{
				return _firstLine;
			}
			set
			{
				// this is overkill i know, but we have to have the right format at this layer
				// and i can't be here to make sure everyone does it correctly, so make sure the first line is terminated correctly
				// according to rfc2616...
				value = HttpUtils.StripCRLF(value);
				value = HttpUtils.TrimLeadingAndTrailingSpaces(value);
				value = string.Format("{0}{1}", value, HttpControlChars.CRLF);
				_firstLine = value;
			}
		}

		/// <summary>
		/// Returns the list of HttpHeaders in the message
		/// </summary>
		public HttpHeaderList Headers
		{
			get
			{
				return _headers;
			}
		}
		
		/// <summary>
		/// Gets or set the byte array that contains the bytes forming the message body. Setting this property will also set the 'Content-Length' header value automatically
		/// </summary>
		public byte[] Body
		{
			get
			{
				return _body;
			}
			set
			{
				_body = value;

				// set the content length to the length of the body
				this.ContentLength = (_body == null ? 0 : _body.Length);				
			}
		}

		#region General Headers
		
		/// <summary>
		/// Gets or sets the General header 'Cache-Control'
		/// </summary>
		public string CacheControl
		{
			get
			{
				return this.ReadHeaderValue(HttpHeaders.GeneralHeaders.CacheControl);
			}
			set
			{
				this.WriteHeaderValue(HttpHeaders.GeneralHeaders.CacheControl, value, true);	
			}
		}

		/// <summary>
		/// Gets or sets the General header 'Connection'
		/// </summary>
		public string Connection
		{
			get
			{
				return this.ReadHeaderValue(HttpHeaders.GeneralHeaders.Connection);
			}
			set
			{
				this.WriteHeaderValue(HttpHeaders.GeneralHeaders.Connection, value, true);	
			}
		}
		
		/// <summary>
		/// Gets or sets the General header 'Date'
		/// </summary>
		public string Date
		{
			get
			{
				return this.ReadHeaderValue(HttpHeaders.GeneralHeaders.Date);
			}
			set
			{
				this.WriteHeaderValue(HttpHeaders.GeneralHeaders.Date, value, true);	
			}
		}

		/// <summary>
		/// Gets or sets the General header 'Pragma'
		/// </summary>
		public string Pragma
		{
			get
			{
				return this.ReadHeaderValue(HttpHeaders.GeneralHeaders.Pragma);
			}
			set
			{
				this.WriteHeaderValue(HttpHeaders.GeneralHeaders.Pragma, value, true);	
			}
		}



		/// <summary>
		/// Gets or sets the General header 'Trailer'
		/// </summary>
		public string Trailer
		{
			get
			{
				return this.ReadHeaderValue(HttpHeaders.GeneralHeaders.Trailer);
			}
			set
			{
				this.WriteHeaderValue(HttpHeaders.GeneralHeaders.Trailer, value, true);	
			}
		}

		/// <summary>
		/// Gets or sets the General header 'Transfer-Encoding'
		/// </summary>
		public string TransferEncoding
		{
			get
			{
				return this.ReadHeaderValue(HttpHeaders.GeneralHeaders.TransferEncoding);
			}
			set
			{
				this.WriteHeaderValue(HttpHeaders.GeneralHeaders.TransferEncoding, value, true);	
			}
		}

		/// <summary>
		/// Gets or sets a flag that controls whether the message is chunked or not
		/// </summary>
		public bool IsChunked
		{
			get
			{
				return HttpUtils.Contains(this.TransferEncoding, HttpTransferEncodings.Chunked);
			}
			set
			{
				this.TransferEncoding = (value ? HttpTransferEncodings.Chunked : string.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the General header 'Upgrade'
		/// </summary>
		public string Upgrade
		{
			get
			{
				return this.ReadHeaderValue(HttpHeaders.GeneralHeaders.Upgrade);
			}
			set
			{
				this.WriteHeaderValue(HttpHeaders.GeneralHeaders.Upgrade, value, true);	
			}
		}

		/// <summary>
		/// Gets or sets the General header 'Via'
		/// </summary>
		public string Via
		{
			get
			{
				return this.ReadHeaderValue(HttpHeaders.GeneralHeaders.Via);
			}
			set
			{
				this.WriteHeaderValue(HttpHeaders.GeneralHeaders.Via, value, true);	
			}
		}

		/// <summary>
		/// Gets or sets the General header 'Warning'
		/// </summary>
		public string Warning
		{
			get
			{
				return this.ReadHeaderValue(HttpHeaders.GeneralHeaders.Warning);
			}
			set
			{
				this.WriteHeaderValue(HttpHeaders.GeneralHeaders.Warning, value, true);	
			}
		}

		#endregion

		#region Entity Headers
		
		/// <summary>
		/// Gets or sets the Entity header 'Allow'
		/// </summary>
		public string Allow
		{
			get
			{
				return this.ReadHeaderValue(HttpHeaders.EntityHeaders.Allow);
			}
			set
			{
				this.WriteHeaderValue(HttpHeaders.EntityHeaders.Allow, value, true);
			}
		}

		/// <summary>
		/// Gets or sets the Entity header 'Content-Encoding'
		/// </summary>
		public string ContentEncoding
		{
			get
			{
				return this.ReadHeaderValue(HttpHeaders.EntityHeaders.ContentEncoding);
			}
			set
			{
				this.WriteHeaderValue(HttpHeaders.EntityHeaders.ContentEncoding, value, true);
			}
		}

		/// <summary>
		/// Gets or sets the Entity header 'Content-Language'
		/// </summary>
		public string ContentLanguage
		{
			get
			{
				return this.ReadHeaderValue(HttpHeaders.EntityHeaders.ContentLanguage);
			}
			set
			{
				this.WriteHeaderValue(HttpHeaders.EntityHeaders.ContentLanguage, value, true);
			}
		}

		/// <summary>
		/// Gets or sets the Entity header 'Content-Length'
		/// </summary>
		public int ContentLength
		{
			get
			{		
				string value = string.Empty;		
				try
				{					
					value = this.ReadHeaderValue(HttpHeaders.EntityHeaders.ContentLength);
					return int.Parse(value);
				}
				catch (Exception ex)
				{					
					Debug.WriteLine(ex);
				}

				// fall back on zero if there is a failure
				return 0;
			}
			set
			{				
				this.WriteHeaderValue(HttpHeaders.EntityHeaders.ContentLength, value.ToString(), true);								
			}
		}
		
		/// <summary>
		/// Gets or sets the Entity header 'Content-Location'
		/// </summary>
		public string ContentLocation
		{
			get
			{
				return this.ReadHeaderValue(HttpHeaders.EntityHeaders.ContentLocation);
			}
			set
			{
				this.WriteHeaderValue(HttpHeaders.EntityHeaders.ContentLocation, value, true);
			}
		}

		/// <summary>
		/// Gets or sets the Entity header 'Content-Range'
		/// </summary>
		public string ContentRange
		{
			get
			{
				return this.ReadHeaderValue(HttpHeaders.EntityHeaders.ContentRange);
			}
			set
			{
				this.WriteHeaderValue(HttpHeaders.EntityHeaders.ContentRange, value, true);
			}
		}

		/// <summary>
		/// Gets or sets the Entity header 'Content-Type'
		/// </summary>
		public string ContentType
		{
			get
			{
				return this.ReadHeaderValue(HttpHeaders.EntityHeaders.ContentType);
			}
			set
			{
				this.WriteHeaderValue(HttpHeaders.EntityHeaders.ContentType, value, true);
			}
		}

		/// <summary>
		/// Gets or sets the Entity header 'Expires'
		/// </summary>
		public string Expires
		{
			get
			{
				return this.ReadHeaderValue(HttpHeaders.EntityHeaders.Expires);
			}
			set
			{
				this.WriteHeaderValue(HttpHeaders.EntityHeaders.Expires, value, true);
			}
		}

		/// <summary>
		/// Gets or sets the Entity header 'Last-Modified'
		/// </summary>
		public string LastModified
		{
			get
			{
				return this.ReadHeaderValue(HttpHeaders.EntityHeaders.LastModified);
			}
			set
			{
				this.WriteHeaderValue(HttpHeaders.EntityHeaders.LastModified, value, true);
			}
		}

		#endregion

		#endregion

		#region My Public Methods

		/// <summary>
		/// Reads the value of the specified header, returns string.empty if the header does not exist
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public virtual string ReadHeaderValue(string name)
		{
			if (!_headers.Contains(name))
				return string.Empty;				

			// retrieve the header
			return _headers[name].Value;			
		}

		/// <summary>
		/// Write the value to the specified header, optionally deletes the header if the value is empty or null
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <param name="deleteIfEmptyOrNull"></param>
		public virtual void WriteHeaderValue(string name, string value, bool deleteIfNullOrEmpty)
		{
			if (deleteIfNullOrEmpty)
				if (HttpUtils.IsEmptryString(value) || HttpUtils.IsNullString(value))
				{
					_headers.Remove(name);
					return;
				}

			// validate the header value, not empty, null, or contains CR or LF
			HttpUtils.ValidateToken(name, value); 

			// otherwise if it doesn't exist
			if (!_headers.Contains(name))
			{
				// add it in with the value specified
				_headers.Add(new HttpHeader(name, value));
				return;
			}
				
			// or finally just update the one that exists
			_headers[name].Value = value;			
		}
		
		/// <summary>
		/// Sets the body of the message from a string
		/// </summary>
		/// <param name="text"></param>
		/// <param name="contentType"></param>
		public virtual void SetBodyFromString(string text, Encoding encoding, string contentType)
		{																									
			// set the body to the bytes of the stream
			this.Body = encoding.GetBytes(text);
			
			// set the content-type 
			this.ContentType = contentType;			
		}

		/// <summary>
		/// Sets the body of the message from a string builder
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="contentType"></param>
		public virtual void SetBodyFromStringBuilder(StringBuilder builder, Encoding encoding, string contentType)
		{
			// set the body to the bytes of the stream
			this.Body = encoding.GetBytes(builder.ToString());
			
			// set the content-type 
			this.ContentType = contentType;
		}

		/// <summary>
		/// Sets the body of the message from an image
		/// </summary>
		/// <param name="image"></param>
		/// <param name="format"></param>
		/// <param name="contentType"></param>
		public virtual void SetBodyFromImage(Image image, System.Drawing.Imaging.ImageFormat format, string contentType)
		{
			// create a stream by which we will save the image
			using (MemoryStream stream = new MemoryStream())
			{
				// save the image in the format specified
				image.Save(stream, format);

				// set the body of the message to the bytes created by the image
				this.Body = stream.GetBuffer();
				
				// set the content-type to whatever they specified
				this.ContentType = contentType;

				// close the stream
				stream.Close();
			}
		}

		/// <summary>
		/// Sets the body of the message from an xml node
		/// </summary>
		/// <param name="node"></param>
		public virtual void SetBodyFromXmlNode(XmlNode node, Encoding encoding)
		{
			// set the body to the bytes of the stream
			this.Body = encoding.GetBytes(node.OuterXml);
			
			// set the content-type
			this.ContentType = MIME.Text.Xml;			
		}
		
		/// <summary>
		/// Sets the body of the message from a memory stream
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="contentType"></param>
		public virtual void SetBodyFromMemoryStream(MemoryStream stream, string contentType)
		{
			// set the body
			this.Body = stream.GetBuffer();

			// set the content-type
			this.ContentType = contentType;
		}

		/// <summary>
		/// Returns the body of the message as a string
		/// </summary>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public virtual string GetBodyAsString(Encoding encoding)
		{
			return encoding.GetString(this.Body);
		}

		/// <summary>
		/// Returns the body of the message as a string builder
		/// </summary>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public virtual StringBuilder GetBodyAsStringBuilder(Encoding encoding)
		{
			return new StringBuilder(this.GetBodyAsString(encoding));
		}

		/// <summary>
		/// Returns the body of the message as a memory string
		/// </summary>
		/// <returns></returns>
		public virtual MemoryStream GetBodyAsMemoryStream()
		{
			return new MemoryStream(this.Body);
		}

		/// <summary>
		/// Returns the body of the message as an image
		/// </summary>
		/// <returns></returns>
		public virtual Image GetBodyAsImage()
		{
			Image image = null;
			using(MemoryStream stream = this.GetBodyAsMemoryStream())
			{
				image = Image.FromStream(stream);
				stream.Close();
			}	
			return image;
		}

		/// <summary>
		/// Returns the body of the message as an xml node
		/// </summary>
		/// <returns></returns>
		public virtual XmlNode GetBodyAsXmlNode()
		{
			XmlDocument document = new XmlDocument();
			using(MemoryStream stream = this.GetBodyAsMemoryStream())
			{
				document.Load(stream);
				stream.Close();
			}
			return document;
		}
		
		/// <summary>
		/// Returns the message as an array of bytes
		/// </summary>
		/// <returns></returns>
		public virtual byte[] ToByteArray()
		{
			return this.ToByteArray(true /* include headers */, true /* include body */);
		}
        
		/// <summary>
		/// Returns the message as an array of bytes
		/// </summary>
		/// <param name="includeHeaders">A flag to indicate whether the headers will be included</param>
		/// <param name="includeBody">A flag to indicate whether the body will be included</param>
		/// <returns></returns>
		public virtual byte[] ToByteArray(bool includeHeaders, bool includeBody)
		{			
			byte[] buffer = new byte[] {};

			// headers
			if (includeHeaders)
			{
				byte[] firstLine = HttpUtils.Encoding.GetBytes(this.FirstLine);
				byte[] headers = HttpUtils.Encoding.GetBytes(this.Headers.ToString());

				buffer = HttpUtils.Combine(firstLine, headers);
			}

			// body
			if (includeBody)
			{
				buffer = HttpUtils.Combine(buffer, this.Body);
			}

			return buffer;
		}

		/// <summary>
		/// Returns the message as an array of bytes
		/// </summary>
		/// <param name="includeBody">A flag to indicate whether the message body should be included in the return result</param>
		/// <returns></returns>
//		public virtual byte[] ToByteArray(bool includeHeaders, bool includeBody)
//		{			
//			byte[] buffer = null;
//
//			// create a stream
//			using (MemoryStream stream = new MemoryStream())
//			{
//				// create a writer
//				using (BinaryWriter writer = new BinaryWriter(stream, HttpUtils.Encoding))
//				{
//					// write the first line
////					writer.Write(this.ToString(includeBody));					
//					
//					// write the headers
//					if (includeHeaders)
//					{
//						writer.Write(this.FirstLine);
//
//						writer.Write(this.Headers.ToString());
//					}
//
//					// write the body
//					if (includeBody)
//					{
//						if (this.Body != null)
//							writer.Write(this.Body); // write the bytes directly, do not do transform
//					}
//																				
//					// retrieve the buffer from the stream
//					buffer = stream.GetBuffer();
//
//					// close the writer and the underlying stream
//					writer.Close();
//				}
//			}
//
//			Debug.WriteLine(HttpUtils.Encoding.GetString(buffer));
//
//			return buffer;
//		}

		/// <summary>
		/// Returns a string representation of the message
		/// </summary>
		/// <param name="includeBody"></param>
		/// <returns></returns>
		public virtual string ToString(bool includeBody)
		{			
			StringBuilder sb = new StringBuilder();						

			// append the first line
			sb.Append(this.FirstLine);

			// append the headers
			sb.Append(_headers.ToString());			
			
			// if they wanted the body with this
			if (includeBody)
				// so what with the body? 
				if (_body != null)
					// there is one, get it as a string using the current encoding?
					sb.Append(HttpUtils.Encoding.GetString(_body));

			return sb.ToString();
		}

		#endregion

		#region My Overrides

		/// <summary>
		/// Returns a string representation of the HttpMessage according to RFC2616 formatting.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.ToString(true /* include the body */);
		}

		#endregion

		#region My Static Methods

		/// <summary>
		/// Returns the type of this message
		/// </summary>
		public static HttpMessageTypes TypeOf(string firstLine)
		{					
			try
			{
				// requests are more common so we'll try that first
				HttpRequestLine requestLine = HttpRequestLine.Parse(firstLine);
				return HttpMessageTypes.HttpRequest;
			}
			catch 
			{
				try
				{
					// followed closely by responses
					HttpStatusLine statusLine = HttpStatusLine.Parse(firstLine);
					return HttpMessageTypes.HttpResponse;
				}
				catch 
				{
					// hmmm, it somehow was parsed into a message but we can't figure out what the hell it is
					// so we're going to say it's an unknown type
					return HttpMessageTypes.Unknown;
				}
			}			
		}

		#endregion
	}
}
