using System;

namespace Razor.Networking.Http
{
	/// <summary>
	/// Defines the protocol version used by the Razor Server
	/// </summary>
	public class RazorServerProtocolVersion : HttpProtocolVersion
	{
		private const string SERVER_PROTOCOL = "RazorServer";
		private const string SERVER_VERSION = "1.0";

		public RazorServerProtocolVersion() : base(SERVER_PROTOCOL, SERVER_VERSION)
		{
			
		}
	}

	/// <summary>
	/// Defines the protocol version used by the Razor Client
	/// </summary>
	public class RazorClientProtocolVersion : HttpProtocolVersion 
	{
		private const string CLIENT_PROTOCOL = "RazorServer";
		private const string CLIENT_VERSION = "1.0";

		public RazorClientProtocolVersion() : base(CLIENT_PROTOCOL, CLIENT_VERSION)
		{
				
		}
	}
}
