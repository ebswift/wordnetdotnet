using System;

namespace Razor.Networking
{
	/// <summary>
	/// Summary description for ConnectionClosedByPeerException.
	/// </summary>
	public class ConnectionClosedByPeerException : Exception
	{
		public ConnectionClosedByPeerException() : base(@"The connection was closed by the remote host.")
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
