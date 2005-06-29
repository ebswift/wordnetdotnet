using System;

namespace Razor.Networking
{
	/// <summary>
	/// Summary description for AddressResolutionEventArgs.
	/// </summary>
	public class AddressResolutionEventArgs
	{
		protected string _address;
		protected int _port;
		protected object _stateObject;

		public AddressResolutionEventArgs(string address, int port)
		{
			_address = address;
			_port = port;
		}

		public AddressResolutionEventArgs(string address, int port, object stateObject)
		{
			_address = address;
			_port = port;
			_stateObject = stateObject;
		}

		/// <summary>
		/// The address being resolved (one of the following, IPv4 dotted quad, IPv6 colon separated hex, or Dns name)
		/// </summary>
		public string Address
		{
			get
			{
				return _address;
			}
		}
		
		/// <summary>
		/// The port number used to during resolution
		/// </summary>
		public int Port
		{
			get
			{
				return _port;
			}
		}

		/// <summary>
		/// Returns a user defined state object that can be used to determine the context of the event
		/// </summary>
		public object StateObject
		{
			get
			{
				return _stateObject;
			}
		}
	}

	public delegate void AddressResolutionEventHandler(object sender, AddressResolutionEventArgs e);
}
