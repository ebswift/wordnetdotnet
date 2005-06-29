using System;

namespace Razor.Networking.Http
{
	/// <summary>
	/// Encapsulates a definition of a protocol and version 
	/// </summary>
	[Serializable()]
	public class HttpProtocolVersion
	{		
		protected string _protocol;
		protected string _version;

		public const string VERSION_1_0 = "HTTP/1.0";
		public const string VERSION_1_1 = "HTTP/1.1";

		/// <summary>
		/// Returns a string in the format 'Protocol/Version'
		/// </summary>
		public const string STRING_FORMAT = "{0}/{1}";

		/// <summary>
		/// Intitializes a new instance of the HttpProtocolVersion class
		/// </summary>
		public HttpProtocolVersion() : this(VERSION_1_1)
		{
			
		}

		/// <summary>
		/// Intitializes a new instance of the HttpProtocolVersion class
		/// </summary>
		/// <param name="httpVersion">The version string to be represented</param>
		public HttpProtocolVersion(string value)
		{			
			value = HttpUtils.StripCRLF(value);
			value = HttpUtils.TrimLeadingAndTrailingSpaces(value);			
			string[] parts = value.Split('/');
			this.Protocol = parts[0];
			this.Version = parts[1];
		}

		/// <summary>
		/// Initializes a new instance of the HttpProtocolVersion class
		/// </summary>
		/// <param name="protocol">The protocol in use</param>
		/// <param name="version">The version of the protocol</param>
		public HttpProtocolVersion(string protocol, string version)
		{
			this.Protocol = protocol;
			this.Version = version;
		}

		/// <summary>
		/// Returns the protocol in use
		/// </summary>
		public string Protocol
		{
			get
			{
				return _protocol;
			}
			set
			{
				HttpUtils.ValidateToken(@"Protocol", value);
				value = HttpUtils.TrimLeadingAndTrailingSpaces(value);
				_protocol = value;
			}
		}

		/// <summary>
		/// Returns the version of the protocol
		/// </summary>
		public string Version
		{
			get
			{
				return _version;
			}
			set
			{
				HttpUtils.ValidateToken(@"Version", value);
				value = HttpUtils.TrimLeadingAndTrailingSpaces(value);
				_version = value;
			}
		}

		/// <summary>
		/// Returns a string in the format 'Protocol/Version'
		/// </summary>
		public override string ToString()
		{
			return string.Format(STRING_FORMAT, _protocol, _version);
		}

		/// <summary>
		/// Parses a string in the foramt 'Protocol/Version' into an HttpProtocolVersion instance
		/// </summary>
		/// <param name="value">The string to parse. May contain CRLF.</param>
		/// <returns></returns>
		public static HttpProtocolVersion Parse(string value)
		{
			value = HttpUtils.StripCRLF(value);
			value = HttpUtils.TrimLeadingAndTrailingSpaces(value);			
			string[] parts = value.Split('/');            			
			return new HttpProtocolVersion(parts[0], parts[1]);
		}		
	}
}
