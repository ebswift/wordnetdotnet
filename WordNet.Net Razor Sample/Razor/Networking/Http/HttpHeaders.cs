using System;

namespace Razor.Networking.Http
{
	/// <summary>
	/// Summary description for HttpHeaders.
	/// </summary>
	public class HttpHeaders
	{
		/// <summary>
		/// Headers that apply to request messages
		/// </summary>
		public class RequestHeaders
		{
			public const string Accept				= @"Accept";
			public const string AcceptEncoding		= @"Accept-Encoding";
			public const string AcceptLanguage		= @"Accept-Language";
			public const string Authorization		= @"Authorization";
			public const string Expect				= @"Expect";
			public const string From				= @"From";
			public const string Host				= @"Host";
			public const string IfMatch				= @"If-Match";
			public const string IfModifiedSince		= @"If-Modified-Since";
			public const string IfNoneMatch			= @"If-None-Match";
			public const string IfRange				= @"If-Range";
			public const string IfUnmodifiedSince	= @"If-Unmodified-Since";
			public const string MaxForwards			= @"Max-Forwards";
			public const string ProxyAuthorization	= @"Proxy-Authorization";
			public const string Range				= @"Range";
			public const string Referrer			= @"Referrer";
			public const string TE					= @"TE";
			public const string UserAgent			= @"User-Agent";
		}

		/// <summary>
		/// Headers that apply to response messages
		/// </summary>
		public class ResponseHeaders
		{
			public const string AcceptRanges		= @"AcceptRanges";
			public const string Age					= @"Age";
			public const string ETag				= @"ETag";
			public const string Location			= @"Location";
			public const string ProxyAuthenticate	= @"ProxyAuthenticate";
			public const string RetryAfter			= @"RetryAfter";
			public const string Server				= @"Server";
			public const string Vary				= @"Vary";
			public const string WWWAuthenticate		= @"WWWAuthenticate";
		}

		/// <summary>
		/// Headers that apply to messages in general
		/// </summary>
		public class GeneralHeaders
		{
			public const string CacheControl		= @"Cache-Control";
			public const string Connection			= @"Connection";
			public const string Date				= @"Date";
			public const string Pragma				= @"Pragma";
			public const string Trailer				= @"Trailer";
			public const string TransferEncoding	= @"Transfer-Encoding";
			public const string Upgrade				= @"Upgrade";
			public const string Via					= @"Via";
			public const string Warning				= @"Warning";
			public const string ResponseNeeded		= @"Response-Needed";
		}

		/// <summary>
		/// Headers that apply to message entities (body)
		/// </summary>
		public class EntityHeaders
		{
			public const string Allow				= @"Allow";
			public const string ContentEncoding		= @"Content-Encoding";
			public const string ContentLanguage		= @"Content-Language";
			public const string ContentLength		= @"Content-Length";
			public const string ContentLocation		= @"Content-Location";
			public const string ContentRange		= @"Content-Range";
			public const string ContentType			= @"Content-Type";
			public const string Expires				= @"Expires";
			public const string LastModified		= @"Last-Modified";
		}
	}
}
