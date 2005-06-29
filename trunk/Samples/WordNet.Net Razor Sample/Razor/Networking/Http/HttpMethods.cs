using System;

namespace Razor.Networking.Http
{
	/// <summary>
	/// Defines the known Http methods as defined in RFC 2616. Http methods are case-sensitive.
	/// </summary>
	public class HttpMethods
	{
		public const string Options		= @"OPTIONS";
		public const string Get			= @"GET";
		public const string Head		= @"HEAD";
		public const string Post		= @"POST";
		public const string Put			= @"PUT";
		public const string Delete		= @"DELETE";
		public const string Trace		= @"TRACE";
	}
}
