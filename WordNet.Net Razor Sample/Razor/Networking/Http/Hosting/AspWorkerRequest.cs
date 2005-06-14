using System;
using System.Diagnostics;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Web;
using System.Web.Hosting;
using Razor.Networking.Http;

namespace Razor.Networking.Http.Hosting
{
	/// <summary>
	/// Defines an Asp Worker Request object to allow the HttpRuntime to process our HttpRequests using the Asp runtime
	/// </summary>
	internal class AspWorkerRequest : SimpleWorkerRequest 
	{
		protected AspHost _aspHost;
		protected HttpConnection _connection;
		protected HttpRequest _request;
		protected HttpResponse _response;
		private string _filePath;
		private string _pathInfo;
		private string _filePathTranslated;
//		private bool _headersSent;

		/// <summary>
		/// Initializes a new instance of the AspWorkerRequest class
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="request"></param>
		public AspWorkerRequest(AspHost aspHost, HttpConnection connection, HttpRequest request) : base(string.Empty, string.Empty, null)
		{						
			_aspHost = aspHost;
			_connection = connection;
			_request = request;
			_response = new HttpResponse(new OkStatus());

			this.ParsePathInfo();
		}

		/// <summary>
		/// Returns the connection that received this request
		/// </summary>
		internal protected HttpConnection Connection
		{
			get
			{
				return _connection;
			}
		}

		/// <summary>
		/// Returns the request that was processed by the connection
		/// </summary>
		internal protected HttpRequest Request
		{
			get
			{
				return _request;
			}
		}

		/// <summary>
		/// Returns the response that will be sent for this request
		/// </summary>
		internal protected HttpResponse Response
		{
			get
			{
				return _response;
			}
		}

		private void ParsePathInfo()
		{
			string value = _request.RequestUriWithoutQueryString;
			int lastDot = value.LastIndexOf('.');
			int lastSlash = value.LastIndexOf('/');

			if (lastDot >= 0 && lastSlash >= 0 && lastDot < lastSlash)
			{
				int sep = value.IndexOf('/', lastDot);
				_filePath = value.Substring(0, sep);
				_pathInfo = value.Substring(sep);
			}
			else
			{
				_filePath = value;
				_pathInfo = string.Empty;
			}

			_filePathTranslated = this.MapPath(_filePath);
		}

		/// <summary>
		/// Returns the virtual directory to the currently executing server application
		/// </summary>
		/// <returns></returns>
		public override string GetAppPath()
		{
			string value = _aspHost.VirtualDirectory;
			return value;
		}

		/// <summary>
		/// Returns the UNC-translated path to the currently executing server application.
		/// </summary>
		/// <returns></returns>
		public override string GetAppPathTranslated()
		{
			string value = _aspHost.PhysicalDirectory;
			return value;
		}

//		public override string GetAppPoolID()
//		{
//			string value = base.GetAppPoolID ();
//			return value;
//		}
//
//		public override long GetBytesRead()
//		{
//			long value = base.GetBytesRead ();
//			return value;
//		}
//
//		public override byte[] GetClientCertificate()
//		{
//			byte[] value = base.GetClientCertificate ();
//			return value;
//		}
//
//		public override byte[] GetClientCertificateBinaryIssuer()
//		{
//			byte[] value = base.GetClientCertificateBinaryIssuer ();
//			return value;
//		}
//
//		public override int GetClientCertificateEncoding()
//		{
//			int value = base.GetClientCertificateEncoding ();
//			return value;
//		}
//
//		public override byte[] GetClientCertificatePublicKey()
//		{
//			byte[] value = base.GetClientCertificatePublicKey ();
//			return value;
//		}
//
//		public override DateTime GetClientCertificateValidFrom()
//		{
//			DateTime value = base.GetClientCertificateValidFrom ();
//			return value;
//		}
//
//		public override DateTime GetClientCertificateValidUntil()
//		{
//			DateTime value = base.GetClientCertificateValidUntil ();
//			return value;
//		}
//
//		public override long GetConnectionID()
//		{
//			long value = base.GetConnectionID ();
//			return value;
//		}

//		public override void FlushResponse(bool finalFlush)
//		{
//			base.FlushResponse(finalFlush);
//		}

		/// <summary>
		/// TODO:
		/// </summary>
		/// <returns></returns>
		public override string GetFilePath()
		{
			return _filePath;
		}

		/// <summary>
		/// TODO:
		/// </summary>
		/// <returns></returns>
		public override string GetFilePathTranslated()
		{
			return _filePathTranslated;
		}

		public override string GetHttpVerbName()
		{
			return _request.Method;
		}

		public override string GetHttpVersion()
		{
			return _request.ProtocolVersion.Version;
		}

		/// <summary>
		/// Returns the value of a known http header (Requests only)
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public override string GetKnownRequestHeader(int index)
		{
			HttpHeader header = _response.Headers[HttpWorkerRequest.GetKnownRequestHeaderName(index)];
			if (header != null)
				return header.Value;

			return base.GetKnownRequestHeader (index);
		}		

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string GetLocalAddress()
		{
			string localAddress = _connection.LocalAddress.ToString();
			return localAddress;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override int GetLocalPort()
		{
			int localPort = _connection.LocalPort;
			return localPort;
		}

		/// <summary>
		/// Returns the path information for the file (I honestly have no idea what the hell this is...)
		/// </summary>
		/// <returns></returns>
		public override string GetPathInfo()
		{
			return _pathInfo;
		}

		/// <summary>
		/// Returns the body of the request
		/// </summary>
		/// <returns></returns>
		public override byte[] GetPreloadedEntityBody()
		{
			return _request.Body;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string GetProtocol()
		{
			string protocol = _request.ProtocolVersion.Protocol;
			return protocol;
		}

		/// <summary>
		/// Returns the query string specified in the request URI
		/// </summary>
		/// <returns></returns>
		public override string GetQueryString()
		{
			string value = _request.QueryString;
			return value;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override byte[] GetQueryStringRawBytes()
		{			
			byte[] bytes = HttpUtils.Encoding.GetBytes(_request.QueryString);
			return bytes;
		}

		/// <summary>
		/// Returns the URI path contained in the request header with the query string appended.
		/// </summary>
		/// <returns></returns>
		public override string GetRawUrl()
		{
			string value = _request.RequestUri;
			return value;
		}

		/// <summary>
		/// Returns the IP address of the client
		/// </summary>
		/// <returns></returns>
		public override string GetRemoteAddress()
		{
			string remoteAddress = _connection.RemoteAddress.ToString();
			return remoteAddress;
		}

//		public override string GetRemoteName()
//		{
//			return base.GetRemoteName ();
//		}

		/// <summary>
		/// Returns the client's port number
		/// </summary>
		/// <returns></returns>
		public override int GetRemotePort()
		{
			int remotePort = _connection.RemotePort;
			return remotePort;
		}


//		public override int GetRequestReason()
//		{
//			int value = _response.Status.Reason;
//			return value;
//		}

//		public override string GetServerName()
//		{
//			return _response.Server;
//		}

		public override string GetServerVariable(string name)
		{
//			Debug.WriteLine("Get Server Variable : " + name);

			return base.GetServerVariable (name);
		}

		public override string GetUnknownRequestHeader(string name)
		{
			HttpHeader header = _request.Headers[name];
			if (header != null)
				return header.Value;

			return base.GetUnknownRequestHeader (name);
		}

		public override string[][] GetUnknownRequestHeaders()
		{
			string[][] unknownHeaders = _request.Headers.GetUnknownHeaders();
			return unknownHeaders;
		}

		/// <summary>
		/// Returns the virtual path to the requested URI
		/// </summary>
		/// <returns></returns>
		public override string GetUriPath()
		{			
			// something like '/default.html'
			string value = _request.RequestUriWithoutQueryString;
			return value;
		}

//		public override long GetUrlContextID()
//		{
//			return base.GetUrlContextID ();
//		}
//
//		public override System.IntPtr GetUserToken()
//		{
//			return base.GetUserToken ();
//		}
//
//		public override System.IntPtr GetVirtualPathToken()
//		{
//			return base.GetVirtualPathToken ();
//		}

		public override bool HeadersSent()
		{
			return false;
		}

		public override bool IsClientConnected()
		{
			return _connection.IsAlive;
		}
		
		public override bool IsEntireEntityBodyIsPreloaded()
		{
			if (_request.Body != null)
				return (_request.Body.Length > 0);
			
			return false;
		}

//		public override bool IsSecure()
//		{
//			return base.IsSecure ();
//		}

//		public override string MachineConfigPath
//		{
//			get
//			{
//				string value = base.MachineConfigPath;
//				return value;
//			}
//		}
//
//		public override string MachineInstallDirectory
//		{
//			get
//			{
//				string value = base.MachineInstallDirectory;
//				return value;
//			}
//		}

		/// <summary>
		/// Maps the virtual path to a physical path
		/// </summary>
		/// <param name="virtualPath"></param>
		/// <returns></returns>
		public override string MapPath(string virtualPath)
		{
			String mappedPath = String.Empty;

			if (virtualPath == null || virtualPath.Length == 0 || virtualPath.Equals("/")) 
			{
				// asking for the site root
				if (_aspHost.VirtualDirectory == "/") 
				{
					// app at the site root
					mappedPath = _aspHost.PhysicalDirectory;
				}
				else 
				{
					// unknown site root - don't point to app root to avoid double config inclusion
					mappedPath = Environment.SystemDirectory;
				}
			}
			else if (_aspHost.IsVirtualPathAppPath(virtualPath)) 
			{
				// application virtualPath
				mappedPath = _aspHost.PhysicalDirectory;
			}
			else if (_aspHost.IsVirtualPathInApp(virtualPath)) 
			{
				// inside app but not the app virtualPath itself
				mappedPath = _aspHost.PhysicalDirectory + virtualPath.Substring(_aspHost.NormalizedVirtualPath.Length);
			}
			else 
			{
				// outside of app -- make relative to app virtualPath
				if (virtualPath.StartsWith("/"))
					mappedPath = _aspHost.PhysicalDirectory + virtualPath.Substring(1);
				else
					mappedPath = _aspHost.PhysicalDirectory + virtualPath;
			}

			mappedPath = mappedPath.Replace('/', '\\');

			if (mappedPath.EndsWith("\\") && !mappedPath.EndsWith(":\\"))
				mappedPath = mappedPath.Substring(0, mappedPath.Length - 1);

			return mappedPath;
		}

		/// <summary>
		/// Reads request data from the client (When not preloaded)
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		public override int ReadEntityBody(byte[] buffer, int size)
		{
			int bytesRead = 0;
			byte[] bytes = HttpUtils.ReceiveBytes(_connection.Socket, size, HttpMessageReader.MAX_BUFFER_LENGTH);

			if (bytes != null)
			{
				if (bytes.Length > 0)
				{
					bytesRead = bytes.Length;
					Buffer.BlockCopy(bytes, 0, buffer, 0, bytesRead);
				}
			}

			return bytesRead;
		}
 
		/// <summary>
		/// Adds a content length to the response
		/// </summary>
		/// <param name="contentLength"></param>
		public override void SendCalculatedContentLength(int contentLength)
		{
			_response.ContentLength = contentLength;
		}

		/// <summary>
		/// Sets known response header values
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		public override void SendKnownResponseHeader(int index, string value)
		{
			switch(index)
			{
					#region Response Headers

				case HttpWorkerRequest.HeaderAcceptRanges:
					_response.AcceptRanges = value;
					break;
				case HttpWorkerRequest.HeaderAge:
					_response.Age = value;
					break;
				case HttpWorkerRequest.HeaderEtag:
					_response.ETag = value;
					break;
				case HttpWorkerRequest.HeaderLocation:
					_response.Location = value;
					break;
				case HttpWorkerRequest.HeaderProxyAuthenticate:
					_response.ProxyAuthenticate = value;
					break;
				case HttpWorkerRequest.HeaderRetryAfter:
					_response.RetryAfter = value;
					break;
				case HttpWorkerRequest.HeaderServer:
					_response.Server = value;
					break;
				case HttpWorkerRequest.HeaderVary:
					_response.Vary = value;
					break;
				case HttpWorkerRequest.HeaderWwwAuthenticate:
					_response.WWWAuthenticate = value;
					break;

					#endregion

					#region General Headers

				case HttpWorkerRequest.HeaderCacheControl:
					_response.CacheControl = value;
					break;
				case HttpWorkerRequest.HeaderConnection:
					_response.Connection = value;
					break;
				case HttpWorkerRequest.HeaderDate:
					_response.Date = value;
					break;
				case HttpWorkerRequest.HeaderPragma:
					_response.Pragma = value;
					break;
				case HttpWorkerRequest.HeaderTrailer:
					_response.Trailer = value;
					break;
				case HttpWorkerRequest.HeaderTransferEncoding:
					_response.TransferEncoding = value;
					break;
				case HttpWorkerRequest.HeaderUpgrade:
					_response.Upgrade = value;
					break;
				case HttpWorkerRequest.HeaderVia:
					_response.Via = value;
					break;
				case HttpWorkerRequest.HeaderWarning:
					_response.Warning = value;
					break;

					#endregion

					#region Entity Headers

				case HttpWorkerRequest.HeaderAllow:
					_response.Allow = value;
					break;
				case HttpWorkerRequest.HeaderContentEncoding:
					_response.ContentEncoding = value;
					break;
				case HttpWorkerRequest.HeaderContentLanguage:
					_response.ContentLanguage = value;
					break;
				case HttpWorkerRequest.HeaderContentLength:
					_response.ContentLength = int.Parse(value);
					break;
				case HttpWorkerRequest.HeaderContentRange:
					_response.ContentRange = value;
					break;
				case HttpWorkerRequest.HeaderContentType:
					_response.ContentType = value;
					break;
				case HttpWorkerRequest.HeaderExpires:
					_response.Expires = value;
					break;
				case HttpWorkerRequest.HeaderLastModified:
					_response.LastModified = value;
					break;

					#endregion
			};

//			base.SendKnownResponseHeader(index, value);
		}
 
		/// <summary>
		/// Sends the contents of the file to the response body
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="offset"></param>
		/// <param name="length"></param>
		public override void SendResponseFromFile(string filename, long offset, long length)
		{
			if (length == 0)
				return;

			FileStream f = null;

			try 
			{
				f = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
				SendResponseFromFileStream(f, offset, length);
			}
			finally 
			{
				if (f != null)
					f.Close();
			}
		}

		/// <summary>
		/// Sends the contents of the file to the response body
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="offset"></param>
		/// <param name="length"></param>
		public override void SendResponseFromFile(System.IntPtr handle, long offset, long length)
		{
			if (length == 0)
				return;

			FileStream f = null;

			try 
			{
				f = new FileStream(handle, FileAccess.Read, false);
				SendResponseFromFileStream(f, offset, length);
			}
			finally 
			{
				if (f != null)
					f.Close();
			}
		}

		/// <summary>
		/// Copies the data in the buffer to the body of the response
		/// </summary>
		/// <param name="data"></param>
		/// <param name="length"></param>
		public override void SendResponseFromMemory(byte[] data, int length)
		{
			if (length > 0)
			{
				byte[] bytes = new byte[length];
				Buffer.BlockCopy(data, 0, bytes, 0, length);
				if (_response.Body == null)
					_response.Body = bytes;
				else
                    _response.Body = HttpUtils.Combine(_response.Body, bytes);
			}
		}

//		public override void SendResponseFromMemory(System.IntPtr data, int length)
//		{
//			base.SendResponseFromMemory (data, length);
//		}

		/// <summary>
		/// Sets the status code and reason in the response
		/// </summary>
		/// <param name="statusCode"></param>
		/// <param name="statusDescription"></param>
		public override void SendStatus(int statusCode, string statusDescription)
		{
			// whatever the asp runtime things is a response, we shall reply with
			_response.Status.Code = statusCode;
			_response.Status.Reason = statusDescription;
		}

		/// <summary>
		/// Sets any unknown headers in the response
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public override void SendUnknownResponseHeader(string name, string value)
		{
			if (!_response.Headers.Contains(name))
				_response.Headers.Add(new HttpHeader(name, value));

			_response.Headers[name].Value = value;
		}

//		/// <summary>
//		/// Registers for an optional notification once all of the response data is sent
//		/// </summary>
//		/// <param name="callback"></param>
//		/// <param name="extraData"></param>
//		public override void SetEndOfSendNotification(System.Web.HttpWorkerRequest.EndOfSendNotification callback, object extraData)
//		{
//			base.SetEndOfSendNotification (callback, extraData);
//		}	
	
		#region My Protected Methods

		protected virtual void SendResponseFromFileStream(FileStream f, long offset, long length)  
		{
			const int maxChunkLength = 64*1024;
			long fileSize = f.Length;

			if (length == -1)
				length = fileSize - offset;

			if (length == 0 || offset < 0 || length > fileSize - offset)
				return;

			if (offset > 0)
				f.Seek(offset, SeekOrigin.Begin);

			if (length <= maxChunkLength) 
			{
				byte[] fileBytes = new byte[(int)length];
				int bytesRead = f.Read(fileBytes, 0, (int)length);
				this.SendResponseFromMemory(fileBytes, bytesRead);
			}
			else 
			{
				byte[] chunk = new byte[maxChunkLength];
				int bytesRemaining = (int)length;

				while (bytesRemaining > 0) 
				{
					int bytesToRead = (bytesRemaining < maxChunkLength) ? bytesRemaining : maxChunkLength;
					int bytesRead = f.Read(chunk, 0, bytesToRead);
					this.SendResponseFromMemory(chunk, bytesRead);
					bytesRemaining -= bytesRead;

					// flush to release keep memory
					//					if (bytesRemaining > 0 && bytesRead > 0)
					//						FlushResponse(false);
				}
			}
		}

		#endregion
	}
}
